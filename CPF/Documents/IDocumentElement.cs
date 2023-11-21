using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Documents
{
    /// <summary>
    /// 文档元素
    /// </summary>
    public interface IDocumentElement
    {
        //Point Position { get; set; }
        /// <summary>
        /// 样式ID，如果小于0 则使用父容器的样式
        /// </summary>
        short StyleId { get; set; }
        /// <summary>
        /// 计算布局并获取最终尺寸
        /// </summary>
        /// <param name="font"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        void Arrange(in Font font,in Size availableSize);

        //Size Size { get; }

        /// <summary>
        /// 布局之后的最终位置
        /// </summary>
        float Left { get; set; }

        /// <summary>
        /// 布局之后的最终位置
        /// </summary>
        float Top { get; set; }
        /// <summary>
        /// 布局之后的尺寸
        /// </summary>
        float Width { get; }
        /// <summary>
        /// 布局之后的尺寸
        /// </summary>
        float Height { get; }
        /// <summary>
        /// 右边间距
        /// </summary>
        float Right { get; }
        /// <summary>
        /// 下边间距
        /// </summary>
        float Bottom { get; }

        /// <summary>
        /// 尺寸计算是否有效，下次需要时重新计算尺寸
        /// </summary>
        bool IsMeasureValid { get; set; }
    }
}
