using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Svg
{
    public interface ISvgShape
    {
        PathGeometry Geometry { get; }
        Transform Transform { get; }
        SvgType SvgType { get; }
        /// <summary>
        /// Group的时候才有内容
        /// </summary>
        IEnumerable<ISvgShape> Elements { get; }
    }

    public enum SvgType : byte
    {
        Shape,
        /// <summary>
        /// 需要通过SVG对象的GetShape来获取具体的Shape
        /// </summary>
        Use,
        Group
    }
}
