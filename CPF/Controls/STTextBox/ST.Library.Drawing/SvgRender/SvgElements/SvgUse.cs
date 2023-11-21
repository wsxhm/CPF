using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace ST.Library.Drawing.SvgRender
{
    [SvgElement("use")]
    public class SvgUse : SvgElement
    {
        public override string TargetName {
            get { return "use"; }
        }

        public override bool AllowElementDraw {
            get { return true; }
        }

        public string LinkID { get; private set; }

        protected internal override void OnInitAttribute(string strName, string strValue) {
            switch (strName) { 
                case "href":
                case "xlink:href":
                    if(string.IsNullOrEmpty(strValue) || strValue.Length < 2){
                    return;
                    }
                    if (strValue[0] != '#') {
                        return;
                    }
                    this.LinkID = strValue.Substring(1);
                    break;
            }
        }

        protected internal override void DrawElement(System.Drawing.Graphics g) {
            if (string.IsNullOrEmpty(this.LinkID)) {
                return;
            }
            var ele = this.Document.GetElementByID(this.LinkID);
            if (ele == null) {
                return;
            }
            this.DrawElement(g, ele, this);
        }

        public override GraphicsPath GetPath() { return null; }

        protected internal override void Dispose() { }
    }
}
