using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Text.RegularExpressions;

namespace ST.Library.Drawing.SvgRender
{
    [SvgElement("path")]
    public partial class SvgPath : SvgElement
    {
        private static Regex m_reg_d = new Regex(@"[a-zA-Z]|[+-]?(\.\d+|\d+(\.\d+)?)([eE]-\d+)?");
        public override string TargetName {
            get { return "path"; }
        }

        public override bool AllowElementDraw {
            get { return true; }
        }

        private string _D;
        public string D {
            get { return _D; }
            set {
                m_strs_d = this.SetD(value);
                _D = string.Join(" ", m_strs_d);
            }
        }

        private string[] m_strs_d;

        protected internal override void OnInitAttribute(string strName, string strValue) {
            switch (strName) { 
                case "d":
                    this.D = strValue;
                    break;
            }
        }

        private string[] SetD(string strD) {
            var ms = m_reg_d.Matches(strD);
            string[] str_arr = new string[ms.Count];
            int nIndex = 0;
            foreach (Match m in ms) {
                str_arr[nIndex++] = m.Value;
            }
            return str_arr;
        }

        public override GraphicsPath GetPath() {
            GraphicsPath gp = new GraphicsPath();
            PointF ptf_last = new PointF();
            PointF ptf_last_m = new PointF();
            PointF[] ptfs_bezier = new PointF[4];
            string[] args = m_strs_d;
            string strCmd = "l";
            for (int i = 0; i < args.Length; i++) {
                if (SvgAttributes.CheckIsFloat(args[i])) {
                    i--;        //use last command
                } else {
                    strCmd = args[i];
                }
                switch (strCmd) {
                    case "m":   //moveto
                    case "M":
                        ptf_last = this.MoveTo(strCmd == "M", ptf_last, _F(args[i + 1]), _F(args[i + 2]));
                        i += 2;
                        strCmd = strCmd == "m" ? "l" : "L";   // set the last command to "l"
                        if (i > 0) {
                            gp.CloseFigure();
                        }
                        ptf_last_m = ptf_last;
                        break;
                    case "l":   //lineto
                    case "L":
                        ptf_last = this.LineTo(strCmd == "L", gp, ptf_last, _F(args[i + 1]), _F(args[i + 2]));
                        i += 2;
                        break;
                    case "h":   //horizontal lineto
                    case "H":
                        ptf_last = this.HorizontalLineTo(strCmd == "H", gp, ptf_last, _F(args[++i]));
                        //gp.CloseFigure();
                        break;
                    case "v":   //vertical lineto
                    case "V":
                        ptf_last = this.VerticalLineTo(strCmd == "V", gp, ptf_last, _F(args[++i]));
                        //gp.CloseFigure();
                        break;
                    case "c":   //curveto
                    case "C":
                        ptfs_bezier[0] = ptf_last;
                        i++;
                        for (int j = 0; j < 3; j++) {
                            ptfs_bezier[j + 1].X = float.Parse(args[i++]);
                            ptfs_bezier[j + 1].Y = float.Parse(args[i++]);
                        }
                        i--;
                        ptf_last = this.CurveTo(strCmd == "C", gp, ptfs_bezier);
                        break;
                    case "s":   //smooth curveto
                    case "S":
                        if (ptfs_bezier[3] == ptf_last) {       // so .. the last command not a curve command
                            ptfs_bezier[0] = ptfs_bezier[3];
                            ptfs_bezier[1].X = ptfs_bezier[3].X + ptfs_bezier[3].X - ptfs_bezier[2].X;
                            ptfs_bezier[1].Y = ptfs_bezier[3].Y + ptfs_bezier[3].Y - ptfs_bezier[2].Y;
                        } else {
                            ptfs_bezier[0].X = ptfs_bezier[1].X = ptf_last.X;
                            ptfs_bezier[0].Y = ptfs_bezier[1].Y = ptf_last.Y;
                        }
                        i++;
                        for (int j = 0; j < 2; j++) {
                            ptfs_bezier[j + 2].X = float.Parse(args[i++]);
                            ptfs_bezier[j + 2].Y = float.Parse(args[i++]);
                        }
                        i--;
                        ptf_last = this.SmoothCurveTo(strCmd == "S", gp, ptfs_bezier);
                        break;
                    case "q":   //quadratic bezier curveto
                    case "Q":
                        ptfs_bezier[0] = ptf_last;
                        i++;
                        for (int j = 0; j < 2; j++) {
                            ptfs_bezier[j + 2].X = float.Parse(args[i++]);
                            ptfs_bezier[j + 2].Y = float.Parse(args[i++]);
                        }
                        ptfs_bezier[1] = ptfs_bezier[2];
                        i--;
                        ptf_last = this.QuadraticBezierCurveTo(strCmd == "Q", gp, ptfs_bezier);
                        break;
                    case "t":   //smooth quadratic bezier curveto
                    case "T":
                        ptfs_bezier[0] = ptfs_bezier[3];
                        ptfs_bezier[1].X = ptfs_bezier[3].X + ptfs_bezier[3].X - ptfs_bezier[2].X;
                        ptfs_bezier[1].Y = ptfs_bezier[3].Y + ptfs_bezier[3].Y - ptfs_bezier[2].Y;
                        ptfs_bezier[3].X = _F(args[i + 1]);
                        ptfs_bezier[3].Y = _F(args[i + 2]);
                        ptfs_bezier[2] = ptfs_bezier[1];
                        ptf_last = this.QuadraticBezierCurveTo(strCmd == "T", gp, ptfs_bezier);
                        i += 2;
                        break;
                    case "a":   //elliptical Arc
                    case "A":
                        ptf_last = this.EllipticalArc(strCmd == "A",
                            gp, ptf_last,
                            _F(args[i + 1]),
                            _F(args[i + 2]),
                            _F(args[i + 3]),
                            args[i + 4] != "0",
                            args[i + 5] != "0",
                            _F(args[i + 6]),
                            _F(args[i + 7]));
                        i += 7;
                        break;
                    case "z":   //close path
                    case "Z":
                        gp.CloseFigure();
                        ptf_last = ptf_last_m;
                        strCmd = "l";   // set the last command to "l"
                        break;
                }
            }
            return gp;
        }

        protected internal override void Dispose() { }

        private float _F(string strText) {
            if (!SvgAttributes.CheckIsFloat(strText)) {
                return 0;
            }
            return float.Parse(strText);
        }

        private PointF MoveTo(bool isAbsolute, PointF ptF, float x, float y) {
            if (!isAbsolute) {
                ptF.X += x;
                ptF.Y += y;
            } else {
                ptF.X = x;
                ptF.Y = y;
            }
            return ptF;
        }

        private PointF LineTo(bool isAbsolute, GraphicsPath gp, PointF ptF, float x, float y) {
            if (!isAbsolute) {
                gp.AddLine(ptF.X, ptF.Y, ptF.X + x, ptF.Y + y);
                ptF.X += x;
                ptF.Y += y;
            } else {
                gp.AddLine(ptF.X, ptF.Y, x, y);
                ptF.X = x;
                ptF.Y = y;
            }
            return ptF;
        }

        private PointF HorizontalLineTo(bool isAbsolute, GraphicsPath gp, PointF ptF, float x) {
            if (!isAbsolute) {
                gp.AddLine(ptF.X, ptF.Y, ptF.X + x, ptF.Y);
                ptF.X += x;
            } else {
                gp.AddLine(ptF.X, ptF.Y, x, ptF.Y);
                ptF.X = x;
            }
            return ptF;
        }

        private PointF VerticalLineTo(bool isAbsolute, GraphicsPath gp, PointF ptF, float y) {
            if (!isAbsolute) {
                gp.AddLine(ptF.X, ptF.Y, ptF.X, ptF.Y + y);
                ptF.Y += y;
            } else {
                gp.AddLine(ptF.X, ptF.Y, ptF.X, y);
                ptF.Y = y;
            }
            return ptF;
        }

        private PointF CurveTo(bool isAbsolute, GraphicsPath gp, PointF[] ptFs) {
            if (!isAbsolute) {
                ptFs[1].X += ptFs[0].X;
                ptFs[1].Y += ptFs[0].Y;
                ptFs[2].X += ptFs[0].X;
                ptFs[2].Y += ptFs[0].Y;
                ptFs[3].X += ptFs[0].X;
                ptFs[3].Y += ptFs[0].Y;
            }
            gp.AddBeziers(ptFs);
            return ptFs[3];
        }

        private PointF SmoothCurveTo(bool isAbsolute, GraphicsPath gp, PointF[] ptFs) {
            if (!isAbsolute) {
                ptFs[2].X += ptFs[0].X;
                ptFs[2].Y += ptFs[0].Y;
                ptFs[3].X += ptFs[0].X;
                ptFs[3].Y += ptFs[0].Y;
            }
            gp.AddBeziers(ptFs);
            return ptFs[3];
        }

        private PointF QuadraticBezierCurveTo(bool isAbsolute, GraphicsPath gp, PointF[] ptFs) {
            if (!isAbsolute) {
                ptFs[1].X += ptFs[0].X;
                ptFs[1].Y += ptFs[0].Y;
                ptFs[2].X += ptFs[0].X;
                ptFs[2].Y += ptFs[0].Y;
                ptFs[3].X += ptFs[0].X;
                ptFs[3].Y += ptFs[0].Y;
            }
            var temp = ptFs[2];
            // from https://stackoverflow.com/questions/3162645/convert-a-quadratic-bezier-to-a-cubic-one
            // CP1 = QP0 + 2/3 *(QP1-QP0)
            // CP2 = QP2 + 2/3 *(QP1-QP2)
            // Note: ptFs -> [P0, P1, P1, P2] -> ptFs[1] == ptFs[2] and ptFs[1,2] need to calc.
            ptFs[1].X = ptFs[0].X + (2 / 3F * (ptFs[1].X - ptFs[0].X));
            ptFs[1].Y = ptFs[0].Y + (2 / 3F * (ptFs[1].Y - ptFs[0].Y));
            // ---------------------------------------------------------------------------------
            ptFs[2].X = ptFs[3].X + (2 / 3F * (ptFs[2].X - ptFs[3].X));
            ptFs[2].Y = ptFs[3].Y + (2 / 3F * (ptFs[2].Y - ptFs[3].Y));
            gp.AddBeziers(ptFs);
            ptFs[2] = temp;
            return ptFs[3];
        }

        private PointF EllipticalArc(bool isAbsolute, GraphicsPath gp, PointF ptF, float rx, float ry, float angle, bool isLargArc, bool isSweep, float x, float y) {
            if (!isAbsolute) {
                x += ptF.X;
                y += ptF.Y;
            }
            // Convert arc to bezier point list
            var lst_points = SvgPath.ArcToBezier(ptF, rx, ry, angle, isLargArc, isSweep, x, y);
            foreach (var ps in lst_points) {
                ptF.X = (float)ps[6];
                ptF.Y = (float)ps[7];
                gp.AddBezier(
                    (float)ps[0], (float)ps[1],
                    (float)ps[2], (float)ps[3],
                    (float)ps[4], (float)ps[5],
                    ptF.X, ptF.Y);
            }
            return ptF;
        }
    }
}
