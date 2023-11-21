using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CPF.Shapes
{
    /// <summary>
    /// 多边形
    /// </summary>
    [Description("多边形")]
    public class Polygon : Shape
    {
        Collection<Point> points;
        [NotCpfProperty]
        public Collection<Point> Points
        {
            get
            {
                if (points == null)
                {
                    points = new Collection<Point>();
                    points.CollectionChanged += Points_CollectionChanged;
                }
                return points;
            }
        }

        private void Points_CollectionChanged(object sender, CollectionChangedEventArgs<Point> e)
        {
            InvalidateGeometry();
        }

        protected override CPF.Drawing.PathGeometry CreateDefiningGeometry()
        {
            //Size size = ActualSize;
            //var w = size.Width;
            //var h = size.Height;
            CPF.Drawing.PathGeometry path = new CPF.Drawing.PathGeometry();

            var ps = Points;
            if (ps != null && ps.Count > 1)
            {
                path.BeginFigure(ps[0].X, ps[0].Y);
                for (int i = 1; i < ps.Count; i++)
                {
                    path.LineTo(ps[i].X, ps[i].Y);
                }
                path.EndFigure(true);
            }
            return path;
        }

    }
}
