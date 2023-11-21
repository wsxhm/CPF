using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF
{
    /// <summary>
    /// 一般变换矩阵，用来做动画
    /// </summary>
    public class GeneralTransform : Transform
    {
        static Type GeneralTransformType = typeof(GeneralTransform);
        //public static readonly DependencyProperty<float> AngleProperty = new DependencyProperty<float>("Angle", GeneralTransformType, 0f);
        ///// <summary>
        /////     The DependencyProperty for the ScaleTransform.ScaleX property.
        ///// </summary>
        //public static readonly DependencyProperty ScaleXProperty = new DependencyProperty("ScaleX", FloatType, GeneralTransformType, 1f);
        ///// <summary>
        /////     The DependencyProperty for the ScaleTransform.ScaleY property.
        ///// </summary>
        //public static readonly DependencyProperty ScaleYProperty = new DependencyProperty("ScaleY", FloatType, GeneralTransformType, 1f);
        ///// <summary>
        ///// The DependencyProperty for the SkewTransform.AngleX property.
        ///// </summary>
        //public static readonly DependencyProperty SkewXProperty = new DependencyProperty("SkewX", FloatType, GeneralTransformType, 0f);
        ///// <summary>
        ///// The DependencyProperty for the SkewTransform.AngleY property.
        ///// </summary>
        //public static readonly DependencyProperty SkewYProperty = new DependencyProperty("SkewY", FloatType, GeneralTransformType, 0f);

        ///// <summary>
        ///// The DependencyProperty for the TranslateTransform.X property.
        ///// </summary>
        //public static readonly DependencyProperty OffsetXProperty = new DependencyProperty("OffsetX", FloatType, GeneralTransformType, 0f);
        ///// <summary>
        ///// The DependencyProperty for the TranslateTransform.Y property.
        ///// </summary>
        //public static readonly DependencyProperty OffsetYProperty = new DependencyProperty("OffsetY", FloatType, GeneralTransformType, 0f);
        /// <summary>
        ///     X - float.  Default value is 0.0.
        /// </summary>
        [PropertyMetadata(0f)]
        public float OffsetX
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

        /// <summary>
        ///     Y - float.  Default value is 0.0.
        /// </summary>
        [PropertyMetadata(0f)]
        public float OffsetY
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
        /// <summary>
        /// Angle - double.  Default value is 0.0.
        /// </summary>
        [PropertyMetadata(0f)]
        public float Angle
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

        /// <summary>
        ///     ScaleX - float.  Default value is 1.0.
        /// </summary>
        [PropertyMetadata(1f)]
        public float ScaleX
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

        /// <summary>
        ///     ScaleY - float.  Default value is 1.0.
        /// </summary>
        [PropertyMetadata(1f)]
        public float ScaleY
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
        /// <summary>
        /// AngleX - float.  Default value is 0.0.
        /// </summary>
        [PropertyMetadata(0f)]
        public float SkewX
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

        /// <summary>
        ///  AngleY - float.  Default value is 0.0 .
        /// </summary>
        [PropertyMetadata(0f)]
        public float SkewY
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
        /// <summary>
        /// 一般变换矩阵，用来做动画
        /// </summary>
        public GeneralTransform()
        { }
        /// <summary>
        /// 一般变换矩阵，用来做动画
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="skewX"></param>
        /// <param name="skewY"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="angle"></param>
        public GeneralTransform(float offsetX, float offsetY, float skewX, float skewY, float scaleX, float scaleY, float angle)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            SkewX = skewX;
            SkewY = skewY;
            ScaleX = scaleX;
            ScaleY = scaleY;
            Angle = angle;
        }

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            //if ((propertyName == "ScaleX" || propertyName == "ScaleY") && (float)value == 0f)
            //{
            //    throw new OverflowException("缩放参数不能等于0");
            //}
            return base.OnSetValue(propertyName, ref value);
        }

        public override Matrix Value
        {
            get
            {
                Matrix value = Matrix.Identity;
                value.Skew(SkewX, SkewY);
                value.Scale(ScaleX, ScaleY);
                value.Rotate(Angle);
                value.Translate(OffsetX, OffsetY);
                return value;
            }
        }
    }
}
