using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkiaSharp;
using CPF.Drawing;
using System.Globalization;

namespace CPF.Skia
{
    public static class Extension
    {
        static Extension()
        {
            SKFontManager fontManager = SKFontManager.Default;
            var n = CultureInfo.InstalledUICulture.Name;
            var temp = n.Split('-');
            foreach (var item in temp)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    var f = fontManager.FontFamilies.FirstOrDefault(a => a.EndsWith(" " + item.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (f != null)
                    {
                        defaultFont = f;
                        break;
                    }
                }
            }
            if (defaultFont == null)
            {
                defaultFont = fontManager.FontFamilies.FirstOrDefault(a => a.EndsWith(" mono", StringComparison.OrdinalIgnoreCase));
            }
            if (defaultFont == null)
            {
                defaultFont = fontManager.FontFamilies.FirstOrDefault();
            }
        }
        static string defaultFont;

        private static string GetFontName(SKFontManager fontManager)
        {
            //var f = fontManager.FontFamilies.FirstOrDefault(a => a.EndsWith(" cn", StringComparison.OrdinalIgnoreCase));
            //if (f == null)
            //{
            //    f = fontManager.FontFamilies.FirstOrDefault(a => a.EndsWith(" zh", StringComparison.OrdinalIgnoreCase));
            //}
            //if (f == null)
            //{
            //    f = fontManager.FontFamilies.FirstOrDefault(a => a.EndsWith(" mono", StringComparison.OrdinalIgnoreCase));
            //}
            //if (f == null)
            //{
            //    f = fontManager.FontFamilies.FirstOrDefault();
            //}
            //return f;
            return defaultFont;
        }

        static string[] bcp47 = new string[] { "en", "zh", "ja", "kr", "enGB", "sw" };
        public static void DrawText(this SKCanvas canvas, SKPaint paint, string text, float x, float y)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var fontManager = SKFontManager.Default;
                var t = paint.Typeface;
                var family = t.FamilyName;
                var si = new StringInfo(text);
                for (int i = 0; i < si.LengthInTextElements; i++)
                {
                    var str = si.SubstringByTextElements(i, 1);
                    if (str.Length != 2)
                    {
                        if (str != "\r")
                        {
                            if (t.CountGlyphs(str) == 0)//如果当前字体不支持该字符
                            {
                                using (var font = fontManager.MatchCharacter(family, t.FontWeight, t.FontWidth, t.FontSlant, bcp47, str[0]))//检索系统默认支持的字体
                                {
                                    SKTypeface f = null;
                                    if (font == null)
                                    {
                                        string fn = GetFontName(fontManager);
                                        if (fn != null)
                                        {
                                            f = fontManager.MatchFamily(fn, t.FontStyle);
                                            if (f != null)
                                            {
                                                paint.Typeface = f;
                                            }
                                            else
                                            {
                                                paint.Typeface = t;
                                            }
                                        }
                                        else
                                        {
                                            paint.Typeface = t;
                                        }
                                    }
                                    else
                                    {
                                        paint.Typeface = font;
                                    }
                                    canvas.DrawText(str, x, y, paint);
                                    x += paint.MeasureText(str);
                                    if (f != null)
                                    {
                                        f.Dispose();
                                    }
                                }
                            }
                            else
                            {
                                paint.Typeface = t;
                                canvas.DrawText(str, x, y, paint);
                                x += paint.MeasureText(str);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        using (var font = fontManager.MatchCharacter(family, t.FontWeight, t.FontWidth, t.FontSlant, bcp47, StringUtilities.GetUnicodeCharacterCode(str, SKTextEncoding.Utf32)))
                        {
                            if (font == null)
                            {
                                paint.Typeface = t;
                            }
                            else
                            {
                                paint.Typeface = font;
                            }
                            canvas.DrawText(str, x, y, paint);
                            x += paint.MeasureText(str);
                        }
                    }
                }
                paint.Typeface = t;
            }
        }


        public static float MeasureString(this SKPaint paint, string text)
        {
            float w = 0;
            if (!string.IsNullOrEmpty(text))
            {
                var fontManager = SKFontManager.Default;
                var t = paint.Typeface;
                var family = t.FamilyName;
                var si = new StringInfo(text);
                for (int i = 0; i < si.LengthInTextElements; i++)
                {
                    var str = si.SubstringByTextElements(i, 1);
                    if (str.Length != 2)
                    {
                        if (str != "\r" && str != "\n")
                        {
                            if (str == " ")
                            {
                                w += paint.FontSpacing / 2;
                            }
                            else// if (str=="\t")
                            {
                                if (t.CountGlyphs(str) == 0)
                                {
                                    using (var font = fontManager.MatchCharacter(family, t.FontWeight, t.FontWidth, t.FontSlant, bcp47, str[0]))
                                    {
                                        SKTypeface f = null;
                                        if (font == null)
                                        {
                                            string fn = GetFontName(fontManager);
                                            if (fn != null)
                                            {
                                                f = fontManager.MatchFamily(fn, t.FontStyle);
                                                if (f != null)
                                                {
                                                    paint.Typeface = f;
                                                }
                                                else
                                                {
                                                    paint.Typeface = t;
                                                }
                                            }
                                            else
                                            {
                                                paint.Typeface = t;
                                            }
                                        }
                                        else
                                        {
                                            paint.Typeface = font;
                                        }
                                        w += paint.MeasureText(str);
                                        if (f != null)
                                        {
                                            f.Dispose();
                                        }
                                    }
                                }
                                else
                                {
                                    paint.Typeface = t;
                                    w += paint.MeasureText(str);
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        using (var font = fontManager.MatchCharacter(family, t.FontWeight, t.FontWidth, t.FontSlant, bcp47, StringUtilities.GetUnicodeCharacterCode(str, SKTextEncoding.Utf32)))
                        {
                            if (font == null)
                            {
                                paint.Typeface = t;
                            }
                            else
                            {
                                paint.Typeface = font;
                            }
                            w += paint.MeasureText(str);
                        }
                    }
                }
                paint.Typeface = t;
            }
            return w;
        }
        /// <summary>
        /// 计算每个字符的宽度，不能有换行符
        /// </summary>
        /// <param name="paint"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IList<(string, float)> MeasureAllChar(this SKPaint paint, string text)
        {
            List<(string, float)> list = new List<(string, float)>();
            if (!string.IsNullOrEmpty(text))
            {
                var fontManager = SKFontManager.Default;
                var t = paint.Typeface;
                var family = t.FamilyName;
                var si = new StringInfo(text);
                for (int i = 0; i < si.LengthInTextElements; i++)
                {
                    var str = si.SubstringByTextElements(i, 1);
                    if (str.Length != 2)
                    {
                        if (str != "\r" && str != "\n")
                        {
                            if (str == " ")
                            {
                                //list.Add((str, paint.FontSpacing / 2));
                                list.Add((str, paint.MeasureText(str)));
                            }
                            else// if (str=="\t")
                            {
                                if (t.CountGlyphs(str) == 0)
                                {
                                    using (var font = fontManager.MatchCharacter(family, t.FontWeight, t.FontWidth, t.FontSlant, bcp47, str[0]))
                                    {
                                        SKTypeface f = null;
                                        if (font == null)
                                        {
                                            string fn = GetFontName(fontManager);
                                            if (fn != null)
                                            {
                                                f = fontManager.MatchFamily(fn, t.FontStyle);
                                                if (f != null)
                                                {
                                                    paint.Typeface = f;
                                                }
                                                else
                                                {
                                                    paint.Typeface = t;
                                                }
                                            }
                                            else
                                            {
                                                paint.Typeface = t;
                                            }
                                        }
                                        else
                                        {
                                            paint.Typeface = font;
                                        }
                                        list.Add((str, paint.MeasureText(str)));
                                        if (f != null)
                                        {
                                            f.Dispose();
                                        }
                                    }
                                }
                                else
                                {
                                    paint.Typeface = t;
                                    list.Add((str, paint.MeasureText(str)));
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        using (var font = fontManager.MatchCharacter(family, t.FontWeight, t.FontWidth, t.FontSlant, bcp47, StringUtilities.GetUnicodeCharacterCode(str, SKTextEncoding.Utf32)))
                        {
                            if (font == null)
                            {
                                paint.Typeface = t;
                            }
                            else
                            {
                                paint.Typeface = font;
                            }
                            list.Add((str, paint.MeasureText(str)));
                        }
                    }
                }
                paint.Typeface = t;
            }

            return list;
        }

        internal static (int, float, string) BreakText(this IList<(string, float)> list, int start, float maxWidth, bool getText = true)
        {
            float w = 0;
            int count = 0;
            for (int i = start; i < list.Count; i++)
            {
                count++;
                w += list[i].Item2;
                if (w > maxWidth)
                {
                    if (i != start)
                    {
                        w -= list[i].Item2;
                        count--;
                    }
                    break;
                }
            }
            string text = string.Empty;
            if (count > 0 && getText)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = start; i < start + count; i++)
                {
                    sb.Append(list[i].Item1);
                }
                text = sb.ToString();
            }
            return (count, w, text);
        }

        public static void DrawTextPath(this SKCanvas canvas, SKPaint paint, SKPaint border, string text, float x, float y)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var fontManager = SKFontManager.Default;
                var t = paint.Typeface;
                var family = t.FamilyName;
                var si = new StringInfo(text);
                for (int i = 0; i < si.LengthInTextElements; i++)
                {
                    var str = si.SubstringByTextElements(i, 1);
                    if (str.Length != 2)
                    {
                        if (str != "\r")
                        {
                            if (t.CountGlyphs(str) == 0)
                            {
                                using (var font = fontManager.MatchCharacter(family, t.FontWeight, t.FontWidth, t.FontSlant, bcp47, str[0]))
                                {
                                    SKTypeface f = null;
                                    if (font == null)
                                    {
                                        string fn = GetFontName(fontManager);
                                        if (fn != null)
                                        {
                                            f = fontManager.MatchFamily(fn, t.FontStyle);
                                            if (f != null)
                                            {
                                                paint.Typeface = f;
                                            }
                                            else
                                            {
                                                paint.Typeface = t;
                                            }
                                        }
                                        else
                                        {
                                            paint.Typeface = t;
                                        }
                                    }
                                    else
                                    {
                                        paint.Typeface = font;
                                    }
                                    using (var path = paint.GetTextPath(str, x, y))
                                    {
                                        canvas.DrawPath(path, border);
                                        canvas.DrawPath(path, paint);
                                    }
                                    x += paint.MeasureText(str);
                                    if (f != null)
                                    {
                                        f.Dispose();
                                    }
                                }
                            }
                            else
                            {
                                paint.Typeface = t;
                                using (var path = paint.GetTextPath(str, x, y))
                                {
                                    canvas.DrawPath(path, border);
                                    canvas.DrawPath(path, paint);
                                }
                                x += paint.MeasureText(str);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        using (var font = fontManager.MatchCharacter(family, t.FontWeight, t.FontWidth, t.FontSlant, bcp47, StringUtilities.GetUnicodeCharacterCode(str, SKTextEncoding.Utf32)))
                        {
                            if (font == null)
                            {
                                paint.Typeface = t;
                            }
                            else
                            {
                                paint.Typeface = font;
                            }
                            using (var path = paint.GetTextPath(str, x, y))
                            {
                                canvas.DrawPath(path, border);
                                canvas.DrawPath(path, paint);
                            }
                            x += paint.MeasureText(str);
                        }
                    }
                }
                paint.Typeface = t;
            }
        }

        public static Matrix ToMatrix(this SKMatrix matrix)
        {
            return new Matrix(matrix.ScaleX, matrix.SkewY, matrix.SkewX, matrix.ScaleY, matrix.TransX, matrix.TransY)
            //{ Persp0 = matrix.Persp0, Persp1 = matrix.Persp1, Persp2 = matrix.Persp2 }
            ;
        }
        public static SKMatrix ToMatrix(this Matrix matrix)
        {
            //return new SKMatrix(matrix.M11, matrix.M21, matrix.OffsetX, matrix.M12, matrix.M22, matrix.OffsetY, 0, 0, 1);
            return new SKMatrix()
            {
                ScaleX = matrix.M11,
                SkewX = matrix.M21,
                TransX = matrix.OffsetX,
                SkewY = matrix.M12,
                ScaleY = matrix.M22,
                TransY = matrix.OffsetY,
                Persp2 =1// matrix.Persp2,
                //Persp0 = matrix.Persp0,
                //Persp1 = matrix.Persp1
            };
        }

        public static SKPoint ToSKPoint(this Point p)
        {
            return new SKPoint((float)p.X, (float)p.Y);
        }

        public static SKRect ToSKRect(this Rect r)
        {
            return new SKRect((float)r.X, (float)r.Y, (float)r.Right, (float)r.Bottom);
        }

        public static Rect ToRect(this SKRect r)
        {
            return new Rect(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
        }

        public static SKColor ToSKColor(this Color c)
        {
            return new SKColor(c.R, c.G, c.B, c.A);
        }

        //public static SKColorType ToSkColorType(this PixelFormat fmt)
        //{
        //    if (fmt == PixelFormat.Rgb565)
        //        return SKColorType.Rgb565;
        //    if (fmt == PixelFormat.Bgra8888)
        //        return SKColorType.Bgra8888;
        //    if (fmt == PixelFormat.Rgba8888)
        //        return SKColorType.Rgba8888;
        //    throw new ArgumentException("Unknown pixel format: " + fmt);
        //}

        //public static PixelFormat ToPixelFormat(this SKColorType fmt)
        //{
        //    if (fmt == SKColorType.Rgb565)
        //        return PixelFormat.Rgb565;
        //    if (fmt == SKColorType.Bgra8888)
        //        return PixelFormat.Bgra8888;
        //    if (fmt == SKColorType.Rgba8888)
        //        return PixelFormat.Rgba8888;
        //    throw new ArgumentException("Unknown pixel format: " + fmt);
        //}

        //public static SKShaderTileMode ToSKShaderTileMode(this GradientSpreadMethod m)
        //{
        //    switch (m)
        //    {
        //        default:
        //        case GradientSpreadMethod.Pad: return SKShaderTileMode.Clamp;
        //        case GradientSpreadMethod.Reflect: return SKShaderTileMode.Mirror;
        //        case GradientSpreadMethod.Repeat: return SKShaderTileMode.Repeat;
        //    }
        //}

        //public static SKTextAlign ToSKTextAlign(this TextAlignment a)
        //{
        //    switch (a)
        //    {
        //        default:
        //        case TextAlignment.Left: return SKTextAlign.Left;
        //        case TextAlignment.Center: return SKTextAlign.Center;
        //        case TextAlignment.Right: return SKTextAlign.Right;
        //    }
        //}

        //public static TextAlignment ToTextAlignment(this SKTextAlign a)
        //{
        //    switch (a)
        //    {
        //        default:
        //        case SKTextAlign.Left: return TextAlignment.Left;
        //        case SKTextAlign.Center: return TextAlignment.Center;
        //        case SKTextAlign.Right: return TextAlignment.Right;
        //    }
        //}
    }
}
