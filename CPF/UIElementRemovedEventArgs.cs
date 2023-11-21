using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    public class UIElementRemovedEventArgs : RoutedEventArgs
    {
        public UIElementRemovedEventArgs(UIElement element, object source) : base(source)
        {
            this.element = element;
        }

        UIElement element;
        /// <summary>
        /// 被移除的UI元素
        /// </summary>
        public UIElement Element
        {
            get { return element; }
        }
    }
}
