using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CPF.Drawing
{
    public interface IImage : IDisposable, ICloneable
    {
        /// <summary>
        /// 宽度
        /// </summary>
        int Width { get; }

        /// <summary>
        /// 高度
        /// </summary>
        int Height
        {
            get;
        }

        Stream SaveToStream(ImageFormat format);

        void SaveToStream(ImageFormat format, Stream m);
    }
}
