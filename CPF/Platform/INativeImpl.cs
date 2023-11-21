using CPF.Controls;
using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Platform
{
    /// <summary>
    /// 定义原生控件内嵌容器的接口
    /// </summary>
    public interface INativeImpl : IDisposable
    {
        void SetOwner(NativeElement owner);
        void SetParent(IViewImpl parent);

        void SetContent(object content);

        void SetBounds(Rect boundsRect, Rect clip, bool visible);

        void SetBackColor(Color color);

        object Handle { get; }
    }
}
