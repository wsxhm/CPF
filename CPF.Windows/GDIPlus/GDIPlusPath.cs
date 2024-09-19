//#if Net4
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;
using CPF.Drawing;

namespace CPF.GDIPlus
{
    public class GDIPlusPath : IPathImpl
    {
        System.Drawing.Drawing2D.GraphicsPath path;
        public GDIPlusPath()
        {
            path = new System.Drawing.Drawing2D.GraphicsPath();
        }
        public GDIPlusPath(System.Drawing.Drawing2D.GraphicsPath path)
        {
            this.path = path;
        }
        static System.Drawing.StringFormat stringFormat = System.Drawing.StringFormat.GenericTypographic;
        public GDIPlusPath(in Font font, string text)
        {
            path = new System.Drawing.Drawing2D.GraphicsPath();
            var f = font.AdapterFont as FontStruct;
            path.AddString(text, f.Font.FontFamily, (int)f.Font.Style, f.Font.Size, new System.Drawing.PointF(), stringFormat);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">终点</param>
        /// <param name="size"></param>
        /// <param name="rotationAngle"></param>
        /// <param name="isClockwise"></param>
        /// <param name="isLargerArc"></param>
        public void ArcTo(Point point, Size size, float rotationAngle, bool isClockwise, bool isLargerArc)
        {
            if (_lastPoint == point)
            {
                // If the endpoints (x, y) and (x0, y0) are identical, then this
                // is equivalent to omitting the elliptical arc segment entirely.
            }
            else if (size.Width == 0 || size.Height == 0)
            {
                // Ensure radii are valid
                path.AddLine(_lastPoint.X, _lastPoint.Y, point.X, point.Y);
            }
            else
            {
                CalculatedArcValues calcValues = GetCalculatedArcValues(_lastPoint, point, size, rotationAngle, isClockwise, isLargerArc);

                GraphicsPath subPath = new GraphicsPath();
                subPath.StartFigure();
                subPath.AddArc((float)(calcValues.Cx - calcValues.CorrRx),
                    (float)(calcValues.Cy - calcValues.CorrRy),
                    (float)calcValues.CorrRx * 2, (float)calcValues.CorrRy * 2,
                    (float)calcValues.AngleStart, (float)calcValues.AngleExtent);

                System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
                matrix.Translate(-(float)calcValues.Cx, -(float)calcValues.Cy);
                subPath.Transform(matrix);

                matrix = new System.Drawing.Drawing2D.Matrix();
                matrix.Rotate((float)rotationAngle);
                subPath.Transform(matrix);

                matrix = new System.Drawing.Drawing2D.Matrix();
                matrix.Translate((float)calcValues.Cx, (float)calcValues.Cy);
                subPath.Transform(matrix);

                path.AddPath(subPath, true);
            }

            _lastPoint = point;
            //throw new NotImplementedException("我曹，太麻烦了");
        }

        private Point _lastPoint;

        public System.Drawing.Drawing2D.GraphicsPath Path
        {
            get { return path; }
        }

        public FillRule FillRule
        {
            get => path.FillMode == System.Drawing.Drawing2D.FillMode.Alternate ? FillRule.EvenOdd : FillRule.NonZero;
            set => path.FillMode = value == FillRule.NonZero ? System.Drawing.Drawing2D.FillMode.Winding : System.Drawing.Drawing2D.FillMode.Alternate;
        }

        public void LineTo(float x, float y)
        {
            path.AddLine(_lastPoint.X, _lastPoint.Y, x, y);
            _lastPoint = new Point(x, y);
        }

        public void BeginFigure(float x, float y)
        {
            _lastPoint = new Point(x, y);
        }

        public void EndFigure(bool closeFigure)
        {
            if (closeFigure)
            {
                path.CloseFigure();
            }
        }

        public void Dispose()
        {
            if (path != null)
            {
                path.Dispose();
                path = null;
            }
        }

        public Rect GetBounds()
        {
            return path.GetBounds().ToRect();
        }

        public void CubicTo(Point p1, Point p2, Point p3)
        {
            path.AddBezier(_lastPoint.ToGdiPoint(), p1.ToGdiPoint(), p2.ToGdiPoint(), p3.ToGdiPoint());
            _lastPoint = p3;
        }

        public void QuadTo(Point p1, Point p2)
        {
            path.AddBezier(_lastPoint.ToGdiPoint(), _lastPoint.ToGdiPoint(), p1.ToGdiPoint(), p2.ToGdiPoint());
            _lastPoint = p2;
        }

        CalculatedArcValues GetCalculatedArcValues(Point lastPoint, Point point, Size size, float rotationAngle, bool isClockwise, bool isLargerArc)
        {
            CalculatedArcValues calcVal = new CalculatedArcValues();

            /*
             *	This algorithm is taken from the Batik source. All cudos to the Batik crew.
             */

            var startPoint = lastPoint;
            var endPoint = point;

            double x0 = startPoint.X;
            double y0 = startPoint.Y;

            double x = endPoint.X;
            double y = endPoint.Y;

            // Compute the half distance between the current and the final point
            double dx2 = (x0 - x) / 2.0;
            double dy2 = (y0 - y) / 2.0;

            // Convert angle from degrees to radians
            double radAngle = rotationAngle * Math.PI / 180;
            double cosAngle = Math.Cos(radAngle);
            double sinAngle = Math.Sin(radAngle);

            //
            // Step 1 : Compute (x1, y1)
            //
            double x1 = (cosAngle * dx2 + sinAngle * dy2);
            double y1 = (-sinAngle * dx2 + cosAngle * dy2);

            // Ensure radii are large enough
            double rx = Math.Abs(size.Width);
            double ry = Math.Abs(size.Height);

            double Prx = rx * rx;
            double Pry = ry * ry;
            double Px1 = x1 * x1;
            double Py1 = y1 * y1;

            // check that radii are large enough
            double radiiCheck = Px1 / Prx + Py1 / Pry;
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;
                Prx = rx * rx;
                Pry = ry * ry;
            }

            //
            // Step 2 : Compute (cx1, cy1)
            //
            double sign = (isLargerArc == isClockwise) ? -1 : 1;
            double sq = ((Prx * Pry) - (Prx * Py1) - (Pry * Px1)) / ((Prx * Py1) + (Pry * Px1));
            sq = (sq < 0) ? 0 : sq;
            double coef = (sign * Math.Sqrt(sq));
            double cx1 = coef * ((rx * y1) / ry);
            double cy1 = coef * -((ry * x1) / rx);

            //
            // Step 3 : Compute (cx, cy) from (cx1, cy1)
            //
            double sx2 = (x0 + x) / 2.0;
            double sy2 = (y0 + y) / 2.0;
            double cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
            double cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);

            //
            // Step 4 : Compute the angleStart (angle1) and the angleExtent (dangle)
            //
            double ux = (x1 - cx1); // rx;
            double uy = (y1 - cy1); // ry;
            double vx = (-x1 - cx1); // rx;
            double vy = (-y1 - cy1); // ry;
            double p, n;
            // Compute the angle start
            n = Math.Sqrt((ux * ux) + (uy * uy));
            p = ux; // (1 * ux) + (0 * uy)
            sign = (uy < 0) ? -1d : 1d;
            double angleStart = sign * Math.Acos(p / n);
            angleStart = angleStart * 180 / Math.PI;

            // Compute the angle extent
            n = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            p = ux * vx + uy * vy;
            sign = (ux * vy - uy * vx < 0) ? -1d : 1d;
            double angleExtent = sign * Math.Acos(p / n);
            angleExtent = angleExtent * 180 / Math.PI;

            if (!isClockwise && angleExtent > 0)
            {
                angleExtent -= 360f;
            }
            else if (isClockwise && angleExtent < 0)
            {
                angleExtent += 360f;
            }
            angleExtent %= 360f;
            angleStart %= 360f;

            calcVal.CorrRx = rx;
            calcVal.CorrRy = ry;
            calcVal.Cx = cx;
            calcVal.Cy = cy;
            calcVal.AngleStart = angleStart;
            calcVal.AngleExtent = angleExtent;

            return calcVal;
        }

        public void Transform(Drawing.Matrix matrix)
        {
            path.Transform(matrix.ToMatrix());
        }

        public object Clone()
        {
            return new GDIPlusPath((GraphicsPath)path.Clone());
        }

        public void AddPath(PathGeometry path, bool connect)
        {
            this.path.AddPath((path.PathIml as GDIPlusPath).path, connect);
        }

        public bool Contains(float x, float y)
        {
            return path.IsVisible(x, y);
        }

        public IPathImpl CreateStrokePath(float strokeWidth)
        {
            var p = (GraphicsPath)path.Clone();
            p.Widen(new System.Drawing.Pen(System.Drawing.Color.Black, strokeWidth));
            return new GDIPlusPath(p);
        }

        public void Reset()
        {
            path.Reset();
        }

        ~GDIPlusPath()
        {
            Dispose();
        }
    }

    public struct CalculatedArcValues
    {
        public double CorrRx;
        public double CorrRy;
        public double Cx;
        public double Cy;
        public double AngleStart;
        public double AngleExtent;

        public CalculatedArcValues(double rx, double ry, double cx, double cy,
            double angleStart, double angleExtent)
        {
            this.CorrRx = rx;
            this.CorrRy = ry;
            this.Cx = cx;
            this.Cy = cy;
            this.AngleStart = angleStart;
            this.AngleExtent = angleExtent;
        }
    }
}
//#endif