using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace CPF.Controls
{
    public class ItemCollection : IList, ICollection, IEnumerable, INotifyCollectionChanged, ISortNotify, IDisposed
    {
        internal CpfObject owner;
        public ItemCollection()
        {
            //list = new ObservableCollection<object>();
            list = new Collection<object>();
            ((INotifyCollectionChanged)list).CollectionChanged += Notify_CollectionChanged;
        }
        public ItemCollection(INotifyCollectionChanged notify)
        {
            var list = notify as IList;
            for (int i = 0; i < list.Count; i++)
            {
                indexs.Add(i);
            }
            this.list = list;
            if (notify is ISortNotify sortNotify)
            {
                sortNotify.Sorted += SortNotify_Sorted;
                sortNotify.StartSort += SortNotify_StartSort;
            }
            notify.CollectionChanged += Notify_CollectionChanged;
            //Add(list);
        }

        WeakEventHandlerList events;
        /// <summary>
        /// 事件列表，用于优化事件订阅内存
        /// </summary>
        protected WeakEventHandlerList Events
        {
            get
            {
                if (events == null)
                {
                    events = new WeakEventHandlerList();
                }
                return events;
            }
        }

        private void SortNotify_Sorted(object sender, EventArgs e)
        {
            //Sorted?.Invoke(this, EventArgs.Empty);
            Events[nameof(Sorted)]?.Invoke(this, e);
        }

        private void SortNotify_StartSort(object sender, EventArgs e)
        {
            Events[nameof(StartSort)]?.Invoke(this, e);
        }

        public object this[int index]
        {
            get
            {
                return list[indexs[index]];
            }
            set
            {
                list[indexs[index]] = value;
            }
        }
        /// <summary>
        /// 通过排序之后的索引或者源数据的索引
        /// </summary>
        /// <param name="sortedIndex"></param>
        /// <returns></returns>
        public int IndexOf(int sortedIndex)
        {
            return indexs[sortedIndex];
        }

        List<int> indexs = new List<int>();
        IList list;

        public int Count
        {
            get { return indexs.Count; }
        }

        enum Changed : byte
        {
            None,
            This,
            INotyfyList,
        }
        Changed changed = Changed.None;

        private void Notify_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (changed == Changed.None)
            {
                changed = Changed.INotyfyList;
                int index = 0;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in e.NewItems)
                        {
                            //this.Insert(e.NewStartingIndex, item);
                            AddIndex(item, e.NewStartingIndex + index);
                            index++;
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        //this.Move(e.OldStartingIndex, e.NewStartingIndex);
                        int max, min;
                        bool add;
                        if (e.NewStartingIndex > e.OldStartingIndex)
                        {
                            add = false;
                            max = e.NewStartingIndex;
                            min = e.OldStartingIndex;
                        }
                        else
                        {
                            add = true;
                            max = e.OldStartingIndex;
                            min = e.NewStartingIndex;
                        }
                        for (int i = 0; i < indexs.Count; i++)
                        {
                            var r = indexs[i];
                            if (r >= min && r <= max)
                            {
                                if (e.OldStartingIndex == r)
                                {
                                    indexs[i] = e.NewStartingIndex;
                                }
                                else
                                {
                                    if (add)
                                    {
                                        indexs[i]++;
                                    }
                                    else
                                    {
                                        indexs[i]--;
                                    }
                                }

                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        for (int i = 0; i < indexs.Count; i++)
                        {
                            index = indexs[i];
                            if (index >= e.OldStartingIndex && index < e.OldStartingIndex + e.OldItems.Count)
                            {
                                indexs.Remove(index);
                                i--;
                            }
                            else if (index >= e.OldStartingIndex + e.OldItems.Count)
                            {
                                indexs[i] -= e.OldItems.Count;
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        //this[e.NewStartingIndex] = e.NewItems[0];
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        //this.Clear();
                        indexs.Clear();
                        break;
                }

                changed = Changed.None;
            }
            //if (CollectionChanged != null)
            //{
            //    CollectionChanged(this, e);
            //}

            Events[nameof(CollectionChanged)]?.Invoke(this, e);
        }

        //void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        //{
        //    if (changed == Changed.None && ItemSource != null)
        //    {
        //        changed = Changed.This;
        //        IList list = InnerList;
        //        if (list == null)
        //        {
        //            list = ItemSource;
        //        }
        //        if (!list.IsFixedSize)
        //        {
        //            switch (e.Action)
        //            {
        //                case NotifyCollectionChangedAction.Add:
        //                    foreach (var item in e.NewItems)
        //                    {
        //                        list.Insert(e.NewStartingIndex, item);
        //                    }
        //                    break;
        //                case NotifyCollectionChangedAction.Move:
        //                    list.RemoveAt(e.OldStartingIndex);
        //                    list.Insert(e.NewStartingIndex, e.NewItems[0]);
        //                    break;
        //                case NotifyCollectionChangedAction.Remove:
        //                    for (int i = e.OldItems.Count - 1; i >= 0; i++)
        //                    {
        //                        list.RemoveAt(e.OldStartingIndex + i);
        //                    }
        //                    break;
        //                case NotifyCollectionChangedAction.Replace:
        //                    if (!isSort)
        //                    {
        //                        list[e.NewStartingIndex] = e.NewItems[0];
        //                    }
        //                    break;
        //                case NotifyCollectionChangedAction.Reset:
        //                    list.Clear();
        //                    break;
        //            }
        //        }
        //        changed = Changed.None;
        //    }
        //}

        //public void Sort(IComparer<object> comparer)
        //{
        //    ((List<object>)Items).Sort(comparer);
        //    //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, this));
        //}
        //public void Sort(int index, int count, IComparer<object> comparer)
        //{
        //    ((List<object>)Items).Sort(index, count, comparer);
        //    //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, this));
        //}
        internal Comparison<object> comparison;
        string propertyName;
        bool descending;
        //bool isSort;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { Events.AddHandler(value); }
            remove { Events.RemoveHandler(value); }
        }

        public event EventHandler Sorted
        {
            add { Events.AddHandler(value); }
            remove { Events.RemoveHandler(value); }
        }
        /// <summary>
        /// 开始排序
        /// </summary>
        public event EventHandler StartSort
        {
            add { Events.AddHandler(value); }
            remove { Events.RemoveHandler(value); }
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="comparison">比较器，如果设置了缓存属性，比较的参数就是属性值</param>
        /// <param name="propertyName">缓存的属性</param>
        /// <param name="descending">降序</param>
        public void Sort(Comparison<object> comparison, string propertyName = null, bool descending = false)
        {//选择排序
            Events[nameof(StartSort)]?.Invoke(this, EventArgs.Empty);

            this.comparison = comparison;
            this.propertyName = propertyName;
            this.descending = descending;
            var count = this.Count;
            if (comparison == null)
            {
                for (int i = 0; i < count; i++)
                {//重新设置索引
                    indexs[i] = i;
                }
                return;
            }
            KeyValue<int, object>[] list = new KeyValue<int, object>[count];
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                for (int i = 0; i < count; i++)
                {
                    list[i] = new KeyValue<int, object>(i, this.list[i]);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    list[i] = new KeyValue<int, object>(i, this.list[i].GetPropretyValue(propertyName));
                }
            }
            QuickSort(list, 0, count - 1, comparison);
            if (descending)
            {
                for (int i = 0; i < count; i++)
                {
                    indexs[i] = list[(count - 1) - i].Key;
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    indexs[i] = list[i].Key;
                }
            }

            Events[nameof(Sorted)]?.Invoke(this, EventArgs.Empty);
            //Sorted?.Invoke(this, EventArgs.Empty);
        }

        private int Division(KeyValue<int, object>[] list, int left, int right, Comparison<object> comparison)
        {
            while (left < right)
            {
                var num = list[left]; //将首元素作为枢轴
                if (comparison(num.Value, list[left + 1].Value) > 0)//(num > list[left + 1])
                {
                    list[left] = list[left + 1];
                    list[left + 1] = num;
                    left++;
                }
                else
                {
                    var temp = list[right];
                    list[right] = list[left + 1];
                    list[left + 1] = temp;
                    right--;
                }
                //Console.WriteLine(string.Join(",", list));
            }
            //Console.WriteLine("--------------\n");
            return left; //指向的此时枢轴的位置
        }
        private void QuickSort(KeyValue<int, object>[] list, int left, int right, Comparison<object> comparison)
        {
            if (left < right)
            {
                int i = Division(list, left, right, comparison);
                //对枢轴的左边部分进行排序
                QuickSort(list, i + 1, right, comparison);
                //对枢轴的右边部分进行排序
                QuickSort(list, left, i - 1, comparison);
            }
        }

        public void Add(IEnumerable list)
        {
            foreach (var item in list)
            {
                Add(item);
            }
        }

        public void Add(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            list.Add(item);
            //AddIndex(item, indexs.Count);
        }

        private void AddIndex(object item, int index)
        {
            var length = list.Count;
            if (comparison != null && !string.IsNullOrWhiteSpace(propertyName))
            {
                for (int i = 0; i < indexs.Count; i++)
                {
                    if (indexs[i] >= index)
                    {
                        indexs[i]++;
                    }
                }
                var data = item.GetPropretyValue(propertyName);
                bool hasSet = false;
                if (descending)
                {
                    for (int i = length - 2; i >= 0; i--)
                    {
                        if (comparison(data, list[indexs[i]].GetPropretyValue(propertyName)) <= 0)
                        {
                            hasSet = true;
                            indexs.Insert(i + 1, index);
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = length - 2; i >= 0; i--)
                    {
                        if (comparison(data, list[indexs[i]].GetPropretyValue(propertyName)) >= 0)
                        {
                            hasSet = true;
                            indexs.Insert(i + 1, index);
                            break;
                        }
                    }
                }
                if (!hasSet)
                {
                    indexs.Insert(0, index);
                }

            }
            else
            {
                indexs.Insert(index, index);
                for (int i = index + 1; i < length; i++)
                {
                    indexs[i]++;
                }
            }
        }

        int IList.Add(object value)
        {
            Add(value);
            return indexs.Count - 1;
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(object value)
        {
            return list.Contains(value);
        }
        /// <summary>
        /// 排序后的索引
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(object value)
        {
            //return list.IndexOf(value);
            var index = -1;
            for (int i = 0; i < Count; i++)
            {
                if (this[i] == value)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        /// <summary>
        /// 排序之后index无效
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Insert(int index, object value)
        {
            list.Insert(index, value);
        }

        public void Remove(object value)
        {
            list.Remove(value);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(indexs[index]);
        }

        public void CopyTo(Array array, int index)
        {
            list.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        void IDisposable.Dispose()
        { }

        public override string ToString()
        {
            return base.ToString() + " Count=" + Count;
        }
        //public void Dispose()
        //{
        //    Clear();
        //    events?.Dispose();
        //    isDisposed = true;
        //}

        /// <summary>
        /// Items=...
        /// </summary>
        public IList ItemSource
        {
            get { return list; }
            internal set
            {
                if (list is ISortNotify sortNotify)
                {
                    sortNotify.Sorted -= SortNotify_Sorted;
                    sortNotify.StartSort -= SortNotify_StartSort;
                }
                if (value is INotifyCollectionChanged notifyCollection)
                {
                    ((INotifyCollectionChanged)list).CollectionChanged -= Notify_CollectionChanged;
                    list.Clear();
                    list = (IList)notifyCollection;
                    for (int i = 0; i < list.Count; i++)
                    {
                        indexs.Add(i);
                    }
                    notifyCollection.CollectionChanged += Notify_CollectionChanged;
                }
                else
                {
                    list.Clear();
                    foreach (var item in value)
                    {
                        list.Add(item);
                    }
                }
                if (list is ISortNotify sortNotify1)
                {
                    sortNotify1.Sorted += SortNotify_Sorted;
                    sortNotify1.StartSort += SortNotify_StartSort;
                }
            }
        }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public bool IsSynchronized => false;

        public object SyncRoot => this;

        public bool IsDisposed => owner != null ? owner.IsDisposed : false;

        //bool isDisposed;
        //public bool IsDisposed => isDisposed;

        object IList.this[int index] { get => this[index]; set => this[index] = value; }

        public static implicit operator ItemCollection(Array list)
        {
            var c = new ItemCollection();
            //foreach (var item in list)
            //{
            //    c.Add(item);
            //}
            c.ItemSource = list;
            return c;
        }

        public static implicit operator ItemCollection(List<object> list)
        {
            var c = new ItemCollection();
            //foreach (var item in list)
            //{
            //    c.Add(item);
            //}
            c.ItemSource = list;
            return c;
        }
    }
}
