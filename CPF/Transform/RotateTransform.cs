using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF
{
    public class RotateTransform : Transform
    {
        ///<summary>
        /// Create a rotation transformation in degrees.
        ///</summary>
        ///<param name="angle">The angle of rotation in degrees.</param>
        public RotateTransform(float angle)
        {
            Angle = angle;
        }
        public RotateTransform()
        {

        }
        
        #region Public Properties

        /// <summary>
        ///     Angle - double.  Default value is 0.0.
        /// </summary>
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

        public float CenterX
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

        public float CenterY
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

        #endregion Public Properties
        #region Dependency Properties

        /// <summary>
        ///     The DependencyProperty for the RotateTransform.Angle property.
        /// </summary>
        //public static readonly DependencyProperty AngleProperty;

        //static RotateTransform()
        //{
        //    // Initializations
        //    Type typeofThis = typeof(RotateTransform);
        //    Type f = typeof(float);
        //    AngleProperty = new DependencyProperty("Angle", f, typeofThis, 0f);
        //}

        #endregion Dependency Properties

        public override Matrix Value
        {
            get
            {
                Matrix m = Matrix.Identity;
                m.RotateAt(Angle, CenterX, CenterY);
                //m.Rotate(Angle);
                return m;
            }
        }
    }
}
