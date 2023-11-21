using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;
using CPF.Platform;

namespace CPF.Controls
{
    /// <summary>
    /// 弹窗
    /// </summary>
    [Description("弹窗")]
    public class Popup : View
    {
        IPopupImpl windowImpl;
        /// <summary>
        /// 弹窗
        /// </summary>
        public Popup()
        {
            windowImpl = this.ViewImpl as IPopupImpl;
            //CanActivate = false;
        }

        protected override IViewImpl CreateView()
        {
            return Application.GetRuntimePlatform().CreatePopup();
        }

        //public void Hide()
        //{
        //    //windowImpl.Hide();
        //    Visibility = Visibility.Collapsed;
        //}

        //public void Show()
        //{
        //    //windowImpl.Show();
        //    Visibility = Visibility.Visible;
        //}
        ///// <summary>
        ///// 拖拽移动窗体
        ///// </summary>
        //public void DragMove()
        //{
        //    windowImpl.DragMove();
        //}
        ///// <summary>
        ///// 关闭窗体
        ///// </summary>
        //public void Close()
        //{
        //    windowImpl.Close();
        //}

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }
        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (!(Application.DisablePopupClose && propertyName == nameof(Visibility) && (Visibility)value != Visibility.Visible))
            {
                return base.OnSetValue(propertyName, ref value);
            }
            return false;
        }
        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            //overridePropertys.Override(nameof(IsAntiAlias), new UIPropertyMetadataAttribute(true, UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits));
            overridePropertys.Override(nameof(Background), new UIPropertyMetadataAttribute((ViewFill)"#fff", UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(Visibility), new UIPropertyMetadataAttribute(Visibility.Collapsed, UIPropertyOptions.AffectsMeasure));
        }


        ///// <summary>
        ///// 窗体所在的屏幕
        ///// </summary>
        //public override Screen Screen { get { return windowImpl.Screen; } }
        /// <summary>
        /// 获取或设置当打开 Popup 控件时该控件相对于其放置的元素。
        /// </summary>
        public UIElement PlacementTarget
        {
            get { return GetValue<UIElement>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示当 Popup 控件焦点不再对准时，是否关闭该控件。
        /// </summary>
        [PropertyMetadata(true)]
        public bool StaysOpen
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置 Popup 控件打开时的控件方向，并指定 Popup 控件在与屏幕边界重叠时的控件行为
        /// </summary>
        [PropertyMetadata(PlacementMode.Absolute)]
        public PlacementMode Placement
        {
            get { return GetValue<PlacementMode>(); }
            set { SetValue(value); }
        }

        public override Screen Screen
        {
            get
            {
                var taget = PlacementTarget;
                if (taget != null && taget.Root != null)
                {
                    return taget.Root.Screen;
                }
                return base.Screen;
            }
        }

        protected override void ArrangeCore(in Rect finalRect)
        {
            var taget = PlacementTarget;
            var pla = Placement;
            if ((taget && (pla == PlacementMode.Padding || pla == PlacementMode.Margin)) || pla == PlacementMode.Mouse)
            {
                //var bounds = this.Screen.Bounds.Size / LayoutScaling;
                var bounds = finalRect.Size;
                var l = MarginLeft;
                var t = MarginTop;
                var r = MarginRight;
                var b = MarginBottom;
                var w = Width;
                var h = Height;
                var maxw = MaxWidth;
                var maxh = MaxHeight;
                var minw = MinWidth;
                var minh = MinHeight;
                //var margin = new Thickness(
                //    l.GetActualValue(finalRect.Width),
                //    t.GetActualValue(finalRect.Height),
                //    r.GetActualValue(finalRect.Width),
                //    b.GetActualValue(finalRect.Height));
                var scale = Root.LayoutScaling;
                if (taget)
                {
                    scale = taget.Root.LayoutScaling;
                }

                var size = DesiredSize;//.Deflate(margin);
                //if (!l.IsAuto && !r.IsAuto)
                //{
                //    size.Width = Math.Max(finalRect.Width - l.GetActualValue(finalRect.Width) - r.GetActualValue(finalRect.Width), 0);
                //}
                //if (!t.IsAuto && !b.IsAuto)
                //{
                //    size.Height = Math.Max(finalRect.Height - t.GetActualValue(finalRect.Height) - b.GetActualValue(finalRect.Height), 0);
                //}
                if (!w.IsAuto)
                {
                    size.Width = Math.Max(0, w.GetActualValue(finalRect.Width));
                }
                if (!h.IsAuto)
                {
                    size.Height = Math.Max(0, h.GetActualValue(finalRect.Height));
                }
                if (!minw.IsAuto)
                {
                    size.Width = Math.Max(size.Width, minw.GetActualValue(finalRect.Width));
                }
                if (!minh.IsAuto)
                {
                    size.Height = Math.Max(size.Height, minh.GetActualValue(finalRect.Height));
                }
                if (!maxw.IsAuto)
                {
                    size.Width = Math.Min(size.Width, maxw.GetActualValue(finalRect.Width));
                }
                if (!maxh.IsAuto)
                {
                    size.Height = Math.Min(size.Height, maxh.GetActualValue(finalRect.Height));
                }

                var s = ArrangeOverride(size);

                size = new Size(Math.Max(s.Width, size.Width), Math.Max(s.Height, size.Height));

                if (!minw.IsAuto)
                {
                    size.Width = Math.Max(size.Width, minw.GetActualValue(finalRect.Width));
                }
                if (!minh.IsAuto)
                {
                    size.Height = Math.Max(size.Height, minh.GetActualValue(finalRect.Height));
                }
                if (!maxw.IsAuto)
                {
                    size.Width = Math.Min(size.Width, maxw.GetActualValue(finalRect.Width));
                }
                if (!maxh.IsAuto)
                {
                    size.Height = Math.Min(size.Height, maxh.GetActualValue(finalRect.Height));
                }
                float originX;
                float originY;
                if (pla == PlacementMode.Padding || pla == PlacementMode.Margin)
                {
                    var targetPos = taget.PointToScreen(new Point());
                    var scr = Screen;
                    targetPos = new Point(targetPos.X - scr.WorkingArea.X, targetPos.Y - scr.WorkingArea.Y);
                    var lt = targetPos / scale;

                    var tAs = taget.ActualSize;
                    originX = lt.X;
                    originY = lt.Y;
                    if (pla == PlacementMode.Padding)
                    {
                        originX = originX + l.GetActualValue(tAs.Width);
                        originY = originY + t.GetActualValue(tAs.Height);
                        if (l.IsAuto && !r.IsAuto)
                        {
                            originX = lt.X + tAs.Width - size.Width - r.GetActualValue(tAs.Width);
                        }
                        else if (l.IsAuto && r.IsAuto)
                        {
                            originX = lt.X + (tAs.Width - size.Width) / 2;
                        }
                        if (t.IsAuto && !b.IsAuto)
                        {
                            originY = lt.Y + tAs.Height - size.Height - b.GetActualValue(tAs.Height);
                        }
                        else if (t.IsAuto && b.IsAuto)
                        {
                            originY = lt.Y + (tAs.Height - size.Height) / 2;
                        }
                    }
                    else if (pla == PlacementMode.Margin)
                    {
                        originX = lt.X + tAs.Width + l.GetActualValue(size.Width);
                        originY = lt.Y + tAs.Height + t.GetActualValue(size.Height);
                        if (!l.IsAuto && r.IsAuto)
                        {
                            if (originX > bounds.Width - size.Width)
                            {
                                originX = lt.X - size.Width - l.GetActualValue(size.Width);
                            }
                        }
                        if (l.IsAuto && !r.IsAuto)
                        {
                            originX = lt.X - size.Width - r.GetActualValue(size.Width);
                            if (originX < 0)
                            {
                                originX = lt.X + tAs.Width + r.GetActualValue(size.Width);
                            }
                        }
                        else if (l.IsAuto && r.IsAuto)
                        {
                            originX = lt.X + (tAs.Width - size.Width) / 2;
                        }
                        if (t.IsAuto && !b.IsAuto)
                        {
                            originY = lt.Y - size.Height - b.GetActualValue(size.Height);
                        }
                        else if (t.IsAuto && b.IsAuto)
                        {
                            originY = lt.Y + (tAs.Height - size.Height) / 2;
                        }
                    }
                }
                else
                {
                    var mouseP = Application.GetRuntimePlatform().MousePosition;
                    var scr = Screen;
                    mouseP = new PixelPoint(mouseP.X - (int)scr.WorkingArea.X, mouseP.Y - (int)scr.WorkingArea.Y);
                    var mp = mouseP.ToPoint(LayoutScaling);
                    originX = mp.X + l.GetActualValue(size.Width);
                    originY = mp.Y + t.GetActualValue(size.Height);
                    if (l.IsAuto && !r.IsAuto)
                    {
                        originX = mp.X - size.Width - r.GetActualValue(size.Width);
                    }
                    else if (l.IsAuto && r.IsAuto)
                    {
                        originX = mp.X - size.Width / 2;
                    }
                    if (t.IsAuto && !b.IsAuto)
                    {
                        originY = mp.Y - size.Height - b.GetActualValue(size.Height);
                    }
                    else if (t.IsAuto && b.IsAuto)
                    {
                        originY = mp.Y - size.Height / 2;
                    }

                }
                if (originX < finalRect.X)
                {
                    originX = finalRect.X;
                }
                else if (originX > finalRect.X + bounds.Width - size.Width)
                {
                    originX = finalRect.X + bounds.Width - size.Width;
                }
                if (originY < finalRect.Y)
                {
                    originY = finalRect.Y;
                }
                else if (originY > finalRect.Y + bounds.Height - size.Height)
                {
                    originY = finalRect.Y + bounds.Height - size.Height;
                }

                if (UseLayoutRounding)
                {
                    originX = (float)Math.Floor(originX * scale) / scale;
                    originY = (float)Math.Floor(originY * scale) / scale;
                    size.Width = (float)Math.Ceiling(size.Width * scale) / scale;
                    size.Height = (float)Math.Ceiling(size.Height * scale) / scale;
                }

                VisualOffset = new Point(originX, originY);
                if (ActualSize != size)
                {
                    ActualSize = size;
                    viewRenderRect = true;
                }
            }
            else
            {
                base.ArrangeCore(finalRect);
            }
        }

        protected override void PositionChanged(PixelPoint pos)
        {
            var scr = Screen;
            var p = new Point(pos.X - scr.WorkingArea.X, pos.Y - scr.WorkingArea.Y);
            var target = PlacementTarget;
            var scal = ViewImpl.LayoutScaling;
            if (target != null)
            {
                scal = target.Root.LayoutScaling;
            }
            offset = true;
            Position = pos;
            offset = false;
            this.VisualOffset = p / scal;
            if (Placement == PlacementMode.Absolute)
            {
                MarginLeft = VisualOffset.X;
                MarginTop = VisualOffset.Y;
                MarginBottom = "auto";
                MarginRight = "auto";
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (!e.Handled && !StaysOpen)
            {
                Visibility = Visibility.Collapsed;
            }
        }

        [PropertyChanged(nameof(Visibility))]
        void RegisterVisibility(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var v = (Visibility)newValue;
            var t = PlacementTarget;
            if (t != null && t.Root != null)
            {
                if (v == Visibility.Visible)
                {
                    t.Root.LostFocus -= Old_LostFocus;
                    t.Root.PreviewMouseDown -= Old_PreviewMouseDown;
                    t.Root.LostFocus += Old_LostFocus;
                    t.Root.PreviewMouseDown += Old_PreviewMouseDown;
                }
                else
                {
                    t.Root.LostFocus -= Old_LostFocus;
                    t.Root.PreviewMouseDown -= Old_PreviewMouseDown;
                }
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(Width))
        //    {
        //        System.Diagnostics.Debug.WriteLine("Set:" + newValue);
        //    }
        //}

        private void Old_PreviewMouseDown(object sender, Input.MouseButtonEventArgs e)
        {
            if (!StaysOpen && !e.Handled && (!e.IsTouch || !(this.PlacementTarget is MenuItem)))
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void Old_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!StaysOpen && !e.Handled && !CanActivate)
            {
                Visibility = Visibility.Collapsed;
            }
        }

        protected override Size MeasureCore(in Size availableSize)
        {
            var taget = PlacementTarget;
            var pla = Placement;
            if ((taget && (pla == PlacementMode.Padding || pla == PlacementMode.Margin)) || pla == PlacementMode.Mouse)
            {
                if (Visibility != Visibility.Collapsed)
                {
                    var w = Width;
                    var h = Height;
                    var maxw = MaxWidth;
                    var maxh = MaxHeight;
                    var minw = MinWidth;
                    var minh = MinHeight;

                    //var constrainedWidth = availableSize.Width;//可以提供的最大尺寸
                    //var constrainedHeight = availableSize.Height;
                    var constrainedWidth = float.PositiveInfinity;//可以提供的最大尺寸
                    var constrainedHeight = float.PositiveInfinity;

                    if (!w.IsAuto)
                    {
                        constrainedWidth = w.GetActualValue(availableSize.Width);
                    }
                    if (!maxw.IsAuto)
                    {
                        var max = maxw.GetActualValue(availableSize.Width);
                        if (constrainedWidth > max)
                        {
                            constrainedWidth = max;
                        }
                    }
                    if (!minw.IsAuto)
                    {
                        var min = minw.GetActualValue(availableSize.Width);
                        if (constrainedWidth < min)
                        {
                            constrainedWidth = min;
                        }
                    }

                    if (!h.IsAuto)
                    {
                        constrainedHeight = h.GetActualValue(availableSize.Height);
                    }

                    if (!maxh.IsAuto)
                    {
                        var max = maxh.GetActualValue(availableSize.Height);
                        if (constrainedHeight > max)
                        {
                            constrainedHeight = max;
                        }
                    }
                    if (!minh.IsAuto)
                    {
                        var min = minh.GetActualValue(availableSize.Height);
                        if (constrainedHeight < min)
                        {
                            constrainedHeight = min;
                        }
                    }

                    //内容尺寸
                    var measured = MeasureOverride(new Size(constrainedWidth, constrainedHeight));
                    var width = measured.Width;
                    var height = measured.Height;

                    if (!w.IsAuto)
                    {
                        width = w.GetActualValue(availableSize.Width);
                    }
                    if (!h.IsAuto)
                    {
                        height = h.GetActualValue(availableSize.Height);
                    }
                    if (!minw.IsAuto)
                    {
                        var min = minw.GetActualValue(availableSize.Width);
                        if (width < min)
                        {
                            width = min;
                        }
                    }
                    if (!minh.IsAuto)
                    {
                        var min = minh.GetActualValue(availableSize.Height);
                        if (height < min)
                        {
                            height = min;
                        }
                    }
                    if (!maxw.IsAuto)
                    {
                        var max = maxw.GetActualValue(availableSize.Width);
                        if (width > max)
                        {
                            width = max;
                        }
                    }
                    if (!maxh.IsAuto)
                    {
                        var max = maxh.GetActualValue(availableSize.Height);
                        if (height > max)
                        {
                            height = max;
                        }
                    }

                    if (UseLayoutRounding)
                    {
                        var scale = taget.Root.LayoutScaling;
                        width = (float)Math.Ceiling(width * scale) / scale;
                        height = (float)Math.Ceiling(height * scale) / scale;
                    }

                    return new Size(Math.Max(0, width), Math.Max(0, height));
                }
                else
                {
                    return new Size();
                }
            }
            else
            {
                return base.MeasureCore(availableSize);
            }
        }

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    return base.MeasureOverride(availableSize);
        //}
    }


    public enum PlacementMode : byte
    {
        /// <summary>
        /// Popup 控件的位置，该位置相对于屏幕的左上角，且在由 Margin 的属性值定义的偏移量处。
        /// </summary>
        Absolute,
        /// <summary>
        /// Popup 控件的位置，即控件边缘与鼠标边缘对齐。 如果屏幕边缘遮盖 Popup，则控件会重新定位自身，使其与屏幕上边缘对齐。百分比参考自身尺寸
        /// </summary>
        Mouse,
        /// <summary>
        /// 相对PlacementTarget元素外边距，需要设置相邻的两个Margin值，百分比参考自身尺寸。如果屏幕边缘遮盖 Popup，则控件会重新定位自身，使其与屏幕上边缘对齐。
        /// </summary>
        Margin,
        /// <summary>
        /// 相对PlacementTarget元素内边距，需要设置相邻的两个Margin值，百分比参考PlacementTarget尺寸。如果屏幕边缘遮盖 Popup，则控件会重新定位自身，使其与屏幕上边缘对齐。
        /// </summary>
        Padding,
    }
}
