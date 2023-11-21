using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Input
{
    /// <summary>
    /// 剪贴板
    /// </summary>
    public static class Clipboard
    {
        static IClipboard clipboard;

        static IClipboard _Clipboard
        {
            get
            {
                if (clipboard == null)
                {
                    clipboard = Platform.Application.GetRuntimePlatform().GetClipboard();
                }
                return clipboard;
            }
        }

        public static void Clear()
        {
            _Clipboard.Clear();
        }

        public static void SetData(params ValueTuple<DataFormat, object>[] data)
        {
            foreach (var item in data)
            {
                if (item.Item1 == DataFormat.Image && !(item.Item2 is Image))
                {
                    throw new Exception("DataFormat.Image必须是CPF.Drawing.Image类型");
                }
                else if (item.Item1 == DataFormat.FileNames && !(item.Item2 is IEnumerable<string>))
                {
                    throw new Exception("DataFormat.FileNames必须是IEnumerable<string>类型");
                }
            }
            _Clipboard.SetData(data);
        }

        public static bool Contains(DataFormat dataFormat)
        {
            return _Clipboard.Contains(dataFormat);
        }

        public static object GetData(DataFormat dataFormat)
        {
            return _Clipboard.GetData(dataFormat);
        }
    }
}
