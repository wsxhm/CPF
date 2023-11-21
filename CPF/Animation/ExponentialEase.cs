using System;
using System.Collections.Generic;
using System.Text;
using CPF;
using CPF.Drawing;

namespace CPF.Animation
{
    /// <summary>
    /// 表示创建的动画加速和/或使用指数公式减速的缓动函数。
    /// </summary>
    public class ExponentialEase : CpfObject, IEase
    {
        /// <summary>
        /// 表示创建的动画加速和/或使用指数公式减速的缓动函数。
        /// </summary>
        public ExponentialEase()
        {

        }

        ///// <summary>
        ///// Factor Property
        ///// </summary>
        //public static readonly DependencyProperty ExponentProperty =
        //    new DependencyProperty(
        //            "Exponent",
        //            typeof(double),
        //            typeof(ExponentialEase),
        //            new PropertyMetadata(2.0));

        /// <summary>
        /// Specifies the factor which controls the shape of easing.默认值2
        /// </summary>
        [PropertyMetadata(2f)]
        public float Exponent
        {
            get
            {
                return (float)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        public float EaseInCore(float normalizedTime)
        {
            float factor = Exponent;
            if (FloatUtil.IsZero(factor))
            {
                return normalizedTime;
            }
            else
            {
                return (float)((Math.Exp(factor * normalizedTime) - 1.0) / (Math.Exp(factor) - 1.0));
            }
        }
    }
}
