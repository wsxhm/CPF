using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Effects
{
    /// <summary>
    /// 位图特效
    /// </summary>
    public abstract class Effect : CpfObject
    {
        /// <summary>
        /// 调整相对根元素的矩形渲染区域，渲染区域只能变大或者不变，不能变小
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public virtual Rect OverrideRenderRect(Rect rect)
        {
            return rect;
        }
        /// <summary>
        /// 执行位图特效
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="bitmap">控件位图图像，位图尺寸和根元素尺寸一样</param>
        public abstract void DoEffect(DrawingContext dc, Bitmap bitmap);
    }
}
