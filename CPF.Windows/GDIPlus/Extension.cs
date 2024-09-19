//#if Net4
using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.GDIPlus
{
    public static class Extension
    {
        public static System.Drawing.Color ToGdiColor(this Color color)
        {
            return System.Drawing.Color.FromArgb(color.A == 255 ? 254 : color.A, color.R, color.G, color.B);
        }

        public static Color ToColor(this System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static System.Drawing.PointF ToGdiPoint(this Point p)
        {
            return new System.Drawing.PointF((float)p.X, (float)p.Y);
        }


        public static DashStyles ToDashStyles(this System.Drawing.Drawing2D.DashStyle d)
        {
            return (DashStyles)(int)d;
        }

        public static System.Drawing.Drawing2D.DashStyle ToGdiDashStyle(this DashStyles d)
        {
            return (System.Drawing.Drawing2D.DashStyle)(int)d;
        }

        public static System.Drawing.Drawing2D.LineCap ToLineCap(this CapStyles c)
        {
            switch (c)
            {
                case CapStyles.Flat:
                    return System.Drawing.Drawing2D.LineCap.Flat;
                case CapStyles.Square:
                    return System.Drawing.Drawing2D.LineCap.Square;
                case CapStyles.Round:
                    return System.Drawing.Drawing2D.LineCap.Round;
                case CapStyles.Triangle:
                    return System.Drawing.Drawing2D.LineCap.Triangle;
                default:
                    return System.Drawing.Drawing2D.LineCap.Flat;
            }
        }

        public static FontStyles ToFontStyles(this System.Drawing.FontStyle fontStyle)
        {
            return (FontStyles)(int)fontStyle;
        }
        public static System.Drawing.FontStyle ToFontStyle(this FontStyles fontStyle)
        {
            return (System.Drawing.FontStyle)(int)fontStyle;
        }

        public static Rect ToRect(this System.Drawing.Rectangle rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static Rect ToRect(this System.Drawing.RectangleF rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static System.Drawing.RectangleF ToRectangle(this Rect rect)
        {
            return new System.Drawing.RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static Matrix ToMatrix(this System.Drawing.Drawing2D.Matrix matrix)
        {
            return new Matrix(matrix.Elements[0], matrix.Elements[1], matrix.Elements[2], matrix.Elements[3], matrix.Elements[4], matrix.Elements[5]);
        }
        public static System.Drawing.Drawing2D.Matrix ToMatrix(this Matrix matrix)
        {
            return new System.Drawing.Drawing2D.Matrix((float)matrix.M11, (float)matrix.M12, (float)matrix.M21, (float)matrix.M22, (float)matrix.OffsetX, (float)matrix.OffsetY); 
        }
    }
}
//#endif