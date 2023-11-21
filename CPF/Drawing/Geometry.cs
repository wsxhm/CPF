using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    public class Geometry : IDisposable
    {
        public Geometry() : this(new PathGeometry()) { }

        public Geometry(PathGeometry path)
        {
            geometryImpl = CPF.Platform.Application.GetDrawingFactory().CreateGeometry(path);
        }
        IGeometryImpl geometryImpl;

        public IGeometryImpl GeometryImpl
        {
            get
            {
                return geometryImpl;
            }
        }

        /// <summary>
        /// 更新此 Geometry，以仅包含其内部与指定的 Geometry 不相交的部分。
        /// </summary>
        /// <param name="geometry"></param>
        public void Exclude(Geometry geometry)
        {
            geometryImpl.Exclude(geometry);
        }
        /// <summary>
        /// 更新此 Geometry，更新为其自身与指定的 Geometry 的交集。
        /// </summary>
        /// <param name="geometry"></param>
        public void Intersect(Geometry geometry)
        {
            geometryImpl.Intersect(geometry);
        }
        /// <summary>
        /// 测试指定 Point 结构是否包含在此 Geometry 中。
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Point point)
        {
            return geometryImpl.Contains(point);
        }
        ///// <summary>
        ///// 测试指定 FillContains 结构是否包含在此 Geometry 中。
        ///// </summary>
        ///// <param name="geometry"></param>
        ///// <returns></returns>
        //public abstract bool FillContains(Geometry geometry);
        ///// <summary>
        ///// 变换图形
        ///// </summary>
        ///// <param name="matrix"></param>
        //public abstract void Transform(Matrix matrix);
        /// <summary>
        /// 将此 Geometry 更新为其自身与指定 Geometry 的并集。
        /// </summary>
        /// <param name="geometry"></param>
        public void Union(Geometry geometry)
        {
            geometryImpl.Union(geometry);
        }
        /// <summary>
        /// 将此 geometry 更新为其自身与指定 geometry 的并集减去这两者的交集。
        /// </summary>
        /// <param name="geometry"></param>
        public void Xor(Geometry geometry)
        {
            geometryImpl.Xor(geometry);
        }
        /// <summary>
        /// 获取能容纳该 Geometry 的最小矩形
        /// </summary>
        /// <returns></returns>
        public Rect GetRenderBounds()
        {
            return geometryImpl.GetRenderBounds();
        }

        public void Dispose()
        {
            geometryImpl.Dispose();
        }
    }
}
