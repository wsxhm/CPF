using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF
{
    public class SkewTransform : Transform
    {
        public SkewTransform(float angleX, float angleY)
        {
            AngleX = angleX;
            AngleY = angleY;
        }
        public SkewTransform()
        {
        }
        /// <summary>
        ///     AngleX - float.  Default value is 0.0.
        /// </summary>
        public float AngleX
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
        ///     AngleY - float.  Default value is 0.0 .
        /// </summary>
        public float AngleY
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

        //static Type typeofThis = typeof(SkewTransform);

        ///// <summary>
        ///// The DependencyProperty for the SkewTransform.AngleX property.
        ///// </summary>
        //public static readonly DependencyProperty AngleXProperty = new DependencyProperty("AngleX", FloatType, typeofThis, 0f);
        ///// <summary>
        ///// The DependencyProperty for the SkewTransform.AngleY property.
        ///// </summary>
        //public static readonly DependencyProperty AngleYProperty = new DependencyProperty("AngleY", FloatType, typeofThis, 0f);

        public override Matrix Value
        {
            get
            {
                Matrix m = Matrix.Identity;
                m.Skew(AngleX, AngleY);
                return m;
            }
        }
    }
}
