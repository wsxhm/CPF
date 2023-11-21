using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    /// <summary>
    /// 文档元素对齐方式
    /// </summary>
    public enum TextAlignment : byte
    {
        /// <summary>
        /// In horizontal inline progression, the text is aligned on the left.
        /// </summary>
        Left,

        /// <summary>
        /// In horizontal inline progression, the text is aligned on the right.
        /// </summary>
        Right,

        /// <summary>
        /// The text is center aligned.
        /// </summary>
        Center,

        ///// <summary>
        ///// The text is justified.
        ///// </summary>
        //Justify,
    }
}
