using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    /// <summary>
    /// 径向渐变画笔
    /// </summary>
    public class RadialGradientBrush : Brush
    {

        /// <summary>
        /// 径向渐变画笔
        /// </summary>
        public RadialGradientBrush(Point center, float radius, GradientStop[] blendColors,Matrix matrix)
        {
            Center = center;
            Radius = radius;
            this.blendColors = blendColors;
            this.Matrix = matrix;
        }
        /// <summary>
        /// 获取或设置径向渐变的最外面圆的中心。
        /// </summary>
        public Point Center { get; }
        /// <summary>
        /// 获取或设置径向渐变的最外面圆半径。
        /// </summary>
        public float Radius { get; }

        public Matrix Matrix { get; }

        GradientStop[] blendColors;

        public GradientStop[] BlendColors
        {
            get { return blendColors; }
        }

    }
}
