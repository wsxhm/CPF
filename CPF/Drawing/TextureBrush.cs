using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    /// <summary>
    /// 图片纹理笔刷
    /// </summary>
    public class TextureBrush : Brush
    {
        public TextureBrush(Image image, WrapMode wrapMode)
        {
            this.Image = image;
            this.WrapMode = wrapMode;
            Matrix = Matrix.Identity;
        }
        public TextureBrush(Image image, WrapMode wrapMode, Matrix matrix)
        {
            this.Image = image;
            this.WrapMode = wrapMode;
            Matrix = matrix;
        }

        public Image Image { get; }
        /// <summary>
        /// 平铺方式
        /// </summary>
        public WrapMode WrapMode { get; }
        /// <summary>
        /// 变换
        /// </summary>
        public Matrix Matrix { get; }
    }

    /// <summary>
    /// 指定纹理或渐变平铺小于所填充的区域时
    /// </summary>
    public enum WrapMode : byte
    {
        /// <summary>
        /// 平铺渐变或纹理
        /// </summary>
        Tile,
        ////
        //// 摘要:
        ////     水平方向将反转纹理或渐变，并将它们平铺纹理或渐变。
        //TileFlipX,
        ////
        //// 摘要:
        ////     垂直方向将反转纹理或渐变，并将它们平铺纹理或渐变。
        //TileFlipY,
        ////
        //// 摘要:
        ////     水平和垂直方向将反转纹理或渐变，并将它们平铺纹理或渐变。
        //TileFlipXY = 3,
        /// <summary>
        /// 纹理或渐变没有平铺
        /// </summary>
        Clamp
    }
}
