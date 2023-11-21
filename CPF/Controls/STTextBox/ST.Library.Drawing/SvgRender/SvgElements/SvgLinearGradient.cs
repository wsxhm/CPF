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
    [SvgElement("linearGradient")]
    public class SvgLinearGradient : SvgGradientBrush
    {
        public override string TargetName {
            get { return "linearGradient"; }
        }

        public override Pen GetPen(SvgElement ele, float fAlpha) {
            return new Pen(this.GetBrush(ele, fAlpha));
        }

        public override Brush GetBrush(SvgElement ele, float fAlpha) {
            PointF ptFStart = PointF.Empty;
            PointF ptFEnd = PointF.Empty;
            RectangleF rectF = this.Document.ViewBox;
            bool bFlag = SvgAttributes.GetString(this, "gradientUnits", true) == "userSpaceOnUse";
            if (!bFlag) {
                using (GraphicsPath gp = ele.GetPath()) {
                    rectF = gp.GetBounds();
                }
            }
            if (this.Elements.Count <= 1) {
                return new SolidBrush(SvgAttributes.GetColor(this.Elements[0], "stop-color", false, Color.Black));
            }
            using (Matrix mm = SvgAttributes.GetTransform(ele, "transform")) {
                using (Matrix m = SvgAttributes.GetTransform(this, "gradientTransform")) {
                    mm.Multiply(m);
                    rectF.Width += Math.Abs(m.Elements[4] * 2);
                    rectF.Height += Math.Abs(m.Elements[5] * 2);
                    rectF.X -= Math.Abs(m.Elements[4]);
                    rectF.Y -= Math.Abs(m.Elements[5]);
                    ptFStart.X = this.GetX1Size(rectF, bFlag);
                    ptFStart.Y = this.GetY1Size(rectF, bFlag);
                    ptFEnd.X = this.GetX2Size(rectF, bFlag);
                    ptFEnd.Y = this.GetY2Size(rectF, bFlag);
                    if (ptFStart == ptFEnd) {
                        return null;
                    }
                    BrushInfo bi;
                    ColorBlend cb = this.GetColorBlend(fAlpha, false);
                    string strMethod = SvgAttributes.GetString(this, "spreadMethod");
                    if (string.IsNullOrEmpty(strMethod) || strMethod == "pad") {
                        if (ptFStart.X == ptFEnd.X) {
                            bi = this.SetColorBlendV(rectF, ptFStart, ptFEnd, cb);
                        } else if (ptFStart.Y == ptFEnd.Y) {
                            bi = this.SetColorBlendH(rectF, ptFStart, ptFEnd, cb);
                        } else {
                            bi = this.SetColorBlendB(rectF, ptFStart, ptFEnd, cb);
                        }
                    } else {
                        bi.Start = ptFStart;
                        bi.End = ptFEnd;
                        bi.ColorBlend = cb;
                    }
                    LinearGradientBrush brush = new LinearGradientBrush(bi.Start, bi.End, Color.Black, Color.Black);
                    brush.InterpolationColors = bi.ColorBlend;
                    if (SvgAttributes.GetString(this, "spreadMethod") != "repeat") {
                        brush.WrapMode = WrapMode.TileFlipX;
                    }
                    brush.MultiplyTransform(mm);
                    return brush;
                }
            }
        }

        private struct BrushInfo
        {
            public PointF Start;
            public PointF End;
            public ColorBlend ColorBlend;
        }

        private float GetX1Size(RectangleF rectF, bool bUserSpace) {
            string strText = this.Attributes["x1"];
            if (string.IsNullOrEmpty(strText)) {
                return rectF.X;
            }
            float fTemp = 0;
            if (strText[strText.Length - 1] == '%') {
                fTemp = rectF.Width * float.Parse(strText.Substring(0, strText.Length - 1)) / 100;
            } else {
                fTemp = SvgAttributes.GetSize(this, "x1", false, 0);
                if (!bUserSpace)
                    fTemp *= rectF.Width;
            }
            if (bUserSpace) {
                return fTemp;
            } else {
                return rectF.X + fTemp;
            }
        }

        private float GetY1Size(RectangleF rectF, bool bUserSpace) {
            string strText = this.Attributes["y1"];
            if (string.IsNullOrEmpty(strText)) {
                return rectF.Y;
            }
            float fTemp = 0;
            if (strText[strText.Length - 1] == '%') {
                fTemp = rectF.Width * float.Parse(strText.Substring(0, strText.Length - 1)) / 100;
            } else {
                fTemp = SvgAttributes.GetSize(this, "y1", false, 0);
                if (!bUserSpace)
                    fTemp *= rectF.Width;
            }
            if (bUserSpace) {
                return fTemp;
            } else {
                return rectF.Y + fTemp;
            }
        }

        private float GetX2Size(RectangleF rectF, bool bUserSpace) {
            string strText = this.Attributes["x2"];
            if (string.IsNullOrEmpty(strText)) {
                return rectF.Right;
            }
            float fTemp = 0;
            if (strText[strText.Length - 1] == '%') {
                fTemp = rectF.Width * float.Parse(strText.Substring(0, strText.Length - 1)) / 100;
            } else {
                fTemp = SvgAttributes.GetSize(this, "x2", false, 0);
                if (!bUserSpace)
                    fTemp *= rectF.Width;
            }
            if (bUserSpace) {
                return fTemp;
            } else {
                return rectF.X + fTemp;
            }
        }

        private float GetY2Size(RectangleF rectF, bool bUserSpace) {
            string strText = this.Attributes["y2"];
            if (string.IsNullOrEmpty(strText)) {
                return rectF.Y;
            }
            float fTemp = 0;
            if (strText[strText.Length - 1] == '%') {
                fTemp = rectF.Width * float.Parse(strText.Substring(0, strText.Length - 1)) / 100;
            } else {
                fTemp = SvgAttributes.GetSize(this, "y2", false, 0);
                if (!bUserSpace)
                    fTemp *= rectF.Width;
            }
            if (bUserSpace) {
                return fTemp;
            } else {
                return rectF.Y + fTemp;
            }
        }

        private BrushInfo GetColorBlend(RectangleF rectF, PointF ptF1, PointF ptF2) {
            BrushInfo bi = new BrushInfo();
            ColorBlend cb = new ColorBlend(this.Elements.Count + 2);
            cb.Colors[0] = SvgAttributes.GetColor(this.Elements[0], "stop-color", false, Color.Black);
            cb.Positions[this.Elements.Count + 1] = 1;
            cb.Colors[this.Elements.Count + 1] = SvgAttributes.GetColor(this.Elements[this.Elements.Count - 1], "stop-color", false, Color.Black);
            for (int i = 0; i < this.Elements.Count; i++) {
                string strOffset = this.Elements[i].Attributes["offset"];
                if (string.IsNullOrEmpty(strOffset)) {
                    cb.Positions[i + 1] = cb.Positions[i];
                    continue;
                }
                bool bFlag = false;
                if (strOffset[strOffset.Length - 1] == '%') {
                    bFlag = true;
                    strOffset = strOffset.Substring(0, strOffset.Length - 1);
                }
                if (!SvgAttributes.CheckIsFloat(strOffset)) {
                    cb.Positions[i + 1] = cb.Positions[i];
                    continue;
                }
                cb.Positions[i + 1] = float.Parse(strOffset);
                if (bFlag) {
                    cb.Positions[i + 1] /= 100;
                }
                cb.Colors[i + 1] = SvgAttributes.GetColor(this.Elements[i], "stop-color", false, Color.Black);
            }
            // y = ax + b -> a = (y2 - y1)/(x2 - x1)
            float a_src = (ptF2.Y - ptF1.Y) / (ptF2.X - ptF1.X);
            // b = y - ax;
            float b_src = ptF2.Y - a_src * ptF2.X;

            float angle = (float)(Math.PI / 180 * 90);  // rotate 90deg
            // b.x = a.x * cos (angle) + a.y * sin (angle)
            // b.y = a.y * cos (angle) - a.x * sin (angle)
            PointF ptF1_new = ptF1;
            ptF1_new.X = (float)(ptF1.X * Math.Cos(angle) + ptF1.Y * Math.Sign(angle));
            ptF1_new.Y = (float)(ptF1.Y * Math.Cos(angle) - ptF1.X * Math.Sign(angle));

            PointF ptF2_new = ptF2;
            ptF2_new.X = (float)(ptF2.X * Math.Cos(angle) + ptF2.Y * Math.Sign(angle));
            ptF2_new.Y = (float)(ptF2.Y * Math.Cos(angle) - ptF2.X * Math.Sign(angle));

            float a_rotate = (ptF2_new.Y - ptF1_new.Y) / (ptF2_new.X - ptF1_new.X);
            float b_left = rectF.Y - a_rotate * rectF.X;
            float b_right = rectF.Bottom - a_rotate * rectF.Right;
            // a1 * x + b1 = a2 * x + b2 => x = (b2 - b1) / (a1 - a2)
            float leftX = (b_left - b_src) / (a_src - a_rotate);
            float leftY = a_rotate * leftX + b_left;

            float rightX = (b_right - b_src) / (a_src - a_rotate);
            float rightY = a_rotate * rightX + b_right;

            float lenOld = (float)Math.Sqrt(Math.Pow(ptF2.X - ptF1.X, 2) + Math.Pow(ptF2.Y - ptF1.Y, 2));
            float lenNew = (float)Math.Sqrt(Math.Pow(rightX - leftX, 2) + Math.Pow(rightY - leftY, 2));
            float lenSeg = 0;
            if (ptF1.X < ptF2.X) {
                lenSeg = (float)Math.Sqrt(Math.Pow(ptF1.X - leftX, 2) + Math.Pow(ptF1.Y - leftY, 2));
            } else {
                lenSeg = (float)Math.Sqrt(Math.Pow(rightX - ptF2.X, 2) + Math.Pow(rightY - ptF2.Y, 2));
            }
            float fStart = lenSeg / lenNew;
            float fIncrement = lenOld / lenNew;
            for (int i = 1; i < cb.Positions.Length - 1; i++) {
                cb.Positions[i] = fStart + cb.Positions[i] * fIncrement;
            }
            bi.Start = new PointF(leftX, leftY);
            bi.End = new PointF(rightX, rightY);
            bi.ColorBlend = cb;
            return bi;
        }

        private BrushInfo SetColorBlendV(RectangleF rectF, PointF ptF1, PointF ptF2, ColorBlend cb) {
            BrushInfo bi = new BrushInfo() {
                ColorBlend = cb
            };
            float fMin = Math.Min(Math.Min(rectF.Y, ptF1.Y), ptF2.Y);
            float fMax = Math.Max(Math.Max(rectF.Bottom, ptF1.Y), ptF2.Y);
            float fLen = fMax - fMin;
            float f = (ptF2.Y - ptF1.Y) / fLen;
            float fStart = 0;
            bi.Start.X = bi.End.X = ptF1.X;
            if (f > 0) {
                fStart = (ptF1.Y - fMin) / fLen;
                bi.Start.Y = fMin;
                bi.End.Y = fMax;
            } else {
                fStart = (fMax - ptF1.Y) / fLen;
                bi.Start.Y = fMax;
                bi.End.Y = fMin;
                f = -f;
            }
            for (int i = 1; i < cb.Positions.Length - 1; i++) {
                cb.Positions[i] = fStart + cb.Positions[i] * f;
            }
            return bi;
        }

        private BrushInfo SetColorBlendH(RectangleF rectF, PointF ptF1, PointF ptF2, ColorBlend cb) {
            BrushInfo bi = new BrushInfo() {
                ColorBlend = cb
            };
            float fMin = Math.Min(Math.Min(rectF.X, ptF1.X), ptF2.X);
            float fMax = Math.Max(Math.Max(rectF.Right, ptF1.X), ptF2.X);
            float fLen = fMax - fMin;
            float f = (ptF2.X - ptF1.X) / fLen;
            float fStart = 0;
            bi.Start.Y = bi.End.Y = ptF1.Y;
            if (f > 0) {
                fStart = (ptF1.X - fMin) / fLen;
                bi.Start.X = fMin;
                bi.End.X = fMax;
            } else {
                fStart = (fMax - ptF1.X) / fLen;
                bi.Start.X = fMax;
                bi.End.X = fMin;
                f = -f;
            }
            for (int i = 1; i < cb.Positions.Length - 1; i++) {
                cb.Positions[i] = fStart + cb.Positions[i] * f;
            }

            return bi;
        }

        private BrushInfo SetColorBlendB(RectangleF rectF, PointF ptF1, PointF ptF2, ColorBlend cb) {
            BrushInfo bi = new BrushInfo() {
                ColorBlend = cb
            };
            // y = ax + b -> a = (y2 - y1)/(x2 - x1)
            float a_src = (ptF2.Y - ptF1.Y) / (ptF2.X - ptF1.X);
            // b = y - ax;
            float b_src = ptF2.Y - a_src * ptF2.X;

            float angle = (float)(Math.PI / 180 * 90);  // rotate 90deg
            // b.x = a.x * cos (angle) + a.y * sin (angle)
            // b.y = a.y * cos (angle) - a.x * sin (angle)
            float new_x1 = (float)(ptF1.X * Math.Cos(angle) + ptF1.Y * Math.Sign(angle));
            float new_y1 = (float)(ptF1.Y * Math.Cos(angle) - ptF1.X * Math.Sign(angle));

            float new_x2 = (float)(ptF2.X * Math.Cos(angle) + ptF2.Y * Math.Sign(angle));
            float new_y2 = (float)(ptF2.Y * Math.Cos(angle) - ptF2.X * Math.Sign(angle));
            // y = ax + b -> a = (y2 - y1)/(x2 - x1)
            float a_rotate = (new_y2 - new_y1) / (new_x2 - new_x1);

            PointF ptF1_temp = ptF1, ptF2_temp = ptF2;
            if (ptF1_temp.X > ptF2_temp.X) {
                var temp = ptF1_temp;
                ptF1_temp = ptF2_temp;
                ptF2_temp = temp;
            }
            float b_left = 0, b_right = 0;
            if (ptF2_temp.Y > ptF1_temp.Y) {    // check the boundary
                b_left = rectF.Y - a_rotate * rectF.X;
                b_right = rectF.Bottom - a_rotate * rectF.Right;
            } else {
                b_left = rectF.Bottom - a_rotate * rectF.Left;
                b_right = rectF.Y - a_rotate * rectF.Right;
            }
            // Get the line equation focus -> a1 * x + b1 = a2 * x + b2 => x = (b2 - b1) / (a1 - a2)
            float leftX = (b_left - b_src) / (a_src - a_rotate);
            float leftY = a_rotate * leftX + b_left;    // y = ax + b
            float rightX = (b_right - b_src) / (a_src - a_rotate);
            float rightY = a_rotate * rightX + b_right; // y = ax + b

            float lenOld = (float)Math.Sqrt(Math.Pow(ptF2.X - ptF1.X, 2) + Math.Pow(ptF2.Y - ptF1.Y, 2));
            float lenNew = (float)Math.Sqrt(Math.Pow(rightX - leftX, 2) + Math.Pow(rightY - leftY, 2));
            float fStart = 0;
            if (ptF1.X < ptF2.X) {
                bi.Start = new PointF(leftX, leftY);
                bi.End = new PointF(rightX, rightY);
                fStart = (float)Math.Sqrt(Math.Pow(ptF1.X - leftX, 2) + Math.Pow(ptF1.Y - leftY, 2)) / lenNew;
            } else {
                bi.Start = new PointF(rightX, rightY);
                bi.End = new PointF(leftX, leftY);
                fStart = (float)Math.Sqrt(Math.Pow(rightX - ptF1.X, 2) + Math.Pow(rightY - ptF1.Y, 2)) / lenNew;
            }
            float f = lenOld / lenNew;
            for (int i = 1; i < cb.Positions.Length - 1; i++) {
                cb.Positions[i] = fStart + cb.Positions[i] * f;
            }
            bi.ColorBlend = cb;
            return bi;
        }

        protected internal override void Dispose() { }
    }
}
