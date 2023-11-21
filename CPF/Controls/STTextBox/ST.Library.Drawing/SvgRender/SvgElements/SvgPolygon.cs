using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace ST.Library.Drawing.SvgRender
{
    [SvgElement("polygon")]
    public class SvgPolygon : SvgElement
    {
        private static Regex m_reg_num = new Regex(@"[+-]?(\.\d+|\d+(\.\d+)?)([eE]-\d+)?");

        public override string TargetName {
            get { return "polygon"; }
        }

        public override bool AllowElementDraw {
            get { return true; }
        }

        private string _Points;

        public string Points {
            get { return _Points; }
            private set {
                m_points = this.SetPoints(value);
                string[] str_arr = new string[m_points.Length];
                for (int i = 0; i < m_points.Length; i++) {
                    str_arr[i] = m_points[i].X + "," + m_points[i].Y;
                }
                _Points = string.Join(" ", str_arr);
            }
        }

        private PointF[] m_points;

        public SvgPolygon() {
            this.Attributes.Set("fill", "black");
        }

        protected internal override void OnInitAttribute(string strName, string strValue) {
            switch (strName) {
                case "points":
                    this.Points = strValue;
                    break;
            }
        }

        public override GraphicsPath GetPath() {
            if (m_points == null || m_points.Length < 1) {
                return null;
            }
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(m_points);
            return gp;
        }

        private PointF[] SetPoints(string strPoints) {
            var ms = m_reg_num.Matches(strPoints);
            PointF[] ptFs = new PointF[ms.Count / 2];
            for (int i = 0, j = 0; i < ms.Count; i += 2, j++) {
                ptFs[j].X = float.Parse(ms[i].Value);
                ptFs[j].Y = float.Parse(ms[i + 1].Value);
            }
            return ptFs;
        }

        protected internal override void Dispose() { }
    }
}
