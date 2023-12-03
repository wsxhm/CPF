using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.Diagnostics;

namespace CPF.Input
{
    public class MouseDevice : InputDevice
    {
        public MouseDevice(InputManager inputManager) : base(inputManager)
        {
        }

        /// <summary>
        /// 鼠标位置屏幕像素坐标
        /// </summary>
        public static PixelPoint Location
        {
            get { return CPF.Platform.Application.GetRuntimePlatform().MousePosition; }
        }
        /// <summary>
        /// 获取相对该元素的鼠标坐标
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Point GetPosition(UIElement element)
        {
            if (element.Root == null)
            {
                return new Point();
            }

            List<UIElement> list = new List<UIElement>();
            list.Add(element);
            var parent = element.Parent;
            while (parent != null)
            {
                list.Add(parent);
                parent = parent.Parent;
            }
            list.Reverse();
            var ml = Location;
            Point point = new Point(ml.X - element.Root.Position.X, ml.Y - element.Root.Position.Y) / element.Root.LayoutScaling;
            foreach (var item in list)
            {
                if (item.IsRoot)
                {
                    continue;
                }
                Point l = item.ActualOffset;
                Point r = item.TransformPointInvert(point);
                point = new Point(r.X - l.X, r.Y - l.Y);
            }
            return point;
        }


        /// <summary>
        /// 被捕获的元素，被捕获的元素可以接受超出元素范围的鼠标事件
        /// </summary>
        public UIElement Captured
        {
            get;
            protected set;
        }
        /// <summary>
        /// 捕获元素
        /// </summary>
        /// <param name="control"></param>
        public virtual void Capture(UIElement control)
        {
            if (Captured != null)
            {
                Captured.SetReleaseMouseCapture();
            }
            Captured = control;
            if (control != null)
            {
                InputManager.Root.Capture();
            }
            else
            {
                InputManager.Root.ReleaseCapture();
            }
        }

        //private DateTime _lastClickTime;
        bool startTunnel = true;
        Point rootPoint;

        enum RS
        {
            none,
            l,
            lt,
            t,
            rt,
            r,
            rb,
            b,
            lb,
        }
        Point startOffset;
        Size startSize;
        RS resize;
        PixelPoint dragPos;
        PixelPoint lastPos;
        Point? lastMousePoint;
        public bool ProcessEvent(InputEventArgs e, VisibleUIElement element, EventType eventName)
        {
            if (eventName == EventType.MouseMove && (InputManager.Root.LayoutManager._toArrange.Count > 0 || InputManager.Root.LayoutManager._toMeasure.Count > 0))
            {
                return false;
            }
            MouseEventArgs me = e as MouseEventArgs;
            if (eventName == EventType.MouseMove || eventName == EventType.MouseDown || eventName == EventType.MouseUp)
            {
                var window = InputManager.Root as CPF.Controls.Window;
                if (window && window.IsDragMove)
                {
                    if (eventName == EventType.MouseUp && me.LeftButton == MouseButtonState.Released)
                    {
                        window.IsDragMove = false;
                        window.ReleaseMouseCapture();
                        var pos = window.Position;
                        var scr = window.Screen;
                        if (pos.Y < scr.WorkingArea.Y)
                        {
                            window.Position = new PixelPoint(pos.X, (int)scr.WorkingArea.Y);
                        }
                    }
                    else if (eventName == EventType.MouseMove)
                    {
                        if (lastPos != Location)
                        {
                            if (window.WindowState == Controls.WindowState.Maximized || window.WindowState == Controls.WindowState.FullScreen)
                            {
                                var left = me.Location.X;
                                var t = me.Location.Y;
                                var w = window.ActualSize.Width;
                                var percent = left / w;
                                window.WindowState = Controls.WindowState.Normal;
                                window.BeginInvoke(() =>
                                {
                                    var scr = window.Screen;
                                    var tt = (Location.Y - scr.WorkingArea.Y) / window.LayoutScaling - t;
                                    window.MarginTop = tt;
                                    window.MarginLeft = left - window.ActualSize.Width * percent;
                                    //Debug.WriteLine(window.MarginLeft);
                                    window.dragMove = Location;
                                    window.startOffset = new PixelPoint((int)((left - window.ActualSize.Width * percent) * window.LayoutScaling) + (int)scr.WorkingArea.X, (int)(tt * window.LayoutScaling) + (int)scr.WorkingArea.Y);
                                });
                            }
                            else
                            {
                                var d = window.dragMove;
                                var p = Location;
                                //var sc = window.LayoutScaling;
                                //window.MarginLeft = window.startOffset.X + (p.X - d.X) / sc;
                                //var top = Math.Max(0, window.startOffset.Y + (p.Y - d.Y) / sc);
                                //top = Math.Min((window.Screen.WorkingArea.Height / window.LayoutScaling) - (window.dragMove.Y / sc - window.startOffset.Y), top);
                                //window.MarginTop = top;
                                var scr = window.Screen;
                                var top = (int)Math.Min((scr.WorkingArea.Height + scr.WorkingArea.Y) - (window.dragMove.Y - window.startOffset.Y), window.startOffset.Y + (p.Y - d.Y));
                                var left = (int)(window.startOffset.X + (p.X - d.X));
                                //Debug.WriteLine(p + "---" + left + "," + top);
                                window.Position = new PixelPoint(left, top);
                            }
                        }
                    }
                }
                if (window && window.CanResize && window.WindowState == Controls.WindowState.Normal)
                {
                    var region = RS.none;
                    //dragPos = Location;
                    var pLocation = me.Location;
                    var rect = window.ActualSize;
                    var dragThickness = window.DragThickness;
                    if (pLocation.X < dragThickness && pLocation.Y < dragThickness)
                    {
                        region = RS.lt;
                        window.SetCursor(((Cursor)Cursors.TopLeftCorner));
                    }
                    else
                    if (pLocation.X > rect.Width - dragThickness && pLocation.Y < dragThickness)
                    {
                        region = RS.rt;
                        window.SetCursor(((Cursor)Cursors.TopRightCorner));
                    }
                    else
                    if (pLocation.X < dragThickness && pLocation.Y > rect.Height - dragThickness)
                    {
                        region = RS.lb;
                        window.SetCursor(((Cursor)Cursors.TopRightCorner));
                    }
                    else
                    if (pLocation.X > rect.Width - dragThickness && pLocation.Y > rect.Height - dragThickness)
                    {
                        region = RS.rb;
                        window.SetCursor(((Cursor)Cursors.TopLeftCorner));
                    }
                    else
                    if (pLocation.Y < dragThickness)
                    {
                        region = RS.t;
                        window.SetCursor(((Cursor)Cursors.SizeNorthSouth));
                    }
                    else
                    if (pLocation.Y > rect.Height - dragThickness)
                    {
                        region = RS.b;
                        window.SetCursor(((Cursor)Cursors.SizeNorthSouth));
                    }
                    else
                    if (pLocation.X < dragThickness)
                    {
                        region = RS.l;
                        window.SetCursor(((Cursor)Cursors.SizeWestEast));
                    }
                    else
                    if (pLocation.X > rect.Width - dragThickness)
                    {
                        region = RS.r;
                        window.SetCursor(((Cursor)Cursors.SizeWestEast));
                    }
                    if (eventName == EventType.MouseDown && me.LeftButton == MouseButtonState.Pressed)
                    {
                        resize = region;
                        if (resize != RS.none)
                        {
                            window.CaptureMouse();
                        }
                        startSize = window.ActualSize;
                        startOffset = window.ActualOffset;
                        dragPos = Location;
                    }
                    else if (eventName == EventType.MouseUp && me.LeftButton == MouseButtonState.Released)
                    {
                        if (resize != RS.none)
                        {
                            window.ReleaseMouseCapture();
                        }
                        resize = RS.none;
                    }
                    if (resize != RS.none)
                    {
                        if (eventName == EventType.MouseMove)
                        {
                            var p = Location;
                            var sca = window.LayoutScaling;
                            var root = window;
                            switch (resize)
                            {
                                case RS.rb:
                                    root.MarginLeft = root.ActualOffset.X;
                                    root.MarginTop = root.ActualOffset.Y;
                                    root.Height = Math.Max(1, startSize.Height + ((float)p.Y - (float)dragPos.Y) / sca);
                                    root.Width = Math.Max(1, startSize.Width + ((float)p.X - (float)dragPos.X) / sca);
                                    break;
                                case RS.rt:
                                    root.MarginLeft = root.ActualOffset.X;
                                    root.MarginTop = startOffset.Y + ((float)p.Y - (float)dragPos.Y) / sca;
                                    root.Height = Math.Max(1, startSize.Height + ((float)dragPos.Y - (float)p.Y) / sca);
                                    root.Width = Math.Max(1, startSize.Width + ((float)p.X - (float)dragPos.X) / sca);
                                    break;
                                case RS.lt:
                                    root.MarginLeft = startOffset.X + ((float)p.X - (float)dragPos.X) / sca;
                                    root.MarginTop = startOffset.Y + ((float)p.Y - (float)dragPos.Y) / sca;
                                    root.Height = Math.Max(1, startSize.Height + ((float)dragPos.Y - (float)p.Y) / sca);
                                    root.Width = Math.Max(1, startSize.Width + ((float)dragPos.X - (float)p.X) / sca);
                                    break;
                                case RS.lb:
                                    root.MarginLeft = startOffset.X + ((float)p.X - (float)dragPos.X) / sca;
                                    root.MarginTop = root.ActualOffset.Y;
                                    root.Width = Math.Max(1, startSize.Width + ((float)dragPos.X - (float)p.X) / sca);
                                    root.Height = Math.Max(1, startSize.Height + ((float)p.Y - (float)dragPos.Y) / sca);
                                    break;
                                case RS.b:
                                    root.MarginLeft = root.ActualOffset.X;
                                    root.MarginTop = root.ActualOffset.Y;
                                    root.Height = Math.Max(1, startSize.Height + ((float)p.Y - (float)dragPos.Y) / sca);
                                    break;
                                case RS.l:
                                    root.MarginLeft = startOffset.X + ((float)p.X - (float)dragPos.X) / sca;
                                    root.MarginTop = root.ActualOffset.Y;
                                    root.Width = Math.Max(1, startSize.Width + ((float)dragPos.X - (float)p.X) / sca);
                                    break;
                                case RS.t:
                                    root.MarginLeft = root.ActualOffset.X;
                                    root.MarginTop = startOffset.Y + ((float)p.Y - (float)dragPos.Y) / sca;
                                    root.Height = Math.Max(1, startSize.Height + ((float)dragPos.Y - (float)p.Y) / sca);
                                    break;
                                case RS.r:
                                    root.MarginLeft = root.ActualOffset.X;
                                    root.MarginTop = root.ActualOffset.Y;
                                    root.Width = Math.Max(1, startSize.Width + ((float)p.X - (float)dragPos.X) / sca);
                                    break;
                                default:
                                    break;
                            }
                        }

                        return true;
                    }

                    if (region != RS.none && Captured == null)
                    {//鼠标移到调整尺寸的边缘

                        if (eventName == EventType.MouseMove)
                        {
                            if (window.IsMouseOver)
                            {
                                MouseLeave(window, me);
                            }
                        }
                        return true;
                    }
                }

                if (eventName == EventType.MouseMove)
                {
                    lastPos = Location;
                }
            }


            startTunnel = true;
            if (element == null)
            {
                return false;
            }

            if ((eventName == EventType.MouseMove || eventName == EventType.MouseDown) && !element.Element.IsMouseOver)
            {
                //Debug.WriteLine("MouseEnter");
                var eee = e as MouseEventArgs;
                var ee = new MouseEventArgs(e.Device, (UIElement)e.OriginalSource, eee.LeftButton == MouseButtonState.Pressed, eee.RightButton == MouseButtonState.Pressed, eee.MiddleButton == MouseButtonState.Pressed, eee.Location, eee.IsTouch);
                element.Element.RaiseDeviceEvent(ee, EventType.MouseEnter);
            }
            rootPoint = me.Location;

            if (eventName == EventType.MouseDown)
            {
                (me as MouseButtonEventArgs).timestamp = DateTime.Now.ToBinary();
                //Debug.WriteLine((me as MouseButtonEventArgs).timestamp);
                element.Element.RaiseDeviceEvent(e, EventType.PreviewMouseDown);
            }
            else if (eventName == EventType.MouseUp)
            {
                element.Element.RaiseDeviceEvent(e, EventType.PreviewMouseUp);
            }
            HitTest(element, me.Location, me, eventName);
            //Debug.WriteLine(me.Location + " " + eventName);
            if (eventName == EventType.MouseMove && !e.Handled && me.IsTouch && lastMousePoint.HasValue)
            {
                //Debug.WriteLine(rootPoint + "---" + lastMousePoint);
                HitTest(element, rootPoint, new MouseWheelEventArgs(me.OriginalSource as UIElement, true, me.RightButton == MouseButtonState.Pressed, me.MiddleButton == MouseButtonState.Pressed, rootPoint, this, rootPoint - lastMousePoint.Value, true), EventType.MouseWheel);
            }
            if (me.IsTouch && (eventName == EventType.MouseDown || eventName == EventType.MouseMove))
            {
                lastMousePoint = rootPoint;
            }
            if (eventName == EventType.MouseUp && (e is MouseButtonEventArgs mb) && mb.MouseButton == MouseButton.Left)
            {
                Capture(null);
            }
            if (eventName == EventType.MouseUp || eventName == EventType.MouseLeave)
            {
                lastMousePoint = null;
            }
            return e.Handled;
        }

        void HitTest(VisibleUIElement element, Point p, MouseEventArgs m, EventType eventName)
        {
            HitTest(p, element, m, eventName);
            if (!m.Handled && element.Element != null)
            {
                element.Element.RaiseDeviceEvent(m, eventName);
            }
        }


        /// <summary>
        /// 命中测试可视范围的UI元素，返回被命中到的UI元素
        /// </summary>
        /// <param name="point"></param>
        /// <param name="element"></param>
        /// <param name="e"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        bool HitTest(Point point, VisibleUIElement element, MouseEventArgs e, EventType eventName)
        {
            bool isHit = false;
            if (element != null && element.Element != null)
            {
                for (int i = element.Children.Count - 1; i >= 0; i--)
                {
                    var item = element.Children[i];
                    if (item.Element.IsEnabled && item.Element.Visibility == Visibility.Visible && item.Element.IsHitTestVisible)
                    {
                        Point l = item.Element.ActualOffset;
                        Point r = item.Element.TransformPointInvert(point);
                        Point t = new Point(r.X - l.X, r.Y - l.Y);
                        if (Captured != null)
                        {
                            if (Captured == item.Element)
                            {
                                isHit = true;
                            }
                            else
                            {
                                isHit = HitTest(t, item, e, eventName);
                            }
                        }
                        else
                        {
                            isHit = item.Element.HitTestCore(r);
                            //Console.WriteLine("hit" + isHit + element.Element.ToString());
                            if (item.Element.ClipToBounds)//如果是裁剪了，需要先判断自己，再判断子元素
                            {
                                if (isHit)
                                {
                                    if (Captured == null)
                                    {
                                        e.OverrideSource(item.Element);
                                    }
                                    HitTest(t, item, e, eventName);
                                }
                            }
                            else
                            {
                                if (isHit && Captured == null)
                                {
                                    if (Captured == null)
                                    {
                                        e.OverrideSource(item.Element);
                                    }
                                }
                                isHit = HitTest(t, item, e, eventName) || isHit;
                            }
                        }
                        if (element.Element == null)
                        {
                            return false;
                        }
                        if (isHit)
                        {
                            if (item.Element != element.Element.mouseOverChild && element.Element.mouseOverChild != null)
                            {
                                //var ee = new MouseEventArgs(e.Device, (UIElement)e.OriginalSource, MouseEventType.MouseLeave, e.Location, e.Delta);
                                //ee.OverrideRoutedEvent(UIElement.MouseLeaveEvent);
                                //element.Element.mouseOverChild.RaiseEvent(ee);
                                MouseLeave(element.Element.mouseOverChild, e);
                            }
                            if (!item.Element.IsMouseOver)
                            {
                                element.Element.mouseOverChild = item.Element;
                                var ee = new MouseEventArgs(e.Device, (UIElement)e.OriginalSource, e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, e.MiddleButton == MouseButtonState.Pressed, e.Location, e.IsTouch);
                                item.Element.RaiseDeviceEvent(ee, EventType.MouseEnter);
                                //Console.WriteLine("MouseEnter" + e.OriginalSource);
                            }
                            e.Location = t;
                            //isHit = true;
                            if (startTunnel && (eventName == EventType.MouseDown || eventName == EventType.MouseUp))
                            {
                                startTunnel = false;
                                var ui = item.Element;
                                List<UIElement> list = new List<UIElement>();
                                while (ui != null && !ui.IsRoot)
                                {
                                    list.Add(ui);
                                    ui = ui.Parent;
                                }
                                var me = e as MouseButtonEventArgs;
                                var pe = new MouseButtonEventArgs(item.Element.Root, e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, e.MiddleButton == MouseButtonState.Pressed, new Point(), this, me.MouseButton, e.IsTouch);
                                Point p = rootPoint;
                                for (int j = list.Count - 1; j >= 0; j--)
                                {
                                    var ele = list[j];
                                    Point ll = ele.ActualOffset;
                                    Point rr = ele.TransformPointInvert(p);
                                    Point tt = new Point(rr.X - ll.X, rr.Y - ll.Y);
                                    pe.Location = tt;
                                    p = tt;
                                    if (!pe.Handled)
                                    {
                                        if (eventName == EventType.MouseUp)
                                        {
                                            ele.RaiseDeviceEvent(pe, EventType.PreviewMouseUp);
                                        }
                                        else
                                        {
                                            ele.RaiseDeviceEvent(pe, EventType.PreviewMouseDown);
                                        }
                                    }
                                }
                            }

                            if (!e.Handled)
                            {
                                item.Element.RaiseDeviceEvent(e, eventName);
                            }
                            break;
                        }
                    }
                }
                if (!isHit)
                {
                    if (element.Element.mouseOverChild != null)
                    {
                        //var ee = new MouseEventArgs(e.Device, (UIElement)e.OriginalSource, MouseEventType.MouseLeave, e.Location, e.Delta);
                        //ee.OverrideRoutedEvent(UIElement.MouseLeaveEvent);
                        //element.Element.mouseOverChild.RaiseEvent(ee);
                        //e.OverrideSource(element.Element.mouseOverChild);
                        MouseLeave(element.Element.mouseOverChild, e);
                        element.Element.mouseOverChild = null;
                    }
                }
            }
            return isHit;
        }

        internal void MouseLeave(UIElement element, MouseEventArgs e)
        {
            var el = element;
            while (el != null && !el.IsDisposed && el.IsMouseOver)
            {
                var ee = new MouseEventArgs(e.Device, null, e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, e.MiddleButton == MouseButtonState.Pressed, e.Location, e.IsTouch);
                el.RaiseDeviceEvent(ee, EventType.MouseLeave);
                el = el.mouseOverChild;
                //Console.WriteLine("MouseLeave" + e.OriginalSource);
            }
        }
    }

    public enum EventType : byte
    {
        MouseDown,
        MouseUp,
        MouseEnter,
        MouseLeave,
        MouseMove,
        MouseWheel,
        KeyDown,
        KeyUp,
        TextInput,
        //DoubleClick,
        PreviewMouseDown,
        PreviewMouseUp,
        DragEnter,
        DragLeave,
        DragOver,
        Drop,
        TouchDown,
        TouchUp,
        TouchMove,
        TouchEnter,
        TouchLeave
    }
}
