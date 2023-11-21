using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Documents
{
    public interface IDocumentStyle
    {
        /// <summary>
        /// 字体名称
        /// </summary>
        string FontFamily
        {
            get;
            set;
        }

        /// <summary>
        /// 字体尺寸
        /// </summary>
        float FontSize
        {
            get;
            set;
        }

        /// <summary>
        /// 字体样式
        /// </summary>
        FontStyles FontStyle
        {
            get;
            set;
        }

        /// <summary>
        /// 前景填充
        /// </summary>
        ViewFill Foreground
        {
            get;
            set;
        }

        ViewFill Background { get; set; }

        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// </summary>
        TextDecoration TextDecoration
        {
            get;
            set;
        }
    }
}
