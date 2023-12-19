using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    public struct DataGridLength : IEquatable<DataGridLength>
    {
        #region Constructors

        /// <summary>
        ///     Initializes as an absolute value in pixels.
        /// </summary>
        /// <param name="pixels">
        ///     Specifies the number of 'device-independent pixels' (96 pixels-per-inch).
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <c>pixels</c> parameter is <c>float.NaN</c>
        ///     or <c>pixels</c> parameter is <c>float.NegativeInfinity</c>
        ///     or <c>pixels</c> parameter is <c>float.PositiveInfinity</c>.
        /// </exception>
        public DataGridLength(float pixels)
            : this(pixels, DataGridLengthUnitType.Default)
        {
        }

        /// <summary>
        ///     Initializes to a specified value and unit.
        /// </summary>
        /// <param name="value">The value to hold.</param>
        /// <param name="type">The unit of <c>value</c>.</param>
        /// <remarks> 
        ///     <c>value</c> is ignored unless <c>type</c> is
        ///     <c>DataGridLengthUnitType.Pixel</c> or
        ///     <c>DataGridLengthUnitType.Star</c>
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     If <c>value</c> parameter is <c>float.NaN</c>
        ///     or <c>value</c> parameter is <c>float.NegativeInfinity</c>
        ///     or <c>value</c> parameter is <c>float.PositiveInfinity</c>.
        /// </exception>
        public DataGridLength(float value, DataGridLengthUnitType type)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException("Value无效");
            }

            if (type != DataGridLengthUnitType.Auto &&
                type != DataGridLengthUnitType.Default &&
                type != DataGridLengthUnitType.Star &&
                type != DataGridLengthUnitType.SizeToCells &&
                type != DataGridLengthUnitType.SizeToHeader)
            {
                throw new ArgumentException("DataGridLengthUnitType无效");
            }

            _unitValue = (type == DataGridLengthUnitType.Auto) ? AutoValue : value;
            _unitType = type;
        }

        #endregion Constructors

        #region Public Methods 

        /// <summary>
        /// Overloaded operator, compares 2 DataGridLength's.
        /// </summary>
        /// <param name="gl1">first DataGridLength to compare.</param>
        /// <param name="gl2">second DataGridLength to compare.</param>
        /// <returns>true if specified DataGridLengths have same value 
        /// and unit type.</returns>
        public static bool operator ==(DataGridLength gl1, DataGridLength gl2)
        {
            return gl1.UnitType == gl2.UnitType
                   && gl1.Value == gl2.Value;
        }

        /// <summary>
        /// Overloaded operator, compares 2 DataGridLength's.
        /// </summary>
        /// <param name="gl1">first DataGridLength to compare.</param>
        /// <param name="gl2">second DataGridLength to compare.</param>
        /// <returns>true if specified DataGridLengths have either different value or 
        /// unit type.</returns>
        public static bool operator !=(DataGridLength gl1, DataGridLength gl2)
        {
            return gl1.UnitType != gl2.UnitType
                   || gl1.Value != gl2.Value;
        }

        /// <summary>
        /// Compares this instance of DataGridLength with another object.
        /// </summary>
        /// <param name="obj">Reference to an object for comparison.</param>
        /// <returns><c>true</c>if this DataGridLength instance has the same value 
        /// and unit type as oCompare.</returns>
        public override bool Equals(object obj)
        {
            if (obj is DataGridLength)
            {
                DataGridLength l = (DataGridLength)obj;
                return this == l;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Compares this instance of DataGridLength with another instance.
        /// </summary>
        /// <param name="other">Grid length instance to compare.</param>
        /// <returns><c>true</c>if this DataGridLength instance has the same value 
        /// and unit type as gridLength.</returns>
        public bool Equals(DataGridLength other)
        {
            return this == other;
        }

        /// <summary>
        /// <see cref="Object.GetHashCode"/>
        /// </summary>
        /// <returns><see cref="Object.GetHashCode"/></returns>
        public override int GetHashCode()
        {
            return (int)_unitValue + (int)_unitType;
        }

        /// <summary>
        ///     Returns <c>true</c> if this DataGridLength instance holds 
        ///     an absolute (pixel) value.
        /// </summary>
        public bool IsAbsolute
        {
            get
            {
                return _unitType == DataGridLengthUnitType.Default;
            }
        }

        /// <summary>
        ///     Returns <c>true</c> if this DataGridLength instance is 
        ///     automatic (not specified).
        /// </summary>
        public bool IsAuto
        {
            get
            {
                return _unitType == DataGridLengthUnitType.Auto;
            }
        }

        /// <summary>
        ///     Returns <c>true</c> if this DataGridLength instance holds a weighted proportion
        ///     of available space.
        /// </summary>
        public bool IsStar
        {
            get
            {
                return _unitType == DataGridLengthUnitType.Star;
            }
        }

        /// <summary>
        ///     Returns <c>true</c> if this instance is to size to the cells of a column or row.
        /// </summary>
        public bool IsSizeToCells
        {
            get { return _unitType == DataGridLengthUnitType.SizeToCells; }
        }

        /// <summary>
        ///     Returns <c>true</c> if this instance is to size to the header of a column or row.
        /// </summary>
        public bool IsSizeToHeader
        {
            get { return _unitType == DataGridLengthUnitType.SizeToHeader; }
        }

        /// <summary>
        ///     Returns value part of this DataGridLength instance.
        /// </summary>
        public float Value
        {
            get
            {
                return (_unitType == DataGridLengthUnitType.Auto) ? AutoValue : _unitValue;
            }
        }

        /// <summary>
        ///     Returns unit type of this DataGridLength instance.
        /// </summary>
        public DataGridLengthUnitType UnitType
        {
            get
            {
                return _unitType;
            }
        }

        /// <summary>
        ///     Returns the string representation of this object.
        /// </summary>
        public override string ToString()
        {
            switch (UnitType)
            {
                case DataGridLengthUnitType.Auto:
                case DataGridLengthUnitType.SizeToCells:
                case DataGridLengthUnitType.SizeToHeader:
                    return UnitType.ToString();

                // Star has one special case when value is "1.0" in which the value can be dropped.
                case DataGridLengthUnitType.Star:
                    return FloatUtil.IsOne(Value) ? "*" : Value + "*";

                // Print out the numeric value. "px" can be omitted.
                default:
                    return Value.ToString();
            }
        }

        #endregion

        #region Pre-defined values

        /// <summary>
        ///     Returns a value initialized to mean "auto."
        /// </summary>
        public static DataGridLength Auto
        {
            get { return _auto; }
        }

        /// <summary>
        ///     Returns a value initialized to mean "size to cells."
        /// </summary>
        public static DataGridLength SizeToCells
        {
            get { return _sizeToCells; }
        }

        /// <summary>
        ///     Returns a value initialized to mean "size to header."
        /// </summary>
        public static DataGridLength SizeToHeader
        {
            get { return _sizeToHeader; }
        }

        #endregion

        #region Implicit Conversions

        /// <summary>
        ///     Allows for values of type float to be implicitly converted
        ///     to DataGridLength.
        /// </summary>
        /// <param name="value">The number of pixels to represent.</param>
        /// <returns>The DataGridLength representing the requested number of pixels.</returns>
        public static implicit operator DataGridLength(float value)
        {
            return new DataGridLength(value);
        }
        public static implicit operator DataGridLength(double value)
        {
            return new DataGridLength((float)value);
        }

        /// <summary>
        /// 数字*或者直接数字或者auto,sizetocells,sizetoheader
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator DataGridLength(string n)
        {
            string goodString = n.Trim().ToLowerInvariant();
            if (goodString == "auto")
            {
                return Auto;
            }
            if (goodString == "sizetocells")
            {
                return SizeToCells;
            }
            if (goodString == "sizetoheader")
            {
                return SizeToHeader;
            }
            if (goodString == "*")
            {
                return new DataGridLength(1, DataGridLengthUnitType.Star);
            }
            try
            {
                var index = goodString.IndexOf('*');
                if (index > 0)
                {
                    return new DataGridLength(float.Parse(goodString.Substring(0, index)), DataGridLengthUnitType.Star);
                }
                return new DataGridLength(float.Parse(goodString));
            }
            catch (Exception)
            {
                throw new Exception("GridLength字符串格式无效");
            }
        }
        #endregion

        #region Fields

        private float _unitValue; // unit value storage
        private DataGridLengthUnitType _unitType; // unit type storage

        private const float AutoValue = 1f;

        // static instance of Auto DataGridLength
        private static readonly DataGridLength _auto = new DataGridLength(AutoValue, DataGridLengthUnitType.Auto);
        private static readonly DataGridLength _sizeToCells = new DataGridLength(AutoValue, DataGridLengthUnitType.SizeToCells);
        private static readonly DataGridLength _sizeToHeader = new DataGridLength(AutoValue, DataGridLengthUnitType.SizeToHeader);

        #endregion
    }
    /// <summary>
    /// 定义一些常量，这些常量指定如何调整 DataGrid 中元素的大小。
    /// </summary>
    public enum DataGridLengthUnitType : byte
    {
        /// <summary>
        /// 元素大小为以像素表示的固定值。
        ///     The value is expressed in pixels.
        /// </summary>
        Default,
        /// <summary>
        /// 元素大小基于单元格的内容和列标题。
        ///     The value indicates that content should be calculated based on the 
        ///     unconstrained sizes of all cells and header in a column.
        /// </summary>
        Auto,
        /// <summary>
        /// 元素大小基于单元格的内容。
        ///     The value indicates that content should be be calculated based on the
        ///     unconstrained sizes of all cells in a column.
        /// </summary>
        SizeToCells,
        /// <summary>
        /// 元素大小基于列标题的内容。
        ///     The value indicates that content should be calculated based on the
        ///     unconstrained size of the column header.
        /// </summary>
        SizeToHeader,
        /// <summary>
        /// 元素大小为可用空间的加权比例。
        ///     The value is expressed as a weighted proportion of available space.
        /// </summary>
        Star,
    }
}
