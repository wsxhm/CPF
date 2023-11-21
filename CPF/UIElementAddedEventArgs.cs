using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    public class UIElementAddedEventArgs : RoutedEventArgs
    {
        public UIElementAddedEventArgs(UIElement element,  object source) : base(source)
        {
            this.element = element;
        }

        UIElement element;
        /// <summary>
        /// 被添加的UI元素
        /// </summary>
        public UIElement Element
        {
            get { return element; }
        }
    }
}
