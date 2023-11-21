using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 表示显式支持 Star 单位类型的元素长度。
    /// </summary>
    public struct GridLength : IEquatable<GridLength>
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Constructor, initializes the GridLength as absolute value in pixels.
        /// </summary>
        /// <param name="pixels">Specifies the number of 'device-independent pixels' 
        /// (96 pixels-per-inch).</param>
        /// <exception cref="ArgumentException">
        /// If <c>pixels</c> parameter is <c>float.NaN</c>
        /// or <c>pixels</c> parameter is <c>float.NegativeInfinity</c>
        /// or <c>pixels</c> parameter is <c>float.PositiveInfinity</c>.
        /// </exception>
        public GridLength(float pixels)
            : this(pixels, GridUnitType.Default)
        {
        }

        /// <summary>
        /// Constructor, initializes the GridLength and specifies what kind of value 
        /// it will hold.
        /// </summary>
        /// <param name="value">Value to be stored by this GridLength 
        /// instance.</param>
        /// <param name="type">Type of the value to be stored by this GridLength 
        /// instance.</param>
        /// <remarks> 
        /// If the <c>type</c> parameter is <c>GridUnitType.Auto</c>, 
        /// then passed in value is ignored and replaced with <c>0</c>.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// If <c>value</c> parameter is <c>float.NaN</c>
        /// or <c>value</c> parameter is <c>float.NegativeInfinity</c>
        /// or <c>value</c> parameter is <c>float.PositiveInfinity</c>.
        /// </exception>
        public GridLength(float value, GridUnitType type)
        {
            if (float.IsNaN(value))
            {
                throw new ArgumentException("无效参数value");
            }
            if (float.IsInfinity(value))
            {
                throw new ArgumentException("无效参数value");
            }
            if (type != GridUnitType.Auto
                && type != GridUnitType.Default
                && type != GridUnitType.Star)
            {
                throw new ArgumentException("GridUnitType错误");
            }

            _unitValue = (type == GridUnitType.Auto) ? 0f : value;
            _unitType = type;
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods 

        /// <summary>
        /// Overloaded operator, compares 2 GridLength's.
        /// </summary>
        /// <param name="gl1">first GridLength to compare.</param>
        /// <param name="gl2">second GridLength to compare.</param>
        /// <returns>true if specified GridLengths have same value 
        /// and unit type.</returns>
        public static bool operator ==(GridLength gl1, GridLength gl2)
        {
            return (gl1.GridUnitType == gl2.GridUnitType
                    && gl1.Value == gl2.Value);
        }

        /// <summary>
        /// Overloaded operator, compares 2 GridLength's.
        /// </summary>
        /// <param name="gl1">first GridLength to compare.</param>
        /// <param name="gl2">second GridLength to compare.</param>
        /// <returns>true if specified GridLengths have either different value or 
        /// unit type.</returns>
        public static bool operator !=(GridLength gl1, GridLength gl2)
        {
            return (gl1.GridUnitType != gl2.GridUnitType
                    || gl1.Value != gl2.Value);
        }

        /// <summary>
        /// Compares this instance of GridLength with another object.
        /// </summary>
        /// <param name="oCompare">Reference to an object for comparison.</param>
        /// <returns><c>true</c>if this GridLength instance has the same value 
        /// and unit type as oCompare.</returns>
        override public bool Equals(object oCompare)
        {
            if (oCompare is GridLength)
            {
                GridLength l = (GridLength)oCompare;
                return (this == l);
            }
            else
                return false;
        }

        /// <summary>
        /// Compares this instance of GridLength with another instance.
        /// </summary>
        /// <param name="gridLength">Grid length instance to compare.</param>
        /// <returns><c>true</c>if this GridLength instance has the same value 
        /// and unit type as gridLength.</returns>
        public bool Equals(GridLength gridLength)
        {
            return (this == gridLength);
        }

        /// <summary>
        /// <see cref="Object.GetHashCode"/>
        /// </summary>
        /// <returns><see cref="Object.GetHashCode"/></returns>
        public override int GetHashCode()
        {
            return ((int)_unitValue + (int)_unitType);
        }

        /// <summary>
        /// Returns <c>true</c> if this GridLength instance holds 
        /// an absolute (pixel) value.
        /// </summary>
        public bool IsAbsolute { get { return (_unitType == GridUnitType.Default); } }

        /// <summary>
        /// Returns <c>true</c> if this GridLength instance is 
        /// automatic (not specified).
        /// </summary>
        public bool IsAuto { get { return (_unitType == GridUnitType.Auto); } }

        /// <summary>
        /// Returns <c>true</c> if this GridLength instance holds weighted propertion 
        /// of available space.
        /// </summary>
        public bool IsStar { get { return (_unitType == GridUnitType.Star); } }

        /// <summary>
        /// Returns value part of this GridLength instance.
        /// </summary>
        public float Value { get { return ((_unitType == GridUnitType.Auto) ? 1f : _unitValue); } }

        /// <summary>
        /// Returns unit type of this GridLength instance.
        /// </summary>
        public GridUnitType GridUnitType { get { return (_unitType); } }

        /// <summary>
        /// Returns the string representation of this object.
        /// </summary>
        public override string ToString()
        {
            switch (_unitType)
            {
                //  for Auto print out "Auto". value is always "1.0"
                case (GridUnitType.Auto):
                    return ("Auto");

                //  Star has one special case when value is "1.0".
                //  in this case drop value part and print only "Star"
                case (GridUnitType.Star):
                    return (
                        FloatUtil.IsOne(_unitValue)
                        ? "*"
                        : _unitValue + "*");

                //  for Pixel print out the numeric value. "px" can be omitted.
                default:
                    return _unitValue.ToString();

            }
        }

        #endregion Public Methods 

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// Returns initialized Auto GridLength value.
        /// </summary>
        public static GridLength Auto
        {
            get { return (s_auto); }
        }
        /// <summary>
        /// * 默认Value=1
        /// </summary>
        public static GridLength Star
        {
            get { return s_star; }
        }

        #endregion Public Properties

        public static implicit operator GridLength(float n)
        {
            return new GridLength(n, GridUnitType.Default);
        }

        /// <summary>
        /// 双精度转化为单精度，单位默认
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator GridLength(double n)
        {
            return new GridLength((float)n, GridUnitType.Default);
        }
        /// <summary>
        /// 数字*或者直接数字或者auto
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator GridLength(string n)
        {
            string goodString = n.Trim().ToLowerInvariant();
            if (goodString == "auto")
            {
                return Auto;
            }
            if (goodString == "*")
            {
                return new GridLength(1, GridUnitType.Star);
            }
            try
            {
                var index = goodString.IndexOf('*');
                if (index > 0)
                {
                    return new GridLength(float.Parse(goodString.Substring(0, index)), GridUnitType.Star);
                }
                return new GridLength(float.Parse(goodString));
            }
            catch (Exception)
            {
                throw new Exception("GridLength字符串格式无效");
            }
        }


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields 
        private float _unitValue;      //  unit value storage
        private GridUnitType _unitType; //  unit type storage

        //  static instance of Auto GridLength
        private static readonly GridLength s_auto = new GridLength(1f, GridUnitType.Auto);
        private static readonly GridLength s_star = new GridLength(1f, GridUnitType.Star);
        #endregion Private Fields 
    }

    public enum GridUnitType : byte
    {
        /// <summary>
        /// The value is expressed as a pixel.
        /// </summary>
        Default,
        /// <summary>
        /// 该大小由内容对象的大小属性确定。 
        /// </summary>
        Auto,
        /// <summary>
        /// 该值表示为可用空间的加权比例。
        /// </summary>
        Star,
    }

}
