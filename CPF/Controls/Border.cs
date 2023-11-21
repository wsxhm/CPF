using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.ComponentModel;
using CPF.Effects;

namespace CPF.Controls
{
    /// <summary>
    /// 在另一个元素四周绘制边框和背景。
    /// </summary>
    [Description("在另一个元素四周绘制边框和背景")]
    public class Border : UIElement
    {
        /// <summary>
        /// 获取或设置 单一子元素。
        /// </summary>
        [Browsable(false)]
        public UIElement Child
        {
            get { return GetValue<UIElement>(); }
            set { SetValue(value); }
        }

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
        /// 边框线条填充
        /// </summary>
        [UIPropertyMetadata(typeof(ViewFill), "#000", UIPropertyOptions.AffectsRender)]
        public ViewFill BorderFill
        {
            get { return (ViewFill)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置线条类型
        /// </summary>
        [Description("获取或设置线条类型")]
        [UIPropertyMetadata(typeof(Stroke), "1", UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure)]
        public Stroke BorderStroke
        {
            get { return (Stroke)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。
        /// </summary>
        [Description("获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值")]
        [UIPropertyMetadata(typeof(Thickness), "0", UIPropertyOptions.AffectsMeasure)]
        public Thickness Padding
        {
            get { return GetValue<Thickness>(); }
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
        /// 阴影颜色
        /// </summary>
        [Description("阴影颜色")]
        [UIPropertyMetadata(typeof(Color), "#000", UIPropertyOptions.AffectsRender)]
        public Color ShadowColor
        {
            get { return GetValue<Color>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 模糊宽度
        /// </summary>
        [Description("模糊宽度")]
        [UIPropertyMetadata((byte)0, UIPropertyOptions.AffectsMeasure)]
        public byte ShadowBlur
        {
            get { return GetValue<byte>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 阴影水平偏移
        /// </summary>
        [Description("阴影水平偏移")]
        [UIPropertyMetadata((sbyte)0, UIPropertyOptions.AffectsMeasure)]
        public sbyte ShadowHorizontal
        {
            get { return GetValue<sbyte>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 阴影垂直偏移
        /// </summary>
        [Description("阴影垂直偏移")]
        [UIPropertyMetadata((sbyte)0, UIPropertyOptions.AffectsMeasure)]
        public sbyte ShadowVertical
        {
            get { return GetValue<sbyte>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 边框类型
        /// </summary>
        [Description("边框类型")]
        [UIPropertyMetadata(BorderType.BorderStroke, UIPropertyOptions.AffectsMeasure | UIPropertyOptions.AffectsRender)]
        public BorderType BorderType
        {
            get { return GetValue<BorderType>(); }
            set { SetValue(value); }
        }

        [PropertyChanged(nameof(Child))]
        void RegisterChild(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var c = newValue as UIElement;
            if (c != null)
            {
                Children.Add(c);
            }
            var o = oldValue as UIElement;
            if (o != null)
            {
                Children.Remove(o);
            }
        }

        [PropertyChanged(nameof(CornerRadius))]
        [PropertyChanged(nameof(ShadowHorizontal))]
        [PropertyChanged(nameof(ShadowVertical))]
        [PropertyChanged(nameof(ShadowBlur))]
        [PropertyChanged(nameof(ShadowColor))]
        void RegisterShadowBlur(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (shadow != null)
            {
                shadow.Dispose();
                shadow = null;
            }
        }


        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(Child))
        //    {
        //        var c = newValue as UIElement;
        //        if (c != null)
        //        {
        //            Children.Add(c);
        //        }
        //        var o = oldValue as UIElement;
        //        if (o != null)
        //        {
        //            Children.Remove(o);
        //        }
        //    }
        //    else if (propertyName == nameof(ShadowBlur) || propertyName == nameof(ShadowColor))
        //    {
        //        if (shadow != null)
        //        {
        //            shadow.Dispose(); shadow = null;
        //        }
        //    }
        //}

        protected override Size MeasureOverride(in Size availableSize)
        {
            var ava = availableSize;
            var p = Padding;
            Size size;
            var sb = ShadowBlur;
            var sh = ShadowHorizontal;
            var sv = ShadowVertical;
            if (sb > 0)
            {
                ava.Width = Math.Max(0, ava.Width - sb * 2);
                ava.Height = Math.Max(0, ava.Height - sb * 2);
            }
            if (Math.Abs(sh) > sb)
            {
                ava.Width -= Math.Abs(sh) - sb;
            }
            if (Math.Abs(sv) > sb)
            {
                ava.Height -= Math.Abs(sv) - sb;
            }
            if (BorderType == BorderType.BorderStroke)
            {
                var b = BorderStroke;
                ava.Width = Math.Max(0, ava.Width - p.Left - p.Right - b.Width - b.Width);
                ava.Height = Math.Max(0, ava.Height - p.Top - p.Bottom - b.Width - b.Width);
                size = base.MeasureOverride(ava);
                size.Width = p.Left + p.Right + size.Width + b.Width + b.Width;
                size.Height = p.Top + p.Bottom + size.Height + b.Width + b.Width;
            }
            else
            {
                var b = BorderThickness;
                ava.Width = Math.Max(0, ava.Width - p.Left - p.Right - b.Left - b.Right);
                ava.Height = Math.Max(0, ava.Height - p.Top - p.Bottom - b.Top - b.Bottom);
                size = base.MeasureOverride(ava);
                size.Width = p.Left + p.Right + size.Width + b.Left + b.Right;
                size.Height = p.Top + p.Bottom + size.Height + b.Top + b.Bottom;
            }
            if (sb > 0)
            {
                size.Width += sb * 2;
                size.Height += sb * 2;
            }
            if (Math.Abs(sh) > sb)
            {
                size.Width += Math.Abs(sh) - sb;
            }
            if (Math.Abs(sv) > sb)
            {
                size.Height += Math.Abs(sv) - sb;
            }
            return size;
        }

        protected override Size ArrangeOverride(in Size finalSize)
        {
            var rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            var p = Padding;
            if (BorderType == BorderType.BorderStroke)
            {
                var b = BorderStroke;
                rect.Location = new Point(p.Left + b.Width, p.Top + b.Width);
                rect.Size = new Size(Math.Max(0, rect.Width - p.Left - p.Right - b.Width - b.Width), Math.Max(0, rect.Height - p.Top - p.Bottom - b.Width - b.Width));
            }
            else
            {
                var b = BorderThickness;
                rect.Location = new Point(p.Left + b.Left, p.Top + b.Top);
                rect.Size = new Size(Math.Max(0, rect.Width - p.Left - p.Right - b.Left - b.Right), Math.Max(0, rect.Height - p.Top - p.Bottom - b.Top - b.Bottom));
            }
            var sb = ShadowBlur;
            if (sb > 0)
            {
                int sh = ShadowHorizontal;
                int sv = ShadowVertical;
                rect.Size = new Size(Math.Max(0, rect.Width - sb * 2), Math.Max(0, rect.Height - sb * 2));
                if (Math.Abs(sh) > sb)
                {
                    rect.Width -= Math.Abs(sh) - sb;
                    sh = sh < 0 ? -Math.Abs(sh) : sb;
                }
                if (Math.Abs(sv) > sb)
                {
                    rect.Height -= Math.Abs(sv) - sb;
                    sv = sv < 0 ? -Math.Abs(sv) : sb;
                }
                rect.Location = new Point(rect.X + sb - sh, rect.Y + sb - sv);
            }
            foreach (UIElement child in Children)
            {
                child.Arrange(rect);
            }
            return finalSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            var s = ActualSize;
            if (s.Width <= 0 || s.Height <= 0)
            {
                return;
            }
            var cr = CornerRadius;
            var sb = ShadowBlur;
            var sh = ShadowHorizontal;
            var sv = ShadowVertical;
            var sc = ShadowColor;
            var rect = new Rect(sb, sb, Math.Max(0, s.Width - sb * 2), Math.Max(0, s.Height - sb * 2));
            var w = (int)(sb + 1) * 4 + (int)(Math.Max(cr.TopLeft, cr.BottomLeft) + Math.Max(cr.TopRight, cr.BottomRight)) + Math.Abs(sh);
            var h = (int)(sb + 1) * 4 + (int)(Math.Max(cr.TopLeft, cr.TopRight) + Math.Max(cr.BottomLeft, cr.BottomRight)) + Math.Abs(sv);
            if (w >= s.Width || h >= s.Height)
            {
                w = (int)Math.Ceiling(s.Width);
                h = (int)Math.Ceiling(s.Height);
            }
            if (shadow != null && (w != shadow.Width || h != shadow.Height))
            {
                shadow.Dispose();
                shadow = null;
            }
            if ((w - sb * 2) < 1 || (h - sb * 2) < 1)
            {
                return;
            }
            if (BorderType == BorderType.BorderStroke)
            {
                var bs = BorderStroke;
                if (sb > 0 && s.Width > 0 && s.Height > 0)
                {
                    if (shadow == null)
                    {
                        Bitmap bitmap = new Bitmap(w, h);
                        using (var path = new PathGeometry())
                        {
                            var w2 = bs.Width / 2;
                            var r = new Rect(sb, sb, bitmap.Width - (sb) * 2, bitmap.Height - (sb) * 2);
                            path.BeginFigure(w2 + cr.TopLeft + r.Left, w2 + r.Top);
                            path.LineTo(r.Right - w2 - cr.TopRight, w2 + r.Top);
                            path.ArcTo(new Point(r.Right - w2, w2 + cr.TopRight + r.Top), new Size(cr.TopRight, cr.TopRight), 90, true, false);
                            path.LineTo(r.Right - w2, r.Bottom - cr.BottomRight - w2);
                            path.ArcTo(new Point(r.Right - w2 - cr.BottomRight, r.Bottom - w2), new Size(cr.BottomRight, cr.BottomRight), 90, true, false);
                            path.LineTo(w2 + cr.BottomLeft + r.Left, r.Bottom - w2);
                            path.ArcTo(new Point(w2 + r.Left, r.Bottom - w2 - cr.BottomLeft), new Size(cr.BottomLeft, cr.BottomLeft), 90, true, false);
                            path.LineTo(w2 + r.Left, w2 + cr.TopLeft + r.Top);
                            path.ArcTo(new Point(w2 + cr.TopLeft + r.Left, w2 + r.Top), new Size(cr.TopLeft, cr.TopLeft), 90, true, false);
                            path.EndFigure(true);
                            using (var d = DrawingContext.FromBitmap(bitmap))
                            {
                                d.Clear(Color.Transparent);
                                using (var brush = new SolidColorBrush(sc))
                                {
                                    d.FillPath(brush, path);
                                }
                            }
                            shadow = bitmap;
                            StackBlur.ProcessOwner(bitmap, (int)sb);
                            using (var geo = new Geometry(path))
                            {
                                var isZ = cr.IsZero;
                                using (var l = shadow.Lock())
                                {
                                    for (int x = 0; x < w; x++)
                                    {
                                        //if (isZ && x >= ((sb + 1) * 3 + Math.Abs(sh)))
                                        if (isZ && x >= w - sh - sb - 1)
                                        {
                                            continue;
                                        }
                                        for (int y = 0; y < h; y++)
                                        {
                                            //if (isZ && y >= h - sv - sb - 1)
                                            //{
                                            //    continue;
                                            //}
                                            if (geo.Contains(new Point(x + sh, y + sv)))
                                            {
                                                l.SetAlpha(x, y, 0);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (shadow.Width == (int)Math.Ceiling(s.Width) || shadow.Height == (int)Math.Ceiling(s.Height))
                    {
                        dc.DrawImage(shadow, new Rect(0, 0, s.Width, s.Height), new Rect(0, 0, shadow.Width, shadow.Height));
                    }
                    else
                    {
                        SudokuDrawImage(dc, shadow, new Rect(0, 0, s.Width, s.Height), new Thickness(
                            sb * 2 + 1 + Math.Max(cr.TopLeft, cr.BottomLeft) + (-sh > 0 ? -sh : 0),
                            sb * 2 + 1 + Math.Max(cr.TopLeft, cr.TopRight) + (-sv > 0 ? -sv : 0),
                            sb * 2 + 1 + Math.Max(cr.TopRight, cr.BottomRight) + (sh > 0 ? sh : 0),
                            sb * 2 + 1 + Math.Max(cr.BottomRight, cr.BottomLeft) + (sv > 0 ? sv : 0)), false);
                    }
                }

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

                    var matx = dc.Transform;
                    var old = matx;
                    matx.TranslatePrepend(-sh, -sv);
                    dc.Transform = matx;
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
                    dc.Transform = old;
                }

            }
            else
            {
                var bt = BorderThickness;

                if (!cr.IsZero)
                {
                    if (sb > 0 && s.Width > 0 && s.Height > 0)
                    {
                        if (shadow == null)
                        {
                            //Bitmap bitmap = new Bitmap(
                            //   (int)(sb + 1) * 4 + (int)(Math.Max(cr.TopLeft, cr.BottomLeft) + Math.Max(cr.TopRight, cr.BottomRight)) + Math.Abs(sh),
                            //   (int)(sb + 1) * 4 + (int)(Math.Max(cr.TopLeft, cr.TopRight) + Math.Max(cr.BottomLeft, cr.BottomRight)) + Math.Abs(sv));
                            Bitmap bitmap = new Bitmap(w, h);
                            {
                                using (var path = new PathGeometry())
                                {
                                    var w2 = 0.5f;
                                    var r = new Rect(sb, sb, bitmap.Width - (sb) * 2, bitmap.Height - (sb) * 2);
                                    path.BeginFigure(w2 + cr.TopLeft + r.Left, w2 + r.Top);
                                    path.LineTo(r.Right - w2 - cr.TopRight, w2 + r.Top);
                                    path.ArcTo(new Point(r.Right - w2, w2 + cr.TopRight + r.Top), new Size(cr.TopRight, cr.TopRight), 90, true, false);
                                    path.LineTo(r.Right - w2, r.Bottom - cr.BottomRight - w2);
                                    path.ArcTo(new Point(r.Right - w2 - cr.BottomRight, r.Bottom - w2), new Size(cr.BottomRight, cr.BottomRight), 90, true, false);
                                    path.LineTo(w2 + cr.BottomLeft + r.Left, r.Bottom - w2);
                                    path.ArcTo(new Point(w2 + r.Left, r.Bottom - w2 - cr.BottomLeft), new Size(cr.BottomLeft, cr.BottomLeft), 90, true, false);
                                    path.LineTo(w2 + r.Left, w2 + cr.TopLeft + r.Top);
                                    path.ArcTo(new Point(w2 + cr.TopLeft + r.Left, w2 + r.Top), new Size(cr.TopLeft, cr.TopLeft), 90, true, false);
                                    path.EndFigure(true);
                                    using (var d = DrawingContext.FromBitmap(bitmap))
                                    {
                                        d.Clear(Color.Transparent);
                                        using (var brush = new SolidColorBrush(sc))
                                        {
                                            d.FillPath(brush, path);
                                        }
                                    }
                                    shadow = bitmap;
                                    StackBlur.ProcessOwner(bitmap, (int)sb);
                                    using (var geo = new Geometry(path))
                                    {
                                        using (var l = shadow.Lock())
                                        {
                                            for (int x = 0; x < shadow.Width; x++)
                                            {
                                                for (int y = 0; y < shadow.Height; y++)
                                                {
                                                    if (geo.Contains(new Point(x + sh, y + sv)))
                                                    {
                                                        l.SetAlpha(x, y, 0);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //SudokuDrawImage(dc, shadow, new Rect(0, 0, s.Width, s.Height), new Thickness(sb + 1 + Math.Max(cr.TopLeft, cr.BottomLeft), sb + 1 + Math.Max(cr.TopLeft, cr.TopRight), sb + 1 + Math.Max(cr.TopRight, cr.BottomRight), sb + 1 + Math.Max(cr.BottomRight, cr.BottomLeft)), false);
                        if (shadow.Width == (int)Math.Ceiling(s.Width) || shadow.Height == (int)Math.Ceiling(s.Height))
                        {
                            dc.DrawImage(shadow, new Rect(0, 0, s.Width, s.Height), new Rect(0, 0, shadow.Width, shadow.Height));
                        }
                        else
                        {
                            SudokuDrawImage(dc, shadow, new Rect(0, 0, s.Width, s.Height), new Thickness(
                            sb * 2 + 1 + Math.Max(cr.TopLeft, cr.BottomLeft) + (-sh > 0 ? -sh : 0),
                            sb * 2 + 1 + Math.Max(cr.TopLeft, cr.TopRight) + (-sv > 0 ? -sv : 0),
                            sb * 2 + 1 + Math.Max(cr.TopRight, cr.BottomRight) + (sh > 0 ? sh : 0),
                            sb * 2 + 1 + Math.Max(cr.BottomRight, cr.BottomLeft) + (sv > 0 ? sv : 0)), false);
                        }
                    }

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
                            var matx = dc.Transform;
                            var old = matx;
                            matx.TranslatePrepend(-sh, -sv);
                            dc.Transform = matx;
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
                            dc.Transform = old;
                        }
                    }
                }
                else
                {
                    //var sb = ShadowBlur;
                    if (sb > 0 && s.Width > 0 && s.Height > 0)
                    {
                        if (shadow == null)
                        {
                            //Bitmap bitmap = new Bitmap(
                            //   (int)(sb + 1) * 4 + Math.Abs(sh),
                            //   (int)(sb + 1) * 4 + Math.Abs(sv));
                            Bitmap bitmap = new Bitmap(w, h);
                            {
                                Rect rect1 = new Rect((int)(sb), (int)(sb), bitmap.Width - (sb) * 2, bitmap.Height - (sb) * 2);
                                using (var d = DrawingContext.FromBitmap(bitmap))
                                {
                                    d.Clear(Color.Transparent);
                                    using (var brush = new SolidColorBrush(sc))
                                    {
                                        d.FillRectangle(brush, rect1);
                                    }
                                }
                                shadow = bitmap;
                                StackBlur.ProcessOwner(bitmap, (int)sb);
                                rect1.Width -= 1;
                                using (var l = shadow.Lock())
                                {
                                    var isZ = cr.IsZero;
                                    for (int x = 0; x < shadow.Width; x++)
                                    {
                                        for (int y = 0; y < shadow.Height; y++)
                                        {
                                            if (isZ && y >= h - sv - sb)
                                            {
                                                continue;
                                            }
                                            if (rect1.Contains(new Point(x + sh, y + sv)))
                                            {
                                                l.SetAlpha(x, y, 0);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //SudokuDrawImage(dc, shadow, new Rect(0, 0, s.Width, s.Height), new Thickness(sb), false);

                        if (shadow.Width == (int)Math.Ceiling(s.Width) || shadow.Height == (int)Math.Ceiling(s.Height))
                        {
                            dc.DrawImage(shadow, new Rect(0, 0, s.Width, s.Height), new Rect(0, 0, shadow.Width, shadow.Height));
                        }
                        else
                        {
                            SudokuDrawImage(dc, shadow, new Rect(0, 0, s.Width, s.Height), new Thickness(
                            sb * 2 + 1 + (-sh > 0 ? -sh : 0),
                            sb * 2 + 1 + (-sv > 0 ? -sv : 0),
                            sb * 2 + 1 + (sh > 0 ? sh : 0),
                            sb * 2 + 1 + (sv > 0 ? sv : 0)), false);
                        }
                    }
                    var bf = BorderFill;
                    if (UseLayoutRounding || !IsAntiAlias)
                    {
                        var rs = Root.RenderScaling;
                        bt.Left = (float)Math.Round(bt.Left * rs) / rs;
                        bt.Right = (float)Math.Round(bt.Right * rs) / rs;
                        bt.Top = (float)Math.Round(bt.Top * rs) / rs;
                        bt.Bottom = (float)Math.Round(bt.Bottom * rs) / rs;
                    }
                    var matx = dc.Transform;
                    var old = matx;
                    matx.TranslatePrepend(-sh, -sv);
                    dc.Transform = matx;
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
                    dc.Transform = old;
                }
            }
        }
        Bitmap shadow;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (shadow != null)
            {
                shadow.Dispose();
                shadow = null;
            }
        }

        /// <summary>
        /// 九宫格绘制图片
        /// </summary>
        /// <param name="g"></param>
        /// <param name="img"></param>
        /// <param name="rect">绘制区域</param>
        /// <param name="padding">裁减宽度</param>
        /// <param name="drawCenter">是否绘制中间</param>
        public static void SudokuDrawImage(DrawingContext g, Image img, Rect rect, Thickness padding, bool drawCenter)
        {
            if (padding.Horizontal > rect.Width || padding.Vertical > rect.Height)
            {
                g.DrawImage(img, new Rect(rect.X, rect.Y, rect.Width, rect.Height), new Rect(0, 0, img.Width, img.Height));
                return;
            }
            //填充四个角
            g.DrawImage(img, new Rect(rect.X, rect.Y, padding.Left, padding.Top),
             new Rect(0, 0, padding.Left, padding.Top));
            g.DrawImage(img, new Rect(rect.Right - padding.Right, rect.Y, padding.Right, padding.Top),
               new Rect(img.Width - padding.Right, 0, padding.Right, padding.Top));

            g.DrawImage(img, new Rect(rect.X, rect.Bottom - padding.Bottom, padding.Left, padding.Bottom),
                new Rect(0, img.Height - padding.Bottom, padding.Left, padding.Bottom));

            g.DrawImage(img, new Rect(rect.Right - padding.Right, rect.Bottom - padding.Bottom, padding.Right, padding.Bottom),
                new Rect(img.Width - padding.Right, img.Height - padding.Bottom, padding.Right, padding.Bottom));

            //四边
            g.DrawImage(img, new Rect(rect.X, rect.Y + padding.Top, padding.Left, rect.Height - padding.Vertical),
            new Rect(0, padding.Top, padding.Left, img.Height - padding.Vertical));


            g.DrawImage(img, new Rect(rect.X + padding.Left, rect.Y, rect.Width - padding.Horizontal, padding.Top),
                new Rect(padding.Left, 0, img.Width - padding.Horizontal, padding.Top));

            g.DrawImage(img, new Rect(rect.Right - padding.Right, rect.Y + padding.Top, padding.Right, rect.Height - padding.Vertical),
             new Rect(img.Width - padding.Right, padding.Top, padding.Right, img.Height - padding.Vertical));

            g.DrawImage(img, new Rect(rect.X + padding.Left, rect.Bottom - padding.Bottom, rect.Width - padding.Horizontal, padding.Bottom),
             new Rect(padding.Left, img.Height - padding.Bottom, img.Width - padding.Horizontal, padding.Bottom));


            if (drawCenter)
            {
                //中间
                g.DrawImage(img,
                    new Rect(rect.X + padding.Left, rect.Y + padding.Top, rect.Width - padding.Horizontal, rect.Height - padding.Vertical),
                    new Rect(padding.Left, padding.Top, img.Width - padding.Horizontal, img.Height - padding.Vertical));
            }

        }

    }

    public enum BorderType : byte
    {
        /// <summary>
        /// 四周边框一样粗，BorderStroke属性定义粗细样式，支持设置虚线
        /// </summary>
        BorderStroke,
        /// <summary>
        /// 四周边框可以不同粗细，BorderThickness属性定义四周粗细，如果定义圆角了可能会有锯齿
        /// </summary>
        BorderThickness,
    }
}
