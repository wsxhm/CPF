using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 表示一个 DataGrid 列，该列在其单元格中承载模板指定的内容。
    /// </summary>
    public class DataGridTemplateColumn : DataGridColumn
    {
        /// <summary>
        /// 单元格模板
        /// </summary>
        public UIElementTemplate<DataGridCellTemplate> CellTemplate
        {
            get { return GetValue<UIElementTemplate<DataGridCellTemplate>>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 编辑模式下的单元格模板
        /// </summary>
        public UIElementTemplate<DataGridCellTemplate> CellEditingTemplate
        {
            get { return GetValue<UIElementTemplate<DataGridCellTemplate>>(); }
            set { SetValue(value); }
        }
        public override DataGridCellTemplate GenerateEditingElement()
        {
            var template = CellEditingTemplate;
            if (template == null)
            {
                throw new Exception("CellEditingTemplate不能为空");
            }
            return template.CreateElement();
        }

        public override DataGridCellTemplate GenerateElement()
        {
            var template = CellTemplate;
            if (template == null)
            {
                throw new Exception("CellTemplate不能为空");
            }
            return template.CreateElement();
        }
    }
}
