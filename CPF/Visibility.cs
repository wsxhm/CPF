using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    public enum Visibility : byte
    {
        /// <summary>
        /// Normally visible.
        /// </summary>
        Visible = 0,

        /// <summary>
        /// Occupies space in the layout, but is not visible (completely transparent).
        /// 相当于透明，保留控件位置占用
        /// </summary>
        Hidden,

        /// <summary>
        /// Not visible and does not occupy any space in layout, as if it doesn't exist.
        /// 完全隐藏
        /// </summary>
        Collapsed
    }
}
