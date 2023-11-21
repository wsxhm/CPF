using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace ST.Library.Drawing.SvgRender
{
    [SvgElement("stop")]
    public class SvgStop : SvgElement
    {
        public override string TargetName {
            get { return "stop"; }
        }

        public override bool AllowElementDraw {
            get { return false; }
        }

        protected internal override void OnInitAttribute(string strName, string strValue) { }

        public  override GraphicsPath GetPath() {
            return null;
        }

        protected internal override void Dispose() { }
    }
}
