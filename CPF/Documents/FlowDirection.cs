using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Documents
{
    /// <summary>
    /// 指定文本和用户界面 (UI) 元素的内容流动方向
    /// </summary>
    public enum FlowDirection : byte
    {
        /// <summary>
        /// 自动，继承上一个元素
        /// </summary>
        Auto,
        /// <summary>
        /// 指示内容应从左向右流动。
        /// </summary>
        LeftToRight,
        /// <summary>
        /// 指示内容应从右向左流动
        /// </summary>
        RightToLeft
    }
}
