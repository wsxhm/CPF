using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using CPF.Platform;
using SkiaSharp;
using System.Runtime.InteropServices;
using CPF.Drawing;

namespace CPF.Skia
{
    public class SkiaDrawingFactory : DrawingFactory
    {
        /// <summary>
        /// 可以让字体显示更清晰，不过在透明背景下会有黑边
        /// </summary>
        public bool ClearType { get; set; }
        /// <summary>
        /// 尝试启用GPU加速，普通的界面不建议开启GPU，因为使用GPU加速后，普通界面加速效果不明显，但是内存占用会翻倍，尤其是使用GIF的时候，因为会缓存每一帧。
        /// 需要有正确的显卡驱动配置，依赖OpenGL。Windows上需要开启了Dwm桌面混合（win7不能使用basic主题）的情况下才GPU加速效果。如果出现花屏或者界面不显示情况，请关闭硬件加速。
        /// </summary>
        public override bool UseGPU { get; set; }

        private static ConcurrentDictionary<FontKey, FontWrapper> _fontFamilies =
            new ConcurrentDictionary<FontKey, FontWrapper>();
        static ConcurrentDictionary<string, SKTypeface> _fonts =
            new ConcurrentDictionary<string, SKTypeface>();

        public override IBitmapImpl CreateBitmap(int w, int h)
        {
            return new SkiaBitmap(new SKBitmap(w, h));
        }

        public override IBitmapImpl CreateBitmap(int w, int h, int pitch, PixelFormat pixelFormat, IntPtr data)
        {
            SKColorType sKColorType = SKColorType.Unknown;
            SKAlphaType sKAlphaType = SKAlphaType.Unpremul;
            switch (pixelFormat)
            {
                case PixelFormat.Undefined:
                    break;
                case PixelFormat.PRgba:
                    sKAlphaType = SKAlphaType.Premul;
                    sKColorType = SKColorType.Rgba8888;
                    break;
                case PixelFormat.Rgba:
                    sKColorType = SKColorType.Rgba8888;
                    break;
                case PixelFormat.PBgra:
                    sKAlphaType = SKAlphaType.Premul;
                    sKColorType = SKColorType.Bgra8888;
                    break;
                case PixelFormat.Bgra:
                    sKColorType = SKColorType.Bgra8888;
                    break;
                case PixelFormat.Rgb565:
                    sKColorType = SKColorType.Rgb565;
                    break;
                default:
                    break;
            }
            SKImageInfo info = new SKImageInfo { Width = w, Height = h, AlphaType = sKAlphaType, ColorType = sKColorType };
            var bitmap = new SKBitmap(info, pitch);
            bitmap.SetPixels(data);
            return new SkiaBitmap(bitmap);
        }

        public override IBitmapImpl CreateBitmap(Stream stream)
        {
            return new SkiaBitmap(SKBitmap.Decode(stream));
        }

        public override IBitmapImpl CreateBitmap(Image img)
        {
            if (img.ImageImpl is SkiaBitmap b)
            {
                return new SkiaBitmap(b.Bitmap.Copy());
            }
            else if (img.ImageImpl is SkiaImage i)
            {
                return new SkiaBitmap(i.Image.Copy());
            }
            return null;
        }

        public override DrawingContext CreateDrawingContext(Bitmap bitmap)
        {
            return new SkiaDrawingContext(bitmap, this);
        }

        public override DrawingContext CreateDrawingContext(IRenderTarget target)
        {
            return new SkiaDrawingContext(target, this);
        }
        //public override DrawingContext CreateDrawingContext<T>(T Canvas)
        //{
        //    return new SkiaDrawingContext((SKCanvas)(object)Canvas, this);
        //}

        public override IDisposable CreateFont(string fontFamily, float fontSize, FontStyles fontStyle)
        {
            var weight = SKFontStyleWeight.Normal;
            if (fontStyle.HasFlag(FontStyles.Bold))
            {
                weight = SKFontStyleWeight.Bold;
            }
            var slant = SKFontStyleSlant.Upright;
            if (fontStyle.HasFlag(FontStyles.Italic))
            {
                slant = SKFontStyleSlant.Italic;
            }
            if (!_fontFamilies.TryGetValue(new FontKey(weight, slant, fontFamily), out FontWrapper fontWrap))
            {
                if (_fonts.TryGetValue(fontFamily, out var font))
                {
#if Net4
                    SKTypefaceStyle style= SKTypefaceStyle.Normal;
                    switch (fontStyle)
                    {
                        case FontStyles.Bold:
                            style = SKTypefaceStyle.Bold;
                            break;
                        case FontStyles.Italic:
                            style = SKTypefaceStyle.Italic;
                            break;
                        default:
                            break;
                    }
                    fontWrap = new FontWrapper { SKTypeface = SKTypeface.FromTypeface(font, style) };
#else
                    SKFontStyle style = new SKFontStyle(weight, SKFontStyleWidth.Normal, slant);
                    if (Application.OperatingSystem == OperatingSystemType.Linux || Application.OperatingSystem == OperatingSystemType.OSX)
                    {
                        fontWrap = new FontWrapper { SKTypeface = font };
                        //Console.WriteLine("Skia的BUG，内嵌字体在Linux和Mac下不能使用字体样式选择");
                    }
                    else
                    {
                        fontWrap = new FontWrapper { SKTypeface = SKFontManager.Default.MatchTypeface(font, style) };
                    }
#endif
                    _fontFamilies.TryAdd(new FontKey(weight, slant, fontFamily), fontWrap);
                }
                else
                {
                    fontWrap = new FontWrapper { SKTypeface = SKTypeface.FromFamilyName(fontFamily, weight, SKFontStyleWidth.Normal, slant) };

                    if (fontWrap.SKTypeface == null)
                    {
                        fontWrap.SKTypeface = SKTypeface.Default;
                    }
                    //SKFontStyle style = new SKFontStyle(weight, SKFontStyleWidth.Normal, slant);
                    //fontWrap = new FontWrapper { SKTypeface = SKFontManager.Default.MatchFamily(fontFamily, style) };
                    _fontFamilies.TryAdd(new FontKey(weight, slant, fontFamily), fontWrap);
                }
            }
            return fontWrap;
        }

        public override IGeometryImpl CreateGeometry(CPF.Drawing.PathGeometry path)
        {
            return new SkiaPathGeometry(path);
        }

        public override IPathImpl CreatePath()
        {
            return new SkiaPath();
        }
        public override IPathImpl CreatePath(in Font font, string text)
        {
            return new SkiaPath(font, text);
        }

        public override void Dispose()
        {
            foreach (var item in _fonts)
            {
                item.Value.Dispose();
            }
            _fonts.Clear();
        }

        public override IImageImpl ImageFromFile(string path)
        {
            return new SkiaImage(path);
        }

        public override IImageImpl ImageFromStream(Stream stream)
        {
            return new SkiaImage(stream);
        }

        public override void LoadFont(Stream stream, string name)
        {
            var f = SKTypeface.FromStream(stream);
            if (string.IsNullOrWhiteSpace(name))
            {
                name = f.FamilyName;
            }
            if (!_fonts.TryGetValue(name, out var value))
            {
                System.Diagnostics.Debug.WriteLine("加载字体：" + f.FamilyName);
                Console.WriteLine("加载字体：" + f.FamilyName);
                _fonts.TryAdd(name, f);
            }
            else
            {
                f.Dispose();
            }
        }

        public override Size MeasureString(string str, Font font)
        {
            using (SKPaint paint = new SKPaint())
            {
                paint.TextEncoding = SKTextEncoding.Utf16;
                //paint.IsStroke = false;
                //paint.LcdRenderText = true;
                //paint.SubpixelText = true;
                paint.IsAntialias = true;
                paint.Typeface = (font.AdapterFont as FontWrapper).SKTypeface;
                paint.TextSize = font.FontSize;
                if (str.Length < 3)
                {
                    var width = paint.MeasureString(str);
                    return new Size(width, paint.FontSpacing + 1);
                }
                else
                {
                    var lines = str.Split('\n');
                    float width = 0;
                    float heght = 0;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i].Trim('\r');
                        if (i == lines.Length - 1 && line == "")
                        {
                            break;
                        }
                        width = Math.Max(width, paint.MeasureString(line));
                        heght += paint.FontSpacing;
                    }
                    return new Size(width, heght);
                }
            }
        }

        public override Size MeasureString(string str, Font font, float maxWidth)
        {
            using (SKPaint paint = new SKPaint())
            {
                paint.TextEncoding = SKTextEncoding.Utf16;
                //paint.IsStroke = false;
                //paint.LcdRenderText = true;
                //paint.SubpixelText = true;
                paint.IsAntialias = true;
                paint.Typeface = (font.AdapterFont as FontWrapper).SKTypeface;
                paint.TextSize = font.FontSize;
                if (str.Length == 1 || (str.Length == 2 && char.IsSurrogate(str[0])))
                {
                    return new Size(paint.MeasureString(str), paint.FontSpacing);
                }
                else
                {
                    var lines = str.Split('\n');
                    float width = 0;
                    float heght = 1;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i].Trim('\r');
                        if (i == lines.Length - 1 && line == "")
                        {
                            break;
                        }
                        //var text = line;
                        var ws = paint.MeasureAllChar(line);
                        var start = 0;
                        while (true)
                        {
                            //var len = Math.Max(1, (int)paint.BreakText(text, (float)Math.Ceiling(maxWidth)));
                            var len = ws.BreakText(start, (float)Math.Ceiling(maxWidth), false);
                            if (start + len.Item1 <= ws.Count)
                            {
                                //text = line.Substring(start, len);
                                start += len.Item1;
                            }
                            width = Math.Max(width, len.Item2);
                            heght += paint.FontSpacing;
                            if (start <= ws.Count - 1)
                            {
                                //text = line.Substring(start);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    return new Size(width, heght);
                }
            }
        }

        public override float GetLineHeight(in Font font)
        {
            using (SKPaint paint = new SKPaint())
            {
                paint.TextEncoding = SKTextEncoding.Utf16;
                //paint.IsStroke = false;
                //paint.LcdRenderText = true;
                //paint.SubpixelText = true;
                paint.IsAntialias = true;
                paint.Typeface = (font.AdapterFont as FontWrapper).SKTypeface;
                paint.TextSize = font.FontSize;

                return paint.FontSpacing;
            }
        }

        public override float GetAscent(in Font font)
        {
            using (SKPaint paint = new SKPaint())
            {
                paint.TextEncoding = SKTextEncoding.Utf16;
                //paint.IsStroke = false;
                //paint.LcdRenderText = true;
                //paint.SubpixelText = true;
                paint.IsAntialias = true;
                paint.Typeface = (font.AdapterFont as FontWrapper).SKTypeface;
                paint.TextSize = font.FontSize;

                return -paint.FontMetrics.Ascent;
            }
        }

        public override float GetDescent(in Font font)
        {
            using (SKPaint paint = new SKPaint())
            {
                paint.TextEncoding = SKTextEncoding.Utf16;
                //paint.IsStroke = false;
                //paint.LcdRenderText = true;
                //paint.SubpixelText = true;
                paint.IsAntialias = true;
                paint.Typeface = (font.AdapterFont as FontWrapper).SKTypeface;
                paint.TextSize = font.FontSize;

                return paint.FontMetrics.Descent;
            }
        }
    }

    class FontWrapper : IDisposable
    {
        public SKTypeface SKTypeface;
        public void Dispose()
        { }
    }
    struct FontKey
    {
        public readonly SKFontStyleSlant Slant;
        public readonly SKFontStyleWeight Weight;
        public readonly string Name;

        public FontKey(SKFontStyleWeight weight, SKFontStyleSlant slant, string name)
        {
            Slant = slant;
            Weight = weight;
            Name = name;
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = (hash * 31) + (int)Slant;
            hash = (hash * 31) + (int)Weight;
            hash ^= Name.GetHashCode();
            return hash;
        }

        public override bool Equals(object other)
        {
            return other is FontKey key && this.Equals(key);
        }

        private bool Equals(FontKey other)
        {
            return Slant == other.Slant &&
                   Weight == other.Weight &&
                   Name == other.Name;
        }
    }
}
