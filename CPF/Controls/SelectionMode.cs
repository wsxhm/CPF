using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{

    public enum SelectionMode : byte
    {
        /// <summary>
        /// 用户可以按下 Shift 键来选择多个连续项。
        /// </summary>
        Extended,
        /// <summary>
        /// 用户可以选择多个项而无需按下修改键。
        /// </summary>
        Multiple,
        /// <summary>
        /// 用户一次只能选择一项。
        /// </summary>
        Single
    }
    /// <summary>
    /// 定义指定 DataGrid 控件支持单项选择还是多项选择的常数
    /// </summary>
    public enum DataGridSelectionMode : byte
    {
        Single,
        Extended,
    }
    /// <summary>
    /// 定义指定单元格、 行或两者，是否用于 DataGrid 空间中的选择的常数。
    /// </summary>
    public enum DataGridSelectionUnit : byte
    {
        Cell,
        FullRow,
    }
}
