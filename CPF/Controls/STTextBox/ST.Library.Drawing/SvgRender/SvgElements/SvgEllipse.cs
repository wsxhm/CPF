using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace ST.Library.Drawing.SvgRender
{
    [SvgElement("ellipse")]
    public class SvgEllipse : SvgElement
    {
        public override string TargetName {
            get { return "ellipse"; }
        }

        public override bool AllowElementDraw {
            get { return true; }
        }

        public float CX { get; set; }
        public float CY { get; set; }
        public float RX { get; set; }
        public float RY { get; set; }

        protected internal override void OnInitAttribute(string strName, string strValue) {
            switch (strName) {
                case "cx":
                    this.CX = SvgAttributes.GetSize(this, "cx");
                    break;
                case "cy":
                    this.CY = SvgAttributes.GetSize(this, "cy");
                    break;
                case "rx":
                    this.RX = SvgAttributes.GetSize(this, "rx");
                    break;
                case "ry":
                    this.RY = SvgAttributes.GetSize(this, "ry");
                    break;
            }
        }

        public override GraphicsPath GetPath() {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(
                SvgAttributes.GetSize(this.CurrentParent, "x") + this.CX - this.RX,
                SvgAttributes.GetSize(this.CurrentParent, "y") + this.CY - this.RY,
                this.RX * 2,
                this.RY * 2);
            return gp;
        }

        protected internal override void Dispose() { }
    }
}
