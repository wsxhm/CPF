using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

using CPF.Drawing;
using RectangleF = CPF.Drawing.Rect;

namespace ST.Library.Drawing.SvgRender
{
    public partial class SvgDocument : SvgElement
    {
        public override string TargetName {
            get { return "svg"; }
        }
        public override bool AllowElementDraw {
            get { return true; }
        }

        public float Width { get; private set; }
        public float Height { get; private set; }
        public RectangleF ViewBox { get; private set; }
        public float XScale { get; private set; }
        public float YScale { get; private set; }

        private struct DrawingRectInfo
        {
            public RectangleF Rect;
            public float XScale;
            public float YScale;
        }

        protected internal override void OnInitAttribute(string strName, string strValue) {
            var ms = m_reg_number.Matches(strValue);
            switch (strName) {
                case "viewBox":
                    if (ms.Count == 4) {
                        this.ViewBox = new RectangleF(_F(ms[0].Value), _F(ms[1].Value), _F(ms[2].Value), _F(ms[3].Value));
                    }
                    break;
                case "height":
                    if (ms.Count == 1) {
                        this.Height = _F(ms[0].Value);
                    }
                    break;
                case "width":
                    if (ms.Count == 1) {
                        this.Width = _F(ms[0].Value);
                    }
                    break;
            }
        }

        protected override void OnInitElementCompleted() {
            if (this.ViewBox == RectangleF.Empty) {
                this.ViewBox = new RectangleF(0, 0, this.Width, this.Height);
            }
            if (this.ViewBox.Width == 0 || this.ViewBox.Height == 0) {
                return;
            }
            if (this.Width == 0 && this.Height == 0) {
                this.Width = this.ViewBox.Width;
                this.Height = this.ViewBox.Height;
            } else {
                if (this.Width != 0 && this.Height == 0) {
                    this.Height = this.ViewBox.Height * this.Width / this.ViewBox.Width;
                }
                if (this.Height != 0 && this.Width == 0) {
                    this.Width = this.ViewBox.Width * this.Height / this.ViewBox.Height;
                }
            }
        }

        public override GraphicsPath GetPath() { return null; }

        public void Draw(Graphics g, Rect rectF) {
            if (rectF.Width <= 0 || rectF.Height <= 0) {
                return;
            }
            var old_g = g.Save();
            g.SmoothingMode = SmoothingMode.HighQuality;
            string strPreserveAspectRatio = this.Attributes["preserveAspectRatio"];
            if (string.IsNullOrEmpty(strPreserveAspectRatio)) {
                strPreserveAspectRatio = "xMidYMid meet";
            }
            var dri = this.CheckDrawingRect(rectF, strPreserveAspectRatio);
            this.XScale = dri.XScale;
            this.YScale = dri.YScale;
            g.SetClip(rectF);
            g.TranslateTransform(dri.Rect.X - this.ViewBox.X * dri.XScale, dri.Rect.Y - this.ViewBox.Y * dri.YScale);
            g.ScaleTransform(dri.XScale, dri.YScale);
            foreach (var v in this.Elements) {
                if (v.AllowElementDraw) {
                    v.DrawElement(g);
                }
            }
            g.Restore(old_g);
            this.Dispose(this);
        }

        private void Dispose(SvgElement ele) {
            foreach (SvgElement e in ele.Elements) {
                this.Dispose(e);
            }
            ele.Dispose();
        }

        private DrawingRectInfo CheckDrawingRect(RectangleF rectF, string strPreserveAspectRatio) {
            DrawingRectInfo dri = new DrawingRectInfo();
            if (this.ViewBox.Width == 0 || this.ViewBox.Height == 0) {
                dri.Rect = rectF;
                dri.XScale = dri.YScale;
                return dri;
            }
            string[] strs = Regex.Split(strPreserveAspectRatio, @"[,\s]+");
            foreach (var v in strs) {
                switch (v) {
                    case "meet":
                        dri = this.ProcessMeet(rectF);
                        break;
                    case "slice":
                        dri = this.ProcessSlice(rectF);
                        break;
                }
            }
            foreach (var v in strs) {
                switch (v) {
                    case "xMinYMin":
                        dri.Rect.Location = this.ProcessXMinYMin(rectF, dri.Rect);
                        break;
                    case "xMidYMin":
                        dri.Rect.Location = this.ProcessXMidYMin(rectF, dri.Rect);
                        break;
                    case "xMaxYMin":
                        dri.Rect.Location = this.ProcessXMaxYMin(rectF, dri.Rect);
                        break;
                    case "xMinYMid":
                        dri.Rect.Location = this.ProcessXMinYMid(rectF, dri.Rect);
                        break;
                    case "xMidYMid":
                        dri.Rect.Location = this.ProcessXMidYMid(rectF, dri.Rect);
                        break;
                    case "xMaxYMid":
                        dri.Rect.Location = this.ProcessXMaxYMid(rectF, dri.Rect);
                        break;
                    case "xMinYMax":
                        dri.Rect.Location = this.ProcessXMinYMax(rectF, dri.Rect);
                        break;
                    case "xMidYMax":
                        dri.Rect.Location = this.ProcessXMidYMax(rectF, dri.Rect);
                        break;
                    case "xMaxYMax":
                        dri.Rect.Location = this.ProcessXMaxYMax(rectF, dri.Rect);
                        break;
                }
            }
            return dri;
        }

        private DrawingRectInfo ProcessMeet(RectangleF rectF) {
            var dri = new DrawingRectInfo();
            float temp = this.ViewBox.Width / this.ViewBox.Height;
            if (rectF.Width / rectF.Height > temp) {
                dri.YScale = dri.XScale = rectF.Height / this.ViewBox.Height;
                dri.Rect.Width = this.ViewBox.Width * dri.XScale;
                dri.Rect.Height = rectF.Height;
                dri.Rect.Y = rectF.Y;
                dri.Rect.X = rectF.X + (rectF.Width - dri.Rect.Width) / 2;
            } else {
                dri.YScale = dri.XScale = rectF.Width / this.ViewBox.Width;
                dri.Rect.Height = this.ViewBox.Height * dri.YScale;
                dri.Rect.Width = rectF.Width;
                dri.Rect.X = rectF.X;
                dri.Rect.Y = rectF.Y + (rectF.Height - dri.Rect.Height) / 2;
            }
            return dri;
        }

        private DrawingRectInfo ProcessSlice(RectangleF rectF) {
            var dri = new DrawingRectInfo();
            float temp = this.ViewBox.Width / this.ViewBox.Height;
            if (rectF.Width / rectF.Height > temp) {
                dri.YScale = dri.XScale = rectF.Width / this.ViewBox.Width;
                dri.Rect.Height = this.ViewBox.Height * dri.YScale;
                dri.Rect.Width = rectF.Width;
                dri.Rect.X = rectF.X;
                dri.Rect.Y = rectF.Y + (rectF.Height - dri.Rect.Height) / 2;
            } else {
                dri.YScale = dri.XScale = rectF.Height / this.ViewBox.Height;
                dri.Rect.Width = this.ViewBox.Width * dri.XScale;
                dri.Rect.Height = rectF.Height;
                dri.Rect.Y = rectF.Y;
                dri.Rect.X = rectF.X + (rectF.Width - dri.Rect.Width) / 2;
            }
            return dri;
        }

        private PointF ProcessXMinYMin(RectangleF rectF, RectangleF rectFView) {
            return rectF.Location;
        }

        private PointF ProcessXMidYMin(RectangleF rectF, RectangleF rectFView) {
            rectF.X += (rectF.Width - rectFView.Width) / 2;
            return rectF.Location;
        }

        private PointF ProcessXMaxYMin(RectangleF rectF, RectangleF rectFView) {
            rectF.X = rectF.Right - rectFView.Width;
            return rectF.Location;
        }

        private PointF ProcessXMinYMid(RectangleF rectF, RectangleF rectFView) {
            rectF.Y = rectF.Y + (rectF.Height - rectFView.Height) / 2;
            return rectF.Location;
        }

        private PointF ProcessXMidYMid(RectangleF rectF, RectangleF rectFView) {
            rectF.X = rectF.X + (rectF.Width - rectFView.Width) / 2;
            rectF.Y = rectF.Y + (rectF.Height - rectFView.Height) / 2;
            return rectF.Location;
        }

        private PointF ProcessXMaxYMid(RectangleF rectF, RectangleF rectFView) {
            rectF.X = rectF.Right - rectFView.Width;
            rectF.Y = rectF.Y + (rectF.Height - rectFView.Width) / 2;
            return rectF.Location;
        }

        private PointF ProcessXMinYMax(RectangleF rectF, RectangleF rectFView) {
            rectF.X = rectF.X;
            rectF.Y = rectF.Bottom - rectFView.Height;
            return rectF.Location;
        }

        private PointF ProcessXMidYMax(RectangleF rectF, RectangleF rectFView) {
            rectF.X = rectF.X + (rectF.Width - rectFView.Width) / 2;
            rectF.Y = rectF.Bottom - rectFView.Height;
            return rectF.Location;
        }

        private PointF ProcessXMaxYMax(RectangleF rectF, RectangleF rectFView) {
            rectF.X = rectF.Right - rectFView.Height;
            rectF.Y = rectF.Bottom - rectFView.Height;
            return rectF.Location;
        }

        protected virtual void SetPen(Pen p, SvgAttributes attrs) { }

        protected virtual Brush GetBrush(SvgAttributes attr) {
            return null;
        }

        protected internal override void Dispose() { }

        private float _F(string strValue) {
            return float.Parse(strValue);
        }

        protected override void OnDrawStroke(Graphics g, GraphicsPath gp) { }

        protected override void OnDrawFill(Graphics g, GraphicsPath gp) { }

        protected override void OnDrawMarkers(Graphics g, GraphicsPath gp) { }
    }
}
