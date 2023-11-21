using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Platform
{
    public interface INotifyIconImpl : IDisposable
    {
        string Text { get; set; }

        Image Icon { get; set; }

        bool Visible { get; set; }

        event EventHandler Click;

        event EventHandler DoubleClick;

        event EventHandler<NotifyIconMouseEventArgs> MouseDown;

        event EventHandler<NotifyIconMouseEventArgs> MouseUp;
    }
}
