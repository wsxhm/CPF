using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;

// ReSharper disable once CheckNamespace
namespace CPF.Styling
{
    /// <summary>
    /// 复杂选择器
    /// </summary>
    public class ComplexSelector : BaseSelector, IEnumerable<CombinatorSelector>
    {
        private readonly List<CombinatorSelector> _selectors;

        public CombinatorSelector this[int index]
        {
            get { return _selectors[index]; }
        }

        public ComplexSelector()
        {
            _selectors = new List<CombinatorSelector>();
        }

        public ComplexSelector AppendSelector(BaseSelector selector, Combinator combinator)
        {
            _selectors.Add(new CombinatorSelector(selector, combinator));
            return this;
        }

        public IEnumerator<CombinatorSelector> GetEnumerator()
        {
            return _selectors.GetEnumerator();
        }

        internal void ConcludeSelector(BaseSelector selector)
        {
            _selectors.Add(new CombinatorSelector { Selector = selector });
        }

        public int Length
        {
            get { return _selectors.Count; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_selectors).GetEnumerator();
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            var builder = new StringBuilder();

            if (_selectors.Count <= 0)
            {
                return builder.ToString();
            }

            var n = _selectors.Count - 1;

            for (var i = 0; i < n; i++)
            {
                builder.Append(_selectors[i].Selector);
                builder.Append(_selectors[i].Character);
            }

            builder.Append(_selectors[n].Selector);

            return builder.ToString();
        }

        public override bool Select(UIElement element)
        {
            //if (_selectors.Count != 2)
            //{
            //    throw new Exception("只能支持两层选择器" + ToString());
            //}
            if (_selectors[_selectors.Count - 1].Selector.Select(element))
            {
                var pre = _selectors.Count - 2;
                var node = element;
                while (true)
                {
                    var s = _selectors[pre];
                    switch (s.Delimiter)
                    {
                        case Combinator.Child://>
                            var p = node.Parent;
                            if (p != null)
                            {
                                if (s.Selector.Select(p))
                                {
                                    if (pre == 0)
                                    {
                                        return true;
                                    }
                                    node = node.Parent;
                                    pre--;
                                    continue;
                                }
                            }
                            return false;
                        case Combinator.Descendent://空格
                            var parent = node.Parent;
                            var conti = false;
                            while (parent != null)
                            {
                                if (s.Selector.Select(parent))
                                {
                                    if (pre == 0)
                                    {
                                        return true;
                                    }
                                    node = parent;
                                    pre--;
                                    conti = true;
                                    break;
                                }
                                parent = parent.Parent;
                            }
                            if (conti)
                            {
                                continue;
                            }
                            return false;
                        case Combinator.AdjacentSibling:
                        case Combinator.Sibling:
                        case Combinator.Namespace:
                        default:
                            return false;
                    }
                }
            }
            return false;
        }
    }
}
