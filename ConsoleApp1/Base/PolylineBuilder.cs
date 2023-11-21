using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace xss_pro.Base
{
    public class PolylineBuilder
    {
        private Point _firstPoint;
        private Point _lastPoint;
        private List<Point> _points;
        public PolylineBuilder(Point startPoint)
        {
            _points = new List<Point>();
            _points.Add(startPoint);
            _lastPoint = startPoint;
            _firstPoint = startPoint;
        }
        public IEnumerable<Point> Points => _points;
        public PolylineBuilder GoUp(float distance)
        {
            _lastPoint.Y -= distance;
            _points.Add(_lastPoint);
            return this;
        }
        public PolylineBuilder GoDown(float distance)
        {
            _lastPoint.Y += distance;
            _points.Add(_lastPoint);
            return this;
        }
        public PolylineBuilder GoLeft(float distance)
        {
            _lastPoint.X -= distance;
            _points.Add(_lastPoint);
            return this;
        }
        public PolylineBuilder GoRight(float distance)
        {
            _lastPoint.X += distance;
            _points.Add(_lastPoint);
            return this;
        }
        public PolylineBuilder GoPoint(Point point)
        {
            _lastPoint = point;
            _points.Add(point);
            return this;
        }
        public PolylineBuilder Close()
        {
            _lastPoint = _firstPoint;
            _points.Add(_firstPoint);
            return this;
        }
    }
}
