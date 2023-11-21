using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Input
{
    public enum NavigationMethod
    {
        /// <summary>
        /// 未指定，比如直接调用Focus方法
        /// </summary>
        Unspecified,

        /// <summary>
        /// 用户 tab 在控件之间更改了焦点。
        /// </summary>
        Tab,

        /// <summary>
        /// 用户按方向导航键更改焦点。
        /// </summary>
        Directional,

        /// <summary>
        /// 焦点由指针单击更改。
        /// </summary>
        Click,
    }
}
