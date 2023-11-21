using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Input
{
    public class GotFocusEventArgs : RoutedEventArgs
    {
        public GotFocusEventArgs(object source) : base(source)
        {

        }

        /// <summary>
        /// 获取或设置指示焦点更改发生方式的值。
        /// </summary>
        public NavigationMethod NavigationMethod { get; set; }
    }
}
