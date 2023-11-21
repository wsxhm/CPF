using System;
using System.Collections.Generic;
using System.Text;
using CPF;
using System.Threading;
using CPF.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using CPF.Controls;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

namespace CPF.Platform
{
    public class Application
    {
        static Application()
        {
            Threading.Dispatcher.mainId = Thread.CurrentThread.ManagedThreadId;
        }
        /// <summary>
        /// 程序启动之后持续时间
        /// </summary>
        public static TimeSpan Elapsed
        {
            get { return _clock.Elapsed; }
        }
        private static Stopwatch _clock = Stopwatch.StartNew();
        public static event EventHandler ApplicationExit;
        static DrawingFactory _getDrawingFactory;
        static RuntimePlatform _getRuntimePlatform;
        //static ValueTuple<OperatingSystemType, RuntimePlatform, DrawingFactory>[] data;
        /// <summary>
        /// 初始化运行环境
        /// </summary>
        /// <param name="runtimes"></param>
        public static void Initialize(params ValueTuple<OperatingSystemType, RuntimePlatform, DrawingFactory>[] runtimes)
        {
            Threading.Dispatcher.mainId = Thread.CurrentThread.ManagedThreadId;
            CPF.Threading.Dispatcher.mainThread = null;
#if Net4
            OperatingSystem = OperatingSystemType.Windows;
            foreach (var item in runtimes)
            {
                if (item.Item1 == OperatingSystemType.Windows)
                {
                    _getDrawingFactory = item.Item3;
                    _getRuntimePlatform = item.Item2;
                    break;
                }
            }
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OperatingSystem = OperatingSystemType.Windows;
                foreach (var item in runtimes)
                {
                    if (item.Item1 == OperatingSystemType.Windows)
                    {
                        _getDrawingFactory = item.Item3;
                        _getRuntimePlatform = item.Item2;
                        break;
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                OperatingSystem = OperatingSystemType.OSX;
                foreach (var item in runtimes)
                {
                    if (item.Item1 == OperatingSystemType.OSX)
                    {
                        _getDrawingFactory = item.Item3;
                        _getRuntimePlatform = item.Item2;
                        break;
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                OperatingSystem = OperatingSystemType.Linux;
                foreach (var item in runtimes)
                {
                    if (item.Item1 == OperatingSystemType.Linux || item.Item1 == OperatingSystemType.Android)
                    {
                        if (item.Item1 == OperatingSystemType.Android && Assembly.Load("Mono.Android") != null)
                        {
                            OperatingSystem = OperatingSystemType.Android;
                        }
                        _getDrawingFactory = item.Item3;
                        _getRuntimePlatform = item.Item2;
                        break;
                    }
                }
            }
#endif
            if (_getDrawingFactory == null || _getRuntimePlatform == null)
            {
                throw new Exception("未找到该平台下的OperatingSystemType, RuntimePlatform");
            }
            SynchronizationContext.SetSynchronizationContext(_getRuntimePlatform.GetSynchronizationContext());

        }
        /// <summary>
        /// 执行消息循环
        /// </summary>
        public static void Run(IApp app)
        {
            _app = app;
            _app.IsMain = true;
            _app.Closed += App_Closed;
            app.Show();
            _getRuntimePlatform.Run();
        }

        public static void RunLoop(CancellationToken cancellation)
        {
            _getRuntimePlatform.Run(cancellation);
        }

        static List<(string, string)> loadFonts = new List<(string, string)>();
        /// <summary>
        /// 可以支持css定义加载的字体，就可以不需要调用该方法。支持内嵌资源路径res://或者本地路径file:///或者在线路径http://
        /// </summary>
        /// <param name="path"></param>        
        /// <param name="fontFamily">不设置的话，用图形引擎解析出来的名字，不同图形引擎加载同一个字体可能会有不同的名字，可以自己定义个确定的名字来避免不同名称加载不到字体的问题。</param>
        public static async void LoadFont(string path, string fontFamily = null)
        {
            if (string.IsNullOrWhiteSpace(path) || loadFonts.Any(a => a.Item1 == path && a.Item2 == fontFamily))
            {
                return;
            }
            loadFonts.Add((path, fontFamily));
            var stream = await Styling.ResourceManager.GetStream(path);
            if (stream == null)
            {
                Console.WriteLine("字体加载失败：" + path);
                Debug.WriteLine("字体加载失败：" + path);
                return;
            }
            GetDrawingFactory().LoadFont(stream, fontFamily);
            stream.Dispose();
        }

        /// <summary>
        /// 当前操作系统类型
        /// </summary>
        public static OperatingSystemType OperatingSystem { get; private set; }

        /// <summary>
        /// 获取图形工厂
        /// </summary>
        /// <returns></returns>
        public static DrawingFactory GetDrawingFactory()
        {
            return _getDrawingFactory;
        }

        public static RuntimePlatform GetRuntimePlatform()
        {
            return _getRuntimePlatform;
        }
        static IApp _app;
        /// <summary>
        /// 主窗体
        /// </summary>
        public static IApp Main
        {
            get { return _app; }
        }

        private static void App_Closed(object sender, EventArgs e)
        {
            foreach (var item in Window.Windows.ToList())
            {
                item.Dispose();
            }
            //Binding.runGC = false;
            _getDrawingFactory.Dispose();
            _app = null;
            if (ApplicationExit != null)
            {
                ApplicationExit(null, EventArgs.Empty);
            }
        }
        /// <summary>
        /// 关闭主窗体，导致程序退出
        /// </summary>
        public static void Exit()
        {
            _app?.Close();
        }

        /// <summary>
        /// 程序启动目录
        /// </summary>
        public static string StartupPath
        {
            get { return System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'); }
        }
        /// <summary>
        /// 当前运行时是否是设计模式
        /// </summary>
        public static bool DesignMode
        {
            get;
            private set;
        }
        /// <summary>
        /// 基础缩放值，默认是1，用于在原有的DPI缩放上再加个缩放比例，只能在程序初始化的时候设置。
        /// </summary>
        public static float BaseScale { get; set; } = 1;

        /// <summary>
        /// 是否允许开发者工具读取元素
        /// </summary>
        public static bool AllowDeveloperTool
        {
            get;
            set;
        } = true;
        /// <summary>
        /// 禁止弹窗关闭，用于调试期间调试弹窗样式
        /// </summary>
        public static bool DisablePopupClose
        {
            get;
            set;
        }
//#if NETSTANDARD
//        /// <summary>
//        /// 启用AOT功能，net5以及以上版本才能使用，其中包括，禁用弱引用事件功能，一般在Aot的release发布的时候设置。如果不用Aot，请勿设置，否则可能有内存泄露风险，而且会导致性能降低。 以及切换windows的com包装
//        /// </summary>
//        public static void EnableAOT()
//        {
//            CPF.WeakDelegate.DisableWeak = true;
//            IsEnableAOT = true;
//        }
//        public static bool IsEnableAOT
//        {
//            get; private set;
//        }
//#endif
    }
}
