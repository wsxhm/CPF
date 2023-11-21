using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CPF.Controls;
using CPF.Drawing;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Android
{
    public class WindowImpl : PopupImpl, IWindowImpl
    {
        public WindowImpl()
        {
            WindowType = WindowManagerTypes.DrawnApplication;
            WindowFlags = WindowManagerFlags.LayoutNoLimits;
        }

        public Func<bool> Closing { get; set; }
        public Action Closed { get; set; }
        WindowState windowState;
        public WindowState WindowState
        {
            get => windowState; set
            {
                if (this.LayoutParameters != null && Parent != null)
                {
                    //if (windowState == WindowState.FullScreen)
                    //{
                    //    //(Context as Activity).RequestWindowFeature(WindowFeatures.DefaultFeatures);
                    //    (Context as Activity).Window.SetFlags(WindowManagerFlags.ForceNotFullscreen, WindowManagerFlags.ForceNotFullscreen);
                    //}
                    if (value == WindowState.Maximized || value == WindowState.FullScreen)
                    {
                        var size = Screen.WorkingArea.Size;
                        var main = (Context as CpfActivity).Main;
                        if (main != null)
                        {
                            size = new Size(main.Width, main.Height);
                        }
                        UpdateLayout(0, 0, (int)size.Width, (int)size.Height);
                    }
                    else if (value == WindowState.Normal)
                    {
                        if (normalRect.HasValue)
                        {
                            //Left = (int)normalRect.Value.Left;
                            //Top = (int)normalRect.Value.Top;
                            //LayoutParameters.Width = (int)normalRect.Value.Width;
                            //LayoutParameters.Height = (int)normalRect.Value.Height;
                            UpdateLayout((int)normalRect.Value.Left, (int)normalRect.Value.Top, (int)normalRect.Value.Width, (int)normalRect.Value.Height);
                        }
                    }
                    //else if (value == WindowState.FullScreen)
                    //{
                    //    //(Context as Activity).RequestWindowFeature(WindowFeatures.NoTitle);//设置无标题
                    //    (Context as Activity).Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
                    //    UpdateLayout(0, 0, (Context as Activity).Window.DecorView.Width, (Context as Activity).Window.DecorView.Height);
                    //}
                }
                this.Visibility = value == WindowState.Minimized ? ViewStates.Gone : ViewStates.Visible;
                windowState = value;
            }
        }

        protected override void OnVisibilityChanged(global::Android.Views.View changedView, [GeneratedEnum] ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);
            windowState = visibility == ViewStates.Gone ? WindowState.Minimized : WindowState.Normal;
            WindowStateChanged();
        }

        Rect? normalRect;
        public override void SetVisible(bool visible)
        {
            //if ((windowState == WindowState.Maximized || windowState == WindowState.FullScreen) && visible)
            //{
            //    Root.LayoutManager.ExecuteLayoutPass();
            //    var margin = new WindowManagerLayoutParams(WindowType, WindowFlags, global::Android.Graphics.Format.Rgbx8888);
            //    margin.Gravity = GravityFlags.Left | GravityFlags.Top;

            //    CpfActivity.CurrentActivity.WindowManager.AddView(this, margin);
            //}
            //else
            //{
            base.SetVisible(visible);
            if (visible)
            {
                normalRect = new Rect(this.Left, this.Top, this.Width, this.Height);
            }
            //}
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            if (Width != (Context as Activity).Window.DecorView.Width && Height != (Context as Activity).Window.DecorView.Height && Location.X != 0 && Location.Y != 0)
            {
                normalRect = new Rect(Location.X, Location.Y, Width, Height);
            }
        }

        protected override void OnMove()
        {
            if (Width != (Context as Activity).Window.DecorView.Width && Height != (Context as Activity).Window.DecorView.Height && Location.X != 0 && Location.Y != 0)
            {
                normalRect = new Rect(Location.X, Location.Y, Width, Height);
            }
        }

        public Action WindowStateChanged { get; set; }
        public bool IsMain { get; set; }

        public void Close()
        {
            if (Closing())
            {
                return;
            }
            base.SetVisible(false);
            Closed();
        }

        public void SetFullscreen(bool fullscreen)
        {
            //if (fullscreen)
            //{
            //    (Context as Activity).RequestWindowFeature(WindowFeatures.NoTitle);//设置无标题
            //    (Context as Activity).Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);//设置全屏
            //}
            //else
            //{
            //    (Context as Activity).RequestWindowFeature(WindowFeatures.DefaultFeatures);
            //    (Context as Activity).Window.SetFlags(WindowManagerFlags.ForceNotFullscreen, WindowManagerFlags.ForceNotFullscreen);
            //}
        }

        public void SetIcon(Image image)
        {

        }

        public void SetTitle(string text)
        {

        }

        public void ShowDialog(Controls.Window window)
        {
            //SetVisible(true);
            Root.Visibility = CPF.Visibility.Visible;
        }

        public void ShowInTaskbar(bool value)
        {

        }

        public void TopMost(bool value)
        {

        }

        public void SetEnable(bool enable)
        {

        }
    }
}