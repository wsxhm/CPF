using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Input
{
    public static class DragDrop
    {
        public static DragDropEffects DoDragDrop(DragDropEffects allowedEffects, params ValueTuple<DataFormat, object>[] data)
        {
            if (data == null || data.Length == 0)
            {
                throw new Exception("data不能为空");
            }
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
            var result = Platform.Application.GetRuntimePlatform().DoDragDrop(allowedEffects, data);
            foreach (var item in CPF.Controls.View.Views)
            {
                if (item.InputManager.MouseDevice.Captured != null)
                {
                    item.InputManager.MouseDevice.Captured.ReleaseMouseCapture();
                }
            }

            return result;
        }
    }


    [Flags]
    public enum DragDropEffects : byte
    {
        None = 0,
        Copy = 1,
        Move = 2,
        Link = 4,
    }
}
