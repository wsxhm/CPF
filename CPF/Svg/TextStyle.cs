using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF.Controls;
using CPF.Drawing;

namespace CPF.Svg
{
    class TextStyle
    {
        public string FontFamily { get; set; }
        public double FontSize { get; set; }
        //public FontWeight Fontweight {get; set;}
        public FontStyles Fontstyle { get; set; }
        public TextDecoration TextDecoration { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public double WordSpacing { get; set; }
        public double LetterSpacing { get; set; }
        public string BaseLineShift { get; set; }
        public void Copy(TextStyle aCopy)
        {
            if (aCopy == null)
                return;
            FontFamily = aCopy.FontFamily;
            FontSize = aCopy.FontSize;
            //Fontweight = aCopy.Fontweight;
            Fontstyle = aCopy.Fontstyle; ;
            TextAlignment = aCopy.TextAlignment;
            WordSpacing = aCopy.WordSpacing;
            LetterSpacing = aCopy.LetterSpacing;
            BaseLineShift = aCopy.BaseLineShift;
        }
        public TextStyle(TextStyle aCopy)
        {
            Copy(aCopy);
        }
        public TextStyle(Shape owner)
        {
            FontFamily = "MetricHPE Unicode MS, Verdana";
            FontSize = 12;
            //Fontweight = FontWeights.Normal;
            Fontstyle = FontStyles.Regular;
            TextAlignment = TextAlignment.Left;
            WordSpacing = 0;
            LetterSpacing = 0;
            BaseLineShift = string.Empty;
            if (owner.Parent != null)
                Copy(owner.Parent.TextStyle);
        }
    }
}
