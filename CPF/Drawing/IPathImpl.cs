using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    /// <summary>
    /// Path内部实现接口
    /// </summary>
    public interface IPathImpl : IDisposable, ICloneable
    {
        void BeginFigure(float x, float y);
        void LineTo(float x, float y);
        void ArcTo(Point point, Size size, float rotationAngle, bool isClockwise, bool isLargeArc);
        void CubicTo(Point p1, Point p2, Point p3);
        void QuadTo(Point p1, Point p2);
        void EndFigure(bool closeFigure);
        Rect GetBounds();

        FillRule FillRule { get; set; }

        void Transform(Matrix matrix);

        void AddPath(PathGeometry path, bool connect);

        bool Contains(float x, float y);

        IPathImpl CreateStrokePath(float strokeWidth);
    }
}
