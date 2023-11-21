using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace ST.Library.Drawing.SvgRender
{
    public abstract class SvgElement : IDisposable
    {
        private static Regex m_reg_url_id = new Regex(@"url\((?:'|"")?#(.*?)(?:'|"")?\)");
        private static Regex m_reg_style = new Regex(@"([a-zA-Z][a-zA-z0-9\-]*)\s?:\s*(.*?)(?:;|$)");

        public abstract string TargetName { get; }
        /// <summary>
        /// Specifies whether children of the current element are allowed to draw.
        /// Such as: [defs]
        /// </summary>
        public abstract bool AllowElementDraw { get; }
        public SvgDocument Document { get; private set; }
        public string OuterXml { get; private set; }
        public string InnerXml { get; private set; }
        public string InnerText { get; private set; }
        public SvgElement DomParent { get; private set; }
        public SvgElement CurrentParent {
            get {
                if (TempParent != null) {
                    return this.TempParent;
                }
                return this.DomParent;
            }
        }
        private SvgElement TempParent { get; set; }
        public SvgAttributes Attributes { get; private set; }
        public SvgElementCollection Elements { get; private set; }

        public SvgElement() {
            this.Attributes = new SvgAttributes();
            this.Elements = new SvgElementCollection(1);
        }

        public SvgElement GetElementByID(string strID) {
            foreach (var v in this.Elements) {
                if (v.Attributes["id"] == strID) {
                    return v;
                }
            }
            foreach (var v in this.Elements) {
                var ret = v.GetElementByID(strID);
                if (ret != null) {
                    return ret;
                }
            }
            return null;
        }

        public List<SvgElement> GetElementsByClass(string strClassName) {
            List<SvgElement> lst = new List<SvgElement>();
            foreach (var v in this.Elements) {
                if (v.Attributes["class"] == strClassName) {
                    lst.Add(v);
                }
                foreach (var e in v.GetElementsByClass(strClassName)) {
                    lst.Add(e);
                }
            }
            return lst;
        }

        public List<SvgElement> GetElementsByTarget(string strTargetName) {
            List<SvgElement> lst = new List<SvgElement>();
            foreach (var v in this.Elements) {
                if (v.TargetName == strTargetName) {
                    lst.Add(v);
                }
                foreach (var e in v.GetElementsByTarget(strTargetName)) {
                    lst.Add(e);
                }
            }
            return lst;
        }

        internal void InitElement(SvgDocument doc, SvgElement parent, XmlNode node) {
            this.Document = doc;
            this.DomParent = parent;
            this.OuterXml = node.OuterXml;
            this.InnerXml = node.InnerXml;
            this.InnerText = node.InnerText;
            string strStyle = null;
            foreach (XmlAttribute attr in node.Attributes) {
                if (attr.Value == "inherit") {
                    continue;
                }
                switch (attr.Name) {    // TODO: ..
                    //case "id":
                    //case "class":
                    case "style":
                        strStyle = attr.Value;
                        continue;
                }
                this.Attributes.Set(attr.Name, attr.Value);
                this.OnInitAttribute(attr.Name, attr.Value);
            }
            if (!string.IsNullOrEmpty(strStyle)) {
                foreach (Match m in m_reg_style.Matches(strStyle)) {
                    this.Attributes.Set(m.Groups[1].Value, m.Groups[2].Value);
                    this.OnInitAttribute(m.Groups[1].Value, m.Groups[2].Value);
                }
            }
            foreach (XmlNode n in node.ChildNodes) {
                if (n.LocalName[0] == '#') {
                    continue;
                }
                var ele = SvgDocument.CreateElement(n.LocalName);
                if (ele == null) {
                    continue;
                }
                ele.InitElement(doc, this, n);
                this.Elements.Add(ele);
            }
            this.OnInitElementCompleted();
        }

        protected void DrawElement(Graphics g, SvgElement ele, SvgElement tempParent) {
            ele.TempParent = tempParent;
            ele.DrawElement(g);
            ele.TempParent = null;
        }

        internal protected virtual void DrawElement(Graphics g) {
            GraphicsPath gp = this.GetDrawPath();// this.GetPath();
            if (gp != null) {
                using (gp) {
                    //gp.FillMode = SvgAttributes.GetString(this, "fill-rule") == "evenodd" ? FillMode.Alternate : FillMode.Winding;
                    string strOrder = SvgAttributes.GetString(this, "paint-order");// this.Attributes["paint-order"];
                    if (string.IsNullOrEmpty(strOrder)) {
                        strOrder = string.Empty;
                    }
                    using (var matrix = SvgAttributes.GetTransform(this, "transform", true)) {
                        gp.Transform(matrix);
                        bool bStrokeDrawed = false, bFillDrawed = false, bMarkersDrawed = false;
                        foreach (var v in strOrder.Split(' ')) {
                            switch (v) {
                                case "stroke":
                                    this.OnDrawStroke(g, gp);
                                    bStrokeDrawed = true;
                                    break;
                                case "fill":
                                    this.OnDrawFill(g, gp);
                                    bFillDrawed = true;
                                    break;
                                case "markers":
                                    this.OnDrawMarkers(g, gp);
                                    bMarkersDrawed = true;
                                    break;
                            }
                        }
                        if (!bFillDrawed) {
                            this.OnDrawFill(g, gp);
                        }
                        if (!bStrokeDrawed) {
                            this.OnDrawStroke(g, gp);
                        }
                        if (!bMarkersDrawed) {
                            this.OnDrawMarkers(g, gp);
                        }
                    }
                }
            }
            if (!this.AllowElementDraw) {
                return;
            }
            foreach (var v in this.Elements) {
                v.DrawElement(g);
            }
        }

        protected virtual Dictionary<string, string> InitDefaultAttrs() {
            return null;
        }

        protected virtual void OnInitElementCompleted() { }
        internal protected abstract void OnInitAttribute(string strName, string strValue);

        internal protected GraphicsPath GetDrawPath(SvgElement ele) {
            this.TempParent = ele;
            var gp = this.GetDrawPath();
            this.TempParent = null;
            return gp;
        }

        internal protected GraphicsPath GetDrawPath() {
            GraphicsPath gp = this.GetPath();
            if (gp == null) {
                return null;
            }
            gp.FillMode = SvgAttributes.GetString(this, "fill-rule") == "evenodd" ? FillMode.Alternate : FillMode.Winding;
            return gp;
        }

        public abstract GraphicsPath GetPath();

        internal protected abstract void Dispose();
        protected virtual void OnDrawStroke(Graphics g, GraphicsPath gp) {
            Pen p = this.GetPen();
            if (p == null) return;
            using (p) {
                g.DrawPath(p, gp);
            }
        }
        protected virtual void OnDrawFill(Graphics g, GraphicsPath gp) {
            Brush brush = this.GetBrush();
            if (brush == null) return;
            using (brush) {
                g.FillPath(brush, gp);
            }
        }
        protected virtual void OnDrawMarkers(Graphics g, GraphicsPath gp) { }

        public Pen GetPen() {
            var p = this.GetPen(this, this.GetStrokeAlpha());
            if (p == null) {
                return null;
            }
            this.SetPen(p);
            return p;
        }

        public virtual Pen GetPen(SvgElement ele, float fAlpha) {
            string strTemp = SvgAttributes.GetString(this, "stroke");
            if (string.IsNullOrEmpty(strTemp)) {
                return null;
            }
            Pen p = null;
            Match m = m_reg_url_id.Match(strTemp);
            if (m.Success) {
                SvgElement svg_obj = this.Document.GetElementByID(m.Groups[1].Value);
                if (svg_obj == null) {
                    return null;
                }
                p = svg_obj.GetPen(ele, fAlpha);
            } else {
                Color clr = SvgAttributes.GetColor(this, "stroke");
                p = new Pen(Color.FromArgb((int)(clr.A * fAlpha), clr));
            }
            p.Width = SvgAttributes.GetSize(this, "stroke-width");
            return p;
        }

        protected float GetStrokeAlpha() {
            float fOpacity = SvgAttributes.GetOpacity(this, "opacity", true);
            float fStrokeOpacity = SvgAttributes.GetFloat(this, "stroke-opacity");
            fOpacity *= fStrokeOpacity;
            if (fOpacity < 0) {
                fOpacity = 0;
            } else if (fOpacity > 1) {
                fOpacity = 1;
            }
            return fOpacity;
        }

        protected float GetFillAlpha() {
            float fOpacity = SvgAttributes.GetOpacity(this, "opacity", true);
            float fStrokeOpacity = SvgAttributes.GetFloat(this, "fill-opacity");
            return fOpacity * fStrokeOpacity;
        }

        protected virtual void SetPen(Pen p) {
            switch (SvgAttributes.GetEnum(this, "stroke-linecap", true, new string[] { "round", "square", "butt" }, "butt")) {
                case "round":
                    p.StartCap = LineCap.Round;
                    p.EndCap = LineCap.Round;
                    break;
                case "square":
                    p.StartCap = LineCap.Square;
                    p.EndCap = LineCap.Square;
                    break;
                case "butt":
                default:
                    p.StartCap = LineCap.NoAnchor;
                    p.EndCap = LineCap.NoAnchor;
                    break;
            }
            switch (SvgAttributes.GetEnum(this, "stroke-linejoin", true, new string[] { "miter-clip", "bevel", "round", "miter" }, "miter")) {
                case "miter-clip":
                    p.LineJoin = LineJoin.MiterClipped;
                    break;
                case "bevel":
                    p.LineJoin = LineJoin.Bevel;
                    break;
                case "round":
                    p.LineJoin = LineJoin.Round;
                    p.DashCap = DashCap.Round;
                    break;
                case "miter":
                default:
                    p.LineJoin = LineJoin.Miter;
                    break;
            }
            p.MiterLimit = SvgAttributes.GetFloat(this, "stroke-miterlimit");// this.GetFloat("stroke-miterlimit");
            p.DashOffset = SvgAttributes.GetFloat(this, "stroke-dashoffset") / p.Width;// this.GetFloat("stroke-dashoffset");
            float[] arr = SvgAttributes.GetSizeArray(this, "stroke-dasharray", true);
            if (arr == null) return;
            int nErrorCount = 0;
            for (int i = 0; i < arr.Length; i++) {
                if (arr[i] <= 0) {
                    nErrorCount++;
                }
                arr[i] = arr[i] / p.Width;
            }
            if (nErrorCount != 0) {
                var temp = new float[arr.Length - nErrorCount];
                for (int i = 0, j = 0; i < arr.Length; i++) {
                    if (arr[i] <= 0) continue;
                    temp[j++] = arr[i];
                }
                arr = temp;
            }
            if (arr != null) {
                if (arr.Length % 2 != 0) {
                    float[] temp_arr = new float[arr.Length * 2];
                    for (int i = 0; i < temp_arr.Length; i++) {
                        temp_arr[i] = arr[i % arr.Length];
                    }
                    p.DashPattern = temp_arr;
                } else {
                    p.DashPattern = arr;
                }
            }
        }

        public Brush GetBrush() {
            return this.GetBrush(this, this.GetFillAlpha());
        }

        public virtual Brush GetBrush(SvgElement ele, float fAlpha) {
            string strTemp = SvgAttributes.GetString(this, "fill");
            if (string.IsNullOrEmpty(strTemp)) {
                return null;
            }
            Brush brush = null;
            Match m = m_reg_url_id.Match(strTemp);
            if (m.Success) {
                SvgElement svg_obj = this.Document.GetElementByID(m.Groups[1].Value);
                if (svg_obj == null) {
                    return null;
                }
                brush = svg_obj.GetBrush(ele, fAlpha);
            } else {
                Color clr = SvgAttributes.GetColor(this, "fill");
                clr = Color.FromArgb((int)(clr.A * fAlpha), clr);
                brush = new SolidBrush(clr);
            }
            return brush;
        }

        void IDisposable.Dispose() {
            this.Dispose();
        }
    }
}
