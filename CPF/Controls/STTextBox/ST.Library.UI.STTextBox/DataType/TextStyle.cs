using CPF;
using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    public struct TextStyle
    {
        private string EmptyGUID { get; set; }
        public string Name;
        public Color ForeColor;
        public Color BackColor;
        public Color UnderLineColor;
        public Color StrikeOutColor;
        public FontStyles FontStyle;
        public bool RejectMix;

        public static TextStyle Empty = new TextStyle() {
            EmptyGUID = Guid.NewGuid().ToString()
        };

        public void Mix(TextStyle style) {
            //this.ForeColor = _ColorMix(this.ForeColor, style.ForeColor);
            //this.BackColor = _ColorMix(this.BackColor, style.BackColor);
            //this.UnderLineColor = _ColorMix(this.UnderLineColor, style.UnderLineColor);
            //this.StrikeOutColor = _ColorMix(this.StrikeOutColor, style.StrikeOutColor);
            //this.FontStyle |= style.FontStyle;
            if (this.ForeColor.A == 0) this.ForeColor = style.ForeColor;
            if (this.BackColor.A == 0) this.BackColor = style.BackColor;
            if (this.UnderLineColor.A == 0) this.UnderLineColor = style.UnderLineColor;
            if (this.StrikeOutColor.A == 0) this.StrikeOutColor = style.StrikeOutColor;
            this.FontStyle |= style.FontStyle;
        }

        private static Color _ColorMix(Color a, Color b) {
            // T = Foreground, B = Background, F = Mixed
            // alphaF = alphaT + alphaB * (1 - alphaT);
            // colorF = (colorT * alphaT + colorB * alphaB * (1 - alphaT)) / alphaF;
            if (a.A == 255) return a;
            float aT = (float)a.A / 255;
            float aB = (float)b.A / 255;
            float aF = aT + aB * (1 - aT);
            float temp = aB * (1 - aT); ;
            int nA, nR, nG, nB;
            nA = (int)(255 * aF);
            nR = (int)((a.R * aT + b.R * temp) / aF);
            nG = (int)((a.G * aT + b.G * temp) / aF);
            nB = (int)((a.B * aT + b.B * temp) / aF);
            return Color.FromArgb(_Range(nA, 0, 255), _Range(nR, 0, 255), _Range(nG, 0, 255), _Range(nB, 0, 255));
        }

        private static byte _Range(int num, int nMin, int nMax) {
            if (num < nMin) {
                num = nMin;
            } else if (num > nMax) {
                num = nMax;
            }
            return (byte)num;
        }
        // [override] ==============================================================
        public override string ToString() {
            return "[" + this.Name + "]{" + this.ForeColor.ConvertToString(null,null) + "," + this.BackColor.ConvertToString(null, null) + "}";
        }

        public static bool operator ==(TextStyle a, TextStyle b) {
            if (a.EmptyGUID != a.EmptyGUID) return false;
            if (a.RejectMix != b.RejectMix) return false;
            if (a.ForeColor != b.ForeColor) return false;
            if (a.BackColor != b.BackColor) return false;
            if (a.UnderLineColor != b.UnderLineColor) return false;
            if (a.StrikeOutColor != b.StrikeOutColor) return false;
            if (a.FontStyle != b.FontStyle) return false;
            return true;
        }

        public static bool operator !=(TextStyle a, TextStyle b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }

    public struct TextStyleRange : IComparable
    {
        private string EmptyGUID { get; set; }
        public int Index;
        public int Length;
        public TextStyle Style;

        public static TextStyleRange Empty = new TextStyleRange() {
            EmptyGUID = Guid.NewGuid().ToString()
        };

        public static bool operator ==(TextStyleRange a, TextStyleRange b) {
            if (a.EmptyGUID != b.EmptyGUID) return false;
            if (a.Index != b.Index) return false;
            if (a.Length != b.Length) return false;
            if (a.Style != b.Style) return false;
            return true;
        }

        public static bool operator !=(TextStyleRange a, TextStyleRange b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return "(" + this.Index + "," + this.Length + ")" + this.Style;
        }

        public int CompareTo(object obj) {
            return this.Index - ((TextStyleRange)obj).Index;
        }
    }
}
