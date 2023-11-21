using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    /// <summary>
    /// 定义一个编辑器
    /// </summary>
    public interface IEditor
    {
        /// <summary>
        /// 是否启用输入法，主要描述的是中文这类输入法
        /// </summary>
        bool IsInputMethodEnabled
        {
            get;
        }
        /// <summary>
        /// 是否是只读，只读模式下，一般不主动显示软键盘
        /// </summary>
        bool IsReadOnly { get; }
    }
}
