using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 关键词样式
    /// </summary>
    public class KeywordsStyle
    {
        /// <summary>
        /// 关键词或者正则表达式
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// 是否为正则表达式
        /// </summary>
        public bool IsRegex { get; set; }
        /// <summary>
        /// 是否忽略大小写
        /// </summary>
        public bool IgnoreCase { get; set; }
        /// <summary>
        /// 样式ID
        /// </summary>
        public short StyleId { get; set; } = -1;
    }
}
