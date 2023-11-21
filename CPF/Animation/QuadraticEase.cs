using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Animation
{
    /// <summary>
    /// 表示一个缓动函数，创建一个动画加速和/或减速使用下面的公式 f(t) = t2
    /// </summary>
    public class QuadraticEase : IEase
    {
        /// <summary>
        /// 表示一个缓动函数，创建一个动画加速和/或减速使用下面的公式 f(t) = t2
        /// </summary>
        public QuadraticEase() { }
        public float EaseInCore(float normalizedTime)
        {
            return normalizedTime * normalizedTime;
        }
    }
}
