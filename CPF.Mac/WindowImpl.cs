using System;
using CPF;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using CPF.Threading;
using System.Collections.Concurrent;
using CPF.Drawing;
using CPF.Input;
using System.Linq;
using CPF.Controls;
using CPF.Platform;
using CPF.Mac.Foundation;
using CPF.Mac.AppKit;
using CPF.Mac.CoreGraphics;
using CPF.Mac.ObjCRuntime;
using System.Diagnostics;

namespace CPF.Mac
{
    //[Register("WindowImpl", true)]
    public class WindowImpl : NSWindow, IWindowImpl, NotMonoMac
    {
        internal NSView view;
        public WindowImpl() : base(new CGRect(0, 0, 10, 10), (NSWindowStyle.Closable | NSWindowStyle.Miniaturizable), NSBackingStore.Nonretained, false)
        {
            AppDelegate.windows.Add(this);
            MinSize = new CGSize(1, 1);
            ContentMinSize = new CGSize(1, 1);
            if (Application.GetDrawingFactory().UseGPU)
            {
                view = new OpenGLView(Frame, this);
            }
            else
            {
                view = new InnerView(Frame, this);
            }

            ContentView = view;
            IsOpaque = false;
            BackgroundColor = NSColor.Clear;
            AcceptsMouseMovedEvents = true;
            HasShadow = false;
            ReleasedWhenClosed = true;

            //RegisterForDraggedTypes(new string[] { NSPasteboard.NSFilenamesType, "*.jpg", "*.jpeg" });
            //ShouldDragDocumentWithEvent = (NSWindow window, NSEvent theEvent, CGPoint dragImageLocation, NSPasteboard withPasteboard) =>
            //{
            //    return true;
            //};
            //DocumentEdited = true;
            //WillReturnFieldEditor = (NSWindow sender, NSObject client) =>
            //{
            //    return FieldEditor(false,client);
            //};

            //WillUseFullScreenPresentationOptions = (s, e) =>
            //{
            //    //return e;
            //    return NSApplicationPresentationOptions.AutoHideToolbar| NSApplicationPresentationOptions.FullScreen| NSApplicationPresentationOptions.AutoHideMenuBar;
            //};
            //this.WindowShouldClose = delegate
            // {
            //     return !Closing();
            // };
            this.DidMiniaturize += (s, e) =>
            {
                //最小化
                windowState = WindowState.Minimized;
                if (WindowStateChanged != null)
                {
                    WindowStateChanged();
                }
            };
            this.DidDeminiaturize += (s, e) =>
            {
                if (windowState == WindowState.Minimized)
                {
                    windowState = WindowState.Normal;
                }
                //取消最小化
                if (WindowStateChanged != null)
                {
                    WindowStateChanged();
                }
            };
            WillClose += WindowImpl_WillClose;
            DidMoved += WindowImpl_DidMoved;
            //scaling = (float)BackingScaleFactor;
        }
        public override void PerformClose(NSObject sender)
        {
            base.PerformClose(sender);
        }
        internal NSEvent LeftMouseDown;
        bool isMouseEnter;
        public override void SendEvent(NSEvent theEvent)
        {
            base.SendEvent(theEvent);
            if (!enable)
            {
                return;
            }
            MouseButton mouseButton;
            switch (theEvent.Type)
            {
                case NSEventType.MouseMoved:
                    var isme = view.HitTest(theEvent.LocationInWindow) != null;
                    if (isme != isMouseEnter)
                    {
                        if (!isme)
                        {
                            mouseButton = Mouse(theEvent);
                            root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y) / LayoutScaling, root.InputManager.MouseDevice), root.LayoutManager.VisibleUIElements, EventType.MouseLeave);
                            NSCursor.ArrowCursor.Set();
                        }
                        isMouseEnter = isme;
                    }

                    if (isMouseEnter || root.InputManager.MouseDevice.Captured != null)
                    {
                        mouseButton = Mouse(theEvent);
                        if (!root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y) / LayoutScaling, root.InputManager.MouseDevice), root.LayoutManager.VisibleUIElements, EventType.MouseMove))
                        {
                            if (root.Children.FirstOrDefault(a => a.IsMouseOver) == null)
                            {
                                SetCursor(root.Cursor);
                            }
                        }
                    }
                    break;
                case NSEventType.LeftMouseDown:
                    LeftMouseDown = theEvent;
                    modifiers |= InputModifiers.LeftMouseButton;
                    mouseButton = Mouse(theEvent);
                    root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y) / LayoutScaling, root.InputManager.MouseDevice, mouseButton), root.LayoutManager.VisibleUIElements, EventType.MouseDown);
                    LeftMouseDown = null;
                    //this.DisableCursorRects();
                    break;
                case NSEventType.LeftMouseUp:
                    //this.EnableCursorRects();
                    modifiers ^= InputModifiers.LeftMouseButton;
                    mouseButton = Mouse(theEvent);
                    root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y) / LayoutScaling, root.InputManager.MouseDevice, mouseButton), root.LayoutManager.VisibleUIElements, EventType.MouseUp);
                    break;
                case NSEventType.LeftMouseDragged:
                    mouseButton = Mouse(theEvent);
                    root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y) / LayoutScaling, root.InputManager.MouseDevice), root.LayoutManager.VisibleUIElements, EventType.MouseMove);
                    break;
                case NSEventType.RightMouseDown:
                    modifiers |= InputModifiers.RightMouseButton;
                    mouseButton = Mouse(theEvent);
                    root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y) / LayoutScaling, root.InputManager.MouseDevice, mouseButton), root.LayoutManager.VisibleUIElements, EventType.MouseDown);
                    break;
                case NSEventType.RightMouseUp:
                    modifiers ^= InputModifiers.RightMouseButton;
                    mouseButton = Mouse(theEvent);
                    root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y) / LayoutScaling, root.InputManager.MouseDevice, mouseButton), root.LayoutManager.VisibleUIElements, EventType.MouseUp);
                    break;
                case NSEventType.RightMouseDragged:
                    mouseButton = Mouse(theEvent);
                    root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y) / LayoutScaling, root.InputManager.MouseDevice), root.LayoutManager.VisibleUIElements, EventType.MouseMove);
                    break;
                case NSEventType.OtherMouseDown:
                    modifiers |= InputModifiers.MiddleMouseButton;
                    mouseButton = Mouse(theEvent);
                    root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y) / LayoutScaling, root.InputManager.MouseDevice, mouseButton), root.LayoutManager.VisibleUIElements, EventType.MouseDown);
                    break;
                case NSEventType.OtherMouseUp:
                    modifiers ^= InputModifiers.MiddleMouseButton;
                    mouseButton = Mouse(theEvent);
                    root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y) / LayoutScaling, root.InputManager.MouseDevice, mouseButton), root.LayoutManager.VisibleUIElements, EventType.MouseUp);
                    break;
                case NSEventType.OtherMouseDragged:
                    mouseButton = Mouse(theEvent);
                    root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y) / LayoutScaling, root.InputManager.MouseDevice), root.LayoutManager.VisibleUIElements, EventType.MouseMove);
                    break;
                //case NSEventType.KeyDown:

                //    break;
                //case NSEventType.KeyUp:

                //    break;
                default:
                    break;
            }
        }

        public override void TouchesBeganWithEvent(NSEvent theEvent)
        {
            base.TouchesBeganWithEvent(theEvent);
            Console.WriteLine("TouchesBeganWithEvent");
        }
        public override void TouchesEndedWithEvent(NSEvent theEvent)
        {
            base.TouchesEndedWithEvent(theEvent);
            Console.WriteLine("TouchesEndedWithEvent");
        }
        public override void TouchesCancelledWithEvent(NSEvent theEvent)
        {
            base.TouchesCancelledWithEvent(theEvent);
            Console.WriteLine("TouchesCancelledWithEvent");
        }

        CGPoint oldPoint;
        private void WindowImpl_DidMoved(object sender, EventArgs e)
        {
            UpdateCursor();
            if (PositionChanged != null)
            {
                var h = this.Screen.Bounds.Height;
                var point = new PixelPoint((int)Frame.X, (int)(h - Frame.Height - Frame.Y));
                oldPoint = Frame.Location;
                PositionChanged(point);
            }
        }

        private void WindowImpl_WillClose(object sender, EventArgs e)
        {
            Console.WriteLine("WillClose");
            if (isMainVisible)
            {
                return;
            }
            if (Closed != null)
            {
                Closed();
            }
            //if (parent != null)
            //{
            //    //parent.IsEnabled = true;
            //    (parent.ViewImpl as WindowImpl).enable = false;
            //}
            //parent = null;
            if (root is Window)
            {
                if ((root as Window).IsMain)
                {
                    var app = MacPlatform.Application;
                    app.Terminate(app);
                }
            }
        }
        //public override bool AcceptsMouseMovedEvents { get => true; set => base.AcceptsMouseMovedEvents = value; }
        public override bool CanBecomeKeyWindow => CanActivate;
        public override bool AcceptsFirstResponder()
        {
            return true;
        }
        public override bool CanBecomeMainWindow => true;
        bool validDpi;
        public void InvalidateDpi()
        {
            validDpi = false;
        }
        //double dpi;
        //internal double DPIY
        //{
        //    get
        //    {
        //        if (!validDpi)
        //        {
        //            using (var str = new NSString("NSDeviceResolution"))
        //            {
        //                var screen = base.Screen;
        //                if (screen == null)
        //                {
        //                    screen = NSScreen.MainScreen;
        //                }
        //                var v = screen.DeviceDescription.ObjectForKey(str) as NSValue;
        //                scaling = (float)BackingScaleFactor;
        //                dpi = v.SizeValue.Height;
        //            }
        //        }
        //        return dpi;
        //    }
        //}

        CGRect lastFrame;
        bool validFrame = true;
        internal CPF.Controls.View root;
        //float scaling = 1;
        internal WindowState windowState = WindowState.Normal;
        public Func<bool> Closing { get; set; }
        public Action Closed { get; set; }

        public override void Close()
        {
            isMainVisible = false;
            if (Closing != null)
            {
                if (!Closing.Invoke())
                {
                    base.Close();
                }
            }
            else
            {
                base.Close();
            }
        }

        public WindowState WindowState
        {
            get
            {
                return windowState;
            }
            set
            {
                switch (value)
                {
                    case WindowState.Normal:
                        if (IsMiniaturized)
                        {
                            Deminiaturize(this);
                            lastFrame = Frame;
                        }
                        else
                        {
                            //if ((root as Window).IsFullScreen)
                            //{
                            //    (root as Window).IsFullScreen = false;
                            //}
                            //else
                            //{
                            if (windowState == WindowState.FullScreen)
                            {
                                IsZoomed = false;
                                ToggleFullScreen(this);
                                CollectionBehavior = NSWindowCollectionBehavior.Default;
                            }
                            windowState = WindowState.Normal;
                            if (WindowStateChanged != null)
                            {
                                WindowStateChanged();
                            }
                            if (!validFrame)
                            {
                                var screen = Screen;
                                lastFrame = new CGRect(screen.WorkingArea.X + (screen.WorkingArea.Width * 0.05), screen.WorkingArea.Y + (screen.WorkingArea.Height * 0.05), screen.WorkingArea.Width * 0.9, screen.WorkingArea.Height * 0.9);
                            }
                            SetFrame(lastFrame, true);
                            Position = new PixelPoint((int)lastFrame.X, (int)lastFrame.Y);
                            //}
                        }
                        validFrame = true;
                        //Zoom(this);
                        break;
                    case WindowState.Minimized:
                        //if ((root as Window).IsFullScreen)
                        //{
                        //    setFullScreem = true;
                        //    (root as Window).IsFullScreen = false;
                        //    setFullScreem = false;
                        //}
                        Miniaturize(this);
                        break;
                    case WindowState.Maximized:
                        if (IsMiniaturized)
                        {
                            Deminiaturize(this);
                        }
                        //PerformZoom(this);有窗体边框才行
                        lastFrame = Frame;
                        windowState = WindowState.Maximized;
                        if (WindowStateChanged != null)
                        {
                            WindowStateChanged();
                        }
                        Position = new PixelPoint();
                        if (base.Screen != null)
                        {
                            validFrame = true;
                            SetFrame(base.Screen.VisibleFrame, true);
                        }
                        else
                        {
                            validFrame = false;
                            SetFrame(NSScreen.MainScreen.VisibleFrame, true);
                        }
                        break;
                    case WindowState.FullScreen:
                        if (windowState != WindowState.Minimized)
                        {
                            validFrame = base.Screen != null;
                            lastFrame = Frame;
                        }
                        windowState = WindowState.FullScreen;
                        if (WindowStateChanged != null)
                        {
                            WindowStateChanged();
                        }
                        CollectionBehavior = NSWindowCollectionBehavior.FullScreenPrimary;
                        //base.StyleMask = NSWindowStyle.FullScreenWindow;
                        ToggleFullScreen(this);
                        break;
                }
            }
        }

        //public override void SetFrame(CGRect frameRect, bool display)
        //{
        //    base.SetFrame(frameRect, display);
        //    var w = WindowState.Normal;
        //    if (IsMiniaturized)
        //    {
        //        w = WindowState.Minimized;
        //    }
        //    else if (IsZoomed)
        //    {
        //        w = WindowState.Maximized;
        //    }
        //    if (w != windowState)
        //    {
        //        windowState = w;
        //        if (WindowStateChanged != null)
        //        {
        //            WindowStateChanged();
        //        }
        //    }
        //    if (Resized != null)
        //    {
        //        Resized(new Size((float)frameRect.Width, (float)frameRect.Height));
        //    }
        //}

        public Action WindowStateChanged { get; set; }

        public new Screen Screen { get { return MacPlatform.GetScreen(base.Screen); } }

        public bool CanResize { get; set; }
        public bool IsMain { get; set; }

        public float RenderScaling { get { return (float)BackingScaleFactor * Application.BaseScale; } }

        public Action ScalingChanged { get; set; }
        public Action<Size> Resized { get; set; }
        public Action<PixelPoint> PositionChanged { get; set; }
        public Action Activated { get; set; }
        public Action Deactivated { get; set; }
        public bool CanActivate { get; set; } = true;

        public void SetIMEEnable(bool value)
        {
            if (value)
            {
                this.view.InputContext.Activate();
            }
            else
            {
                //view.InputContext.Deactivate();
                this.view.InputContext.SelectedKeyboardInputSource = "com.apple.keylayout.ABC";
            }
        }

        public float LayoutScaling => Application.BaseScale;

        public PixelPoint Position
        {
            get
            {
                var sc = Screen;
                var h = sc.Bounds.Height;
                var s = root.ActualSize;
                var f = Frame;
                return new PixelPoint((int)f.X, (int)(h - (s.Height * LayoutScaling + f.Y)));
            }
            set
            {
                var sc = Screen;
                var h = sc.Bounds.Height;
                var s = root.ActualSize;
                this.SetFrame(new CGRect(value.X, h - (s.Height * LayoutScaling + value.Y), s.Width * LayoutScaling, s.Height * LayoutScaling), root.Visibility == Visibility.Visible);
                if (oldPoint != Frame.Location)
                {
                    WindowImpl_DidMoved(null, null);
                }
            }
        }

        public void Activate()
        {
            if (CanActivate)
            {
                MakeKeyAndOrderFront(this);
            }
            else
            {
                OrderFront(this);
            }
        }

        public override void ResignKeyWindow()
        {
            base.ResignKeyWindow();
            if (Deactivated != null)
            {
                Deactivated();
            }
        }

        public override void BecomeKeyWindow()
        {
            base.BecomeKeyWindow();
            if (Activated != null)
            {
                Activated();
            }
        }

        public void Capture()
        {
            //throw new NotImplementedException();
        }

        //bool dragMove;
        //CGPoint dragPos;
        //public void DragMove()
        //{
        //    //MovableByWindowBackground = true;
        //    dragMove = true;
        //    dragPos = NSEvent.CurrentMouseLocation;
        //}


        public void Invalidate(in Rect rect)
        {
            var r = new Rect(rect.X * LayoutScaling, rect.Y * LayoutScaling, rect.Width * LayoutScaling, rect.Height * LayoutScaling);
            view.SetNeedsDisplayInRect(new CGRect(r.X - 1, view.Bounds.Height - r.Height - r.Y - 1, r.Width + 2, r.Height + 2));
        }

        public Point PointToScreen(Point point)
        {
            var p = ConvertBaseToScreen(new CGPoint(point.X * LayoutScaling, view.Bounds.Height - point.Y * LayoutScaling));
            var pp = new Point((float)p.X, Screen.Bounds.Height - (float)p.Y);
            //System.Diagnostics.Debug.WriteLine(pp);
            return pp;
        }

        public void ReleaseCapture()
        {
            //throw new NotImplementedException();
        }

        NSCursor cursor;
        public void SetCursor(Cursor cursor)
        {
            var cu = cursor.PlatformCursor as NSCursor;
            if (cu != this.cursor)
            {
                this.cursor = cu;
                UpdateCursor();
            }
            //Debug.WriteLine(cu);
        }

        internal void UpdateCursor()
        {
            if (cursor != null)
            {
                //view.ResetCursorRects();
                //view.AddCursorRect(Frame, cursor);
                cursor.Set();
            }
        }

        public void SetIcon(Image image)
        {

        }

        internal Point imePoint;
        public void SetIMEPosition(Point point)
        {
            imePoint = point;
        }

        public void SetRoot(CPF.Controls.View view)
        {
            root = view;
            root.LayoutUpdated += Root_LayoutUpdated;
        }
        // bool isFirst = true;
        private void Root_LayoutUpdated(object sender, RoutedEventArgs e)
        {
            var b = this.Frame;
            var s = root.ActualSize;
            var l = root.ActualOffset;
            var src = root.Screen;
            var scaling = LayoutScaling;
            var h = src.Bounds.Height;
            if ((int)b.Width != (int)(s.Width * scaling) || (int)b.Height != (int)(s.Height * scaling) || (int)b.X != (int)(l.X * scaling) || (int)b.Y != (int)((h - (s.Height + l.Y) * scaling - src.WorkingArea.Y)))
            {
                //Bounds = new Rect(l.X * scaling, l.Y * scaling, s.Width, s.Height);
                this.SetFrame(new CGRect(l.X * scaling + src.WorkingArea.X, h - (s.Height + l.Y) * scaling - src.WorkingArea.Y, s.Width * scaling, s.Height * scaling), root.Visibility == Visibility.Visible);
                //Console.WriteLine(l + "," + s + "   " + Frame + "   " + src.WorkingArea);
                if (oldPoint != Frame.Location)
                {
                    WindowImpl_DidMoved(null, null);
                }
            }
            //Debug.WriteLine("Root_LayoutUpdated" + s);
        }

        public void SetTitle(string text)
        {
            base.Title = text;
            // base.TitleVisibility = NSWindowTitleVisibility.Visible;
        }

        internal bool isMainVisible;
        public void SetVisible(bool visible)
        {
            if (visible)
            {
                isMainVisible = false;
                if (CanActivate)
                {
                    MakeKeyAndOrderFront(this);
                    //var window = new NSWindow(new CGRect(0, 0, 200, 200), NSWindowStyle.Borderless, NSBackingStore.Buffered, false);
                    //window.ContentView = new GLView(window.Frame, new NSOpenGLPixelFormat());
                    //window.MakeKeyAndOrderFront(window);
                }
                else
                {
                    OrderFront(this);
                }
                root.LayoutManager.ExecuteLayoutPass();
                root.Visibility = Visibility.Visible;
            }
            else
            {
                OrderOut(this);
                isMainVisible = IsMain;
            }
        }

        //Window parent;
        public void ShowDialog(Window window)
        {
            //if (window.ViewImpl != this)
            //{
            //    //window.IsEnabled = false;
            //    (window.ViewImpl as WindowImpl).enable = false;
            //    parent = window;
            //}
            //ReleasedWhenClosed = true;
            this.Level = NSWindowLevel.ModalPanel;
            SetVisible(true);
        }

        public void ShowInTaskbar(bool value)
        {
            //TitleVisibility = value ? NSWindowTitleVisibility.Visible : NSWindowTitleVisibility.Hidden;
        }

        public void TopMost(bool value)
        {
            Level = value ? NSWindowLevel.Floating : NSWindowLevel.Normal;
        }

        //public override void MouseUp(NSEvent theEvent)
        //{
        //    //dragMove = false;
        //    //rs = RS.none;
        //    base.MouseUp(theEvent);
        //    //view.mouseDownCanMoveWindow = false;
        //    modifiers ^= InputModifiers.LeftMouseButton;
        //    //System.Diagnostics.Debug.WriteLine("MouseUp");
        //    var mouseButton = Mouse(theEvent);
        //    root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y), 0, root.InputManager.MouseDevice, mouseButton), root.LayoutManager.VisibleUIElements, EventType.MouseUp);
        //}

        InputModifiers modifiers = InputModifiers.None;
        MouseButton Mouse(NSEvent theEvent)
        {
            SetModifier();
            MouseButton mouseButton = MouseButton.Left;
            switch (theEvent.Type)
            {
                case NSEventType.LeftMouseDown:
                case NSEventType.LeftMouseUp:
                    mouseButton = MouseButton.Left;
                    break;
                case NSEventType.RightMouseDown:
                case NSEventType.RightMouseUp:
                    mouseButton = MouseButton.Right;
                    break;
                case NSEventType.MouseMoved:
                    break;
                case NSEventType.LeftMouseDragged:
                    break;
                case NSEventType.RightMouseDragged:
                    break;
                case NSEventType.MouseEntered:
                    break;
                case NSEventType.MouseExited:
                    break;
                case NSEventType.KeyDown:
                    break;
                case NSEventType.KeyUp:
                    break;
                case NSEventType.FlagsChanged:
                    break;
                case NSEventType.AppKitDefined:
                    break;
                case NSEventType.SystemDefined:
                    break;
                case NSEventType.ApplicationDefined:
                    break;
                case NSEventType.Periodic:
                    break;
                case NSEventType.CursorUpdate:
                    break;
                case NSEventType.ScrollWheel:
                    break;
                case NSEventType.TabletPoint:
                    break;
                case NSEventType.TabletProximity:
                    break;
                case NSEventType.OtherMouseDown:
                case NSEventType.OtherMouseUp:
                    mouseButton = MouseButton.Middle;
                    break;
                case NSEventType.OtherMouseDragged:
                    break;
                case NSEventType.Gesture:
                    break;
                case NSEventType.Magnify:
                    break;
                case NSEventType.Swipe:
                    break;
                case NSEventType.Rotate:
                    break;
                case NSEventType.BeginGesture:
                    break;
                case NSEventType.EndGesture:
                    break;
                case NSEventType.SmartMagnify:
                    break;
                case NSEventType.QuickLook:
                    break;
                default:
                    break;
            }

            return mouseButton;
        }

        internal void SetModifier()
        {
            var m = NSEvent.CurrentModifierFlags;
            if (modifiers.HasFlag(InputModifiers.Shift))
            {
                modifiers ^= InputModifiers.Shift;
            }
            if (modifiers.HasFlag(InputModifiers.Alt))
            {
                modifiers ^= InputModifiers.Alt;
            }
            if (modifiers.HasFlag(InputModifiers.Control))
            {
                modifiers ^= InputModifiers.Control;
            }
            if (modifiers.HasFlag(InputModifiers.Windows))
            {
                modifiers ^= InputModifiers.Windows;
            }

            switch (m)
            {
                case NSEventModifierMask.AlphaShiftKeyMask:
                    break;
                case NSEventModifierMask.ShiftKeyMask:
                    modifiers |= InputModifiers.Shift;
                    break;
                case NSEventModifierMask.ControlKeyMask:
                    modifiers |= InputModifiers.Control;
                    break;
                case NSEventModifierMask.AlternateKeyMask:
                    modifiers |= InputModifiers.Alt;
                    break;
                case NSEventModifierMask.CommandKeyMask:
                    modifiers |= InputModifiers.Windows;
                    break;
                case NSEventModifierMask.NumericPadKeyMask:
                    break;
                case NSEventModifierMask.HelpKeyMask:
                    break;
                case NSEventModifierMask.FunctionKeyMask:
                    break;
                case NSEventModifierMask.DeviceIndependentModifierFlagsMask:
                    break;
                default:
                    break;
            }
            root.InputManager.KeyboardDevice.Modifiers = modifiers;
        }


        //public override void MouseDown(NSEvent theEvent)
        //{
        //    base.MouseDown(theEvent);
        //    modifiers |= InputModifiers.LeftMouseButton;
        //    var mouseButton = Mouse(theEvent);
        //    //Debug.WriteLine(modifiers);
        //    root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y), 0, root.InputManager.MouseDevice, mouseButton), root.LayoutManager.VisibleUIElements, EventType.MouseDown);
        //}

        //public override void MouseDragged(NSEvent theEvent)
        //{
        //    base.MouseDragged(theEvent);
        //    var mouseButton = Mouse(theEvent);
        //    root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y), 0, root.InputManager.MouseDevice), root.LayoutManager.VisibleUIElements, EventType.MouseMove);
        //}
        //public override void RightMouseDown(NSEvent theEvent)
        //{
        //    base.RightMouseDown(theEvent);
        //    modifiers |= InputModifiers.RightMouseButton;
        //    var mouseButton = Mouse(theEvent);
        //    root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y), 0, root.InputManager.MouseDevice, mouseButton), root.LayoutManager.VisibleUIElements, EventType.MouseDown);

        //}
        //public override void RightMouseUp(NSEvent theEvent)
        //{
        //    base.RightMouseUp(theEvent);
        //    modifiers ^= InputModifiers.RightMouseButton;
        //    var mouseButton = Mouse(theEvent);
        //    root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y), 0, root.InputManager.MouseDevice, mouseButton), root.LayoutManager.VisibleUIElements, EventType.MouseUp);

        //}
        //public override void RightMouseDragged(NSEvent theEvent)
        //{
        //    base.RightMouseDragged(theEvent);
        //    var mouseButton = Mouse(theEvent);
        //    root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y), 0, root.InputManager.MouseDevice), root.LayoutManager.VisibleUIElements, EventType.MouseMove);
        //}
        //public override void OtherMouseDown(NSEvent theEvent)
        //{
        //    base.OtherMouseDown(theEvent);
        //    modifiers |= InputModifiers.MiddleMouseButton;
        //    var mouseButton = Mouse(theEvent);
        //    root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y), 0, root.InputManager.MouseDevice, mouseButton), root.LayoutManager.VisibleUIElements, EventType.MouseDown);

        //}
        //public override void OtherMouseUp(NSEvent theEvent)
        //{
        //    base.OtherMouseUp(theEvent);
        //    modifiers ^= InputModifiers.MiddleMouseButton;
        //    var mouseButton = Mouse(theEvent);
        //    root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y), 0, root.InputManager.MouseDevice, mouseButton), root.LayoutManager.VisibleUIElements, EventType.MouseUp);
        //}
        //public override void OtherMouseDragged(NSEvent theEvent)
        //{
        //    base.OtherMouseDragged(theEvent);
        //    var mouseButton = Mouse(theEvent);
        //    root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y), 0, root.InputManager.MouseDevice), root.LayoutManager.VisibleUIElements, EventType.MouseMove);
        //}

        //public override void MouseMoved(NSEvent theEvent)
        //{
        //    base.MouseMoved(theEvent);
        //    var mouseButton = Mouse(theEvent);
        //    //System.Diagnostics.Debug.WriteLine(modifiers);
        //    root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y), 0, root.InputManager.MouseDevice), root.LayoutManager.VisibleUIElements, EventType.MouseMove);
        //    if (root.Children.FirstOrDefault(a => a.IsMouseOver) == null)
        //    {
        //        (root.Cursor.PlatformCursor as NSCursor).Set();
        //    }
        //}
        //bool isMouseEnter;
        //public override void MouseExited(NSEvent theEvent)
        //{
        //    //isMouseEnter = false;
        //    base.MouseExited(theEvent);
        //    var mouseButton = Mouse(theEvent);
        //    root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y), 0, root.InputManager.MouseDevice), root.LayoutManager.VisibleUIElements, EventType.MouseLeave);
        //}
        public override void MouseEntered(NSEvent theEvent)
        {
            base.MouseEntered(theEvent);
            //isMouseEnter = true;
        }
        public override void ScrollWheel(NSEvent theEvent)
        {
            base.ScrollWheel(theEvent);
            if (!enable)
            {
                return;
            }
            var mouseButton = Mouse(theEvent);
            //Debug.WriteLine(theEvent.ScrollingDeltaY);
            root.InputManager.MouseDevice.ProcessEvent(new MouseWheelEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), new Point((float)theEvent.LocationInWindow.X, (float)view.Bounds.Height - (float)theEvent.LocationInWindow.Y) / LayoutScaling, root.InputManager.MouseDevice, new Vector((float)(100 * theEvent.DeltaX), (float)(100 * theEvent.DeltaY))), root.LayoutManager.VisibleUIElements, EventType.MouseWheel);
        }

        public override void KeyDown(NSEvent theEvent)
        {
            base.KeyDown(theEvent);
            //root.LayoutManager.ExecuteLayoutPass();
            //if (!string.IsNullOrEmpty(theEvent.Characters))
            //{
            //    root.InputManager.KeyboardDevice.ProcessEvent(new TextInputEventArgs(root, root.InputManager.KeyboardDevice, theEvent.Characters), KeyEventType.TextInput);
            //}
            ////System.Diagnostics.Debug.WriteLine(theEvent.Characters);
            //Keys k;
            //s_KeyMap.TryGetValue(theEvent.KeyCode, out k);
            //SetModifier();
            //root.InputManager.KeyboardDevice.ProcessEvent(new KeyEventArgs(root, k, root.InputManager.KeyboardDevice.Modifiers, root.InputManager.KeyboardDevice), KeyEventType.KeyDown);
        }

        public override void KeyUp(NSEvent theEvent)
        {
            base.KeyUp(theEvent);
            //Debug.WriteLine(theEvent.Characters);
            if (!enable)
            {
                return;
            }
            Keys k;
            Key.s_KeyMap.TryGetValue(theEvent.KeyCode, out k);
            //Console.WriteLine(k + " " + theEvent.KeyCode);
            SetModifier();
            var plat = Application.GetRuntimePlatform() as MacPlatform;
            plat.CurrentKeyEvent = theEvent;
            root.InputManager.KeyboardDevice.ProcessEvent(new KeyEventArgs(root, k, theEvent.KeyCode, root.InputManager.KeyboardDevice.Modifiers, root.InputManager.KeyboardDevice), KeyEventType.KeyUp);

            plat.CurrentKeyEvent = null;
        }
        public override bool PerformKeyEquivalent(NSEvent theEvent)
        {
            if (!enable)
            {
                return true;
            }
            Keys k;
            Key.s_KeyMap.TryGetValue(theEvent.KeyCode, out k);
            if (k == Keys.Back)
            {
                return true;
            }
            //Console.WriteLine(k + " " + theEvent.Characters);
            //Console.WriteLine((int)theEvent.CharactersIgnoringModifiers[0] + "  " + (int)theEvent.Characters[0]);
            //Console.WriteLine(root.InputManager.KeyboardDevice.FocusedElement.ToString() + theEvent.KeyCode);
            SetModifier();
            var plat = Application.GetRuntimePlatform() as MacPlatform;
            plat.CurrentKeyEvent = theEvent;
            root.InputManager.KeyboardDevice.ProcessEvent(new KeyEventArgs(root, k, theEvent.KeyCode, root.InputManager.KeyboardDevice.Modifiers, root.InputManager.KeyboardDevice), KeyEventType.KeyDown);
            plat.CurrentKeyEvent = null;
            //if (!string.IsNullOrEmpty(theEvent.Characters))
            //{
            //    if (!MacPlatform.keyValuePairs.ContainsKey(new KeyGesture(k, root.InputManager.KeyboardDevice.Modifiers)))
            //    {
            //        var uc = char.GetUnicodeCategory(theEvent.Characters[0]);
            //        if (uc != System.Globalization.UnicodeCategory.Control && uc != System.Globalization.UnicodeCategory.PrivateUse)
            //        {
            //            root.InputManager.KeyboardDevice.ProcessEvent(new TextInputEventArgs(root, root.InputManager.KeyboardDevice, theEvent.Characters), KeyEventType.TextInput);
            //        }
            //    }
            //}
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            AppDelegate.windows.Remove(this);
            base.Dispose(disposing);
        }

        public Point PointToClient(Point point)
        {
            var p = base.ConvertScreenToBase(new CGPoint(point.X, Screen.Bounds.Height - point.Y));
            return new Point((float)p.X, (float)view.Bounds.Height - (float)p.Y) / LayoutScaling;
        }

        //bool setFullScreem;
        //public void SetFullscreen(bool fullscreen)
        //{
        //    if (fullscreen)
        //    {
        //        if (windowState != WindowState.Minimized)
        //        {
        //            lastFrame = Frame;
        //        }
        //        if (!setFullScreem)
        //        {
        //            windowState = WindowState.Maximized;
        //            if (WindowStateChanged != null)
        //            {
        //                WindowStateChanged();
        //            }
        //        }
        //        CollectionBehavior = NSWindowCollectionBehavior.FullScreenPrimary;
        //        //base.StyleMask = NSWindowStyle.FullScreenWindow;
        //        ToggleFullScreen(this);
        //    }
        //    else
        //    {
        //        if (!setFullScreem)
        //        {
        //            windowState = WindowState.Normal;
        //            if (WindowStateChanged != null)
        //            {
        //                WindowStateChanged();
        //            }
        //        }
        //        IsZoomed = false;
        //        ToggleFullScreen(this);
        //        CollectionBehavior = NSWindowCollectionBehavior.Default;
        //        Position = new PixelPoint((int)lastFrame.X, (int)lastFrame.Y);
        //        SetFrame(lastFrame, true);
        //    }
        //}
        bool enable = true;
        public void SetEnable(bool enable)
        {
            this.enable = enable;
        }
    }


    //[Register("NSTextInputClient", false)]
    class InnerView : NSView, NotMonoMac, NSTextInputClient
    {
        public InnerView(CGRect rect, WindowImpl window) : base(rect)
        {

            //AddSubview(new NSText(new CGRect(10, 10, 100, 100)) {BackgroundColor=NSColor.FromCGColor(new CGColor(0,0,0,0)), TextColor= NSColor.FromCGColor(new CGColor(0, 0, 0, 0)), CanDrawConcurrently=false });
            this.window = window;
            RegisterForDraggedTypes(new string[] { NSPasteboard.NSPictType, NSPasteboard.NSStringType, NSPasteboard.NSFilenamesType, NSPasteboard.NSHtmlType });
        }

        public override void MouseMoved(NSEvent theEvent)
        {
            base.MouseMoved(theEvent);

            // Debug.WriteLine("MouseMoved" + window);
        }

        public override void MouseDown(NSEvent theEvent)
        {
            InputContext.HandleEvent(theEvent);
        }

        public override void KeyDown(NSEvent theEvent)
        {
            base.KeyDown(theEvent);
            //Keys k;
            //Key.s_KeyMap.TryGetValue(theEvent.KeyCode, out k);
            //if (k == Keys.Back || k == Keys.Tab || k == Keys.Enter)
            //{
            //    return;
            //}
            var r = InputContext.HandleEvent(theEvent);
            //Debug.WriteLine(r);
        }

        public override void MouseDragged(NSEvent theEvent)
        {
            InputContext.HandleEvent(theEvent);
        }
        public override void MouseUp(NSEvent theEvent)
        {
            InputContext.HandleEvent(theEvent);
        }
        WindowImpl window;
        //long bounds;
        //NSTrackingArea bounds;
        //public override void ViewDidMoveToWindow()
        //{
        //    base.ViewDidMoveToWindow();
        //    bounds = new NSTrackingArea(Bounds, NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.ActiveInKeyWindow, this, new NSDictionary(""));
        //    AddTrackingArea(bounds);
        //}

        //public override void UpdateTrackingAreas()
        //{
        //    base.UpdateTrackingAreas();
        //    RemoveTrackingArea(bounds);
        //    bounds = new NSTrackingArea(Bounds, NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.ActiveInKeyWindow, this, new NSDictionary(""));
        //    AddTrackingArea(bounds);

        //}
        public override void SetFrameSize(CGSize newSize)
        {
            base.SetFrameSize(newSize);
            //var w = WindowState.Normal;
            //if (window.IsMiniaturized)
            //{
            //    w = WindowState.Minimized;
            //}
            //else if (window.IsZoomed)
            //{
            //    w = WindowState.Maximized;
            //}
            //if (w != window.windowState)
            //{
            //    window.windowState = w;
            //    if (window.WindowStateChanged != null)
            //    {
            //        window.WindowStateChanged();
            //    }
            //}
            window.UpdateCursor();
            if (window.Resized != null)
            {
                window.Resized(new Size((float)newSize.Width, (float)newSize.Height) / window.LayoutScaling);
                //Console.WriteLine(newSize);
            }
        }


        static CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
        Size oldSize;
        Bitmap bitmap;
        public override void DrawRect(CGRect dirtyRect)
        {
            if (window.root.IsDisposed)
            {
                return;
            }
            var scaling = (float)window.BackingScaleFactor;
            window.root.LayoutManager.ExecuteLayoutPass();
            var size = new Size((float)Bounds.Width, (float)Bounds.Height) * scaling;
            if (bitmap == null || oldSize != size)
            {
                oldSize = size;
                if (bitmap != null)
                {
                    bitmap.Dispose();
                }
                if (oldSize.Width <= 0 || oldSize.Height <= 0)
                {
                    return;
                }
                bitmap = new Bitmap((int)oldSize.Width, (int)oldSize.Height);
            }
            var context = NSGraphicsContext.CurrentContext.GraphicsPort;

            using (var dc = DrawingContext.FromBitmap(bitmap))
            {
                //dc.Clear(Color.Transparent);
                //var m = dc.Transform;
                //m.ScalePrepend(window.RenderScaling, window.RenderScaling);
                //dc.Transform = m;
                if (window.root.LayoutManager.VisibleUIElements != null)
                {
                    var b = new Rect((float)dirtyRect.X * scaling, ((float)Bounds.Height - (float)dirtyRect.Height - (float)dirtyRect.Y) * scaling, (float)dirtyRect.Width * scaling, (float)dirtyRect.Height * scaling);
                    //Console.WriteLine(b);
                    window.root.RenderView(dc, b);
                }
            }
            using (var lk = bitmap.Lock())
            {
                using (CGBitmapContext bitmapContext = new CGBitmapContext(lk.DataPointer, (int)(bitmap.Width), (int)(bitmap.Height), 8, (int)(bitmap.Width * 4), colorSpace, CGImageAlphaInfo.PremultipliedLast))
                {
                    using (CGImage img = bitmapContext.ToImage())
                    {
                        context.DrawImage(new CGRect(0, 0, bitmap.Width / scaling, bitmap.Height / scaling), img);
                    }
                }
            }


        }
        public override bool CanBecomeKeyView
        {
            get
            {
                return true;
            }
        }


        public override bool AcceptsFirstResponder()
        {
            return true;
        }

        public override bool BecomeFirstResponder()
        {
            return true;
        }
        public override bool ResignFirstResponder()
        {
            return true;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
        }
        public static NSDragOperation ConvertDropEffect(DragDropEffects operation)
        {
            NSDragOperation result = NSDragOperation.None;
            if (operation.HasFlag(DragDropEffects.Copy))
                result |= NSDragOperation.Copy;
            if (operation.HasFlag(DragDropEffects.Move))
                result |= NSDragOperation.Move;
            if (operation.HasFlag(DragDropEffects.Link))
                result |= NSDragOperation.Link;
            return result;
        }

        public static DragDropEffects ConvertDropEffect(NSDragOperation effect)
        {
            DragDropEffects result = DragDropEffects.None;
            if (effect.HasFlag(NSDragOperation.Copy))
                result |= DragDropEffects.Copy;
            if (effect.HasFlag(NSDragOperation.Move))
                result |= DragDropEffects.Move;
            if (effect.HasFlag(NSDragOperation.Link))
                result |= DragDropEffects.Link;
            return result;
        }
        private Point GetDragLocation(CGPoint dragPoint)
        {
            return new Point((float)dragPoint.X, (float)(Frame.Height - dragPoint.Y));
        }
        IDataObject _currentDrag;
        public override NSDragOperation DraggingEntered(NSDraggingInfo sender)
        {
            _currentDrag = new DataObject(sender.DraggingPasteboard);
            //_currentDrag.Get(DataFormat.FileNames);
            var effect = window.root.InputManager.DragDropDevice.DragEnter(new DragEventArgs(_currentDrag, GetDragLocation(sender.DraggingLocation), window.root) { DragEffects = ConvertDropEffect(sender.DraggingSourceOperationMask) }, window.root.LayoutManager.VisibleUIElements);
            return ConvertDropEffect(effect);
        }
        public override NSDragOperation DraggingUpdated(NSDraggingInfo sender)
        {
            _currentDrag = new DataObject(sender.DraggingPasteboard);
            var effect = window.root.InputManager.DragDropDevice.DragOver(new DragEventArgs(_currentDrag, GetDragLocation(sender.DraggingLocation), window.root) { DragEffects = ConvertDropEffect(sender.DraggingSourceOperationMask) }, window.root.LayoutManager.VisibleUIElements);
            return ConvertDropEffect(effect);
        }
        public override void DraggingExited(NSDraggingInfo sender)
        {
            window.root.InputManager.DragDropDevice.DragLeave(window.root.LayoutManager.VisibleUIElements);
            _currentDrag = null;
        }

        public override bool PerformDragOperation(NSDraggingInfo sender)
        {
            //Debug.WriteLine("PerformDragOperation");
            window.root.InputManager.DragDropDevice.Drop(new DragEventArgs(_currentDrag, GetDragLocation(sender.DraggingLocation), window.root) { DragEffects = ConvertDropEffect(sender.DraggingSourceOperationMask) }, window.root.LayoutManager.VisibleUIElements);
            return true;
            //return base.PerformDragOperation(sender);
        }

        [Export("doCommandBySelector:")]
        public virtual void DoCommandBySelector(Selector selector)
        {
            //Debug.WriteLine(selector.Name);
            if (selector.Name == "deleteBackward:")
            {
                window.SetModifier();
                window.root.InputManager.KeyboardDevice.ProcessEvent(new KeyEventArgs(window.root, Keys.Back, Key.kVK_Delete, window.root.InputManager.KeyboardDevice.Modifiers, window.root.InputManager.KeyboardDevice), KeyEventType.KeyDown);
            }
        }


        [Export("baselineDeltaForCharacterAtIndex:")]
        public virtual float GetBaselineDelta(uint charIndex)
        {
            throw new You_Should_Not_Call_base_In_This_Method();
        }

        [Export("characterIndexForPoint:")]
        public virtual uint GetCharacterIndex(CGPoint point)
        {
            //throw new You_Should_Not_Call_base_In_This_Method();
            return 0;
        }

        [Export("firstRectForCharacterRange:actualRange:")]
        public virtual CGRect GetFirstRect(NSRange characterRange, out NSRange actualRange)
        {
            actualRange = new NSRange(0, 0);
            return new CGRect(window.Frame.X + window.imePoint.X, window.Frame.Y + (window.Frame.Height - window.imePoint.Y - 15), 1, 10);
        }

        [Export("fractionOfDistanceThroughGlyphForPoint:")]
        public virtual float GetFractionOfDistanceThroughGlyph(CGPoint point)
        {
            return 0;
        }

        [Export("insertText:replacementRange:")]
        public virtual void InsertText(NSObject text, NSRange replacementRange)
        {
            //throw new You_Should_Not_Call_base_In_This_Method();
            //Console.WriteLine("输入法输出：" + text);
            window.root.InputManager.KeyboardDevice.ProcessEvent(new TextInputEventArgs(window.root, window.root.InputManager.KeyboardDevice, text.ToString()), KeyEventType.TextInput);
            InputContext.InvalidateCharacterCoordinates();
        }

        [Export("setMarkedText:selectedRange:replacementRange:")]
        public virtual void SetMarkedText(NSObject text, NSRange selectedRange, NSRange replacementRange)
        {
            InputContext.InvalidateCharacterCoordinates();
        }

        [Export("unmarkText")]
        public virtual void UnmarkText()
        {
            InputContext.DiscardMarkedText();
        }

        [Export("attributedSubstringForProposedRange:actualRange:")]
        public NSAttributedString GetAttributedSubstring(NSRange proposedRange, out NSRange actualRange)
        {
            actualRange = new NSRange(0, 0);
            return null;
        }

        public virtual bool HasMarkedText
        {
            [Export("hasMarkedText")]
            get
            {
                return false;
            }
        }

        public virtual NSRange MarkedRange
        {
            [Export("markedRange")]
            get
            {
                return new NSRange(0, 0);
            }
        }

        public virtual NSRange SelectedRange
        {
            [Export("selectedRange")]
            get
            {
                return new NSRange(0, 0);
            }
        }

        public virtual NSString[] ValidAttributesForMarkedText
        {
            [Export("validAttributesForMarkedText")]
            get
            {
                return new NSString[] { NSAttributedString.MarkedClauseSegmentAttributeName, NSAttributedString.GlyphInfo, };
            }
        }

        public virtual NSWindowLevel WindowLevel
        {
            [Export("windowLevel")]
            get
            {
                return window.Level;
            }
        }

        //[Export("attributedString")]
        //public NSAttributedString AttributedString => null;
    }

    public class PopImpl : WindowImpl, IPopupImpl
    {
        public PopImpl()
        {
            this.Level = NSWindowLevel.PopUpMenu;
        }

        //public override bool CanBecomeKeyWindow => CanActivate;
    }
}
