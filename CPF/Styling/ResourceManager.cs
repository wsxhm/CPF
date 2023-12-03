using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Net;
using CPF.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CPF.Styling
{
    /// <summary>
    /// 内嵌资源管理，让资源可以在CSS里引用。所有加载过的图片都会被弱引用缓存
    /// </summary>
    public class ResourceManager
    {
        static ResourceManager()
        {
            //Register(Assembly.GetExecutingAssembly());
            //Register(Assembly.GetEntryAssembly());
            //Register(Assembly.GetCallingAssembly());
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                Register(item);
            }
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Register(args.LoadedAssembly);
        }

        static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
        /// <summary>
        /// 注册资源程序集，资源必须是内嵌的才能读取，程序集名称不能重复。一般情况下会自动注册，不需要手动调用
        /// </summary>
        /// <param name="assembly"></param>
        public static void Register(Assembly assembly)
        {
            if (assembly == null)
            {
                return;
            }
            if (assemblies.ContainsKey(assembly.FullName))
            {
                assemblies.Remove(assembly.FullName);
            }
            assemblies.Add(assembly.FullName, assembly);
        }
        /// <summary>
        /// 移除程序集
        /// </summary>
        public static void RemoveAssembly(string assemblyFullName)
        {
            if (assemblies.ContainsKey(assemblyFullName))
            {
                assemblies.Remove(assemblyFullName);
            }
        }

        static Image errorImage;
        /// <summary>
        /// 获取或设置加载图片失败后的错误图片
        /// </summary>
        public static Image ErrorImage
        {
            get
            {
                if (errorImage == null || errorImage.ImageImpl == null)
                {
                    GetImage("res://CPF/error.png", a => errorImage = a);
                }
                return errorImage;
            }
            set { errorImage = value; }
        }

        static ConcurrentDictionary<string, WeakReference<Image>> res = new ConcurrentDictionary<string, WeakReference<Image>>();
        static ConcurrentDictionary<string, ConcurrentBag<Action<Image>>> downloading = new ConcurrentDictionary<string, ConcurrentBag<Action<Image>>>();
        /// <summary>
        /// 清除缓存数据
        /// </summary>
        public static void ClearCache()
        {
            foreach (var item in res)
            {
                if (item.Value.TryGetTarget(out var img))
                {
                    img.Dispose();
                }
            }
            res.Clear();
            //downloading.Clear();
            texts.Clear();
        }
        /// <summary>
        /// 清除指定路径的缓存资源
        /// </summary>
        /// <param name="path"></param>
        public static void ClearCache(string path)
        {
            if (res.TryRemove(path, out var reference))
            {
                if (reference.TryGetTarget(out var img))
                {
                    img.Dispose();
                }
            }
            texts.Remove(path);
            downloading.TryRemove(path, out _);
        }

        /// <summary>
        /// 读取文件或者内嵌或者网络的图片，弱引用缓存
        /// </summary>
        /// <param name="path"></param>
        /// <param name="action"></param>
        public static void GetImage(string path, Action<Image> action)
        {
            Image img = null;
            if (!res.TryGetValue(path, out WeakReference<Image> image) || !image.TryGetTarget(out img))
            {
                res.TryRemove(path, out var v);
                var lower = path.ToLower();
                if (lower.StartsWith("http://") || lower.StartsWith("https://"))
                {
                    if (downloading.TryGetValue(path, out ConcurrentBag<Action<Image>> act))
                    {
                        act.Add(action);
                    }
                    else
                    {
                        downloading.TryAdd(path, new ConcurrentBag<Action<Image>> { action });
                        ThreadPool.QueueUserWorkItem(a =>
                        {
                            img = null;
                            try
                            {
                                var uri = new Uri(path);
                                //WebClient webClient = new WebClient();
                                //webClient.Headers.Add(HttpRequestHeader.Referer, uri.Scheme + "://" + uri.Host);
                                //var data = webClient.DownloadData(path);
                                var request = WebRequest.Create(path) as HttpWebRequest;
                                request.Method = "GET";
                                //request.Headers.Add(HttpRequestHeader.Referer, uri.Scheme + "://" + uri.Host);
                                request.Referer = uri.Scheme + "://" + uri.Host;
                                using (var response = request.GetResponse())
                                {
                                    using (Stream stream = response.GetResponseStream())
                                    {
                                        img = Image.FromStream(stream);
                                        res.TryAdd(path, new WeakReference<Image>(img));
                                        //action(img);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("下载图片失败：" + path + "   " + e.Message);
                                //action(null);
                            }
                            if (downloading.TryRemove(path, out ConcurrentBag<Action<Image>> list))
                            {
                                foreach (var item in list)
                                {
                                    item(img);
                                }
                            }
                        });
                    }
                    return;
                }
                else if (lower.StartsWith("res://"))
                {
                    try
                    {
                        var str = path.Substring(6);
                        str = str.TrimStart('\\', '/');
                        //str = str.Replace('/', '.');
                        //str = str.Replace('\\', '.');
                        var l = str.IndexOf('/');
                        if (!string.IsNullOrWhiteSpace(CPF.Design.DesignerLoadStyleAttribute.ProjectPath) && File.Exists(Path.Combine(CPF.Design.DesignerLoadStyleAttribute.ProjectPath, str.Substring(l + 1))))
                        {
                            var image1 = Image.FromFile(Path.Combine(CPF.Design.DesignerLoadStyleAttribute.ProjectPath, str.Substring(l + 1)));
                            res.TryAdd(path, new WeakReference<Image>(image1));
                            action(image1);
                        }
                        else
                        {
                            var name = str.Substring(0, l) + ",";
                            Image image1 = null;
                            foreach (var item in assemblies)
                            {
                                if (item.Value.FullName.StartsWith(name))
                                {
                                    Stream fs = item.Value.GetManifestResourceStream(str.Replace('/', '.').Replace('\\', '.'));
                                    if (fs != null)
                                    {
                                        var im = Image.FromStream(fs);
                                        res.TryAdd(path, new WeakReference<Image>(im));
                                        fs.Dispose();
                                        image1 = im;
                                    }
                                    break;
                                }
                            }
                            action(image1);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("读取图片失败：" + e.Message);
                        action(null);
                    }
                    return;
                }
                else if (path.Length > 20 && lower.StartsWith("data:image/"))
                {
                    var s = path.IndexOf(',');
                    Image i = null;
                    if (s > 0)
                    {
                        try
                        {
                            var b = Convert.FromBase64String(path.Substring(s + 1));
                            i = Image.FromBuffer(b);
                            res.TryAdd(path, new WeakReference<Image>(i));
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("解析图片出错：" + (path.Length > 100 ? path.Substring(0, 100) : path) + e);
                        }
                    }
                    action(i);
                    return;
                }
                else
                {
                    var s = path;
                    if (lower.StartsWith("file://"))
                    {
                        s = s.Substring(7).TrimStart('/');
                    }
                    try
                    {
                        Image i;
                        using (var im = Image.FromFile(s))
                        {
                            i = im.Clone() as Image;
                            res.TryAdd(path, new WeakReference<Image>(i));
                        }
                        action(i);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("加载图片失败：" + e.Message);
                        action(null);
                    }
                    return;
                }
            }
            action(img);
        }


        static Dictionary<string, string> texts = new Dictionary<string, string>();
        /// <summary>
        /// 读取文件或者内嵌或者网络的文本，弱引用缓存。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="action"></param>
        public static void GetText(string path, Action<string> action)
        {
            if (!texts.TryGetValue(path, out string text))
            {
                var lower = path.ToLower();
                if (lower.StartsWith("http://") || lower.StartsWith("https://"))
                {
                    ThreadPool.QueueUserWorkItem(a =>
                    {
                        try
                        {
                            //WebClient webClient = new WebClient();
                            //webClient.Headers.Add(HttpRequestHeader.Referer, new Uri(path).Host);
                            //text = webClient.DownloadString(path);
                            //texts.Add(path, text);

                            var uri = new Uri(path);
                            var request = WebRequest.Create(path);
                            request.Method = "GET";
                            request.Headers.Add(HttpRequestHeader.Referer, uri.Scheme + "://" + uri.Host);
                            using (var response = request.GetResponse())
                            {
                                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                                {
                                    text = stream.ReadToEnd();
                                    if (text != null)
                                    {
                                        text = text.TrimStart((char)65279);
                                    }
                                    texts.Add(path, text);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("加载网页失败：" + path + "   " + e.Message);
                        }
                        action(text);
                    }, null);
                    return;
                }
                else if (lower.StartsWith("res://"))
                {
                    try
                    {
                        var str = path.Substring(6);
                        str = str.TrimStart('\\', '/');
                        //str = str.Replace('/', '.');
                        //str = str.Replace('\\', '.');
                        var l = str.IndexOf('/');
                        if (!string.IsNullOrWhiteSpace(CPF.Design.DesignerLoadStyleAttribute.ProjectPath) && File.Exists(Path.Combine(CPF.Design.DesignerLoadStyleAttribute.ProjectPath, str.Substring(l + 1))))
                        {
                            text = File.ReadAllText(Path.Combine(CPF.Design.DesignerLoadStyleAttribute.ProjectPath, str.Substring(l + 1)));
                            if (text != null)
                            {
                                text = text.TrimStart((char)65279);
                            }
                        }
                        else
                        {
                            var name = str.Substring(0, l) + ",";
                            foreach (var item in assemblies)
                            {
                                if (item.Value.FullName.StartsWith(name))
                                {
                                    Stream fs = item.Value.GetManifestResourceStream(str.Replace('\\', '.').Replace('/', '.'));
                                    if (fs != null)
                                    {
                                        var data = new byte[fs.Length];
                                        fs.Read(data, 0, data.Length);
                                        text = Encoding.UTF8.GetString(data);
                                        if (text != null)
                                        {
                                            text = text.TrimStart((char)65279);
                                        }
                                        texts.Add(path, text);
                                        fs.Dispose();
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("读取文本失败：" + path + "   " + e.Message);
                    }
                    action(text);
                    return;
                }
                else
                {
                    var s = path;
                    if (lower.StartsWith("file://"))
                    {
                        s = s.Substring(7).TrimStart('/');
                    }
                    try
                    {
                        text = File.ReadAllText(s);
                        if (text != null)
                        {
                            text = text.TrimStart((char)65279);
                        }
                        texts.Add(path, text);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("加载文本失败：" + path + "   " + e.Message);
                    }
                    action(text);
                    return;
                }
            }
            action(text);
        }
        /// <summary>
        /// 读取文件或者内嵌或者网络的文本，弱引用缓存。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<string> GetText(string path)
        {
            //var task = Task.Factory.StartNew(() =>
            //{
            //var invokeMre = new ManualResetEvent(false);
            //string result = null;
            TaskCompletionSource<string> completionSource = new TaskCompletionSource<string>();
            GetText(path, a =>
            {
                completionSource.SetResult(a);
                //result = a;
                //invokeMre.Set();
            });
            //invokeMre.WaitOne();
            //return result;
            return completionSource.Task;
            //});
            //return task;
        }
        /// <summary>
        /// 读取文件或者内嵌或者网络的图片，弱引用缓存
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<Image> GetImage(string path)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var invokeMre = new ManualResetEvent(false);
                Image result = null;
                GetImage(path, a =>
                {
                    result = a;
                    invokeMre.Set();
                });
                invokeMre.WaitOne();
                return result;
            });
            return task;
        }
        /// <summary>
        /// 加载流数据，不缓存
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<Stream> GetStream(string path)
        {
            TaskCompletionSource<Stream> task = new TaskCompletionSource<Stream>();
            var lower = path.ToLower();
            if (lower.StartsWith("http://") || lower.StartsWith("https://"))
            {
                ThreadPool.QueueUserWorkItem(a =>
                {
                    try
                    {
                        //WebClient webClient = new WebClient();
                        //webClient.Headers.Add(HttpRequestHeader.Referer, new Uri(path).Host);
                        //text = webClient.DownloadString(path);
                        //texts.Add(path, text);

                        var uri = new Uri(path);
                        var request = WebRequest.Create(path);
                        request.Method = "GET";
                        request.Headers.Add(HttpRequestHeader.Referer, uri.Scheme + "://" + uri.Host);
                        using (var response = request.GetResponse())
                        {
                            task.SetResult(response.GetResponseStream());
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("加载网页失败：" + e.Message);
                        task.SetResult(null);
                    }
                }, null);
            }
            else if (lower.StartsWith("res://"))
            {
                try
                {
                    var str = path.Substring(6);
                    str = str.TrimStart('\\', '/');
                    //str = str.Replace('/', '.');
                    //str = str.Replace('\\', '.');
                    var l = str.IndexOf('/');
                    var name = str.Substring(0, l) + ",";
                    Stream fs = null;
                    foreach (var item in assemblies)
                    {
                        if (item.Value.FullName.StartsWith(name))
                        {
                            fs = item.Value.GetManifestResourceStream(str.Replace('/', '.').Replace('\\', '.'));
                            //if (fs != null)
                            //{
                            //    var data = new byte[fs.Length];
                            //    fs.Read(data, 0, data.Length);
                            //    text = Encoding.UTF8.GetString(data);
                            //    texts.Add(path, text);
                            //    fs.Dispose();
                            //}
                            break;
                        }
                    }
                    task.SetResult(fs);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("读取文本失败：" + e.Message);
                    task.SetResult(null);
                }
            }
            else
            {
                var s = path;
                if (lower.StartsWith("file://"))
                {
                    s = s.Substring(7).TrimStart('/');
                }
                try
                {
                    task.SetResult(File.OpenRead(s));
                }
                catch (Exception e)
                {
                    Debug.WriteLine("加载数据失败：" + e.Message);
                    task.SetResult(null);
                }
            }
            return task.Task;
        }
    }
}
