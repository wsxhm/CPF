using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkiaSharp;
using System.Runtime.InteropServices;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Platform;
using System.Diagnostics;
using CPF.OpenGL;

namespace CPF.Skia
{
    public class SkiaDrawingContext : DrawingContext
    {
        public SKCanvas SKCanvas
        {
            get { return canvas; }
        }

        SKCanvas canvas;
        SKBitmap bitmap;
        SKSurface surface;
        List<PaintWrapper> cachePaints = new List<PaintWrapper>(2);
        //GRContext grContext;
        GRBackendRenderTarget backendRenderTarget;
        SKAutoCanvasRestore canvasRestore;
        //static GRGlInterface glInterface;
        public SkiaDrawingContext(Bitmap bitmap, SkiaDrawingFactory drawingFactory)
        {
            this.drawingFactory = drawingFactory;
            var bmp = bitmap.BitmapImpl as SkiaBitmap;
            //glContext = GlContext.Create(null);
            //glContext.MakeCurrent();
            ////glInterface = GRGlInterface.CreateNativeGlInterface();
            //grContext = GRContext.CreateGl();
            canvas = new SKCanvas(bmp.Bitmap);
            //this.bitmap = bmp.Bitmap;

            //surface = SKSurface.Create(grContext, true, new SKImageInfo { Width = this.bitmap.Width, Height = this.bitmap.Height, AlphaType = this.bitmap.AlphaType, ColorSpace = this.bitmap.ColorSpace, ColorType = this.bitmap.ColorType }, 0, GRSurfaceOrigin.TopLeft);
            //canvas = surface.Canvas;
        }
        public SkiaDrawingContext(SKCanvas Canvas, SkiaDrawingFactory drawingFactory)
        {
            this.drawingFactory = drawingFactory;
            canvas = Canvas;
        }

        GRContext gRContext;
        public IGlContext GlContext;
        public SkiaDrawingContext(IRenderTarget target, SkiaDrawingFactory drawingFactory)
        {
            this.drawingFactory = drawingFactory;
            if (target.Width < 1 || target.Height < 1)
            {
                Debug.WriteLine("绘图表面为空");
                Console.WriteLine("绘图表面为空");
            }
            if (!target.CanUseGPU || !drawingFactory.UseGPU)
            {
                IntPtr hdc = IntPtr.Zero;
                if (target is HDCRenderTarget hDC)
                {
                    hdc = hDC.Hdc;
                }
                else if (target is OpenGlRenderTarget<IntPtr> open)
                {
                    hdc = open.FailBackTarget;
                }
                if (hdc != IntPtr.Zero)
                {
                    CreateHdcCanvas(hdc);
                }
            }
            else
            {
                //System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                //glInterface = GRGlInterface.CreateNativeGlInterface();
                if (target is OpenGlRenderTarget open)
                {
                    var grContext = open.GlContext.GRContext as GRContext;
                    if (grContext == null)
                    {
                        grContext = GRContext.CreateGl();
                        open.GlContext.GRContext = grContext;
                    }
                    gRContext = grContext;
                    GlContext = open.GlContext;
                    if (grContext == null)
                    {
                        if (target is OpenGlRenderTarget<IntPtr> openHdc)
                        {
                            Console.WriteLine("opengl创建失败，将自动改为CPU渲染");
                            Debug.WriteLine("opengl创建失败，将自动改为CPU渲染");
                            drawingFactory.UseGPU = false;
                            CreateHdcCanvas(openHdc.FailBackTarget);
                        }
                    }
                    else
                    {
                        var maxSamples = grContext.GetMaxSurfaceSampleCount(SKColorType.Rgba8888);
                        var samples = open.Samples;
                        if (samples > maxSamples)
                            samples = maxSamples;
                        var framebufferInfo = new GRGlFramebufferInfo((uint)open.Framebuffer, SKColorType.Rgba8888.ToGlSizedFormat());
                        backendRenderTarget = new GRBackendRenderTarget(target.Width, target.Height, samples, open.Stencil, framebufferInfo);
                        surface = SKSurface.Create(grContext, backendRenderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
                        canvas = surface.Canvas;
                        canvasRestore = new SKAutoCanvasRestore(canvas, true);
                    }
                }
                //System.Diagnostics.Debug.WriteLine(stopwatch.ElapsedMilliseconds);
            }
            if (canvas == null)
            {
                throw new Exception("Canvas创建失败:" + target);
            }
        }

        private void CreateHdcCanvas(IntPtr hdc)
        {
            //获取dc里的位图信息，Windows平台
            var hbitmap = UnmanagedMethods.GetCurrentObject(hdc, ObjectType.OBJ_BITMAP);
            BITMAP BITMAP = new BITMAP();
            UnmanagedMethods.GetObject(hbitmap, Marshal.SizeOf(typeof(BITMAP)), BITMAP);

            var bitmap = new SKBitmap();
            bitmap.InstallPixels(new SKImageInfo(BITMAP.bmWidth, BITMAP.bmHeight, SKImageInfo.PlatformColorType), BITMAP.bmBits, BITMAP.bmWidthBytes);

            this.bitmap = bitmap;
            canvas = new SKCanvas(this.bitmap);
        }

        //public SkiaDrawingContext(SKBitmap bitmap)
        //{
        //    this.bitmap = bitmap;
        //    canvas = new SKCanvas(this.bitmap);
        //}

        public override void Dispose()
        {
            //System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
            canvasRestore?.Dispose();
            canvasRestore = null;
            if (canvas != null)
            {
                canvas.Flush();
                canvas.Dispose();
                canvas = null;
            }
            if (surface != null)
            {
                surface.Dispose();
                surface = null;
            }
            if (gRContext != null)
            {
                //gRContext.Flush();
                gRContext.ResetContext();
            }
            //if (grContext != null)
            //{
            //    //grContext.Flush();
            //    grContext.Dispose();
            //    grContext = null;
            //}
            //if (glContext != null)
            //{
            //    //glContext.DestroyTexture(backendRenderTarget.GetGlFramebufferInfo().FramebufferObjectId);
            //    //glContext.SwapBuffers();
            //    glContext.Dispose();
            //    glContext = null;
            //}
            if (backendRenderTarget != null)
            {
                backendRenderTarget.Dispose();
                backendRenderTarget = null;
            }
            foreach (var item in cachePaints)
            {
                item.Paint.Dispose();
            }
            cachePaints.Clear();
            //System.Diagnostics.Debug.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        //public SkiaDrawingContext(IntPtr hdc, Rect clip)
        //{
        //    //获取dc里的位图信息，Windows平台
        //    var hbitmap = ConsoleApp1.UnmanagedMethods.GetCurrentObject(hdc, ConsoleApp1.UnmanagedMethods.ObjectType.OBJ_BITMAP);
        //    ConsoleApp1.UnmanagedMethods.BITMAP BITMAP = new ConsoleApp1.UnmanagedMethods.BITMAP();
        //    ConsoleApp1.UnmanagedMethods.GetObject(hbitmap, Marshal.SizeOf(typeof(ConsoleApp1.UnmanagedMethods.BITMAP)), BITMAP);

        //    bitmap = new SKBitmap(BITMAP.bmWidth, BITMAP.bmHeight);
        //    bitmap.SetPixels(BITMAP.bmBits);

        //    canvas = new SKCanvas(bitmap);
        //    //currentRect = new Rect(0, 0, bitmap.Width, bitmap.Height);
        //}

        public override Matrix Transform
        {
            get
            {
                return canvas.TotalMatrix.ToMatrix();
            }

            set
            {
                canvas.SetMatrix(value.ToMatrix());
            }
        }

        public override AntialiasMode AntialiasMode
        {
            get;
            set;
        }
        SkiaDrawingFactory drawingFactory;
        public override DrawingFactory DrawingFactory
        {
            get
            {
                return drawingFactory;
            }
        }

        public override void Clear(Color color)
        {
            canvas.Clear(color.ToSKColor());
        }

        internal PaintWrapper CreatePaint(Brush brush, in Font font = default)
        {
            PaintWrapper paintWrapper;
            SKPaint paint;
            GetPaint(out paintWrapper, out paint);
            //double opacity = brush.Opacity * _currentOpacity;

            //if (brush is SolidColorBrush solid)
            //{
            //    paint.Color = solid.Color.ToSKColor();

            //    return paintWrapper;
            //}
            if (font.FontFamily != null)
            {
                paint.TextEncoding = SKTextEncoding.Utf16;
                paint.Typeface = (font.AdapterFont as FontWrapper).SKTypeface;
                paint.TextSize = font.FontSize;
            }

            paint.Shader = brush.AdapterBrush as SKShader;
            return paintWrapper;
        }

        private void GetPaint(out PaintWrapper paintWrapper, out SKPaint paint)
        {
            paintWrapper = default;
            paint = null;
            foreach (var item in cachePaints)
            {
                if (!item.isUsing)
                {
                    paintWrapper = item;
                    paint = item.Paint;
                    break;
                }
            }
            if (paint == null)
            {
                paintWrapper = new PaintWrapper(new SKPaint());
                paint = paintWrapper.Paint;
                cachePaints.Add(paintWrapper);
            }
            paint.IsAntialias = AntialiasMode == AntialiasMode.AntiAlias;
            paint.FilterQuality = AntialiasMode == AntialiasMode.AntiAlias ? SKFilterQuality.Medium : SKFilterQuality.None;
            paintWrapper.isUsing = true;
        }

        private PaintWrapper CreatePaint(Brush brush, Stroke stroke, in Font font = default)
        {
            var rv = CreatePaint(brush, font);
            var paint = rv.Paint;

            paint.IsStroke = true;
            paint.StrokeWidth = stroke.Width;

            // Need to modify dashes due to Skia modifying their lengths
            // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/paths/dots
            // TODO: Still something is off, dashes are now present, but don't look the same as D2D ones.

            switch (stroke.StrokeCap)
            {
                case CapStyles.Round:
                    paint.StrokeCap = SKStrokeCap.Round;
                    break;
                case CapStyles.Square:
                    paint.StrokeCap = SKStrokeCap.Square;
                    break;
                default:
                    paint.StrokeCap = SKStrokeCap.Butt;
                    break;
            }

            switch (stroke.LineJoin)
            {
                case LineJoins.Miter:
                    paint.StrokeJoin = SKStrokeJoin.Miter;
                    break;
                case LineJoins.Round:
                    paint.StrokeJoin = SKStrokeJoin.Round;
                    break;
                default:
                    paint.StrokeJoin = SKStrokeJoin.Bevel;
                    break;
            }

            //paint.StrokeMiter = (float)pen.MiterLimit;

            var srcDashes = stroke.DashPattern;
            if (stroke.DashStyle != DashStyles.Solid)
            {
                srcDashes = stroke.GetDashPattern();
            }
            if (srcDashes != null && srcDashes.Length > 0)
            {
                var dashesArray = new float[srcDashes.Length];

                for (var i = 0; i < srcDashes.Length; ++i)
                {
                    dashesArray[i] = srcDashes[i] * paint.StrokeWidth;
                }

                //var offset = stroke.DashOffset * stroke.Width;

                var pe = SKPathEffect.CreateDash(dashesArray, stroke.DashOffset);

                paint.PathEffect = pe;
                rv.AddDisposable(pe);
            }

            return rv;
        }

        public override void DrawEllipse(Brush strokeBrush, in Stroke stroke, in Point center, in float radiusX, in float radiusY)
        {
            InitializeBrush(strokeBrush);
            using (var paint = CreatePaint(strokeBrush, stroke))
            {
                canvas.DrawOval(center.X, center.Y, radiusX, radiusY, paint.Paint);
            }
        }

        public override void DrawImage(Image image, in Rect destRect, in Rect srcRect, in float opacity = 1)
        {
            //using (var paint = new SKPaint { Color = new SKColor(255, 255, 255, (byte)(255 * opacity)) })
            PaintWrapper paintWrapper;
            SKPaint paint;
            GetPaint(out paintWrapper, out paint);
            using (paintWrapper)
            {
                paint.Color = new SKColor(255, 255, 255, (byte)(255 * opacity));
                //paint.IsAntialias = AntialiasMode == AntialiasMode.AntiAlias;
                //paint.FilterQuality = AntialiasMode == AntialiasMode.AntiAlias ? SKFilterQuality.Medium : SKFilterQuality.None;
                SKBitmap bitmap;
                if (image.ImageImpl is SkiaBitmap bmp)
                {
                    bitmap = bmp.Bitmap;
                }
                else if (image.ImageImpl is SkiaImage img)
                {
                    bitmap = img.Image;
                }
                else
                {
                    throw new Exception("图片类型不支持");
                }
                //if (image is Image img)
                //{
                //    canvas.DrawBitmap((img.ImageImpl as SkiaImage).Image, srcRect.ToSKRect(), destRect.ToSKRect(), paint);
                //}
                //else if (image is Bitmap bitmap)
                //{
                canvas.DrawBitmap(bitmap, srcRect.ToSKRect(), destRect.ToSKRect(), paint);
                //}
            }
        }

        public override void DrawLine(in Stroke stroke, Brush strokeBrush, in Point point1, in Point point2)
        {
            InitializeBrush(strokeBrush);
            using (var paint = CreatePaint(strokeBrush, stroke))
            {
                canvas.DrawLine(point1.ToSKPoint(), point2.ToSKPoint(), paint.Paint);
            }
        }

        public override void DrawPath(Brush strokeBrush, in Stroke stroke, PathGeometry path)
        {
            InitializeBrush(strokeBrush);
            using (var paint = CreatePaint(strokeBrush, stroke))
            {
                canvas.DrawPath((path.PathIml as SkiaPath).SKPath, paint.Paint);
            }
        }

        public override void DrawRectangle(Brush strokeBrush, in Stroke stroke, in Rect rect)
        {
            InitializeBrush(strokeBrush);
            using (var paint = CreatePaint(strokeBrush, stroke))
            {
                canvas.DrawRect(rect.ToSKRect(), paint.Paint);
            }
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

        public override void DrawString(out bool ellipsis, in Point location, Brush fillBrush, string text, in Font font, in TextAlignment textAlignment = TextAlignment.Left, in float maxWidth = float.MaxValue, in TextDecoration decoration = default,
            in float maxHeight = float.MaxValue,
            in TextTrimming textTrimming = TextTrimming.None,
            in Stroke stroke = default,
            Brush strokeBrush = null)
        {
            InitializeBrush(fillBrush);
            ellipsis = false;
            PaintWrapper paintWrapper = default;
            if (strokeBrush != null && stroke.Width > 0)
            {
                InitializeBrush(strokeBrush);
                paintWrapper = CreatePaint(strokeBrush, stroke, font);
            }
            var drawline = decoration.Stroke.Width > 0 && decoration.Brush != null && decoration.Location != TextDecorationLocation.None;
            List<LinePosition> linePositions = null;
            if (drawline)
            {
                linePositions = new List<LinePosition>();
            }
            using (var paint = CreatePaint(fillBrush, font))
            {
                paint.Paint.LcdRenderText = drawingFactory.ClearType;
                paint.Paint.TextEncoding = SKTextEncoding.Utf16;
                //paint.Paint.SubpixelText = true;
                //paint.Paint.HintingLevel = SKPaintHinting.Full;
                paint.Paint.IsStroke = false;
                if (text.Length == 1 || (text.Length == 2 && char.IsSurrogate(text[0])))
                {
                    var w = 0f;
                    var x = location.X;
                    if (textAlignment != TextAlignment.Left && maxWidth != float.MaxValue || drawline)
                    {
                        w = paint.Paint.MeasureString(text);
                        if (textAlignment != TextAlignment.Left && maxWidth != float.MaxValue)
                        {
                            if (textAlignment == TextAlignment.Right)
                            {
                                x = x + (maxWidth - w);
                            }
                            else if (textAlignment == TextAlignment.Center)
                            {
                                x = x + (maxWidth - w) / 2;
                            }
                        }
                    }
                    if (strokeBrush != null && stroke.Width > 0)
                    {
                        //using (var path = paintWrapper.Paint.GetTextPath(text, x, location.Y - paintWrapper.Paint.FontMetrics.Ascent))
                        //{
                        //    canvas.DrawPath(path, paintWrapper.Paint);
                        //    canvas.DrawPath(path, paint.Paint);
                        //}
                        canvas.DrawTextPath(paint.Paint, paintWrapper.Paint, text, x, location.Y - paintWrapper.Paint.FontMetrics.Ascent);
                    }
                    else
                    {
                        //canvas.DrawText(text, x, location.Y - paint.Paint.FontMetrics.Ascent, paint.Paint);
                        canvas.DrawText(paint.Paint, text, x, location.Y - paint.Paint.FontMetrics.Ascent);
                    }

                    if (drawline)
                    {
                        if (text == " ")
                        {
                            w += 2;
                        }
                        if (decoration.Location.HasFlag(TextDecorationLocation.Underline))
                        {
                            linePositions.Add(new LinePosition { X = x, Y = location.Y + paint.Paint.FontSpacing, Width = w });
                        }
                        if (decoration.Location.HasFlag(TextDecorationLocation.OverLine))
                        {
                            linePositions.Add(new LinePosition { X = x, Y = location.Y, Width = w });
                        }
                        if (decoration.Location.HasFlag(TextDecorationLocation.Strikethrough))
                        {
                            linePositions.Add(new LinePosition { X = x, Y = location.Y + paint.Paint.FontSpacing / 2, Width = w });
                        }
                    }
                }
                else
                {
                    var lines = text.Split('\n');
                    //float width = 0;
                    float y = location.Y - paint.Paint.FontMetrics.Ascent;//top
                    float x = location.X;
                    List<(string text, float y, float w)> list = new List<(string, float, float)>();
                    float w = 0f;
                    float h = 0;
                    bool br = false;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i].Trim('\r');
                        if (i == lines.Length - 1 && line == "")
                        {
                            break;
                        }
                        var ws = paint.Paint.MeasureAllChar(line);
                        //var str = line;
                        var start = 0;
                        while (true)
                        {
                            h += paint.Paint.FontSpacing;
                            if (h > maxHeight && list.Count > 0)
                            {
                                br = true;
                                break;
                            }
                            //var len = Math.Max(1, (int)paint.Paint.BreakText(str, (float)Math.Ceiling(maxWidth)));
                            var len = ws.BreakText(start, (float)Math.Ceiling(maxWidth));
                            if (start + len.Item1 <= ws.Count)
                            {
                                //str = line.Substring(start, len);
                                start += len.Item1;
                            }

                            //var ww = paint.Paint.MeasureString(str);
                            list.Add((len.Item3, y, len.Item2));
                            w = Math.Max(w, len.Item2);
                            y += paint.Paint.FontSpacing;
                            if (start <= ws.Count - 1)
                            {
                                //str = line.Substring(start);
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (br)
                        {
                            if (textTrimming == TextTrimming.CharacterEllipsis)
                            {
                                ellipsis = true;
                            }
                            if (textTrimming == TextTrimming.CharacterCenterEllipsis)
                            {
                                ellipsis = true;
                                DrawCenterString(location, fillBrush, text, font, textAlignment, maxWidth, decoration, maxHeight, stroke, strokeBrush);
                                return;
                            }
                            break;
                        }
                    }

                    if (maxWidth != float.MaxValue)
                    {
                        w = Math.Max(w, maxWidth);
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        var xx = x;
                        if (textAlignment == TextAlignment.Right)
                        {
                            xx = x + w - item.w;
                        }
                        else if (textAlignment == TextAlignment.Center)
                        {
                            xx = x + (w - item.w) / 2;
                        }
                        var txt = item.text;
                        if (ellipsis && i == list.Count - 1)
                        {
                            if (item.w < w - paint.Paint.FontSpacing)
                            {
                                txt = txt + "...";
                            }
                            else
                            {
                                txt = txt.Substring(0, txt.Length - 1) + "...";
                            }
                        }
                        if (strokeBrush != null && stroke.Width > 0)
                        {
                            //using (var path = paintWrapper.Paint.GetTextPath(txt, xx, item.y))
                            //{
                            //    paintWrapper.Paint.TextEncoding = SKTextEncoding.Utf16;
                            //    canvas.DrawPath(path, paintWrapper.Paint);
                            //    canvas.DrawPath(path, paint.Paint);
                            //}

                            canvas.DrawTextPath(paint.Paint, paintWrapper.Paint, txt, xx, item.y);
                        }
                        else
                        {
                            //canvas.DrawText(txt, xx, item.y, paint.Paint);
                            canvas.DrawText(paint.Paint, txt, xx, item.y);
                        }
                        if (drawline)
                        {
                            if (decoration.Location.HasFlag(TextDecorationLocation.Underline))
                            {
                                linePositions.Add(new LinePosition { X = xx, Y = item.y + paint.Paint.FontSpacing + paint.Paint.FontMetrics.Ascent, Width = item.w });
                            }
                            if (decoration.Location.HasFlag(TextDecorationLocation.OverLine))
                            {
                                linePositions.Add(new LinePosition { X = xx, Y = item.y + paint.Paint.FontMetrics.Ascent, Width = item.w });
                            }
                            if (decoration.Location.HasFlag(TextDecorationLocation.Strikethrough))
                            {
                                linePositions.Add(new LinePosition { X = xx, Y = item.y + paint.Paint.FontSpacing / 2 + paint.Paint.FontMetrics.Ascent, Width = item.w });
                            }
                        }
                    }

                }
            }
            paintWrapper?.Dispose();
            if (drawline)
            {
                //using (var brush = decoration.Brush)
                {
                    InitializeBrush(decoration.Brush);
                    using (var paint = CreatePaint(decoration.Brush, decoration.Stroke))
                    {
                        foreach (var item in linePositions)
                        {
                            canvas.DrawLine(item.X, item.Y, item.X + item.Width, item.Y, paint.Paint);
                        }
                    }
                }
            }
        }

        public void DrawCenterString(in Point location, Brush fillBrush, string text, in Font font, in TextAlignment textAlignment = TextAlignment.Left, in float maxWidth = float.MaxValue, in TextDecoration decoration = default,
            in float maxHeight = float.MaxValue,
            in Stroke stroke = default,
            Brush strokeBrush = null)
        {
            InitializeBrush(fillBrush);
            PaintWrapper paintWrapper = default;
            if (strokeBrush != null && stroke.Width > 0)
            {
                InitializeBrush(strokeBrush);
                paintWrapper = CreatePaint(strokeBrush, stroke, font);
            }
            var drawline = decoration.Stroke.Width > 0 && decoration.Brush != null && decoration.Location != TextDecorationLocation.None;
            List<LinePosition> linePositions = null;
            if (drawline)
            {
                linePositions = new List<LinePosition>();
            }
            using (var paint = CreatePaint(fillBrush, font))
            {
                paint.Paint.LcdRenderText = drawingFactory.ClearType;
                paint.Paint.TextEncoding = SKTextEncoding.Utf16;
                paint.Paint.IsStroke = false;
                if (text.Length == 1 || (text.Length == 2 && char.IsSurrogate(text[0])))
                {
                    var w = 0f;
                    var x = location.X;
                    if (textAlignment != TextAlignment.Left && maxWidth != float.MaxValue || drawline)
                    {
                        w = paint.Paint.MeasureString(text);
                        if (textAlignment != TextAlignment.Left && maxWidth != float.MaxValue)
                        {
                            if (textAlignment == TextAlignment.Right)
                            {
                                x = x + (maxWidth - w);
                            }
                            else if (textAlignment == TextAlignment.Center)
                            {
                                x = x + (maxWidth - w) / 2;
                            }
                        }
                    }
                    if (strokeBrush != null && stroke.Width > 0)
                    {
                        canvas.DrawTextPath(paint.Paint, paintWrapper.Paint, text, x, location.Y - paintWrapper.Paint.FontMetrics.Ascent);
                    }
                    else
                    {
                        //canvas.DrawText(text, x, location.Y - paint.Paint.FontMetrics.Ascent, paint.Paint);
                        canvas.DrawText(paint.Paint, text, x, location.Y - paint.Paint.FontMetrics.Ascent);
                    }

                    if (drawline)
                    {
                        if (text == " ")
                        {
                            w += 2;
                        }
                        if (decoration.Location.HasFlag(TextDecorationLocation.Underline))
                        {
                            linePositions.Add(new LinePosition { X = x, Y = location.Y + paint.Paint.FontSpacing, Width = w });
                        }
                        if (decoration.Location.HasFlag(TextDecorationLocation.OverLine))
                        {
                            linePositions.Add(new LinePosition { X = x, Y = location.Y, Width = w });
                        }
                        if (decoration.Location.HasFlag(TextDecorationLocation.Strikethrough))
                        {
                            linePositions.Add(new LinePosition { X = x, Y = location.Y + paint.Paint.FontSpacing / 2, Width = w });
                        }
                    }
                }
                else
                {

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
                    lines = newText.Split('\n');
                    var ws = paint.Paint.MeasureAllChar(lines[0]);

                    var wsEnd = new List<(string, float)>();
                    for (int i = ws.Count - 1; i >= 0; i--)
                    {
                        wsEnd.Add(ws[i]);
                    }

                    var lenStart = ws.BreakText(0, (float)Math.Ceiling(maxWidth / 2));
                    var lenEnd = wsEnd.BreakText(0, (float)Math.Ceiling(maxWidth / 2));
                    if (lenStart.Item1 + lenEnd.Item1 < ws.Count)
                    {
                        //lenEnd = (lenEnd.Item1, lenEnd.Item2, newText.Substring(newText.Length - lenEnd.Item1, lenEnd.Item1));
                        lenEnd.Item3 = newText.Substring(newText.Length - lenEnd.Item1, lenEnd.Item1);
                        if (lenEnd.Item3.Length > 2)
                        {
                            lenEnd.Item3 = lenEnd.Item3.Substring(1, lenEnd.Item3.Length - 1);
                        }
                        //if (lenStart.Item3.Length > 2)
                        //{
                        //    lenStart.Item3 = lenStart.Item3.Substring(0, lenStart.Item3.Length - 1);
                        //}
                        newText = lenStart.Item3 + "..." + lenEnd.Item3;
                    }


                    var w = 0f;
                    var x = location.X;
                    if (textAlignment != TextAlignment.Left && maxWidth != float.MaxValue || drawline)
                    {
                        w = paint.Paint.MeasureString(newText);
                        if (textAlignment != TextAlignment.Left && maxWidth != float.MaxValue)
                        {
                            if (textAlignment == TextAlignment.Right)
                            {
                                x = x + (maxWidth - w);
                            }
                            else if (textAlignment == TextAlignment.Center)
                            {
                                x = x + (maxWidth - w) / 2;
                            }
                        }
                    }
                    if (strokeBrush != null && stroke.Width > 0)
                    {
                        canvas.DrawTextPath(paint.Paint, paintWrapper.Paint, newText, x, location.Y - paintWrapper.Paint.FontMetrics.Ascent);
                    }
                    else
                    {
                        //canvas.DrawText(text, x, location.Y - paint.Paint.FontMetrics.Ascent, paint.Paint);
                        canvas.DrawText(paint.Paint, newText, x, location.Y - paint.Paint.FontMetrics.Ascent);
                    }

                    if (drawline)
                    {
                        if (newText == " ")
                        {
                            w += 2;
                        }
                        if (decoration.Location.HasFlag(TextDecorationLocation.Underline))
                        {
                            linePositions.Add(new LinePosition { X = x, Y = location.Y + paint.Paint.FontSpacing, Width = w });
                        }
                        if (decoration.Location.HasFlag(TextDecorationLocation.OverLine))
                        {
                            linePositions.Add(new LinePosition { X = x, Y = location.Y, Width = w });
                        }
                        if (decoration.Location.HasFlag(TextDecorationLocation.Strikethrough))
                        {
                            linePositions.Add(new LinePosition { X = x, Y = location.Y + paint.Paint.FontSpacing / 2, Width = w });
                        }
                    }
                }
            }
            paintWrapper?.Dispose();
            if (drawline)
            {
                //using (var brush = decoration.Brush)
                {
                    InitializeBrush(decoration.Brush);
                    using (var paint = CreatePaint(decoration.Brush, decoration.Stroke))
                    {
                        foreach (var item in linePositions)
                        {
                            canvas.DrawLine(item.X, item.Y, item.X + item.Width, item.Y, paint.Paint);
                        }
                    }
                }
            }
        }

        public override void FillEllipse(Brush fillBrush, in Point center, in float radiusX, in float radiusY)
        {
            InitializeBrush(fillBrush);
            using (var paint = CreatePaint(fillBrush))
            {
                canvas.DrawOval(center.X, center.Y, radiusX, radiusY, paint.Paint);
            }
        }

        public override void FillGeometry(Brush fillBrush, Geometry geometry)
        {
            InitializeBrush(fillBrush);
            using (var paint = CreatePaint(fillBrush))
            {
                canvas.DrawRegion((geometry.GeometryImpl as SkiaPathGeometry).SKRegion, paint.Paint);
            }
        }

        //public override void DrawGeometry(Brush strokeBrush, in Stroke stroke, Geometry geometry)
        //{
        //    InitializeBrush(strokeBrush);
        //    using (var paint = CreatePaint(strokeBrush, stroke))
        //    {
        //        canvas.DrawRegion((geometry.GeometryImpl as SkiaPathGeometry).SKRegion, paint.Paint);
        //    }
        //}

        public override void FillPath(Brush fillBrush, PathGeometry path)
        {
            InitializeBrush(fillBrush);
            using (var paint = CreatePaint(fillBrush))
            {
                canvas.DrawPath((path.PathIml as SkiaPath).SKPath, paint.Paint);
            }
        }

        public override void FillRectangle(Brush fillBrush, Rect rect)
        {
            InitializeBrush(fillBrush);
            using (var paint = CreatePaint(fillBrush))
            {
                canvas.DrawRect(rect.ToSKRect(), paint.Paint);
            }
        }

        //Stack<Rect> clips = new Stack<Rect>();
        //Rect currentRect;
        public override void PushClip(Rect clip)
        {
            canvas.Save();
            canvas.ClipRect(clip.ToSKRect());
            //clips.Push(currentRect);
            //currentRect = clip;
        }

        public override void PopClip()
        {
            canvas.Restore();
            //if (clips.Count > 0)
            //{
            //    var c = clips.Pop();
            //    //canvas.ClipRect(c.ToSKRect());
            //    currentRect = c;
            //}
        }

        protected override IDisposable CreateLinearGradientBrush(GradientStop[] bcs, in Point start, in Point end, in Matrix matrix)
        {
            bcs[0].Position = 0;
            var shader = SKShader.CreateLinearGradient(
                                                          start.ToSKPoint(),
                                                          end.ToSKPoint(),
                                                          bcs.Select(a => a.Color.ToSKColor()).ToArray(),
                                                          bcs.Select(a => a.Position).ToArray(),
                                                          SKShaderTileMode.Clamp,
                                                          matrix.ToMatrix()
                                                      );
            return shader;
        }

        protected override IDisposable CreateRadialGradientBrush(in Point center, in float radius, GradientStop[] bcs, in Matrix matrix)
        {
            bcs[0].Position = 0;
            var shader = SKShader.CreateRadialGradient(
                                                          center.ToSKPoint(),
                                                          radius,
                                                          bcs.Select(a => a.Color.ToSKColor()).ToArray(),
                                                          bcs.Select(a => a.Position).ToArray(),
                                                          SKShaderTileMode.Clamp,
                                                          matrix.ToMatrix()
                                                      );
            return shader;
        }

        protected override IDisposable CreateSolidBrush(Color color)
        {
            return SKShader.CreateColor(color.ToSKColor());
        }

        protected override IDisposable CreateTextureBrush(Image image, in WrapMode wrapMode, in Matrix matrix)
        {
            SKBitmap bitmap;
            if (image.ImageImpl is SkiaBitmap bmp)
            {
                bitmap = bmp.Bitmap;
            }
            else if (image.ImageImpl is SkiaImage img)
            {
                bitmap = img.Image;
            }
            else
            {
                throw new Exception("无效图片");
            }
            if (wrapMode == WrapMode.Clamp)
            {
                using (SKBitmap bitmap1 = new SKBitmap(bitmap.Width + 1, bitmap.Height + 1))
                {
                    using (SKCanvas canvas = new SKCanvas(bitmap1))
                    {
                        canvas.Clear();
                        canvas.DrawBitmap(bitmap, 0, 0);
                        return SKShader.CreateBitmap(bitmap1, SKShaderTileMode.Clamp, SKShaderTileMode.Clamp, matrix.ToMatrix());
                    }
                }
            }
            using (SKImage im = SKImage.FromBitmap(bitmap))
            {
                return im.ToShader(wrapMode == WrapMode.Clamp ? SKShaderTileMode.Clamp : SKShaderTileMode.Repeat, wrapMode == WrapMode.Clamp ? SKShaderTileMode.Clamp : SKShaderTileMode.Repeat, matrix.ToMatrix());
            }

        }
    }

    struct LinePosition
    {
        public float X;
        public float Y;
        public float Width;
    }
    /// <summary>
    /// Skia paint wrapper.
    /// </summary>
    internal class PaintWrapper : IDisposable
    {
        public readonly SKPaint Paint;

        public bool isUsing;
        private IDisposable _disposable1;

        public PaintWrapper(SKPaint paint)
        {
            Paint = paint;
            isUsing = true;
            _disposable1 = null;
        }


        public void AddDisposable(IDisposable disposable)
        {
            _disposable1 = disposable;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            //Paint?.Dispose();
            _disposable1?.Dispose();
            _disposable1 = null;
            Paint?.Reset();
            isUsing = false;
        }
    }
}
