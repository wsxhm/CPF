using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using CPF.Reflection;

namespace CPF.Drawing
{
    [Description("颜色，格式：r,g,b,a  #rrggbbaa")]
    [TypeConverter(typeof(ColorConverter))]
    public struct Color : IFormattable, IEquatable<Color>
    {
        public static readonly Color Empty = new Color();

        /// <summary>
        /// 透明
        /// </summary>
        public static Color Transparent
        {
            get { return Color.FromArgb(0, 0, 0, 0); }
        }


        /// <summary>
        /// Well-known color: AliceBlue
        /// </summary>
        public static Color AliceBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.AliceBlue);
            }
        }

        /// <summary>
        /// Well-known color: AntiqueWhite
        /// </summary>
        public static Color AntiqueWhite
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.AntiqueWhite);
            }
        }

        /// <summary>
        /// Well-known color: Aqua
        /// </summary>
        public static Color Aqua
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Aqua);
            }
        }

        /// <summary>
        /// Well-known color: Aquamarine
        /// </summary>
        public static Color Aquamarine
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Aquamarine);
            }
        }

        /// <summary>
        /// Well-known color: Azure
        /// </summary>
        public static Color Azure
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Azure);
            }
        }

        /// <summary>
        /// Well-known color: Beige
        /// </summary>
        public static Color Beige
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Beige);
            }
        }

        /// <summary>
        /// Well-known color: Bisque
        /// </summary>
        public static Color Bisque
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Bisque);
            }
        }

        /// <summary>
        /// Well-known color: Black
        /// </summary>
        public static Color Black
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Black);
            }
        }

        /// <summary>
        /// Well-known color: BlanchedAlmond
        /// </summary>
        public static Color BlanchedAlmond
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.BlanchedAlmond);
            }
        }

        /// <summary>
        /// Well-known color: Blue
        /// </summary>
        public static Color Blue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Blue);
            }
        }

        /// <summary>
        /// Well-known color: BlueViolet
        /// </summary>
        public static Color BlueViolet
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.BlueViolet);
            }
        }

        /// <summary>
        /// Well-known color: Brown
        /// </summary>
        public static Color Brown
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Brown);
            }
        }

        /// <summary>
        /// Well-known color: BurlyWood
        /// </summary>
        public static Color BurlyWood
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.BurlyWood);
            }
        }

        /// <summary>
        /// Well-known color: CadetBlue
        /// </summary>
        public static Color CadetBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.CadetBlue);
            }
        }

        /// <summary>
        /// Well-known color: Chartreuse
        /// </summary>
        public static Color Chartreuse
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Chartreuse);
            }
        }

        /// <summary>
        /// Well-known color: Chocolate
        /// </summary>
        public static Color Chocolate
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Chocolate);
            }
        }

        /// <summary>
        /// Well-known color: Coral
        /// </summary>
        public static Color Coral
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Coral);
            }
        }

        /// <summary>
        /// Well-known color: CornflowerBlue
        /// </summary>
        public static Color CornflowerBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.CornflowerBlue);
            }
        }

        /// <summary>
        /// Well-known color: Cornsilk
        /// </summary>
        public static Color Cornsilk
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Cornsilk);
            }
        }

        /// <summary>
        /// Well-known color: Crimson
        /// </summary>
        public static Color Crimson
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Crimson);
            }
        }

        /// <summary>
        /// Well-known color: Cyan
        /// </summary>
        public static Color Cyan
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Cyan);
            }
        }

        /// <summary>
        /// Well-known color: DarkBlue
        /// </summary>
        public static Color DarkBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkBlue);
            }
        }

        /// <summary>
        /// Well-known color: DarkCyan
        /// </summary>
        public static Color DarkCyan
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkCyan);
            }
        }

        /// <summary>
        /// Well-known color: DarkGoldenrod
        /// </summary>
        public static Color DarkGoldenrod
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkGoldenrod);
            }
        }

        /// <summary>
        /// Well-known color: DarkGray
        /// </summary>
        public static Color DarkGray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkGray);
            }
        }

        /// <summary>
        /// Well-known color: DarkGreen
        /// </summary>
        public static Color DarkGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkGreen);
            }
        }

        /// <summary>
        /// Well-known color: DarkKhaki
        /// </summary>
        public static Color DarkKhaki
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkKhaki);
            }
        }

        /// <summary>
        /// Well-known color: DarkMagenta
        /// </summary>
        public static Color DarkMagenta
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkMagenta);
            }
        }

        /// <summary>
        /// Well-known color: DarkOliveGreen
        /// </summary>
        public static Color DarkOliveGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkOliveGreen);
            }
        }

        /// <summary>
        /// Well-known color: DarkOrange
        /// </summary>
        public static Color DarkOrange
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkOrange);
            }
        }

        /// <summary>
        /// Well-known color: DarkOrchid
        /// </summary>
        public static Color DarkOrchid
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkOrchid);
            }
        }

        /// <summary>
        /// Well-known color: DarkRed
        /// </summary>
        public static Color DarkRed
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkRed);
            }
        }

        /// <summary>
        /// Well-known color: DarkSalmon
        /// </summary>
        public static Color DarkSalmon
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkSalmon);
            }
        }

        /// <summary>
        /// Well-known color: DarkSeaGreen
        /// </summary>
        public static Color DarkSeaGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkSeaGreen);
            }
        }

        /// <summary>
        /// Well-known color: DarkSlateBlue
        /// </summary>
        public static Color DarkSlateBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkSlateBlue);
            }
        }

        /// <summary>
        /// Well-known color: DarkSlateGray
        /// </summary>
        public static Color DarkSlateGray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkSlateGray);
            }
        }

        /// <summary>
        /// Well-known color: DarkTurquoise
        /// </summary>
        public static Color DarkTurquoise
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkTurquoise);
            }
        }

        /// <summary>
        /// Well-known color: DarkViolet
        /// </summary>
        public static Color DarkViolet
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DarkViolet);
            }
        }

        /// <summary>
        /// Well-known color: DeepPink
        /// </summary>
        public static Color DeepPink
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DeepPink);
            }
        }

        /// <summary>
        /// Well-known color: DeepSkyBlue
        /// </summary>
        public static Color DeepSkyBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DeepSkyBlue);
            }
        }

        /// <summary>
        /// Well-known color: DimGray
        /// </summary>
        public static Color DimGray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DimGray);
            }
        }

        /// <summary>
        /// Well-known color: DodgerBlue
        /// </summary>
        public static Color DodgerBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.DodgerBlue);
            }
        }

        /// <summary>
        /// Well-known color: Firebrick
        /// </summary>
        public static Color Firebrick
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Firebrick);
            }
        }

        /// <summary>
        /// Well-known color: FloralWhite
        /// </summary>
        public static Color FloralWhite
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.FloralWhite);
            }
        }

        /// <summary>
        /// Well-known color: ForestGreen
        /// </summary>
        public static Color ForestGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.ForestGreen);
            }
        }

        /// <summary>
        /// Well-known color: Fuchsia
        /// </summary>
        public static Color Fuchsia
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Fuchsia);
            }
        }

        /// <summary>
        /// Well-known color: Gainsboro
        /// </summary>
        public static Color Gainsboro
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Gainsboro);
            }
        }

        /// <summary>
        /// Well-known color: GhostWhite
        /// </summary>
        public static Color GhostWhite
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.GhostWhite);
            }
        }

        /// <summary>
        /// Well-known color: Gold
        /// </summary>
        public static Color Gold
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Gold);
            }
        }

        /// <summary>
        /// Well-known color: Goldenrod
        /// </summary>
        public static Color Goldenrod
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Goldenrod);
            }
        }

        /// <summary>
        /// Well-known color: Gray
        /// </summary>
        public static Color Gray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Gray);
            }
        }

        /// <summary>
        /// Well-known color: Green
        /// </summary>
        public static Color Green
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Green);
            }
        }

        /// <summary>
        /// Well-known color: GreenYellow
        /// </summary>
        public static Color GreenYellow
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.GreenYellow);
            }
        }

        /// <summary>
        /// Well-known color: Honeydew
        /// </summary>
        public static Color Honeydew
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Honeydew);
            }
        }

        /// <summary>
        /// Well-known color: HotPink
        /// </summary>
        public static Color HotPink
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.HotPink);
            }
        }

        /// <summary>
        /// Well-known color: IndianRed
        /// </summary>
        public static Color IndianRed
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.IndianRed);
            }
        }

        /// <summary>
        /// Well-known color: Indigo
        /// </summary>
        public static Color Indigo
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Indigo);
            }
        }

        /// <summary>
        /// Well-known color: Ivory
        /// </summary>
        public static Color Ivory
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Ivory);
            }
        }

        /// <summary>
        /// Well-known color: Khaki
        /// </summary>
        public static Color Khaki
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Khaki);
            }
        }

        /// <summary>
        /// Well-known color: Lavender
        /// </summary>
        public static Color Lavender
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Lavender);
            }
        }

        /// <summary>
        /// Well-known color: LavenderBlush
        /// </summary>
        public static Color LavenderBlush
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LavenderBlush);
            }
        }

        /// <summary>
        /// Well-known color: LawnGreen
        /// </summary>
        public static Color LawnGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LawnGreen);
            }
        }

        /// <summary>
        /// Well-known color: LemonChiffon
        /// </summary>
        public static Color LemonChiffon
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LemonChiffon);
            }
        }

        /// <summary>
        /// Well-known color: LightBlue
        /// </summary>
        public static Color LightBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightBlue);
            }
        }

        /// <summary>
        /// Well-known color: LightCoral
        /// </summary>
        public static Color LightCoral
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightCoral);
            }
        }

        /// <summary>
        /// Well-known color: LightCyan
        /// </summary>
        public static Color LightCyan
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightCyan);
            }
        }

        /// <summary>
        /// Well-known color: LightGoldenrodYellow
        /// </summary>
        public static Color LightGoldenrodYellow
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightGoldenrodYellow);
            }
        }

        /// <summary>
        /// Well-known color: LightGray
        /// </summary>
        public static Color LightGray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightGray);
            }
        }

        /// <summary>
        /// Well-known color: LightGreen
        /// </summary>
        public static Color LightGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightGreen);
            }
        }

        /// <summary>
        /// Well-known color: LightPink
        /// </summary>
        public static Color LightPink
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightPink);
            }
        }

        /// <summary>
        /// Well-known color: LightSalmon
        /// </summary>
        public static Color LightSalmon
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightSalmon);
            }
        }

        /// <summary>
        /// Well-known color: LightSeaGreen
        /// </summary>
        public static Color LightSeaGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightSeaGreen);
            }
        }

        /// <summary>
        /// Well-known color: LightSkyBlue
        /// </summary>
        public static Color LightSkyBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightSkyBlue);
            }
        }

        /// <summary>
        /// Well-known color: LightSlateGray
        /// </summary>
        public static Color LightSlateGray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightSlateGray);
            }
        }

        /// <summary>
        /// Well-known color: LightSteelBlue
        /// </summary>
        public static Color LightSteelBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightSteelBlue);
            }
        }

        /// <summary>
        /// Well-known color: LightYellow
        /// </summary>
        public static Color LightYellow
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LightYellow);
            }
        }

        /// <summary>
        /// Well-known color: Lime
        /// </summary>
        public static Color Lime
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Lime);
            }
        }

        /// <summary>
        /// Well-known color: LimeGreen
        /// </summary>
        public static Color LimeGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.LimeGreen);
            }
        }

        /// <summary>
        /// Well-known color: Linen
        /// </summary>
        public static Color Linen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Linen);
            }
        }

        /// <summary>
        /// Well-known color: Magenta
        /// </summary>
        public static Color Magenta
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Magenta);
            }
        }

        /// <summary>
        /// Well-known color: Maroon
        /// </summary>
        public static Color Maroon
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Maroon);
            }
        }

        /// <summary>
        /// Well-known color: MediumAquamarine
        /// </summary>
        public static Color MediumAquamarine
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumAquamarine);
            }
        }

        /// <summary>
        /// Well-known color: MediumBlue
        /// </summary>
        public static Color MediumBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumBlue);
            }
        }

        /// <summary>
        /// Well-known color: MediumOrchid
        /// </summary>
        public static Color MediumOrchid
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumOrchid);
            }
        }

        /// <summary>
        /// Well-known color: MediumPurple
        /// </summary>
        public static Color MediumPurple
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumPurple);
            }
        }

        /// <summary>
        /// Well-known color: MediumSeaGreen
        /// </summary>
        public static Color MediumSeaGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumSeaGreen);
            }
        }

        /// <summary>
        /// Well-known color: MediumSlateBlue
        /// </summary>
        public static Color MediumSlateBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumSlateBlue);
            }
        }

        /// <summary>
        /// Well-known color: MediumSpringGreen
        /// </summary>
        public static Color MediumSpringGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumSpringGreen);
            }
        }

        /// <summary>
        /// Well-known color: MediumTurquoise
        /// </summary>
        public static Color MediumTurquoise
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumTurquoise);
            }
        }

        /// <summary>
        /// Well-known color: MediumVioletRed
        /// </summary>
        public static Color MediumVioletRed
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MediumVioletRed);
            }
        }

        /// <summary>
        /// Well-known color: MidnightBlue
        /// </summary>
        public static Color MidnightBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MidnightBlue);
            }
        }

        /// <summary>
        /// Well-known color: MintCream
        /// </summary>
        public static Color MintCream
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MintCream);
            }
        }

        /// <summary>
        /// Well-known color: MistyRose
        /// </summary>
        public static Color MistyRose
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.MistyRose);
            }
        }

        /// <summary>
        /// Well-known color: Moccasin
        /// </summary>
        public static Color Moccasin
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Moccasin);
            }
        }

        /// <summary>
        /// Well-known color: NavajoWhite
        /// </summary>
        public static Color NavajoWhite
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.NavajoWhite);
            }
        }

        /// <summary>
        /// Well-known color: Navy
        /// </summary>
        public static Color Navy
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Navy);
            }
        }

        /// <summary>
        /// Well-known color: OldLace
        /// </summary>
        public static Color OldLace
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.OldLace);
            }
        }

        /// <summary>
        /// Well-known color: Olive
        /// </summary>
        public static Color Olive
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Olive);
            }
        }

        /// <summary>
        /// Well-known color: OliveDrab
        /// </summary>
        public static Color OliveDrab
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.OliveDrab);
            }
        }

        /// <summary>
        /// Well-known color: Orange
        /// </summary>
        public static Color Orange
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Orange);
            }
        }

        /// <summary>
        /// Well-known color: OrangeRed
        /// </summary>
        public static Color OrangeRed
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.OrangeRed);
            }
        }

        /// <summary>
        /// Well-known color: Orchid
        /// </summary>
        public static Color Orchid
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Orchid);
            }
        }

        /// <summary>
        /// Well-known color: PaleGoldenrod
        /// </summary>
        public static Color PaleGoldenrod
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PaleGoldenrod);
            }
        }

        /// <summary>
        /// Well-known color: PaleGreen
        /// </summary>
        public static Color PaleGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PaleGreen);
            }
        }

        /// <summary>
        /// Well-known color: PaleTurquoise
        /// </summary>
        public static Color PaleTurquoise
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PaleTurquoise);
            }
        }

        /// <summary>
        /// Well-known color: PaleVioletRed
        /// </summary>
        public static Color PaleVioletRed
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PaleVioletRed);
            }
        }

        /// <summary>
        /// Well-known color: PapayaWhip
        /// </summary>
        public static Color PapayaWhip
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PapayaWhip);
            }
        }

        /// <summary>
        /// Well-known color: PeachPuff
        /// </summary>
        public static Color PeachPuff
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PeachPuff);
            }
        }

        /// <summary>
        /// Well-known color: Peru
        /// </summary>
        public static Color Peru
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Peru);
            }
        }

        /// <summary>
        /// Well-known color: Pink
        /// </summary>
        public static Color Pink
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Pink);
            }
        }

        /// <summary>
        /// Well-known color: Plum
        /// </summary>
        public static Color Plum
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Plum);
            }
        }

        /// <summary>
        /// Well-known color: PowderBlue
        /// </summary>
        public static Color PowderBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.PowderBlue);
            }
        }

        /// <summary>
        /// Well-known color: Purple
        /// </summary>
        public static Color Purple
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Purple);
            }
        }

        /// <summary>
        /// Well-known color: Red
        /// </summary>
        public static Color Red
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Red);
            }
        }

        /// <summary>
        /// Well-known color: RosyBrown
        /// </summary>
        public static Color RosyBrown
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.RosyBrown);
            }
        }

        /// <summary>
        /// Well-known color: RoyalBlue
        /// </summary>
        public static Color RoyalBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.RoyalBlue);
            }
        }

        /// <summary>
        /// Well-known color: SaddleBrown
        /// </summary>
        public static Color SaddleBrown
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SaddleBrown);
            }
        }

        /// <summary>
        /// Well-known color: Salmon
        /// </summary>
        public static Color Salmon
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Salmon);
            }
        }

        /// <summary>
        /// Well-known color: SandyBrown
        /// </summary>
        public static Color SandyBrown
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SandyBrown);
            }
        }

        /// <summary>
        /// Well-known color: SeaGreen
        /// </summary>
        public static Color SeaGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SeaGreen);
            }
        }

        /// <summary>
        /// Well-known color: SeaShell
        /// </summary>
        public static Color SeaShell
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SeaShell);
            }
        }

        /// <summary>
        /// Well-known color: Sienna
        /// </summary>
        public static Color Sienna
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Sienna);
            }
        }

        /// <summary>
        /// Well-known color: Silver
        /// </summary>
        public static Color Silver
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Silver);
            }
        }

        /// <summary>
        /// Well-known color: SkyBlue
        /// </summary>
        public static Color SkyBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SkyBlue);
            }
        }

        /// <summary>
        /// Well-known color: SlateBlue
        /// </summary>
        public static Color SlateBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SlateBlue);
            }
        }

        /// <summary>
        /// Well-known color: SlateGray
        /// </summary>
        public static Color SlateGray
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SlateGray);
            }
        }

        /// <summary>
        /// Well-known color: Snow
        /// </summary>
        public static Color Snow
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Snow);
            }
        }

        /// <summary>
        /// Well-known color: SpringGreen
        /// </summary>
        public static Color SpringGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SpringGreen);
            }
        }

        /// <summary>
        /// Well-known color: SteelBlue
        /// </summary>
        public static Color SteelBlue
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.SteelBlue);
            }
        }

        /// <summary>
        /// Well-known color: Tan
        /// </summary>
        public static Color Tan
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Tan);
            }
        }

        /// <summary>
        /// Well-known color: Teal
        /// </summary>
        public static Color Teal
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Teal);
            }
        }

        /// <summary>
        /// Well-known color: Thistle
        /// </summary>
        public static Color Thistle
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Thistle);
            }
        }

        /// <summary>
        /// Well-known color: Tomato
        /// </summary>
        public static Color Tomato
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Tomato);
            }
        }

        /// <summary>
        /// Well-known color: Turquoise
        /// </summary>
        public static Color Turquoise
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Turquoise);
            }
        }

        /// <summary>
        /// Well-known color: Violet
        /// </summary>
        public static Color Violet
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Violet);
            }
        }

        /// <summary>
        /// Well-known color: Wheat
        /// </summary>
        public static Color Wheat
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Wheat);
            }
        }

        /// <summary>
        /// Well-known color: White
        /// </summary>
        public static Color White
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.White);
            }
        }

        /// <summary>
        /// Well-known color: WhiteSmoke
        /// </summary>
        public static Color WhiteSmoke
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.WhiteSmoke);
            }
        }

        /// <summary>
        /// Well-known color: Yellow
        /// </summary>
        public static Color Yellow
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.Yellow);
            }
        }

        /// <summary>
        /// Well-known color: YellowGreen
        /// </summary>
        public static Color YellowGreen
        {
            get
            {
                return Color.FromUInt32((uint)KnownColor.YellowGreen);
            }
        }


        #region Constructors


        ///<summary>
        /// Color - sRgb legacy interface, assumes Rgb values are sRgb
        ///</summary>
        internal static Color FromUInt32(uint argb)// internal legacy sRGB interface
        {
            Color c1 = new Color();

            c1.sRgbColor.a = (byte)((argb & 0xff000000) >> 24);
            c1.sRgbColor.r = (byte)((argb & 0x00ff0000) >> 16);
            c1.sRgbColor.g = (byte)((argb & 0x0000ff00) >> 8);
            c1.sRgbColor.b = (byte)(argb & 0x000000ff);
            c1.scRgbColor.a = (float)c1.sRgbColor.a / 255.0f;
            c1.scRgbColor.r = sRgbToScRgb(c1.sRgbColor.r);  // note that context is undefined and thus unloaded
            c1.scRgbColor.g = sRgbToScRgb(c1.sRgbColor.g);
            c1.scRgbColor.b = sRgbToScRgb(c1.sRgbColor.b);

            c1.isFromScRgb = false;

            return c1;
        }

        ///<summary>
        /// FromScRgb
        ///</summary>
        public static Color FromScRgb(float a, float r, float g, float b)
        {
            Color c1 = new Color();

            c1.scRgbColor.r = r;
            c1.scRgbColor.g = g;
            c1.scRgbColor.b = b;
            c1.scRgbColor.a = a;
            if (a < 0.0f)
            {
                a = 0.0f;
            }
            else if (a > 1.0f)
            {
                a = 1.0f;
            }

            c1.sRgbColor.a = (byte)((a * 255.0f) + 0.5f);
            c1.sRgbColor.r = ScRgbTosRgb(c1.scRgbColor.r);
            c1.sRgbColor.g = ScRgbTosRgb(c1.scRgbColor.g);
            c1.sRgbColor.b = ScRgbTosRgb(c1.scRgbColor.b);

            c1.isFromScRgb = true;

            return c1;
        }
        public static Color FromRgba(byte r, byte g, byte b, byte a)
        {
            return FromArgb(a, r, g, b);
        }
        ///<summary>
        /// Color - sRgb legacy interface, assumes Rgb values are sRgb, alpha channel is linear 1.0 gamma
        ///</summary>
        public static Color FromArgb(byte a, byte r, byte g, byte b)// legacy sRGB interface, bytes are required to properly round trip
        {
            Color c1 = new Color();

            c1.scRgbColor.a = a / 255.0f;
            c1.scRgbColor.r = sRgbToScRgb(r);  // note that context is undefined and thus unloaded
            c1.scRgbColor.g = sRgbToScRgb(g);
            c1.scRgbColor.b = sRgbToScRgb(b);
            c1.sRgbColor.a = a;
            c1.sRgbColor.r = ScRgbTosRgb(c1.scRgbColor.r);
            c1.sRgbColor.g = ScRgbTosRgb(c1.scRgbColor.g);
            c1.sRgbColor.b = ScRgbTosRgb(c1.scRgbColor.b);

            c1.isFromScRgb = false;

            return c1;
        }

        ///<summary>
        /// Color - sRgb legacy interface, assumes Rgb values are sRgb
        ///</summary>
        public static Color FromRgb(byte r, byte g, byte b)// legacy sRGB interface, bytes are required to properly round trip
        {
            Color c1 = Color.FromArgb(0xff, r, g, b);
            return c1;
        }

        /// <summary>
        /// Parses a color string. #ffffff、r,g,b、r,g,b,a
        /// </summary>
        /// <param name="s">The color string.</param>
        /// <returns>The <see cref="Color"/>.</returns>
        public static Color Parse(string s)
        {
            try
            {

                if (s[0] == '#')
                {
                    //var or = 0u;

                    //if (s.Length == 7)
                    //{
                    //    or = 0xff000000;
                    //}
                    //else if (s.Length != 9)
                    //{
                    //    throw new FormatException($"Invalid color string: '{s}'.");
                    //}

                    //return FromUInt32(uint.Parse(s.Substring(1)) | or);
                    if ((s[0] == '#') &&
                    ((s.Length == 7) || (s.Length == 4)) || (s.Length == 9))
                    {
                        if (s.Length == 9)
                        {
                            return Color.FromArgb(Convert.ToByte(s.Substring(7, 2), 16),
                                                Convert.ToByte(s.Substring(1, 2), 16),
                                                Convert.ToByte(s.Substring(3, 2), 16),
                                                Convert.ToByte(s.Substring(5, 2), 16));
                        }
                        if (s.Length == 7)
                        {
                            return Color.FromRgb(Convert.ToByte(s.Substring(1, 2), 16),
                                                Convert.ToByte(s.Substring(3, 2), 16),
                                                Convert.ToByte(s.Substring(5, 2), 16));
                        }
                        else
                        {
                            string r = Char.ToString(s[1]);
                            string g = Char.ToString(s[2]);
                            string b = Char.ToString(s[3]);

                            return Color.FromRgb(Convert.ToByte(r + r, 16),
                                                Convert.ToByte(g + g, 16),
                                                Convert.ToByte(b + b, 16));
                        }
                    }
                }
                else if (s.StartsWith("rgb("))
                {
                    var ns = s.Substring(4).Trim(')').Split(',');
                    if (ns.Length == 3)
                    {
                        return Color.FromRgb(byte.Parse(ns[0]), byte.Parse(ns[1]), byte.Parse(ns[2]));
                    }
                }
                else if (s.StartsWith("rgba("))
                {
                    var ns = s.Substring(5).Trim(')').Split(',');
                    if (ns.Length == 4)
                    {
                        return Color.FromArgb((byte)(float.Parse(ns[3]) * 255), byte.Parse(ns[0]), byte.Parse(ns[1]), byte.Parse(ns[2]));
                    }
                }
                else if (s.IndexOf(',') > 0)
                {
                    var ns = s.Split(',');
                    if (ns.Length == 3)
                    {
                        return Color.FromRgb(byte.Parse(ns[0]), byte.Parse(ns[1]), byte.Parse(ns[2]));
                    }
                    else if (ns.Length == 4)
                    {
                        return Color.FromRgba(byte.Parse(ns[0]), byte.Parse(ns[1]), byte.Parse(ns[2]), byte.Parse(ns[3]));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("无效颜色字符串:" + s, e);
            }
            //else
            //{
            var upper = s.ToUpperInvariant();
            var member = typeof(Color).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .FirstOrDefault(x => x.Name.ToUpperInvariant() == upper);

            if (member != null)
            {
                return (Color)member.GetValue(null, null);
            }
            else
            {
                throw new FormatException($"Invalid color string: '{s}'.");
            }
            //}
        }

        public static bool TryParse(string s, out Color color)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    color = Color.Transparent;
                    return true;
                }
                if (s[0] == '#')
                {
                    //var or = 0u;

                    //if (s.Length == 7)
                    //{
                    //    or = 0xff000000;
                    //}
                    //else if (s.Length != 9)
                    //{
                    //    throw new FormatException($"Invalid color string: '{s}'.");
                    //}

                    //return FromUInt32(uint.Parse(s.Substring(1)) | or);
                    if ((s[0] == '#') &&
                    ((s.Length == 7) || (s.Length == 4)) || (s.Length == 9))
                    {
                        if (s.Length == 9)
                        {
                            color = Color.FromArgb(Convert.ToByte(s.Substring(7, 2), 16),
                                                Convert.ToByte(s.Substring(1, 2), 16),
                                                Convert.ToByte(s.Substring(3, 2), 16),
                                                Convert.ToByte(s.Substring(5, 2), 16));
                            return true;
                        }
                        if (s.Length == 7)
                        {
                            color = Color.FromRgb(Convert.ToByte(s.Substring(1, 2), 16),
                                                Convert.ToByte(s.Substring(3, 2), 16),
                                                Convert.ToByte(s.Substring(5, 2), 16));
                            return true;
                        }
                        else
                        {
                            string r = Char.ToString(s[1]);
                            string g = Char.ToString(s[2]);
                            string b = Char.ToString(s[3]);

                            color = Color.FromRgb(Convert.ToByte(r + r, 16),
                                                Convert.ToByte(g + g, 16),
                                                Convert.ToByte(b + b, 16));
                            return true;
                        }
                    }
                }
                else if (s.StartsWith("rgb("))
                {
                    var ns = s.Substring(4).Trim(')').Split(',');
                    if (ns.Length == 3)
                    {
                        color = Color.FromRgb(byte.Parse(ns[0]), byte.Parse(ns[1]), byte.Parse(ns[2]));
                        return true;
                    }
                }
                else if (s.StartsWith("rgba("))
                {
                    var ns = s.Substring(5).Trim(')').Split(',');
                    if (ns.Length == 4)
                    {
                        color = Color.FromArgb((byte)(float.Parse(ns[3]) * 255), byte.Parse(ns[0]), byte.Parse(ns[1]), byte.Parse(ns[2]));
                        return true;
                    }
                }
                else if (s.IndexOf(',') > 0)
                {
                    var ns = s.Split(',');
                    if (ns.Length == 3)
                    {
                        color = Color.FromRgb(byte.Parse(ns[0]), byte.Parse(ns[1]), byte.Parse(ns[2]));
                        return true;
                    }
                    else if (ns.Length == 4)
                    {
                        color = Color.FromRgba(byte.Parse(ns[0]), byte.Parse(ns[1]), byte.Parse(ns[2]), byte.Parse(ns[3]));
                        return true;
                    }
                }
                var upper = s.ToUpperInvariant();
                var member = typeof(Color).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                    .FirstOrDefault(x => x.Name.ToUpperInvariant() == upper);

                if (member != null)
                {
                    color = (Color)member.FastGetValue(null);
                    return true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("颜色格式不对:" + s + " " + e.Message);
            }
            color = new Color();
            return false;
        }

        /// <summary>
        /// Parses a color string. #ffffff、r,g,b、r,g,b,a
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator Color(string n)
        {
            return Parse(n);
        }

        public static implicit operator Color(Styling.HtmlColor n)
        {
            return Color.FromArgb(n.A, n.R, n.G, n.B);
        }
        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods
        ///<summary>
        /// GetHashCode
        ///</summary>
        public override int GetHashCode()
        {
            return this.scRgbColor.GetHashCode(); //^this.context.GetHashCode();
        }

        /// <summary>
        /// Creates a string representation of this object based on the current culture.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        public override string ToString()
        {
            // Delegate to the internal method which implements all ToString calls.

            string format = isFromScRgb ? c_scRgbFormat : null;

            return ConvertToString(format, null);
        }

        /// <summary>
        /// Creates a string representation of this object based on the IFormatProvider
        /// passed in.  If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        public string ToString(IFormatProvider provider)
        {
            // Delegate to the internal method which implements all ToString calls.

            string format = isFromScRgb ? c_scRgbFormat : null;

            return ConvertToString(format, provider);
        }

        /// <summary>
        /// Creates a string representation of this object based on the format string
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(format, provider);
        }

        /// <summary>
        /// Creates a string representation of this object based on the format string 
        /// and IFormatProvider passed in.  
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        internal string ConvertToString(string format, IFormatProvider provider)
        {
            StringBuilder sb = new StringBuilder();

            if (format == null)
            {
                sb.AppendFormat(provider, "#{0:X2}", this.sRgbColor.r);
                sb.AppendFormat(provider, "{0:X2}", this.sRgbColor.g);
                sb.AppendFormat(provider, "{0:X2}", this.sRgbColor.b);
                if (this.sRgbColor.a != 255)
                {
                    sb.AppendFormat(provider, "{0:X2}", this.sRgbColor.a);
                }
            }
            else
            {
                // Helper to get the numeric list separator for a given culture.
                char separator = ',';

                sb.AppendFormat(provider,
                    "sc#{1:" + format + "}{0} {2:" + format + "}{0} {3:" + format + "}{0} {4:" + format + "}",
                    separator, scRgbColor.a, scRgbColor.r,
                    scRgbColor.g, scRgbColor.b);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Compares two colors for fuzzy equality.  This function
        /// helps compensate for the fact that float values can
        /// acquire error when operated upon
        /// </summary>
        /// <param name='color1'>The first color to compare</param>
        /// <param name='color2'>The second color to compare</param>
        /// <returns>Whether or not the two colors are equal</returns>
        public static bool AreClose(Color color1, Color color2)
        {
            return color1.IsClose(color2);
        }

        /// <summary>
        /// Compares two colors for fuzzy equality.  This function
        /// helps compensate for the fact that float values can
        /// acquire error when operated upon
        /// </summary>
        /// <param name='color'>The color to compare to this</param>
        /// <returns>Whether or not the two colors are equal</returns>
        private bool IsClose(Color color)
        {
            // Alpha is the least likely channel to differ
            bool result = true;

            //if (color.nativeColorValue == null)
            //{
            result = result && FloatUtil.AreClose(scRgbColor.r, color.scRgbColor.r);
            result = result && FloatUtil.AreClose(scRgbColor.g, color.scRgbColor.g);
            result = result && FloatUtil.AreClose(scRgbColor.b, color.scRgbColor.b);
            //}
            //else
            //{
            //    for (int i = 0; i < color.nativeColorValue.GetLength(0); i++)
            //        result = result && FloatUtil.AreClose(nativeColorValue[i], color.nativeColorValue[i]);
            //}

            return result && FloatUtil.AreClose(scRgbColor.a, color.scRgbColor.a);
        }

        ///<summary>
        /// Clamp - the color channels to the gamut [0..1].  If a channel is out
        /// of gamut, it will be set to 1, which represents full saturation.
        /// todo: [....] up context values if they exist
        ///</summary>
        public void Clamp()
        {
            scRgbColor.r = (scRgbColor.r < 0) ? 0 : (scRgbColor.r > 1.0f) ? 1.0f : scRgbColor.r;
            scRgbColor.g = (scRgbColor.g < 0) ? 0 : (scRgbColor.g > 1.0f) ? 1.0f : scRgbColor.g;
            scRgbColor.b = (scRgbColor.b < 0) ? 0 : (scRgbColor.b > 1.0f) ? 1.0f : scRgbColor.b;
            scRgbColor.a = (scRgbColor.a < 0) ? 0 : (scRgbColor.a > 1.0f) ? 1.0f : scRgbColor.a;
            sRgbColor.a = (byte)(scRgbColor.a * 255f);
            sRgbColor.r = ScRgbTosRgb(scRgbColor.r);
            sRgbColor.g = ScRgbTosRgb(scRgbColor.g);
            sRgbColor.b = ScRgbTosRgb(scRgbColor.b);

            //
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Public Operators
        //
        //------------------------------------------------------

        #region Public Operators
        ///<summary>
        /// Addition operator - Adds each channel of the second color to each channel of the
        /// first and returns the result
        ///</summary>
        public static Color operator +(Color color1, Color color2)
        {
            Color c1 = FromScRgb(
                  color1.scRgbColor.a + color2.scRgbColor.a,
                  color1.scRgbColor.r + color2.scRgbColor.r,
                  color1.scRgbColor.g + color2.scRgbColor.g,
                  color1.scRgbColor.b + color2.scRgbColor.b);
            return c1;
        }

        ///<summary>
        /// Addition method - Adds each channel of the second color to each channel of the
        /// first and returns the result
        ///</summary>
        public static Color Add(Color color1, Color color2)
        {
            return (color1 + color2);
        }

        /// <summary>
        /// Subtract operator - substracts each channel of the second color from each channel of the
        /// first and returns the result
        /// </summary>
        /// <param name='color1'>The minuend</param>
        /// <param name='color2'>The subtrahend</param>
        /// <returns>Returns the unclamped differnce</returns>
        public static Color operator -(Color color1, Color color2)
        {
            Color c1 = FromScRgb(
                color1.scRgbColor.a - color2.scRgbColor.a,
                color1.scRgbColor.r - color2.scRgbColor.r,
                color1.scRgbColor.g - color2.scRgbColor.g,
                color1.scRgbColor.b - color2.scRgbColor.b
                );
            return c1;
        }

        ///<summary>
        /// Subtract method - subtracts each channel of the second color from each channel of the
        /// first and returns the result
        ///</summary>
        public static Color Subtract(Color color1, Color color2)
        {
            return (color1 - color2);
        }

        /// <summary>
        /// Multiplication operator - Multiplies each channel of the color by a coefficient and returns the result
        /// </summary>
        /// <param name='color'>The color</param>
        /// <param name='coefficient'>The coefficient</param>
        /// <returns>Returns the unclamped product</returns>
        public static Color operator *(Color color, float coefficient)
        {
            Color c1 = FromScRgb(color.scRgbColor.a * coefficient, color.scRgbColor.r * coefficient, color.scRgbColor.g * coefficient, color.scRgbColor.b * coefficient);

            return c1;
        }

        ///<summary>
        /// 乘法-每个通道的颜色乘以一个系数，并返回结果
        ///</summary>
        public static Color Multiply(Color color, float coefficient)
        {
            return (color * coefficient);
        }

        ///<summary>
        /// Equality method for two colors - return true of colors are equal, otherwise returns false
        ///</summary>
        public static bool Equals(Color color1, Color color2)
        {
            return (color1 == color2);
        }

        /// <summary>
        /// Compares two colors for exact equality.  Note that float values can acquire error
        /// when operated upon, such that an exact comparison between two values which are logically
        /// equal may fail. see cref="AreClose" for a "fuzzy" version of this comparison.
        /// </summary>
        /// <param name='color'>The color to compare to "this"</param>
        /// <returns>Whether or not the two colors are equal</returns>
        public bool Equals(Color color)
        {
            return this == color;
        }

        /// <summary>
        /// Compares two colors for exact equality.  Note that float values can acquire error
        /// when operated upon, such that an exact comparison between two vEquals(color);alues which are logically
        /// equal may fail. see cref="AreClose" for a "fuzzy" version of this comparison.
        /// </summary>
        /// <param name='o'>The object to compare to "this"</param>
        /// <returns>Whether or not the two colors are equal</returns>
        public override bool Equals(object o)
        {
            if (o is Color)
            {
                Color color = (Color)o;

                return (this == color);
            }
            else
            {
                return false;
            }
        }

        ///<summary>
        /// IsEqual operator - Compares two colors for exact equality.  Note that float values can acquire error
        /// when operated upon, such that an exact comparison between two values which are logically
        /// equal may fail. see cref="AreClose".
        ///</summary>
        public static bool operator ==(Color color1, Color color2)
        {
            if (color1.scRgbColor.r != color2.scRgbColor.r)
            {
                return false;
            }

            if (color1.scRgbColor.g != color2.scRgbColor.g)
            {
                return false;
            }

            if (color1.scRgbColor.b != color2.scRgbColor.b)
            {
                return false;
            }

            if (color1.scRgbColor.a != color2.scRgbColor.a)
            {
                return false;
            }

            return true;
        }

        ///<summary>
        /// !=
        ///</summary>
        public static bool operator !=(Color color1, Color color2)
        {
            return (!(color1 == color2));
        }
        #endregion Public Operators

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        ///<summary>
        /// A
        ///</summary>
        public byte A
        {
            get
            {
                return sRgbColor.a;
            }
            set
            {
                scRgbColor.a = (float)value / 255.0f;
                sRgbColor.a = value;
            }
        }

        /// <value>The Red channel as a byte whose range is [0..255].
        /// the value is not allowed to be out of range</value>
        /// <summary>
        /// R
        /// </summary>
        public byte R
        {
            get
            {
                return sRgbColor.r;
            }
            set
            {
                scRgbColor.r = sRgbToScRgb(value);
                sRgbColor.r = value;
            }
        }

        ///<value>The Green channel as a byte whose range is [0..255].
        /// the value is not allowed to be out of range</value><summary>
        /// G
        ///</summary>
        public byte G
        {
            get
            {
                return sRgbColor.g;
            }
            set
            {
                scRgbColor.g = sRgbToScRgb(value);
                sRgbColor.g = value;
            }
        }

        ///<value>The Blue channel as a byte whose range is [0..255].
        /// the value is not allowed to be out of range</value><summary>
        /// B
        ///</summary>
        public byte B
        {
            get
            {
                return sRgbColor.b;
            }
            set
            {
                scRgbColor.b = sRgbToScRgb(value);
                sRgbColor.b = value;
            }
        }

        ///<value>The Alpha channel as a float whose range is [0..1].
        /// the value is allowed to be out of range</value><summary>
        /// ScA
        ///</summary>
        public float ScA
        {
            get
            {
                return scRgbColor.a;
            }
            set
            {
                scRgbColor.a = value;
                if (value < 0.0f)
                {
                    sRgbColor.a = 0;
                }
                else if (value > 1.0f)
                {
                    sRgbColor.a = (byte)255;
                }
                else
                {
                    sRgbColor.a = (byte)(value * 255f);
                }
            }
        }

        ///<value>The Red channel as a float whose range is [0..1].
        /// the value is allowed to be out of range</value>
        ///<summary>
        /// ScR
        ///</summary>
        public float ScR
        {
            get
            {
                return scRgbColor.r;
                // throw new ArgumentException(SR.Get(SRID.Color_ColorContextNotsRgb_or_ScRgb, null));
            }
            set
            {
                scRgbColor.r = value;
                sRgbColor.r = ScRgbTosRgb(value);
            }
        }

        ///<value>The Green channel as a float whose range is [0..1].
        /// the value is allowed to be out of range</value><summary>
        /// ScG
        ///</summary>
        public float ScG
        {
            get
            {
                return scRgbColor.g;
                // throw new ArgumentException(SR.Get(SRID.Color_ColorContextNotsRgb_or_ScRgb, null));
            }
            set
            {
                scRgbColor.g = value;
                sRgbColor.g = ScRgbTosRgb(value);
            }
        }

        ///<value>The Blue channel as a float whose range is [0..1].
        /// the value is allowed to be out of range</value><summary>
        /// ScB
        ///</summary>
        public float ScB
        {
            get
            {
                return scRgbColor.b;
                // throw new ArgumentException(SR.Get(SRID.Color_ColorContextNotsRgb_or_ScRgb, null));
            }
            set
            {
                scRgbColor.b = value;
                sRgbColor.b = ScRgbTosRgb(value);
            }
        }

        #endregion Public Properties

        //------------------------------------------------------
        //
        //  Public Events
        //
        //------------------------------------------------------
        //------------------------------------------------------
        //
        //  Public Events
        //
        //------------------------------------------------------
        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------
        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------
        //------------------------------------------------------
        //
        //  Internal Events
        //
        //------------------------------------------------------
        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------
        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------
        #region Private Methods

        ///<summary>
        /// private helper function to set context values from a color value with a set context and ScRgb values
        ///</summary>
        private static float sRgbToScRgb(byte bval)
        {
            float val = (bval / 255.0f);

            if (!(val > 0.0))       // Handles NaN case too. (Though, NaN isn't actually
            // possible in this case.)
            {
                return (0.0f);
            }
            else if (val <= 0.04045)
            {
                return (val / 12.92f);
            }
            else if (val < 1.0f)
            {
                return (float)Math.Pow(((double)val + 0.055) / 1.055, 2.4);
            }
            else
            {
                return (1.0f);
            }
        }

        ///<summary>
        /// private helper function to set context values from a color value with a set context and ScRgb values
        ///</summary>
        ///
        private static byte ScRgbTosRgb(float val)
        {
            if (!(val > 0.0))       // Handles NaN case too
            {
                return (0);
            }
            else if (val <= 0.0031308)
            {
                return ((byte)((255.0f * val * 12.92f) + 0.5f));
            }
            else if (val < 1.0)
            {
                return ((byte)((255.0f * ((1.055f * (float)Math.Pow(val, (1.0 / 2.4))) - 0.055f)) + 0.5f));
            }
            else
            {
                return (255);
            }
        }

        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private struct MILColorF // this structure is the "milrendertypes.h" structure and should be identical for performance
        {
            public float a, r, g, b;
        };

        private MILColorF scRgbColor;

        private struct MILColor
        {
            public byte a, r, g, b;
        }

        private MILColor sRgbColor;

        //private float[] nativeColorValue;

        private bool isFromScRgb;

        private const string c_scRgbFormat = "R";

        #endregion Private Fields
    }

    internal enum KnownColor : uint
    {
        // We've reserved the value "1" as unknown.  If for some odd reason "1" is added to the
        // list, redefined UnknownColor

        AliceBlue = 0xFFF0F8FF,
        AntiqueWhite = 0xFFFAEBD7,
        Aqua = 0xFF00FFFF,
        Aquamarine = 0xFF7FFFD4,
        Azure = 0xFFF0FFFF,
        Beige = 0xFFF5F5DC,
        Bisque = 0xFFFFE4C4,
        Black = 0xFF000000,
        BlanchedAlmond = 0xFFFFEBCD,
        Blue = 0xFF0000FF,
        BlueViolet = 0xFF8A2BE2,
        Brown = 0xFFA52A2A,
        BurlyWood = 0xFFDEB887,
        CadetBlue = 0xFF5F9EA0,
        Chartreuse = 0xFF7FFF00,
        Chocolate = 0xFFD2691E,
        Coral = 0xFFFF7F50,
        CornflowerBlue = 0xFF6495ED,
        Cornsilk = 0xFFFFF8DC,
        Crimson = 0xFFDC143C,
        Cyan = 0xFF00FFFF,
        DarkBlue = 0xFF00008B,
        DarkCyan = 0xFF008B8B,
        DarkGoldenrod = 0xFFB8860B,
        DarkGray = 0xFFA9A9A9,
        DarkGreen = 0xFF006400,
        DarkKhaki = 0xFFBDB76B,
        DarkMagenta = 0xFF8B008B,
        DarkOliveGreen = 0xFF556B2F,
        DarkOrange = 0xFFFF8C00,
        DarkOrchid = 0xFF9932CC,
        DarkRed = 0xFF8B0000,
        DarkSalmon = 0xFFE9967A,
        DarkSeaGreen = 0xFF8FBC8F,
        DarkSlateBlue = 0xFF483D8B,
        DarkSlateGray = 0xFF2F4F4F,
        DarkTurquoise = 0xFF00CED1,
        DarkViolet = 0xFF9400D3,
        DeepPink = 0xFFFF1493,
        DeepSkyBlue = 0xFF00BFFF,
        DimGray = 0xFF696969,
        DodgerBlue = 0xFF1E90FF,
        Firebrick = 0xFFB22222,
        FloralWhite = 0xFFFFFAF0,
        ForestGreen = 0xFF228B22,
        Fuchsia = 0xFFFF00FF,
        Gainsboro = 0xFFDCDCDC,
        GhostWhite = 0xFFF8F8FF,
        Gold = 0xFFFFD700,
        Goldenrod = 0xFFDAA520,
        Gray = 0xFF808080,
        Green = 0xFF008000,
        GreenYellow = 0xFFADFF2F,
        Honeydew = 0xFFF0FFF0,
        HotPink = 0xFFFF69B4,
        IndianRed = 0xFFCD5C5C,
        Indigo = 0xFF4B0082,
        Ivory = 0xFFFFFFF0,
        Khaki = 0xFFF0E68C,
        Lavender = 0xFFE6E6FA,
        LavenderBlush = 0xFFFFF0F5,
        LawnGreen = 0xFF7CFC00,
        LemonChiffon = 0xFFFFFACD,
        LightBlue = 0xFFADD8E6,
        LightCoral = 0xFFF08080,
        LightCyan = 0xFFE0FFFF,
        LightGoldenrodYellow = 0xFFFAFAD2,
        LightGreen = 0xFF90EE90,
        LightGray = 0xFFD3D3D3,
        LightPink = 0xFFFFB6C1,
        LightSalmon = 0xFFFFA07A,
        LightSeaGreen = 0xFF20B2AA,
        LightSkyBlue = 0xFF87CEFA,
        LightSlateGray = 0xFF778899,
        LightSteelBlue = 0xFFB0C4DE,
        LightYellow = 0xFFFFFFE0,
        Lime = 0xFF00FF00,
        LimeGreen = 0xFF32CD32,
        Linen = 0xFFFAF0E6,
        Magenta = 0xFFFF00FF,
        Maroon = 0xFF800000,
        MediumAquamarine = 0xFF66CDAA,
        MediumBlue = 0xFF0000CD,
        MediumOrchid = 0xFFBA55D3,
        MediumPurple = 0xFF9370DB,
        MediumSeaGreen = 0xFF3CB371,
        MediumSlateBlue = 0xFF7B68EE,
        MediumSpringGreen = 0xFF00FA9A,
        MediumTurquoise = 0xFF48D1CC,
        MediumVioletRed = 0xFFC71585,
        MidnightBlue = 0xFF191970,
        MintCream = 0xFFF5FFFA,
        MistyRose = 0xFFFFE4E1,
        Moccasin = 0xFFFFE4B5,
        NavajoWhite = 0xFFFFDEAD,
        Navy = 0xFF000080,
        OldLace = 0xFFFDF5E6,
        Olive = 0xFF808000,
        OliveDrab = 0xFF6B8E23,
        Orange = 0xFFFFA500,
        OrangeRed = 0xFFFF4500,
        Orchid = 0xFFDA70D6,
        PaleGoldenrod = 0xFFEEE8AA,
        PaleGreen = 0xFF98FB98,
        PaleTurquoise = 0xFFAFEEEE,
        PaleVioletRed = 0xFFDB7093,
        PapayaWhip = 0xFFFFEFD5,
        PeachPuff = 0xFFFFDAB9,
        Peru = 0xFFCD853F,
        Pink = 0xFFFFC0CB,
        Plum = 0xFFDDA0DD,
        PowderBlue = 0xFFB0E0E6,
        Purple = 0xFF800080,
        Red = 0xFFFF0000,
        RosyBrown = 0xFFBC8F8F,
        RoyalBlue = 0xFF4169E1,
        SaddleBrown = 0xFF8B4513,
        Salmon = 0xFFFA8072,
        SandyBrown = 0xFFF4A460,
        SeaGreen = 0xFF2E8B57,
        SeaShell = 0xFFFFF5EE,
        Sienna = 0xFFA0522D,
        Silver = 0xFFC0C0C0,
        SkyBlue = 0xFF87CEEB,
        SlateBlue = 0xFF6A5ACD,
        SlateGray = 0xFF708090,
        Snow = 0xFFFFFAFA,
        SpringGreen = 0xFF00FF7F,
        SteelBlue = 0xFF4682B4,
        Tan = 0xFFD2B48C,
        Teal = 0xFF008080,
        Thistle = 0xFFD8BFD8,
        Tomato = 0xFFFF6347,
        Transparent = 0x00FFFFFF,
        Turquoise = 0xFF40E0D0,
        Violet = 0xFFEE82EE,
        Wheat = 0xFFF5DEB3,
        White = 0xFFFFFFFF,
        WhiteSmoke = 0xFFF5F5F5,
        Yellow = 0xFFFFFF00,
        YellowGreen = 0xFF9ACD32,
        UnknownColor = 0x00000001
    }
}
