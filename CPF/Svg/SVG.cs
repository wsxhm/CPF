using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using CPF.Styling;
using System.Xml;
using CPF.Controls;
using System.ComponentModel;
using System.Linq;

namespace CPF.Svg
{
    /// <summary>
    /// 支持显示SVG图形，暂时不支持里面的滤镜，动画，图片引用，文字等，只能显示简单的图形
    /// </summary>
    [DefaultProperty(nameof(Source))]
    public class SVG : UIElement
    {
        /// <summary>
        /// 支持显示SVG图形，暂时不支持里面的滤镜，动画，图片引用，文字等，只能显示简单的图形
        /// </summary>
        public SVG()
        {

        }
        /// <summary>
        /// 可以是svg文件路径，或者直接是svg文档
        /// </summary>
        /// <param name="svgSource"></param>
        public SVG(string svgSource)
        {
            Source = svgSource;
        }
        /// <summary>
        /// svg文档
        /// </summary>
        [UIPropertyMetadata("", UIPropertyOptions.AffectsMeasure | UIPropertyOptions.AffectsRender)]
        public string Data
        {
            get { return GetValue<string>(); }
            private set { SetValue(value); }
        }

        /// <summary>
        /// SVG源，可以是路径、Url、或者svg文档字符串
        /// </summary>
        [Description("SVG源，可以是路径、Url、或者svg文档字符串")]
        [PropertyMetadata(null), CPF.Design.FileBrowser(".svg")]
        public string Source
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        [PropertyChanged(nameof(Source))]
        void OnSourceChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var svg = newValue as string;
            if (!string.IsNullOrWhiteSpace(svg))
            {
                svg = svg.Trim();
                if (svg.StartsWith("<") && svg.EndsWith(">"))
                {
                    Data = svg;
                }
                else
                {
                    ResourceManager.GetText(svg, a =>
                    {
                        Invoke(() =>
                        {
                            Data = a;
                        });
                    });
                }
            }
        }
        [PropertyChanged(nameof(Data))]
        void OnDataChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            m_elements.Clear();
            if (newValue is string str)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.XmlResolver = null;
                    doc.LoadXml(str);
                    XmlNode n = doc.GetElementsByTagName("svg")[0];
                    this.Parse(n);


                    //bool success = false;
                    //canvasSize = GetCanvas(true, false, out success);
                    //if (!success)
                    //    canvasSize = GetCanvas(true, true, out success);
                    //if (!success)
                    //    canvasSize = GetCanvas(false, false, out success);
                    //if (!success)
                    //    canvasSize = GetCanvas(false, true, out success);
                    bool realSuccess = false;
                    realCanvasSize = GetCanvas(false, false, out realSuccess);
                    if (!realSuccess)
                        realCanvasSize = GetCanvas(false, true, out realSuccess);

                    bool viewBoxSuccess = false;
                    viewBoxCanvasSize = GetCanvas(true, false, out viewBoxSuccess);
                    if (!viewBoxSuccess)
                        viewBoxCanvasSize = GetCanvas(true, true, out viewBoxSuccess);

                    canvasSize = realSuccess ? viewBoxCanvasSize : realCanvasSize;
                }
                catch (Exception e)
                {
                    throw new Exception("svg格式不对", e);
                }
            }
        }

        SizeField viewBoxCanvasSize;
        SizeField realCanvasSize;

        SizeField canvasSize;

        SizeField GetCanvas(bool viewBox, bool px, out bool success)
        {
            try
            {
                string width = "0"; //宽 
                string height = "0"; //高 
                string pattern = viewBox ? (px ? "viewBox\\s?=\\s?\"(\\d+px)\\s(\\d+px)\\s(\\d+px)\\s(\\d+px)\"" : "viewBox\\s?=\\s?\"(\\d+)\\s(\\d+)\\s(\\d+)\\s(\\d+)\"") :
                    (px ? "width\\s?=\\s?\"(\\d+px)\"\\s+height\\s?=\\s?\"(\\d+px)" : "width\\s?=\\s?\"(\\d+)\"\\s+height\\s?=\\s?\"(\\d+)");
                string str = Data;

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Match m = regex.Match(str);
                if (m.Success)
                {
                    if (viewBox)
                    {
                        width = m.Groups[3].Value.Replace("px", ""); //宽 
                        height = m.Groups[4].Value.Replace("px", ""); //高 
                    }
                    else
                    {
                        width = m.Groups[1].Value.Replace("px", ""); //宽 
                        height = m.Groups[2].Value.Replace("px", ""); //高 
                    }
                    success = true;
                }
                else
                    success = false;
                return new SizeField()
                {
                    Width = float.Parse(width),
                    Height = float.Parse(height)
                };
            }
            catch
            {
                success = false;
                return new SizeField();
            }
        }


        static PaintServerManager m_paintServers = new PaintServerManager();

        // for "use" shape only.
        Dictionary<string, SvgShape> m_shapes = new Dictionary<string, SvgShape>();
        //internal void AddShape(string id, SvgShape shape)
        //{
        //    //System.Diagnostics.Debug.Assert(id.Length > 0 && m_shapes.ContainsKey(id) == false);
        //    m_shapes[id] = shape;
        //}
        /// <summary>
        /// 通过ID获取Shape
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ISvgShape GetShape(string id)
        {
            //SvgShape shape = null;
            m_shapes.TryGetValue(id, out var shape);
            return shape;
        }
        internal static PaintServerManager PaintServers
        {
            get { return m_paintServers; }
        }
        List<SvgShape> m_elements = new List<SvgShape>();
        /// <summary>
        /// 解析到的元素
        /// </summary>
        [NotCpfProperty, Browsable(false)]
        public IEnumerable<ISvgShape> Elements
        {
            get { return m_elements.Select(a => a as ISvgShape); }
        }
        void Parse(XmlNode node)
        {
            if (node == null || node.Name != "svg")
                throw new FormatException("Not a valide SVG node");
            foreach (XmlNode childnode in node.ChildNodes)
                Group.AddToList(/*this, */m_elements, childnode, null);

            var fill = Fill;
            foreach (var item in m_elements)
            {
                item.GetFill().FillBrush = fill;
            }
        }

        /// <summary>
        /// 图片缩放模式
        /// </summary>
        [Description("图片缩放模式")]
        [UIPropertyMetadata(Stretch.None, UIPropertyOptions.AffectsMeasure)]
        public Stretch Stretch
        {
            get { return GetValue<Stretch>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 描述如何对内容应用缩放，并限制对已命名像素类型的缩放。
        /// </summary>
        [Description("描述如何对内容应用缩放，并限制对已命名像素类型的缩放。")]
        [UIPropertyMetadata(StretchDirection.Both, UIPropertyOptions.AffectsMeasure)]
        public StretchDirection StretchDirection
        {
            get { return GetValue<StretchDirection>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 图形的默认填充
        /// </summary>
        [UIPropertyMetadata(typeof(ViewFill), "#000", UIPropertyOptions.AffectsRender)]
        public ViewFill Fill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }
        [PropertyChanged(nameof(Fill))]
        void OnFillChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var fill = newValue as ViewFill;
            foreach (var item in m_elements)
            {
                item.GetFill().FillBrush = fill;
            }
        }

        Size naturalSize;
        protected override Size MeasureOverride(in Size availableSize)
        {
            var b = base.MeasureOverride(availableSize);
            var list = m_elements;
            if (list.Count > 0)
            {
                if (canvasSize.Width.Value > 0 && canvasSize.Height.Value > 0)
                {
                    naturalSize = new Size(canvasSize.Width.Value, canvasSize.Height.Value);
                }
                else
                {
                    foreach (var item in list)
                    {
                        var s = Measure(item);
                        b = new Size(Math.Max(s.Width, b.Width), Math.Max(s.Height, b.Height));
                    }
                    naturalSize = new Size(b.Width, b.Height);
                }

                var size = availableSize;
                if (realCanvasSize.Width.Value > 0 && realCanvasSize.Width.Value < viewBoxCanvasSize.Width.Value &&
                   realCanvasSize.Height.Value > 0 && realCanvasSize.Height.Value < viewBoxCanvasSize.Height.Value)
                {
                    size = new Size(realCanvasSize.Width.Value, realCanvasSize.Height.Value);
                }

                var maxW = MaxWidth;
                if (!maxW.IsAuto && maxW.Unit == Unit.Default)
                {
                    size.Width = maxW.Value;
                }
                var maxH = MaxHeight;
                if (!maxH.IsAuto && maxH.Unit == Unit.Default)
                {
                    size.Height = maxH.Value;
                }
                Size scaleFactor = ComputeScaleFactor(size,
                                                         naturalSize,
                                                         this.Stretch, StretchDirection);
                Size sizeField = new Size(naturalSize.Width * scaleFactor.Width, naturalSize.Height * scaleFactor.Height);
                //Console.WriteLine("newSize:" + naturalSize.Width + "," + naturalSize.Height);
                // Returns our minimum size & sets DesiredSize.
                return sizeField;
            }
            return b;
        }

        internal static Size ComputeScaleFactor(Size availableSize,
                                          Size contentSize,
                                          Stretch stretch, StretchDirection stretchDirection)
        {
            // Compute scaling factors to use for axes
            float scaleX = 1.0f;
            float scaleY = 1.0f;

            bool isConstrainedWidth = !float.IsPositiveInfinity(availableSize.Width);
            bool isConstrainedHeight = !float.IsPositiveInfinity(availableSize.Height);

            if ((stretch == Stretch.Uniform || stretch == Stretch.UniformToFill || stretch == Stretch.Fill)
                 && (isConstrainedWidth || isConstrainedHeight))
            {
                // Compute scaling factors for both axes
                scaleX = (FloatUtil.IsZero(contentSize.Width)) ? 0f : availableSize.Width / contentSize.Width;
                scaleY = (FloatUtil.IsZero(contentSize.Height)) ? 0f : availableSize.Height / contentSize.Height;

                if (!isConstrainedWidth) scaleX = scaleY;
                else if (!isConstrainedHeight) scaleY = scaleX;
                else
                {
                    // If not preserving aspect ratio, then just apply transform to fit
                    switch (stretch)
                    {
                        case Stretch.Uniform:       //Find minimum scale that we use for both axes
                            float minscale = scaleX < scaleY ? scaleX : scaleY;
                            scaleX = scaleY = minscale;
                            break;

                        case Stretch.UniformToFill: //Find maximum scale that we use for both axes
                            float maxscale = scaleX > scaleY ? scaleX : scaleY;
                            scaleX = scaleY = maxscale;
                            break;

                        case Stretch.Fill:          //We already computed the fill scale factors above, so just use them
                            break;
                    }
                }

                //Apply stretch direction by bounding scales.
                //In the uniform case, scaleX=scaleY, so this sort of clamping will maintain aspect ratio
                //In the uniform fill case, we have the same result too.
                //In the fill case, note that we change aspect ratio, but that is okay
                switch (stretchDirection)
                {
                    case StretchDirection.UpOnly:
                        if (scaleX < 1.0) scaleX = 1f;
                        if (scaleY < 1.0) scaleY = 1f;
                        break;

                    case StretchDirection.DownOnly:
                        if (scaleX > 1.0) scaleX = 1f;
                        if (scaleY > 1.0) scaleY = 1f;
                        break;

                    case StretchDirection.Both:
                        break;

                    default:
                        break;
                }
            }
            //Return this as a size now
            return new Size(scaleX, scaleY);
        }



        protected override void OnRender(DrawingContext dc)
        {
            if (m_elements.Count > 0)
            {
                var size = ActualSize;
                var w = naturalSize.Width;
                var h = naturalSize.Height;
                var x = 0f;
                var y = 0f;
                var sw = 1f;
                var sh = 1f;
                switch (this.Stretch)
                {
                    case Stretch.None:
                        //dc.DrawImage(img, new Rect((size.Width - w) / 2, (size.Height - h) / 2, w, h), new Rect(0, 0, w, h));
                        x = (size.Width - w) / 2;
                        y = (size.Height - h) / 2;
                        break;
                    case Stretch.Fill:
                        //dc.DrawImage(img, new Rect(0, 0, size.Width, size.Height), new Rect(0, 0, w, h));
                        sw = size.Width / w;
                        sh = size.Height / h;
                        break;
                    case Stretch.Uniform:
                        var ww = size.Width;
                        var hh = size.Height;
                        if (w / size.Width > h / size.Height)
                        {
                            hh = size.Width * h / w;
                        }
                        else
                        {
                            ww = size.Height * w / h;
                        }
                        x = (size.Width - ww) / 2;
                        y = (size.Height - hh) / 2;
                        sw = ww / w;
                        sh = hh / h;
                        //dc.DrawImage(img, new Rect((size.Width - ww) / 2, (size.Height - hh) / 2, ww, hh), new Rect(0, 0, w, h));
                        break;
                    case Stretch.UniformToFill:
                        var www = size.Width;
                        var hhh = size.Height;
                        if (w / size.Width < h / size.Height)
                        {
                            hhh = size.Width * h / w;
                        }
                        else
                        {
                            www = size.Height * w / h;
                        }
                        x = (size.Width - www) / 2;
                        y = (size.Height - hhh) / 2;
                        sw = www / w;
                        sh = hhh / h;
                        //dc.DrawImage(img, new Rect((size.Width - www) / 2, (size.Height - hhh) / 2, www, hhh), new Rect(0, 0, w, h));
                        break;
                }
                var old = dc.Transform;
                var tran = old;
                tran.TranslatePrepend(x, y);
                tran.ScalePrepend(sw, sh);
                dc.Transform = tran;
                foreach (var item in m_elements)
                {
                    DrawShape(dc, item);
                }
                dc.Transform = old;
            }
        }

        private void DrawShape(DrawingContext dc, SvgShape item)
        {
            var old = dc.Transform;
            if (item.Transform != null)
            {
                var tran = old;
                tran.Prepend(item.Transform.Value);
                dc.Transform = tran;
            }

            if (item is Group group)
            {
                foreach (SvgShape shape in group.Elements)
                {
                    DrawShape(dc, shape);
                }
            }
            else if (item is UseShape use)
            {
                var shape = GetShape(use.hRef);
                if (shape != null)
                {
                    DrawShape(dc, (SvgShape)shape);
                }
            }
            else
            {
                var stroke = item.Stroke;
                if (stroke != null && stroke.Width > 0 && stroke.StrokeBrush != null)
                {
                    using (var brush = stroke.StrokeBrush.CreateBrush(item.Geometry.GetBounds(), Root.RenderScaling))
                    {
                        var stro = new Drawing.Stroke((float)stroke.Width, DashStyles.Solid);
                        if (stroke.StrokeArray != null)
                        {
                            stro.DashStyle = DashStyles.Custom;
                            stro.DashPattern = stroke.StrokeArray;
                            switch (stroke.LineJoin)
                            {
                                case Stroke.eLineJoin.miter:
                                    stro.LineJoin = LineJoins.Miter;
                                    break;
                                case Stroke.eLineJoin.round:
                                    stro.LineJoin = LineJoins.Round;
                                    break;
                                case Stroke.eLineJoin.bevel:
                                    stro.LineJoin = LineJoins.Bevel;
                                    break;
                            }
                            switch (stroke.LineCap)
                            {
                                case Stroke.eLineCap.butt:
                                    stro.StrokeCap = CapStyles.Flat;
                                    break;
                                case Stroke.eLineCap.round:
                                    stro.StrokeCap = CapStyles.Round;
                                    break;
                                case Stroke.eLineCap.square:
                                    stro.StrokeCap = CapStyles.Square;
                                    break;
                            }
                        }
                        dc.DrawPath(brush, stro, item.Geometry);
                    }
                }
                if (item.Fill != null && item.Fill.FillBrush != null)
                {
                    using (var brush = item.Fill.FillBrush.CreateBrush(item.Geometry.GetBounds(), Root.RenderScaling))
                    {
                        dc.FillPath(brush, item.Geometry);
                    }
                }
            }
            if (item.Transform != null)
            {
                dc.Transform = old;
            }
        }

        private Size Measure(SvgShape item)
        {
            if (item is Group group)
            {
                Size b = new Size();
                foreach (SvgShape shape in group.Elements)
                {
                    var s = Measure(shape);
                    b = new Size(Math.Max(s.Width, b.Width), Math.Max(s.Height, b.Height));
                }
                return b;
            }
            else if (item is UseShape useShape)
            {
                var shape = GetShape(useShape.hRef);
                if (shape != null)
                {
                    return Measure((SvgShape)shape);
                }
                return new Size();
            }
            else
            {
                var bound = item.Geometry.GetBounds();
                return new Size(bound.Right, bound.Bottom);
            }
        }
    }
}
