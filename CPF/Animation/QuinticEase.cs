using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Animation
{
    /// <summary>
    /// 表示一个缓动函数，创建一个动画加速和/或减速使用下面的公式 f(t) = t4。
    /// </summary>
    public class QuinticEase : IEase
    {

        /// <summary>
        /// 表示一个缓动函数，创建一个动画加速和/或减速使用下面的公式 f(t) = t4。
        /// </summary>
        public QuinticEase() { }

        public float EaseInCore(float normalizedTime)
        {
            return normalizedTime * normalizedTime * normalizedTime * normalizedTime * normalizedTime;
        }
    }
}
