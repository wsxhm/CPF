using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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

        bool disposeGeometryOnInvalidateGeometry = false;
        protected override bool DisposeGeometryOnInvalidateGeometry => disposeGeometryOnInvalidateGeometry;

        protected override CPF.Drawing.PathGeometry CreateDefiningGeometry()
        {
            var data = Data;
            if (data == null)
            {
                return new Drawing.PathGeometry();
            }
            return data;
        }

        //[PropertyChanged(nameof(Data))]
        //void OnData(object newValue, object oldValue, PropertyMetadataAttribute propertyTabAttribute)
        //{
        //    disposeGeometryOnInvalidateGeometry = true;
        //    InvalidateGeometry();
        //    disposeGeometryOnInvalidateGeometry = false;
        //}
    }
}
