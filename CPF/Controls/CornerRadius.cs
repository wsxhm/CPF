using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 表示矩形的角的半径，格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft
    /// </summary>
    public struct CornerRadius : IEquatable<CornerRadius>
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------
        #region Constructors
        /// <summary>
        /// 所有圆角度数都一样
        /// </summary>
        /// <param name="uniformRadius"></param>
        public CornerRadius(float uniformRadius)
        {
            _topLeft = _topRight = _bottomLeft = _bottomRight = uniformRadius;
        }

        /// <summary>
        /// 设置每个角
        /// </summary>
        /// <param name="topLeft">The thickness for the top left corner.</param>
        /// <param name="topRight">The thickness for the top right corner.</param>
        /// <param name="bottomRight">The thickness for the bottom right corner.</param>
        /// <param name="bottomLeft">The thickness for the bottom left corner.</param>
        public CornerRadius(float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            _topLeft = topLeft;
            _topRight = topRight;
            _bottomRight = bottomRight;
            _bottomLeft = bottomLeft;
        }

        #endregion Constructors

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
        /// <returns>True if object is a CornerRadius and all sides of it are equal to this CornerRadius'.</returns>
        public override bool Equals(object obj)
        {
            if (obj is CornerRadius)
            {
                CornerRadius otherObj = (CornerRadius)obj;
                return (this == otherObj);
            }
            return (false);
        }

        /// <summary>
        /// Compares this instance of CornerRadius with another instance.
        /// </summary>
        /// <param name="cornerRadius">CornerRadius instance to compare.</param>
        /// <returns><c>true</c>if this CornerRadius instance has the same value 
        /// and unit type as cornerRadius.</returns>
        public bool Equals(CornerRadius cornerRadius)
        {
            return (this == cornerRadius);
        }

        /// <summary>
        /// This function returns a hash code.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return _topLeft.GetHashCode() ^ _topRight.GetHashCode() ^ _bottomLeft.GetHashCode() ^ _bottomRight.GetHashCode();
        }

        /// <summary>
        /// Converts this Thickness object to a string.
        /// </summary>
        /// <returns>String conversion.</returns>
        public override string ToString()
        {
            char listSeparator = ',';

            // Initial capacity [64] is an estimate based on a sum of:
            // 48 = 4x double (twelve digits is generous for the range of values likely)
            //  8 = 4x UnitType string (approx two characters)
            //  4 = 4x separator characters
            StringBuilder sb = new StringBuilder(64);

            sb.Append(TopLeft.ToString());
            sb.Append(listSeparator);
            sb.Append(TopRight.ToString());
            sb.Append(listSeparator);
            sb.Append(BottomRight.ToString());
            sb.Append(listSeparator);
            sb.Append(BottomLeft.ToString());
            return sb.ToString();
        }

        #endregion Public Methods

        //-------------------------------------------------------------------
        //
        //  Public Operators
        //
        //-------------------------------------------------------------------
        #region Public Operators

        /// <summary>
        /// Overloaded operator to compare two CornerRadiuses for equality.
        /// </summary>
        /// <param name="cr1">First CornerRadius to compare</param>
        /// <param name="cr2">Second CornerRadius to compare</param>
        /// <returns>True if all sides of the CornerRadius are equal, false otherwise</returns>
        //  SEEALSO
        public static bool operator ==(CornerRadius cr1, CornerRadius cr2)
        {
            return ((cr1._topLeft == cr2._topLeft || (float.IsNaN(cr1._topLeft) && float.IsNaN(cr2._topLeft)))
                    && (cr1._topRight == cr2._topRight || (float.IsNaN(cr1._topRight) && float.IsNaN(cr2._topRight)))
                    && (cr1._bottomRight == cr2._bottomRight || (float.IsNaN(cr1._bottomRight) && float.IsNaN(cr2._bottomRight)))
                    && (cr1._bottomLeft == cr2._bottomLeft || (float.IsNaN(cr1._bottomLeft) && float.IsNaN(cr2._bottomLeft)))
                    );
        }

        /// <summary>
        /// Overloaded operator to compare two CornerRadiuses for inequality.
        /// </summary>
        /// <param name="cr1">First CornerRadius to compare</param>
        /// <param name="cr2">Second CornerRadius to compare</param>
        /// <returns>False if all sides of the CornerRadius are equal, true otherwise</returns>
        //  SEEALSO
        public static bool operator !=(CornerRadius cr1, CornerRadius cr2)
        {
            return (!(cr1 == cr2));
        }

        public static implicit operator CornerRadius(string n)
        {
            if (string.IsNullOrWhiteSpace(n))
            {
                throw new Exception("CornerRadius字符转换格式不对");
            }
            if (n.IndexOf(',') >= 0)
            {
                var temp = n.Split(',');
                if (temp.Length < 4)
                {
                    throw new Exception("CornerRadius字符转换格式不对");
                }
                try
                {
                    return new CornerRadius(float.Parse(temp[0].Trim()), float.Parse(temp[1].Trim()), float.Parse(temp[2].Trim()), float.Parse(temp[3].Trim()));
                }
                catch (Exception)
                {
                    throw new Exception("CornerRadius字符转换格式不对");
                }
            }
            else
            {
                try
                {
                    var nn = float.Parse(n);
                    return new CornerRadius(nn);
                }
                catch (Exception)
                {
                    throw new Exception("CornerRadius字符转换格式不对");
                }
            }
        }

        #endregion Public Operators


        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //-------------------------------------------------------------------

        #region Public Properties

        /// <summary>This property is the Length on the thickness' top left corner</summary>
        public float TopLeft
        {
            get { return _topLeft; }
            set { _topLeft = value; }
        }

        /// <summary>This property is the Length on the thickness' top right corner</summary>
        public float TopRight
        {
            get { return _topRight; }
            set { _topRight = value; }
        }

        /// <summary>This property is the Length on the thickness' bottom right corner</summary>
        public float BottomRight
        {
            get { return _bottomRight; }
            set { _bottomRight = value; }
        }

        /// <summary>This property is the Length on the thickness' bottom left corner</summary>
        public float BottomLeft
        {
            get { return _bottomLeft; }
            set { _bottomLeft = value; }
        }

        #endregion Public Properties

        //-------------------------------------------------------------------
        //
        //  Internal Methods Properties
        //
        //-------------------------------------------------------------------

        #region Internal Methods Properties

        internal bool IsValid(bool allowNegative, bool allowNaN, bool allowPositiveInfinity, bool allowNegativeInfinity)
        {
            if (!allowNegative)
            {
                if (_topLeft < 0d || _topRight < 0d || _bottomLeft < 0d || _bottomRight < 0d)
                {
                    return (false);
                }
            }

            if (!allowNaN)
            {
                if (float.IsNaN(_topLeft) || float.IsNaN(_topRight) || float.IsNaN(_bottomLeft) || float.IsNaN(_bottomRight))
                {
                    return (false);
                }
            }

            if (!allowPositiveInfinity)
            {
                if (Double.IsPositiveInfinity(_topLeft) || Double.IsPositiveInfinity(_topRight) || Double.IsPositiveInfinity(_bottomLeft) || Double.IsPositiveInfinity(_bottomRight))
                {
                    return (false);
                }
            }

            if (!allowNegativeInfinity)
            {
                if (Double.IsNegativeInfinity(_topLeft) || Double.IsNegativeInfinity(_topRight) || Double.IsNegativeInfinity(_bottomLeft) || Double.IsNegativeInfinity(_bottomRight))
                {
                    return (false);
                }
            }

            return (true);
        }

        internal bool IsZero
        {
            get
            {
                return (FloatUtil.IsZero(_topLeft)
                        && FloatUtil.IsZero(_topRight)
                        && FloatUtil.IsZero(_bottomRight)
                        && FloatUtil.IsZero(_bottomLeft)
                        );
            }
        }

        #endregion Internal Methods Properties

        //-------------------------------------------------------------------
        //
        //  Private Fields
        //
        //-------------------------------------------------------------------

        #region Private Fields
        private float _topLeft;
        private float _topRight;
        private float _bottomLeft;
        private float _bottomRight;
        #endregion
    }
}
