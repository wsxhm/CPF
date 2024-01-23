#if Net4
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using System.Drawing;
using CPF.Drawing;

namespace CPF.GDIPlus
{
    public class GDIPlusDrawingFactory : DrawingFactory
    {
        /// <summary>
        /// 可以让字体显示更清晰，不过在透明背景下会有黑边
        /// </summary>
        public bool ClearType { get; set; }
        /// <summary>
        /// GDI+不支持GPU加速
        /// </summary>
        public override bool UseGPU { get => false; set => throw new NotSupportedException("GDI+不支持GPU加速"); }

        public override IImageImpl ImageFromFile(string path)
        {
            var img = System.Drawing.Image.FromFile(path);
            if (img.RawFormat.Guid != System.Drawing.Imaging.ImageFormat.Gif.Guid)
            {
                var old = img;
                img = new System.Drawing.Bitmap(img);
                old.Dispose();
            }
            return new GDIPlusImage(img);
        }

        public override IImageImpl ImageFromStream(System.IO.Stream stream)
        {
            var mem = new MemoryStream();//播放gif的时候必须保留stream，所以拷贝一份
            int d;
            while ((d = stream.ReadByte()) > -1)
            {
                mem.WriteByte((byte)d);
            }
            var img = System.Drawing.Image.FromStream(mem);
            if (img.RawFormat.Guid != System.Drawing.Imaging.ImageFormat.Gif.Guid)
            {
                var old = img;
                img = new System.Drawing.Bitmap(img);
                old.Dispose();
                mem.Dispose();
            }
            return new GDIPlusImage(img);
        }

        //public override DrawingContext CreateDrawingContext(IntPtr hdc, Rect rect)
        //{
        //    return new GDIPlusDrawingContext(hdc, rect){ drawingFactory = this };
        //}
        public GDIPlusDrawingFactory()
        {
            g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            sf = System.Drawing.StringFormat.GenericTypographic;
        }

        internal System.Drawing.Graphics g;
        System.Drawing.StringFormat sf;
        public override CPF.Drawing.Size MeasureString(string str, CPF.Drawing.Font font)
        {
            //var size = System.Windows.Forms.TextRenderer.MeasureText(str, (System.Drawing.Font)font.AdapterFont, new System.Drawing.Size(int.MaxValue, int.MaxValue), System.Windows.Forms.TextFormatFlags.NoPadding);
            //return new Size(size.Width, size.Height);

            if (ClearType)
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            }
            else
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            }

            if (str == " ")
            {
                return new Drawing.Size(font.FontSize / 2, ((FontStruct)font.AdapterFont).Height);
            }
            var s = g.MeasureString(str, ((FontStruct)font.AdapterFont).Font, int.MaxValue, sf);
            if (str.EndsWith(" "))
            {
                for (int i = str.Length - 1; i >= 0; i--)
                {
                    if (str[i] != ' ')
                    {
                        break;
                    }
                    s.Width += font.FontSize / 2;
                }
            }
            return new CPF.Drawing.Size(s.Width, s.Height);
        }

        public override CPF.Drawing.Size MeasureString(string str, CPF.Drawing.Font font, float width)
        {
            if (g == null)
            {
                g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
                sf = System.Drawing.StringFormat.GenericTypographic;
            }
            int w = (int)Math.Ceiling(width);
            if (float.IsInfinity(width))
            {
                w = int.MaxValue;
            }
            if (str == " ")
            {
                return new Drawing.Size(font.FontSize / 2, ((FontStruct)font.AdapterFont).Height);
            }
            var s = g.MeasureString(str, ((FontStruct)font.AdapterFont).Font, w, sf);
            if (str.EndsWith(" "))
            {
                for (int i = str.Length - 1; i >= 0; i--)
                {
                    if (str[i] != ' ')
                    {
                        break;
                    }
                    s.Width += font.FontSize / 2;
                }
            }
            return new CPF.Drawing.Size(s.Width, s.Height);
        }
        public override void Dispose()
        {

        }

        public override IDisposable CreateFont(string fontFamily, float fontSize, FontStyles fontStyle)
        {
            var key = new FontKey { FontFamily = fontFamily, FontSize = fontSize, FontStyle = fontStyle };
            if (!fontStructs.TryGetValue(key, out var font))
            {
                if (fonts.TryGetValue(fontFamily, out var fontFamily1))
                {
                    font = new FontStruct(new System.Drawing.Font(fontFamily1, fontSize, fontStyle.ToFontStyle(), GraphicsUnit.Pixel));
                }
                else
                {
                    font = new FontStruct(new System.Drawing.Font(fontFamily, fontSize, fontStyle.ToFontStyle(), GraphicsUnit.Pixel));
                }

                fontStructs.TryAdd(key, font);
            }
            return font;
        }

        public override DrawingContext CreateDrawingContext(CPF.Drawing.Bitmap bitmap)
        {
            return new GDIPlusDrawingContext(System.Drawing.Graphics.FromImage(((GDIPlusBitmap)bitmap.BitmapImpl).Bitmap)) { drawingFactory = this };
        }

        public override DrawingContext CreateDrawingContext(IRenderTarget target)
        {
            if (target is HDCRenderTarget hDC)
            {
                return new GDIPlusDrawingContext(hDC.Hdc, new Rect(0, 0, target.Width, target.Height)) { drawingFactory = this };
            }
            //else if (target is GDIPlusGraphicsRenderTarget)
            //{
            //    return new GDIPlusDrawingContext((System.Drawing.Graphics)target.Target) { drawingFactory = this };
            //}
            return null;
        }

        public override IPathImpl CreatePath()
        {
            return new GDIPlusPath();
        }

        public override IPathImpl CreatePath(in Drawing.Font font, string text)
        {
            return new GDIPlusPath(font, text);
        }


        public override IBitmapImpl CreateBitmap(int w, int h)
        {
            return new GDIPlusBitmap(w, h);
        }

        public override IBitmapImpl CreateBitmap(int w, int h, int pitch, PixelFormat pixelFormat, IntPtr data)
        {
            return new GDIPlusBitmap(w, h, pitch, pixelFormat, data);
        }

        public override IBitmapImpl CreateBitmap(Stream stream)
        {
            return new GDIPlusBitmap(stream);
        }

        public override IBitmapImpl CreateBitmap(CPF.Drawing.Image img)
        {
            return new GDIPlusBitmap(img.ImageImpl as GDIPlusImage);
        }

        public override IGeometryImpl CreateGeometry(PathGeometry path)
        {
            return new GDIPlusPathGeometry(path);
        }

        static ConcurrentDictionary<string, FontFamily> fonts = new ConcurrentDictionary<string, FontFamily>();
        static ConcurrentDictionary<FontKey, FontStruct> fontStructs = new ConcurrentDictionary<FontKey, FontStruct>();
        public override void LoadFont(Stream stream, string name)
        {
            byte[] f = new byte[stream.Length];
            stream.Read(f, 0, (int)stream.Length);
            PrivateFontCollection privateFonts = new PrivateFontCollection();
            unsafe
            {
                fixed (byte* pFontData = f)
                {
                    privateFonts.AddMemoryFont((IntPtr)pFontData, f.Length);
                }
            }
            foreach (var item in privateFonts.Families)
            {
                System.Diagnostics.Debug.WriteLine("加载字体：" + item.Name);
                Console.WriteLine("加载字体：" + item.Name);
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = item.Name;
                }
                fonts.TryAdd(name, item);
            }
        }

        public override float GetLineHeight(in Drawing.Font font)
        {
            //var f = (FontStruct)font.AdapterFont;
            //return f.Height;
            var f = (FontStruct)font.AdapterFont;
            float emHeight = f.Font.FontFamily.GetEmHeight(f.Font.Style);
            var lineSpacing = f.Font.FontFamily.GetLineSpacing(f.Font.Style) * f.Font.Size / emHeight;
            return lineSpacing;
        }

        public override float GetAscent(in Drawing.Font font)
        {
            var f = (FontStruct)font.AdapterFont;
            float emHeight = f.Font.FontFamily.GetEmHeight(f.Font.Style);
            //var lineSpacing = f.Font.FontFamily.GetLineSpacing(f.Font.Style) * f.Font.Size / emHeight;
            float ascent = f.Font.FontFamily.GetCellAscent(f.Font.Style) * f.Font.Size / emHeight;
            return ascent;
        }

        public override float GetDescent(in Drawing.Font font)
        {
            var f = (FontStruct)font.AdapterFont;
            float emHeight = f.Font.FontFamily.GetEmHeight(f.Font.Style);
            float descent = f.Font.FontFamily.GetCellDescent(f.Font.Style) * f.Font.Size / emHeight;
            return descent;
        }

        ///// <summary>
        ///// 从canvas创建context失败返回null
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="Canvas"></param>
        ///// <returns></returns>
        //public override DrawingContext CreateDrawingContext<T>(T Canvas)
        //{
        //    //gdi不支持就返回null
        //    return null;
        //}




        //public override DrawingContext CreateDrawingContext(IntPtr hwnd)
        //{
        //    return new GDIPlusDrawingContext(hwnd);
        //}
    }

    class FontStruct : IDisposable
    {
        public FontStruct(System.Drawing.Font font)
        {
            Font = font;
            h = -1;
        }

        float h;
        public float Height
        {
            get
            {
                if (h == -1)
                {
                    h = Font.Height + 0.01f;//有些字体用行高计算字符尺寸的时候会有问题，只能加0.01
                }
                return h;
            }
        }


        public System.Drawing.Font Font;

        public void Dispose()
        {

        }
    }

    struct FontKey
    {
        public string FontFamily;

        public float FontSize;

        public FontStyles FontStyle;

        public override bool Equals(object obj)
        {
            if (obj is FontKey fontStruct)
            {
                return fontStruct.FontFamily == FontFamily && fontStruct.FontSize == FontSize && fontStruct.FontStyle == FontStyle;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return FontFamily.GetHashCode() ^ FontSize.GetHashCode() ^ FontStyle.GetHashCode();
        }
    }
}
#endif