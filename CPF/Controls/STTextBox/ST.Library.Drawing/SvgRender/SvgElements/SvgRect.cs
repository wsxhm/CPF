using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace ST.Library.Drawing.SvgRender
{
    [SvgElement("rect")]
    public class SvgRect : SvgElement
    {
        public override string TargetName {
            get { return "rect"; }
        }

        public override bool AllowElementDraw {
            get { return true; }
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public float RX { get; private set; }
        public float RY { get; private set; }

        protected internal override void OnInitAttribute(string strName, string strValue) {
            switch (strName) {
                case "x":
                    this.X = SvgAttributes.GetSize(this, "x");
                    break;
                case "y":
                    this.Y = SvgAttributes.GetSize(this, "y");
                    break;
                case "width":
                    this.Width = SvgAttributes.GetSize(this, "width");
                    break;
                case "height":
                    this.Height = SvgAttributes.GetSize(this, "height");
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
            return this.GetRoundRectPath(
                SvgAttributes.GetSize(this.CurrentParent, "x") + this.X,
                SvgAttributes.GetSize(this.CurrentParent, "y") + this.Y,
                this.Width,
                this.Height,
                this.RX,
                this.RY);
        }

        protected internal override void Dispose() { }

        private GraphicsPath GetRoundRectPath(float nX, float nY, float nWidth, float nHeight, float nRX, float nRY) {
            GraphicsPath gp = new GraphicsPath();
            if (nRX == 0 || nRY == 0) {
                gp.AddRectangle(new RectangleF(nX, nY, nWidth, nHeight));
            } else {
                nRX *= 2;
                nRY *= 2;
                if (nRX > nWidth) {
                    nRX = nWidth;
                }
                if (nRY > nHeight) {
                    nRY = nHeight;
                }
                gp.AddArc(nX, nY, nRX, nRY, 180, 90);
                gp.AddArc(nX + nWidth - nRX, nY, nRX, nRY, 270, 90);
                gp.AddArc(nX + nWidth - nRX, nY + nHeight - nRY, nRX, nRY, 0, 90);
                gp.AddArc(nX, nY + nHeight - nRY, nRX, nRY, 90, 90);
                gp.CloseFigure();
            }
            return gp;
        }
    }
}
