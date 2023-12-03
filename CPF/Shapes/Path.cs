using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Controls;
using CPF.Drawing;

namespace CPF.Shapes
{
    /// <summary>
    /// 绘制一系列相互连接的直线和曲线。
    /// </summary>
    [Description("绘制一系列相互连接的直线和曲线。")]
    public class Path : Shape
    {
        /// <summary>
        /// 路径数据，支持WPF里的字符串格式数据
        /// </summary>
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsMeasure | UIPropertyOptions.AffectsRender)]
        public PathGeometry Data { get { return GetValue<PathGeometry>(); } set { SetValue(value); } }

        public Path()
        { }
        /// <summary>
        /// 支持WPF里的字符串格式数据
        /// </summary>
        /// <param name="path"></param>
        public Path(CPF.Drawing.PathGeometry path)
        {
            Data = path;
        }
        /// <summary>
        /// 获取或设置 Stretch 模式，该模式确定内容适应可用空间的方式。
        /// </summary>
        [Description("获取或设置 Stretch 模式，该模式确定内容适应可用空间的方式。")]
        [UIPropertyMetadata(Stretch.None, UIPropertyOptions.AffectsMeasure | UIPropertyOptions.AffectsRender)]
        public Stretch Stretch
        {
            get { return GetValue<Stretch>(); }
            set { SetValue(value); }
        }

        [PropertyChanged(nameof(Stretch))]
        void OnStretch(object newValue, object oldValue, PropertyMetadataAttribute propertyTabAttribute)
        {
            InvalidateGeometry();
        }

        //bool disposeGeometryOnInvalidateGeometry = false;
        //protected override bool DisposeGeometryOnInvalidateGeometry => disposeGeometryOnInvalidateGeometry;

        protected override CPF.Drawing.PathGeometry CreateDefiningGeometry()
        {
            var data = Data;
            if (data == null)
            {
                return new Drawing.PathGeometry();
            }
            var clone = data.Clone();

            var pathBounds = data.GetBounds();

            Matrix transform;

            if (this.Stretch == Stretch.None)
            {
                transform = Matrix.Identity;
            }
            else
            {
                transform = Matrix.Identity;
                var calculatedWidth = (float)(this.Width.Value / pathBounds.Width);
                var calculatedHeight = (float)(this.Height.Value / pathBounds.Height);
                var widthScale = float.IsNaN(calculatedWidth) ? 1 : calculatedWidth;
                var heightScale = float.IsNaN(calculatedHeight) ? 1 : calculatedHeight;

                switch (Stretch)
                {
                    case Stretch.None:
                        break;
                    case Stretch.Fill:
                        transform.Scale(widthScale, heightScale);
                        transform.TranslatePrepend(
                            (float)(-pathBounds.Left),
                            (float)(-pathBounds.Top));
                        break;
                    case Stretch.Uniform:
                        var minScale = Math.Min(widthScale, heightScale);
                        transform.Scale(minScale, minScale);
                        var l = -pathBounds.Left;
                        var t = -pathBounds.Top;
                        transform.TranslatePrepend(l, t);
                        if (!Width.IsAuto && Width.Unit == Unit.Default)
                        {
                            l = (float)((this.Width.Value - minScale * pathBounds.Width) / 2);
                        }
                        else
                        {
                            l = 0;
                        }
                        if (!Height.IsAuto && Width.Unit == Unit.Default)
                        {
                            t = (float)((this.Height.Value - minScale * pathBounds.Height) / 2);
                        }
                        else
                        {
                            t = 0;
                        }
                        transform.Translate(l, t);
                        break;
                    case Stretch.UniformToFill:
                        var maxScale = Math.Max(widthScale, heightScale);
                        transform.Scale(maxScale, maxScale);
                        transform.TranslatePrepend((float)(-pathBounds.Left), (float)(-pathBounds.Top));
                        break;
                }
            }

            if (!transform.IsIdentity)
                clone.Transform(transform);

            return clone;
        }

        //[PropertyChanged(nameof(Data))]
        //void OnData(object newValue, object oldValue, PropertyMetadataAttribute propertyTabAttribute)
        //{
        //    //disposeGeometryOnInvalidateGeometry = true;
        //    //InvalidateGeometry();
        //    //disposeGeometryOnInvalidateGeometry = false;
        //}
    }
}
