using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Documents
{
    /// <summary>
    /// 文档元素容器
    /// </summary>
    public interface IDocumentContainer : IDocumentElement
    {
        Collection<IDocumentElement> Children { get; }
        /// <summary>
        /// 子元素是否可以选择
        /// </summary>
        bool ChildrenCanSelect { get; }
        /// <summary>
        /// 布局好的行
        /// </summary>
#if NET40
        IList<TextLine> Lines { get; }
#else
        IReadOnlyList<TextLine> Lines { get; }
#endif

        ViewFill Background { get; set; }
    }
}
