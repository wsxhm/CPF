using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Drawing
{
    public interface IGeometryImpl : IDisposable
    {
        /// <summary>
        /// 更新此 Geometry，以仅包含其内部与指定的 Geometry 不相交的部分。
        /// </summary>
        /// <param name="geometry"></param>
        void Exclude(Geometry geometry);
        /// <summary>
        /// 更新此 Geometry，更新为其自身与指定的 Geometry 的交集。
        /// </summary>
        /// <param name="geometry"></param>
        void Intersect(Geometry geometry);
        /// <summary>
        /// 测试指定 Point 结构是否包含在此 Geometry 中。
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        bool Contains(Point point);
        ///// <summary>
        ///// 测试指定 FillContains 结构是否包含在此 Geometry 中。
        ///// </summary>
        ///// <param name="geometry"></param>
        ///// <returns></returns>
        //public abstract bool Contains(Geometry geometry);
        ///// <summary>
        ///// 变换图形
        ///// </summary>
        ///// <param name="matrix"></param>
        //public abstract void Transform(Matrix matrix);
        /// <summary>
        /// 将此 Geometry 更新为其自身与指定 Geometry 的并集。
        /// </summary>
        /// <param name="geometry"></param>
        void Union(Geometry geometry);
        /// <summary>
        /// 将此 geometry 更新为其自身与指定 geometry 的并集减去这两者的交集。
        /// </summary>
        /// <param name="geometry"></param>
        void Xor(Geometry geometry);
        /// <summary>
        /// 获取能容纳该 Geometry 的最小矩形
        /// </summary>
        /// <returns></returns>
        Rect GetRenderBounds();
    }
}
