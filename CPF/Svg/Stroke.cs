using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF.Drawing;

namespace CPF.Svg
{
    class Stroke
    {
        public enum eLineCap
        {
            butt,
            round,
            square,
        }
        public enum eLineJoin
        {
            miter,
            round,
            bevel,
        }
        public PaintServer Color { get; set; }
        public float Width { get; set; }
        public double Opacity { get; set; }
        public eLineCap LineCap { get; set; }
        public eLineJoin LineJoin { get; set; }
        public float[] StrokeArray { get; set; }
        public Stroke()
        {
            Color = new SolidColor(SVG.PaintServers, CPF.Drawing.Color.Black);
            Width = 1;
            LineCap = eLineCap.butt;
            LineJoin = eLineJoin.miter;
            Opacity = 100;
        }
        public ViewFill StrokeBrush
        {
            get
            {
                if (Color != null)
                    return Color.GetBrush(Opacity);
                return null;
            }
        }
    }
}
