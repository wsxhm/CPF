using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Animation
{
    /// <summary>
    /// 表示一个缓动函数，该函数创建一个使用公式 f(t) = t3 进行加速和/或减速的动画。
    /// </summary>
    public class CubicEase : IEase
    {
        /// <summary>
        /// 表示一个缓动函数，该函数创建一个使用公式 f(t) = t3 进行加速和/或减速的动画。
        /// </summary>
        public CubicEase() { }
        public float EaseInCore(float normalizedTime)
        {
            return normalizedTime * normalizedTime * normalizedTime;
        }

    }
}
