using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;

namespace CPF.Shapes
{
    /// <summary>
    /// 为 Ellipse、Polygon 和 Rectangle 之类的形状元素提供基类。
    /// </summary>
    [Description("为 Ellipse、Polygon 和 Rectangle 之类的形状元素提供基类。")]
    public abstract class Shape : UIElement
    {

        Drawing.PathGeometry definingGeometry;
        /// <summary>
        /// 图形内部填充
        /// </summary>
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public ViewFill Fill
        {
            get { return (ViewFill)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 边框线条填充
        /// </summary>
        [UIPropertyMetadata(typeof(ViewFill), "black", UIPropertyOptions.AffectsRender)]
        public ViewFill StrokeFill
        {
            get { return (ViewFill)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置线条类型
        /// </summary>
        [UIPropertyMetadata(typeof(Stroke), "1", UIPropertyOptions.AffectsRender)]
        public Stroke StrokeStyle
        {
            get { return (Stroke)GetValue(); }
            set { SetValue(value); }
        }

        //public Thickness Padding
        //{
        //    get { return GetValue(PaddingProperty); }
        //    set { SetValue(PaddingProperty, value); }
        //}

        //   protected Thickness Padding
        //   {
        //       get
        //       {
        //           var s = ActualSize;
        //           var pl = PaddingLeft;
        //           var pt = PaddingTop;
        //           var pr = PaddingRight;
        //           var pb = PaddingBottom;
        //           return new Thickness(
        //pl.IsAuto ? 0 : (pl.Unit == Unit.Default ? pl.Value : pl.Value * s.Width),
        //pt.IsAuto ? 0 : (pt.Unit == Unit.Default ? pt.Value : pt.Value * s.Height),
        //pr.IsAuto ? 0 : (pr.Unit == Unit.Default ? pr.Value : pr.Value * s.Width),
        //pb.IsAuto ? 0 : (pb.Unit == Unit.Default ? pb.Value : pb.Value * s.Height));
        //       }
        //   }
        /// <summary>
        /// 调用InvalidateGeometry的时候是否释放Geometry
        /// </summary>
        protected virtual bool DisposeGeometryOnInvalidateGeometry
        {
            get { return true; }
        }

        /// <summary>
        /// 定义的图形
        /// </summary>
        public Drawing.PathGeometry DefiningGeometry
        {
            get
            {
                if (definingGeometry == null)
                {
                    definingGeometry = CreateDefiningGeometry();
                    if (VisualClip != null)
                    {
                        VisualClip.Dispose();
                    }

                    if (IsHitTestOnPath)
                    {
                        VisualClip = new Geometry(definingGeometry.CreateStrokePath(StrokeStyle.Width));
                    }
                    else
                    {
                        VisualClip = new Geometry(definingGeometry);
                    }
                }
                return definingGeometry;
            }
        }
        /// <summary>
        /// 事件响应范围是路径的线条上还是路径围成的范围内，true就是在线条上
        /// </summary>
        [Description("事件响应范围是路径的线条上还是路径围成的范围内，true就是在线条上")]
        public bool IsHitTestOnPath
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        [PropertyChanged(nameof(IsHitTestOnPath))]
        void OnHitTestOnPath(object newValue, object oldValue, PropertyMetadataAttribute propertyTabAttribute)
        {
            InvalidateGeometry();
        }
        [PropertyChanged(nameof(StrokeStyle))]
        void OnStrokeStyle(object newValue, object oldValue, PropertyMetadataAttribute propertyTabAttribute)
        {
            InvalidateGeometry();
        }
        protected override void OnRender(DrawingContext dc)
        {
            Size size = ActualSize;
            if (size.Width > 0 && size.Height > 0)
            {
                Drawing.PathGeometry geometry = DefiningGeometry;
                if (geometry != null)
                {
                    //var p = Padding;
                    var m = dc.Transform;
                    //m.Translate(p.Left, p.Top);
                    dc.Transform = m;
                    var f = Fill;
                    if (f != null)
                    {
                        using (var fill = f.CreateBrush(new Rect(0, 0, size.Width, size.Height), Root.RenderScaling))
                        {
                            dc.FillPath(fill, geometry);
                        }
                    }
                    var stroke = StrokeFill;
                    if (stroke != null)
                    {
                        using (var s = stroke.CreateBrush(new Rect(0, 0, size.Width, size.Height), Root.RenderScaling))
                        {
                            var ss = StrokeStyle;
                            if (UseLayoutRounding)
                            {
                                var sc = Root.RenderScaling;
                                ss.Width = (float)Math.Round(ss.Width * sc) / sc;
                            }
                            if (ss.Width > 0)
                            {
                                dc.DrawPath(s, ss, geometry);
                            }
                        }
                    }
                }
            }
            base.OnRender(dc);
        }

        protected abstract Drawing.PathGeometry CreateDefiningGeometry();

        protected void InvalidateGeometry()
        {
            if (definingGeometry != null && DisposeGeometryOnInvalidateGeometry)
            {
                definingGeometry.Dispose();
            }
            definingGeometry = null;
            InvalidateMeasure();
        }

        Size oldSize;
        protected override void OnLayoutUpdated()
        {
            if (oldSize != ActualSize)
            {
                oldSize = ActualSize;
                InvalidateGeometry();
            }
            base.OnLayoutUpdated();
        }

        protected override Size MeasureOverride(in Size availableSize)
        {
            //var wv = Width;
            //var hv = Height;
            //var w = wv.Unit == Unit.Default ? wv.Value : wv.Value * availableSize.Width;
            //var h = hv.Unit == Unit.Default ? hv.Value : hv.Value * availableSize.Height;
            //if (float.IsNaN(w) || float.IsNaN(h))
            //{
            //    //var p = Padding;
            //    var rect = DefiningGeometry.GetBounds();
            //    var stroke = StrokeStyle;
            //    if (wv.IsAuto)
            //    {
            //        w = rect.Width + rect.Left + stroke.Width;
            //    }
            //    if (hv.IsAuto)
            //    {
            //        h = rect.Height + rect.Top + stroke.Width;
            //    }
            //}
            //if (!float.IsNaN(w) || !float.IsNaN(h))
            //{
            //    var size = base.MeasureOverride(availableSize);
            //    return new Size(Math.Max(size.Width, w), Math.Max(size.Height, h));
            //}
            //else
            //{
            //    return new Size(w, h);
            //}
            var rect = DefiningGeometry.GetBounds();
            var stroke = StrokeStyle;
            var size = new Size(Math.Max(1, rect.Width + stroke.Width + rect.Left), Math.Max(1, rect.Height + stroke.Width + rect.Top));
            return size;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (definingGeometry != null)
            {
                definingGeometry.Dispose();
                definingGeometry = null;
            }
        }
    }
}
