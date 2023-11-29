using CPF.Controls;
using CPF.Drawing;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Design
{
    public class DesignWindow : IWindowImpl
    {
        public Func<bool> Closing { get; set; }
        public Action Closed { get; set; }
        public WindowState WindowState { get; set; }
        public Action WindowStateChanged { get; set; }
        public bool IsMain { get; set; }

        public Screen Screen => new Screen(new Rect(0, 0, 1920, 1080), new Rect(0, 0, 1920, 1080), true);

        float scaling = 1;
        public float Scaling
        {
            get { return scaling; }
            set
            {
                scaling = value;
                ScalingChanged?.Invoke();
            }
        }

        public float RenderScaling => scaling;

        public float LayoutScaling => scaling;

        public Action ScalingChanged { get; set; }
        public Action<Size> Resized { get; set; }
        public Action<PixelPoint> PositionChanged { get; set; }
        public Action Activated { get; set; }
        public Action Deactivated { get; set; }
        public bool CanActivate { get; set; }
        public PixelPoint Position { get; set; }

        public void Activate()
        {

        }

        public void Capture()
        {

        }

        public void Close()
        {
            bool? preventClosing = Closing?.Invoke();
            if (preventClosing == true)
            {
                return;
            }

            Closed?.Invoke();
            if (IsMain)
            {
                (Application.GetRuntimePlatform() as DesignPlatform).Exit();
            }
        }

        bool isDisposed;
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                Close();
            }
        }

        bool paint = false;
        Rect invalidateRect;
        public void Invalidate(in Rect rect)
        {
            Rect all = new Rect(new Point(), root.ActualSize);//控件区域
            if ((all.Contains(rect) || rect.IntersectsWith(all) || all == rect || rect.Contains(all)) && !rect.IsEmpty && !all.IsEmpty)
            {
                //Debug.WriteLine(invalidateRect);
                if (invalidateRect.IsEmpty || rect.Contains(invalidateRect) || rect == invalidateRect)//如果更新区域大于原有失效区域，则当前失效区域设为更新区域
                {
                    invalidateRect = rect;
                }
                else if (invalidateRect.Contains(rect))//如果更新区域小于原来的失效区域，则失效区域不变
                {

                }
                else if (invalidateRect.IsEmpty)//如果原来的失效区域为空
                {
                    invalidateRect = rect;
                }
                else
                {//如果两个区域没有关联或者相交
                    var minX = invalidateRect.X < rect.X ? invalidateRect.X : rect.X;//确定包含这两个矩形的最小矩形
                    var minY = invalidateRect.Y < rect.Y ? invalidateRect.Y : rect.Y;
                    var maxW = (invalidateRect.Width + invalidateRect.X - minX) > (rect.Width + rect.X - minX) ? (invalidateRect.Width + invalidateRect.X - minX) : (rect.Width + rect.X - minX);
                    var maxH = (invalidateRect.Height + invalidateRect.Y - minY) > (rect.Height + rect.Y - minY) ? (invalidateRect.Height + invalidateRect.Y - minY) : (rect.Height + rect.Y - minY);
                    Rect min = new Rect(minX, minY, maxW, maxH);

                    invalidateRect = min;
                }
                invalidateRect.Intersect(all);//最后失效区域为在控件区域里面的相交区域
            }
            if (!paint)
            {
                paint = true;
                CPF.Threading.Dispatcher.MainThread.BeginInvoke(a =>
               {
                   root.LayoutManager.ExecuteLayoutPass();
                   Rect rect1 = new Rect(((invalidateRect.X - 1) * scaling), ((invalidateRect.Y - 1) * scaling), ((invalidateRect.Width + 2) * scaling), ((invalidateRect.Height + 2) * scaling));

                   OnPaint(new Rect((invalidateRect.X - 1) * scaling, (invalidateRect.Y - 1) * scaling, (invalidateRect.Width + 2) * scaling, (invalidateRect.Height + 2) * scaling));

                   //System.Diagnostics.Debug.WriteLine(invalidateRect);
                   invalidateRect = new Rect();
                   paint = false;
               }, null);
            }
        }

        Size oldSize;
        void OnPaint(Rect rect)
        {
            if (isDisposed)
            {
                return;
            }
            var size = root.ActualSize * scaling;
            if (RenderBitmap == null || oldSize != size)
            {
                oldSize = size;
                if (RenderBitmap != null)
                {
                    RenderBitmap.Dispose();
                }
                RenderBitmap = new Bitmap((int)oldSize.Width, (int)oldSize.Height);
            }
            if (root.LayoutManager.VisibleUIElements != null)
            {
                using (DrawingContext dc = DrawingContext.FromBitmap(RenderBitmap))
                {
                    root.RenderView(dc, rect);
                }
            }
            if (OnRenderedBitmap != null)
            {
                OnRenderedBitmap();
            }
        }

        /// <summary>
        /// RenderToBitmap=true，图像渲染到这个位图里，界面将不显示
        /// </summary>
        public Bitmap RenderBitmap { get; private set; }
        public Action OnRenderedBitmap { get; set; }

        public Point PointToClient(Point point)
        {
            return point / scaling;
        }

        public Point PointToScreen(Point point)
        {
            throw new NotImplementedException("不能在设计模式下调用");
        }

        public void ReleaseCapture()
        {

        }

        public void SetCursor(Cursor cursor)
        {

        }

        public void SetEnable(bool enable)
        {

        }

        public void SetIcon(Image image)
        {

        }

        public void SetIMEEnable(bool enable)
        {

        }

        public void SetIMEPosition(Point point)
        {

        }

        View root;
        public void SetRoot(View view)
        {
            root = view;
        }

        public void SetTitle(string text)
        {

        }

        public void SetVisible(bool visible)
        {
            root.LayoutManager.ExecuteLayoutPass();
            root.Invalidate();
        }

        public void ShowDialog(Window window)
        {
            throw new NotImplementedException("不能在设计模式下调用");
        }

        public void ShowInTaskbar(bool value)
        {

        }

        public void TopMost(bool value)
        {

        }
    }

    class DesignPopupImpl : DesignWindow, IPopupImpl
    {

    }
}
