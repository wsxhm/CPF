using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;

namespace CPF
{
    /// <summary>
    /// 子元素集合
    /// </summary>
    public class UIElementCollection : IEnumerable<UIElement>, IList, IList<UIElement>
#if Net4
#else
        , IReadOnlyList<UIElement>
#endif
    {
        UIElement owner;
        public UIElementCollection(UIElement owner)
        {
            this.owner = owner;
            orderByZIndexList = ElementList;
        }

        List<UIElement> UIElements = new List<UIElement>();
        /// <summary>
        /// 内部UIElementList
        /// </summary>
        public List<UIElement> ElementList
        {
            get { return UIElements; }
        }

        UIElement[] list;
        /// <summary>
        /// 获取根据TabIndex排序之后的List
        /// </summary>
        internal UIElement[] OrderByTabIndexList()
        {
            //get
            //{
            if (list == null)
            {
                list = UIElements.ToArray();
                SortOfTabIndex(list);
            }
            return list;
            //}
        }

        private static void SortOfTabIndex(UIElement[] list)
        {//选择排序
            int min;
            for (int i = 0; i < list.Length; i++)
            {
                min = i;
                for (int j = i + 1; j < list.Length; j++)
                {
                    if (list[j].TabIndex - list[min].TabIndex < 0)
                    {
                        min = j;
                    }
                }
                UIElement temp = list[i];
                list[i] = list[min];
                list[min] = temp;
            }
        }

        IList<UIElement> orderByZIndexList;
        /// <summary>
        /// 获取根据ZIndex排序之后的List
        /// </summary>
        internal IList<UIElement> OrderByZIndexList()
        {
            //get
            //{
            if (orderByZIndexList == null)
            {
                SortOfZIndex();
            }
            return orderByZIndexList;
            //}
        }

        /// <summary>
        /// 相同的值，如果出现不相同，将为int.Min
        /// </summary>
        int zindex = 0;
        /// <summary>
        /// 根据ZIndex属性排序
        /// </summary>
        void SortOfZIndex()
        {//选择排序
            if (UIElements.Count < 2)
            {
                orderByZIndexList = UIElements;
                return;
            }
            orderByZIndexList = UIElements.ToList();
            int min;
            for (int i = 0; i < orderByZIndexList.Count; i++)
            {
                min = i;
                for (int j = i + 1; j < orderByZIndexList.Count; j++)
                {
                    if (orderByZIndexList[j].ZIndex - orderByZIndexList[min].ZIndex < 0)
                    {
                        min = j;
                    }
                }
                UIElement temp = orderByZIndexList[i];
                orderByZIndexList[i] = orderByZIndexList[min];
                orderByZIndexList[min] = temp;
            }
            zindex = orderByZIndexList[0].ZIndex;
            for (int i = 1; i < orderByZIndexList.Count; i++)
            {
                if (zindex != orderByZIndexList[i].ZIndex)
                {
                    zindex = int.MinValue;
                    break;
                }
            }
        }

        /// <summary>
        /// 重新按照ZIndex排序
        /// </summary>
        internal void InvalidateZIndex()
        {
            orderByZIndexList = null;
        }

        void AddUpdateZindex(UIElement element)
        {
            if (UIElements.Count == 1)
            {
                zindex = element.ZIndex;
            }
            else if (UIElements.Count > 1 && zindex != element.ZIndex)
            {
                zindex = int.MinValue;
                InvalidateZIndex();
            }
        }
        void RemoveUpdateZindex(UIElement element)
        {
            if (UIElements.Count == 1)
            {
                zindex = ElementList[0].ZIndex;
                if (orderByZIndexList != null && orderByZIndexList != ElementList)
                {
                    orderByZIndexList.Remove(element);
                }
            }
            else if (ElementList.Count > 1)
            {
                if (orderByZIndexList != null && orderByZIndexList != ElementList)
                {
                    orderByZIndexList.Remove(element);
                }
            }
            else
            {
                orderByZIndexList = UIElements;
            }
        }

        public T Add<T>(T visual) where T : UIElement
        {
            if (visual == null)
            { return null; }
            if (visual.Parent != null)
            {
                visual.Parent.Children.Remove(visual);
            }
            visual.Parent = owner;
            UIElements.Add(visual);
            list = null;
            AddUpdateZindex(visual);
            owner.RaiseUIElementAdded(visual);
            return visual;
        }
        void ICollection<UIElement>.Add(UIElement visual)
        {
            Add(visual);
        }

        public void Add(IEnumerable<UIElement> items)
        {
            if (items == null)
            {
                return;
            }
            UIElements.AddRange(items);
            foreach (UIElement item in items)
            {
                if (item.Parent != null)
                {
                    item.Parent.Children.Remove(item);
                }
                item.Parent = owner;
                list = null;
                AddUpdateZindex(item);
                owner.RaiseUIElementAdded(item);
            }
        }

        public void Clear()
        {
            UIElement[] v = UIElements.ToArray();
            UIElements.Clear();
            list = null;
            foreach (UIElement item in v)
            {
                item.Parent = null;
                owner.RaiseUIElementRemoved(item);
            }
            orderByZIndexList = UIElements;
        }

        public bool Contains(UIElement value)
        {
            return UIElements.Contains(value);
        }

        public int IndexOf(UIElement value)
        {
            return UIElements.IndexOf(value);
        }

        /// <summary>
        /// 将控件插入到指定索引处
        /// </summary>
        /// <param name="index"></param>
        /// <param name="visual"></param>
        public void Insert(int index, UIElement visual)
        {
            //if (!controls.Contains(visual))
            //{
            UIElement parent = visual.Parent as UIElement;
            if (parent != null)//如果原来已经添加在其他控件集合里
            {
                parent.Children.Remove(visual);//移出控件
            }
            UIElements.Insert(index, visual);
            visual.Parent = owner;
            list = null;
            AddUpdateZindex(visual);
            owner.RaiseUIElementAdded(visual);
            //}
        }

        bool IList.Contains(object value)
        {
            return UIElements.Contains((UIElement)value);
        }

        int IList.IndexOf(object value)
        {
            return UIElements.IndexOf((UIElement)value);
        }

        void IList.Insert(int index, object value)
        {
            UIElement visual = value as UIElement;
            if (visual != null)
            {
                Insert(index, visual);
            }
        }

        int IList.Add(object value)
        {
            UIElement v = value as UIElement;
            if (v != null)
            {
                Add(v);
                return UIElements.Count - 1;
            }
            return -1;
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            UIElement v = value as UIElement;
            if (v != null)
            {
                Remove(v);
            }
        }

        object IList.this[int index]
        {
            get
            {
                return UIElements[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection.IsSynchronized
        {
            get { return true; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        public UIElement this[int index]
        {
            get { return UIElements[index]; }
            set
            {
                if (value == null)
                {
                    throw new Exception("UIElement不能为空");
                }
                var old = UIElements[index];
                if (old == value)
                {
                    return;
                }
                if (value.Parent != null)
                {
                    value.Parent.Children.Remove(value);
                }

                old.Parent = null;
                list = null;
                RemoveUpdateZindex(old);
                owner.RaiseUIElementRemoved(old);

                value.Parent = owner;
                UIElements[index] = value;
                list = null;
                AddUpdateZindex(value);
                owner.RaiseUIElementAdded(value);
            }
        }

        public UIElement this[string name]
        {
            get
            {
                foreach (UIElement item in UIElements)
                {
                    if (item.Name == name)
                    {
                        return item;
                    }
                }
                return null;
            }
        }

        public void Remove(UIElement value)
        {
            if (value == null || UIElements.Count == 0)
            {
                return;
            }
            UIElements.Remove(value);
            value.Parent = null;
            list = null;
            RemoveUpdateZindex(value);
            owner.RaiseUIElementRemoved(value);
        }

        public void RemoveAt(int index)
        {
            UIElement visual = UIElements[index];
            UIElements.RemoveAt(index);
            visual.Parent = null;
            list = null;
            RemoveUpdateZindex(visual);
            owner.RaiseUIElementRemoved(visual);
        }

        public void RemoveRange(int start, int count)
        {
            UIElement[] vs = new UIElement[count];
            int index = 0;
            for (int i = start; i < start + count; i++)
            {
                vs[index] = UIElements[i];
                index++;
            }
            UIElements.RemoveRange(start, count);
            list = null;
            foreach (UIElement item in vs)
            {
                item.Parent = null;
                RemoveUpdateZindex(item);
                owner.RaiseUIElementRemoved(item);
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (array != null && array.Length > 0)
            {
                UIElements.CopyTo((UIElement[])array, index);
            }
        }

        public int Count
        {
            get { return UIElements.Count; }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public UIElement Owner
        {
            get
            {
                return owner;
            }
        }

        UIElement IList<UIElement>.this[int index]
        {
            get
            {
                return UIElements[index];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 使用指定的比较器对所有元素进行排序
        /// </summary>
        /// <param name="comparer">比较元素时要使用的比较器</param>
        public void Sort(Comparer<UIElement> comparer)
        {
            UIElements.Sort(comparer);
        }
        /// <summary>
        /// 使用指定的比较器对所有元素进行排序。
        /// </summary>
        /// <param name="comparer">比较元素时要使用的比较器</param>
        public void Sort(IComparer<UIElement> comparer)
        {
            UIElements.Sort(comparer);
        }
        /// <summary>
        /// 使用指定的 System.Comparison  对整个 UIElementCollection 中的元素进行排序。
        /// </summary>
        /// <param name="comparison"></param>
        public void Sort(Comparison<UIElement> comparison)
        {
            UIElements.Sort(comparison);
        }

        /// <summary>
        /// 使用指定的比较器对某个范围内的元素进行排序。
        /// </summary>
        /// <param name="index">要排序的范围的从零开始的起始索引。</param>
        /// <param name="count">要排序的范围的长度</param>
        /// <param name="comparer">比较元素时要使用的比较器</param>
        public void Sort(int index, int count, IComparer<UIElement> comparer)
        {
            UIElements.Sort(index, count, comparer);
        }

        /// <summary>
        /// 搜索与指定谓词所定义的条件相匹配的元素，并返回整个集合中的第一个匹配元素
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public UIElement Find(Predicate<UIElement> match)
        {
            return UIElements.Find(match);
        }
        /// <summary>
        /// 确定 集合 是否包含与指定谓词所定义的条件相匹配的元素
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public bool Exists(Predicate<UIElement> match)
        {
            return UIElements.Exists(match);
        }
        /// <summary>
        /// 检索与指定谓词定义的条件匹配的所有元素
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<UIElement> FindAll(Predicate<UIElement> match)
        {
            return UIElements.FindAll(match);
        }
        /// <summary>
        /// 搜索与指定谓词所定义的条件相匹配的元素，并返回整个集合 中第一个匹配元素的从零开始的索引。
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int FindIndex(Predicate<UIElement> match)
        {
            return UIElements.FindIndex(match);
        }

        /// <summary>
        /// 搜索与指定谓词所定义的条件相匹配的元素，并返回整个 集合 中的最后一个匹配元素。
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public UIElement FindLast(Predicate<UIElement> match)
        {
            return UIElements.FindLast(match);
        }

        /// <summary>
        /// 搜索与指定谓词所定义的条件相匹配的元素，并返回整个 集合 中最后一个匹配元素的从零开始的索引。
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int FindLastIndex(Predicate<UIElement> match)
        {
            return UIElements.FindLastIndex(match);
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)UIElements).GetEnumerator();
        }
        public IEnumerator<UIElement> GetEnumerator()
        {
            return UIElements.GetEnumerator();
        }

        public void CopyTo(UIElement[] array, int arrayIndex)
        {
            UIElements.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// 释放并清空子元素
        /// </summary>
        internal void DisposeChildren()
        {
            var count = UIElements.Count;
            for (int i = 0; i < count; i++)
            {
                //UIElements[count - i - 1].Dispose();
                var item = UIElements[count - i - 1];
                if (item is Controls.ContentControl c)
                {
                    c.Content = null;
                }
                item.DataContext = null;
                item.Dispose();
            }
        }

        bool ICollection<UIElement>.Remove(UIElement item)
        {
            var r = item.Parent == owner;
            Remove(item);
            return r;
        }

        public override string ToString()
        {
            return Count.ToString();
        }
    }
}
