using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    /// <summary>
    /// 命令参数的数据
    /// </summary>
    public enum CommandParameter : byte
    {
        /// <summary>
        /// 事件数据，如果是属性的话，则事件数据对象是CPFPropertyChangedEventArgs
        /// </summary>
        EventArgs,
        /// <summary>
        /// 事件发送者
        /// </summary>
        EventSender,
        /// <summary>
        /// 属性值
        /// </summary>
        PropertyValue,
        /// <summary>
        /// 旧的属性值
        /// </summary>
        OldPropertyValue,
        /// <summary>
        /// 属性元数据
        /// </summary>
        PropertyMetadata,
    }
}
