using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CPF.Input
{
    public class TouchDevice : InputDevice
    {
        public TouchDevice(InputManager inputManager) : base(inputManager)
        {
        }

        Dictionary<int, TouchPoint> points = new Dictionary<int, TouchPoint>();
        TouchPoint? lastPoint1;
        TouchPoint? lastPoint2;
        /// <summary>
        /// 获取相对于该元素的触摸点
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<TouchPoint> GetPositions(UIElement element)
        {
            if (element.Root == null)
            {
                return new TouchPoint[0];
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
            List<TouchPoint> touchPoints = new List<TouchPoint>();
            foreach (var p in points)
            {
                //var ml = p.Value.Position;
                //Point point = new Point(ml.X - element.Root.Position.X, ml.Y - element.Root.Position.Y) / element.Root.LayoutScaling;
                var point = p.Value.Position;
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
                touchPoints.Add(new TouchPoint { Id = p.Key, Position = point });
            }

            return touchPoints;
        }
        /// <summary>
        /// 清除记录的触摸点
        /// </summary>
        public void ClearPoints()
        {
            points.Clear();
        }

        public void ProcessEvent(TouchEventArgs e, VisibleUIElement element, EventType eventName)
        {
            if (eventName == EventType.TouchMove && (InputManager.Root.LayoutManager._toArrange.Count > 0 || InputManager.Root.LayoutManager._toMeasure.Count > 0))
            {
                return;
            }

            if (element == null)
            {
                return;
            }

            //if (eventName == EventType.TouchMove && points.TryGetValue(e.Position.Id, out var p) && p != e.Position)
            //{
            //    InputManager.MouseDevice.ProcessEvent(new MouseWheelEventArgs((UIElement)e.OriginalSource, true, false, false, e.Position.Position, InputManager.MouseDevice, e.Position.Position - p.Position, true), element, EventType.MouseWheel);
            //}

            //if (eventName == EventType.TouchDown)
            //{
            points[e.Position.Id] = e.Position;
            //}

            //if ((eventName == EventType.TouchMove || eventName == EventType.TouchDown) && !element.Element.IsMouseOver)
            //{
            //    //Debug.WriteLine("TouchEnter");
            //    var ee = new TouchEventArgs(e.Position, this, (UIElement)e.OriginalSource);
            //    element.Element.RaiseDeviceEvent(ee, EventType.TouchEnter);
            //}
            if (eventName == EventType.TouchMove)
            {
                e = new TouchMoveEventArgs(e.Position, e.TouchDevice, (UIElement)e.OriginalSource, new ManipulationDelta(new Vector(), 0, new Vector(0, 0)));
                if (points.Count > 1 && lastPoint2.HasValue && lastPoint1.HasValue && points.TryGetValue(lastPoint1.Value.Id, out var p1) && points.TryGetValue(lastPoint2.Value.Id, out var p2))
                {
                    var m = e as TouchMoveEventArgs;
                    m.DeltaManipulation.Rotation = Vector.AngleBetween(p2.Position - p1.Position, lastPoint2.Value.Position - lastPoint1.Value.Position);
                    m.DeltaManipulation.Translation = (p2.Position + new Vector(p1.Position.X, p1.Position.Y)) / 2 - (lastPoint2.Value.Position + new Vector(lastPoint1.Value.Position.X, lastPoint1.Value.Position.Y)) / 2;
                    var s = Vector.Distance(new Vector(p1.Position.X, p1.Position.Y), new Vector(p2.Position.X, p2.Position.Y)) / Vector.Distance(new Vector(lastPoint1.Value.Position.X, lastPoint1.Value.Position.Y), new Vector(lastPoint2.Value.Position.X, lastPoint2.Value.Position.Y));
                    m.DeltaManipulation.Scale = new Vector(s, s);
                    //System.Diagnostics.Debug.WriteLine(s);
                }
            }

            HitTest(element, e.Position, e, eventName);

            if (eventName == EventType.TouchUp)
            {
                points.Remove(e.Position.Id);
                if (lastPoint1.HasValue && lastPoint1.Value.Id == e.Position.Id)
                {
                    var f = points.FirstOrDefault(a => a.Key != lastPoint2.Value.Id);
                    if (f.Key != 0)
                    {
                        lastPoint1 = f.Value;
                    }
                    else
                    {
                        lastPoint1 = null;
                    }
                }
                if (lastPoint2.HasValue && lastPoint2.Value.Id == e.Position.Id)
                {
                    var f = points.FirstOrDefault(a => a.Key != lastPoint1.Value.Id);
                    if (f.Key != 0)
                    {
                        lastPoint2 = f.Value;
                    }
                    else
                    {
                        lastPoint2 = null;
                    }
                }
            }
            else
            {
                if (lastPoint1 == null || lastPoint1.Value.Id == e.Position.Id)
                {
                    lastPoint1 = e.Position;
                }
                else if (lastPoint2 == null || lastPoint2.Value.Id == e.Position.Id)
                {
                    lastPoint2 = e.Position;
                }
            }
        }

        void HitTest(VisibleUIElement element, TouchPoint p, TouchEventArgs m, EventType eventName)
        {
            HitTest(p, element, m, eventName);
            if (!m.Handled)
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
        bool HitTest(TouchPoint point, VisibleUIElement element, TouchEventArgs e, EventType eventName)
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
                        Point r = item.Element.TransformPointInvert(point.Position);
                        TouchPoint t = new TouchPoint { Position = new Point(r.X - l.X, r.Y - l.Y), Id = point.Id };
                        if (InputManager.MouseDevice.Captured != null)
                        {
                            if (InputManager.MouseDevice.Captured == item.Element)
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
                                    if (InputManager.MouseDevice.Captured == null)
                                    {
                                        e.OverrideSource(item.Element);
                                    }
                                    HitTest(t, item, e, eventName);
                                }
                            }
                            else
                            {
                                if (isHit && InputManager.MouseDevice.Captured == null)
                                {
                                    if (InputManager.MouseDevice.Captured == null)
                                    {
                                        e.OverrideSource(item.Element);
                                    }
                                }
                                isHit = (HitTest(t, item, e, eventName) || isHit);
                            }
                        }
                        if (isHit)
                        {
                            //if (item.Element != element.Element.mouseOverChild && element.Element.mouseOverChild != null)
                            //{
                            //    TouchLeave(element.Element.mouseOverChild, e);
                            //}
                            //if (!item.Element.IsMouseOver)
                            //{
                            //    element.Element.mouseOverChild = item.Element;
                            //    var ee = new TouchEventArgs(e.Position, e.Device, (UIElement)e.OriginalSource);
                            //    item.Element.RaiseDeviceEvent(ee, EventType.TouchEnter);
                            //    //Console.WriteLine("TouchEnter" + e.OriginalSource);
                            //}
                            e.Position = t;

                            if (!e.Handled)
                            {
                                item.Element.RaiseDeviceEvent(e, eventName);
                            }
                            break;
                        }
                    }
                }
                //if (!isHit)
                //{
                //    if (element.Element.mouseOverChild != null)
                //    {
                //        TouchLeave(element.Element.mouseOverChild, e);
                //        element.Element.mouseOverChild = null;
                //    }
                //}
            }
            return isHit;
        }

        //internal void TouchLeave(UIElement element, TouchEventArgs e)
        //{
        //    var el = element;
        //    while (el != null && !el.IsDisposed && el.IsMouseOver)
        //    {
        //        var ee = new TouchEventArgs(e.Position, e.Device, null);
        //        el.RaiseDeviceEvent(ee, EventType.TouchLeave);
        //        el = el.mouseOverChild;
        //        //Console.WriteLine("MouseLeave" + e.OriginalSource);
        //    }
        //}
    }
}
