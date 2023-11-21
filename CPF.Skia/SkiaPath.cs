using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkiaSharp;
using CPF.Drawing;

namespace CPF.Skia
{
    public class SkiaPath : IPathImpl
    {
        private readonly SKPath path;

        public SkiaPath()
        {
            path = new SKPath();
        }

        public SkiaPath(SKPath path)
        {
            this.path = path;
        }

        public SkiaPath(in Font font, string text)
        {
            using (SKPaint paint = new SKPaint())
            {
                paint.TextEncoding = SKTextEncoding.Utf16;
                paint.Typeface = (font.AdapterFont as FontWrapper).SKTypeface;
                paint.TextSize = font.FontSize;
                path = paint.GetTextPath(text, 0, -paint.FontMetrics.Ascent);
            }
        }

        public SKPath SKPath
        {
            get { return path; }
        }

        public FillRule FillRule
        {
            get { return path.FillType == SKPathFillType.EvenOdd ? FillRule.EvenOdd : FillRule.NonZero; }
            set { path.FillType = value == FillRule.NonZero ? SKPathFillType.Winding : SKPathFillType.EvenOdd; }
        }

        public void ArcTo(Point point, Size size, float rotationAngle, bool isClockwise, bool isLargeArc)
        {
            path.ArcTo(
                size.Width,
                size.Height,
                rotationAngle,
                isLargeArc ? SKPathArcSize.Large : SKPathArcSize.Small,
                isClockwise ? SKPathDirection.Clockwise : SKPathDirection.CounterClockwise,
                point.X,
                point.Y);
        }

        public void CubicTo(Point p1, Point p2, Point p3)
        {
            path.CubicTo(p1.ToSKPoint(), p2.ToSKPoint(), p3.ToSKPoint());
        }

        public void BeginFigure(float x, float y)
        {
            path.MoveTo(x, y);
        }

        public void Dispose()
        {
            path.Dispose();
        }

        public void EndFigure(bool closeFigure)
        {
            if (closeFigure)
            {
                path.Close();
            }
        }

        public Rect GetBounds()
        {
            path.GetBounds(out SKRect rect);
            return rect.ToRect();
        }

        public void LineTo(float x, float y)
        {
            path.LineTo(x, y);
        }

        public void QuadTo(Point p1, Point p2)
        {
            path.QuadTo(p1.ToSKPoint(), p2.ToSKPoint());
        }

        public void Transform(Matrix matrix)
        {
            path.Transform(matrix.ToMatrix());
        }

        public object Clone()
        {
            return new SkiaPath(new SKPath(path));
        }

        public void AddPath(PathGeometry path, bool connect)
        {
            this.path.AddPath((path.PathIml as SkiaPath).path, connect ? SKPathAddMode.Extend : SKPathAddMode.Append);
        }

        public bool Contains(float x, float y)
        {
            return path.Contains(x, y);
        }

        public IPathImpl CreateStrokePath(float strokeWidth)
        {
            using (var paint = new SKPaint())
            {
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = strokeWidth;
                var p = new SKPath();
                var fill = paint.GetFillPath(path, p);
                return new SkiaPath(p);
            }
        }
    }
}
