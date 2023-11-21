using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPF.Input;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示可包含其他可视元素的可滚动区域。
    /// </summary>
    [Description("表示可包含其他可视元素的可滚动区域")]
    public class ScrollViewer : ContentControl
    {
        ///// <summary>
        ///// 获取或设置一个值，该值指示是否允许滚动支持 IScrollInfo 接口的元素。
        ///// </summary>
        //[PropertyMetadata(true)]
        //public bool CanContentScroll
        //{
        //    get { return GetValue<bool>(); }
        //    set { SetValue(value); }
        //}
        /// <summary>
        /// 获取或设置一个值，该值指示是否应显示水平 ScrollBar。
        /// </summary>
        [PropertyMetadata(ScrollBarVisibility.Auto)]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return GetValue<ScrollBarVisibility>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示是否应显示垂直 ScrollBar。
        /// </summary>
        [PropertyMetadata(ScrollBarVisibility.Auto)]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return GetValue<ScrollBarVisibility>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 水平 ScrollBar 是否可见。
        /// </summary>
        public Visibility ComputedHorizontalScrollBarVisibility
        {
            get { return GetValue<Visibility>(); }
            private set { SetValue(value); }
        }
        /// <summary>
        ///  垂直 ScrollBar 是否可见。
        /// </summary>
        public Visibility ComputedVerticalScrollBarVisibility
        {
            get { return GetValue<Visibility>(); }
            private set { SetValue(value); }
        }
        ///// <summary>
        ///// 获取可见内容的垂直偏移量。
        ///// </summary>
        //public double ContentVerticalOffset
        //{
        //    get { return GetValue<double>(); }
        //    set { SetValue(value); }
        //}

        ///// <summary>
        ///// 获取可见内容的水平偏移量。
        ///// </summary>
        //public double ContentHorizontalOffset
        //{
        //    get { return GetValue<double>(); }
        //    set { SetValue(value); }
        //}

        /// <summary>
        /// 获取包含盘区垂直大小的值
        /// </summary>
        [NotCpfProperty]
        public float ExtentHeight
        {
            get
            {
                if (scrollInfo == null)
                {
                    return 0;
                }
                return scrollInfo.ExtentHeight;
            }
        }
        /// <summary>
        /// 获取包含盘区水平大小的值
        /// </summary>
        [NotCpfProperty]
        public float ExtentWidth
        {
            get
            {
                if (scrollInfo == null)
                {
                    return 0;
                }
                return scrollInfo.ExtentWidth;
            }
        }
        /// <summary>
        /// ViewportWidth contains the horizontal size of the scrolling viewport.
        /// </summary>
        /// <remarks>
        /// ExtentWidth is only an output property; it can effectively be set by specifying
        /// Width on this element.
        /// </remarks>
        [NotCpfProperty]
        public float ViewportWidth
        {
            get
            {
                if (scrollInfo == null)
                {
                    return 0;
                }
                return scrollInfo.ViewportWidth;
            }
        }
        /// <summary>
        /// ViewportHeight contains the vertical size of the scrolling viewport.
        /// </summary>
        /// <remarks>
        /// ViewportHeight is only an output property; it can effectively be set by specifying
        /// Height on this element.
        /// </remarks>
        [NotCpfProperty]
        public float ViewportHeight
        {
            get
            {
                if (scrollInfo == null)
                {
                    return 0;
                }
                return scrollInfo.ViewportHeight;
            }
        }
        IScrollInfo scrollInfo;

        /// <summary>
        /// Scroll content by one line to the top.
        /// </summary>
        public void LineUp()
        {
            if (scrollInfo != null)
            {
                scrollInfo.LineUp();
            }
        }
        /// <summary>
        /// Scroll content by one line to the bottom.
        /// </summary>
        public void LineDown()
        {
            if (scrollInfo != null)
            {
                scrollInfo.LineDown();
            }
        }
        /// <summary>
        /// Scroll content by one line to the left.
        /// </summary>
        public void LineLeft()
        {
            if (scrollInfo != null)
            {
                scrollInfo.LineLeft();
            }
        }
        /// <summary>
        /// Scroll content by one line to the right.
        /// </summary>
        public void LineRight()
        {
            if (scrollInfo != null)
            {
                scrollInfo.LineRight();
            }
        }

        /// <summary>
        /// Scroll content by one page to the top.
        /// </summary>
        public void PageUp()
        {
            if (scrollInfo != null)
            {
                scrollInfo.PageUp();
            }
        }
        /// <summary>
        /// Scroll content by one page to the bottom.
        /// </summary>
        public void PageDown()
        {
            if (scrollInfo != null)
            {
                scrollInfo.PageDown();
            }
        }
        /// <summary>
        /// Scroll content by one page to the left.
        /// </summary>
        public void PageLeft()
        {
            if (scrollInfo != null)
            {
                scrollInfo.PageLeft();
            }
        }
        /// <summary>
        /// Scroll content by one page to the right.
        /// </summary>
        public void PageRight()
        {
            if (scrollInfo != null)
            {
                scrollInfo.PageRight();
            }
        }

        ///// <summary>
        ///// 0 and ExtentWidth less ViewportWidth" 
        ///// </summary>
        //public void SetHorizontalOffset(float offset)
        //{
        //    if (scrollInfo != null)
        //    {
        //        scrollInfo.SetHorizontalOffset(offset);
        //    }
        //}

        ///// <summary>
        ///// Set the VerticalOffset to the passed value.  
        ///// An implementation may coerce this value into a valid range, typically inclusively between 0 and <see cref="ExtentHeight" /> less <see cref="ViewportHeight" />.
        ///// </summary>
        //public void SetVerticalOffset(float offset)
        //{
        //    if (scrollInfo != null)
        //    {
        //        scrollInfo.SetVerticalOffset(offset);
        //    }
        //}

        protected override void InitializeComponent()
        {
            //var col = new ColumnDefinition { };
            //var row = new RowDefinition { };
            Children.Add(new Grid
            {
                Width = "100%",
                Height = "100%",
                ColumnDefinitions = { new ColumnDefinition { }, new ColumnDefinition { Width = "auto" } },
                RowDefinitions = { new RowDefinition { }, new RowDefinition { Height = "auto" } },
                Children = {
                    {new Border{Name="contentPresenter",Width="100%",Height="100%", BorderStroke="0", PresenterFor=this, } },
                    {new ScrollBar{ Width=20, Height="100%",Cursor=Cursors.Arrow, IncreaseLargeChanged=PageDown, DecreaseLargeChanged=PageUp, IncreaseSmallChange=LineDown, DecreaseSmallChange=LineUp,Bindings={
                            { nameof(ScrollBar.Maximum),nameof(VerticalMaximum),this,BindingMode.TwoWay },
                            { nameof(ScrollBar.Value),nameof(VerticalOffset),this,BindingMode.TwoWay},
                            { nameof(ScrollBar.ViewportSize),nameof(VerticalViewportSize),2 },
                            { nameof(Visibility),nameof(ComputedVerticalScrollBarVisibility),2},
                            { nameof(IsEnabled),nameof(VerticalScrollBarVisibility),this,BindingMode.OneWay,a=>((ScrollBarVisibility)a)!=ScrollBarVisibility.Disabled },
                            //{ nameof(Visibility),nameof(Width),col,BindingMode.OneWayToSource,null,a=>((Visibility)a)==Visibility.Visible?(GridLength)((FloatField)Binding.Current.Owner.GetValue(nameof(ScrollBar.Width))).Value:(GridLength)0}
                        } },1,0 },
                    {new ScrollBar{ Orientation=Orientation.Horizontal,Width="100%", Height=20,Cursor=Cursors.Arrow, IncreaseLargeChanged=PageRight, DecreaseLargeChanged=PageLeft, IncreaseSmallChange=LineRight, DecreaseSmallChange=LineLeft,Bindings={
                            { nameof(ScrollBar.Maximum),nameof(HorizontalMaximum),this,BindingMode.TwoWay },
                            { nameof(ScrollBar.Value),nameof(HorizontalOffset),this,BindingMode.TwoWay},
                            { nameof(ScrollBar.ViewportSize),nameof(HorizontalViewportSize),2 },
                            { nameof(Visibility),nameof(ComputedHorizontalScrollBarVisibility),2},
                            { nameof(IsEnabled),nameof(HorizontalScrollBarVisibility),this,BindingMode.OneWay,a=>((ScrollBarVisibility)a)!=ScrollBarVisibility.Disabled },
                            //{ nameof(Visibility),nameof(Height),row,BindingMode.OneWayToSource,null,a=>((Visibility)a)==Visibility.Visible?(GridLength)((FloatField)Binding.Current.Owner.GetValue(nameof(ScrollBar.Height))).Value:(GridLength)0  }
                        } },0,1 },
                }
            });
        }

        protected float VerticalViewportSize
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        protected float HorizontalViewportSize
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 垂直偏移，0到ExtentHeight-ViewportHeight
        /// </summary>
        public float VerticalOffset
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 垂直最大值
        /// </summary>
        [Browsable(false)]
        public float VerticalMaximum
        {
            get { return GetValue<float>(); }
            private set { SetValue(value); }
        }
        /// <summary>
        /// 水平最大值
        /// </summary>
        [Browsable(false)]
        public float HorizontalMaximum
        {
            get { return GetValue<float>(); }
            private set { SetValue(value); }
        }
        /// <summary>
        /// 水平偏移，0到ExtentWidth-ViewportWidth
        /// </summary>
        public float HorizontalOffset
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!e.Handled)
            {
                if (scrollInfo != null)
                {
                    if (e.IsTouch)
                    {
                        if (scrollInfo.CanHorizontallyScroll)
                        {
                            scrollInfo.SetHorizontalOffset(HorizontalOffset - e.Delta.X);
                            e.Handled = true;
                        }
                        if (scrollInfo.CanVerticallyScroll)
                        {
                            scrollInfo.SetVerticalOffset(VerticalOffset - e.Delta.Y);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        if (scrollInfo.CanVerticallyScroll)
                        {
                            if (e.Delta.Y < 0)
                            {
                                scrollInfo.MouseWheelUp();
                            }
                            else if (e.Delta.Y > 0)
                            {
                                scrollInfo.MouseWheelDown();
                            }
                            e.Handled = true;
                        }
                        if (scrollInfo.CanHorizontallyScroll)
                        {
                            if (e.Delta.X < 0)
                            {
                                scrollInfo.MouseWheelLeft();
                            }
                            else if (e.Delta.X > 0)
                            {
                                scrollInfo.MouseWheelRight();
                            }
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        protected override void OnSetContentElement(UIElement contentPresenter, UIElement oldValue, UIElement newValue)
        {
            if (oldValue != null)
            {
                if (scrollInfo != null)
                {
                    scrollInfo.ScrollChanged -= Scroll_ScrollChanged;
                    scrollInfo.ScrollOwner = null;
                }
                contentPresenter.Children.Remove(oldValue);
            }
            scrollInfo = null;
            if (newValue != null)
            {
                scrollInfo = newValue as IScrollInfo;
                if (scrollInfo == null)
                {
                    scrollInfo = new ScrollContentPresenter { Child = newValue, Width = "100%", Height = "100%" };
                }
                scrollInfo.ScrollChanged += Scroll_ScrollChanged;
                scrollInfo.ScrollOwner = this;
                var n = scrollInfo as UIElement;
                n.ClipToBounds = true;
                contentPresenter.Children.Add(n);
                Scroll_ScrollChanged(null, null);
            }
        }

        protected override void OnLayoutUpdated()
        {
            base.OnLayoutUpdated();
            Scroll_ScrollChanged(null, null);
        }

        private void Scroll_ScrollChanged(object sender, EventArgs e)
        {
            if (scrollInfo != null)
            {

                HorizontalMaximum = Math.Max(0, scrollInfo.ExtentWidth - scrollInfo.ViewportWidth);
                VerticalMaximum = Math.Max(0, scrollInfo.ExtentHeight - scrollInfo.ViewportHeight);
                HorizontalOffset = scrollInfo.HorizontalOffset;
                VerticalOffset = scrollInfo.VerticalOffset;
                VerticalViewportSize = scrollInfo.ViewportHeight;
                HorizontalViewportSize = scrollInfo.ViewportWidth;
                //var len = (float)(scrollInfo.ViewportHeight / scrollInfo.ExtentHeight);
                //if (scrollInfo.ExtentHeight == 0)
                //{
                //    len = 1;
                //}
                //len = Math.Min(1, len);
                //VerticalThumbLength = new FloatField(len, Unit.Percent);
                //len = (float)(scrollInfo.ViewportWidth / scrollInfo.ExtentWidth);
                //if (scrollInfo.ExtentWidth == 0)
                //{
                //    len = 1;
                //}
                //len = Math.Min(1, len);
                //HorizontalThumbLength = new FloatField(len, Unit.Percent);
                switch (HorizontalScrollBarVisibility)
                {
                    case ScrollBarVisibility.Auto:
                        ComputedHorizontalScrollBarVisibility = scrollInfo.CanHorizontallyScroll ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case ScrollBarVisibility.Hidden:
                        ComputedHorizontalScrollBarVisibility = Visibility.Collapsed;
                        break;
                    case ScrollBarVisibility.Disabled:
                    case ScrollBarVisibility.Visible:
                        ComputedHorizontalScrollBarVisibility = Visibility.Visible;
                        break;
                }
                switch (VerticalScrollBarVisibility)
                {
                    case ScrollBarVisibility.Auto:
                        ComputedVerticalScrollBarVisibility = scrollInfo.CanVerticallyScroll ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case ScrollBarVisibility.Hidden:
                        ComputedVerticalScrollBarVisibility = Visibility.Collapsed;
                        break;
                    case ScrollBarVisibility.Disabled:
                    case ScrollBarVisibility.Visible:
                        ComputedVerticalScrollBarVisibility = Visibility.Visible;
                        break;
                }
                OnScroll();
            }
            else
            {
                switch (HorizontalScrollBarVisibility)
                {
                    case ScrollBarVisibility.Auto:
                        ComputedHorizontalScrollBarVisibility = Visibility.Collapsed;
                        break;
                    case ScrollBarVisibility.Hidden:
                        ComputedHorizontalScrollBarVisibility = Visibility.Collapsed;
                        break;
                    case ScrollBarVisibility.Disabled:
                    case ScrollBarVisibility.Visible:
                        ComputedHorizontalScrollBarVisibility = Visibility.Visible;
                        break;
                }
                switch (VerticalScrollBarVisibility)
                {
                    case ScrollBarVisibility.Auto:
                        ComputedVerticalScrollBarVisibility = Visibility.Collapsed;
                        break;
                    case ScrollBarVisibility.Hidden:
                        ComputedVerticalScrollBarVisibility = Visibility.Collapsed;
                        break;
                    case ScrollBarVisibility.Disabled:
                    case ScrollBarVisibility.Visible:
                        ComputedVerticalScrollBarVisibility = Visibility.Visible;
                        break;
                }
            }
        }

        protected virtual void OnScroll()
        {

        }

        [PropertyChanged(nameof(VerticalOffset))]
        void OnVerticalOffset(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (scrollInfo != null)
            {
                scrollInfo.SetVerticalOffset((float)newValue);
            }
        }
        [PropertyChanged(nameof(HorizontalOffset))]
        void OnHorizontalOffset(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (scrollInfo != null)
            {
                scrollInfo.SetHorizontalOffset((float)newValue);
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    if (propertyName == nameof(VerticalOffset))
        //    {
        //        if (scrollInfo != null)
        //        {
        //            scrollInfo.SetVerticalOffset((float)newValue);
        //        }
        //    }
        //    else if (propertyName == nameof(HorizontalOffset))
        //    {
        //        if (scrollInfo != null)
        //        {
        //            scrollInfo.SetHorizontalOffset((float)newValue);
        //        }
        //    }
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //}
    }

    public enum ScrollBarVisibility
    {
        /// <summary>
        /// No scrollbars and no scrolling in this dimension.
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// The scrollbar should be visible only if there is more content than fits in the viewport.
        /// </summary>
        Auto,
        /// <summary>
        /// The scrollbar should never be visible.  No space should ever be reserved for the scrollbar.
        /// </summary>
        Hidden,
        /// <summary>
        /// The scrollbar should always be visible.  Space should always be reserved for the scrollbar.
        /// </summary>
        Visible,
    }
}
