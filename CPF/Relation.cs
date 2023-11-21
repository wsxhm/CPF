using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CPF
{
    /// <summary>
    /// 元素关系
    /// </summary>
    public class Relation
    {
        List<IUIElementEnumerable> list = new List<IUIElementEnumerable>();
        Relation(IEnumerable<IUIElementEnumerable> enumerable)
        {
            list.AddRange(enumerable);
        }
        /// <summary>
        /// 根据构建的关系条件查询元素
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<UIElement> Query(UIElement element)
        {
            if (list.Count == 1)
            {
                foreach (var item in list)
                {
                    item.UIElement = element;
                    foreach (var e in item)
                    {
                        yield return e;
                    }
                }
            }
            //var ui = element;
            List<UIElement> cacheList = new List<UIElement>();
            cacheList.Add(element);
            List<UIElement> cacheList1 = new List<UIElement>();
            var index = 0;
            foreach (var item in list.Skip(1))
            {
                index++;
                foreach (var ui in cacheList)
                {
                    item.UIElement = ui;
                    foreach (var uiItem in item)
                    {
                        if (uiItem == null)
                        {
                            continue;
                        }
                        cacheList1.Add(uiItem);
                        if (index == list.Count - 1)
                        {
                            yield return uiItem;
                        }
                    }
                }
                cacheList = cacheList1.ToList();
                cacheList1.Clear();
            }
        }

        /// <summary>
        /// 当前元素
        /// </summary>
        public static Relation Me { get; } = new Relation(new IUIElementEnumerable[] { new UIElementMe() });
        /// <summary>
        /// 被选元素的直接父元素
        /// </summary>
        public Relation Parent
        {
            get
            {
                var l = list.ToList();
                l.Add(new UIElementParent { Parent = true });
                return new Relation(l);
            }
        }
        /// <summary>
        /// 被选元素的所有祖先元素，它一路向上直到文档的根元素
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Relation Parents(Func<UIElement, bool> filter = null)
        {
            var l = list.ToList();
            l.Add(new UIElementParent { func = filter });
            return new Relation(l);
        }
        /// <summary>
        /// 所有直接子元素
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Relation Children(Func<UIElement, bool> filter = null)
        {
            var l = list.ToList();
            l.Add(new UIElementChildren { func = filter });
            return new Relation(l);
        }
        /// <summary>
        /// 被选元素的后代元素，一路向下直到最后一个后代
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Relation Find(Func<UIElement, bool> func = null)
        {
            var l = list.ToList();
            l.Add(new UIElementFind { func = func });
            return new Relation(l);
        }
        ///// <summary>
        ///// 被选元素的所有同胞元素
        ///// </summary>
        ///// <param name="func"></param>
        ///// <returns></returns>
        //public Relation Siblings(Func<UIElement, bool> func)
        //{
        //    return new Relation();
        //}
        /// <summary>
        /// 绝对关系
        /// </summary>
        /// <param name="element"></param>
        public static implicit operator Relation(UIElement element)
        {
            return new Relation(new IUIElementEnumerable[] { new UIElementIEnumerable { Element = element } });
        }

        public override string ToString()
        {
            if (list.Count > 0)
            {
                return "Relation" + "." + string.Join(".", list.Select(a => a.ToString()).ToArray());
            }
            return base.ToString();
        }
    }

    class UIElementParent : IUIElementEnumerable
    {
        public UIElement UIElement { get; set; }
        public Func<UIElement, bool> func;
        public bool Parent = false;
        public IEnumerator<UIElement> GetEnumerator()
        {
            return new ParentEnumerator(UIElement, func, Parent);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ParentEnumerator(UIElement, func, Parent);
        }

        public override string ToString()
        {
            if (Parent)
            {
                return "Parent";
            }
            else
            {
                return "Parents";
            }
        }
    }
    /// <summary>
    /// 枚举父级
    /// </summary>
    class ParentEnumerator : IEnumerator, IEnumerator<UIElement>
    {
        UIElement element;
        UIElement current;
        Func<UIElement, bool> func;
        bool Parent = false;
        bool first = true;
        public ParentEnumerator(UIElement element, Func<UIElement, bool> func, bool parent)
        {
            this.element = element;
            this.func = func;
            this.Parent = parent;
            current = element;
        }
        public object Current
        {
            get
            {
                return current;
            }
        }

        UIElement IEnumerator<UIElement>.Current
        {
            get
            {
                return current;
            }
        }

        public void Dispose()
        { }

        public bool MoveNext()
        {
            if (Parent)
            {
                var r = first;
                first = false;
                current = element.Parent;
                return r;
            }
            if (func != null)
            {
                while (current != null)
                {
                    current = current.Parent;
                    if (current != null && func(current))
                    {
                        break;
                    }
                }
            }
            else
            {
                current = current.Parent;
            }
            return current != null;
        }

        public void Reset()
        {
            current = element;
            first = true;
        }
    }

    class UIElementMe : IUIElementEnumerable
    {
        public UIElement UIElement { get; set; }
        public IEnumerator<UIElement> GetEnumerator()
        {
            return new MeEnumerator(UIElement);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MeEnumerator(UIElement);
        }

        public override string ToString()
        {
            return "Me";
        }
    }

    class UIElementIEnumerable : IUIElementEnumerable
    {
        public UIElement Element { get; set; }
        public UIElement UIElement { get; set; }
        public IEnumerator<UIElement> GetEnumerator()
        {
            return new MeEnumerator(Element);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MeEnumerator(Element);
        }

        public override string ToString()
        {
            if (Element != null)
            {
                return Element.ToString();
            }
            return base.ToString();
        }
    }

    class MeEnumerator : IEnumerator, IEnumerator<UIElement>
    {
        UIElement element;
        public MeEnumerator(UIElement element)
        {
            this.element = element;
        }

        public object Current
        {
            get
            {
                return element;
            }
        }

        UIElement IEnumerator<UIElement>.Current
        {
            get
            {
                first = false;
                return element;
            }
        }

        public void Dispose()
        { }

        bool first = true;
        public bool MoveNext()
        {
            return first;
        }

        public void Reset()
        {
            first = true;
        }
    }

    class UIElementChildren : IUIElementEnumerable
    {
        public UIElement UIElement
        {
            get;
            set;
        }

        public Func<UIElement, bool> func;

        public IEnumerator<UIElement> GetEnumerator()
        {
            return new ChildrenEnumerator(UIElement, func);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ChildrenEnumerator(UIElement, func);
        }

        public override string ToString()
        {
            return "Children";
        }
    }

    class ChildrenEnumerator : IEnumerator, IEnumerator<UIElement>
    {
        UIElement element;
        Func<UIElement, bool> func;
        int index = -1;
        UIElementCollection elements;
        public ChildrenEnumerator(UIElement element, Func<UIElement, bool> func)
        {
            this.element = element;
            elements = element.Children;
            this.func = func;
        }

        public UIElement Current
        {
            get
            {
                return elements[index];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return elements[index];
            }
        }

        public void Dispose()
        { }

        public bool MoveNext()
        {
            if (func != null)
            {
                while (true)
                {
                    index++;
                    if (index < elements.Count && func(elements[index]))
                    {
                        return true;
                    }
                    else if (index >= elements.Count)
                    {
                        return false;
                    }
                }
            }
            else
            {
                index++;
            }
            return index < elements.Count;
        }

        public void Reset()
        {
            index = -1;
        }
    }

    class UIElementFind : IUIElementEnumerable
    {
        public UIElement UIElement
        {
            get;
            set;
        }

        public Func<UIElement, bool> func;

        public IEnumerator<UIElement> GetEnumerator()
        {
            return new FindEnumerator(UIElement, func);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FindEnumerator(UIElement, func);
        }

        public override string ToString()
        {
            return "Find";
        }
    }

    class FindEnumerator : IEnumerator<UIElement>
    {
        IEnumerator<UIElement> enumerator;
        public FindEnumerator(UIElement element, Func<UIElement, bool> func)
        {
            if (func == null)
            {
                enumerator = element.Find<UIElement>().GetEnumerator();
            }
            else
            {
                enumerator = element.Find<UIElement>().Where(func).GetEnumerator();
            }

        }
        public UIElement Current
        {
            get
            {
                return enumerator.Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return enumerator.Current;
            }
        }

        public void Dispose()
        { }

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }

        public void Reset()
        {
            enumerator.Reset();
        }
    }

    interface IUIElementEnumerable : IEnumerable<UIElement>
    {
        UIElement UIElement { get; set; }
    }
}
