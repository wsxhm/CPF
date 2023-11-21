using System;
using System.Text;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    public class PrimitiveTerm : Term
    {
        public object Value { get; set; }
        public UnitType PrimitiveType { get; set; }

        public PrimitiveTerm(UnitType unitType, string value)
        {
            PrimitiveType = unitType;
            Value = value;
        }

        public PrimitiveTerm(UnitType unitType, Single value)
        {
            PrimitiveType = unitType;
            Value = value;
        }

        public PrimitiveTerm(string unit, Single value)
        {
            PrimitiveType = ConvertStringToUnitType(unit); ;
            Value = value;
        }

        public Single? GetFloatValue(UnitType unit)
        {
            if (!(Value is Single))
            {
                return null;
            }

            var quantity = (Single)Value;

            switch (unit)
            {
                case UnitType.Percentage:
                    quantity = quantity / 100f;
                    break;
            }

            return quantity;
        }

        public override string ToString()
        {
            switch (PrimitiveType)
            {
                case UnitType.String:
                    return EscapedString(Value.ToString());

                case UnitType.Uri:
                    return "url(" + Value + ")";

                default:
                    if (Value is Single)
                        return ((Single)Value).ToString(CultureInfo.InvariantCulture) + ConvertUnitTypeToString(PrimitiveType);

                    return Value.ToString();
            }
        }

        public override string GetValue()
        {
            if (PrimitiveType == UnitType.String)
            {
                var v = Value.ToString();
                StringBuilder sb = new StringBuilder(v);
                StringBuilder str = new StringBuilder();
                bool a = false;
                int index = 0;
                while (index < sb.Length)
                {
                    if (sb[index] == '\\')
                    {
                        if (a && str.Length == 0)
                        {
                            a = false;
                            sb.Remove(index, 1);
                            index--;
                        }
                        else
                        {
                            a = true;
                        }
                    }
                    else if (a)
                    {
                        var c = sb[index];
                        if (c >= '0' && c <= '9' && c >= 'A' && c <= 'F' && c >= 'a' && c <= 'f' && str.Length < 8)
                        {
                            str.Append(c);
                        }
                        else
                        {
                            if (str.Length > 0)
                            {
                                var bs = BitConverter.GetBytes(Convert.ToInt32(str.ToString(), 16));
                                string txt;
                                if (bs[0] == 0 && bs[1] == 0)
                                {
                                    txt = BitConverter.ToChar(bs, 2).ToString();
                                }
                                else
                                {
                                    txt = BitConverter.ToChar(bs, 0).ToString() + BitConverter.ToChar(bs, 2).ToString();
                                }
                                sb.Remove(index - str.Length - 1, str.Length + 1);
                                sb.Insert(index - str.Length - 1, txt);
                                str.Clear();
                                index -= (str.Length + 1 - txt.Length);
                            }
                            a = false;
                        }
                    }
                    index++;
                }
                if (str.Length > 0)
                {
                    var bs = BitConverter.GetBytes(Convert.ToInt32(str.ToString(), 16));
                    string txt;
                    if (bs[0] == 0 && bs[1] == 0)
                    {
                        txt = BitConverter.ToChar(bs, 2).ToString();
                    }
                    else
                    {
                        txt = BitConverter.ToChar(bs, 0).ToString() + BitConverter.ToChar(bs, 2).ToString();
                    }
                    sb.Remove(index - str.Length - 1, str.Length + 1);
                    sb.Insert(index - str.Length - 1, txt);
                    str.Clear();
                }

                return sb.ToString();
            }
            return base.GetValue();
        }

        internal static string EscapedString(string value)
        {
            StringBuilder encoded = new StringBuilder();

            var hasControl = false;
            foreach (var ch in value)
            {
                if (ch != '\n' && Char.IsControl(ch))
                {
                    encoded.AppendFormat("\\{0:X}", Convert.ToInt32(ch));
                    hasControl = true;
                }
                else
                    encoded.Append(ch);

            }

            char quoted = hasControl ? '\"' : '\'';
            encoded.Insert(0, quoted);
            encoded.Append(quoted);

            return encoded.ToString();
        }

        internal static UnitType ConvertStringToUnitType(string unit)
        {
            switch (unit)
            {
                case "%":
                    return UnitType.Percentage;
                case "em":
                    return UnitType.Ems;
                case "cm":
                    return UnitType.Centimeter;
                case "deg":
                    return UnitType.Degree;
                case "grad":
                    return UnitType.Grad;
                case "rad":
                    return UnitType.Radian;
                case "turn":
                    return UnitType.Turn;
                case "ex":
                    return UnitType.Exs;
                case "hz":
                    return UnitType.Hertz;
                case "in":
                    return UnitType.Inch;
                case "khz":
                    return UnitType.KiloHertz;
                case "mm":
                    return UnitType.Millimeter;
                case "ms":
                    return UnitType.Millisecond;
                case "s":
                    return UnitType.Second;
                case "pc":
                    return UnitType.Percent;
                case "pt":
                    return UnitType.Point;
                case "px":
                    return UnitType.Pixel;
                case "vw":
                    return UnitType.ViewportWidth;
                case "vh":
                    return UnitType.ViewportHeight;
                case "vmin":
                    return UnitType.ViewportMin;
                case "vmax":
                    return UnitType.ViewportMax;
                case "rem":
                    return UnitType.RootEms;
            }

            return UnitType.Unknown;
        }

        internal static string ConvertUnitTypeToString(UnitType unit)
        {
            switch (unit)
            {
                case UnitType.Percentage:
                    return "%";
                case UnitType.Ems:
                    return "em";
                case UnitType.Centimeter:
                    return "cm";
                case UnitType.Degree:
                    return "deg";
                case UnitType.Grad:
                    return "grad";
                case UnitType.Radian:
                    return "rad";
                case UnitType.Turn:
                    return "turn";
                case UnitType.Exs:
                    return "ex";
                case UnitType.Hertz:
                    return "hz";
                case UnitType.Inch:
                    return "in";
                case UnitType.KiloHertz:
                    return "khz";
                case UnitType.Millimeter:
                    return "mm";
                case UnitType.Millisecond:
                    return "ms";
                case UnitType.Second:
                    return "s";
                case UnitType.Percent:
                    return "pc";
                case UnitType.Point:
                    return "pt";
                case UnitType.Pixel:
                    return "px";
                case UnitType.ViewportWidth:
                    return "vw";
                case UnitType.ViewportHeight:
                    return "vh";
                case UnitType.ViewportMin:
                    return "vmin";
                case UnitType.ViewportMax:
                    return "vmax";
                case UnitType.RootEms:
                    return "rem";
            }

            return string.Empty;
        }
    }
}
