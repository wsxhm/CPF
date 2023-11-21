using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CPF.Drawing
{
    /// <summary>
    /// 绘图上下文
    /// </summary>
    public abstract class DrawingContext : IDisposable
    {
        /// <summary>
        /// 绘制线条
        /// </summary>
        /// <param name="stroke"></param>
        /// <param name="strokeBrush"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        public abstract void DrawLine(in Stroke stroke, Brush strokeBrush, in Point point1, in Point point2);
        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="strokeBrush"></param>
        /// <param name="stroke"></param>
        /// <param name="rect"></param>
        public abstract void DrawRectangle(Brush strokeBrush, in Stroke stroke, in Rect rect);
        /// <summary>
        /// 填充矩形
        /// </summary>
        /// <param name="fillBrush"></param>
        /// <param name="rect"></param>
        public abstract void FillRectangle(Brush fillBrush, Rect rect);
        ///// <summary>
        ///// 画一个圆角的矩形与提供的填充笔刷和/或描边笔刷。笔刷为空则无操作
        ///// </summary>
        ///// <param name="strokeBrush"></param>
        ///// <param name="stroke"></param>
        ///// <param name="fillBrush"></param>
        ///// <param name="rectangle"></param>
        ///// <param name="radiusX">X 维的这的圆角半径 圆角的矩形此值将被限制到范围 [0..rectangle。宽度/2]</param>
        ///// <param name="radiusY">Y 维的这的圆角半径 圆角的矩形此值将被限制到范围 [0..rectangle。高度/2]。</param>
        //public abstract void DrawRoundedRectangle(Brush strokeBrush, Stroke stroke, Brush fillBrush, Rect rectangle, Double radiusX, Double radiusY);
        /// <summary>
        /// 绘制椭圆
        /// </summary>
        /// <param name="strokeBrush"></param>
        /// <param name="stroke"></param>
        /// <param name="center">椭圆填充或描边的中心。</param>
        /// <param name="radiusX">在椭圆的 X 尺寸半径。将使用提供的半径绝对值。</param>
        /// <param name="radiusY">椭圆的 Y 轴半径。将使用提供的半径绝对值。</param>
        public abstract void DrawEllipse(Brush strokeBrush, in Stroke stroke, in Point center, in float radiusX, in float radiusY);
        /// <summary>
        /// 填充椭圆
        /// </summary>
        /// <param name="fillBrush"></param>
        /// <param name="center">中心点</param>
        /// <param name="radiusX">水平半径</param>
        /// <param name="radiusY">垂直半径</param>
        public abstract void FillEllipse(Brush fillBrush, in Point center, in float radiusX, in float radiusY);
        /// <summary>
        /// 填充图形
        /// </summary>
        /// <param name="fillBrush">填充笔刷</param>
        /// <param name="geometry"></param>
        public abstract void FillGeometry(Brush fillBrush, Geometry geometry);
        ///// <summary>
        ///// 描边图形
        ///// </summary>
        ///// <param name="strokeBrush"></param>
        ///// <param name="stroke"></param>
        ///// <param name="geometry"></param>
        //public abstract void DrawGeometry(Brush strokeBrush, in Stroke stroke, Geometry geometry);
        /// <summary>
        /// 绘制路径
        /// </summary>
        /// <param name="strokeBrush"></param>
        /// <param name="stroke"></param>
        /// <param name="path"></param>
        public abstract void DrawPath(Brush strokeBrush, in Stroke stroke, PathGeometry path);
        /// <summary>
        /// 填充路径
        /// </summary>
        /// <param name="fillBrush"></param>
        /// <param name="path"></param>
        public abstract void FillPath(Brush fillBrush, PathGeometry path);
        /// <summary>
        /// 绘制图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="destRect">目标矩形</param>
        /// <param name="srcRect">源图片裁剪矩形</param>
        /// <param name="opacity">不透明度</param>
        public abstract void DrawImage(Image image, in Rect destRect, in Rect srcRect, in float opacity = 1);
        ///// <summary>
        ///// 绘制文字
        ///// </summary>
        ///// <param name="location"></param>
        ///// <param name="fillBrush"></param>
        ///// <param name="text"></param>
        ///// <param name="font"></param>
        ///// <param name="maxWidth"></param>
        //public abstract void DrawString(Point location, Brush fillBrush, string text, Font font, float maxWidth = float.MaxValue);
        /// <summary>
        /// 绘制文字
        /// </summary>
        /// <param name="location">位置</param>
        /// <param name="fillBrush"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="textAlignment">文本对齐</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="decoration">文本修饰，它是可添加到文本的视觉装饰（如下划线）</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="textTrimming">文本裁剪修饰</param>
        /// <param name="stroke">文字描边</param>
        /// <param name="strokeBrush">文字描边填充</param>
        public abstract void DrawString(in Point location, Brush fillBrush, string text, in Font font, in TextAlignment textAlignment = TextAlignment.Left, in float maxWidth = float.MaxValue,
            in TextDecoration decoration = default,
            in float maxHeight = float.MaxValue,
            in TextTrimming textTrimming = TextTrimming.None,
            in Stroke stroke = default,
            Brush strokeBrush = null
            );
        /// <summary>
        /// 绘制文字
        /// </summary>
        /// <param name="ellipsis">是否裁剪了文本</param>
        /// <param name="location">位置</param>
        /// <param name="fillBrush"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="textAlignment">文本对齐</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="decoration">文本修饰，它是可添加到文本的视觉装饰（如下划线）</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="textTrimming">文本裁剪修饰</param>
        /// <param name="stroke">文字描边</param>
        /// <param name="strokeBrush">文字描边填充</param>
        public abstract void DrawString(out bool ellipsis, in Point location, Brush fillBrush, string text, in Font font, in TextAlignment textAlignment = TextAlignment.Left, in float maxWidth = float.MaxValue,
            in TextDecoration decoration = default,
            in float maxHeight = float.MaxValue,
            in TextTrimming textTrimming = TextTrimming.None,
            in Stroke stroke = default,
            Brush strokeBrush = null
            );

        /// <summary>
        /// 清空绘图区域
        /// </summary>
        /// <param name="color"></param>
        public abstract void Clear(Color color);
        /// <summary>
        /// 设置剪辑区域
        /// </summary>
        /// <param name="rect"></param>
        public abstract void PushClip(Rect rect);
        /// <summary>
        /// 删除最后一个剪辑区域
        /// </summary>
        public abstract void PopClip();
        ///// <summary>
        ///// 设置图形变换矩阵
        ///// </summary>
        ///// <param name="matrix"></param>
        //public abstract void PushMatrix(Matrix matrix);
        ///// <summary>
        ///// 获取图形变换矩阵
        ///// </summary>
        //public abstract Matrix GetMatrix();
        /// <summary>
        /// 获取或设置2D变换
        /// </summary>
        public abstract Matrix Transform { get; set; }
        /// <summary>
        /// 获取或设置抗锯齿模式
        /// </summary>
        public abstract AntialiasMode AntialiasMode { get; set; }

        /// <summary>
        /// 创建纯色笔刷
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        protected abstract IDisposable CreateSolidBrush(Color color);
        /// <summary>
        /// 创建渐变笔刷，需要定义2个颜色以上
        /// </summary>
        /// <param name="bcs">需要定义2个颜色以上</param>
        /// <param name="start">开始</param>
        /// <param name="end">结束</param>
        /// <returns></returns>
        protected abstract IDisposable CreateLinearGradientBrush(GradientStop[] bcs, in Point start, in Point end, in Matrix matrix);
        /// <summary>
        /// 获取纹理笔刷
        /// </summary>
        /// <returns></returns>
        protected abstract IDisposable CreateTextureBrush(Image image, in WrapMode wrapMode, in Matrix matrix);
        /// <summary>
        /// 创建径向渐变画笔
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="blendColors"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        protected abstract IDisposable CreateRadialGradientBrush(in Point center, in float radius, GradientStop[] blendColors, in Matrix matrix);

        /// <summary>
        /// 释放资源
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// 创建对应笔刷的适配笔刷
        /// </summary>
        /// <param name="brush"></param>
        protected virtual void InitializeBrush(Brush brush)
        {
            if (brush.AdapterBrush == null)
            {
                SolidColorBrush scb = brush as SolidColorBrush;
                LinearGradientBrush lgb;
                TextureBrush tb;
                RadialGradientBrush rg;
                if (scb != null)
                {
                    scb.AdapterBrush = CreateSolidBrush(scb.Color);
                }
                else if ((lgb = brush as LinearGradientBrush) != null)
                {
                    lgb.AdapterBrush = CreateLinearGradientBrush(lgb.BlendColors, lgb.StartPoint, lgb.EndPoint, lgb.Matrix);
                }
                else if ((tb = brush as TextureBrush) != null)
                {
                    tb.AdapterBrush = CreateTextureBrush(tb.Image, tb.WrapMode, tb.Matrix);
                }
                else if ((rg = brush as RadialGradientBrush) != null)
                {
                    rg.AdapterBrush = CreateRadialGradientBrush(rg.Center, rg.Radius, rg.BlendColors, rg.Matrix);
                }
            }
        }

        public abstract DrawingFactory DrawingFactory { get; }

        public static DrawingContext FromBitmap(Bitmap bmp)
        {
            return Platform.Application.GetDrawingFactory().CreateDrawingContext(bmp);
        }
        public static DrawingContext FromRenderTarget(IRenderTarget renderTarget)
        {
            return Platform.Application.GetDrawingFactory().CreateDrawingContext(renderTarget);
        }
        ///// <summary>
        ///// 从canvas创建drawcontext
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="Canvas"></param>
        ///// <returns></returns>
        //public static DrawingContext FromCanvas<T>(T Canvas)
        //{
        //    return Platform.Application.GetDrawingFactory().CreateDrawingContext(Canvas);
        //}
    }
    /// <summary>
    /// 描述当文本溢出其包含框的边缘时如何修整文本。
    /// </summary>
    public enum TextTrimming : byte
    {
        /// <summary>
        /// 不修整文本。
        /// </summary>
        None,
        /// <summary>
        /// 在字符边界处修整文本。 将绘制省略号 (...) 来替代剩余的文本。
        /// </summary>
        CharacterEllipsis,
        /// <summary>
        /// 在字符中间出处修整文本。 将绘制省略号 (...) 来替代剩余的文本。
        /// </summary>
        CharacterCenterEllipsis,
    }
    /// <summary>
    /// 定义一个阴影
    /// </summary>
    public struct Shadow
    {
        /// <summary>
        /// 水平偏移
        /// </summary>
        public float HOffset { get; set; }
        /// <summary>
        /// 垂直偏移
        /// </summary>
        public float VOffset { get; set; }
        /// <summary>
        /// 模糊的距离
        /// </summary>
        public ushort Blur { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public Color Color { get; set; }
    }
}
