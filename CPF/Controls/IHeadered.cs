using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 定义一个带标题Header的组件
    /// </summary>
    public interface IHeadered
    {
        /// <summary>
        /// 获取或设置标记控件的项
        /// </summary>
        object Header
        {
            get;
            set;
        }
    }
}
