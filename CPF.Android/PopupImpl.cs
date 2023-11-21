using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CPF.Drawing;
using CPF.Platform;

namespace CPF.Android
{
    public class PopupImpl : CpfView, IPopupImpl
    {
        public PopupImpl() : base(CpfActivity.CurrentActivity, null)
        {
            WindowType = WindowManagerTypes.ApplicationSubPanel;
            WindowFlags = WindowManagerFlags.NotFocusable;
        }

        public Screen Screen => androidView.Screen;

        public float RenderScaling => androidView.RenderScaling;

        public float LayoutScaling => androidView.LayoutScaling;

        public Action ScalingChanged { get => androidView.ScalingChanged; set => androidView.ScalingChanged = value; }
        public Action<Size> Resized { get => androidView.Resized; set => androidView.Resized = value; }
        public Action<PixelPoint> PositionChanged { get => (androidView as IViewImpl).PositionChanged; set => (androidView as IViewImpl).PositionChanged = value; }
        public Action Deactivated { get => androidView.Deactivated; set => androidView.Deactivated = value; }
        public bool CanActivate { get => (androidView as IViewImpl).CanActivate; set => (androidView as IViewImpl).CanActivate = value; }
        public PixelPoint Position { get => (androidView as IViewImpl).Position; set => (androidView as IViewImpl).Position = value; }
        Action IViewImpl.Activated { get => (androidView as IViewImpl).Activated; set => (androidView as IViewImpl).Activated = value; }

        public void Activate()
        {
            (androidView as IViewImpl).Activate();
        }

        public void Capture()
        {
            (androidView as IViewImpl).Capture();
        }

        public void Invalidate(in Rect rect)
        {
            (androidView as IViewImpl).Invalidate(rect);
        }

        public Point PointToClient(Point point)
        {
            return (androidView as IViewImpl).PointToClient(point);
        }

        public Point PointToScreen(Point point)
        {
            return (androidView as IViewImpl).PointToScreen(point);
        }

        public void ReleaseCapture()
        {
            (androidView as IViewImpl).ReleaseCapture();
        }

        public void SetCursor(Cursor cursor)
        {
            (androidView as IViewImpl).SetCursor(cursor);
        }

        public void SetIMEEnable(bool enable)
        {
            (androidView as IViewImpl).SetIMEEnable(enable);
        }

        public void SetIMEPosition(Point point)
        {
            (androidView as IViewImpl).SetIMEPosition(point);
        }

        public void SetRoot(Controls.View view)
        {
            (androidView as IViewImpl).SetRoot(view);
        }
        bool isVisible;
        public virtual void SetVisible(bool visible)
        {
            if (visible)
            {
                Root.LayoutManager.ExecuteLayoutPass();
                if (!isVisible)
                {
                    var margin = new WindowManagerLayoutParams((int)(Root.ActualSize.Width * LayoutScaling), (int)(Root.ActualSize.Height * LayoutScaling), (int)(Root.ActualOffset.X * LayoutScaling), (int)(Root.ActualOffset.Y * LayoutScaling), WindowType, WindowFlags, global::Android.Graphics.Format.Rgbx8888);
                    margin.Gravity = GravityFlags.Left | GravityFlags.Top;
                    Location = new PixelPoint((int)(Root.ActualOffset.X * LayoutScaling), (int)(Root.ActualOffset.Y * LayoutScaling));
                    CpfActivity.CurrentActivity.WindowManager.AddView(this, margin);
                }
            }
            else
            {
                if (isVisible)
                {
                    CpfActivity.CurrentActivity.WindowManager.RemoveView(this);
                }
            }
            isVisible = visible;
        }
    }
}