using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using static CPF.Windows.UnmanagedMethods;
using CPF;
using System.Threading;
using System.IO;
using CPF.Threading;
using System.Collections.Concurrent;
using CPF.Drawing;
using CPF.Input;
using System.Linq;
using CPF.Controls;
using CPF.Platform;
using CPF.Shapes;
using CPF.Animation;
using System.Diagnostics;
using System.Text;
using CPF.Design;
using CPF.Windows.OpenGL;
using CPF.OpenGL;
//using System.Runtime.InteropServices.ComTypes;

namespace CPF.Windows
{
    public class WindowImpl : IWindowImpl
    {
        internal static int threadCallbackMessage;
        public ConcurrentQueue<SendOrPostData> invokeQueue = new ConcurrentQueue<SendOrPostData>();
        public ConcurrentQueue<SendOrPostData> asyncQueue = new ConcurrentQueue<SendOrPostData>();
        float scaling = 1;
        internal static WindowImpl Window;
        const double wheelDelta = 120.0;
        private bool _trackingMouse;
        DispatcherTimer timer;
        View root;
        public View Root
        {
            get { return root; }
        }
        WindowState windowState = WindowState.Normal;
        WindowState oldwindowState = WindowState.Normal;
        //bool isFirst = true;
        private string _className;

        static WindowImpl()
        {
            threadCallbackMessage = RegisterWindowMessage("CPF_ThreadCallbackMessage");
            if (WindowsPlatform.registerForMarshalling)
            {
#if NET5_0_OR_GREATER
                ComWrappers.RegisterForMarshalling(ComWrapper.CpfComWrappers.Instance);
#endif
            }
            HRESULT res = UnmanagedMethods.OleInitialize(IntPtr.Zero);
            if (res != HRESULT.S_OK &&
                res != HRESULT.S_FALSE /*already initialized*/)
                throw new Win32Exception((int)res, "Failed to initialize OLE：必须设置线程STA，Main方法上加[STAThread]");

        }
        static IntPtr moduleHandle;
        public static IntPtr ModuleHandle
        {
            get
            {
                if (moduleHandle == IntPtr.Zero)
                {
                    moduleHandle = GetModuleHandle(null);
                }
                return moduleHandle;
            }
        }

        WNDCLASSEX wc = new WNDCLASSEX();
        bool touchMsg;
        public WindowImpl()
        {
            if (Window == null)
            {
                Window = this;
            }
            posttime = Application.Elapsed;
            var v = System.Environment.OSVersion.Version;
            if (v.Major >= 6 && DwmIsCompositionEnabled() && !Application.DesignMode)
            {
            }
            else
            {
                isLayered = true;
            }
            if ((v.Major == 6 && v.Minor < 2))
            {
               touchMsg =  RegisterTouchWindow(handle, RegisterTouchFlags.TWF_NONE);
            }
            _className = "CPFWindow-" + Guid.NewGuid();
            // 初始化窗口类结构  
            wc.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            //wc.style = (int)ClassStyles.CS_DBLCLKS;
            wc.lpfnWndProc = WndProc;
            wc.hInstance = ModuleHandle;
            wc.hbrBackground = IntPtr.Zero;
            wc.lpszClassName = _className;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hIcon = IntPtr.Zero;
            wc.hCursor = DefaultCursor;
            wc.lpszMenuName = "";
            // 注册窗口类  
            RegisterClassEx(ref wc);
            // 创建并显示窗口  
            handle = CreateWindowEx((int)ExStyle, _className, "窗体标题", (uint)Style,
              CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, IntPtr.Zero, IntPtr.Zero, ModuleHandle, null);

            //timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1), IsEnabled = true };
            //timer.Tick += Timer_Tick;

            if (UnmanagedMethods.ShCoreAvailable)
            {
                uint dpix, dpiy;

                var monitor = UnmanagedMethods.MonitorFromWindow(
                    handle,
                    UnmanagedMethods.MONITOR.MONITOR_DEFAULTTONEAREST);

                if (UnmanagedMethods.GetDpiForMonitor != null)
                {
                    if (UnmanagedMethods.GetDpiForMonitor(
                            monitor,
                            UnmanagedMethods.MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI,
                            out dpix,
                            out dpiy) == 0)
                    {
                        scaling = (float)(dpix / 96.0);
                    }
                }
            }

            scaling = Application.BaseScale * scaling;
            //scaling = 1;

            imeCentext = ImmGetContext(handle);
            if (imeCentext == IntPtr.Zero)
            {
                imeCentext = ImmCreateContext();
                //ImmAssociateContext(handle, imeCentext);
                createimeCentext = true;
            }
            dropTarget = new OleDropTarget(this);

            var r = UnmanagedMethods.RegisterDragDrop(handle, dropTarget);

            //UnmanagedMethods.RegisterDragDrop(handle, Marshal.GetComInterfaceForObject(dropTarget, typeof(IDropTarget)));
            //UpdateWindow(handle);
            useGPU = Application.GetDrawingFactory().UseGPU;
            if (!isLayered && useGPU)
            {
                wglContext = new WglContext(handle);
                if (wglContext.RC == IntPtr.Zero)
                {
                    //useGPU = false;
                    wglContext.Dispose();
                    wglContext = null;
                    isLayered = true;
                    SetWindowLong(handle, (int)WindowLongParam.GWL_EXSTYLE, (uint)ExStyle);
                }
            }
            if (!isLayered)
            {
                DWM_BLURBEHIND dwm = new DWM_BLURBEHIND();
                dwm.dwFlags = DWM_BB_ENABLE | DWM_BB_BLURREGION;
                dwm.fEnable = true;
                dwm.hRegionBlur = CreateRectRgn(0, 0, -1, -1);
                DwmEnableBlurBehindWindow(handle, ref dwm);
                DeleteObject(dwm.hRegionBlur);
                //MARGINS sRT = new MARGINS();
                //sRT.Right = sRT.Left = sRT.Top = sRT.Bottom = -1;
                //DwmExtendFrameIntoClientArea(handle, ref sRT);
            }
        }


        bool useGPU;

        CPF.Windows.OpenGL.WglContext wglContext;
        //[DllImport("QCall")]
        //private static extern void SetGlobalInstanceRegisteredForMarshalling(long id);
        OleDropTarget dropTarget;

        public static readonly IntPtr DefaultCursor = LoadCursor(IntPtr.Zero, new IntPtr((int)UnmanagedMethods.Cursor.IDC_ARROW));

        protected virtual WindowStyles ExStyle
        {
            get
            {
                var style = WindowStyles.WS_EX_LEFT;
                if (isLayered)
                {
                    style |= WindowStyles.WS_EX_LAYERED;
                }
                return style;
            }
        }

        protected bool isLayered;

        protected virtual WindowStyles Style
        {
            get
            {
                return WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_POPUP |
                  //WindowStyles.WS_BORDER |
                  //WindowStyles.WS_THICKFRAME |
                  WindowStyles.WS_MINIMIZEBOX
                  ;
            }
        }

        public void DragMove()
        {
            SendMessage(handle, (int)WindowsMessage.WM_SYSCOMMAND, (IntPtr)SystemCommand.SC_MOUSEMOVE, IntPtr.Zero);
            BeginInvoke(a =>
            {
                SendMessage(handle, (int)WindowsMessage.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
            }, null);
        }

        public void Invoke(SendOrPostCallback callback, object data)
        {
            if (Dispatcher.MainThread.CheckAccess())
            {
                callback(data);
            }
            else
            {
                var invokeMre = new ManualResetEvent(false);
                invokeQueue.Enqueue(new SendOrPostData { Data = data, SendOrPostCallback = callback, ManualResetEvent = invokeMre });
                PostMessage(handle, (uint)threadCallbackMessage, IntPtr.Zero, IntPtr.Zero);
                invokeMre.WaitOne();
            }
        }

        TimeSpan posttime;
        public void BeginInvoke(SendOrPostCallback callback, object data)
        {
            var time = Application.Elapsed;
            asyncQueue.Enqueue(new SendOrPostData { Data = data, SendOrPostCallback = callback });
            if (asyncQueue.Count <= 1 || (time.TotalMilliseconds - posttime.TotalMilliseconds) > 10)
            {
                PostMessage(handle, (uint)threadCallbackMessage, (IntPtr)1, IntPtr.Zero);
            }
            posttime = time;
        }
        bool canResize;
        public bool CanResize
        {
            get { return canResize; }
            set
            {
                canResize = value;
                var ex = GetWindowLong(handle, (int)WindowLongParam.GWL_STYLE);
                if (value)
                {
                    SetWindowLong(handle, (int)WindowLongParam.GWL_STYLE, ex | (uint)WindowStyles.WS_MAXIMIZEBOX);
                }
                else
                {
                    SetWindowLong(handle, (int)WindowLongParam.GWL_STYLE, ex ^ (uint)WindowStyles.WS_MAXIMIZEBOX);
                }
            }
        }
        public Action<PixelPoint> PositionChanged { get; set; }
        public event Action Move;
        public Action<Size> Resized { get; set; }
        public Action WindowStateChanged { get; set; }
        public Action ScalingChanged { get; set; }
        public Action Activated { get; set; }

        public Func<bool> Closing { get; set; }

        public Action Closed { get; set; }

        public Action Deactivated { get; set; }

        public IntPtr Handle { get { return handle; } }
        IntPtr handle;
        IntPtr hBitmap;
        Size oldSize;
        IntPtr ppvBits;
        /// <summary>
        /// 为了防止多屏幕拖动的时候抖动
        /// </summary>
        PixelPoint? ScalingChangedPosition;
        //bool cc;
        // 窗口过程  
        protected unsafe virtual IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == threadCallbackMessage)
            {
                if (wParam == IntPtr.Zero)
                {
                    if (invokeQueue.TryDequeue(out SendOrPostData result))
                    {
                        try
                        {
                            result.SendOrPostCallback(result.Data);
                        }
                        //catch (Exception e)
                        //{
                        //    throw new InvalidOperationException("Invoke操作异常", e);
                        //}
                        finally
                        {
                            result.ManualResetEvent.Set();
                        }
                    }
                }
                else
                {
                    while (asyncQueue.TryDequeue(out SendOrPostData data))
                    {
                        data.SendOrPostCallback(data.Data);
                    }
                }
            }
            if (WndPro != null)
            {
                var r = WndPro(hwnd, (WindowsMessage)msg, wParam, lParam);
                if (r != null)
                {
                    return r.Value;
                }
            }
            InputModifiers modifiers;
            switch ((WindowsMessage)msg)
            {
                case WindowsMessage.WM_ACTIVATEAPP:
                    //Debug.WriteLine(root.ToString() + "Deactivated");
                    //if (!cc)
                    //{
                    //    var s = Bounds;
                    //    InitOpenGLWin((int)s.Width, (int)s.Height, handle);
                    //}
                    //cc = true;
                    break;
                case WindowsMessage.WM_CREATE:
                    break;
                case UnmanagedMethods.WindowsMessage.WM_ACTIVATE:
                    //{
                    //}
                    //{
                    //    var s = Bounds;
                    //    ChangeOpenGLWinSize((int)s.Width, (int)s.Height);
                    //}
                    var wa = (UnmanagedMethods.WindowActivate)(ToInt32(wParam) & 0xffff);
                    //Debug.WriteLine(root.ToString() + " " + wa);
                    switch (wa)
                    {
                        case UnmanagedMethods.WindowActivate.WA_ACTIVE:
                        case UnmanagedMethods.WindowActivate.WA_CLICKACTIVE:
                            Activated?.Invoke();
                            break;
                        case UnmanagedMethods.WindowActivate.WA_INACTIVE:
                            Deactivated?.Invoke();
                            //Debug.WriteLine(root.ToString() + "Deactivated");
                            //root.InputManager.KeyboardDevice.ProcessEvent(new KeyEventArgs(root, Keys.None, root.InputManager.KeyboardDevice.Modifiers, root.InputManager.KeyboardDevice), KeyEventType.LostFocus);
                            break;
                    }
                    return IntPtr.Zero;
                //case WindowsMessage.WM_NCACTIVATE:
                //    if (!canActivate)
                //    {
                //        Debug.WriteLine(root.ToString() + "WM_NCACTIVATE");
                //        if (((int)wParam & 0xFFFF) != (int)WindowActivate.WA_INACTIVE)
                //        {
                //            if (lParam != IntPtr.Zero)
                //            {
                //                SetActiveWindow(lParam);
                //            }
                //            else
                //            {
                //                SetActiveWindow(IntPtr.Zero);
                //            }
                //        }
                //    }
                //    break;
                case WindowsMessage.WM_MOUSEACTIVATE:
                    if (!canActivate)
                    {
                        return new IntPtr(3);
                    }
                    break;
                case WindowsMessage.WM_DPICHANGED:
                    var dpi = ToInt32(wParam) & 0xffff;
#if Net4
                    var newDisplayRect = (UnmanagedMethods.RECT)Marshal.PtrToStructure(lParam, typeof(UnmanagedMethods.RECT));
#else
                    var newDisplayRect = Marshal.PtrToStructure<UnmanagedMethods.RECT>(lParam);
#endif
                    scaling = dpi / 96f;
                    //SetWindowPos(hwnd,
                    //    IntPtr.Zero,
                    //    newDisplayRect.left,
                    //    newDisplayRect.top,
                    //    newDisplayRect.right - newDisplayRect.left,
                    //    newDisplayRect.bottom - newDisplayRect.top,
                    //    SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);
                    ScalingChangedPosition = new PixelPoint(newDisplayRect.left, newDisplayRect.top);
                    ScalingChanged?.Invoke();
                    root.LayoutManager.ExecuteLayoutPass();
                    ScalingChangedPosition = null;
                    return IntPtr.Zero;
                case WindowsMessage.WM_NCHITTEST:
                    break;
                case WindowsMessage.WM_NCCALCSIZE:
                    if (wParam != (IntPtr)0)
                    {
                        //var style = GetWindowLong(handle, (int)WindowLongParam.GWL_STYLE);
                        //if ((style & (uint)WindowStyles.WS_THICKFRAME) == 0)
                        //{
                        //    SetWindowLong(handle, (int)WindowLongParam.GWL_STYLE, style | (uint)WindowStyles.WS_THICKFRAME);
                        //}
                    }
                    //break;
                    return (IntPtr)1;
                case WindowsMessage.WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(hwnd, lParam);
                    //break;
                    return IntPtr.Zero;
                case WindowsMessage.WM_NCLBUTTONDOWN:
                    int test = (int)wParam;
                    if (test == (int)HitTestValues.HTCLOSE || test == (int)HitTestValues.HTHELP || test == (int)HitTestValues.HTMINBUTTON || test == (int)HitTestValues.HTMAXBUTTON)
                    {
                        return IntPtr.Zero;
                    }
                    break;
                case WindowsMessage.WM_SYSCOMMAND:
                    //if (wParam.ToInt32() == 61728)
                    //{
                    //    return 0;
                    //}
                    break;
                case UnmanagedMethods.WindowsMessage.WM_KEYDOWN:
                case UnmanagedMethods.WindowsMessage.WM_SYSKEYDOWN:
                    GetKeyboardState(_keyStates);
                    InputModifiers result = 0;

                    if (IsDown(Keys.LeftAlt) || IsDown(Keys.RightAlt))
                    {
                        result |= InputModifiers.Alt;
                    }

                    if (IsDown(Keys.LeftCtrl) || IsDown(Keys.RightCtrl))
                    {
                        result |= InputModifiers.Control;
                    }

                    if (IsDown(Keys.LeftShift) || IsDown(Keys.RightShift))
                    {
                        result |= InputModifiers.Shift;
                    }

                    if (IsDown(Keys.LWin) || IsDown(Keys.RWin))
                    {
                        result |= InputModifiers.Windows;
                    }
                    root.InputManager.KeyboardDevice.Modifiers = result;
                    if (isEnable)
                    {
                        root.InputManager.KeyboardDevice.ProcessEvent(new KeyEventArgs(root, KeyInterop.KeyFromVirtualKey(ToInt32(wParam)), ToInt32(wParam), root.InputManager.KeyboardDevice.Modifiers, root.InputManager.KeyboardDevice), KeyEventType.KeyDown);
                    }
                    //System.Diagnostics.Debug.WriteLine(root.ToString() + wParam + "|" + lParam);
                    break;
                case UnmanagedMethods.WindowsMessage.WM_KEYUP:
                case UnmanagedMethods.WindowsMessage.WM_SYSKEYUP:

                    InputModifiers result1 = 0;

                    if (IsDown(Keys.LeftAlt) || IsDown(Keys.RightAlt))
                    {
                        result1 |= InputModifiers.Alt;
                    }

                    if (IsDown(Keys.LeftCtrl) || IsDown(Keys.RightCtrl))
                    {
                        result1 |= InputModifiers.Control;
                    }

                    if (IsDown(Keys.LeftShift) || IsDown(Keys.RightShift))
                    {
                        result1 |= InputModifiers.Shift;
                    }

                    if (IsDown(Keys.LWin) || IsDown(Keys.RWin))
                    {
                        result1 |= InputModifiers.Windows;
                    }
                    root.InputManager.KeyboardDevice.Modifiers = result1;
                    //root.InputManager.KeyboardDevice.Modifiers = GetMouseModifiers(wParam);
                    if (isEnable)
                    {
                        root.InputManager.KeyboardDevice.ProcessEvent(new KeyEventArgs(root, KeyInterop.KeyFromVirtualKey(ToInt32(wParam)), ToInt32(wParam), root.InputManager.KeyboardDevice.Modifiers, root.InputManager.KeyboardDevice), KeyEventType.KeyUp);
                    }
                    break;
                case UnmanagedMethods.WindowsMessage.WM_CHAR:
                    // Ignore control chars
                    if (ToInt32(wParam) >= 32 && isEnable)
                    {
                        var str = new string((char)ToInt32(wParam), 1);
                        root.InputManager.KeyboardDevice.ProcessEvent(new TextInputEventArgs(root, root.InputManager.KeyboardDevice, str), KeyEventType.TextInput);
                    }

                    break;
                case WindowsMessage.WM_IME_SETCONTEXT:
                    if (ToInt32(wParam) == 1)
                    {
                        var old = ImmAssociateContext(handle, imeCentext);
                    }
                    break;
                case WindowsMessage.WM_IME_STARTCOMPOSITION:

                    break;
                case WindowsMessage.WM_IME_ENDCOMPOSITION:

                    break;
                case WindowsMessage.WM_IME_NOTIFY:

                    break;
                case WindowsMessage.WM_MOUSEMOVE:
                    if (!_trackingMouse)
                    {
                        var tm = new TRACKMOUSEEVENT
                        {
#if Net4
                            cbSize = Marshal.SizeOf(typeof(TRACKMOUSEEVENT)),
#else
                            cbSize = Marshal.SizeOf<TRACKMOUSEEVENT>(),
#endif
                            dwFlags = 2,
                            hwndTrack = hwnd,
                            dwHoverTime = 0,
                        };

                        TrackMouseEvent(ref tm);
                    }
                    root.InputManager.KeyboardDevice.Modifiers = GetMouseModifiers(wParam);
                    modifiers = root.InputManager.KeyboardDevice.Modifiers;
                    if (isEnable)
                    {
                        root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), DipFromLParam(lParam), root.InputManager.MouseDevice, IsTouch()), root.LayoutManager.VisibleUIElements, EventType.MouseMove);
                    }
                    break;
                case WindowsMessage.WM_MOUSELEAVE:
                    _trackingMouse = false;
                    root.InputManager.KeyboardDevice.Modifiers = GetMouseModifiers(wParam);
                    modifiers = root.InputManager.KeyboardDevice.Modifiers;
                    if (isEnable)
                    {
                        root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), DipFromLParam(lParam), root.InputManager.MouseDevice, IsTouch()), root.LayoutManager.VisibleUIElements, EventType.MouseLeave);
                    }
                    break;
                case WindowsMessage.WM_MOUSEWHEEL:
                    root.InputManager.KeyboardDevice.Modifiers = GetMouseModifiers(wParam);
                    modifiers = root.InputManager.KeyboardDevice.Modifiers;
                    if (isEnable)
                    {
                        root.InputManager.MouseDevice.ProcessEvent(new MouseWheelEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), PointToClient(PointFromLParam(lParam)), root.InputManager.MouseDevice, new Vector(0, (ToInt32(wParam) >> 16))), root.LayoutManager.VisibleUIElements, EventType.MouseWheel);
                    }
                    break;
                case WindowsMessage.WM_MBUTTONUP:
                case WindowsMessage.WM_RBUTTONUP:
                case WindowsMessage.WM_LBUTTONUP:
                    MouseButton mouseButton = MouseButton.Left;
                    if ((WindowsMessage)msg == WindowsMessage.WM_RBUTTONUP)
                    {
                        mouseButton = MouseButton.Right;
                    }
                    if ((WindowsMessage)msg == WindowsMessage.WM_MBUTTONUP)
                    {
                        mouseButton = MouseButton.Middle;
                    }
                    root.InputManager.KeyboardDevice.Modifiers = GetMouseModifiers(wParam);
                    if (isEnable)
                    {
                        root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, root.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.LeftMouseButton), root.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.RightMouseButton), root.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.MiddleMouseButton), DipFromLParam(lParam), root.InputManager.MouseDevice, mouseButton, IsTouch()), root.LayoutManager.VisibleUIElements, EventType.MouseUp);
                    }
                    break;
                case WindowsMessage.WM_MBUTTONDOWN:
                case WindowsMessage.WM_LBUTTONDOWN:
                case WindowsMessage.WM_RBUTTONDOWN:
                    //Debug.WriteLine("触摸点击：" + IsTouch());
                    mouseButton = MouseButton.Left;
                    if ((WindowsMessage)msg == WindowsMessage.WM_RBUTTONDOWN)
                    {
                        mouseButton = MouseButton.Right;
                    }
                    if ((WindowsMessage)msg == WindowsMessage.WM_MBUTTONDOWN)
                    {
                        mouseButton = MouseButton.Middle;
                    }
                    root.InputManager.KeyboardDevice.Modifiers = GetMouseModifiers(wParam);
                    if (isEnable)
                    {
                        root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, root.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.LeftMouseButton), root.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.RightMouseButton), root.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.MiddleMouseButton), DipFromLParam(lParam), root.InputManager.MouseDevice, mouseButton, IsTouch()), root.LayoutManager.VisibleUIElements, EventType.MouseDown);
                    }
                    break;
                case WindowsMessage.WM_LBUTTONDBLCLK:
                case WindowsMessage.WM_MBUTTONDBLCLK:
                case WindowsMessage.WM_RBUTTONDBLCLK:
                    //Debug.WriteLine("DBLCLK");
                    break;
                case WindowsMessage.WM_MOVE:
                    //var pos = new Point((short)(ToInt32(lParam) & 0xffff) / scaling, (short)(ToInt32(lParam) >> 16) / scaling);
                    //var pos = Bounds.Location;
                    if (Move != null)
                    {
                        Move();
                    }
                    var pos = new PixelPoint((short)(ToInt32(lParam) & 0xffff), (short)(ToInt32(lParam) >> 16));
                    //bounds = Bounds;
                    PositionChanged?.Invoke(pos);
                    break;
                case WindowsMessage.WM_SIZE:
                    var size = (SizeCommand)wParam;

                    if (Resized != null &&
                        (size == SizeCommand.Restored ||
                         size == SizeCommand.Maximized))
                    {
                        var clientSize = new Size(ToInt32(lParam) & 0xffff, ToInt32(lParam) >> 16);
                        Resized(clientSize / scaling);
                    }

                    var windowState = size == SizeCommand.Maximized ? WindowState.Maximized
                        : (size == SizeCommand.Minimized ? WindowState.Minimized : WindowState.Normal);

                    if (windowState != this.oldwindowState || this.windowState == WindowState.FullScreen)
                    {
                        if (this.windowState != WindowState.FullScreen)
                        {
                            this.windowState = windowState;
                            oldwindowState = windowState;
                            WindowStateChanged?.Invoke();
                        }
                        else
                        {
                            if (oldwindowState != WindowState.FullScreen && windowState != WindowState.Normal)
                            {
                                oldwindowState = WindowState.FullScreen;
                                WindowStateChanged?.Invoke();
                            }
                        }
                    }
                    if (windowState != WindowState.Minimized)
                    {
                        RECT rr = new RECT(0, 0, LOWORD((int)lParam), HIWORD((int)lParam));
                        //Debug.WriteLine("WM_SIZE:" + rr.Width + "," + rr.Height);
                        //InvalidateRect(handle, ref rr, false);
                        if (root != null)
                        {
                            Invalidate(new Rect(new Point(), root.ActualSize));
                        }
                        //if (root != null)
                        //{
                        //System.Diagnostics.Debug.WriteLine(rr);
                        //root.Width = rr.Width / scaling;
                        //root.Height = rr.Height / scaling;
                        //root.ActualSize = rr;
                        //root.LayoutManager.ExecuteLayoutPass();
                        //}
                        if (!isLayered)
                        {
                            RedrawWindow(handle, ref rr, IntPtr.Zero, RDW_INVALIDATE | RDW_ERASE | RDW_ALLCHILDREN);
                            //UpdateWindow(handle);
                            //InvalidateRect(handle, ref rr, false);
                        }
                    }
                    break;
                case WindowsMessage.WM_NCUAHDRAWCAPTION:
                case WindowsMessage.WM_NCUAHDRAWFRAME:
                    return IntPtr.Zero;
                case WindowsMessage.WM_ERASEBKGND:
                    return (IntPtr)1;
                //case WindowsMessage.WM_NCACTIVATE:
                case WindowsMessage.WM_NCPAINT:
                    //DrawBoder(hwnd);
                    var _hdc = GetDC(hwnd);
                    OnPaint(_hdc, new Rect(new Point(), Bounds.Size));
                    ReleaseDC(hwnd, _hdc);
                    return (IntPtr)1;
                case WindowsMessage.WM_PAINT:
                    PAINTSTRUCT ps = new PAINTSTRUCT();
                    IntPtr hdc = BeginPaint(hwnd, out ps);
                    if (ps.rcPaint.Width > 0 && ps.rcPaint.Height > 0)
                    {
                        OnPaint(hdc, new Rect(ps.rcPaint.left, ps.rcPaint.top, ps.rcPaint.Width, ps.rcPaint.Height));
                    }
                    EndPaint(hwnd, ref ps);
                    break;
                case WindowsMessage.WM_SHOWWINDOW:
                    //System.Diagnostics.Debug.WriteLine(wParam);
                    //if (wParam == (IntPtr)1)
                    //{
                    //    root.Visibility = Visibility.Visible;
                    //}
                    //else
                    //{
                    //    root.Visibility = Visibility.Collapsed;
                    //}
                    break;
                case WindowsMessage.WM_POINTERDOWN:
                case WindowsMessage.WM_POINTERUP:
                case WindowsMessage.WM_POINTERUPDATE:
                    if (!touchMsg)
                    {
                        var id = GetPointerId(wParam);
                        var position = GetPointerLocation(lParam);
                        position = PointToClient(position);
                        //Debug.WriteLine("id:" + id + "  " + position);
                        POINTER_INFO pi = new POINTER_INFO();
                        if (GetPointerInfo(id, ref pi))
                        {
                            switch ((WindowsMessage)msg)
                            {
                                case WindowsMessage.WM_POINTERDOWN:
                                    //Debug.WriteLine("down");
                                    root.InputManager.TouchDevice.ProcessEvent(new TouchEventArgs(new TouchPoint { Id = id, Position = position }, root.InputManager.TouchDevice, Root), root.LayoutManager.VisibleUIElements, EventType.TouchDown);
                                    break;
                                case WindowsMessage.WM_POINTERUP:
                                    //Debug.WriteLine("up");
                                    root.InputManager.TouchDevice.ProcessEvent(new TouchEventArgs(new TouchPoint { Id = id, Position = position }, root.InputManager.TouchDevice, Root), root.LayoutManager.VisibleUIElements, EventType.TouchUp);
                                    //root.InputManager.TouchDevice.ProcessEvent(new TouchEventArgs(new TouchPoint { Id = id, Position = position }, root.InputManager.TouchDevice, Root), root.LayoutManager.VisibleUIElements, EventType.TouchLeave);
                                    break;
                                case WindowsMessage.WM_POINTERUPDATE:
                                    //Debug.WriteLine("update");
                                    root.InputManager.TouchDevice.ProcessEvent(new TouchEventArgs(new TouchPoint { Id = id, Position = position }, root.InputManager.TouchDevice, Root), root.LayoutManager.VisibleUIElements, EventType.TouchMove);
                                    break;
                            }
                        }
                    }
                    break;
                case WindowsMessage.WM_POINTERWHEEL:
                    Debug.WriteLine("WM_POINTERWHEEL");
                    break;
                case WindowsMessage.WM_GESTURE:
                    Debug.WriteLine("WM_GESTURE");
                    break;
                case WindowsMessage.WM_TOUCH:
                    if (touchMsg)
                    {
                        var touchInputCount = wParam.ToInt32();
                        var pTouchInputs = stackalloc TOUCHINPUT[touchInputCount];
                        if (GetTouchInputInfo(lParam, (uint)touchInputCount, pTouchInputs, Marshal.SizeOf(typeof(TOUCHINPUT))))
                        {
                            for (int i = 0; i < touchInputCount; i++)
                            {
                                var input = pTouchInputs[i];

                                if ((input.Flags & TouchInputFlags.TOUCHEVENTF_DOWN) > 0)
                                {
                                    Root.InputManager.TouchDevice.ProcessEvent(new TouchEventArgs(new TouchPoint
                                    {
                                        Id = (int)input.Id,
                                        Position = new Point((float)(input.X * 0.01), (float)(input.Y * 0.01))
                                    }, Root.InputManager.TouchDevice, Root), Root.LayoutManager.VisibleUIElements, EventType.TouchDown);

                                }
                                else if ((input.Flags & TouchInputFlags.TOUCHEVENTF_UP) > 0)
                                {
                                    Root.InputManager.TouchDevice.ProcessEvent(new TouchEventArgs(new TouchPoint
                                    {
                                        Id = (int)input.Id,
                                        Position = new Point((float)(input.X * 0.01), (float)(input.Y * 0.01))
                                    }, Root.InputManager.TouchDevice, Root), Root.LayoutManager.VisibleUIElements, EventType.TouchUp);

                                }
                                else if ((input.Flags & TouchInputFlags.TOUCHEVENTF_MOVE) > 0)
                                {
                                    Root.InputManager.TouchDevice.ProcessEvent(new TouchEventArgs(new TouchPoint
                                    {
                                        Id = (int)input.Id,
                                        Position = new Point((float)(input.X * 0.01), (float)(input.Y * 0.01))
                                    }, Root.InputManager.TouchDevice, Root), Root.LayoutManager.VisibleUIElements, EventType.TouchMove);
                                }
                            }
                            CloseTouchInputHandle(lParam);
                            return IntPtr.Zero;
                        }
                    }
                    break;
                case UnmanagedMethods.WindowsMessage.WM_CLOSE:
                    bool? preventClosing = Closing?.Invoke();
                    if (preventClosing == true)
                    {
                        return IntPtr.Zero;
                    }
                    if (_parent != null)
                    {
                        //EnableWindow(_parent.handle, true);
                        //_parent.SetEnable(true);
                        _parent.Activate();
                        SetFocus(_parent.handle);
                        SetWindowLongPtr(handle, (int)WindowLongParam.GWL_HWNDPARENT, IntPtr.Zero);
                        //_parent.SetVisible(true);
                        _parent = null;
                    }
                    break;
                case WindowsMessage.WM_DESTROY:
                    Closed?.Invoke();
                    if (handle != IntPtr.Zero)
                    {
                        try
                        {
                            RevokeDragDrop(handle);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }
                    if (imeCentext != IntPtr.Zero)
                    {
                        ImmReleaseContext(handle, imeCentext);
                        if (createimeCentext)
                        {
                            ImmDestroyContext(imeCentext);
                        }
                        imeCentext = IntPtr.Zero;
                    }
                    if (ismain)
                    {
                        PostQuitMessage(0);
                    }
                    timer?.Dispose();
                    timer = null;
                    wglContext?.Dispose();
                    wglContext = null;
                    if (hBitmap != IntPtr.Zero)
                    {
                        DeleteObject(hBitmap);
                        hBitmap = IntPtr.Zero;
                    }
                    //Dispose();
                    if (IconHandle != IntPtr.Zero)
                    {
                        DestroyIcon(IconHandle);
                        IconHandle = IntPtr.Zero;
                    }
                    if (_className != null)
                    {
                        UnmanagedMethods.UnregisterClass(_className, ModuleHandle);
                        _className = null;
                    }
                    if (RenderBitmap != null)
                    {
                        RenderBitmap.Dispose();
                        RenderBitmap = null;
                    }
                    handle = IntPtr.Zero;
                    foreach (var item in invokeQueue)
                    {
                        item.SendOrPostCallback(item.Data);
                        item.ManualResetEvent.Set();
                    }
                    if (ismain)
                    {
                        Window = null;
                    }
                    //GC.Collect();
                    break;
            }
            return DefWindowProc(hwnd, msg, wParam, lParam);
        }


        [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void InitOpenGLWin(int width, int height, IntPtr pwin);

        [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void ChangeOpenGLWinSize(int width, int height);
        /// <summary>
        /// 处理WindowProc
        /// </summary>
        public Func<IntPtr, WindowsMessage, IntPtr, IntPtr, IntPtr?> WndPro { get; set; }

        bool rendToBitmap;
        public bool RenderToBitmap
        {
            get { return rendToBitmap; }
            set
            {
                rendToBitmap = value;
                //if (value)
                //{
                //    if (!isLayered)
                //    {
                //        isLayered = true;
                //        SetWindowLong(handle, (int)WindowLongParam.GWL_EXSTYLE, (uint)ExStyle);
                //        ShowInTaskbar(false);
                //    }
                //}
            }
        }
        /// <summary>
        /// RenderToBitmap=true，图像渲染到这个位图里，界面将不显示
        /// </summary>
        public Bitmap RenderBitmap { get; private set; }
        public Action OnRenderedBitmap { get; set; }
        //int cc = 0;

        //void testPaint()
        //{
        //    OpenGl gl = new OpenGl();
        //    gl.Enable(OpenGl.GL_ALPHA_TEST);
        //    gl.Enable(OpenGl.GL_DEPTH_TEST);
        //    gl.Enable(OpenGl.GL_COLOR_MATERIAL);

        //    gl.Enable(OpenGl.GL_LIGHTING);
        //    gl.Enable(OpenGl.GL_LIGHT0);

        //    gl.Enable(OpenGl.GL_BLEND);
        //    gl.BlendFunc(OpenGl.GL_SRC_ALPHA, OpenGl.GL_ONE_MINUS_SRC_ALPHA);
        //    gl.ClearColor(0, 0, 0, 0);
        //    gl.Clear(OpenGl.GL_COLOR_BUFFER_BIT | OpenGl.GL_DEPTH_BUFFER_BIT);

        //    gl.PushMatrix();

        //    gl.Color(0, 1, 1);
        //    gl.Begin(OpenGl.GL_TRIANGLES);                              // Drawing Using Triangles
        //    gl.Color(1.0f, 0.0f, 0.0f);                      // Set The Color To Red
        //    gl.Vertex(0.0f, 1.0f, 0.0f);                  // Top
        //    gl.Color(0.0f, 1.0f, 0.0f);                      // Set The Color To Green
        //    gl.Vertex(-1.0f, -1.0f, 0.0f);                  // Bottom Left
        //    gl.Color(0.0f, 0.0f, 1.0f);                      // Set The Color To Blue
        //    gl.Vertex(1.0f, -1.0f, 0.0f);                  // Bottom Right
        //    gl.End();

        //    gl.PopMatrix();
        //    gl.Flush();
        //}
        void OnPaint(IntPtr hdc, Rect rect)
        {
            if (handle == IntPtr.Zero)
            {
                return;
            }
            var r = this.Bounds;

            root.LayoutManager.ExecuteLayoutPass();
            if (RenderToBitmap)
            {
                if (RenderBitmap == null || oldSize != r.Size)
                {
                    oldSize = r.Size;
                    if (RenderBitmap != null)
                    {
                        RenderBitmap.Dispose();
                    }
                    RenderBitmap = new Bitmap((int)r.Width, (int)r.Height);
                }
                if (root.LayoutManager.VisibleUIElements != null)
                {
                    using (DrawingContext dc = DrawingContext.FromBitmap(RenderBitmap))
                    {
                        root.RenderView(dc, rect);
                        //DrawRenderRectangle(dc);
                    }
                }
                if (OnRenderedBitmap != null)
                {
                    //Debug.WriteLine(r.Size);
                    OnRenderedBitmap();
                }
            }
            else
            {
                IntPtr screenDC = IntPtr.Zero;
                IntPtr memDc = IntPtr.Zero;
                var oldBits = IntPtr.Zero;

                if (isLayered || !useGPU)
                {
                    screenDC = GetDC(IntPtr.Zero);
                    memDc = CreateCompatibleDC(screenDC);
                    if (oldSize != r.Size || hBitmap == IntPtr.Zero)
                    {
                        oldSize = r.Size;
                        if (hBitmap != IntPtr.Zero)
                        {
                            DeleteObject(hBitmap);
                        }

                        BITMAPINFOHEADER info = new BITMAPINFOHEADER();
                        info.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                        info.biBitCount = 32;
                        info.biHeight = -(int)r.Height;
                        info.biWidth = (int)r.Width;
                        info.biPlanes = 1;
                        hBitmap = CreateDIBSection(memDc, ref info, 0, out ppvBits, IntPtr.Zero, 0);
                    }
                    oldBits = SelectObject(memDc, hBitmap);
                }

                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                wglContext?.MakeCurrent();
                if (root.LayoutManager.VisibleUIElements != null)
                {
                    IRenderTarget rt = new HDCRenderTarget((isLayered || !useGPU) ? memDc : hdc, (int)Bounds.Width, (int)Bounds.Height, !isLayered);
                    if (wglContext != null)
                    {
                        wglContext.GetFramebufferInfo(out var fb, out var sam, out var sten);
                        rt = new OpenGlRenderTarget<IntPtr>(hdc, wglContext, (int)Bounds.Width, (int)Bounds.Height, fb, sam, sten);
                    }
                    //Console.WriteLine(rt + " " + isLayered + "  " + useGPU + "  " + memDc);
                    using (DrawingContext dc = DrawingContext.FromRenderTarget(rt))
                    {
                        root.RenderView(dc, rect);
                        //DrawRenderRectangle(dc);
                    }
                    //testPaint();
                }
                wglContext?.SwapBuffers();
                //stopwatch.Stop();
                //Debug.WriteLine(stopwatch.ElapsedMilliseconds);

                if (hdc != IntPtr.Zero)
                {
                    //AlphaBlend(hdc, (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, memDc, (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, new BLENDFUNCTION { SourceConstantAlpha = 255, AlphaFormat = 1 });
                    if (!useGPU)
                    {
                        BitBlt(hdc, (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, memDc, (int)rect.X, (int)rect.Y, TernaryRasterOperations.SRCCOPY);
                    }
                }
                else
                {
                    if (isLayered)
                    {
                        if (windowState != WindowState.Minimized)
                        {
                            //Stopwatch stopwatch = new Stopwatch();
                            //stopwatch.Start();
                            BLENDFUNCTION blendFunc = new BLENDFUNCTION();
                            blendFunc.BlendOp = 0;
                            blendFunc.SourceConstantAlpha = 255;
                            blendFunc.AlphaFormat = 1;
                            blendFunc.BlendFlags = 0;
                            POINT topLoc = new POINT((int)r.Left, (int)r.Top);
                            SIZE bitMapSize = new SIZE((int)r.Width, (int)r.Height);
                            POINT srcLoc = new POINT(0, 0);
                            var rr = UpdateLayeredWindow(handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, 2);
                            //stopwatch.Stop();
                            //Debug.WriteLine(stopwatch.ElapsedMilliseconds);
                        }
                    }
                    else
                    {
                        //UpdateWindow(handle);
                    }
                }
                if (isLayered || !useGPU)
                {
                    SelectObject(memDc, oldBits);
                    ReleaseDC(IntPtr.Zero, screenDC);
                    DeleteDC(memDc);
                }
            }
        }

        //private void DrawRenderRectangle(DrawingContext dc)
        //{
        //    if (renderRectElement != null)
        //    {
        //        var rect1 = renderRectElement.GetContentBounds();
        //        var points = new List<Point>();
        //        points.Add(new Point());
        //        points.Add(new Point((float)Math.Round(rect1.Width * scaling) / scaling, 0));
        //        points.Add(new Point((float)Math.Round(rect1.Width * scaling) / scaling, (float)Math.Round(rect1.Height * scaling) / scaling));
        //        points.Add(new Point(0, (float)Math.Round(rect1.Height * scaling) / scaling));
        //        for (int i = 0; i < points.Count; i++)
        //        {
        //            points[i] = renderRectElement.TransformPoint(points[i]);
        //        }
        //        var p = renderRectElement.Parent;
        //        while (p != null && !p.IsRoot)
        //        {
        //            for (int i = 0; i < points.Count; i++)
        //            {
        //                points[i] = p.TransformPoint(points[i]);
        //            }
        //            p = p.Parent;
        //        }

        //        using (var brush = new SolidColorBrush(Color.FromRgb(255, 0, 0)))
        //        {
        //            var stroke = new Stroke(1);
        //            for (int i = 0; i < points.Count; i++)
        //            {
        //                dc.DrawLine(stroke, brush, points[i] * scaling, points[i == 3 ? 0 : (i + 1)] * scaling);
        //            }
        //        }
        //    }
        //}

        private Point PointFromLParam(IntPtr lParam)
        {
            return new Point((short)(ToInt32(lParam) & 0xffff), (short)(ToInt32(lParam) >> 16));
        }
        public Point PointToClient(Point point)
        {
            var p = new POINT { X = (int)point.X, Y = (int)point.Y };
            UnmanagedMethods.ScreenToClient(handle, ref p);
            return new Point(p.X, p.Y) / scaling;
        }
        //private Point DipFromLParam(IntPtr lParam)
        //{
        //    //var rect = WindowPixelRectangle;
        //    Debug.WriteLine((ToInt32(lParam) & 0xffff));
        //    return new Point((ToInt32(lParam) & 0xffff), (ToInt32(lParam) >> 16)) / scaling;
        //}
        private Point DipFromLParam(IntPtr lParam)
        {
            return new Point((short)(ToInt32(lParam) & 0xffff), (short)(ToInt32(lParam) >> 16)) / RenderScaling;
        }
        public static int ToInt32(IntPtr ptr)
        {
            if (IntPtr.Size == 4) return ptr.ToInt32();

            return (int)(ptr.ToInt64() & 0xffffffff);
        }
        public static int SignedHIWORD(int n)
        {
            int i = (int)(short)((n >> 16) & 0xffff);

            return i;
        }

        public static int SignedLOWORD(int n)
        {
            int i = (int)(short)(n & 0xFFFF);

            return i;
        }
        public static int SignedHIWORD(IntPtr n)
        {
            return SignedHIWORD(unchecked((int)(long)n));
        }
        public static int SignedLOWORD(IntPtr n)
        {
            return SignedLOWORD(unchecked((int)(long)n));
        }
        public bool IsTouch()
        {
            uint extra = GetMessageExtraInfo();
            bool isTouchOrPen = ((extra & 0xFFFFFF00) == 0xFF515700);
            if (!isTouchOrPen)
                return false;
            bool isTouch = ((extra & 0x00000080) == 0x00000080);
            return isTouch;
        }
        public static int GetPointerId(IntPtr WParam)
        {
            return (short)WParam;
        }

        public static Point GetPointerLocation(IntPtr LParam)
        {
            var lowword = LParam.ToInt32();
            return new Point()
            {
                X = (short)(lowword),
                Y = (short)(lowword >> 16),
            };
        }

        private bool IsDown(Keys key)
        {
            return (GetKeyStates(key) & KeyStates.Down) != 0;
        }

        byte[] _keyStates = new byte[256];
        private KeyStates GetKeyStates(Keys key)
        {
            int vk = KeyInterop.VirtualKeyFromKey(key);
            byte state = _keyStates[vk];
            KeyStates result = 0;

            if ((state & 0x80) != 0)
            {
                result |= KeyStates.Down;
            }

            if ((state & 0x01) != 0)
            {
                result |= KeyStates.Toggled;
            }

            return result;
        }
        [Flags]
        public enum KeyStates : byte
        {
            None = 0,
            Down = 1,
            Toggled = 2,
        }
        InputModifiers GetMouseModifiers(IntPtr wParam)
        {
            GetKeyboardState(_keyStates);
            var keys = (ModifierKeys)ToInt32(wParam);
            //var modifiers = InputManager.KeyboardDevice.Modifiers;
            var modifiers = InputModifiers.None;
            if (keys.HasFlag(ModifierKeys.MK_LBUTTON))
                modifiers |= InputModifiers.LeftMouseButton;
            if (keys.HasFlag(ModifierKeys.MK_RBUTTON))
                modifiers |= InputModifiers.RightMouseButton;
            if (keys.HasFlag(ModifierKeys.MK_MBUTTON))
                modifiers |= InputModifiers.MiddleMouseButton;
            if (keys.HasFlag(ModifierKeys.MK_CONTROL))
                modifiers |= InputModifiers.Control;
            if (keys.HasFlag(ModifierKeys.MK_SHIFT))
                modifiers |= InputModifiers.Shift;
            if (keys.HasFlag(ModifierKeys.MK_ALT) || IsDown(Keys.LeftAlt) || IsDown(Keys.RightAlt))
                modifiers |= InputModifiers.Alt;//因为wParam获取不到alt键
            return modifiers;
        }
        /// <summary>
        /// 窗体位置和尺寸，屏幕像素
        /// </summary>
        public Rect Bounds
        {
            get
            {
                RECT r = new RECT();
                GetWindowRect(handle, out r);
                return new Rect(r.left, r.top, r.Width, r.Height);
            }
            set
            {
                //bounds = value;
                if (ScalingChangedPosition.HasValue)
                {
                    SetWindowPos(handle, IntPtr.Zero, ScalingChangedPosition.Value.X, ScalingChangedPosition.Value.Y, (int)value.Width, (int)value.Height, SetWindowPosFlags.SWP_NOACTIVATE);
                }
                else
                {
                    SetWindowPos(handle, IntPtr.Zero, (int)value.Left, (int)value.Top, (int)value.Width, (int)value.Height, SetWindowPosFlags.SWP_NOACTIVATE);
                }

            }
        }
        //Rect? bounds;

        public PixelPoint Position
        {
            get
            {
                RECT r = new RECT();
                GetWindowRect(handle, out r);
                return new PixelPoint(r.left, r.top);
            }
            set
            {
                RECT r = new RECT();
                GetWindowRect(handle, out r);
                //bounds = new Rect(value.X, value.Y, r.Width, r.Height);
                SetWindowPos(handle, IntPtr.Zero, value.X, value.Y, r.Width, r.Height, SetWindowPosFlags.SWP_NOACTIVATE);
            }
        }


        public WindowState WindowState
        {
            get
            {
                var placement = default(UnmanagedMethods.WINDOWPLACEMENT);
                UnmanagedMethods.GetWindowPlacement(handle, ref placement);

                switch (placement.ShowCmd)
                {
                    case ShowWindowCommand.ShowMaximized:
                        if (windowState == WindowState.FullScreen)
                        {
                            return windowState;
                        }
                        return WindowState.Maximized;
                    case ShowWindowCommand.ShowMinimized:
                        return WindowState.Minimized;
                    default:
                        return WindowState.Normal;
                }
            }

            set
            {
                windowState = value;
                if (UnmanagedMethods.IsWindowVisible(handle))
                {
                    SetVisible(true);
                }
            }
        }

        private static bool multiMonitorSupport = (GetSystemMetrics(SystemMetric.SM_CMONITORS) != 0);
        private const int PRIMARY_MONITOR = unchecked((int)0xBAADF00D);
        public Screen Screen
        {
            get
            {
                if (multiMonitorSupport)
                {
                    var sc = MonitorFromWindow(handle, MONITOR.MONITOR_DEFAULTTONEAREST);
                    return ScreenImpl.FromMonitor(sc, IntPtr.Zero);
                }
                else
                {
                    return ScreenImpl.FromMonitor((IntPtr)PRIMARY_MONITOR, IntPtr.Zero);
                }
            }
        }
        bool canActivate = true;
        public bool CanActivate
        {
            get { return canActivate; }

            set
            {
                if (canActivate != value)
                {
                    canActivate = value;
                    if (!value)
                    {
                        SetWindowLongPtr(handle, (int)WindowLongParam.GWL_EXSTYLE, (IntPtr)(ExStyle | WindowStyles.WS_EX_NOACTIVATE));
                        SetWindowLongPtr(handle, (int)WindowLongParam.GWL_STYLE, (IntPtr)(Style | WindowStyles.WS_CHILD));
                    }
                    else
                    {
                        SetWindowLongPtr(handle, (int)WindowLongParam.GWL_EXSTYLE, (IntPtr)(ExStyle));
                        SetWindowLongPtr(handle, (int)WindowLongParam.GWL_STYLE, (IntPtr)(Style));
                    }
                }
            }
        }

        private void WmGetMinMaxInfo(IntPtr handle, IntPtr lParam)
        {
            // MINMAXINFO structure  
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Get handle for nearest monitor to this window  
            //WindowInteropHelper wih = new WindowInteropHelper(this);
            IntPtr hMonitor = MonitorFromWindow(handle, MONITOR.MONITOR_DEFAULTTONEAREST);

            // Get monitor info  
            MONITORINFO monitorInfo = new MONITORINFO();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            GetMonitorInfo(hMonitor, ref monitorInfo);

            //// Get HwndSource  
            //HwndSource source = HwndSource.FromHwnd(wih.Handle);
            //if (source == null)
            //    // Should never be null  
            //    throw new Exception("Cannot get HwndSource instance.");
            //if (source.CompositionTarget == null)
            //    // Should never be null  
            //    throw new Exception("Cannot get HwndTarget instance.");

            //// Get transformation matrix  
            //Matrix matrix = source.CompositionTarget.TransformFromDevice;

            // Convert working area  
            RECT workingArea = monitorInfo.rcWork;
            //Point dpiIndependentSize =
            //    matrix.Transform(new Point(
            //            workingArea.Right - workingArea.Left,
            //            workingArea.Bottom - workingArea.Top
            //            ));

            //// Convert minimum size  
            //Point dpiIndenpendentTrackingSize = matrix.Transform(new Point(
            //    this.MinWidth,
            //    this.MinHeight
            //    ));

            // Set the maximized size of the window  
            //mmi.ptMaxSize.x = (int)dpiIndependentSize.X;
            //mmi.ptMaxSize.y = (int)dpiIndependentSize.Y;
            if (windowState == WindowState.FullScreen)
            {
                mmi.ptMaxSize.X = monitorInfo.rcMonitor.right - monitorInfo.rcMonitor.left;
                mmi.ptMaxSize.Y = monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top;
            }
            else
            {
                mmi.ptMaxSize.X = workingArea.right - workingArea.left;
                mmi.ptMaxSize.Y = workingArea.bottom - workingArea.top;
            }
            ////mmi.ptMaxTrackSize.x = 1370;
            ////mmi.ptMaxTrackSize.y = 772;
            //// Set the position of the maximized window  
            mmi.ptMaxPosition.X = windowState == WindowState.FullScreen ? 0 : workingArea.left - monitorInfo.rcMonitor.left;
            mmi.ptMaxPosition.Y = windowState == WindowState.FullScreen ? 0 : workingArea.top - monitorInfo.rcMonitor.top;

            // Set the minimum tracking size  
            //mmi.ptMinTrackSize.X = 1;//(int)dpiIndenpendentTrackingSize.X;
            //mmi.ptMinTrackSize.Y = 1;//(int)dpiIndenpendentTrackingSize.Y;

            mmi.ptMinTrackSize.X = 0;
            mmi.ptMinTrackSize.Y = 0;

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        public void SetCursor(Cursor cursor)
        {
            var hCursor = (IntPtr)cursor.PlatformCursor == IntPtr.Zero ? DefaultCursor : (IntPtr)cursor.PlatformCursor;
            UnmanagedMethods.SetClassLong(handle, UnmanagedMethods.ClassLongIndex.GCLP_HCURSOR, hCursor);

            //if (_owner.IsPointerOver)
            //    UnmanagedMethods.SetCursor(hCursor);
        }

        public void SetRoot(View view)
        {
            root = view;
            root.LayoutUpdated += Root_LayoutUpdated;
        }

        private void Root_LayoutUpdated(object sender, RoutedEventArgs e)
        {
            var b = Bounds;
            var s = root.ActualSize;
            var l = root.ActualOffset;
            var src = root.Screen;
            if ((int)b.Width != (int)Math.Ceiling(s.Width * scaling) || (int)b.Height != (int)Math.Ceiling(s.Height * scaling) || (int)b.X != (int)(l.X * scaling + src.WorkingArea.X) || (int)b.Y != (int)(l.Y * scaling + src.WorkingArea.Y))
            {
                Bounds = new Rect(l.X * scaling + src.WorkingArea.X, l.Y * scaling + src.WorkingArea.Y, (float)Math.Ceiling(s.Width * scaling), (float)Math.Ceiling(s.Height * scaling));
            }
            //Debug.WriteLine("Root_LayoutUpdated" + s);
        }

        public float RenderScaling
        {
            get { return scaling; }
        }

        bool createimeCentext = false;
        IntPtr imeCentext;
        //public bool EnabledIME
        //{
        //    get { return ImmGetOpenStatus(imeCentext); }
        //    set
        //    {
        //        ImmSetOpenStatus(imeCentext, value);
        //    }
        //}
        public void SetIMEEnable(bool value)
        {
            ImmSetOpenStatus(imeCentext, value);
            if (IsTouch())
            {
                string windir = Environment.GetEnvironmentVariable("WINDIR");
                string osk = null;

                if (osk == null)
                {
                    osk = System.IO.Path.Combine(System.IO.Path.Combine(windir, "sysnative"), "osk.exe");
                    if (!System.IO.File.Exists(osk))
                        osk = null;
                }

                if (osk == null)
                {
                    osk = System.IO.Path.Combine(System.IO.Path.Combine(windir, "system32"), "osk.exe");
                    if (!System.IO.File.Exists(osk))
                    {
                        osk = null;
                    }
                }

                if (osk == null)
                    osk = "osk.exe";
                Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = osk });
            }
        }

        bool ismain;
        public bool IsMain
        {
            get
            {
                return ismain;
            }

            set
            {
                ismain = value;
                if (value)
                {
                    Window = this;
                }
            }
        }

        public void SetIMEPosition(Point point)
        {
            COMPOSITIONFORM cf = new COMPOSITIONFORM();
            cf.dwStyle = 2;
            cf.ptCurrentPos = new POINT((int)(point.X * scaling), (int)(point.Y * scaling));
            ImmSetCompositionWindow(imeCentext, ref cf);
        }

        public Point PointToScreen(Point point)
        {
            point *= scaling;
            var p = new POINT { X = (int)point.X, Y = (int)point.Y };
            ClientToScreen(handle, ref p);
            return new Point(p.X, p.Y);
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
            if (!paint && (timer == null))//&& isLayered
            {
                paint = true;
                BeginInvoke(a =>
                {
                    root.LayoutManager.ExecuteLayoutPass();
                    Rect rect1 = new Rect(((invalidateRect.X - 1) * scaling), ((invalidateRect.Y - 1) * scaling), ((invalidateRect.Width + 2) * scaling), ((invalidateRect.Height + 2) * scaling));
                    if (isLayered)
                    {
                        OnPaint(IntPtr.Zero, rect1);
                    }
                    else
                    {
                        RECT r = new RECT(rect1);
                        //InvalidateRect(handle, ref r, false);
                        RedrawWindow(handle, ref r, IntPtr.Zero, RDW_INVALIDATE | RDW_ALLCHILDREN | RDW_UPDATENOW);
                    }
                    //System.Diagnostics.Debug.WriteLine(invalidateRect);
                    invalidateRect = new Rect();
                    paint = false;
                }, null);
            }
            //if (timer == null && !isLayered)
            //{
            //    RECT r = new RECT((int)((rect.X - 1) * scaling), (int)((rect.Y - 1) * scaling), (int)((rect.Right + 2) * scaling), (int)((rect.Bottom + 2) * scaling));
            //    InvalidateRect(handle, ref r, false);
            //}
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (handle != IntPtr.Zero && IsWindowVisible(handle) && windowState != WindowState.Minimized && invalidateRect.Width > 0 && invalidateRect.Height > 0)
            {
                root.LayoutManager.ExecuteLayoutPass();
                Rect rect1 = new Rect(((invalidateRect.X - 1) * scaling), ((invalidateRect.Y - 1) * scaling), ((invalidateRect.Width + 2) * scaling), ((invalidateRect.Height + 2) * scaling));
                if (isLayered)
                {
                    OnPaint(IntPtr.Zero, rect1);
                }
                else
                {
                    RECT r = new RECT(rect1);
                    //InvalidateRect(handle, ref r, false);
                    RedrawWindow(handle, ref r, IntPtr.Zero, RDW_INVALIDATE | RDW_ALLCHILDREN | RDW_UPDATENOW);
                }
                //System.Diagnostics.Debug.WriteLine(invalidateRect);
                invalidateRect = new Rect();
            }
        }

        public void Activate()
        {
            UnmanagedMethods.SetActiveWindow(handle);
        }

        public void Capture()
        {
            SetCapture(handle);
        }

        WindowImpl _parent;
        public void ShowDialog(Window parent)
        {
            _parent = (WindowImpl)parent.ViewImpl;
            Show();
            SetWindowLongPtr(handle, (int)WindowLongParam.GWL_HWNDPARENT, _parent.handle);
        }

        public void ReleaseCapture()
        {
            UnmanagedMethods.ReleaseCapture();
        }

        public void Show()
        {
            //_parent = null;
            root.Visibility = Visibility.Visible;
            //if (isFirst)
            //{
            //    isFirst = false;
            //root.LayoutManager.ExecuteLayoutPass();

        }

        public void Hide()
        {
            root.Visibility = Visibility.Collapsed;
        }

        public void Close()
        {
            SendMessage(handle, (int)WindowsMessage.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        public void SetTitle(string title)
        {
            UnmanagedMethods.SetWindowText(handle, title);
        }

        bool _taskbarIcon = true;
        public void ShowInTaskbar(bool value)
        {
            if (_taskbarIcon == value)
            {
                return;
            }

            _taskbarIcon = value;

            var style = (UnmanagedMethods.WindowStyles)UnmanagedMethods.GetWindowLong(handle, (int)UnmanagedMethods.WindowLongParam.GWL_EXSTYLE);

            style &= ~(UnmanagedMethods.WindowStyles.WS_VISIBLE);

            style |= UnmanagedMethods.WindowStyles.WS_EX_TOOLWINDOW;

            if (value)
                style |= UnmanagedMethods.WindowStyles.WS_EX_APPWINDOW;
            else
                style &= ~(UnmanagedMethods.WindowStyles.WS_EX_APPWINDOW);

            WINDOWPLACEMENT windowPlacement = UnmanagedMethods.WINDOWPLACEMENT.Default;
            if (UnmanagedMethods.GetWindowPlacement(handle, ref windowPlacement))
            {
                //Toggle to make the styles stick
                UnmanagedMethods.ShowWindow(handle, ShowWindowCommand.Hide);
                UnmanagedMethods.SetWindowLong(handle, (int)UnmanagedMethods.WindowLongParam.GWL_EXSTYLE, (uint)style);
                UnmanagedMethods.ShowWindow(handle, windowPlacement.ShowCmd);
            }
        }

        public bool IsTopMost
        {
            get { return _topmost; }
        }

        bool _topmost = false;
        public void TopMost(bool value)
        {
            if (value == _topmost)
            {
                return;
            }

            IntPtr hWndInsertAfter = value ? WindowPosZOrder.HWND_TOPMOST : WindowPosZOrder.HWND_NOTOPMOST;
            UnmanagedMethods.SetWindowPos(handle,
                   hWndInsertAfter,
                   0, 0, 0, 0,
                   SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);

            _topmost = value;
        }

        public void Dispose()
        {
            if (handle != IntPtr.Zero)
            {
                var h = handle;
                handle = IntPtr.Zero;
                UnmanagedMethods.DestroyWindow(h);
            }
            //if (Window == this)
            //{
            //    Window = null;
            //}
        }

        public void SetVisible(bool visible)
        {
            //var v = IsWindowVisible(handle);
            if (visible)
            {
                if (windowState == WindowState.Normal)
                {
                    root.LayoutManager.ExecuteLayoutPass();
                    invalidateRect = new Rect(new Point(), root.ActualSize);
                    if (isLayered)
                    {
                        OnPaint(IntPtr.Zero, new Rect((invalidateRect.X - 1) * scaling, (invalidateRect.Y - 1) * scaling, (invalidateRect.Width + 2) * scaling, (invalidateRect.Height + 2) * scaling));
                    }
                    else
                    {
                        //UpdateWindow(handle);
                    }
                    invalidateRect = new Rect();
                    if (canActivate)
                    {
                        ShowWindow(handle, ShowWindowCommand.Normal);
                    }
                    else
                    {
                        ShowWindow(handle, ShowWindowCommand.ShowNoActivate);
                    }
                    //if (_parent != null)
                    //{
                    //    //EnableWindow(_parent.handle, false);
                    //    _parent.SetEnable(false);
                    //}
                    if (this is PopupImpl)
                    {
                        SetWindowPos(handle, WindowPosZOrder.HWND_TOP, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
                    }
                }
                else if (windowState == WindowState.Maximized || windowState == WindowState.FullScreen)
                {
                    root.LayoutManager.ExecuteLayoutPass();
                    invalidateRect = new Rect(new Point(), root.ActualSize);
                    if (isLayered)
                    {
                        OnPaint(IntPtr.Zero, new Rect((invalidateRect.X - 1) * scaling, (invalidateRect.Y - 1) * scaling, (invalidateRect.Width + 2) * scaling, (invalidateRect.Height + 2) * scaling));
                    }
                    else
                    {

                    }
                    invalidateRect = new Rect();
                    if (windowState == WindowState.FullScreen)
                    {
                        var placement = default(UnmanagedMethods.WINDOWPLACEMENT);
                        UnmanagedMethods.GetWindowPlacement(handle, ref placement);
                        if (placement.ShowCmd == ShowWindowCommand.ShowMaximized)
                        {
                            ShowWindow(handle, ShowWindowCommand.Normal);
                        }
                    }
                    ShowWindow(handle, ShowWindowCommand.Maximize);
                    //if (_parent != null)
                    //{
                    //    //EnableWindow(_parent.handle, false);
                    //    _parent.SetEnable(false);
                    //}
                }
                else
                {
                    ShowWindow(handle, ShowWindowCommand.Minimize);
                    //if (_parent != null)
                    //{
                    //    //EnableWindow(_parent.handle, false);
                    //    _parent.SetEnable(false);
                    //}
                }
                //if (bounds.HasValue)
                //{
                //    Bounds = bounds.Value;
                //}
                //Show();
            }
            else if (!visible)
            {
                //Hide();
                ShowWindow(handle, ShowWindowCommand.Hide);
            }
        }

        //private static int bitDepth;
        IntPtr IconHandle = IntPtr.Zero;
        public unsafe void SetIcon(Image image)
        {
            var stream = image.SaveToStream(ImageFormat.Png);
            var states = GdipCreateBitmapFromStream(new GPStream(stream), out IntPtr bitmap);
            stream.Dispose();
            GdipCreateHICONFromBitmap(bitmap, out IntPtr hIcon);
            GdipDisposeImage(bitmap);
            if (IconHandle != IntPtr.Zero)
            {
                DestroyIcon(IconHandle);
            }
            IconHandle = hIcon;
            PostMessage(handle, (int)WindowsMessage.WM_SETICON, new IntPtr((int)Icons.ICON_BIG), hIcon);
        }
        private unsafe short GetShort(byte* pb)
        {
            int retval = 0;
            if (0 != (unchecked((byte)pb) & 1))
            {
                retval = *pb;
                pb++;
                retval = unchecked(retval | (*pb << 8));
            }
            else
            {
                retval = unchecked((int)(*(short*)pb));
            }
            return unchecked((short)retval);
        }

        private unsafe int GetInt(byte* pb)
        {
            int retval = 0;
            if (0 != (unchecked((byte)pb) & 3))
            {
                retval = *pb; pb++;
                retval = retval | (*pb << 8); pb++;
                retval = retval | (*pb << 16); pb++;
                retval = unchecked(retval | (*pb << 24));
            }
            else
            {
                retval = *(int*)pb;
            }
            return retval;
        }


        //bool fullscreen;
        //public void SetFullscreen(bool fullscreen)
        //{
        //    this.fullscreen = fullscreen;
        //    if (fullscreen)
        //    {
        //        WindowState = WindowState.Maximized;
        //    }
        //    else
        //    {
        //        if (WindowState == WindowState.Maximized)
        //        {
        //            WindowState = WindowState.Normal;
        //        }
        //    }
        //}

        bool isEnable = true;
        public void SetEnable(bool enable)
        {
            isEnable = enable;
            //EnableWindow(handle, enable);
        }


        public float LayoutScaling { get { return scaling; } }
    }
    class PopupImpl : WindowImpl, IPopupImpl
    {
        protected override WindowStyles Style
        {
            get { return WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_POPUP; }// | WindowStyles.WS_MAXIMIZEBOX
        }
        protected override WindowStyles ExStyle
        {
            get
            {
                var style = WindowStyles.WS_EX_TOOLWINDOW | WindowStyles.WS_EX_TOPMOST;
                if (isLayered)
                {
                    style |= WindowStyles.WS_EX_LAYERED;
                }
                return style;
            }
        }

        protected override IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch ((WindowsMessage)msg)
            {
                case WindowsMessage.WM_ACTIVATE:
                    var wa = (WindowActivate)(ToInt32(wParam) & 0xffff);

                    switch (wa)
                    {
                        case WindowActivate.WA_INACTIVE:

                            break;
                    }
                    break;
                default:
                    break;
            }
            return base.WndProc(hwnd, msg, wParam, lParam);
        }
    }

    public class SendOrPostData
    {
        public SendOrPostCallback SendOrPostCallback;

        public object Data;

        public ManualResetEvent ManualResetEvent;
    }

}
