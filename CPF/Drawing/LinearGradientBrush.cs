using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    /// <summary>
    /// 线性渐变笔刷，默认是左到右
    /// </summary>
    public class LinearGradientBrush : Brush
    {
        //public LinearGradientBrush(Color color1, Color color2, float width)
        //{
        //    //this.StartPoint = point1;
        //    //this.EndPoint = point2;
        //    Width = width;
        //    blendColors = new GradientStop[] { new GradientStop(color1, 0), new GradientStop(color2, 1) };
        //}

        public LinearGradientBrush(GradientStop[] blendColors, Point startPoint, Point endPoint,Matrix matrix)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
            this.blendColors = blendColors;
            Matrix = matrix;
        }

        GradientStop[] blendColors;

        public GradientStop[] BlendColors
        {
            get { return blendColors; }
        }


        public Matrix Matrix { get; }

        //public float Width { get; }

        public Point EndPoint { get; }

        public Point StartPoint { get; }

    }
}
