using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 表示一个 DataGrid 列，该列在其单元格中承载文本内容。
    /// </summary>
    public class DataGridTextColumn : DataGridColumn
    {
        public override DataGridCellTemplate GenerateEditingElement()
        {
            return new DataGridCellTextEditTemplate();
        }

        public override DataGridCellTemplate GenerateElement()
        {
            return new DataGridCellTemplate();
        }
    }
}
