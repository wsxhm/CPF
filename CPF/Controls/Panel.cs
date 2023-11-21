using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 为所有 Panel 元素提供基类。 使用 Panel 元素放置和排列UIElement，对于Panel的继承者的子对象的如果尺寸超过Panel对齐方式是左上角而不是居中
    /// </summary>
    [Description("为所有 Panel 元素提供基类。 使用 Panel 元素放置和排列UIElement，对于Panel的继承者的子对象的如果尺寸超过Panel对齐方式是左上角而不是居中")]
    public class Panel : UIElement
    {
        /// <summary>
        /// 背景填充
        /// </summary>
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public ViewFill Background
        {
            get { return (ViewFill)GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the font family used to draw the control's text.
        /// </summary>
        [UIPropertyMetadata("宋体", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits)]
        public string FontFamily
        {
            get { return (string)GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the size of the control's text in points.
        /// </summary>
        [UIPropertyMetadata(12f, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure | UIPropertyOptions.Inherits)]
        public float FontSize
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the font style used to draw the control's text.
        /// </summary>
        [UIPropertyMetadata(FontStyles.Regular, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure | UIPropertyOptions.Inherits)]
        public FontStyles FontStyle
        {
            get { return (FontStyles)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// </summary>
        [UIPropertyMetadata(typeof(TextDecoration), "", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits)]
        [Description("表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]")]
        public TextDecoration TextDecoration
        {
            get { return (TextDecoration)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// Gets or sets the brush used to draw the control's text and other foreground elements.
        /// </summary>
        [UIPropertyMetadata(typeof(ViewFill), "Black", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits)]
        public ViewFill Foreground
        {
            get { return (ViewFill)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 边框线条填充
        /// </summary>
        [Description("边框线条填充")]
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public ViewFill BorderFill
        {
            get { return (ViewFill)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置线条类型
        /// </summary>
        [Description("获取或设置线条类型")]
        [UIPropertyMetadata(typeof(Stroke), "0", UIPropertyOptions.AffectsMeasure | UIPropertyOptions.AffectsRender)]
        public Stroke BorderStroke
        {
            get { return (Stroke)GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 四周边框粗细
        /// </summary>
        [Description("四周边框粗细")]
        [UIPropertyMetadata(typeof(Thickness), "0", UIPropertyOptions.AffectsMeasure | UIPropertyOptions.AffectsRender)]
        public Thickness BorderThickness
        {
            get { return GetValue<Thickness>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。
        /// </summary>
        [Description("获取或设置一个值，该值表示将 Border 的角倒圆的程度。")]
        [UIPropertyMetadata(typeof(CornerRadius), "0", UIPropertyOptions.AffectsRender)]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 边框类型
        /// </summary>
        [UIPropertyMetadata(BorderType.BorderStroke, UIPropertyOptions.AffectsMeasure | UIPropertyOptions.AffectsRender)]
        public BorderType BorderType
        {
            get { return GetValue<BorderType>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 定义一个控件组，一般由多个元素组成，在设计器中，子元素和该控件为一个控件组，点击子元素拖动时，将作为整体拖动整个控件组。
        /// </summary>
        [Description("定义一个控件组，一般由多个元素组成，在设计器中，子元素和该控件为一个控件组，点击子元素拖动时，将作为整体拖动整个控件组。如果该控件被子元素盖住，按Alt+Ctrl键加鼠标左键可以选中该控件。按住Alt键可以移动子元素。")]
        public bool IsGroup
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public Panel()
        {

        }

        /// <summary>
        /// 子元素
        /// </summary>
        [NotCpfProperty]
        public virtual new UIElementCollection Children
        {
            get
            {
                return base.Children;
            }
        }
        //Collection<UIElement> bind;

        //protected override bool OnSetValue(string propertyName, ref object value)
        //{
        //    if (propertyName == nameof(Children))
        //    {
        //        if (bind != null)
        //        {
        //            bind.CollectionChanged -= List_CollectionChanged;
        //            bind = null;
        //        }
        //        if (value is Collection<UIElement> list)
        //        {
        //            bind = list;
        //            base.Children.Clear();
        //            foreach (var item in list)
        //            {
        //                base.Children.Add(item);
        //            }
        //            list.CollectionChanged += List_CollectionChanged;
        //        }
        //        else if (value == null)
        //        {
        //            base.Children.Clear();
        //        }
        //        else
        //        {
        //            throw new Exception("Children只能支持Collection<UIElement>类型绑定");
        //        }
        //        value = base.Children;
        //    }
        //    return base.OnSetValue(propertyName, ref value);
        //}

        //private void List_CollectionChanged(object sender, CollectionChangedEventArgs<UIElement> e)
        //{
        //    switch (e.Action)
        //    {
        //        case CollectionChangedAction.Add:
        //            base.Children.Insert(e.Index, e.NewItem);
        //            break;
        //        case CollectionChangedAction.Remove:
        //            base.Children.RemoveAt(e.Index);
        //            break;
        //        case CollectionChangedAction.Replace:
        //            base.Children.RemoveAt(e.Index);
        //            base.Children.Insert(e.Index, e.NewItem);
        //            break;
        //        case CollectionChangedAction.Sort:
        //            base.Children.ElementList.Clear();
        //            foreach (var item in (sender as Collection<UIElement>))
        //            {
        //                base.Children.ElementList.Add(item);
        //            }
        //            InvalidateMeasure();
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //protected override object OnGetDefaultValue(PropertyMetadataAttribute pm)
        //{
        //    if (pm.PropertyName == nameof(Children))
        //    {
        //        return base.Children;
        //    }
        //    return base.OnGetDefaultValue(pm);
        //}

        protected override Size MeasureOverride(in Size availableSize)
        {
            var ava = availableSize;
            Size size;
            if (BorderType == BorderType.BorderStroke)
            {
                var b = BorderStroke;
                ava.Width = Math.Max(0, ava.Width - b.Width - b.Width);
                ava.Height = Math.Max(0, ava.Height - b.Width - b.Width);
                size = base.MeasureOverride(ava);
                size.Width = size.Width + b.Width + b.Width;
                size.Height = size.Height + b.Width + b.Width;
            }
            else
            {
                var b = BorderThickness;
                ava.Width = Math.Max(0, ava.Width - b.Left - b.Right);
                ava.Height = Math.Max(0, ava.Height - b.Top - b.Bottom);
                size = base.MeasureOverride(ava);
                size.Width = size.Width + b.Left + b.Right;
                size.Height = size.Height + b.Top + b.Bottom;
            }
            return size;
        }

        protected override Size ArrangeOverride(in Size finalSize)
        {
            var rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            if (BorderType == BorderType.BorderStroke)
            {
                var b = BorderStroke;
                rect.Location = new Point(b.Width, b.Width);
                rect.Size = new Size(Math.Max(0, rect.Width - b.Width - b.Width), Math.Max(0, rect.Height - b.Width - b.Width));
            }
            else
            {
                var b = BorderThickness;
                rect.Location = new Point(b.Left, b.Top);
                rect.Size = new Size(Math.Max(0, rect.Width - b.Left - b.Right), Math.Max(0, rect.Height - b.Top - b.Bottom));
            }
            foreach (UIElement child in Children)
            {
                child.Arrange(rect);
            }
            return finalSize;
        }
        protected override void OnRender(DrawingContext dc)
        {
            var cr = CornerRadius;
            var s = ActualSize;

            var rect = new Rect(0, 0, Math.Max(0, s.Width), Math.Max(0, s.Height));
            if (BorderType == BorderType.BorderStroke)
            {
                var bs = BorderStroke;

                using (var path = new PathGeometry())
                {
                    if (UseLayoutRounding)
                    {
                        bs.Width = (float)Math.Round(bs.Width * Root.RenderScaling) / Root.RenderScaling;
                    }
                    var w2 = bs.Width / 2;
                    path.BeginFigure(w2 + cr.TopLeft + rect.Left, w2 + rect.Top);
                    path.LineTo(rect.Right - w2 - cr.TopRight, w2 + rect.Top);
                    path.ArcTo(new Point(rect.Right - w2, w2 + cr.TopRight + rect.Top), new Size(cr.TopRight, cr.TopRight), 90, true, false);
                    path.LineTo(rect.Right - w2, rect.Bottom - cr.BottomRight - w2);
                    path.ArcTo(new Point(rect.Right - w2 - cr.BottomRight, rect.Bottom - w2), new Size(cr.BottomRight, cr.BottomRight), 90, true, false);
                    path.LineTo(w2 + cr.BottomLeft + rect.Left, rect.Bottom - w2);
                    path.ArcTo(new Point(w2 + rect.Left, rect.Bottom - w2 - cr.BottomLeft), new Size(cr.BottomLeft, cr.BottomLeft), 90, true, false);
                    path.LineTo(w2 + rect.Left, w2 + cr.TopLeft + rect.Top);
                    path.ArcTo(new Point(w2 + cr.TopLeft + rect.Left, w2 + rect.Top), new Size(cr.TopLeft, cr.TopLeft), 90, true, false);
                    path.EndFigure(true);

                    var ba = Background;
                    if (ba != null)
                    {
                        using (var brush = ba.CreateBrush(rect, Root.RenderScaling))
                        {
                            dc.FillPath(brush, path);
                        }
                    }
                    if (bs.Width > 0)
                    {
                        var bf = BorderFill;
                        if (bf != null)
                        {
                            using (var brush = bf.CreateBrush(rect, Root.RenderScaling))
                            {
                                dc.DrawPath(brush, bs, path);
                            }
                        }
                    }
                }

            }
            else
            {
                var bt = BorderThickness;

                if (!cr.IsZero)
                {
                    using (var path = new PathGeometry())
                    {
                        using (var p = new PathGeometry())
                        {
                            var w2 = 0.5f;

                            p.BeginFigure(w2 + cr.TopLeft + bt.Left + rect.Left, w2 + bt.Top + rect.Top);
                            p.LineTo(rect.Right - w2 - cr.TopRight - bt.Right, w2 + bt.Top + rect.Top);
                            p.ArcTo(new Point(rect.Right - w2 - bt.Right, w2 + cr.TopRight + bt.Top + rect.Top), new Size(cr.TopRight, cr.TopRight), 90, true, false);
                            p.LineTo(rect.Right - w2 - bt.Right, rect.Bottom - cr.BottomRight - w2 - bt.Bottom);
                            p.ArcTo(new Point(rect.Right - w2 - cr.BottomRight - bt.Right, rect.Bottom - w2 - bt.Bottom), new Size(cr.BottomRight, cr.BottomRight), 90, true, false);
                            p.LineTo(w2 + cr.BottomLeft + bt.Left + rect.Left, rect.Bottom - w2 - bt.Bottom);
                            p.ArcTo(new Point(w2 + bt.Left + rect.Left, rect.Bottom - w2 - cr.BottomLeft - bt.Bottom), new Size(cr.BottomLeft, cr.BottomLeft), 90, true, false);
                            p.LineTo(w2 + bt.Left + rect.Left, w2 + cr.TopLeft + bt.Top + rect.Top);
                            p.ArcTo(new Point(w2 + cr.TopLeft + bt.Left + rect.Left, w2 + bt.Top + rect.Top), new Size(cr.TopLeft, cr.TopLeft), 90, true, false);
                            p.EndFigure(true);
                            //w2 = 1;
                            path.BeginFigure(w2 + cr.TopLeft + rect.Left, w2 + rect.Top);
                            path.LineTo(rect.Right - w2 - cr.TopRight, w2 + rect.Top);
                            path.ArcTo(new Point(rect.Right - w2, w2 + cr.TopRight + rect.Top), new Size(cr.TopRight, cr.TopRight), 90, true, false);
                            path.LineTo(rect.Right - w2, rect.Bottom - cr.BottomRight - w2);
                            path.ArcTo(new Point(rect.Right - w2 - cr.BottomRight, rect.Bottom - w2), new Size(cr.BottomRight, cr.BottomRight), 90, true, false);
                            path.LineTo(w2 + cr.BottomLeft + rect.Left, rect.Bottom - w2);
                            path.ArcTo(new Point(w2 + rect.Left, rect.Bottom - w2 - cr.BottomLeft), new Size(cr.BottomLeft, cr.BottomLeft), 90, true, false);
                            path.LineTo(w2 + rect.Left, w2 + cr.TopLeft + rect.Top);
                            path.ArcTo(new Point(w2 + cr.TopLeft + rect.Left, w2 + rect.Top), new Size(cr.TopLeft, cr.TopLeft), 90, true, false);
                            path.EndFigure(true);

                            using (var geo = new Geometry(path))
                            {
                                using (var ge = new Geometry(p))
                                {
                                    geo.Xor(ge);

                                    var bf = BorderFill;
                                    Brush brush = null;
                                    var str = new Stroke(1);
                                    var ww = 0.8f;
                                    if (bf != null)
                                    {
                                        brush = bf.CreateBrush(rect, Root.RenderScaling);
                                    }
                                    if (brush != null)
                                    {
                                        dc.FillGeometry(brush, geo);
                                    }

                                    var ba = Background;
                                    if (ba != null)
                                    {
                                        using (var brush1 = ba.CreateBrush(rect, Root.RenderScaling))
                                        {
                                            dc.FillGeometry(brush1, ge);
                                            dc.DrawPath(brush1, new Stroke(0.8f), p);
                                        }
                                    }
                                    if (brush != null)
                                    {
                                        if (cr.TopLeft > 0 && (bt.Top > 0 || bt.Left > 0))
                                        {
                                            using (var pp = new PathGeometry())
                                            {
                                                pp.BeginFigure(ww + rect.Left, ww + cr.TopLeft + rect.Top);
                                                pp.ArcTo(new Point(ww + cr.TopLeft + rect.Left, ww + rect.Top), new Size(cr.TopLeft, cr.TopLeft), 90, true, false);
                                                pp.EndFigure(false);
                                                dc.DrawPath(brush, str, pp);
                                            }
                                        }
                                        if (cr.TopRight > 0 && (bt.Top > 0 || bt.Right > 0))
                                        {
                                            using (var pp = new PathGeometry())
                                            {
                                                pp.BeginFigure(rect.Right - ww - cr.TopRight, ww + rect.Top);
                                                pp.ArcTo(new Point(rect.Right - ww, ww + cr.TopRight + rect.Top), new Size(cr.TopRight, cr.TopRight), 90, true, false);
                                                pp.EndFigure(false);
                                                dc.DrawPath(brush, str, pp);
                                            }
                                        }
                                        if (cr.BottomLeft > 0 && (bt.Bottom > 0 || bt.Left > 0))
                                        {
                                            using (var pp = new PathGeometry())
                                            {
                                                pp.BeginFigure(ww + cr.BottomLeft + rect.Left, rect.Bottom - ww);
                                                pp.ArcTo(new Point(ww + rect.Left, rect.Bottom - ww - cr.BottomLeft), new Size(cr.BottomLeft, cr.BottomLeft), 90, true, false);
                                                pp.EndFigure(false);
                                                dc.DrawPath(brush, str, pp);
                                            }
                                        }
                                        if (cr.BottomRight > 0 && (bt.Bottom > 0 || bt.Right > 0))
                                        {
                                            using (var pp = new PathGeometry())
                                            {
                                                pp.BeginFigure(rect.Right - ww, rect.Bottom - cr.BottomRight - ww);
                                                pp.ArcTo(new Point(rect.Right - ww - cr.BottomRight, rect.Bottom - ww), new Size(cr.BottomRight, cr.BottomRight), 90, true, false);
                                                pp.EndFigure(false);
                                                dc.DrawPath(brush, str, pp);
                                            }
                                        }

                                        ww = 0.5f;
                                        str = new Stroke(1);
                                        if (bt.Left > 0)
                                        {
                                            dc.DrawLine(str, brush, new Point(ww + rect.Left, ww + cr.TopLeft + rect.Top), new Point(ww + rect.Left, rect.Bottom - cr.BottomLeft - ww));
                                        }
                                        if (bt.Top > 0)
                                        {
                                            dc.DrawLine(str, brush, new Point(ww + cr.TopLeft + rect.Left, ww + rect.Top), new Point(rect.Right - ww - cr.TopRight, ww + rect.Top));
                                        }
                                        if (bt.Right > 0)
                                        {
                                            dc.DrawLine(str, brush, new Point(rect.Right - ww, ww + cr.TopRight + rect.Top), new Point(rect.Right - ww, rect.Bottom - cr.BottomRight - ww));
                                        }
                                        if (bt.Bottom > 0)
                                        {
                                            dc.DrawLine(str, brush, new Point(ww + cr.BottomLeft + rect.Left, rect.Bottom - ww), new Point(rect.Right - ww - cr.BottomRight, rect.Bottom - ww));
                                        }
                                        brush.Dispose();
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    var bf = BorderFill;
                    if (UseLayoutRounding || !IsAntiAlias)
                    {
                        var sc = Root.RenderScaling;
                        bt.Left = (float)Math.Round(bt.Left * sc) / sc;
                        bt.Right = (float)Math.Round(bt.Right * sc) / sc;
                        bt.Top = (float)Math.Round(bt.Top * sc) / sc;
                        bt.Bottom = (float)Math.Round(bt.Bottom * sc) / sc;
                    }
                    if (bf != null)
                    {
                        using (var brush = bf.CreateBrush(rect, Root.RenderScaling))
                        {
                            if (bt.Top > 0)
                            {
                                dc.DrawLine(new Stroke(bt.Top), brush, new Point(rect.Left, bt.Top / 2 + rect.Top), new Point(rect.Right, bt.Top / 2 + rect.Top));
                            }
                            if (bt.Left > 0)
                            {
                                dc.DrawLine(new Stroke(bt.Left), brush, new Point(bt.Left / 2 + rect.Left, rect.Top), new Point(bt.Left / 2 + rect.Left, rect.Bottom));
                            }
                            if (bt.Right > 0)
                            {
                                dc.DrawLine(new Stroke(bt.Right), brush, new Point(rect.Right - bt.Right / 2, rect.Top), new Point(rect.Right - bt.Right / 2, rect.Bottom));
                            }
                            if (bt.Bottom > 0)
                            {
                                dc.DrawLine(new Stroke(bt.Bottom), brush, new Point(rect.Left, rect.Bottom - bt.Bottom / 2), new Point(rect.Right, rect.Bottom - bt.Bottom / 2));
                            }
                        }
                    }
                    var ba = Background;
                    if (ba != null)
                    {
                        using (var brush = ba.CreateBrush(rect, Root.RenderScaling))
                        {
                            dc.FillRectangle(brush, new Rect(bt.Left + rect.Left, bt.Top + rect.Top, Math.Max(0, rect.Width - bt.Left - bt.Right), Math.Max(0, rect.Height - bt.Top - bt.Bottom)));
                        }
                    }
                }
            }
        }

    }
}
