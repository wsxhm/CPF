using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace ST.Library.Drawing.SvgRender
{
    [SvgElement("line")]
    public class SvgLine : SvgElement
    {
        public override string TargetName {
            get { return "line"; }
        }

        public override bool AllowElementDraw {
            get { return true; }
        }

        public float X1 { get; private set; }
        public float Y1 { get; private set; }
        public float X2 { get; private set; }
        public float Y2 { get; private set; }

        protected internal override void OnInitAttribute(string strName, string strValue) {
            switch (strName) {
                case "x1":
                    this.X1 = SvgAttributes.GetSize(this, "x1");
                    break;
                case "y1":
                    this.Y1 = SvgAttributes.GetSize(this, "y1");
                    break;
                case "x2":
                    this.X2 = SvgAttributes.GetSize(this, "x2");
                    break;
                case "y2":
                    this.Y2 = SvgAttributes.GetSize(this, "y2");
                    break;
            }
        }

        public override GraphicsPath GetPath() {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(
                SvgAttributes.GetSize(this.CurrentParent, "x") + this.X1,
                SvgAttributes.GetSize(this.CurrentParent, "y") + this.Y1,
                SvgAttributes.GetSize(this.CurrentParent, "x") + this.X2,
                SvgAttributes.GetSize(this.CurrentParent, "y") + this.Y2);
            return gp;
        }

        protected internal override void Dispose() {

        }
    }
}
