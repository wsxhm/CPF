using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CPF.Drawing;

namespace CPF.Svg
{
    abstract class SvgShape : SvgElement, ISvgShape
    {
        // Approximating a 1/4 circle with a Bezier curve                _
        internal const double c_arcAsBezier = 0.5522847498307933984; // =( \/2 - 1)*4/3

        public virtual SvgType SvgType
        {
            get { return SvgType.Shape; }
        }

        public virtual IEnumerable<ISvgShape> Elements { get { return null; } }

        SvgFill m_fill;
        Stroke m_stroke;
        //TextStyle m_textstyle;
        public virtual Stroke Stroke
        {
            get
            {
                if (m_stroke != null)
                    return m_stroke;
                if (Parent != null)
                    return Parent.Stroke;

                return null;
            }
        }
        public virtual SvgFill Fill
        {
            get
            {
                if (m_fill != null)
                    return m_fill;
                if (Parent != null)
                    return Parent.Fill;
                return null;
            }
        }
        //public virtual TextStyle TextStyle
        //{
        //    get
        //    {
        //        if (m_textstyle != null)
        //            return m_textstyle;
        //        while (Parent != null)
        //        {
        //            if (Parent.m_textstyle != null)
        //                return Parent.m_textstyle;
        //            Parent = Parent.Parent;
        //        }
        //        return null;
        //    }
        //}
        public float Opacity { get; set; } = 1;

        public virtual Transform Transform { get; private set; }
        public SvgShape Parent { get; private set; }
        //public SvgShape(XmlNode node) : this(node, null) { }
        public SvgShape(XmlNode node, SvgShape parent) : base(node)
        {
            Parent = parent;
            if (node != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                { Parse(attr); }
                if (Opacity != 1)
                {
                    if (m_fill != null)
                    {
                        m_fill.Opacity *= Opacity;
                    }
                    if (m_stroke != null)
                    {
                        m_stroke.Opacity *= Opacity;
                    }
                }
                if (parent != null && parent.Opacity != 1)
                {
                    if (m_fill != null)
                    {
                        m_fill.Opacity *= parent.Opacity;
                        m_fill.FillBrush = null;
                    }
                    if (m_stroke != null)
                    {
                        m_stroke.Opacity *= parent.Opacity;
                    }
                }
            }
        }
        public SvgShape(List<ShapeUtil.Attribute> attrs, SvgShape parent) : base(null)
        {
            Parent = parent;
            if (attrs != null)
            {
                foreach (ShapeUtil.Attribute attr in attrs)
                { Parse(attr); }
                if (Opacity != 1)
                {
                    if (m_fill != null)
                    {
                        m_fill.Opacity *= Opacity;
                    }
                    if (m_stroke != null)
                    {
                        m_stroke.Opacity *= Opacity;
                    }
                }
            }
        }
        protected virtual void Parse(XmlAttribute attr)
        {
            string name = attr.Name;
            string value = attr.Value;
            Parse(name, value);
        }
        protected virtual void Parse(ShapeUtil.Attribute attr)
        {
            string name = attr.Name;
            string value = attr.Value;
            Parse(name, value);
        }
        protected virtual void Parse(string name, string value)
        {
            if (name == SVGTags.sTransform)
            {
                Transform = ShapeUtil.ParseTransform(value.ToLower());
                return;
            }
            if (name == SVGTags.sStroke)
            {
                GetStroke().Color = SVG.PaintServers.Parse(value);
                return;
            }
            if (name == SVGTags.sOpacity)
            {
                Opacity = (float)XmlUtil.ParseDouble(value);
                if (Opacity > 1)
                {
                    Opacity = Opacity / 100;
                }
            }
            if (name == SVGTags.sStrokeWidth)
            {
                GetStroke().Width = (float)XmlUtil.ParseDouble(value);
                return;
            }
            if (name == SVGTags.sStrokeOpacity)
            {
                GetStroke().Opacity = XmlUtil.ParseDouble(value) * 100;
                return;
            }
            if (name == SVGTags.sStrokeDashArray)
            {
                if (value == "none")
                {
                    GetStroke().StrokeArray = null;
                    return;
                }
                ShapeUtil.StringSplitter sp = new ShapeUtil.StringSplitter(value);
                List<float> a = new List<float>();
                while (sp.More)
                {
                    a.Add((float)sp.ReadNextValue());
                }
                GetStroke().StrokeArray = a.ToArray();
                return;
            }
            if (name == SVGTags.sStrokeLinecap)
            {
                GetStroke().LineCap = (Stroke.eLineCap)Enum.Parse(typeof(Stroke.eLineCap), value);
                return;
            }
            if (name == SVGTags.sStrokeLinejoin)
            {
                GetStroke().LineJoin = (Stroke.eLineJoin)Enum.Parse(typeof(Stroke.eLineJoin), value);
                return;
            }
            if (name == SVGTags.sFill)
            {
                GetFill().Color = SVG.PaintServers.Parse(value);
                return;
            }
            if (name == SVGTags.sFillOpacity)
            {
                GetFill().Opacity = XmlUtil.ParseDouble(value) * 100;
                return;
            }
            if (name == SVGTags.sFillRule)
            {
                GetFill().FillRule = (FillRule)Enum.Parse(typeof(FillRule), value, true);
                return;
            }
            if (name == SVGTags.sStyle)
            {
                foreach (ShapeUtil.Attribute item in XmlUtil.SplitStyle(value))
                    Parse(item);
            }
            //********************** text *******************
            //if (name == SVGTags.sFontFamily)
            //{
            //    GetTextStyle().FontFamily = value;
            //    return;
            //}
            //if (name == SVGTags.sFontSize)
            //{
            //    GetTextStyle().FontSize = XmlUtil.AttrValue(new ShapeUtil.Attribute(name, value));
            //    return;
            //}
            ////if (name == SVGTags.sFontWeight)
            ////{
            ////    GetTextStyle().Fontweight = (FontWeight)new FontWeightConverter().ConvertFromString(value);
            ////    return;
            ////}
            //if (name == SVGTags.sFontStyle)
            //{
            //    FontStyles fontStyles = FontStyles.Regular;
            //    if (value.ToLower().Trim() != "normal")
            //    {
            //        fontStyles = FontStyles.Italic;
            //    }
            //    GetTextStyle().Fontstyle = fontStyles;
            //    return;
            //}
            //if (name == SVGTags.sTextDecoration)
            //{
            //    TextDecoration t = new TextDecoration();
            //    if (value == "none")
            //        return;
            //    if (value == "underline")
            //        t.Location = TextDecorationLocation.Underline;
            //    if (value == "overline")
            //        t.Location = TextDecorationLocation.OverLine;
            //    if (value == "line-through")
            //        t.Location = TextDecorationLocation.Strikethrough;
            //    //TextDecorationCollection tt = new TextDecorationCollection();
            //    //tt.Add(t);
            //    GetTextStyle().TextDecoration = t;
            //    return;
            //}
            //if (name == SVGTags.sTextAnchor)
            //{
            //    if (value == "start")
            //        GetTextStyle().TextAlignment = TextAlignment.Left;
            //    if (value == "middle")
            //        GetTextStyle().TextAlignment = TextAlignment.Center;
            //    if (value == "end")
            //        GetTextStyle().TextAlignment = TextAlignment.Right;
            //    return;
            //}
            //if (name == "word-spacing")
            //{
            //    GetTextStyle().WordSpacing = XmlUtil.AttrValue(new ShapeUtil.Attribute(name, value));
            //    return;
            //}
            //if (name == "letter-spacing")
            //{
            //    GetTextStyle().LetterSpacing = XmlUtil.AttrValue(new ShapeUtil.Attribute(name, value));
            //    return;
            //}
            //if (name == "baseline-shift")
            //{
            //    //GetTextStyle().BaseLineShift = XmlUtil.AttrValue(new ShapeUtil.Attribute(name, value));
            //    GetTextStyle().BaseLineShift = value;
            //    return;
            //}
        }
        Stroke GetStroke()
        {
            if (m_stroke == null)
                m_stroke = new Stroke();
            return m_stroke;
        }
        public SvgFill GetFill()
        {
            if (m_fill == null)
                m_fill = new SvgFill();
            return m_fill;
        }
        //protected TextStyle GetTextStyle()
        //{
        //    if (m_textstyle == null)
        //        m_textstyle = new TextStyle(this);
        //    return m_textstyle;
        //}
        PathGeometry path;
        public PathGeometry Geometry
        {
            get
            {
                if (path == null)
                {
                    path = CreateGeometry();
                    var fill = Fill;
                    if (fill != null)
                    {
                        path.FillRule = fill.FillRule;
                    }
                }
                return path;
            }
        }

        public abstract PathGeometry CreateGeometry();
    }
    class RectangleShape : SvgShape
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float RX { get; set; }
        public float RY { get; set; }
        public RectangleShape(XmlNode node, SvgShape parent) : base(node, parent)
        {
            X = (float)XmlUtil.AttrValue(node, "x", 0);
            Y = (float)XmlUtil.AttrValue(node, "y", 0);
            Width = (float)XmlUtil.AttrValue(node, "width", 0);
            Height = (float)XmlUtil.AttrValue(node, "height", 0);
            RX = (float)XmlUtil.AttrValue(node, "rx", 0);
            RY = (float)XmlUtil.AttrValue(node, "ry", 0);
        }

        public override PathGeometry CreateGeometry()
        {
            var path = new PathGeometry { };
            if (IsRounded(RX, RY))
            {
                var points = new Point[17];
                GetPointList(points, new Rect(X, Y, Width, Height), RX, RY);
                path.BeginFigure(points[0].X, points[0].Y);
                path.CubicTo(points[1], points[2], points[3]);
                path.LineTo(points[4].X, points[4].Y);
                path.CubicTo(points[5], points[6], points[7]);
                path.LineTo(points[8].X, points[8].Y);
                path.CubicTo(points[9], points[10], points[11]);
                path.LineTo(points[12].X, points[12].Y);
                path.CubicTo(points[13], points[14], points[15]);
            }
            else
            {
                var points = new Point[5];
                GetPointList(points, new Rect(X, Y, Width, Height), RX, RY);
                path.BeginFigure(points[0].X, points[0].Y);
                path.LineTo(points[1].X, points[1].Y);
                path.LineTo(points[2].X, points[2].Y);
                path.LineTo(points[3].X, points[3].Y);
                path.LineTo(points[4].X, points[4].Y);
            }
            path.EndFigure(true);
            return path;
        }
        private unsafe static void GetPointList(Point[] points, Rect rect, float radiusX, float radiusY)
        {
            if (IsRounded(radiusX, radiusY))
            {
                // It is a rounded rectangle
                //Invariant.Assert(pointsCount >= c_roundedPointCount);

                radiusX = (float)Math.Min(rect.Width * (1.0 / 2.0), Math.Abs(radiusX));
                radiusY = (float)Math.Min(rect.Height * (1.0 / 2.0), Math.Abs(radiusY));

                var bezierX = (float)((1f - c_arcAsBezier) * radiusX);
                var bezierY = (float)((1f - c_arcAsBezier) * radiusY);

                points[1].X = points[0].X = points[15].X = points[14].X = rect.X;
                points[2].X = points[13].X = rect.X + bezierX;
                points[3].X = points[12].X = rect.X + radiusX;
                points[4].X = points[11].X = rect.Right - radiusX;
                points[5].X = points[10].X = rect.Right - bezierX;
                points[6].X = points[7].X = points[8].X = points[9].X = rect.Right;

                points[2].Y = points[3].Y = points[4].Y = points[5].Y = rect.Y;
                points[1].Y = points[6].Y = rect.Y + bezierY;
                points[0].Y = points[7].Y = rect.Y + radiusY;
                points[15].Y = points[8].Y = rect.Bottom - radiusY;
                points[14].Y = points[9].Y = rect.Bottom - bezierY;
                points[13].Y = points[12].Y = points[11].Y = points[10].Y = rect.Bottom;

                points[16] = points[0];
            }
            else
            {
                // The rectangle is not rounded
                //Invariant.Assert(pointsCount >= c_squaredPointCount);

                points[0].X = points[3].X = points[4].X = rect.X;
                points[1].X = points[2].X = rect.Right;

                points[0].Y = points[1].Y = points[4].Y = rect.Y;
                points[2].Y = points[3].Y = rect.Bottom;
            }
        }
        internal static bool IsRounded(double radiusX, double radiusY)
        {
            return (radiusX != 0.0) && (radiusY != 0.0);
        }

    }
    class CircleShape : SvgShape
    {
        public float CX { get; set; }
        public float CY { get; set; }
        public float R { get; set; }
        public CircleShape(XmlNode node, SvgShape parent) : base(node, parent)
        {
            CX = (float)XmlUtil.AttrValue(node, "cx", 0);
            CY = (float)XmlUtil.AttrValue(node, "cy", 0);
            R = (float)XmlUtil.AttrValue(node, "r", 0);
        }

        public override PathGeometry CreateGeometry()
        {
            var path = new PathGeometry();
            var points = new Point[13];
            GetPointList(points, new Point(CX, CY), R, R);
            path.BeginFigure(points[0].X, points[0].Y);

            // i == 0, 3, 6, 9
            for (int i = 0; i < 12; i += 3)
            {
                path.CubicTo(points[i + 1], points[i + 2], points[i + 3]);
            }

            path.EndFigure(true);

            return path;
        }

        private unsafe static void GetPointList(Point[] points, Point center, float radiusX, float radiusY)
        {
            //Invariant.Assert(pointsCount >= c_pointCount);

            radiusX = Math.Abs(radiusX);
            radiusY = Math.Abs(radiusY);

            // Set the X coordinates
            var mid = (float)(radiusX * c_arcAsBezier);

            points[0].X = points[1].X = points[11].X = points[12].X = center.X + radiusX;
            points[2].X = points[10].X = center.X + mid;
            points[3].X = points[9].X = center.X;
            points[4].X = points[8].X = center.X - mid;
            points[5].X = points[6].X = points[7].X = center.X - radiusX;

            // Set the Y coordinates
            mid = (float)(radiusY * c_arcAsBezier);

            points[2].Y = points[3].Y = points[4].Y = center.Y + radiusY;
            points[1].Y = points[5].Y = center.Y + mid;
            points[0].Y = points[6].Y = points[12].Y = center.Y;
            points[7].Y = points[11].Y = center.Y - mid;
            points[8].Y = points[9].Y = points[10].Y = center.Y - radiusY;
        }
    }
    class EllipseShape : SvgShape
    {
        public float CX { get; set; }
        public float CY { get; set; }
        public float RX { get; set; }
        public float RY { get; set; }
        public EllipseShape(XmlNode node, SvgShape parent) : base(node, parent)
        {
            CX = (float)XmlUtil.AttrValue(node, "cx", 0);
            CY = (float)XmlUtil.AttrValue(node, "cy", 0);
            RX = (float)XmlUtil.AttrValue(node, "rx", 0);
            RY = (float)XmlUtil.AttrValue(node, "ry", 0);
        }
        public override PathGeometry CreateGeometry()
        {
            var path = new PathGeometry();
            var points = new Point[13];
            GetPointList(points, new Point(CX, CY), RX, RY);
            path.BeginFigure(points[0].X, points[0].Y);

            // i == 0, 3, 6, 9
            for (int i = 0; i < 12; i += 3)
            {
                path.CubicTo(points[i + 1], points[i + 2], points[i + 3]);
            }

            path.EndFigure(true);

            return path;
        }
        private unsafe static void GetPointList(Point[] points, Point center, float radiusX, float radiusY)
        {
            //Invariant.Assert(pointsCount >= c_pointCount);

            radiusX = Math.Abs(radiusX);
            radiusY = Math.Abs(radiusY);

            // Set the X coordinates
            var mid = (float)(radiusX * c_arcAsBezier);

            points[0].X = points[1].X = points[11].X = points[12].X = center.X + radiusX;
            points[2].X = points[10].X = center.X + mid;
            points[3].X = points[9].X = center.X;
            points[4].X = points[8].X = center.X - mid;
            points[5].X = points[6].X = points[7].X = center.X - radiusX;

            // Set the Y coordinates
            mid = (float)(radiusY * c_arcAsBezier);

            points[2].Y = points[3].Y = points[4].Y = center.Y + radiusY;
            points[1].Y = points[5].Y = center.Y + mid;
            points[0].Y = points[6].Y = points[12].Y = center.Y;
            points[7].Y = points[11].Y = center.Y - mid;
            points[8].Y = points[9].Y = points[10].Y = center.Y - radiusY;
        }
    }
    class LineShape : SvgShape
    {
        public Point P1 { get; private set; }
        public Point P2 { get; private set; }
        public LineShape(XmlNode node, SvgShape parent) : base(node, parent)
        {
            double x1 = XmlUtil.AttrValue(node, "x1", 0);
            double y1 = XmlUtil.AttrValue(node, "y1", 0);
            double x2 = XmlUtil.AttrValue(node, "x2", 0);
            double y2 = XmlUtil.AttrValue(node, "y2", 0);
            P1 = new Point((float)x1, (float)y1);
            P2 = new Point((float)x2, (float)y2);
        }

        public override PathGeometry CreateGeometry()
        {
            var path = new PathGeometry();
            path.BeginFigure(P1.X, P1.Y);
            path.LineTo(P2.X, P2.Y);
            path.EndFigure(false);
            return path;
        }
    }
    class PolylineShape : SvgShape
    {
        public Point[] Points { get; private set; }
        public PolylineShape(XmlNode node, SvgShape parent) : base(node, parent)
        {
            string points = XmlUtil.AttrValue(node, SVGTags.sPoints, string.Empty);
            ShapeUtil.StringSplitter split = new ShapeUtil.StringSplitter(points);
            List<Point> list = new List<Point>();
            while (split.More)
            {
                list.Add(split.ReadNextPoint());
            }
            Points = list.ToArray();
        }

        public override PathGeometry CreateGeometry()
        {
            var path = new PathGeometry();
            if (Points != null && Points.Length > 1)
            {
                path.BeginFigure(Points[0].X, Points[0].Y);
                for (int index = 1; index < Points.Length; index++)
                {
                    path.LineTo(Points[index].X, Points[index].Y);
                }
                path.EndFigure(false);
            }
            return path;
        }
    }
    class PolygonShape : SvgShape
    {
        public Point[] Points { get; private set; }
        public PolygonShape(XmlNode node, SvgShape parent) : base(node, parent)
        {
            string points = XmlUtil.AttrValue(node, SVGTags.sPoints, string.Empty);
            ShapeUtil.StringSplitter split = new ShapeUtil.StringSplitter(points);
            List<Point> list = new List<Point>();
            while (split.More)
            {
                list.Add(split.ReadNextPoint());
            }
            Points = list.ToArray();
        }

        public override PathGeometry CreateGeometry()
        {
            var path = new PathGeometry();
            if (Points != null && Points.Length > 1)
            {
                path.BeginFigure(Points[0].X, Points[0].Y);
                for (int index = 1; index < Points.Length; index++)
                {
                    path.LineTo(Points[index].X, Points[index].Y);
                }
                path.EndFigure(true);
            }
            return path;
        }
    }
    class UseShape : SvgShape
    {
        public override SvgType SvgType => SvgType.Use;
        public double X { get; set; }
        public double Y { get; set; }
        public string hRef { get; set; }
        public UseShape(XmlNode node, SvgShape parent) : base(node, parent)
        {
            X = XmlUtil.AttrValue(node, "x", 0);
            Y = XmlUtil.AttrValue(node, "y", 0);
            hRef = XmlUtil.AttrValue(node, "xlink:href", string.Empty);
            if (hRef.StartsWith("#"))
                hRef = hRef.Substring(1);
        }

        public override PathGeometry CreateGeometry()
        {
            throw new NotImplementedException();
        }
    }
    //class ImageShape : Shape
    //{
    //    public double X { get; set; }
    //    public double Y { get; set; }
    //    public double Width { get; set; }
    //    public double Height { get; set; }
    //    public string ImageSource { get; private set; }
    //    public ImageShape(XmlNode node) : base(node)
    //    {
    //        X = XmlUtil.AttrValue(node, "x", 0);
    //        Y = XmlUtil.AttrValue(node, "y", 0);
    //        Width = XmlUtil.AttrValue(node, "width", 0);
    //        Height = XmlUtil.AttrValue(node, "height", 0);
    //        string hRef = XmlUtil.AttrValue(node, "xlink:href", string.Empty);
    //        if (hRef.Length > 0)
    //        {
    //            // filename given must be relative to the location of the svg file
    //            //string svgpath = System.IO.Path.GetDirectoryName(svg.Filename);
    //            //string filename = System.IO.Path.Combine(svgpath, hRef);

    //            //BitmapImage b = new  BitmapImage();
    //            //b.BeginInit();
    //            //b.UriSource = new Uri(filename, UriKind.RelativeOrAbsolute);
    //            //b.EndInit();
    //            //ImageSource = b;
    //        }
    //    }
    //}
    class Group : SvgShape
    {
        public override SvgType SvgType => SvgType.Group;
        List<SvgShape> m_elements = new List<SvgShape>();
        public override IEnumerable<ISvgShape> Elements
        {
            get { return m_elements; }
        }

        //SvgShape AddChild(SvgShape shape)
        //{
        //    m_elements.Add(shape);
        //    shape.Parent = this;
        //    return shape;
        //}
        public Group(XmlNode node, SvgShape parent) : base(node, parent)
        {
            //// parent on group must be set before children are added
            //this.Parent = parent;
            foreach (XmlNode childnode in node.ChildNodes)
            {
                SvgShape shape = AddToList(m_elements, childnode, this);
                //if (shape != null)
                //    shape.Parent = this;
            }
            //if (Id.Length > 0)
            //	svg.AddShape(Id, this);
        }
        static public SvgShape AddToList(List<SvgShape> list, XmlNode childnode, SvgShape parent)
        {
            if (childnode.NodeType != XmlNodeType.Element)
                return null;
            if (childnode.Name == SVGTags.sShapeRect)
            {
                list.Add(new RectangleShape(childnode, parent));
                return list[list.Count - 1];
            }
            if (childnode.Name == SVGTags.sShapeCircle)
            {
                list.Add(new CircleShape(childnode, parent));
                return list[list.Count - 1];
            }
            if (childnode.Name == SVGTags.sShapeEllipse)
            {
                list.Add(new EllipseShape(childnode, parent));
                return list[list.Count - 1];
            }
            if (childnode.Name == SVGTags.sShapeLine)
            {
                list.Add(new LineShape(childnode, parent));
                return list[list.Count - 1];
            }
            if (childnode.Name == SVGTags.sShapePolyline)
            {
                list.Add(new PolylineShape(childnode, parent));
                return list[list.Count - 1];
            }
            if (childnode.Name == SVGTags.sShapePolygon)
            {
                list.Add(new PolygonShape(childnode, parent));
                return list[list.Count - 1];
            }
            if (childnode.Name == SVGTags.sShapePath)
            {
                list.Add(new PathShape(childnode, parent));
                return list[list.Count - 1];
            }
            if (childnode.Name == SVGTags.sShapeGroup)
            {
                list.Add(new Group(childnode, parent));
                return list[list.Count - 1];
            }
            if (childnode.Name == SVGTags.sLinearGradient)
            {
                SVG.PaintServers.Create(childnode);
                return null;
            }
            if (childnode.Name == SVGTags.sRadialGradient)
            {
                SVG.PaintServers.Create(childnode);
                return null;
            }
            if (childnode.Name == SVGTags.sDefinitions)
            {
                ReadDefs(list, childnode);
                return null;
            }
            if (childnode.Name == SVGTags.sShapeUse)
            {
                list.Add(new UseShape(childnode, parent));
                return list[list.Count - 1];
            }
            //if (childnode.Name == SVGTags.sShapeImage)
            //{
            //    list.Add(new ImageShape(childnode));
            //    return list[list.Count - 1];
            //}
            //if (childnode.Name == "text")
            //{
            //    list.Add(new TextShape(childnode, parent));
            //    return list[list.Count - 1];
            //}
            return null;
        }
        static void ReadDefs(List<SvgShape> list, XmlNode node)
        {
            list = new List<SvgShape>(); // temp list, not needed. 
                                         //ShapeGroups defined in the 'def' section is added the the 'Shapes' dictionary in SVG for later reference
            foreach (XmlNode childnode in node.ChildNodes)
            {
                if (childnode.Name == SVGTags.sLinearGradient)
                {
                    SVG.PaintServers.Create(childnode);
                    continue;
                }
                if (childnode.Name == SVGTags.sRadialGradient)
                {
                    SVG.PaintServers.Create(childnode);
                    continue;
                }
                AddToList(list, childnode, null);
            }
        }

        public override PathGeometry CreateGeometry()
        {
            throw new NotImplementedException();
        }
    }
}
