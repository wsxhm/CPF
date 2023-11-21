using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace CPF
{
    /// <summary>
    /// 包含项目添加移出事件的泛型集合，由于System.Collections.ObjectModel.ObservableCollection的Clear操作事件里没有OldItems值，所以重新定义个
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Collection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, INotifyCollectionChanged, ISortNotify
    {
        List<T> t = new List<T>();
        //NotifyCollectionChangedEventHandler collectionChanged;

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                //collectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Combine(collectionChanged, value);
                Events.AddHandler(nameof(INotifyCollectionChanged), value);
            }

            remove
            {
                //collectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Remove(collectionChanged, value);
                Events.RemoveHandler(nameof(INotifyCollectionChanged), value);
            }
        }

        ///// <summary>
        ///// 内部列表
        ///// </summary>
        //[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public List<T> InnerList { get { return t; } }

        int IList.Add(object value)
        {
            t.Add((T)value);
            OnCollectionChanged(new CollectionChangedEventArgs<T>((T)value, t.Count - 1, default, CollectionChangedAction.Add));
            return t.Count - 1;
        }

        public void Add(T value)
        {
            t.Add(value);
            OnCollectionChanged(new CollectionChangedEventArgs<T>(value, t.Count - 1, default, CollectionChangedAction.Add));
        }
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Insert(int index, T item)
        {
            t.Insert(index, item);
            OnCollectionChanged(new CollectionChangedEventArgs<T>(item, index, default, CollectionChangedAction.Add));
        }
        ///// <summary>
        ///// 使用指定的比较器对所有元素进行排序
        ///// </summary>
        ///// <param name="comparison">比较元素时要使用的比较器</param>
        //public void Sort(Comparison<T> comparison)
        //{
        //    t.Sort(comparison);
        //    OnCollectionChanged(new CollectionChangedEventArgs<T>(default(T), -1, CollectionChangedAction.Replace));
        //}

        ///// <summary>
        ///// 使用指定的比较器对所有元素进行排序
        ///// </summary>
        ///// <param name="comparer">比较元素时要使用的比较器</param>
        //public void Sort(Comparer<T> comparer)
        //{
        //    t.Sort(comparer);
        //    OnCollectionChanged(new CollectionChangedEventArgs<T>(default(T), -1, CollectionChangedAction.Replace));
        //}
        /// <summary>
        /// 使用指定的比较器对所有元素进行排序。
        /// </summary>
        /// <param name="comparer">比较元素时要使用的比较器</param>
        public void Sort(IComparer<T> comparer)
        {
            Events[nameof(StartSort)]?.Invoke(this, EventArgs.Empty);
            t.Sort(comparer);
            OnCollectionChanged(new CollectionChangedEventArgs<T>(default, -1, default, CollectionChangedAction.Sort));
            //Sorted?.Invoke(this, EventArgs.Empty);
            Events[nameof(Sorted)]?.Invoke(this, EventArgs.Empty);
        }

        ///// <summary>
        ///// 使用指定的比较器对某个范围内的元素进行排序。
        ///// </summary>
        ///// <param name="index">要排序的范围的从零开始的起始索引。</param>
        ///// <param name="count">要排序的范围的长度</param>
        ///// <param name="comparer">比较元素时要使用的比较器</param>
        //public void Sort(int index, int count, IComparer<T> comparer)
        //{
        //    t.Sort(index, count, comparer);
        //    OnCollectionChanged(new CollectionChangedEventArgs<T>(default(T), -1, CollectionChangedAction.Replace));
        //}
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="comparison"></param>
        public void Sort(Comparison<T> comparison)
        {
            Events[nameof(StartSort)]?.Invoke(this, EventArgs.Empty);
            t.Sort(comparison);
            OnCollectionChanged(new CollectionChangedEventArgs<T>(default, -1, default, CollectionChangedAction.Sort));
            //Sorted?.Invoke(this, EventArgs.Empty);
            Events[nameof(Sorted)]?.Invoke(this, EventArgs.Empty);
            ////选择排序
            //var count = this.Count;
            //int min;
            //for (int i = 0; i < count; i++)
            //{
            //    min = i;
            //    for (int j = i + 1; j < count; j++)
            //    {
            //        if (comparison(this[j], this[min]) < 0)//(list[j].TabIndex - list[min].TabIndex < 0)
            //        {
            //            min = j;
            //        }
            //    }
            //    var temp = this[i];
            //    this[i] = this[min];
            //    this[min] = temp;
            //}

            //QuickSort(this, 0, count - 1, comparison);
        }
        //private int Division(Collection<T> list, int left, int right, Comparison<T> comparison)
        //{
        //    while (left < right)
        //    {
        //        var num = list[left]; //将首元素作为枢轴
        //        if (comparison(num, list[left + 1]) > 0)//(num > list[left + 1])
        //        {
        //            list[left] = list[left + 1];
        //            list[left + 1] = num;
        //            left++;
        //        }
        //        else
        //        {
        //            var temp = list[right];
        //            list[right] = list[left + 1];
        //            list[left + 1] = temp;
        //            right--;
        //        }
        //        //Console.WriteLine(string.Join(",", list));
        //    }
        //    //Console.WriteLine("--------------\n");
        //    return left; //指向的此时枢轴的位置
        //}
        //private void QuickSort(Collection<T> list, int left, int right, Comparison<T> comparison)
        //{
        //    if (left < right)
        //    {
        //        int i = Division(list, left, right, comparison);
        //        //对枢轴的左边部分进行排序
        //        QuickSort(list, i + 1, right, comparison);
        //        //对枢轴的右边部分进行排序
        //        QuickSort(list, left, i - 1, comparison);
        //    }
        //}
        /// <summary>
        /// 搜索与指定谓词所定义的条件相匹配的元素，并返回整个集合中的第一个匹配元素
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public T Find(Predicate<T> match)
        {
            return t.Find(match);
        }
        /// <summary>
        /// 确定 集合 是否包含与指定谓词所定义的条件相匹配的元素
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public bool Exists(Predicate<T> match)
        {
            return t.Exists(match);
        }
        /// <summary>
        /// 检索与指定谓词定义的条件匹配的所有元素
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<T> FindAll(Predicate<T> match)
        {
            return t.FindAll(match);
        }
        /// <summary>
        /// 搜索与指定谓词所定义的条件相匹配的元素，并返回整个集合 中第一个匹配元素的从零开始的索引。
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int FindIndex(Predicate<T> match)
        {
            return t.FindIndex(match);
        }

        /// <summary>
        /// 搜索与指定谓词所定义的条件相匹配的元素，并返回整个 集合 中的最后一个匹配元素。
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public T FindLast(Predicate<T> match)
        {
            return t.FindLast(match);
        }

        /// <summary>
        /// 搜索与指定谓词所定义的条件相匹配的元素，并返回整个 集合 中最后一个匹配元素的从零开始的索引。
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int FindLastIndex(Predicate<T> match)
        {
            return t.FindLastIndex(match);
        }
        bool reset = false;
        public void Clear()
        {
            reset = true;
            int c = t.Count;
            //int tem = c;
            //for (int i = 0; i < tem; i++)
            //{
            //    c = t.Count - 1;
            //    RemoveAt(c);
            //}
            for (int i = c - 1; i > -1; i--)
            {
                RemoveAt(i);
            }
            //if (collectionChanged != null)
            //{
            //    collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            //}
            Events[nameof(INotifyCollectionChanged)]?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            reset = false;
        }

        bool IList.Contains(object value)
        {
            return t.Contains((T)value);
        }

        public bool Contains(T value)
        {
            return t.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return t.IndexOf((T)value);
        }

        public int IndexOf(T value)
        {
            return t.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            t.Insert(index, (T)value);
            OnCollectionChanged(new CollectionChangedEventArgs<T>((T)value, index, default, CollectionChangedAction.Add));
        }

        //public void Insert(int index, T value)
        //{
        //    t.Insert(index, value);
        //    OnItemAdded(new CollectionEventArgs<T>(value));
        //}
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
            int index = t.IndexOf((T)value);
            t.RemoveAt(index);
            OnCollectionChanged(new CollectionChangedEventArgs<T>(default, index, (T)value, CollectionChangedAction.Remove));
        }

        public void Remove(T value)
        {
            int index = t.IndexOf(value);
            if (index < 0)
            {
                return;
            }
            t.RemoveAt(index);
            OnCollectionChanged(new CollectionChangedEventArgs<T>(default, index, value, CollectionChangedAction.Remove));
        }

        public void RemoveRange(int start, int count)
        {
            //for (int i = start; i < count; i++)
            //{
            //    RemoveAt(start);
            //}
            if (count <= 0)
            {
                return;
            }
            var l = new T[count];
            t.CopyTo(start, l, 0, count);
            t.RemoveRange(start, count);
            for (int i = count - 1; i >= 0; i--)
            {
                OnCollectionChanged(new CollectionChangedEventArgs<T>(default, start + i, l[i], CollectionChangedAction.Remove));
            }
        }

        public void RemoveAt(int index)
        {
            T i = t[index];
            t.RemoveAt(index);
            OnCollectionChanged(new CollectionChangedEventArgs<T>(default, index, i, CollectionChangedAction.Remove));
        }

        object IList.this[int index]
        {
            get
            {
                return t[index];
            }
            set
            {
                //RemoveAt(index);
                //Insert(index, (T)value);
                var old = t[index];
                if ((old == null && value == null) || (old != null && old.Equals((T)value)))
                {
                    return;
                }
                //OnCollectionChanged(new CollectionChangedEventArgs<T>(old, index, CollectionChangedAction.Remove));
                t[index] = (T)value;
                //OnCollectionChanged(new CollectionChangedEventArgs<T>((T)value, index, CollectionChangedAction.Add));
                OnCollectionChanged(new CollectionChangedEventArgs<T>((T)value, index, old, CollectionChangedAction.Replace));
            }
        }

        public T this[int index]
        {
            get { return t[index]; }
            set
            {
                var old = t[index];
                if ((old == null && value == null) || (old != null && old.Equals(value)))
                {
                    return;
                }
                //OnCollectionChanged(new CollectionChangedEventArgs<T>(old, index, CollectionChangedAction.Remove));
                //t[index] = value;
                //OnCollectionChanged(new CollectionChangedEventArgs<T>(value, index, CollectionChangedAction.Add));
                t[index] = value;
                OnCollectionChanged(new CollectionChangedEventArgs<T>(value, index, old, CollectionChangedAction.Replace));
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)t).CopyTo(array, index);
        }

        public int Count
        {
            get { return t.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        public object Tag
        {
            get
            {
                return tag;
            }

            set
            {
                tag = value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        T IList<T>.this[int index]
        {
            get
            {
                return t[index];
            }

            set
            {
                //RemoveAt(index);
                //Insert(index, value);
                var old = t[index];
                if ((old == null && value == null) || (old != null && old.Equals(value)))
                {
                    return;
                }
                t[index] = value;
                OnCollectionChanged(new CollectionChangedEventArgs<T>(value, index, old, CollectionChangedAction.Replace));
            }
        }

        WeakEventHandlerList events;
        /// <summary>
        /// 事件列表，用于优化事件订阅内存
        /// </summary>
        internal protected WeakEventHandlerList Events
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

        /// <summary>
        /// 项目添加之后
        /// </summary>
        public event EventHandler<CollectionChangedEventArgs<T>> CollectionChanged
        {
            add
            {
                Events.AddHandler(value);
            }
            remove { Events.RemoveHandler(value); }
        }
        /// <summary>
        /// 调用Sort排序之后
        /// </summary>
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

        protected virtual void OnCollectionChanged(CollectionChangedEventArgs<T> e)
        {
            Events[nameof(CollectionChanged)]?.Invoke(this, e);
            //if (CollectionChanged != null)
            //{
            //    CollectionChanged(this, e);
            //}
            var collectionChanged = Events[nameof(INotifyCollectionChanged)];
            if (collectionChanged != null)
            {
                switch (e.Action)
                {
                    case CollectionChangedAction.Add:
                        collectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItem, e.Index));
                        break;
                    case CollectionChangedAction.Remove:
                        if (!reset)
                        {
                            collectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.NewItem, e.Index));
                        }
                        break;
                    case CollectionChangedAction.Replace:
                        collectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItem, e.OldItem, e.Index));
                        break;
                    default:
                        break;
                }
            }
        }

        public T[] ToArray()
        {
            return t.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return t.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)t).GetEnumerator();
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            t.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item)
        {
            var index = t.IndexOf(item);
            if (index < 0)
            {
                return false;
            }
            RemoveAt(index);
            return true;
        }

        public override string ToString()
        {
            return typeof(T).ToString() + " Count:" + t.Count;
        }

        object tag;

    }
    /// <summary>
    /// 提供排序之后的通知
    /// </summary>
    public interface ISortNotify
    {
        event EventHandler StartSort;
        event EventHandler Sorted;
    }

    public struct CollectionChangedEventArgs<T>
    {
        public CollectionChangedEventArgs(T item, int index, T oldItem, CollectionChangedAction action)
        {
            this.NewItem = item;
            this.Index = index;
            Action = action;
            OldItem = oldItem;
        }
        /// <summary>
        /// 被替换的项，Replace的时候才有意义。Item就是替换之后的新的项
        /// </summary>
        public T OldItem { get; set; }

        public int Index { get; private set; }

        public T NewItem { get; private set; }

        public CollectionChangedAction Action { get; private set; }
    }

    public enum CollectionChangedAction : byte
    {
        Add,
        Remove,
        Replace,
        Sort,
    }
}
