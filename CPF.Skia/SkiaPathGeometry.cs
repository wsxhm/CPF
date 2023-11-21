using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkiaSharp;
using CPF.Drawing;

namespace CPF.Skia
{
    public class SkiaPathGeometry : IGeometryImpl
    {
        SKRegion region;
        public SKRegion SKRegion
        {
            get { return region; }
        }

        public SkiaPathGeometry(PathGeometry path)
        {
            region = new SKRegion();
            region.SetPath((path.PathIml as SkiaPath).SKPath);
        }

        public void Dispose()
        {
            region.Dispose();
        }

        public void Exclude(Geometry geometry)
        {
            region.Op((geometry.GeometryImpl as SkiaPathGeometry).SKRegion, SKRegionOperation.Difference);
        }

        public bool Contains(Point point)
        {
            return region.Contains((int)point.X, (int)point.Y);
        }

        public Rect GetRenderBounds()
        {
            var rect = region.Bounds;
            return new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void Intersect(Geometry geometry)
        {
            region.Op((geometry.GeometryImpl as SkiaPathGeometry).SKRegion, SKRegionOperation.Intersect);
        }

        public void Union(Geometry geometry)
        {
            region.Op((geometry.GeometryImpl as SkiaPathGeometry).SKRegion, SKRegionOperation.Union);
        }

        public void Xor(Geometry geometry)
        {
            region.Op((geometry.GeometryImpl as SkiaPathGeometry).SKRegion, SKRegionOperation.XOR);
        }
    }
}
