using System;
using System.Collections.Generic;
using System.Text;
using CPF;

namespace CPF.Animation
{
    /// <summary>
    /// 表示创建的动画加速和/或使用循环函数减速的缓动函数。
    /// </summary>
    public class CircleEase : IEase
    {
        /// <summary>
        /// 表示创建的动画加速和/或使用循环函数减速的缓动函数。
        /// </summary>
        public CircleEase() { }
        public float EaseInCore(float normalizedTime)
        {
            normalizedTime = (float)Math.Max(0.0, Math.Min(1.0, normalizedTime));
            return 1.0f - (float)Math.Sqrt(1.0 - normalizedTime * normalizedTime);
        }
    }
}
