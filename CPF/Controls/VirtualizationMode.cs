using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 用来管理子项虚拟化的方法
    /// </summary>
    public enum VirtualizationMode
    {
        /// <summary>
        /// 创建并放弃项容器。 Standard virtualization mode -- containers are thrown away when offscreen.
        /// </summary>
        Standard,

        /// <summary>
        /// 重用项容器。  Recycling virtualization mode -- containers are re-used when offscreen.
        /// </summary>
        Recycling
    }
}
