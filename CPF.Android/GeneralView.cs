using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CPF.Drawing;
using CPF.Input;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Android
{
    class GeneralView : IViewImpl
    {
        public GeneralView(ISurfaceView view, UIElement content)
        {
            owner = view as View;
            CpfView = view.CpfView;
            this.activity = owner.Context as Activity;
            //owner.CpfView.AddView(owner);
            owner.SetOnTouchListener(view);
            owner.SetOnKeyListener(view);
            owner.FocusableInTouchMode = true;
            softKeyboardListner = new SoftKeyboardListner(owner);
            owner.ViewTreeObserver.AddOnGlobalLayoutListener(softKeyboardListner);
            this.content = content;
        }
        UIElement content;
        public void Create()
        {
            if (content != null)
            {
                root = new InnerView(owner as ISurfaceView);
                root.Background = CPF.Drawing.Color.White;
                root.Children.Add(content);
                root.Visibility = CPF.Visibility.Visible;
                root.CanActivate = true;
                root.LayoutUpdated += Root_LayoutUpdated;
                scale = root.LayoutScaling;
            }
        }

        View owner;

        public CpfView CpfView { get; }
        internal SoftKeyboardListner softKeyboardListner;
        CPF.Drawing.Rect? layoutRect;
        bool isLayout;
        private void Root_LayoutUpdated(object sender, RoutedEventArgs e)
        {
            var b = new CPF.Drawing.Rect(owner.Left, owner.Top, owner.Width, owner.Height);
            if (CpfView.Handle != default)
            {
                b.X = CpfView.Left;
                b.Y = CpfView.Top;
                var s = root.ActualSize;
                var l = root.ActualOffset;
                var scaling = LayoutScaling;
                if ((int)b.Width != (int)(s.Width * scaling) || (int)b.Height != (int)(s.Height * scaling) || (int)b.X != (int)(l.X * scaling) || (int)b.Y != (int)(l.Y * scaling))
                {
                    var r = new CPF.Drawing.Rect((l.X * scaling), (l.Y * scaling), (s.Width * scaling), (s.Height * scaling));
                    if (layoutRect.HasValue)
                    {
                        layoutRect = r;
                        if (isLayout)
                        {
                            return;
                        }
                        isLayout = true;
                        //由于安卓自带动画影响布局，只能延迟刷新布局
                        this.Delay(TimeSpan.FromMilliseconds(50), () =>
                        {
                            if ((CpfView is PopupImpl) && CpfView.WindowVisibility == ViewStates.Visible)
                            {
                                CpfView.UpdateLayout((int)layoutRect.Value.Left, (int)layoutRect.Value.Top, (int)layoutRect.Value.Width, (int)layoutRect.Value.Height);
                            }
                            isLayout = false;
                        });
                    }
                    else
                    {
                        layoutRect = r;
                        if ((CpfView is PopupImpl) && CpfView.WindowVisibility == ViewStates.Visible)
                        {
                            CpfView.UpdateLayout((int)r.Left, (int)r.Top, (int)r.Width, (int)r.Height);
                        }
                    }
                    //Bounds = new Rect(l.X * scaling, l.Y * scaling, s.Width * scaling, s.Height * scaling);

                }
            }
        }

        internal Activity activity;
        CPF.Controls.View root;
        PixelSize oldSize;
        public CPF.Controls.View Root
        {
            get { return root; }
        }

        public void OnCreateDrawableState(int extraSpace)
        {
            Invalidate();
        }


        public Rect invalidateRect;
        bool _invalidateQueued;
        public void Invalidate()
        {
            Invalidate(new Rect(new Point(), root.ActualSize));
        }

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
            if (_invalidateQueued)
            {
                return;
            }
            _invalidateQueued = true;
            Threading.Dispatcher.MainThread.BeginInvoke(() =>
            {
                _invalidateQueued = false;
                if (CpfView.Handle != default)
                {
                    OnPaint();
                    invalidateRect = new Rect();
                }
            });
        }

        float scale;
        public void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            //base.OnLayout(changed, left, top, right, bottom);
            var size = new PixelSize(right, bottom);
            var sc = LayoutScaling;
            if (LayoutScaling != scale)
            {
                scale = sc;
                ScalingChanged();
            }
            if (size != oldSize)
            {
                oldSize = size;
                Resized(new Size(size.Width / LayoutScaling, size.Height / LayoutScaling));
            }
        }

        void OnPaint()
        {
            if (owner is AndroidView)
            {
                (owner as ISurfaceView).OnPaint();
            }
            else if (owner is OpenGLView)
            {
                //owner.Invalidate();
            }
        }

        DateTime _lastTouchMoveEventTime;
        Drawing.Point? _lastTouchMovePoint;


        InputConnection inputConnection;
        public IInputConnection OnCreateInputConnection(EditorInfo outAttrs)
        {
            outAttrs.InputType = (editor == null || editor.IsInputMethodEnabled) ? global::Android.Text.InputTypes.ClassText : global::Android.Text.InputTypes.TextVariationPassword;
            outAttrs.ImeOptions = ImeFlags.NoFullscreen;
            if (inputConnection == null)
            {
                inputConnection = new InputConnection(owner as ISurfaceView);
            }
            return inputConnection;
        }

        public bool OnKey(View v, [GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (e.Action != KeyEventActions.Multiple)
            {
                var routEvent = new CPF.Input.KeyEventArgs(root, ConvertKey(e.KeyCode), (int)e.KeyCode, root.InputManager.KeyboardDevice.Modifiers, root.InputManager.KeyboardDevice);
                root.InputManager.KeyboardDevice.Modifiers = GetModifierKeys(e);
                root.InputManager.KeyboardDevice.ProcessEvent(routEvent, e.Action == KeyEventActions.Down ? KeyEventType.KeyDown : KeyEventType.KeyUp);

                if (e.Action == KeyEventActions.Down && e.UnicodeChar >= 32)
                {
                    root.InputManager.KeyboardDevice.ProcessEvent(new TextInputEventArgs(root, root.InputManager.KeyboardDevice, Convert.ToChar(e.UnicodeChar).ToString()), KeyEventType.TextInput);
                }
            }
            return false;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            CPF.Drawing.Point _point = new Drawing.Point(e.GetX(), e.GetY());
            var position = new int[] { 0, 0 };
            owner.GetLocationOnScreen(position);
            double x = owner.GetX();
            double y = owner.GetY();
            (CPF.Platform.Application.GetRuntimePlatform() as AndroidPlatform).mousePosition = new PixelPoint((int)(e.RawX), (int)(e.RawY));

            //System.Diagnostics.Debug.WriteLine($"：视图在屏幕位置{position[0]},{position[1]} " +
            //    $" 鼠标位置{_point}、{e.RawX},{e.RawY} 鼠标在屏幕位置：{(CPF.Platform.Application.GetRuntimePlatform() as AndroidPlatform).mousePosition} 视图位置：{root.Position}  {Parent.IsLayoutRequested}");

            EventType? mouseEventType = null;
            EventType touchEvent = EventType.TouchDown;
            var eventTime = DateTime.Now;
            //Basic touch support
            switch (e.Action)
            {
                case MotionEventActions.Move:
                    //may be bot flood the evnt system with too many event especially on not so powerfull mobile devices
                    if ((eventTime - _lastTouchMoveEventTime).TotalMilliseconds > 10)
                    {
                        mouseEventType = EventType.MouseMove;
                        touchEvent = EventType.TouchMove;
                    }
                    break;

                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    mouseEventType = EventType.MouseDown;
                    touchEvent = EventType.TouchDown;
                    _lastTouchMoveEventTime = eventTime;
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    mouseEventType = EventType.MouseUp;
                    touchEvent = EventType.TouchUp;
                    break;
            }

            InputEventArgs routedEvent = null;
            if (mouseEventType != null)
            {
                //double r = x + owner.Width;
                //double b = y + owner.Height;

                //if (x <= _point.X && r >= _point.X && y <= _point.Y && b >= _point.Y)
                {
                    routedEvent = new MouseButtonEventArgs(root, mouseEventType != EventType.MouseUp, false, false, _point / root.LayoutScaling, root.InputManager.MouseDevice, MouseButton.Left, true);
                    //System.Diagnostics.Debug.WriteLine(_point / root.LayoutScaling + "  " + mouseEventType.Value);
                    root.InputManager.MouseDevice.ProcessEvent(routedEvent, root.LayoutManager.VisibleUIElements, mouseEventType.Value);
                    root.InputManager.TouchDevice.ProcessEvent(new TouchEventArgs(new TouchPoint { Id = e.ActionIndex, Position = _point / root.LayoutScaling }, root.InputManager.TouchDevice, root), root.LayoutManager.VisibleUIElements, touchEvent);
                    if (e.Action == MotionEventActions.Move && root.InputManager.MouseDevice.Captured == null)
                    {
                        if (_lastTouchMovePoint != null)
                        {
                            ////raise mouse scroll event so the scrollers
                            ////are moving with the cursor
                            //double vectorX = _point.X - _lastTouchMovePoint.Value.X;
                            //double vectorY = _point.Y - _lastTouchMovePoint.Value.Y;
                            ////based on test correction of 0.02 is working perfect
                            //double correction = 1;
                            //var ps = root.LayoutScaling;
                            //root.InputManager.MouseDevice.ProcessEvent(new MouseWheelEventArgs(root, false, false, false, _point / root.LayoutScaling, root.InputManager.MouseDevice, new Vector((float)(vectorX * correction / ps), (float)(vectorY * correction / ps)), true), root.LayoutManager.VisibleUIElements, EventType.MouseWheel);
                        }
                        _lastTouchMovePoint = _point;
                        _lastTouchMoveEventTime = eventTime;
                    }
                    else if (e.Action == MotionEventActions.Down)
                    {
                        _lastTouchMovePoint = _point;
                        UIElement parent = root.InputManager.MouseDevice.Captured;
                        while (parent != null)
                        {
                            if (parent is IEditor)
                            {
                                break;
                            }
                            else
                            {
                                parent = parent.Parent;
                            }
                        }
                        if (parent is IEditor editor)
                        {
                            ShowKeyboard(!editor.IsReadOnly, editor);
                        }
                        else
                        {
                            ShowKeyboard(false, null);
                        }
                    }
                    else
                    {
                        _lastTouchMovePoint = null;
                        if (mouseEventType == EventType.MouseUp)
                        {
                            root.InputManager.MouseDevice.Capture(null);
                        }
                    }
                    if (mouseEventType == EventType.MouseUp && root.IsMouseOver)
                    {
                        root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, false, false, false, new CPF.Drawing.Point(-1, -1), root.InputManager.MouseDevice, true), root.LayoutManager.VisibleUIElements, EventType.MouseLeave);
                    }
                }
                //else if (root.IsMouseOver)
                //{
                //    root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, false, false, false, new CPF.Drawing.Point(-1, -1), root.InputManager.MouseDevice), root.LayoutManager.VisibleUIElements, EventType.MouseLeave);
                //}
            }
            return e.Action != MotionEventActions.Up;
        }

        //public override bool DispatchKeyEvent(KeyEvent e)
        //{
        //    //var m = new WindowManagerLayoutParams(100, 100, 100, 100, WindowManagerTypes.Phone | WindowManagerTypes.ApplicationOverlay, WindowManagerFlags.NotFocusable, global::Android.Graphics.Format.Rgbx8888);
        //    ////m.Gravity = GravityFlags.Left | GravityFlags.Top;
        //    //activity.WindowManager.AddView(new TestView(activity) { Background = new global::Android.Graphics.Drawables.ColorDrawable(global::Android.Graphics.Color.Argb(100, 255, 0, 0)) }, m);


        //}

        public void DispatchSetActivated(bool activated)
        {
            if (activated)
            {
                (this as IViewImpl).Activated();
            }
            else
            {
                Deactivated();
            }
        }


        public Screen Screen
        {
            get
            {
                var display = owner.Display;
                if (display == null)
                {
                    display = activity.WindowManager.DefaultDisplay;
                    //return new Screen(new Drawing.Rect(), new Drawing.Rect(), true);
                }
                //global::Android.Graphics.Point point = new global::Android.Graphics.Point();
                //display.GetRealSize(point);

                global::Android.Graphics.Rect rect = new global::Android.Graphics.Rect();
                display.GetRectSize(rect);

                global::Android.Graphics.Point point1 = new global::Android.Graphics.Point();
                display.GetRealSize(point1);

                var screen = new Screen(new Drawing.Rect(0, 0, point1.X, point1.Y), new Drawing.Rect(rect.Left, rect.Top, rect.Right, rect.Bottom), true);
                return screen;
            }
        }

        public float RenderScaling
        {
            get
            {
                return global::Android.App.Application.Context.Resources.DisplayMetrics.ScaledDensity;
            }
        }

        public float LayoutScaling => RenderScaling;

        public Action ScalingChanged { get; set; }
        public Action<Size> Resized { get; set; }
        public Action<PixelPoint> PositionChanged { get; set; }
        public Action Activated { get; set; }
        public Action Deactivated { get; set; }
        public bool CanActivate { get { return owner.Focusable; } set { owner.Focusable = value; } }
        public PixelPoint Position
        {
            get
            {
                if (owner.Parent is PopupImpl popup)
                {
                    return popup.Location;
                }
                if (owner.Parent is ViewGroup group)
                {
                    return new PixelPoint(group.Left, group.Top);
                }
                return new PixelPoint(owner.Left, owner.Top);
            }
            set
            {
                //Console.WriteLine(value);
                if (owner.Parent is PopupImpl windowImpl)
                {
                    windowImpl.UpdateLayout(value.X, value.Y, windowImpl.LayoutParameters.Width, windowImpl.LayoutParameters.Height);
                }
                else if (owner.Parent is ViewGroup group)
                {
                    //var w = group.LayoutParameters.Width;
                    //var h = group.LayoutParameters.Height;
                    //group.Layout(value.X, value.Y, w, h);
                    group.Top = value.Y;
                    group.Left = value.X;
                }
            }
        }

        public void Activate()
        {
            //throw new NotImplementedException();
            owner.RequestFocus();
        }

        public void Capture()
        {
            owner.RequestPointerCapture();
            //throw new NotImplementedException();
        }

        public Drawing.Point PointToClient(Drawing.Point point)
        {
            var position = new int[] { 0, 0 };
            owner.GetLocationOnScreen(position);
            var p = new Drawing.Point(point.X - position[0], point.Y - position[1]);
            return p / LayoutScaling;
        }

        public Drawing.Point PointToScreen(Drawing.Point point)
        {
            var p = point * LayoutScaling;
            var position = new int[] { 0, 0 };
            owner.GetLocationOnScreen(position);
            return new Drawing.Point(position[0] + p.X, position[1] + p.Y);
        }

        public void ReleaseCapture()
        {
            owner.ReleasePointerCapture();
            //throw new NotImplementedException();
        }

        public void SetCursor(Cursor cursor)
        {
            //throw new NotImplementedException();
        }

        //internal bool IMEEnable = true;
        public void SetIMEEnable(bool enable)
        {
            //IMEEnable = enable;
        }

        bool showInput;
        internal static IEditor editor;
        public void ShowKeyboard(bool show, IEditor editor)
        {
            if (show == showInput && GeneralView.editor == editor)
            {
                return;
            }
            var input = owner.Context.GetSystemService(Context.InputMethodService).JavaCast<InputMethodManager>();
            GeneralView.editor = editor;
            if (show)
            {
                input.HideSoftInputFromWindow(owner.WindowToken, HideSoftInputFlags.None);
                input.RestartInput(owner);
            }
            showInput = show;

            if (show)
            {
                input.ShowSoftInput(owner, 0);
            }
            else
            {
                input.HideSoftInputFromWindow(owner.WindowToken, HideSoftInputFlags.None);
                //IMEEnable = true;
            }
        }

        void IViewImpl.SetIMEPosition(Drawing.Point point)
        {
            //throw new NotImplementedException();
        }

        public void SetRoot(Controls.View view)
        {
            root = view;
            root.Background = CPF.Drawing.Color.White;
            //root.Visibility = CPF.Visibility.Visible;
            root.CanActivate = true;
            root.LayoutUpdated += Root_LayoutUpdated;
            scale = root.LayoutScaling;
        }

        public void SetVisible(bool visible)
        {
            if (owner.Parent == null)
            {
                return;
            }
            if (visible)
            {
                ((ViewGroup)owner.Parent).Visibility = ViewStates.Visible;
            }
            else
            {
                ((ViewGroup)owner.Parent).Visibility = ViewStates.Gone;
            }
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (owner != null)
            {
                var o = owner;
                owner = null;
                o.Dispose();
            }
        }



        private static readonly Dictionary<Keycode, Keys> KeyDic = new Dictionary<Keycode, Keys>
     {
         //   { Keycode.Cancel?, Key.Cancel },
            { Keycode.Del, Keys.Back },
            { Keycode.Tab, Keys.Tab },
          //  { Keycode.Linefeed?, Key.LineFeed },
            { Keycode.Clear, Keys.Clear },
            { Keycode.Enter, Keys.Return },
            { Keycode.MediaPause, Keys.Pause },
            //{ Keycode.?, Key.CapsLock }
            //{ Keycode.?, Key.HangulMode }
            //{ Keycode.?, Key.JunjaMode }
            //{ Keycode.?, Key.FinalMode }
            //{ Keycode.?, Key.KanjiMode }
            { Keycode.Escape, Keys.Escape },
            //{ Keycode.?, Key.ImeConvert }
            //{ Keycode.?, Key.ImeNonConvert }
            //{ Keycode.?, Key.ImeAccept }
            //{ Keycode.?, Key.ImeModeChange }
            { Keycode.Space, Keys.Space },
            { Keycode.PageUp, Keys.Prior },
            { Keycode.PageDown, Keys.PageDown },
           // { Keycode.end?, Key.End },
            { Keycode.Home, Keys.Home },
            { Keycode.DpadLeft, Keys.Left },
            { Keycode.DpadUp, Keys.Up },
            { Keycode.DpadRight, Keys.Right },
            { Keycode.DpadDown, Keys.Down },
           // { Keycode.ButtonSelect?, Key.Select },
           // { Keycode.print?, Key.Print },
            //{ Keycode.execute?, Key.Execute },
           // { Keycode.snap, Key.Snapshot }
            { Keycode.Insert, Keys.Insert },
            { Keycode.ForwardDel, Keys.Delete },
            //{ Keycode.help, Key.Help },
            //{ Keycode.?, Key.D0 }
            //{ Keycode.?, Key.D1 }
            //{ Keycode.?, Key.D2 }
            //{ Keycode.?, Key.D3 }
            //{ Keycode.?, Key.D4 }
            //{ Keycode.?, Key.D5 }
            //{ Keycode.?, Key.D6 }
            //{ Keycode.?, Key.D7 }
            //{ Keycode.?, Key.D8 }
            //{ Keycode.?, Key.D9 }
            { Keycode.A, Keys.A },
            { Keycode.B, Keys.B },
            { Keycode.C, Keys.C },
            { Keycode.D, Keys.D },
            { Keycode.E, Keys.E },
            { Keycode.F, Keys.F },
            { Keycode.G, Keys.G },
            { Keycode.H, Keys.H },
            { Keycode.I, Keys.I },
            { Keycode.J, Keys.J },
            { Keycode.K, Keys.K },
            { Keycode.L, Keys.L },
            { Keycode.M, Keys.M },
            { Keycode.N, Keys.N },
            { Keycode.O, Keys.O },
            { Keycode.P, Keys.P },
            { Keycode.Q, Keys.Q },
            { Keycode.R, Keys.R },
            { Keycode.S, Keys.S },
            { Keycode.T, Keys.T },
            { Keycode.U, Keys.U },
            { Keycode.V, Keys.V },
            { Keycode.W, Keys.W },
            { Keycode.X, Keys.X },
            { Keycode.Y, Keys.Y },
            { Keycode.Z, Keys.Z },
            //{ Keycode.a, Key.A },
            //{ Keycode.b, Key.B },
            //{ Keycode.c, Key.C },
            //{ Keycode.d, Key.D },
            //{ Keycode.e, Key.E },
            //{ Keycode.f, Key.F },
            //{ Keycode.g, Key.G },
            //{ Keycode.h, Key.H },
            //{ Keycode.i, Key.I },
            //{ Keycode.j, Key.J },
            //{ Keycode.k, Key.K },
            //{ Keycode.l, Key.L },
            //{ Keycode.m, Key.M },
            //{ Keycode.n, Key.N },
            //{ Keycode.o, Key.O },
            //{ Keycode.p, Key.P },
            //{ Keycode.q, Key.Q },
            //{ Keycode.r, Key.R },
            //{ Keycode.s, Key.S },
            //{ Keycode.t, Key.T },
            //{ Keycode.u, Key.U },
            //{ Keycode.v, Key.V },
            //{ Keycode.w, Key.W },
            //{ Keycode.x, Key.X },
            //{ Keycode.y, Key.Y },
            //{ Keycode.z, Key.Z },
            //{ Keycode.?, Key.LWin }
            //{ Keycode.?, Key.RWin }
            //{ Keycode.?, Key.Apps }
            //{ Keycode.?, Key.Sleep }
            //{ Keycode.?, Key.NumPad0 }
            //{ Keycode.?, Key.NumPad1 }
            //{ Keycode.?, Key.NumPad2 }
            //{ Keycode.?, Key.NumPad3 }
            //{ Keycode.?, Key.NumPad4 }
            //{ Keycode.?, Key.NumPad5 }
            //{ Keycode.?, Key.NumPad6 }
            //{ Keycode.?, Key.NumPad7 }
            //{ Keycode.?, Key.NumPad8 }
            //{ Keycode.?, Key.NumPad9 }
            { Keycode.NumpadMultiply, Keys.Multiply },
            { Keycode.NumpadAdd, Keys.Add },
            { Keycode.NumpadComma, Keys.Separator },
            { Keycode.NumpadSubtract, Keys.Subtract },
            //{ Keycode.numpaddecimal?, Key.Decimal }
            { Keycode.NumpadDivide, Keys.Divide },
            { Keycode.F1, Keys.F1 },
            { Keycode.F2, Keys.F2 },
            { Keycode.F3, Keys.F3 },
            { Keycode.F4, Keys.F4 },
            { Keycode.F5, Keys.F5 },
            { Keycode.F6, Keys.F6 },
            { Keycode.F7, Keys.F7 },
            { Keycode.F8, Keys.F8 },
            { Keycode.F9, Keys.F9 },
            { Keycode.F10, Keys.F10 },
            { Keycode.F11, Keys.F11 },
            { Keycode.F12, Keys.F12 },
            //{ Keycode.f13, Key.F13 },
            //{ Keycode.F14, Key.F14 },
            //{ Keycode.L5, Key.F15 },
            //{ Keycode.F16, Key.F16 },
            //{ Keycode.F17, Key.F17 },
            //{ Keycode.L8, Key.F18 },
            //{ Keycode.L9, Key.F19 },
            //{ Keycode.L10, Key.F20 },
            //{ Keycode.R1, Key.F21 },
            //{ Keycode.R2, Key.F22 },
            //{ Keycode.F23, Key.F23 },
            //{ Keycode.R4, Key.F24 },
           // { Keycode.numpad, Key.NumLock }
            { Keycode.ScrollLock, Keys.Scroll },
            { Keycode.ShiftLeft, Keys.LeftShift },
            //{ Keycode.?, Key.RightShift }
            //{ Keycode.?, Key.LeftCtrl }
            //{ Keycode.?, Key.RightCtrl }
            //{ Keycode.?, Key.LeftAlt }
            //{ Keycode.?, Key.RightAlt }
            //{ Keycode.?, Key.BrowserBack }
            //{ Keycode.?, Key.BrowserForward }
            //{ Keycode.?, Key.BrowserRefresh }
            //{ Keycode.?, Key.BrowserStop }
            //{ Keycode.?, Key.BrowserSearch }
            //{ Keycode.?, Key.BrowserFavorites }
            //{ Keycode.?, Key.BrowserHome }
            //{ Keycode.?, Key.VolumeMute }
            //{ Keycode.?, Key.VolumeDown }
            //{ Keycode.?, Key.VolumeUp }
            //{ Keycode.?, Key.MediaNextTrack }
            //{ Keycode.?, Key.MediaPreviousTrack }
            //{ Keycode.?, Key.MediaStop }
            //{ Keycode.?, Key.MediaPlayPause }
            //{ Keycode.?, Key.LaunchMail }
            //{ Keycode.?, Key.SelectMedia }
            //{ Keycode.?, Key.LaunchApplication1 }
            //{ Keycode.?, Key.LaunchApplication2 }
            //{ Keycode.?, Key.OemSemicolon }
            //{ Keycode.?, Key.OemPlus }
            //{ Keycode.?, Key.OemComma }
            //{ Keycode.?, Key.OemMinus }
            //{ Keycode.?, Key.OemPeriod }
            //{ Keycode.?, Key.Oem2 }
            //{ Keycode.?, Key.OemTilde }
            //{ Keycode.?, Key.AbntC1 }
            //{ Keycode.?, Key.AbntC2 }
            //{ Keycode.?, Key.Oem4 }
            //{ Keycode.?, Key.OemPipe }
            //{ Keycode.?, Key.OemCloseBrackets }
            //{ Keycode.?, Key.Oem7 }
            //{ Keycode.?, Key.Oem8 }
            //{ Keycode.?, Key.Oem102 }
            //{ Keycode.?, Key.ImeProcessed }
            //{ Keycode.?, Key.System }
            //{ Keycode.?, Key.OemAttn }
            //{ Keycode.?, Key.OemFinish }
            //{ Keycode.?, Key.DbeHiragana }
            //{ Keycode.?, Key.OemAuto }
            //{ Keycode.?, Key.DbeDbcsChar }
            //{ Keycode.?, Key.OemBackTab }
            //{ Keycode.?, Key.Attn }
            //{ Keycode.?, Key.DbeEnterWordRegisterMode }
            //{ Keycode.?, Key.DbeEnterImeConfigureMode }
            //{ Keycode.?, Key.EraseEof }
            //{ Keycode.?, Key.Play }
            //{ Keycode.?, Key.Zoom }
            //{ Keycode.?, Key.NoName }
            //{ Keycode.?, Key.DbeEnterDialogConversionMode }
            //{ Keycode.?, Key.OemClear }
            //{ Keycode.?, Key.DeadCharProcessed }
        };

        internal static Keys ConvertKey(Keycode key)
        {
            Keys result;
            return KeyDic.TryGetValue(key, out result) ? result : Keys.None;
        }

        private static InputModifiers GetModifierKeys(KeyEvent e)
        {
            var rv = InputModifiers.None;

            if (e.IsCtrlPressed) rv |= InputModifiers.Control;
            if (e.IsShiftPressed) rv |= InputModifiers.Shift;

            return rv;
        }
    }
}