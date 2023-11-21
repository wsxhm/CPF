using System;
using System.Collections.Generic;
using System.Text;
using static CPF.Windows.UnmanagedMethods;
using System.Runtime.InteropServices;

namespace CPF.Windows
{
    public class NativeWindow// : IDisposable
    {

        WNDCLASSEX wndclassex = new WNDCLASSEX();
        private string _className;
        IntPtr handle;
        public IntPtr Handle
        {
            get { return handle; }
        }

        public NativeWindow()
        {

        }

        public virtual void CreateHandle(CreateParams cp)
        {
            _className = cp.ClassName;
            if (string.IsNullOrWhiteSpace(_className))
            {
                _className = "CPFWindow-" + Guid.NewGuid();
            }
            // 初始化窗口类结构  
            wndclassex.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            //wc.style = (int)ClassStyles.CS_DBLCLKS;
            wndclassex.lpfnWndProc = WndProc;
            wndclassex.hInstance = GetModuleHandle(null);
            wndclassex.hbrBackground = (IntPtr)6;
            wndclassex.lpszClassName = _className;
            wndclassex.cbClsExtra = 0;
            wndclassex.cbWndExtra = 0;
            wndclassex.hIcon = IntPtr.Zero;
            wndclassex.hCursor = WindowImpl.DefaultCursor;
            wndclassex.lpszMenuName = "";
            // 注册窗口类  
            RegisterClassEx(ref wndclassex);
            // 创建并显示窗口  
            handle = CreateWindowEx(cp.ExStyle, _className, cp.Caption, (uint)(cp.Style),
              cp.X, cp.Y, cp.Width, cp.Height, cp.Parent, IntPtr.Zero, GetModuleHandle(null), cp.Param);
        }

        //        public virtual void ReleaseHandle()
        //        {
        //            ReleaseHandle(true);
        //        }

        //        /// <devdoc>
        //        ///     Releases the handle associated with this window.  If handleValid
        //        ///     is true, this will unsubclass the window as well.  HandleValid
        //        ///     should be false if we are releasing in response to a 
        //        ///     WM_DESTROY.  Unsubclassing during this message can cause problems
        //        ///     with XP's theme manager and it's not needed anyway.
        //        /// </devdoc>
        //        private void ReleaseHandle(bool handleValid)
        //        {
        //            // Don't touch if the current window proc is not ours.
        //            //
        //            IntPtr currentWinPrc = UnsafeNativeMethods.GetWindowLong(new HandleRef(this, handle), NativeMethods.GWL_WNDPROC);
        //            if (windowProcPtr == currentWinPrc)
        //            {
        //                if (previousWindow == null)
        //                {

        //#if DEBUG
        //                    subclassStatus = "Unsubclassing back to native defWindowProc";
        //#endif

        //                    // If the defWindowProc points to a native window proc, previousWindow will
        //                    // be null.  In this case, it is completely safe to assign defWindowProc
        //                    // to the current wndproc.
        //                    //
        //                    UnsafeNativeMethods.SetWindowLong(href, NativeMethods.GWL_WNDPROC, new HandleRef(this, defWindowProc));
        //                }
        //                else
        //                {
        //                    if (finalizing)
        //                    {

        //#if DEBUG
        //                        subclassStatus = "Setting back to userDefWindowProc -- next chain is managed";
        //#endif

        //                        // Here, we are finalizing and defWindowProc is pointing to a managed object.  We must assume
        //                        // that the object defWindowProc is pointing to is also finalizing.  Why?  Because we're
        //                        // holding a ref to it, and it is holding a ref to us.  The only way this cycle will
        //                        // finalize is if no one else is hanging onto it.  So, we re-assign the window proc to
        //                        // userDefWindowProc.
        //                        UnsafeNativeMethods.SetWindowLong(href, NativeMethods.GWL_WNDPROC, new HandleRef(this, userDefWindowProc));
        //                    }
        //                    else
        //                    {

        //#if DEBUG
        //                        subclassStatus = "Setting back to next managed subclass object";
        //#endif

        //                        // Here we are not finalizing so we use the windowProc for our previous window.  This may
        //                        // DIFFER from the value we are currently storing in defWindowProc because someone may
        //                        // have re-subclassed.
        //                        UnsafeNativeMethods.SetWindowLong(href, NativeMethods.GWL_WNDPROC, previousWindow.windowProc);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // cutting the subclass chain anyway, even if we're not the last one in the chain
        //                // if the whole chain is all managed NativeWindow it doesnt matter, 
        //                // if the chain is not, then someone has been dirty and didn't clean up properly, too bad for them...

        //                //We will cut off the chain if we cannot unsubclass.
        //                //If we find previouswindow pointing to us, then we can let RemoveWindowFromTable reassign the
        //                //defwndproc pointers properly when this guy gets removed (thereby unsubclassing ourselves)

        //                if (nextWindow == null || nextWindow.defWindowProc != windowProcPtr)
        //                {
        //                    // we didn't find it... let's unhook anyway and cut the chain... this prevents crashes
        //                    UnsafeNativeMethods.SetWindowLong(href, NativeMethods.GWL_WNDPROC, new HandleRef(this, userDefWindowProc));
        //                }
        //#if DEBUG
        //                subclassStatus = "FORCE unsubclass -- we do not own the subclass";
        //#endif
        //            }
        //        }

        public virtual void DestroyHandle()
        {
            // 
            lock (this)
            {
                if (handle != IntPtr.Zero)
                {
                    if (!DestroyWindow(handle))
                    {
                        //UnSubclass();
                        ////then post a close and let it do whatever it needs to do on its own.
                        //UnsafeNativeMethods.PostMessage(new HandleRef(this, handle), NativeMethods.WM_CLOSE, 0, 0);
                    }
                    handle = IntPtr.Zero;
                }

                GC.SuppressFinalize(this);
            }
        }

        protected unsafe virtual IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            var m = new Message { HWnd = hwnd, Msg = (int)msg, LParam = lParam, WParam = wParam };
            WndProc(ref m);
            return m.Result;
        }

        protected virtual void WndProc(ref Message m)
        {
            DefWndProc(ref m);
        }

        public void DefWndProc(ref Message m)
        {
            // At this point, there isn't much we can do.  There's a
            // small chance the following line will allow the rest of
            // the program to run, but don't get your hopes up.
            m.Result = DefWindowProc(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
        }

        //#region IDisposable Support
        //private bool disposedValue = false; // 要检测冗余调用

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposedValue)
        //    {
        //        if (disposing)
        //        {
        //            // TODO: 释放托管状态(托管对象)。
        //        }

        //        // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
        //        // TODO: 将大型字段设置为 null。

        //        disposedValue = true;
        //    }
        //}

        //// TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        //~NativeWindow()
        //{
        //    // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //    Dispose(false);
        //}

        //// 添加此代码以正确实现可处置模式。
        //public void Dispose()
        //{
        //    // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //    Dispose(true);
        //    // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
        //    GC.SuppressFinalize(this);
        //}
        //#endregion
    }
}
