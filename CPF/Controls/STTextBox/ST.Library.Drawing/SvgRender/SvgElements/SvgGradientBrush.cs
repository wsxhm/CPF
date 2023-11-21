using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace ST.Library.Drawing.SvgRender
{
    public abstract class SvgGradientBrush : SvgElement
    {
        public override bool AllowElementDraw {
            get { return false; }
        }

        public override GraphicsPath GetPath() { return null; }

        protected internal override void OnInitAttribute(string strName, string strValue) { }

        protected virtual ColorBlend GetColorBlend(float fAlpha, bool bClear) {
            Color clr_start = SvgAttributes.GetColor(this.Elements[0], "stop-color", false, Color.Black);
            Color clr_end = SvgAttributes.GetColor(this.Elements[this.Elements.Count - 1], "stop-color", false, Color.Black);
            Color[] clr_arr = new Color[this.Elements.Count + 2];
            float[] pos_arr = new float[clr_arr.Length];
            pos_arr[pos_arr.Length - 1] = 1;
            clr_arr[0] = Color.FromArgb((int)(clr_start.A * fAlpha), clr_start);
            clr_arr[clr_arr.Length - 1] = Color.FromArgb((int)(clr_end.A * fAlpha), clr_end);
            for (int i = 0; i < this.Elements.Count; i++) {
                string strOffset = this.Elements[i].Attributes["offset"];
                if (string.IsNullOrEmpty(strOffset)) {
                    pos_arr[i + 1] = pos_arr[i];
                    continue;
                }
                bool bFlag = false;
                if (strOffset[strOffset.Length - 1] == '%') {
                    bFlag = true;
                    strOffset = strOffset.Substring(0, strOffset.Length - 1);
                }
                if (!SvgAttributes.CheckIsFloat(strOffset)) {
                    pos_arr[i + 1] = pos_arr[i];
                    continue;
                }
                pos_arr[i + 1] = float.Parse(strOffset);
                if (bFlag) {
                    pos_arr[i + 1] /= 100;
                }
                clr_arr[i + 1] = SvgAttributes.GetColor(this.Elements[i], "stop-color", false, Color.Black);
                clr_arr[i + 1] = Color.FromArgb((int)(clr_arr[i + 1].A * fAlpha), clr_arr[i + 1]);
            }
            int nIndex = 0, nLen = pos_arr.Length;
            if (bClear) {
                if (pos_arr[1] == 0) {
                    nLen--;
                    nIndex++;
                }
                if (pos_arr[pos_arr.Length - 2] == 1) {
                    nLen--;
                }
            }
            ColorBlend cb = new ColorBlend(nLen);
            Array.Copy(clr_arr, nIndex, cb.Colors, 0, nLen);
            Array.Copy(pos_arr, nIndex, cb.Positions, 0, nLen);
            return cb;
        }
    }
}
