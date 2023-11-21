using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Input
{
    public class DragDropDevice : InputDevice
    {
        public DragDropDevice(InputManager inputManager) : base(inputManager)
        {
        }

        public DragDropEffects DragEnter(DragEventArgs e, VisibleUIElement element)
        {
            if (e.DragEffects == DragDropEffects.Link && element.Element is CPF.Controls.View view && view.Type.FullName != "DeveloperTools.DeveloperToolWindow" && CPF.Platform.Application.AllowDeveloperTool)
            {
                var dea = e;
                if (dea.Data.Contains(DataFormat.Text))
                {
                    var text = dea.Data.GetData(DataFormat.Text) as string;
                    if (!string.IsNullOrWhiteSpace(text) && text.StartsWith("@*!devTool") && text.EndsWith("!*@"))
                    {
                        return DragDropEffects.Link;
                    }
                }
            }
            //Console.WriteLine("DragEnter" + e.DragEffects);
            HitTest(element, e.Location, e, EventType.DragEnter);
            //Console.WriteLine("DragEnter:" + e.DragEffects);
            return e.DragEffects;
        }
        public DragDropEffects DragOver(DragEventArgs e, VisibleUIElement element)
        {
            if (e.DragEffects == DragDropEffects.Link && element.Element is CPF.Controls.View view && view.Type.FullName != "DeveloperTools.DeveloperToolWindow" && CPF.Platform.Application.AllowDeveloperTool)
            {
                var dea = e;
                if (dea.Data.Contains(DataFormat.Text))
                {
                    var text = dea.Data.GetData(DataFormat.Text) as string;
                    if (!string.IsNullOrWhiteSpace(text) && text.StartsWith("@*!devTool") && text.EndsWith("!*@"))
                    {
                        return DragDropEffects.Link;
                    }
                }
            }
            //Console.WriteLine("DragOver" + e.DragEffects);
            HitTest(element, e.Location, e, EventType.DragOver);
            //Console.WriteLine("DragOver:" + e.DragEffects);
            return e.DragEffects;
        }
        public void DragLeave(VisibleUIElement element)
        {
            DragLeave(element.Element, EventArgs.Empty, element.Element.AllowDrop);
        }
        public DragDropEffects Drop(DragEventArgs e, VisibleUIElement element)
        {
            if (e.DragEffects == DragDropEffects.Link && element.Element is CPF.Controls.View view && view.Type.FullName != "DeveloperTools.DeveloperToolWindow" && CPF.Platform.Application.AllowDeveloperTool)
            {
                var dea = e;
                if (dea.Data.Contains(DataFormat.Text))
                {
                    var text = dea.Data.GetData(DataFormat.Text) as string;
                    if (!string.IsNullOrWhiteSpace(text) && text.StartsWith("@*!devTool") && text.EndsWith("!*@"))
                    {
                        var id = text.Substring(10, text.Length - 13);
                        view.ConnnetDev(id);
                        return DragDropEffects.Link;
                    }
                }
            }
            HitTest(element, e.Location, e, EventType.Drop);
            DragLeave(element.Element, EventArgs.Empty, element.Element.AllowDrop);
            //Console.WriteLine("Drop:" + e.DragEffects);
            return e.DragEffects;
        }

        void HitTest(VisibleUIElement element, Point p, DragEventArgs m, EventType eventName)
        {
            if (element != null)
            {
                var hit = HitTest(p, element, m, eventName, element.Element.AllowDrop).isHitAllow;
                if (!m.Handled && element.Element.AllowDrop)
                {
                    element.Element.RaiseDeviceEvent(m, eventName);
                }
                if (!hit && !element.Element.AllowDrop)
                {
                    m.DragEffects = DragDropEffects.None;
                }
            }
            else
            {
                m.DragEffects = DragDropEffects.None;
            }
        }


        /// <summary>
        /// 命中测试可视范围的UI元素，返回被命中到的UI元素
        /// </summary>
        /// <param name="point"></param>
        /// <param name="element"></param>
        /// <param name="e"></param>
        /// <param name="eventName"></param>
        /// <param name="allowDrop"></param>
        /// <returns></returns>
        (bool isHit, bool isHitAllow) HitTest(Point point, VisibleUIElement element, DragEventArgs e, EventType eventName, bool allowDrop = false)
        {
            bool isHitChild = false;
            var isHit = false;
            if (element != null && element.Element != null)
            {
                for (int i = element.Children.Count - 1; i >= 0; i--)
                {
                    var item = element.Children[i];
                    if (item.Element.IsEnabled && item.Element.Visibility == Visibility.Visible)
                    {
                        Point l = item.Element.ActualOffset;
                        Point r = item.Element.TransformPointInvert(point);
                        Point t = new Point(r.X - l.X, r.Y - l.Y);

                        var allow = allowDrop || item.Element.AllowDrop;
                        isHit = (isHit || item.Element.HitTestCore(r));
                        //Console.WriteLine("hit" + isHit + element.Element.ToString());
                        if (item.Element.ClipToBounds)//如果是裁剪了，需要先判断自己，再判断子元素
                        {
                            if (isHit)
                            {
                                e.OverrideSource(item.Element);

                                var result = HitTest(t, item, e, eventName, allow);
                                isHit = result.isHit || isHit;
                                isHitChild = isHitChild || result.isHitAllow;
                            }
                        }
                        else
                        {
                            if (isHit)
                            {
                                e.OverrideSource(item.Element);
                            }
                            var result = HitTest(t, item, e, eventName, allow);
                            isHit = result.isHit || isHit;
                            isHitChild = isHitChild || result.isHitAllow;
                        }
                        isHitChild = isHitChild || (isHit && allow);

                        if (isHit)
                        {
                            if (item.Element != element.Element.dragOverChild && element.Element.dragOverChild != null)
                            {
                                DragLeave(element.Element.dragOverChild, e, allow);
                            }
                            if (!item.Element.IsDragOver)
                            {
                                element.Element.dragOverChild = item.Element;
                                if (allow)
                                {
                                    var ee = new DragEventArgs(e.Data, e.Location, (UIElement)e.OriginalSource);
                                    item.Element.DragDropEffects = e.DragEffects;
                                    ee.DragEffects = e.DragEffects;
                                    item.Element.RaiseDeviceEvent(ee, EventType.DragEnter);
                                    item.Element.DragDropEffects = ee.DragEffects;
                                    e.DragEffects = ee.DragEffects;
                                }
                                //Console.WriteLine("DragEnter" + e.OriginalSource);
                            }
                            e.Location = t;

                            if (!e.Handled && allow && eventName != EventType.DragEnter)
                            {
                                e.DragEffects = item.Element.DragDropEffects;
                                item.Element.RaiseDeviceEvent(e, eventName);
                                item.Element.DragDropEffects = e.DragEffects;
                            }
                            break;
                        }
                    }
                }
                if (!isHit)
                {
                    if (element.Element.dragOverChild != null)
                    {
                        e.OverrideSource(element.Element.dragOverChild);
                        DragLeave(element.Element.dragOverChild, e, allowDrop);
                        element.Element.dragOverChild = null;
                    }
                }
            }
            return (isHit, isHitChild);
        }

        void DragLeave(UIElement element, EventArgs e, bool allowDrop = false)
        {
            var el = element;
            if (element != null)
            {
                allowDrop = element.AllowDrop || allowDrop;
            }
            while (el != null)
            {
                if ((allowDrop || el.AllowDrop) && el.IsDragOver)
                {
                    el.RaiseDeviceEvent(EventArgs.Empty, EventType.DragLeave);
                }
                el = el.dragOverChild;
            }
            //Console.WriteLine("DragLeave");
        }
    }
}
