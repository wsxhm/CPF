using System;
using System.Collections.Generic;
using System.Text;
using CPF;
using CPF.Drawing;

namespace CPF.Animation
{
    /// <summary>
    /// 表示一个缓动函数，创建一个动画加速和/或减速使用下面的公式 f(t) = tp p 是等于 Power 属性。
    /// </summary>
    public class PowerEase : CpfObject, IEase
    {
        /// <summary>
        /// 表示一个缓动函数，创建一个动画加速和/或减速使用下面的公式 f(t) = tp p 是等于 Power 属性。
        /// </summary>
        public PowerEase()
        {

        }

        ///// <summary>
        ///// Power Property
        ///// </summary>
        //public static readonly DependencyProperty PowerProperty =
        //    new DependencyProperty(
        //            "Power",
        //            typeof(double),
        //            typeof(PowerEase),
        //            new PropertyMetadata(2.0));

        /// <summary>
        /// Specifies the power for the polynomial equation.默认值2
        /// </summary>
        [PropertyMetadata(2.0)]
        public double Power
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
            double power = Math.Max(0.0, Power);
            return (float)Math.Pow(normalizedTime, power);
        }
    }
}
