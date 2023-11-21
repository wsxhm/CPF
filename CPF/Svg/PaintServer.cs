using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF.Drawing;
using System.Reflection;
using System.Xml;

namespace CPF.Svg
{
    // http://www.w3.org/TR/SVGTiny12/painting.html#PaintServers
    abstract class PaintServer
    {
        public PaintServerManager Owner { get; private set; }
        public PaintServer(PaintServerManager owner)
        {
            Owner = owner;
        }
        public abstract ViewFill GetBrush(double opacity);
    }
    internal class PaintServerManager
    {
        static Dictionary<string, Color> m_knownColors = null;
        Dictionary<string, PaintServer> m_servers = new Dictionary<string, PaintServer>();
        public PaintServer Create(XmlNode node)
        {
            if (node.Name == SVGTags.sLinearGradient)
            {
                string id = XmlUtil.AttrValue(node, "id");
                if (m_servers.ContainsKey(id) == false)
                    m_servers[id] = new LinearGradientColor(this, node);
                return m_servers[id];
            }
            if (node.Name == SVGTags.sRadialGradient)
            {
                string id = XmlUtil.AttrValue(node, "id");
                if (m_servers.ContainsKey(id) == false)
                    m_servers[id] = new RadialGradientColor(this, node);
                return m_servers[id];
            }
            return null;
        }
        public PaintServer Parse(string value)
        {
            if (value == "none")
                return null;
            if (value[0] == '#')
                return ParseSolidColor(value);
            PaintServer result = null; ;
            if (m_servers.TryGetValue(value, out result))
                return result;
            if (value.StartsWith("url"))
            {
                string id = ShapeUtil.ExtractBetween(value, '(', ')');
                if (id.Length > 0 && id[0] == '#')
                    id = id.Substring(1);
                m_servers.TryGetValue(id, out result);
                return result;
            }
            return ParseKnownColor(value);
        }
        public static Color ParseHexColor(string value)
        {
            // format is #xxFF00FF where xx is optional (the a value)
            // if format ix #rgb then the values are replicated #rrggbb
            int start = 0;
            if (value[start] == '#')
                start++;

            uint u = Convert.ToUInt32(value.Substring(start), 16);
            if (value.Length <= 4)
            {
                uint newval = 0;
                newval |= (u & 0x000f00) << 12;
                newval |= (u & 0x000f00) << 8;
                newval |= (u & 0x0000f0) << 8;
                newval |= (u & 0x0000f0) << 4;
                newval |= (u & 0x00000f) << 4;
                newval |= (u & 0x00000f);
                u = newval;
            }
            byte a = (byte)((u & 0xff000000) >> 24);
            byte r = (byte)((u & 0x00ff0000) >> 16);
            byte g = (byte)((u & 0x0000ff00) >> 8);
            byte b = (byte)(u & 0x000000ff);
            if (a == 0)
                a = 255;
            return Color.FromArgb(a, r, g, b);
        }
        public static Color KnownColor(string value)
        {
            LoadKnownColors();
            if (m_knownColors.ContainsKey(value))
                return m_knownColors[value];
            return Color.Black;
        }
        SolidColor ParseSolidColor(string value)
        {
            string id = "_solid" + value;
            PaintServer result;
            if (m_servers.TryGetValue(id, out result))
                return result as SolidColor;
            result = new SolidColor(this, ParseHexColor(value));
            m_servers[id] = result;
            return result as SolidColor;
        }
        SolidColor ParseKnownColor(string value)
        {
            LoadKnownColors();
            PaintServer result;
            if (m_servers.TryGetValue(value, out result))
                return result as SolidColor;
            Color c;
            if (m_knownColors.TryGetValue(value, out c))
            {
                result = new SolidColor(this, c);
                m_servers[value] = result;
                return result as SolidColor;
            }
            return null;
        }
        static void LoadKnownColors()
        {
            if (m_knownColors == null)
                m_knownColors = new Dictionary<string, Color>();
            if (m_knownColors.Count == 0)
            {
                PropertyInfo[] propinfos = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
                foreach (PropertyInfo info in propinfos)
                {
                    if (info.PropertyType == typeof(Color))
                        m_knownColors[info.Name.ToLower()] = (Color)info.GetValue(typeof(Color), null);
                }
            }
        }
    }

    class SolidColor : PaintServer
    {
        public Color Color { get; set; }
        public SolidColor(PaintServerManager owner, Color c) : base(owner)
        {
            Color = c;
        }

        SolidColorFill sb;

        public override ViewFill GetBrush(double opacity)
        {
            byte a = (byte)(255 * opacity / 100);
            Color c = Color;
            Color newcol = Color.FromArgb(a, c.R, c.G, c.B);
            if (sb == null || sb.IsDisposed)
            {
                sb = new SolidColorFill();
            }
            sb.Color = newcol;
            return sb;
        }
    }
    abstract class GradientColor : PaintServer
    {
        // http://www.w3.org/TR/SVG11/pservers.html#LinearGradients
        List<GradientStop> m_stops = new List<GradientStop>();
        public IList<GradientStop> Stops
        {
            get { return m_stops.AsReadOnly(); }
        }
        public Transform Transform { get; protected set; }
        public string GradientUnits { get; private set; }

        public GradientColor(PaintServerManager owner, XmlNode node) : base(owner)
        {
            GradientUnits = XmlUtil.AttrValue(node, "gradientUnits", string.Empty);
            string transform = XmlUtil.AttrValue(node, "gradientTransform", string.Empty);
            if (transform.Length > 0)
            {
                Transform = ShapeUtil.ParseTransform(transform.ToLower());
            }

            if (node.ChildNodes.Count == 0 && XmlUtil.AttrValue(node, "xlink:href", string.Empty).Length > 0)
            {
                string refid = XmlUtil.AttrValue(node, "xlink:href", string.Empty);
                GradientColor refcol = owner.Parse(refid.Substring(1)) as GradientColor;
                if (refcol == null)
                    return;
                m_stops = new List<GradientStop>(refcol.m_stops);
            }
            foreach (XmlNode childnode in node.ChildNodes)
            {
                if (childnode.Name == "stop")
                {
                    List<XmlAttribute> styleattr = new List<XmlAttribute>();
                    string fullstyle = XmlUtil.AttrValue(childnode, SVGTags.sStyle, string.Empty);
                    if (fullstyle.Length > 0)
                    {
                        foreach (ShapeUtil.Attribute styleitem in XmlUtil.SplitStyle(fullstyle))
                            styleattr.Add(new XmlUtil.StyleItem(childnode, styleitem.Name, styleitem.Value));
                    }
                    foreach (XmlAttribute attr1 in styleattr)
                        childnode.Attributes.Append(attr1);


                    double offset = XmlUtil.AttrValue(childnode, "offset", 0);
                    string s = XmlUtil.AttrValue(childnode, "stop-color", "#0");

                    double stopopacity = XmlUtil.AttrValue(childnode, "stop-opacity", 1);

                    Color color;
                    if (s.StartsWith("#"))
                        color = PaintServerManager.ParseHexColor(s);
                    else
                        color = PaintServerManager.KnownColor(s);

                    if (stopopacity != 1)
                        color = Color.FromArgb((byte)(stopopacity * 255), color.R, color.G, color.B);

                    if (offset > 1)
                        offset = offset / 100;
                    m_stops.Add(new GradientStop(color, (float)offset));
                }
            }
        }
    }

    class LinearGradientColor : GradientColor
    {
        public double X1 { get; private set; }
        public double Y1 { get; private set; }
        public double X2 { get; private set; }
        public double Y2 { get; private set; }
        public string Id { get; private set; }

        public LinearGradientColor(PaintServerManager owner, XmlNode node) : base(owner, node)
        {
            System.Diagnostics.Debug.Assert(node.Name == SVGTags.sLinearGradient);
            Id = XmlUtil.AttrValue(node, "id");
            X1 = XmlUtil.AttrValue(node, "x1", double.NaN);
            Y1 = XmlUtil.AttrValue(node, "y1", double.NaN);
            X2 = XmlUtil.AttrValue(node, "x2", double.NaN);
            Y2 = XmlUtil.AttrValue(node, "y2", double.NaN);
        }
        public override ViewFill GetBrush(double opacity)
        {
            LinearGradientFill b = new LinearGradientFill();
            foreach (GradientStop stop in Stops)
                b.GradientStops.Add(stop);

            //b.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            b.StartPoint = "0, 0";
            b.EndPoint = "100%, 0";

            if (GradientUnits == SVGTags.sGradientUserSpace)
            {
                b.StartPoint = new Point((float)X1, (float)Y1);
                b.EndPoint = new Point((float)X2, (float)Y2);
                //b.MappingMode = BrushMappingMode.Absolute;
            }
            else
            {
                Normalize();
                if (double.IsNaN(X1) == false)
                    b.StartPoint = new PointField(new FloatField((float)X1, Unit.Percent), new FloatField((float)Y1, Unit.Percent));
                if (double.IsNaN(X2) == false)
                    b.EndPoint = new PointField(new FloatField((float)X2, Unit.Percent), new FloatField((float)Y2, Unit.Percent));
            }
            if (Transform != null)
            {
                b.Matrix = Transform.Value;
            }

            return b;
        }
        void Normalize()
        {
            // This is until proper 'userspace' is supported.
            // crude normalization of the transition points.
            // gradient transition line is alwaysfrom 0 to 1
            if (double.IsNaN(X1) == false && double.IsNaN(X2) == false)
            {
                double min = X1;
                if (X2 < X1)
                    min = X2;
                X1 -= min;
                X2 -= min;
                double scale = X1;
                if (X2 > X1)
                    scale = X2;
                if (scale != 0)
                {
                    X1 /= scale;
                    X2 /= scale;
                }
            }
            if (double.IsNaN(Y1) == false && double.IsNaN(Y2) == false)
            {
                double min = Y1;
                if (Y2 < Y1)
                    min = Y2;
                Y1 -= min;
                Y2 -= min;
                double scale = Y1;
                if (Y2 > Y1)
                    scale = Y2;
                if (scale != 0)
                {
                    Y1 /= scale;
                    Y2 /= scale;
                }
            }
        }
    }

    class RadialGradientColor : GradientColor
    {
        public double CX { get; private set; }
        public double CY { get; private set; }
        public double FX { get; private set; }
        public double FY { get; private set; }
        public double R { get; private set; }
        public string Id { get; private set; }

        public RadialGradientColor(PaintServerManager owner, XmlNode node) : base(owner, node)
        {
            System.Diagnostics.Debug.Assert(node.Name == SVGTags.sRadialGradient);
            Id = XmlUtil.AttrValue(node, "id");

            CX = XmlUtil.AttrValue(node, "cx", double.NaN);
            CY = XmlUtil.AttrValue(node, "cy", double.NaN);
            FX = XmlUtil.AttrValue(node, "fx", double.NaN);
            FY = XmlUtil.AttrValue(node, "fy", double.NaN);
            R = XmlUtil.AttrValue(node, "r", double.NaN);
            Normalize();
        }
        public override ViewFill GetBrush(double opacity)
        {
            RadialGradientFill b = new RadialGradientFill();
            foreach (GradientStop stop in Stops)
                b.GradientStops.Add(stop);

            //b.GradientOrigin = new System.Windows.Point(0.5, 0.5);
            b.Center = "50%,50%";
            b.Radius = "50%";
            //b.RadiusY = 0.5;

            if (GradientUnits == SVGTags.sGradientUserSpace)
            {
                b.Center = new Point((float)CX, (float)CY);
                //b.GradientOrigin = new System.Windows.Point(FX, FY);
                b.Radius = R;
                //b.RadiusY = R;
                //b.MappingMode = BrushMappingMode.Absolute;
            }
            else
            {
                float scale = 1f / 100f;
                if (double.IsNaN(CX) == false && double.IsNaN(CY) == false)
                {
                    //b.GradientOrigin = new System.Windows.Point(CX * scale, CY * scale);
                    //b.Center = new System.Windows.Point(CX * scale, CY * scale);
                    b.Center = new PointField(new FloatField((float)CX * scale, Unit.Percent), new FloatField((float)CY * scale, Unit.Percent));
                }
                //if (double.IsNaN(FX) == false && double.IsNaN(FY) == false)
                //{
                //    b.GradientOrigin = new System.Windows.Point(FX * scale, FY * scale);
                //}
                if (double.IsNaN(R) == false)
                {
                    b.Radius = new FloatField((float)R * scale, Unit.Percent);
                    //b.RadiusY = R * scale;
                }
                //b.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            }
            if (Transform != null)
                b.Matrix = Transform.Value;
            return b;
        }
        void Normalize()
        {
        }
    }
}
