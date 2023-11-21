using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;
using CPF.Input;

namespace CPF.Controls
{
    /// <summary>
    /// 显示 ScrollViewer 控件的内容。
    /// </summary>
    [Description("显示 ScrollViewer 控件的内容。"), Browsable(false)]
    public class ScrollContentPresenter : Panel, IScrollInfo
    {
        //Size preFinalSize;
        //Point preOffset;
        //bool measure = false;
        //protected override Size MeasureOverride(in Size availableSize)
        //{
        //    //measure = true;
        //    return base.MeasureOverride(availableSize);
        //}

        //protected override void OnChildDesiredSizeChanged(UIElement child)
        //{
        //    base.OnChildDesiredSizeChanged(child);
        //    InvalidateArrange();
        //}

        float extentWidth = 0;
        float extentHeight = 0;
        protected override Size ArrangeOverride(in Size finalSize)
        {
            var rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            rect.X = -HorizontalOffset;
            rect.Y = -VerticalOffset;
            //bool flag = measure;
            extentWidth = 0;
            extentHeight = 0;
            foreach (UIElement child in Children)
            {
                //rect.Size = child.DesiredSize;
                //var w = child.Width;
                //if (w.Unit == Unit.Percent)
                //{
                //    rect.Width = finalSize.Width * w.Value;
                //}
                //var h = child.Height;
                //if (h.Unit == Unit.Percent)
                //{
                //    rect.Height = finalSize.Height * h.Value;
                //}
                //var minw = child.MinWidth;
                //if (!minw.IsAuto && minw.Unit == Unit.Percent)
                //{
                //    var min = minw.GetActualValue(finalSize.Width);
                //    if (min > rect.Width)
                //    {
                //        rect.Width = min;
                //    }
                //}
                //var minh = child.MinHeight;
                //if (!minh.IsAuto && minh.Unit == Unit.Percent)
                //{
                //    var min = minh.GetActualValue(finalSize.Height);
                //    if (min > rect.Height)
                //    {
                //        rect.Height = min;
                //    }
                //}
                child.Arrange(rect);
                extentWidth = Math.Max(extentWidth, child.ActualSize.Width);
                extentHeight = Math.Max(extentHeight, child.ActualSize.Height);
            }
            //if (preFinalSize == rect.Size && !flag)
            //{
            //    SetOffsetLayoutFlag(new Point(rect.X - preOffset.X, rect.Y - preOffset.Y));
            //}
            //measure = false;
            //preFinalSize = rect.Size;
            //preOffset = rect.Location;
            return finalSize;
        }

        /// <summary>
        /// 获取或设置 单一子元素。
        /// </summary>
        public UIElement Child
        {
            get { return GetValue<UIElement>(); }
            set { SetValue(value); }
        }

        [PropertyChanged(nameof(Child))]
        void RegisterChild(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var o = oldValue as UIElement;
            if (o != null)
            {
                Children.Remove(o);
            }
            var c = newValue as UIElement;
            if (c != null)
            {
                Children.Add(c);
            }
        }

        protected override void OnUIElementRemoved(UIElementRemovedEventArgs e)
        {
            base.OnUIElementRemoved(e);
            if (e.Element == Child)
            {
                Child = null;
            }
        }

        public event EventHandler ScrollChanged
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            base.OnGotFocus(e);
            if (e.OriginalSource != this)
            {

            }
        }

        protected override void OnLayoutUpdated()
        {
            var size = ActualSize;
            bool changed = false;
            if (!extentHeight.Equal(ExtentHeight))
            {
                ExtentHeight = extentHeight;
                changed = true;
            }
            if (!extentWidth.Equal(ExtentWidth))
            {
                ExtentWidth = extentWidth;
                changed = true;
            }
            if (!ViewportHeight.Equal(size.Height))
            {
                ViewportHeight = size.Height;
                changed = true;
            }
            if (!ViewportWidth.Equal(size.Width))
            {
                ViewportWidth = size.Width;
                changed = true;
            }
            if (CanVerticallyScroll != (!ExtentHeight.Equal(ViewportHeight) && ExtentHeight > ViewportHeight))
            {
                CanVerticallyScroll = (!ExtentHeight.Equal(ViewportHeight) && ExtentHeight > ViewportHeight);
                changed = true;
            }
            if (CanHorizontallyScroll != (!ExtentWidth.Equal(ViewportWidth) && ExtentWidth > ViewportWidth))
            {
                CanHorizontallyScroll = (!ExtentWidth.Equal(ViewportWidth) && ExtentWidth > ViewportWidth);
                changed = true;
            }
            if (changed)
            {
                if (scrollChanged)
                {
                    scrollChanged = false;
                    this.BeginInvoke(() =>
                    {
                        RaiseEvent(EventArgs.Empty, nameof(ScrollChanged));
                        scrollChanged = true;
                    });
                }
            }
            base.OnLayoutUpdated();
        }
        bool scrollChanged = true;

        [PropertyMetadata(16f)]
        public float ScrollLineDelta
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(48f)]
        public float MouseWheelDelta
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        //internal const float _scrollLineDelta = 16;   // Default physical amount to scroll with one Up/Down/Left/Right key
        //internal const float _mouseWheelDelta = 48;   // Default physical amount to scroll with one MouseWheel.


        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - ScrollLineDelta);
        }

        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + ScrollLineDelta);
        }

        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ScrollLineDelta);
        }

        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + ScrollLineDelta);
        }

        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset + MouseWheelDelta);
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset - MouseWheelDelta);
        }

        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - MouseWheelDelta);
        }

        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + MouseWheelDelta);
        }

        public void SetHorizontalOffset(float offset)
        {
            HorizontalOffset = offset;
            if (HorizontalOffset < 0 || ExtentWidth < ViewportWidth)
            {
                HorizontalOffset = 0;
            }
            else if (HorizontalOffset > ExtentWidth - ViewportWidth)
            {
                HorizontalOffset = ExtentWidth - ViewportWidth;
            }
            RaiseEvent(EventArgs.Empty, nameof(ScrollChanged));
            InvalidateArrange();
        }

        public void SetVerticalOffset(float offset)
        {
            VerticalOffset = offset;
            if (VerticalOffset < 0 || ExtentHeight < ViewportHeight)
            {
                VerticalOffset = 0;
            }
            else if (VerticalOffset > ExtentHeight - ViewportHeight)
            {
                VerticalOffset = ExtentHeight - ViewportHeight;
            }
            RaiseEvent(EventArgs.Empty, nameof(ScrollChanged));
            InvalidateArrange();
        }

        [NotCpfProperty]
        public bool CanVerticallyScroll
        {
            get;
            set;
        }

        [NotCpfProperty]
        public bool CanHorizontallyScroll
        {
            get;
            set;
        }

        [NotCpfProperty]
        public float ExtentWidth
        {
            get; private set;
        }

        [NotCpfProperty]
        public float ExtentHeight
        {
            get; private set;
        }

        [NotCpfProperty]
        public float ViewportWidth
        {
            get; private set;
        }

        [NotCpfProperty]
        public float ViewportHeight
        {
            get; private set;
        }

        [NotCpfProperty]
        public float HorizontalOffset
        {
            get; private set;
        }

        [NotCpfProperty]
        public float VerticalOffset
        {
            get;
            private set;
        }

        [NotCpfProperty]
        public ScrollViewer ScrollOwner
        {
            get;
            set;
        }
    }
}
