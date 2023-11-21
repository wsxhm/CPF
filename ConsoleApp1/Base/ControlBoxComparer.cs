using CPF;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace xss_pro.Base
{
    public class ControlBoxComparer : IComparer<UIElement>
    {
        private readonly UIElement[] _map;
        private bool _descending;
        public ControlBoxComparer(UIElement[] map , bool descending)
        {
            _map = map;
            _descending = descending;
        }
        public int Compare(UIElement x, UIElement y)
        {
            return _descending ? GetIndex(y) - GetIndex(x) : GetIndex(x) - GetIndex(y);
        }
        public int GetIndex(UIElement target)
        {
            int i = 0;
            foreach(UIElement element in _map)
            {
                if (element == target)
                    break;
                i++;
            }
            return i;
        }
    }
}
