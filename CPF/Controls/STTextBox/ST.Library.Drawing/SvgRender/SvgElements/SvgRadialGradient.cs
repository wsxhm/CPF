using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
/*
 * Note: This class implementation is not complete.
 */
namespace ST.Library.Drawing.SvgRender
{
    [SvgElement("radialGradient")]
    public class SvgRadialGradient : SvgGradientBrush
    {
        public override string TargetName {
            get { return "radialGradient"; }
        }

        public override bool AllowElementDraw {
            get { return false; }
        }

        public SvgRadialGradient() {
            this.Attributes.Set("cx", "50%");
            this.Attributes.Set("cy", "50%");
            this.Attributes.Set("r", "50%");
            this.Attributes.Set("fx", "50%");
            this.Attributes.Set("fy", "50%");
            this.Attributes.Set("fr", "0%");
        }

        protected internal override void OnInitAttribute(string strName, string strValue) { }

        public override GraphicsPath GetPath() { return null; }

        protected internal override void Dispose() { }

        private float m_cx;
        private float m_cy;
        private float m_r;
        private float m_fx;
        private float m_fy;
        private float m_fr; // not implemented

        public override Pen GetPen(SvgElement ele, float fAlpha) {
            return new Pen(this.GetBrush(ele, fAlpha));
        }

        public override Brush GetBrush(SvgElement ele, float fAlpha) {
            if (this.Elements.Count <= 1) {
                return new SolidBrush(SvgAttributes.GetColor(this.Elements[0], "stop-color", false, Color.Black));
            }
            RectangleF rectF = this.Document.ViewBox;
            bool bFlag = SvgAttributes.GetString(this, "gradientUnits", true) == "userSpaceOnUse";
            if (!bFlag) {
                using (GraphicsPath gp = ele.GetPath()) {
                    rectF = gp.GetBounds();
                }
            }
            m_cx = this.GetCXSize(rectF, bFlag);
            m_cy = this.GetCYSize(rectF, bFlag);
            m_fx = this.GetFXSize(rectF, bFlag);
            m_fy = this.GetFYSize(rectF, bFlag);
            m_r = this.GetRSize(rectF, bFlag);
            if (m_r == 0) {
                if (this.Elements.Count != 0) {
                    return new SolidBrush(SvgAttributes.GetColor(this.Elements[this.Elements.Count - 1], "stop-color", false, Color.Black));
                }
                return null;
            }
            using (Matrix mm = SvgAttributes.GetTransform(ele, "transform")) {
                using (Matrix m = SvgAttributes.GetTransform(this, "gradientTransform")) {
                    mm.Multiply(m);
                    ColorBlend cb = this.GetColorBlend(fAlpha, true);
                    this.SetColorBlend(rectF, cb, m.Elements[4], m.Elements[5]);
                    for (int i = 0; i < cb.Positions.Length / 2; i++) {
                        var temp = cb.Colors[i];
                        cb.Colors[i] = cb.Colors[cb.Colors.Length - 1 - i];
                        cb.Colors[cb.Colors.Length - 1 - i] = temp;

                    }
                    if (SvgAttributes.GetString(this, "spreadMethod") == "pad") {
                        // not implemented
                    } else {
                        // not implemented
                    }
                    using (GraphicsPath gp_e = new GraphicsPath()) {
                        gp_e.AddEllipse(new RectangleF(m_cx - m_r, m_cy - m_r, 2 * m_r, 2 * m_r));
                        PathGradientBrush brush = new PathGradientBrush(gp_e);
                        brush.CenterPoint = new PointF(m_fx, m_fy);
                        brush.InterpolationColors = cb;
                        float tx = this.Document.XScale < 1 ? mm.Elements[4] * (this.Document.XScale - 1) : 0;
                        float ty = this.Document.YScale < 1 ? mm.Elements[5] * (this.Document.YScale - 1) : 0;
                        mm.Translate(tx, ty);
                        brush.MultiplyTransform(mm);
                        return brush;
                    }
                }
            }
        }

        private float GetCXSize(RectangleF rectF, bool bUserSpace) {
            string strText = this.Attributes["cx"];
            if (string.IsNullOrEmpty(strText)) {
                return rectF.X + rectF.Width / 2;
            }
            float fTemp = 0;
            if (strText[strText.Length - 1] == '%') {
                fTemp = rectF.Width * float.Parse(strText.Substring(0, strText.Length - 1)) / 100;
            } else {
                fTemp = SvgAttributes.GetSize(this, "cx", false, 0);
                if (!bUserSpace)
                    fTemp *= rectF.Width;
            }
            if (bUserSpace) {
                return fTemp;
            } else {
                return rectF.X + fTemp;
            }
        }

        private float GetCYSize(RectangleF rectF, bool bUserSpace) {
            string strText = this.Attributes["cy"];
            if (string.IsNullOrEmpty(strText)) {
                return rectF.Y + rectF.Height / 2;
            }
            float fTemp = 0;
            if (strText[strText.Length - 1] == '%') {
                fTemp = rectF.Width * float.Parse(strText.Substring(0, strText.Length - 1)) / 100;
            } else {
                fTemp = SvgAttributes.GetSize(this, "cy", false, 0);
                if (!bUserSpace)
                    fTemp *= rectF.Height;
            }
            if (bUserSpace) {
                return fTemp;
            } else {
                return rectF.Y + fTemp;
            }
        }

        private float GetRSize(RectangleF rectF, bool bUserSpace) {
            string strText = this.Attributes["r"];
            if (string.IsNullOrEmpty(strText)) {
                return rectF.Width / 2;
            }
            float fTemp = 0;
            if (strText[strText.Length - 1] == '%') {
                fTemp = rectF.Width * float.Parse(strText.Substring(0, strText.Length - 1)) / 100;
            } else {
                fTemp = SvgAttributes.GetSize(this, "r", false, 0);
                if (!bUserSpace)
                    fTemp *= rectF.Width;
            }
            return fTemp;
        }

        private float GetFXSize(RectangleF rectF, bool bUserSpace) {
            string strText = this.Attributes["fx"];
            if (string.IsNullOrEmpty(strText)) {
                return this.GetCXSize(rectF, bUserSpace);
            }
            float fTemp = 0;
            if (strText[strText.Length - 1] == '%') {
                fTemp = rectF.Width * float.Parse(strText.Substring(0, strText.Length - 1)) / 100;
            } else {
                fTemp = SvgAttributes.GetSize(this, "fx", false, 0);
                if (!bUserSpace)
                    fTemp *= rectF.Width;
            }
            if (bUserSpace) {
                return fTemp;
            } else {
                return rectF.X + fTemp;
            }
        }

        private float GetFYSize(RectangleF rectF, bool bUserSpace) {
            string strText = this.Attributes["fy"];
            if (string.IsNullOrEmpty(strText)) {
                return this.GetCYSize(rectF, bUserSpace);
            }
            float fTemp = 0;
            if (strText[strText.Length - 1] == '%') {
                fTemp = rectF.Height * float.Parse(strText.Substring(0, strText.Length - 1)) / 100;
            } else {
                fTemp = SvgAttributes.GetSize(this, "fy", false, 0);
                if (!bUserSpace)
                    fTemp *= rectF.Height;
            }
            if (bUserSpace) {
                return fTemp;
            } else {
                return rectF.Y + fTemp;
            }
        }

        private void SetColorBlend(RectangleF rectF, ColorBlend cb, float translateX, float translateY) {
            float r_new = rectF.Width + rectF.Height + Math.Abs(translateX) + Math.Abs(translateY);
            if (m_cx > rectF.Right) r_new += m_cx - rectF.Right;
            if (m_cx < rectF.X) r_new += rectF.X - m_cx;
            if (m_cy > rectF.Bottom) r_new += m_cy - rectF.Bottom;
            if (m_cy < rectF.Y) r_new += rectF.Y - m_cy;
            float fStart = (r_new - m_r) / r_new;
            for (int i = 0; i < cb.Positions.Length; i++) {
                cb.Positions[i] = 1 - cb.Positions[i];
            }
            for (int i = 0; i < cb.Positions.Length / 2; i++) {
                var temp = cb.Positions[i];
                cb.Positions[i] = cb.Positions[cb.Positions.Length - 1 - i];
                cb.Positions[cb.Positions.Length - 1 - i] = temp;
            }
            Color[] clr_arr = new Color[cb.Colors.Length + 1];
            float[] pos_arr = new float[cb.Positions.Length + 1];

            Array.Copy(cb.Colors, 0, clr_arr, 0, cb.Colors.Length);
            Array.Copy(cb.Positions, 0, pos_arr, 1, cb.Positions.Length);
            clr_arr[clr_arr.Length - 1] = clr_arr[clr_arr.Length - 2];
            cb.Positions = pos_arr;
            cb.Colors = clr_arr;
            for (int i = 1; i < cb.Positions.Length - 1; i++) {
                cb.Positions[i] = fStart + (1 - fStart) * cb.Positions[i];
            }
            m_r = r_new;
            m_fx = m_cx + (m_fx - m_cx) * (fStart);
            m_fy = m_cy + (m_fy - m_cy) * (fStart);
        }
    }
}
