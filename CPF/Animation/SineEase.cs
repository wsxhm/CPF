using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Animation
{
    /// <summary>
    /// 表示一个缓动函数，创建一个动画加速和/或减速使用正弦值的公式
    /// </summary>
    public class SineEase : IEase
    {
        /// <summary>
        /// 表示一个缓动函数，创建一个动画加速和/或减速使用正弦值的公式
        /// </summary>
        public SineEase() { }
        public float EaseInCore(float normalizedTime)
        {
            return (float)(1.0 - Math.Sin(Math.PI * 0.5 * (1 - normalizedTime)));
        }
    }
}
