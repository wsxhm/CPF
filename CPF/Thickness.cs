using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPF.Drawing;
using System.ComponentModel;

namespace CPF
{
    /// <summary>
    /// 表示四周的厚度，字符串格式 all、left,top,right,bottom
    /// </summary>
    [Description("表示四周的厚度，不能用百分比，格式：all或者left,top,right,bottom")]
    public struct Thickness : IEquatable<Thickness>
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------

        #region Constructors
        /// <summary>
        /// 表示四周的厚度 This constructur builds a Thickness with a specified value on every side.
        /// </summary>
        /// <param name="uniformLength">The specified uniform length.</param>
        public Thickness(in float uniformLength)
        {
            _Left = _Top = _Right = _Bottom = uniformLength;
        }

        /// <summary>
        /// This constructor builds a Thickness with the specified number of pixels on each side.
        /// 表示四周的厚度
        /// </summary>
        /// <param name="left">The thickness for the left side.</param>
        /// <param name="top">The thickness for the top side.</param>
        /// <param name="right">The thickness for the right side.</param>
        /// <param name="bottom">The thickness for the bottom side.</param>
        public Thickness(in float left, in float top, in float right, in float bottom)
        {
            _Left = left;
            _Top = top;
            _Right = right;
            _Bottom = bottom;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Thickness"/> structure.
        /// </summary>
        /// <param name="horizontal">The thickness on the left and right.</param>
        /// <param name="vertical">The thickness on the top and bottom.</param>
        public Thickness(in float horizontal, in float vertical)
        {
            _Left = _Right = horizontal;
            _Top = _Bottom = vertical;
        }
        #endregion
        /// <summary>
        /// Top + Bottom
        /// </summary>
        public float Vertical
        {
            get { return _Top + _Bottom; }
        }
        /// <summary>
        /// Right + Left
        /// </summary>
        public float Horizontal
        {
            get { return _Right + _Left; }
        }
        //-------------------------------------------------------------------
        //
        //  Public Methods
        //
        //-------------------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// This function compares to the provided object for type and value equality.
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>True if object is a Thickness and all sides of it are equal to this Thickness'.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Thickness)
            {
                Thickness otherObj = (Thickness)obj;
                return (this == otherObj);
            }
            return (false);
        }

        /// <summary>
        /// Compares this instance of Thickness with another instance.
        /// </summary>
        /// <param name="thickness">Thickness instance to compare.</param>
        /// <returns><c>true</c>if this Thickness instance has the same value 
        /// and unit type as thickness.</returns>
        public bool Equals(Thickness thickness)
        {
            return (this == thickness);
        }

        /// <summary>
        /// This function returns a hash code.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return _Left.GetHashCode() ^ _Top.GetHashCode() ^ _Right.GetHashCode() ^ _Bottom.GetHashCode();
        }

        /// <summary>
        /// {Left},{Top},{Right},{Bottom}
        /// </summary>
        /// <returns>String conversion.</returns>
        public override string ToString()
        {
            return $"{Left},{Top},{Right},{Bottom}";
        }
        /// <summary>
        /// Parses a <see cref="Thickness"/> string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The <see cref="Thickness"/>.</returns>
        public static Thickness Parse(string s)
        {
            var parts = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            switch (parts.Count)
            {
                case 1:
                    var uniform = float.Parse(parts[0]);
                    return new Thickness(uniform);
                case 2:
                    var horizontal = float.Parse(parts[0]);
                    var vertical = float.Parse(parts[1]);
                    return new Thickness(horizontal, vertical);
                case 4:
                    var left = float.Parse(parts[0]);
                    var top = float.Parse(parts[1]);
                    var right = float.Parse(parts[2]);
                    var bottom = float.Parse(parts[3]);
                    return new Thickness(left, top, right, bottom);
            }

            throw new FormatException("Invalid Thickness.");
        }
        ///// <summary>
        ///// Converts this Thickness object to a string.
        ///// </summary>
        ///// <returns>String conversion.</returns>
        //internal string ToString(CultureInfo cultureInfo)
        //{
        //    return ThicknessConverter.ToString(this, cultureInfo);
        //}

        internal bool IsZero
        {
            get
            {
                return FloatUtil.IsZero(Left)
                        && FloatUtil.IsZero(Top)
                        && FloatUtil.IsZero(Right)
                        && FloatUtil.IsZero(Bottom);
            }
        }

        internal bool IsUniform
        {
            get
            {
                return FloatUtil.AreClose(Left, Top)
                        && FloatUtil.AreClose(Left, Right)
                        && FloatUtil.AreClose(Left, Bottom);
            }
        }

        /// <summary>
        /// Verifies if this Thickness contains only valid values
        /// The set of validity checks is passed as parameters.
        /// </summary>
        /// <param name='allowNegative'>allows negative values</param>
        /// <param name='allowNaN'>allows Double.NaN</param>
        /// <param name='allowPositiveInfinity'>allows Double.PositiveInfinity</param>
        /// <param name='allowNegativeInfinity'>allows Double.NegativeInfinity</param>
        /// <returns>Whether or not the thickness complies to the range specified</returns>
        internal bool IsValid(bool allowNegative, bool allowNaN, bool allowPositiveInfinity, bool allowNegativeInfinity)
        {
            if (!allowNegative)
            {
                if (Left < 0d || Right < 0d || Top < 0d || Bottom < 0d)
                    return false;
            }

            if (!allowNaN)
            {
                if (FloatUtil.IsNaN(Left) || FloatUtil.IsNaN(Right) || FloatUtil.IsNaN(Top) || FloatUtil.IsNaN(Bottom))
                    return false;
            }

            if (!allowPositiveInfinity)
            {
                if (Double.IsPositiveInfinity(Left) || Double.IsPositiveInfinity(Right) || Double.IsPositiveInfinity(Top) || Double.IsPositiveInfinity(Bottom))
                {
                    return false;
                }
            }

            if (!allowNegativeInfinity)
            {
                if (Double.IsNegativeInfinity(Left) || Double.IsNegativeInfinity(Right) || Double.IsNegativeInfinity(Top) || Double.IsNegativeInfinity(Bottom))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two thicknesses for fuzzy equality.  This function
        /// helps compensate for the fact that float values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='thickness'>The thickness to compare to this</param>
        /// <returns>Whether or not the two points are equal</returns>
        internal bool IsClose(Thickness thickness)
        {
            return (FloatUtil.AreClose(Left, thickness.Left)
                    && FloatUtil.AreClose(Top, thickness.Top)
                    && FloatUtil.AreClose(Right, thickness.Right)
                    && FloatUtil.AreClose(Bottom, thickness.Bottom));
        }

        /// <summary>
        /// Compares two thicknesses for fuzzy equality.  This function
        /// helps compensate for the fact that float values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='thickness0'>The first thickness to compare</param>
        /// <param name='thickness1'>The second thickness to compare</param>
        /// <returns>Whether or not the two thicknesses are equal</returns>
        static internal bool AreClose(Thickness thickness0, Thickness thickness1)
        {
            return thickness0.IsClose(thickness1);
        }

        #endregion


        //-------------------------------------------------------------------
        //
        //  Public Operators
        //
        //-------------------------------------------------------------------

        #region Public Operators

        /// <summary>
        /// Overloaded operator to compare two Thicknesses for equality.
        /// </summary>
        /// <param name="t1">first Thickness to compare</param>
        /// <param name="t2">second Thickness to compare</param>
        /// <returns>True if all sides of the Thickness are equal, false otherwise</returns>
        //  SEEALSO
        public static bool operator ==(Thickness t1, Thickness t2)
        {
            return ((t1._Left == t2._Left || (FloatUtil.IsNaN(t1._Left) && FloatUtil.IsNaN(t2._Left)))
                    && (t1._Top == t2._Top || (FloatUtil.IsNaN(t1._Top) && FloatUtil.IsNaN(t2._Top)))
                    && (t1._Right == t2._Right || (FloatUtil.IsNaN(t1._Right) && FloatUtil.IsNaN(t2._Right)))
                    && (t1._Bottom == t2._Bottom || (FloatUtil.IsNaN(t1._Bottom) && FloatUtil.IsNaN(t2._Bottom)))
                    );
        }

        /// <summary>
        /// Overloaded operator to compare two Thicknesses for inequality.
        /// </summary>
        /// <param name="t1">first Thickness to compare</param>
        /// <param name="t2">second Thickness to compare</param>
        /// <returns>False if all sides of the Thickness are equal, true otherwise</returns>
        //  SEEALSO
        public static bool operator !=(Thickness t1, Thickness t2)
        {
            return (!(t1 == t2));
        }

        public static implicit operator Thickness(string n)
        {
            if (string.IsNullOrWhiteSpace(n))
            {
                throw new Exception("Thickness字符转换格式不对");
            }
            if (n.IndexOf(',') >= 0)
            {
                var temp = n.Split(',');
                if (temp.Length < 4)
                {
                    throw new Exception("Thickness字符转换格式不对");
                }
                try
                {
                    return new Thickness(float.Parse(temp[0].Trim()), float.Parse(temp[1].Trim()), float.Parse(temp[2].Trim()), float.Parse(temp[3].Trim()));
                }
                catch (Exception)
                {
                    throw new Exception("Thickness字符转换格式不对");
                }
            }
            else
            {
                try
                {
                    var nn = float.Parse(n);
                    return new Thickness(nn);
                }
                catch (Exception)
                {
                    throw new Exception("Thickness字符转换格式不对");
                }
            }
        }

        #endregion


        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //-------------------------------------------------------------------

        #region Public Properties

        /// <summary>This property is the Length on the thickness' left side</summary>
        public float Left
        {
            get { return _Left; }
            set { _Left = value; }
        }

        /// <summary>This property is the Length on the thickness' top side</summary>
        public float Top
        {
            get { return _Top; }
            set { _Top = value; }
        }

        /// <summary>This property is the Length on the thickness' right side</summary>
        public float Right
        {
            get { return _Right; }
            set { _Right = value; }
        }

        /// <summary>This property is the Length on the thickness' bottom side</summary>
        public float Bottom
        {
            get { return _Bottom; }
            set { _Bottom = value; }
        }
        #endregion

        //-------------------------------------------------------------------
        //
        //  INternal API
        //
        //-------------------------------------------------------------------

        #region Internal API

        internal Size Size
        {
            get
            {
                return new Size(_Left + _Right, _Top + _Bottom);
            }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Fields
        //
        //-------------------------------------------------------------------

        #region Private Fields

        private float _Left;
        private float _Top;
        private float _Right;
        private float _Bottom;

        #endregion
    }
}
