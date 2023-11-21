using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;

namespace CPF.Shapes
{
    /// <summary>
    /// 绘制椭圆形。
    /// </summary>
    [Description("绘制椭圆形")]
    public class Ellipse : Shape
    {
        protected override CPF.Drawing.PathGeometry CreateDefiningGeometry()
        {
            Size size = ActualSize;
            //var p = Padding;
            Stroke s = StrokeStyle;
            var w = size.Width - s.Width * 2;//- p.Left - p.Right;
            var h = size.Height - s.Width * 2;
            //var width = s.Width * 2;
            //if (w < width + p.Left + p.Right)
            //{
            //    w = width;
            //}
            //if (h < width + p.Top + p.Bottom)
            //{
            //    h = width;
            //}

            CPF.Drawing.PathGeometry path = new CPF.Drawing.PathGeometry();
            if (w > 0 && h > 0)
            {
                double controlPointRatio = (Math.Sqrt(2) - 1) * 4 / 3;
                //Rect rect = new Rect(s.Width / 2, s.Width / 2, w, h);
                var center = new Point(w / 2 + s.Width, h / 2 + s.Width);
                var radius = new Vector((w + s.Width) / 2, (h + s.Width) / 2);

                var x0 = center.X - radius.X;
                var x1 = center.X - (radius.X * controlPointRatio);
                var x2 = center.X;
                var x3 = center.X + (radius.X * controlPointRatio);
                var x4 = center.X + radius.X;

                var y0 = center.Y - radius.Y;
                var y1 = center.Y - (radius.Y * controlPointRatio);
                var y2 = center.Y;
                var y3 = center.Y + (radius.Y * controlPointRatio);
                var y4 = center.Y + radius.Y;

                path.BeginFigure(x2, y0);
                path.CubicTo(new Point((float)x3, y0), new Point(x4, (float)y1), new Point(x4, y2));
                path.CubicTo(new Point(x4, (float)y3), new Point((float)x3, y4), new Point(x2, y4));
                path.CubicTo(new Point((float)x1, y4), new Point(x0, (float)y3), new Point(x0, y2));
                path.CubicTo(new Point(x0, (float)y1), new Point((float)x1, y0), new Point(x2, y0));
                path.EndFigure(true);
            }
            else
            {
                path.BeginFigure(0, 0);
                path.EndFigure(true);
            }
            return path;
        }
        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    var w = StrokeStyle.Width * 2;
        //    var size = base.MeasureOverride(availableSize);
        //    if (size.Width < w || size.Height < w)
        //    {
        //        return new Size(w, w);
        //    }
        //    return size;
        //}
    }
}
