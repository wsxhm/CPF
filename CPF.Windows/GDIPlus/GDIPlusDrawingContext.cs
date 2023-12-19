#if Net4
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using CPF.Drawing;

namespace CPF.GDIPlus
{
    public class GDIPlusDrawingContext : DrawingContext
    {
        Graphics g;

        public Graphics Graphics
        {
            get { return g; }
        }
        IntPtr hdc = IntPtr.Zero;
        StringFormat GenericTypographic = (StringFormat)StringFormat.GenericTypographic.Clone();
        StringFormat CenterFormat = new StringFormat() { Alignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit };
        StringFormat RightFormat = new StringFormat { Alignment = StringAlignment.Far, FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit };

        Pen pen;
        public GDIPlusDrawingContext(Graphics g)
        {
            this.g = g;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            GenericTypographic.Trimming = StringTrimming.Character;
            CenterFormat.Trimming = StringTrimming.Character;
            RightFormat.Trimming = StringTrimming.Character;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
        }

        public GDIPlusDrawingContext(IntPtr hdc, Rect rect)
        {
            this.hdc = hdc;
            g = Graphics.FromHdc(hdc);
            g.SetClip(new RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height));
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            GenericTypographic.Trimming = StringTrimming.Character;
            CenterFormat.Trimming = StringTrimming.Character;
            RightFormat.Trimming = StringTrimming.Character;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
        }

        public GDIPlusDrawingContext(IntPtr hwnd)
        {
            this.g = Graphics.FromHwnd(hwnd);
            GenericTypographic.Trimming = StringTrimming.Character;
            CenterFormat.Trimming = StringTrimming.Character;
            RightFormat.Trimming = StringTrimming.Character;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
        }

        public override AntialiasMode AntialiasMode
        {
            get
            {
                if (g.SmoothingMode == System.Drawing.Drawing2D.SmoothingMode.Default || g.SmoothingMode == System.Drawing.Drawing2D.SmoothingMode.None)
                {
                    return AntialiasMode.Default;
                }
                return AntialiasMode.AntiAlias;
            }

            set
            {
                if (value == AntialiasMode.Default)
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

                    if (drawingFactory.ClearType)
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    }
                    else
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                    }
                }
                else
                {
                    if (drawingFactory.ClearType)
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    }
                    else
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    }
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                }
            }
        }

        private void SetPen(Stroke stroke, System.Drawing.Brush strokeBrush)
        {
            if (pen == null)
            {
                pen = new Pen(strokeBrush);
            }
            pen.Width = stroke.Width;
            pen.DashStyle = stroke.DashStyle.ToGdiDashStyle();
            if (stroke.DashStyle != DashStyles.Solid)
            {
                pen.DashOffset = stroke.DashOffset;
                pen.DashPattern = stroke.GetDashPattern();
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            }
            pen.StartCap = stroke.StrokeCap.ToLineCap();
            pen.EndCap = pen.StartCap;
            pen.Brush = strokeBrush;
        }
        private Pen CreatePen(Stroke stroke, System.Drawing.Brush strokeBrush)
        {
            var pen = new Pen(strokeBrush);
            pen.Width = stroke.Width;
            pen.DashStyle = stroke.DashStyle.ToGdiDashStyle();
            if (stroke.DashStyle != DashStyles.Solid)
            {
                pen.DashOffset = stroke.DashOffset;
                pen.DashPattern = stroke.GetDashPattern();
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            }
            pen.StartCap = stroke.StrokeCap.ToLineCap();
            pen.EndCap = pen.StartCap;
            pen.Brush = strokeBrush;
            return pen;
        }

        public override void DrawLine(in Stroke stroke, CPF.Drawing.Brush strokeBrush, in CPF.Drawing.Point point1, in CPF.Drawing.Point point2)
        {
            InitializeBrush(strokeBrush);
            SetPen(stroke, (System.Drawing.Brush)strokeBrush.AdapterBrush);
            g.DrawLine(pen, point1.ToGdiPoint(), point2.ToGdiPoint());
        }

        public override void DrawRectangle(CPF.Drawing.Brush strokeBrush, in CPF.Drawing.Stroke stroke, in CPF.Drawing.Rect rect)
        {
            InitializeBrush(strokeBrush);
            SetPen(stroke, (System.Drawing.Brush)strokeBrush.AdapterBrush);
            g.DrawRectangle(pen, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
        }

        public override void FillRectangle(CPF.Drawing.Brush fillBrush, Rect rect)
        {
            InitializeBrush(fillBrush);
            g.FillRectangle((System.Drawing.Brush)fillBrush.AdapterBrush, rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
        }

        //public override void DrawRoundedRectangle(Brush strokeBrush, Stroke stroke, Brush fillBrush, Rect rectangle, double radiusX, double radiusY)
        //{
        //    throw new NotImplementedException();
        //}

        public override void DrawEllipse(CPF.Drawing.Brush strokeBrush, in Stroke stroke, in CPF.Drawing.Point center, in float radiusX, in float radiusY)
        {
            InitializeBrush(strokeBrush);
            SetPen(stroke, (System.Drawing.Brush)strokeBrush.AdapterBrush);
            var x = Math.Abs(radiusX);
            var y = Math.Abs(radiusY);
            g.DrawEllipse(pen, center.X - x, center.Y - y, x * 2, y * 2);
        }

        public override void FillEllipse(CPF.Drawing.Brush fillBrush, in CPF.Drawing.Point center, in float radiusX, in float radiusY)
        {
            InitializeBrush(fillBrush);
            var x = Math.Abs(radiusX);
            var y = Math.Abs(radiusY);
            g.FillEllipse((System.Drawing.Brush)fillBrush.AdapterBrush, center.X - x, center.Y - y, x * 2, y * 2);
        }

        public override void FillGeometry(CPF.Drawing.Brush fillBrush, Geometry geometry)
        {
            InitializeBrush(fillBrush);
            g.FillRegion((System.Drawing.Brush)fillBrush.AdapterBrush, (geometry.GeometryImpl as GDIPlusPathGeometry).Region);
        }

        public override void DrawImage(CPF.Drawing.Image image, in Rect destRect, in Rect srcRect, in float opacity = 1)
        {
            GDIPlusImage i = image.ImageImpl as GDIPlusImage;
            if (i != null)
            {
                if (opacity != 1)
                {
                    ImageAttributes imageAttributes = new ImageAttributes();
                    ChangeOpacity(opacity, imageAttributes);
                    g.DrawImage(i.Image,
                        new Rectangle((int)destRect.X, (int)destRect.Y, (int)destRect.Width, (int)destRect.Height),
                        srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAttributes);
                }
                else
                {
                    g.DrawImage(i.Image,
                        new RectangleF(destRect.X, destRect.Y, destRect.Width, destRect.Height),
                        new RectangleF(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height), GraphicsUnit.Pixel);
                }
            }
        }

        public static void ChangeOpacity(float opacity, ImageAttributes imageAttributes)
        {
            float[][] nArray ={
                                  new float[] {1, 0, 0, 0, 0},
                                  new float[] {0, 1, 0, 0, 0},
                                  new float[] {0, 0, 1, 0, 0},
                                  new float[] {0, 0, 0, opacity, 0},
                                  new float[] {0, 0, 0, 0, 1}
                              };
            ColorMatrix matrix = new ColorMatrix(nArray);
            imageAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        }

        public override void DrawString(in CPF.Drawing.Point location, CPF.Drawing.Brush fillBrush, string text, in CPF.Drawing.Font font, in TextAlignment textAlignment = TextAlignment.Left, in float maxWidth = float.MaxValue, in TextDecoration decoration = default,
           in float maxHeight = float.MaxValue,
           in TextTrimming textTrimming = TextTrimming.None,
           in Stroke stroke = default,
           CPF.Drawing.Brush strokeBrush = null)
        {
            bool ellipsis = false;
            DrawString(out ellipsis, location, fillBrush, text, font, textAlignment, maxWidth, decoration, maxHeight, textTrimming, stroke, strokeBrush);
        }

        public override void DrawString(out bool ellipsis, in CPF.Drawing.Point location, CPF.Drawing.Brush fillBrush, string text, in CPF.Drawing.Font font, in TextAlignment textAlignment = TextAlignment.Left, in float maxWidth = float.MaxValue, in TextDecoration decoration = default,
            in float maxHeight = float.MaxValue,
            in TextTrimming textTrimming = TextTrimming.None,
            in Stroke stroke = default,
            CPF.Drawing.Brush strokeBrush = null)
        {
            InitializeBrush(fillBrush);
            ellipsis = false;
            Pen strokePen = null;
            if (strokeBrush != null && stroke.Width > 0)
            {
                InitializeBrush(strokeBrush);
                strokePen = CreatePen(stroke, strokeBrush.AdapterBrush as System.Drawing.Brush);
            }
            StringFormat stringFormat = GenericTypographic;
            var mw = maxWidth;
            switch (textAlignment)
            {
                case TextAlignment.Right:
                    stringFormat = RightFormat;
                    break;
                case TextAlignment.Center:
                    stringFormat = CenterFormat;
                    break;
            }
            var f = (FontStruct)font.AdapterFont;
            if (textAlignment != TextAlignment.Left && maxWidth == float.MaxValue)
            {
                mw = drawingFactory.g.MeasureString(text, f.Font).Width;
            }
            if (mw != float.MaxValue)
            {
                mw = (float)Math.Ceiling(mw);
            }
            var mH = maxHeight;
            if (mH != float.MaxValue)
            {
                mH = (float)Math.Ceiling(mH);
            }
            float y = location.Y;
            float x = location.X;
            var lines = text.Split('\n');
            var drawLine = false;
            if (decoration.Brush != null && decoration.Stroke.Width > 0)
            {
                drawLine = true;
                InitializeBrush(decoration.Brush);
                SetPen(decoration.Stroke, (System.Drawing.Brush)decoration.Brush.AdapterBrush);
            }
            var fHeight = f.Height;
            var h = 0f;
            var br = false;
            //bool ellipsis = false;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim('\r');
                if (i == lines.Length - 1 && line == "")
                {
                    break;
                }
                //Console.WriteLine(line + "," + cf + "," + ls);

                var index = 0;
                var count = 0;
                var len = line.Length;
                while (index < len)
                {
                    h += fHeight;
                    if (h > mH && y != location.Y)
                    {
                        br = true;
                        break;
                    }

                    var s = drawingFactory.g.MeasureString(line, f.Font, new SizeF(mw, fHeight), stringFormat, out count, out var ls);
                    string sub = line.Substring(0, count);
                    if (!string.IsNullOrWhiteSpace(sub))
                    {
                        var txt = sub;
                        if (textTrimming == TextTrimming.CharacterEllipsis && (sub.Length < line.Length || i < lines.Length - 1))
                        {
                            if (h + fHeight > mH)
                            {
                                if (s.Width < mw - fHeight)
                                {
                                    txt = txt + "...";
                                }
                                else
                                {
                                    txt = txt.Substring(0, txt.Length - 1) + "...";
                                }
                                ellipsis = true;
                            }
                        }
                        if (textTrimming == TextTrimming.CharacterCenterEllipsis && (sub.Length < line.Length || i < lines.Length - 1))
                        {
                            ellipsis = true;
                            DrawCenterString(location, fillBrush, text, font, textAlignment, maxWidth, decoration, maxHeight, stroke, strokeBrush);
                            return;
                        }
                        if (strokeBrush != null && stroke.Width > 0)
                        {
                            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                            {
                                path.AddString(txt, f.Font.FontFamily, (int)f.Font.Style, f.Font.Size, new RectangleF(x - 0.5f, y - 0.5f, mw, fHeight), stringFormat);
                                g.DrawPath(strokePen, path);
                                g.FillPath((System.Drawing.Brush)fillBrush.AdapterBrush, path);
                            }
                        }
                        else
                        {
                            g.DrawString(txt, f.Font, (System.Drawing.Brush)fillBrush.AdapterBrush, new RectangleF(x, y, mw, fHeight), stringFormat);
                        }

                    }
                    if (drawLine)
                    {
                        var lw = drawingFactory.g.MeasureString(sub, f.Font).Width;

                        if (decoration.Location.HasFlag(TextDecorationLocation.Underline))
                        {
                            var ly = y;
                            ly = y + fHeight;

                            var lx = x;
                            switch (textAlignment)
                            {
                                case TextAlignment.Right:
                                    lx = x + mw - lw;
                                    break;
                                case TextAlignment.Center:
                                    lx = x + (mw - lw) / 2;
                                    break;
                            }
                            g.DrawLine(pen, lx, ly, lx + lw, ly);
                        }
                        if (decoration.Location.HasFlag(TextDecorationLocation.OverLine))
                        {
                            var ly = y;
                            var lx = x;
                            switch (textAlignment)
                            {
                                case TextAlignment.Right:
                                    lx = x + mw - lw;
                                    break;
                                case TextAlignment.Center:
                                    lx = x + (mw - lw) / 2;
                                    break;
                            }
                            g.DrawLine(pen, lx, ly, lx + lw, ly);
                        }
                        if (decoration.Location.HasFlag(TextDecorationLocation.Strikethrough))
                        {
                            var ly = y;
                            ly = y + fHeight / 2;

                            var lx = x;
                            switch (textAlignment)
                            {
                                case TextAlignment.Right:
                                    lx = x + mw - lw;
                                    break;
                                case TextAlignment.Center:
                                    lx = x + (mw - lw) / 2;
                                    break;
                            }
                            g.DrawLine(pen, lx, ly, lx + lw, ly);
                        }

                    }

                    y += fHeight;
                    index += count;
                    if (index >= len)
                    {
                        break;
                    }
                    line = line.Substring(count);
                }
                if (br)
                {
                    break;
                }
            }
            strokePen?.Dispose();
        }

        public void DrawCenterString(in CPF.Drawing.Point location, CPF.Drawing.Brush fillBrush, string text, in CPF.Drawing.Font font, in TextAlignment textAlignment = TextAlignment.Left, in float maxWidth = float.MaxValue, in TextDecoration decoration = default,
            in float maxHeight = float.MaxValue,
            in Stroke stroke = default,
            CPF.Drawing.Brush strokeBrush = null)
        {
            InitializeBrush(fillBrush);
            Pen strokePen = null;
            if (strokeBrush != null && stroke.Width > 0)
            {
                InitializeBrush(strokeBrush);
                strokePen = CreatePen(stroke, strokeBrush.AdapterBrush as System.Drawing.Brush);
            }
            StringFormat stringFormat = GenericTypographic;
            var mw = maxWidth;
            switch (textAlignment)
            {
                case TextAlignment.Right:
                    stringFormat = RightFormat;
                    break;
                case TextAlignment.Center:
                    stringFormat = CenterFormat;
                    break;
            }
            var f = (FontStruct)font.AdapterFont;
            if (textAlignment != TextAlignment.Left && maxWidth == float.MaxValue)
            {
                mw = drawingFactory.g.MeasureString(text, f.Font).Width;
            }
            if (mw != float.MaxValue)
            {
                mw = (float)Math.Ceiling(mw);
            }
            var mH = maxHeight;
            if (mH != float.MaxValue)
            {
                mH = (float)Math.Ceiling(mH);
            }
            float y = location.Y;
            float x = location.X;

            var drawLine = false;
            if (decoration.Brush != null && decoration.Stroke.Width > 0)
            {
                drawLine = true;
                InitializeBrush(decoration.Brush);
                SetPen(decoration.Stroke, (System.Drawing.Brush)decoration.Brush.AdapterBrush);
            }
            var fHeight = f.Height;
            var h = 0f;
            var br = false;
            //bool ellipsis = false;

            string newText = text;
            var lines = text.Split('\n');
            if (lines.Length > 1)
            {
                newText = lines[0].Trim('\r') + "..." + lines[lines.Length - 1].Trim('\r');
            }
            else
            {
                newText = text.Trim('\r');
            }

            var index = 0;
            var count = 0;
            var len = newText.Length;
            while (index < len)
            {
                h += fHeight;
                if (h > mH && y != location.Y)
                {
                    br = true;
                    break;
                }

                var s = drawingFactory.g.MeasureString(newText, f.Font, new SizeF(mw, fHeight), stringFormat, out count, out var ls);

                string sub = newText.Substring(0, count);
                if (!string.IsNullOrWhiteSpace(sub) && sub == newText)
                {
                    var txt = sub;
                    if (strokeBrush != null && stroke.Width > 0)
                    {
                        using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                        {
                            path.AddString(txt, f.Font.FontFamily, (int)f.Font.Style, f.Font.Size, new RectangleF(x - 0.5f, y - 0.5f, mw, fHeight), stringFormat);
                            g.DrawPath(strokePen, path);
                            g.FillPath((System.Drawing.Brush)fillBrush.AdapterBrush, path);
                        }
                    }
                    else
                    {
                        g.DrawString(txt, f.Font, (System.Drawing.Brush)fillBrush.AdapterBrush, new RectangleF(x, y, mw, fHeight), stringFormat);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(sub) && sub != newText)
                {
                    var txt = sub;

                    int countStart = 0;
                    int countEnd = 0;
                    var sStart = drawingFactory.g.MeasureString(newText, f.Font, new SizeF(mw / 2, fHeight), stringFormat, out countStart, out var lsStart);
                    List<char> wsEnds = new List<char>();
                    for (int i = newText.Length - 1; i >= 0; i--)
                    {
                        wsEnds.Add(newText[i]);
                    }
                    StringBuilder wsEnd = new StringBuilder();
                    wsEnds.ForEach(p =>
                    {
                        wsEnd.Append(p);
                    });
                    var sEnd = drawingFactory.g.MeasureString(wsEnd.ToString(), f.Font, new SizeF(mw / 2, fHeight), stringFormat, out countEnd, out var lsEnd);
                    string strStart = "";
                    if (countStart > 0)
                        strStart = newText.Substring(0, countStart - 1);

                    string strEnd = "";
                    if (countEnd > 0)
                        strEnd = newText.Substring(newText.Length - countEnd + 1, countEnd - 1);

                    newText = strStart + "..." + strEnd;
                    txt = newText;
                    if (strokeBrush != null && stroke.Width > 0)
                    {
                        using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                        {
                            path.AddString(txt, f.Font.FontFamily, (int)f.Font.Style, f.Font.Size, new RectangleF(x - 0.5f, y - 0.5f, mw, fHeight), stringFormat);
                            g.DrawPath(strokePen, path);
                            g.FillPath((System.Drawing.Brush)fillBrush.AdapterBrush, path);
                        }
                    }
                    else
                    {
                        g.DrawString(txt, f.Font, (System.Drawing.Brush)fillBrush.AdapterBrush, new RectangleF(x, y, mw, fHeight), stringFormat);
                    }

                }
                if (drawLine)
                {
                    var lw = drawingFactory.g.MeasureString(sub, f.Font).Width;

                    if (decoration.Location.HasFlag(TextDecorationLocation.Underline))
                    {
                        var ly = y;
                        ly = y + fHeight;

                        var lx = x;
                        switch (textAlignment)
                        {
                            case TextAlignment.Right:
                                lx = x + mw - lw;
                                break;
                            case TextAlignment.Center:
                                lx = x + (mw - lw) / 2;
                                break;
                        }
                        g.DrawLine(pen, lx, ly, lx + lw, ly);
                    }
                    if (decoration.Location.HasFlag(TextDecorationLocation.OverLine))
                    {
                        var ly = y;
                        var lx = x;
                        switch (textAlignment)
                        {
                            case TextAlignment.Right:
                                lx = x + mw - lw;
                                break;
                            case TextAlignment.Center:
                                lx = x + (mw - lw) / 2;
                                break;
                        }
                        g.DrawLine(pen, lx, ly, lx + lw, ly);
                    }
                    if (decoration.Location.HasFlag(TextDecorationLocation.Strikethrough))
                    {
                        var ly = y;
                        ly = y + fHeight / 2;

                        var lx = x;
                        switch (textAlignment)
                        {
                            case TextAlignment.Right:
                                lx = x + mw - lw;
                                break;
                            case TextAlignment.Center:
                                lx = x + (mw - lw) / 2;
                                break;
                        }
                        g.DrawLine(pen, lx, ly, lx + lw, ly);
                    }

                }

                y += fHeight;
                index += count;
                if (index >= len)
                {
                    break;
                }
            }

            strokePen?.Dispose();
        }

        List<Region> oldRegions = new List<Region>();
        public override void PushClip(Rect rect)
        {
            oldRegions.Add(g.Clip);
            g.SetClip(rect.ToRectangle(), System.Drawing.Drawing2D.CombineMode.Intersect);
        }

        public override void PopClip()
        {
            if (oldRegions.Count > 1)
            {
                g.SetClip(oldRegions[oldRegions.Count - 1], System.Drawing.Drawing2D.CombineMode.Replace);
                oldRegions.RemoveAt(oldRegions.Count - 1);
            }
            else
            {
                g.ResetClip();
            }
        }

        public override Matrix Transform
        {
            get
            {
                return g.Transform.ToMatrix();
            }
            set
            {
                //System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix((float)matrix.M11, (float)matrix.M12, (float)matrix.M21, (float)matrix.M22, (float)matrix.OffsetX, (float)matrix.OffsetY);
                g.Transform = value.ToMatrix();
            }
        }
        GDIPlusDrawingFactory _drawingFactory;
        internal GDIPlusDrawingFactory drawingFactory
        {
            get { return _drawingFactory; }
            set
            {
                _drawingFactory = value;
                this.AntialiasMode = this.AntialiasMode;
            }
        }
        public override DrawingFactory DrawingFactory
        {
            get { return drawingFactory; }
        }

        protected override IDisposable CreateSolidBrush(CPF.Drawing.Color color)
        {
            return new SolidBrush(color.ToGdiColor());
        }

        protected override IDisposable CreateLinearGradientBrush(GradientStop[] bcs, in CPF.Drawing.Point start, in CPF.Drawing.Point end, in Matrix matrix)
        {
            bcs[0].Position = 0;
            System.Drawing.Drawing2D.ColorBlend cb = new System.Drawing.Drawing2D.ColorBlend();
            cb.Colors = bcs.Select(a => a.Color.ToGdiColor()).ToArray();
            cb.Positions = bcs.Select(a => a.Position).ToArray();
            System.Drawing.Drawing2D.LinearGradientBrush l = new System.Drawing.Drawing2D.LinearGradientBrush(start.ToGdiPoint(), end.ToGdiPoint(), bcs[0].Color.ToGdiColor(), bcs[1].Color.ToGdiColor());
            l.InterpolationColors = cb;
            l.MultiplyTransform(matrix.ToMatrix());
            //l.Transform = matrix.ToMatrix();
            //l.RotateTransform(40);
            //var m = l.Transform;
            //m.Multiply(matrix.ToMatrix());
            //l.Transform = m;
            return l;
        }



        public override void Dispose()
        {
            if (hdc != IntPtr.Zero)
            {
                g.Dispose();
                hdc = IntPtr.Zero;
            }

            if (pen != null)
            {
                pen.Dispose();
                pen = null;
            }
            if (GenericTypographic != null)
            {
                GenericTypographic.Dispose();
                GenericTypographic = null;
            }
            foreach (var item in oldRegions)
            {
                item.Dispose();
            }
        }

        public override void Clear(CPF.Drawing.Color color)
        {
            g.Clear(color.ToGdiColor());
        }

        protected override IDisposable CreateTextureBrush(CPF.Drawing.Image image, in WrapMode wrapMode, in Matrix matrix)
        {
            GDIPlusImage i = image.ImageImpl as GDIPlusImage;
            //try
            //{
            var wrap = wrapMode == WrapMode.Clamp ? System.Drawing.Drawing2D.WrapMode.Clamp : System.Drawing.Drawing2D.WrapMode.Tile;
            System.Drawing.TextureBrush hb;
            //if (i.Image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Png.Guid)
            //{
            //using (var bitmap = new System.Drawing.Bitmap(i.Width, i.Height))
            //{//某些格式的图片会出现异常，只能重新创建过图片了
            //    using (Graphics g = Graphics.FromImage(bitmap))
            //    {
            //        g.DrawImage(i.Image, new Rectangle(0, 0, i.Width, i.Height), new Rectangle(0, 0, i.Width, i.Height), GraphicsUnit.Pixel);
            //    }
            //    hb = new System.Drawing.TextureBrush(bitmap, wrap, new RectangleF(0, 0, i.Width, i.Height));
            //}
            //}
            //else
            //{
            //    try
            //    {
            hb = new System.Drawing.TextureBrush(i.Image, wrap, new RectangleF(0, 0, i.Width, i.Height));
            //    }
            //    catch (Exception e)
            //    {
            //        throw new Exception("可能是GDI+图片兼容性问题，换个图片试试", e);
            //    }
            //}
            //hb.ScaleTransform(96 / g.DpiX, 96 / g.DpiY);
            //hb.Transform = matrix.ToMatrix();
            hb.MultiplyTransform(matrix.ToMatrix());
            return hb;

            //}
            //catch (Exception e)
            //{
            //    //Console.WriteLine(i.Image + ":" + i.Image.Size + i.Image.PixelFormat + (i.Image.RawFormat == System.Drawing.Imaging.ImageFormat.Png));
            //    throw e;
            //}
        }

        public override void DrawPath(CPF.Drawing.Brush strokeBrush, in Stroke stroke, PathGeometry path)
        {
            InitializeBrush(strokeBrush);
            SetPen(stroke, (System.Drawing.Brush)strokeBrush.AdapterBrush);
            g.DrawPath(pen, (path.PathIml as GDIPlusPath).Path);
        }

        public override void FillPath(CPF.Drawing.Brush fillBrush, PathGeometry path)
        {
            InitializeBrush(fillBrush);
            g.FillPath((System.Drawing.Brush)fillBrush.AdapterBrush, (path.PathIml as GDIPlusPath).Path);
        }

        protected override IDisposable CreateRadialGradientBrush(in CPF.Drawing.Point center, in float radius, GradientStop[] blendColors, in Matrix matrix)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2));
            System.Drawing.Drawing2D.PathGradientBrush p = new System.Drawing.Drawing2D.PathGradientBrush(path);
            p.CenterPoint = new PointF(center.X, center.Y);
            blendColors[0].Position = 0;
            System.Drawing.Drawing2D.ColorBlend cb = new System.Drawing.Drawing2D.ColorBlend();
            cb.Colors = blendColors.Select(a => a.Color.ToGdiColor()).Reverse().ToArray();
            cb.Positions = blendColors.Select(a => 1 - a.Position).Reverse().ToArray();
            p.InterpolationColors = cb;
            //p.Transform = matrix.ToMatrix();
            p.MultiplyTransform(matrix.ToMatrix());
            return p;
        }

        //public override void DrawGeometry(CPF.Drawing.Brush strokeBrush, in Stroke stroke, Geometry geometry)
        //{
        //    //InitializeBrush(strokeBrush);
        //    //g.FillRegion((System.Drawing.Brush)fillBrush.AdapterBrush, (geometry.GeometryImpl as GDIPlusPathGeometry).Region);
        //    throw new Exception("未实现DrawGeometry");
        //}

        //public override IDisposable CreateGeometry(Path path)
        //{
        //    return new Region((System.Drawing.Drawing2D.GraphicsPath)path.AdapterPath);
        //}
    }


}
#endif