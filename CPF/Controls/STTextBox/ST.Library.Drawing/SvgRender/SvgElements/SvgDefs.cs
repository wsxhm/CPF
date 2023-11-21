using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.Drawing.SvgRender
{
    [SvgElement("defs")]
    public class SvgDefs : SvgElement
    {
        public override string TargetName {
            get { return "defs"; }
        }

        public override bool AllowElementDraw {
            get { return false; }
        }

        protected internal override void OnInitAttribute(string strName, string strValue) { }

        public override System.Drawing.Drawing2D.GraphicsPath GetPath() {
            return null;
        }

        protected internal override void Dispose() { }
    }
}
