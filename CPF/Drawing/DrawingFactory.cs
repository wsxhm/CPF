using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CPF.Drawing
{
    public abstract class DrawingFactory : IDisposable
    {
        public static DrawingFactory Default => CPF.Platform.Application.GetDrawingFactory();
        /// <summary>
        /// 创建图形
        /// </summary>
        /// <returns></returns>
        public abstract IGeometryImpl CreateGeometry(PathGeometry path);
        /// <summary>
        /// 创建路径
        /// </summary>
        /// <returns></returns>
        public abstract IPathImpl CreatePath();
        /// <summary>
        /// 读取文件图片
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract IImageImpl ImageFromFile(string path);
        /// <summary>
        /// 读取图片流
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public abstract IImageImpl ImageFromStream(Stream stream);

        /// <summary>
        /// 创建位图
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public abstract IBitmapImpl CreateBitmap(int w, int h);
        /// <summary>
        /// 创建位图
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="pitch"></param>
        /// <param name="pixelFormat"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract IBitmapImpl CreateBitmap(int w, int h, int pitch, PixelFormat pixelFormat, IntPtr data);
        /// <summary>
        /// 创建位图
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public abstract IBitmapImpl CreateBitmap(Stream stream);
        /// <summary>
        /// 创建位图
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public abstract IBitmapImpl CreateBitmap(Image img);
        ///// <summary>
        ///// 创建绘图上下文
        ///// </summary>
        ///// <param name="hdc"></param>
        ///// <param name="rect"></param>
        ///// <returns></returns>
        //public abstract DrawingContext CreateDrawingContext(IntPtr hdc, Rect rect);
        ///// <summary>
        ///// 通过Canvas创建绘图上下文
        ///// </summary>
        ///// <param name="Canvas"></param>
        ///// <returns></returns>
        //public abstract DrawingContext CreateDrawingContext<T>(T Canvas);
        /// <summary>
        /// 通过图片创建绘图上下文
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public abstract DrawingContext CreateDrawingContext(Bitmap bitmap);
        /// <summary>
        /// 通过窗体句柄创建绘图上下文
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public abstract DrawingContext CreateDrawingContext(IRenderTarget target);
        ///// <summary>
        ///// 通过窗体句柄创建绘图上下文
        ///// </summary>
        ///// <param name="hwnd"></param>
        ///// <returns></returns>
        //public abstract DrawingContext CreateDrawingContext(IntPtr hwnd);
        /// <summary>
        /// 计算文字尺寸
        /// </summary>
        /// <param name="str"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public abstract Size MeasureString(string str, Font font);
        /// <summary>
        /// 计算文字尺寸，限定最大宽度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="font"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public abstract Size MeasureString(string str, Font font, float width);
        /// <summary>
        /// 计算一行固定宽度下，该行可以放下几个字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="font"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public virtual int MeasureCharCount(string str, Font font, int width)
        {
            if (!string.IsNullOrEmpty(str))
            {
                int len = (int)(width / font.FontSize);//估算字符数量
                if (len < str.Length)
                {
                    var s = MeasureString(str.Substring(0, len), font, width);
                    if (s.Width > width)
                    {
                        while (len > 1 && s.Width > width)
                        {
                            len--;
                            s = MeasureString(str.Substring(0, len), font, width);
                        }
                    }
                    else if (s.Width < width)
                    {
                        while (len < str.Length && s.Width < width)
                        {
                            len++;
                            s = MeasureString(str.Substring(0, len), font, width);
                        }
                        len--;
                    }
                }
                return len;
            }
            return 0;
        }

        /// <summary>
        /// 创建字体
        /// </summary>
        /// <param name="fontFamily"></param>
        /// <param name="fontSize">像素为单位pix</param>
        /// <param name="fontStyle"></param>
        /// <returns></returns>
        public abstract IDisposable CreateFont(string fontFamily, float fontSize, FontStyles fontStyle);
        /// <summary>
        /// 加载字体
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fontFamily">不设置的话，用图形引擎解析出来的名字，不同图形引擎加载同一个字体可能会有不同的名字，可以自己定义个确定的名字来避免不同名称加载不到字体的问题。</param>
        public abstract void LoadFont(Stream stream, string fontFamily = null);
        /// <summary>
        /// 创建文字路径
        /// </summary>
        /// <param name="font"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public abstract IPathImpl CreatePath(in Font font,string text);
        /// <summary>
        /// 字体默认行高
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public abstract float GetLineHeight(in Font font);
        /// <summary>
        /// 获取从字样的基线到英语大写字母顶部的距离。
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public abstract float GetAscent(in Font font);
        /// <summary>
        /// 获取从字样的基线到行底部的距离。
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public abstract float GetDescent(in Font font);

        public abstract void Dispose();
        /// <summary>
        /// 尝试启用GPU
        /// </summary>
        public abstract bool UseGPU { get; set; }
    }
}
