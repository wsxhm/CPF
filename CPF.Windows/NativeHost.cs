using CPF.Controls;
using CPF.Drawing;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CPF.Windows.UnmanagedMethods;

namespace CPF.Windows
{
    public class NativeHost : NativeWindow, INativeImpl
    {
        public NativeHost()
        {
            CreateHandle(new CreateParams
            {
                ExStyle = (int)(WindowStyles.WS_EX_LEFT | WindowStyles.WS_EX_LTRREADING),
                Style = (int)(WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_POPUP),
            });
        }

        public void Dispose()
        {
            if (hbrush != IntPtr.Zero)
            {
                DeleteObject(hbrush);
                hbrush = IntPtr.Zero;
            }
            DestroyHandle();
        }
        IntPtr hbrush;
        //int color;
        public void SetBackColor(Color color)
        {
            if (hbrush != IntPtr.Zero)
            {
                DeleteObject(hbrush);
            }
            var bc = (int)color.R | ((int)color.G << 8) | ((int)color.B << 16);
            hbrush = CreateSolidBrush(bc);
            SetClassLong(Handle, ClassLongIndex.GCLP_HBRBACKGROUND, hbrush);
            //GetWindowRect(Handle, out var lp);
            //RECT rect = new RECT(0, 0, lp.right, lp.bottom);
            //InvalidateRect(Handle, ref rect, false);
            //UpdateWindow(Handle);
        }

        Rect renderRect;
        Rect? clipRect;
        public void SetBounds(Rect boundsRect, Rect clip, bool visible)
        {
            if (visible)
            {
                this.renderRect = boundsRect;
                SetWindowPos(Handle, parent.IsTopMost ? WindowPosZOrder.HWND_TOPMOST : WindowPosZOrder.HWND_NOTOPMOST, (int)(boundsRect.Left * parent.RenderScaling + parent.Position.X), (int)(boundsRect.Top * parent.RenderScaling + parent.Position.Y), (int)(boundsRect.Width * parent.RenderScaling), (int)(boundsRect.Height * parent.RenderScaling), SetWindowPosFlags.SWP_NOACTIVATE);

                if (!float.IsInfinity(clip.Width) && !float.IsInfinity(clip.Height) && clipRect != clip)
                {
                    clipRect = clip;
                    var rgn = CreateRectRgn((int)(clip.X * parent.RenderScaling), (int)(clip.Y * parent.RenderScaling), (int)(clip.Right * parent.RenderScaling), (int)(clip.Bottom * parent.RenderScaling));
                    SetWindowRgn(Handle, rgn, true);
                    DeleteObject(rgn);
                }
                ShowWindow(Handle, ShowWindowCommand.ShowNoActivate);
            }
            else
            {
                ShowWindow(Handle, ShowWindowCommand.Hide);
            }
        }


        IntPtr contentHandle;
        public void SetContent(object content)
        {
            if (content != null)
            {
                if (content is IntPtr intPtr)
                {
                    contentHandle = intPtr;
                    if (contentHandle != IntPtr.Zero)
                    {
                        UnmanagedMethods.SetParent(intPtr, Handle);
                    }
                }
                else
                {
                    throw new Exception("必须是控件句柄IntPtr类型");
                }
            }
            else
            {
                if (contentHandle != IntPtr.Zero)
                {
                    UnmanagedMethods.SetParent(contentHandle, IntPtr.Zero);
                }
                contentHandle = IntPtr.Zero;
            }
        }
        WindowImpl parent;
        public void SetParent(IViewImpl parent)
        {
            if (parent is WindowImpl window)
            {
                window.Move += Window_Move;
                if (window.Root is Window win)
                {
                    win.PropertyChanged += Win_PropertyChanged;
                }
                SetWindowLongPtr(Handle, (int)WindowLongParam.GWL_HWNDPARENT, window.Handle);
            }
            else
            {
                this.parent.Move -= Window_Move;
                if (this.parent.Root is Window win)
                {
                    win.PropertyChanged -= Win_PropertyChanged;
                }
                SetWindowLongPtr(Handle, (int)WindowLongParam.GWL_HWNDPARENT, IntPtr.Zero);
            }
            this.parent = parent as WindowImpl;
        }

        private void Win_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Window.TopMost))
            {
                IntPtr hWndInsertAfter = parent.IsTopMost ? WindowPosZOrder.HWND_TOPMOST : WindowPosZOrder.HWND_NOTOPMOST;
                UnmanagedMethods.SetWindowPos(Handle, hWndInsertAfter, 0, 0, 0, 0,  SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
            }
        }

        private void Window_Move()
        {
            SetWindowPos(Handle, parent.IsTopMost ? WindowPosZOrder.HWND_TOPMOST : WindowPosZOrder.HWND_NOTOPMOST, (int)(renderRect.Left * parent.RenderScaling + parent.Position.X), (int)(renderRect.Top * parent.RenderScaling + parent.Position.Y), (int)(renderRect.Width * parent.RenderScaling), (int)(renderRect.Height * parent.RenderScaling), SetWindowPosFlags.SWP_NOACTIVATE);
        }

        protected override void WndProc(ref Message m)
        {
            switch ((WindowsMessage)m.Msg)
            {
                case WindowsMessage.WM_NCUAHDRAWCAPTION:
                case WindowsMessage.WM_NCUAHDRAWFRAME:
                    m.Result = IntPtr.Zero;
                    return;
                case WindowsMessage.WM_ERASEBKGND:
                case WindowsMessage.WM_NCPAINT:
                    //var _hdc = GetDC(hwnd);
                    //OnPaint(_hdc, new Rect(new Point(), Size));
                    //ReleaseDC(hwnd, _hdc);
                    m.Result = (IntPtr)1;
                    break;
                //case WindowsMessage.WM_PAINT:
                //    PAINTSTRUCT ps = new PAINTSTRUCT();
                //    IntPtr hdc = BeginPaint(m.HWnd, out ps);
                //    //SetBkColor(hdc, (int)255 | ((int)0 << 8) | ((int)0 << 16));
                //    FillRect(hdc, ref ps.rcPaint, hbrush);
                //    //OnPaint(hdc, new Rect(ps.rcPaint.left, ps.rcPaint.top, ps.rcPaint.Width, ps.rcPaint.Height));
                //    EndPaint(m.HWnd, ref ps);
                //    break;
                default:
                    break;
            }
            base.WndProc(ref m);
        }
        UIElement owner;

        object INativeImpl.Handle => Handle;

        public void SetOwner(NativeElement owner)
        {
            this.owner = owner;
        }
    }
}
