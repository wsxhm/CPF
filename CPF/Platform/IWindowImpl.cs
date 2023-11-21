using System;
using System.Collections.Generic;
using System.Text;
using CPF;
using CPF.Drawing;
using CPF.Controls;

namespace CPF.Platform
{
    public interface IWindowImpl : IViewImpl
    {
        Func<bool> Closing { get; set; }

        Action Closed { get; set; }

        //void Show();

        //void Hide();

        WindowState WindowState { get; set; }

        //void DragMove();

        void Close();

        Action WindowStateChanged { get; set; }

        //bool CanResize { get; set; }

        void SetTitle(string text);

        bool IsMain { get; set; }

        void ShowDialog(Window window);

        void ShowInTaskbar(bool value);

        void TopMost(bool value);

        void SetIcon(Image image);

        //void SetFullscreen(bool fullscreen);

        void SetEnable(bool enable);
    }
}
