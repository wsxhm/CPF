using System;
using System.Collections.Generic;
using System.Text;
using CPF;

namespace CPF.Animation
{
    /// <summary>
    /// 表示指定的路径中进行动画处理在开始之前将略有收回动画的运动的缓动函数。
    /// </summary>
    public class BackEase : CpfObject, IEase
    {
        /// <summary>
        /// 表示指定的路径中进行动画处理在开始之前将略有收回动画的运动的缓动函数。
        /// </summary>
        public BackEase()
        {

        }

        ///// <summary>
        ///// Amplitude Property
        ///// </summary>
        //public static readonly DependencyProperty AmplitudeProperty =
        //    new DependencyProperty(
        //            "Amplitude",
        //            typeof(double),
        //            typeof(BackEase),
        //            new PropertyMetadata(1.0));

        /// <summary>
        /// Specifies how much the function will pull back
        /// </summary>
        [PropertyMetadata(1d)]
        public double Amplitude
        {
            get
            {
                return (double)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        public float EaseInCore(float normalizedTime)
        {
            float amp = (float)Math.Max(0.0, Amplitude);
            return (float)Math.Pow(normalizedTime, 3.0) - normalizedTime * amp * (float)Math.Sin(Math.PI * normalizedTime);
        }
    }
}
