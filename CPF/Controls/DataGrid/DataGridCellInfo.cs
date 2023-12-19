using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 表示有关 DataGrid 类中特定单元格的信息。
    /// </summary>
    public struct DataGridCellInfo
    {
        public DataGridCellInfo(int rowIndex, DataGridColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }

            _column = column;
            this.rowIndex = rowIndex;
        }
        private DataGridColumn _column;
        int rowIndex;
        public int RowIndex
        {
            get { return rowIndex; }
        }

        /// <summary>
        ///   获取与包含此单元格的行的数据项。
        /// </summary>
        public object Item
        {
            get { return Column.DataGridOwner.Items[rowIndex]; }
        }

        /// <summary>
        /// 尝试获取单元格值
        /// </summary>
        /// <returns></returns>
        public object GetCellValue()
        {
            if (Column != null && Column.Binding != null && Item != null)
            {
                return Item.GetPropretyValue(Column.Binding.SourcePropertyName);
            }
            return null;
        }

        /// <summary>
        /// 获取包含单元格的列。
        /// </summary>
        public DataGridColumn Column
        {
            get { return _column; }
        }

        public override int GetHashCode()
        {
            return (_column == null ? 0 : _column.GetHashCode()) ^
                   rowIndex.GetHashCode();
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is DataGridCellInfo))
            {
                return false;
            }

            DataGridCellInfo value = (DataGridCellInfo)o;
            return value._column == _column && value.rowIndex == rowIndex;
        }
    }
}
