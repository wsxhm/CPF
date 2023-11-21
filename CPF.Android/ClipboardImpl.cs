using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CPF.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CPF.Android
{
    class ClipboardImpl : CPF.Input.IClipboard
    {
        private Context context = CpfActivity.CurrentActivity;

        private ClipboardManager ClipboardManager
        {
            get
            {
                return this.context.GetSystemService(Context.ClipboardService).JavaCast<ClipboardManager>();
            }
        }

        public void Clear()
        {
            ClipboardManager.PrimaryClip = null;
        }

        public bool Contains(DataFormat dataFormat)
        {
            var pc = ClipboardManager.PrimaryClip;
            if (pc != null && ClipboardManager.HasPrimaryClip)
            {
                for (int i = 0; i < pc.ItemCount; i++)
                {
                    var data = pc.GetItemAt(i);
                    if (dataFormat == DataFormat.Text)
                    {
                        if (data.Text != null)
                        {
                            return true;
                        }
                    }
                    else if (dataFormat == DataFormat.Html)
                    {
                        if (data.HtmlText != null)
                        {
                            return true;
                        }
                    }
                    else if (dataFormat == DataFormat.FileNames)
                    {
                        if (data.Uri != null)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public object GetData(DataFormat dataFormat)
        {
            var pc = ClipboardManager.PrimaryClip;
            if (pc != null && ClipboardManager.HasPrimaryClip)
            {
                for (int i = 0; i < pc.ItemCount; i++)
                {
                    var data = pc.GetItemAt(i);
                    switch (dataFormat)
                    {
                        case DataFormat.Text:
                            return data.Text;
                        case DataFormat.Html:
                            return data.HtmlText;
                        case DataFormat.FileNames:
                            var file = data.Uri?.ToString();
                            if (file != null)
                            {
                                return new string[] { file };
                            }
                            break;
                    }

                }
            }
            return null;
        }

        public void SetData(params (DataFormat, object)[] data)
        {
            ClipData clipData = null;
            foreach (var item in data)
            {
                switch (item.Item1)
                {
                    case DataFormat.Text:
                        if (clipData == null)
                        {
                            clipData = ClipData.NewPlainText("text", item.Item2 as string);
                        }
                        else
                        {
                            clipData.AddItem(new ClipData.Item(item.Item2 as string));
                        }
                        break;
                    case DataFormat.Html:
                        var text = data.FirstOrDefault(a => a.Item1 == DataFormat.Text);
                        var str = text.Item1 != DataFormat.Unknown && text.Item2 != null ? text.Item2 as string : NoHTML(item.Item2 as string);

                        if (clipData == null)
                        {
                            clipData = ClipData.NewHtmlText("html", str, item.Item2 as string);
                        }
                        else
                        {
                            clipData.AddItem(new ClipData.Item(str, item.Item2 as string));
                        }
                        break;
                    case DataFormat.FileNames:
                        foreach (var file in item.Item2 as IEnumerable<string>)
                        {
                            if (clipData == null)
                            {
                                clipData = ClipData.NewRawUri("URI", global::Android.Net.Uri.Parse(file));
                            }
                            else
                            {
                                clipData.AddItem(new ClipData.Item(global::Android.Net.Uri.Parse(file)));
                            }
                        }
                        break;
                }
            }
        }

        ///   <summary>
        ///   去除HTML标记
        ///   </summary>
        ///   <param   name=”NoHTML”>包括HTML的源码   </param>
        ///   <returns>已经去除后的文字</returns>
        public static string NoHTML(string Htmlstring)
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "",
            RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"–>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!–.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = WebUtility.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }
    }
}