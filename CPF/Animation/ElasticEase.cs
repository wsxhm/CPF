using System;
using System.Collections.Generic;
using System.Text;
using CPF;
using CPF.Drawing;

namespace CPF.Animation
{
    /// <summary>
    /// 表示创建类似于弹簧 rest 直到显示来回振荡的动画的缓动函数。
    /// </summary>
    public class ElasticEase : CpfObject, IEase
    {
        /// <summary>
        /// 表示创建类似于弹簧 rest 直到显示来回振荡的动画的缓动函数。
        /// </summary>
        public ElasticEase()
        {

        }

        ///// <summary>
        ///// Bounces Property
        ///// </summary>
        //public static readonly DependencyProperty OscillationsProperty =
        //    new DependencyProperty(
        //            "Oscillations",
        //            typeof(int),
        //            typeof(ElasticEase),
        //            new PropertyMetadata(3));

        /// <summary>
        /// Specifies the number of oscillations
        /// </summary>
        [PropertyMetadata(3)]
        public int Oscillations
        {
            get
            {
                return (int)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        ///// <summary>
        ///// Springiness Property
        ///// </summary>
        //public static readonly DependencyProperty SpringinessProperty =
        //    new DependencyProperty(
        //            "Springiness",
        //            typeof(double),
        //            typeof(ElasticEase),
        //            new PropertyMetadata(3.0));

        /// <summary>
        /// Specifies the amount of springiness
        /// </summary>
        [PropertyMetadata(3.0)]
        public double Springiness
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
            double oscillations = Math.Max(0.0, (double)Oscillations);
            double springiness = Math.Max(0.0, Springiness);
            double expo;
            if (DoubleUtil.IsZero(springiness))
            {
                expo = normalizedTime;
            }
            else
            {
                expo = (Math.Exp(springiness * normalizedTime) - 1.0) / (Math.Exp(springiness) - 1.0);
            }

            return (float)(expo * (Math.Sin((Math.PI * 2.0 * oscillations + Math.PI * 0.5) * normalizedTime)));
        }
    }
}
