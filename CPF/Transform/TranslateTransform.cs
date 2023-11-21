using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF
{
    public class TranslateTransform : Transform
    {
        //static Type typeofThis = typeof(TranslateTransform);
        ///// <summary>
        ///// The DependencyProperty for the TranslateTransform.X property.
        ///// </summary>
        //public static readonly DependencyProperty XProperty = new DependencyProperty("X", FloatType, typeofThis, 0f);
        ///// <summary>
        ///// The DependencyProperty for the TranslateTransform.Y property.
        ///// </summary>
        //public static readonly DependencyProperty YProperty = new DependencyProperty("Y", FloatType, typeofThis, 0f);
        /// <summary>
        ///     X - float.  Default value is 0.0.
        /// </summary>
        public float X
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
        public float Y
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
        public override Matrix Value
        {
            get
            {
                Matrix m = Matrix.Identity;
                m.Translate(X, Y);
                return m;
            }
        }
    }
}
