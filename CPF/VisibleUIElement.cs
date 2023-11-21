using System;
using System.Collections.Generic;
using System.Text;
using CPF;

namespace CPF
{
    /// <summary>
    /// 保存当前在可视范围内的元素，由内部对象池保存，请勿外部保存引用
    /// </summary>
    public class VisibleUIElement
    {
        /// <summary>
        /// 当前元素
        /// </summary>
        public UIElement Element { get; private set; }

        public void SetElement(UIElement element)
        {
            Element = element;
        }

        List<VisibleUIElement> children;
        /// <summary>
        /// 子元素
        /// </summary>
        public List<VisibleUIElement> Children
        {
            get
            {
                if (children == null)
                {
                    children = new List<VisibleUIElement>();
                }
                return children;
            }
        }
    }
}
