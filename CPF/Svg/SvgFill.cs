using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF.Drawing;

namespace CPF.Svg
{
    class SvgFill
    {
        public FillRule FillRule { get; set; }
        public PaintServer Color { get; set; }
        public double Opacity { get; set; }
        public SvgFill()
        {
            FillRule = FillRule.NonZero;
            //Color = SVG.PaintServers.Parse("#5a5b5d");
            Opacity = 100;
        }

        ViewFill fillBrush;

        public ViewFill FillBrush
        {
            get
            {

                if (fillBrush != null)
                    return fillBrush;
                if (Color != null)
                {
                    return Color.GetBrush(Opacity);
                }
                return fillBrush;
            }

            set
            {
                fillBrush = value;
            }
        }
    }
}
