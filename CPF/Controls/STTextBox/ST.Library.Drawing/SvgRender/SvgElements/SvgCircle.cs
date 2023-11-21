using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace ST.Library.Drawing.SvgRender
{
    [SvgElement("circle")]
    public class SvgCircle : SvgElement
    {
        public override string TargetName {
            get { return "circle"; }
        }

        public override bool AllowElementDraw {
            get { return true; }
        }

        public float CX { get; set; }
        public float CY { get; set; }
        public float R { get; set; }

        protected internal override void OnInitAttribute(string strName, string strValue) {
            switch (strName) {
                case "cx":
                    this.CX = SvgAttributes.GetSize(this, "cx");
                    break;
                case "cy":
                    this.CY = SvgAttributes.GetSize(this, "cy");
                    break;
                case "r":
                    this.R = SvgAttributes.GetSize(this, "r");
                    break;
            }
        }

        public override GraphicsPath GetPath() {
            GraphicsPath gp = new GraphicsPath();
            RectangleF rectF = new RectangleF(
                SvgAttributes.GetSize(this.CurrentParent, "x") + this.CX - this.R,
                SvgAttributes.GetSize(this.CurrentParent, "y") + this.CY - this.R,
                this.R * 2,
                this.R * 2);
            gp.AddEllipse(rectF);
            return gp;
        }

        protected internal override void Dispose() { }
    }
}
