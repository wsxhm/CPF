using System;
using System.Collections.Generic;
using System.Text;
using CPF.Threading;
using CPF.Drawing;
using System.Linq;
using CPF.Platform;
using CPF.Controls;

namespace CPF
{
    public class LayoutManager
    {
        View root;
        internal LayoutManager(View view)
        {
            this.root = view;
        }

        #region LayoutManage
        VisibleUIElement visibleElement;
        /// <summary>
        /// 在可视范围内的元素
        /// </summary>
        public VisibleUIElement VisibleUIElements { get { return visibleElement; } }
        internal readonly HashSet<UIElement> _toMeasure = new HashSet<UIElement>();
        internal readonly HashSet<UIElement> _toArrange = new HashSet<UIElement>();
        private bool _queued;
        private bool _running;

        internal readonly List<NativeElement> nativeElements = new List<NativeElement>();

        public void InvalidateMeasure(UIElement control)
        {
            //Contract.Requires<ArgumentNullException>(control != null);
            if (control.IsDisposed || control.IsDisposing)
            {
                return;
            }
            Dispatcher.MainThread.VerifyAccess();

            _toMeasure.Add(control);
            _toArrange.Add(control);
            QueueLayoutPass();
        }

        public void InvalidateArrange(UIElement control)
        {
            if (control.IsDisposed || control.IsDisposing)
            {
                return;
            }
            //Contract.Requires<ArgumentNullException>(control != null);
            Dispatcher.MainThread.VerifyAccess();

            _toArrange.Add(control);
            QueueLayoutPass();
        }

        /// <inheritdoc/>
        public void ExecuteLayoutPass()
        {
            Dispatcher.MainThread.VerifyAccess();

            if (!_running)
            {
                _running = true;

                var updateVisible = _toArrange.Count > 0 || _toMeasure.Count > 0;
                if (visibleElement == null && !updateVisible)
                {
                    _toArrange.Add(root);
                    root.viewRenderRect = true;
                    updateVisible = true;
                }
                //try
                //{
                while (true)
                {
                    ExecuteMeasurePass();
                    ExecuteArrangePass();

                    if (_toMeasure.Count == 0 && _toArrange.Count == 0)
                    {
                        break;
                    }
                }
                //}
                //finally
                //{
                _running = false;
                //}
                if (updateVisible)
                {
                    visibleElements.Clear();
                    Desdroy();
                    visibleElement = VisibleUIElement(root, new Rect(new Point(), root.ActualSize));
                    foreach (var item in nativeElements)
                    {
                        if (item.IsDisposed)
                        {
                            break;
                        }
                        var rect = GetRect(item.Parent, item.GetContentBounds(), item);
                        rect.Intersect(new Rect(new Point(), item.ActualSize));
                        item.NativeImpl.SetBounds(item.RenderBounds, rect, IsVisible(item));
                    }
                }
            }

            _queued = false;
        }


        private Rect GetRect(UIElement control, Rect rect, UIElement owner)
        {
            Rect r;
            if (control.Parent != null)
            {
                r = GetRect(control.Parent, control.GetContentBounds(), owner);
            }
            else
            {
                r = control.GetContentBounds();
            }
            if (control.ClipToBounds || control.IsRoot)
            {
                r.Intersect(rect);
            }
            else
            {
                //r = rect;
            }
            r.Offset(-rect.Left, -rect.Top);
            return r;
        }

        HashSet<UIElement> visibleElements = new HashSet<UIElement>();
        /// <summary>
        /// 元素是否在可视化范围内
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsVisible(UIElement element)
        {
            return visibleElements.Contains(element);
        }

        internal void ExecuteInitialLayoutPass(UIElement root)
        {
            Measure(root);
            Arrange(root);

            ExecuteLayoutPass();
        }

        private void ExecuteMeasurePass()
        {
            while (_toMeasure.Count > 0)
            {
                var next = _toMeasure.First();
                Measure(next);
            }
        }

        private void ExecuteArrangePass()
        {
            while (_toArrange.Count > 0 && _toMeasure.Count == 0)
            {
                var next = _toArrange.First();
                Arrange(next);
            }
        }

        private void Measure(UIElement control)
        {
            _toMeasure.Remove(control);
            if (control.Root == null)
            {
                return;
            }

            if (control.IsRoot)
            {
                var window = control as ITopLevel;
                if (window != null && control.Parent == null)
                {
                    //var w = window.Width;
                    //var h = window.Height;
                    //if (w.Unit == Unit.Percent || w.IsAuto || h.Unit == Unit.Percent || h.IsAuto)
                    //{
                    ITopLevel root = window;
                    Screen scr = root.Screen;
                    //if (window is Popup popup && popup.PlacementTarget != null && popup.PlacementTarget.Root != null)
                    //{
                    //    root = popup.PlacementTarget.Root;
                    //    scr = root.Screen;
                    //}
                    //else
                    //{
                    //    scr = window.Screen;
                    //}
                    control.Measure(new Size(scr.WorkingArea.Width / root.LayoutScaling, scr.WorkingArea.Height / root.LayoutScaling));
                    //}
                    //else
                    //{
                    //    control.Measure(new Size(w.Value, h.Value));
                    //}
                }
                else
                {
                    control.Measure(new Size(control.Width.Value, control.Height.Value));
                }
            }
            //else if (parent != null)
            //{
            //    Measure(parent);
            //}

            if (!control.IsMeasureValid)
            {
                if (control._previousMeasure.HasValue)
                {
                    control.Measure(control._previousMeasure.Value);
                }
                else
                {
                    //var parent = control.Parent;
                    //while (!parent._previousMeasure.HasValue)
                    //{
                    //    parent = parent.Parent;
                    //}
                    ////parent.Measure(parent._previousMeasure.Value);

                    //_toMeasure.Add(parent);

                }
            }

        }

        private void Arrange(UIElement control)
        {
            _toArrange.Remove(control);
            if (control.Root == null || control.Visibility == Visibility.Collapsed)
            {
                if (control.Root != null)
                {
                    control.IsArrangeValid = true;
                }
                return;
            }
            //var parent = control.Parent;

            if (control.IsRoot)
            {
                var window = control as ITopLevel;
                if (window != null && control.Parent == null)
                {
                    //var w = window.Width;
                    //var h = window.Height;
                    //if (w.Unit == Unit.Percent || w.IsAuto || h.Unit == Unit.Percent || h.IsAuto)
                    //{
                    ITopLevel root = window;
                    Screen scr = root.Screen;
                    control.Arrange(new Rect(0, 0, scr.WorkingArea.Width / root.LayoutScaling, scr.WorkingArea.Height / root.LayoutScaling));
                    //}
                    //else
                    //{
                    //    control.Arrange(new Rect(0, 0, w.Value, h.Value));
                    //}
                }
                else
                {
                    control.Arrange(new Rect(0, 0, control.Width.Value, control.Height.Value));
                }
            }
            //else if (parent != null)
            //{
            //    Arrange(parent);
            //}

            else if (control._previousArrange.HasValue)
            {
                control.Arrange(control._previousArrange.Value);
            }
            //control.Invalidate();

            //control.RendRect();
            //if (control.offsetChange.HasValue)
            //{
            //    foreach (var item in control.Find<UIElement>())
            //    {
            //        var r = item.RenderBounds;
            //        var offset = control.offsetChange.Value;
            //        item.RenderBounds = new Rect(r.X + offset.X, r.Y + offset.Y, r.Width, r.Height);
            //    }
            //    control.offsetChange = null;
            //}
            //else
            //{
            //foreach (var item in control.Find<UIElement>().Where(a => a.Visibility != Visibility.Collapsed))
            //{
            //    item.RendRect();
            //}
            //}
            if (control.Visibility != Visibility.Collapsed)
            {
                if (control.IsRoot && control.viewRenderRect || !control.IsRoot)
                {
                    RendRect(control);
                }
                control.viewRenderRect = false;
            }
        }

        void RendRect(UIElement element)
        {
            element.RendRect();
            var length = element.Children.Count;
            for (int i = 0; i < length; i++)
            {
                var item = element.Children[i];
                if (item.Visibility != Visibility.Collapsed)
                {
                    RendRect(item);
                }
            }
            //foreach (var item in element.Children)
            //{
            //    if (item.Visibility != Visibility.Collapsed)
            //    {
            //        RendRect(item);
            //    }
            //}
        }

        private VisibleUIElement VisibleUIElement(UIElement control, Rect rect)
        {
            VisibleUIElement i = null;
            if (control.Visibility == Visibility.Collapsed)
            {
                return i;
            }
            if (rect.IntersectsWith(control.RenderBounds) || root.InputManager.MouseDevice.Captured == control)
            {
                visibleElements.Add(control);
                //i = new VisibleUIElement { Element = control };
                i = Create(control);
            }
            //foreach (UIElement item in control.Children.OrderByZIndexList())
            var list = control.Children.OrderByZIndexList();
            for (int j = 0; j < list.Count; j++)
            {
                var ii = VisibleUIElement(list[j], rect);
                if (ii != null)
                {
                    if (i == null)
                    {
                        visibleElements.Add(control);
                        //i = new VisibleUIElement { Element = control };
                        i = Create(control);
                    }
                    i.Children.Add(ii);
                }
            }
            return i;
        }

        private void QueueLayoutPass()
        {
            if (!_queued && !_running)
            {
                _queued = true;
                Dispatcher.MainThread.BeginInvoke(ExecuteLayoutPass);
            }
        }

        #endregion

        //对象池，减少布局时候GC
        List<VisibleUIElement> empties = new List<VisibleUIElement>();
        List<VisibleUIElement> useds = new List<VisibleUIElement>();
        VisibleUIElement Create(UIElement ele)
        {
            if (empties.Count == 0)
            {
                var v = new VisibleUIElement();
                v.SetElement(ele);
                useds.Add(v);
                return v;
            }
            var vu = empties[empties.Count - 1];
            empties.RemoveAt(empties.Count - 1);
            useds.Add(vu);
            vu.SetElement(ele);
            return vu;
        }

        void Desdroy()
        {
            foreach (var item in useds)
            {
                item.SetElement(null);
                item.Children.Clear();
                empties.Add(item);
            }
            useds.Clear();
        }
    }
}
