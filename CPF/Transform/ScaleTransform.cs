using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF
{
    public class ScaleTransform : Transform
    {
        ///<summary>
        /// Create a scale transformation.
        ///</summary>
        public ScaleTransform()
        {
        }

        ///<summary>
        /// Create a scale transformation.
        ///</summary>
        public ScaleTransform(
            float scaleX,
            float scaleY
            )
        {
            ScaleX = scaleX;
            ScaleY = scaleY;
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

        static Type typeofThis = typeof(ScaleTransform);

        ///// <summary>
        /////     The DependencyProperty for the ScaleTransform.ScaleX property.
        ///// </summary>
        //public static readonly DependencyProperty ScaleXProperty = new DependencyProperty("ScaleX", FloatType, typeofThis, 1f);
        ///// <summary>
        /////     The DependencyProperty for the ScaleTransform.ScaleY property.
        ///// </summary>
        //public static readonly DependencyProperty ScaleYProperty = new DependencyProperty("ScaleY", FloatType, typeofThis, 1f);

        public override Matrix Value
        {
            get
            {
                Matrix m = Matrix.Identity;
                m.Scale(ScaleX, ScaleY);
                return m;
            }
        }
    }
}
