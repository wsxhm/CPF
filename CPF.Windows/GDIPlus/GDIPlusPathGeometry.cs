//#if Net4
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using CPF.Drawing;

namespace CPF.GDIPlus
{
    public class GDIPlusPathGeometry : IGeometryImpl
    {
        Region region;
        public GDIPlusPathGeometry(PathGeometry path)
        {
            region = new Region((path.PathIml as GDIPlusPath).Path);
        }

        public Region Region
        {
            get { return region; }
        }

        public void Exclude(Geometry geometry)
        {
            Region.Exclude((geometry.GeometryImpl as GDIPlusPathGeometry).Region);
        }

        //public override bool FillContains(Geometry geometry)
        //{
        //    Geometry.IsVisible()
        //}

        public bool Contains(CPF.Drawing.Point point)
        {
            return Region.IsVisible(point.X, point.Y);
        }

        public Rect GetRenderBounds()
        {
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                return Region.GetBounds(g).ToRect();
            }
        }

        public void Intersect(Geometry geometry)
        {
            Region.Intersect((geometry.GeometryImpl as GDIPlusPathGeometry).Region);
        }

        //public override void Transform(Matrix matrix)
        //{
        //    Geometry.Transform(matrix.ToMatrix());
        //}

        public void Union(Geometry geometry)
        {
            Region.Union((geometry.GeometryImpl as GDIPlusPathGeometry).Region);
        }

        public void Xor(Geometry geometry)
        {
            Region.Xor((geometry.GeometryImpl as GDIPlusPathGeometry).Region);
        }

        public void Dispose()
        {
            region.Dispose();
        }

    }
}
//#endif