using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Input
{
    /// <summary>
    /// Interface to access information about the data of a drag-and-drop or Clipboard operation.
    /// </summary>
    public interface IDataObject
    {
        ///// <summary>
        ///// Lists all formats which are present in the DataObject.
        ///// <seealso cref="DataFormat"/>
        ///// </summary>
        //IEnumerable<DataFormat> GetDataFormats();

        /// <summary>
        /// Checks whether a given DataFormat is present in this object
        /// <seealso cref="DataFormat"/>
        /// </summary>
        bool Contains(DataFormat dataFormat);

        ///// <summary>
        ///// Returns the dragged text if the DataObject contains any text.
        ///// <seealso cref="DataFormats.Text"/>
        ///// </summary>
        //string GetText();

        /// <summary>
        /// Tries to get the data of the given DataFormat.
        /// </summary>
        object GetData(DataFormat dataFormat);
    }

    public enum DataFormat : byte
    {
        Unknown = 0,
        Text = 1,
        Html = 2,
        /// <summary>
        /// Image对象
        /// </summary>
        Image = 4,
        /// <summary>
        /// 文件名字符串数组
        /// </summary>
        FileNames = 8,
    }
}
