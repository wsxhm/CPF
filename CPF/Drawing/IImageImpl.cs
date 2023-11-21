using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    public interface IImageImpl : IImage
    {
        ///// <summary>
        ///// 宽度
        ///// </summary>
        //uint Width
        //{
        //    get;
        //}

        ///// <summary>
        ///// 高度
        ///// </summary>
        //uint Height
        //{
        //    get;
        //}
        ///// <summary>
        ///// 保存到文件
        ///// </summary>
        ///// <param name="fileName"></param>
        ///// <param name="format"></param>
        //void SaveToFile(string fileName, ImageFormat format);
        /// <summary>
        /// 图片帧数，比如GIF
        /// </summary>
        uint FrameCount { get; }

        uint Index { get; set; }

        int Duration { get; }

        int[] FrameDelay { get; }
    }
}
