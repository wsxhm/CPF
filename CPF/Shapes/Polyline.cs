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
    /// 折线
    /// </summary>
    [Description("折线")]
    public class Polyline : Shape
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
//#if NET40
                    points.CollectionChanged += Points_CollectionChanged;
//#else
//                    var method = typeof(Polyline).GetMethod(nameof(Points_CollectionChanged), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
//                    Console.WriteLine("--：" + method.Name);
//                    var del = method.CreateDelegate(typeof(EventHandler<CollectionChangedEventArgs<Point>>), this);
//                    Console.WriteLine("==：" + ":" + del.Method.Name);
//                    points.Events.AddHandler(nameof(points.CollectionChanged), del);
//#endif
                }
                return points;
            }
        }

        private void Points_CollectionChanged(object sender, CollectionChangedEventArgs<Point> e)
        {
            InvalidateGeometry();
        }

        /// <summary>
        /// 折线
        /// </summary>
        public Polyline()
        { }

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
                path.EndFigure(false);
            }
            return path;
        }
    }
}
