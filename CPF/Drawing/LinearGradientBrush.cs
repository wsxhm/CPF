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

        public LinearGradientBrush(GradientStop[] blendColors, Point startPoint, Point endPoint, Matrix matrix)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
            this.blendColors = blendColors;
            Matrix = matrix;
        }
        //public LinearGradientBrush(GradientStop[] blendColors, float angle, Matrix matrix)
        //{
        //    this.StartPoint = new Point();
        //    this.EndPoint = EndPointFromAngle(angle);
        //    this.blendColors = blendColors;
        //    Matrix = matrix;
        //}

        private Point EndPointFromAngle(float angle)
        {
            // Convert the angle from degrees to radians
            angle = (float)(angle * (1.0 / 180.0) * System.Math.PI);
            return (new Point((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)));
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
