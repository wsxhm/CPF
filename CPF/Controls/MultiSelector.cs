using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPF.Reflection;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 为允许选择多项的控件提供抽象类。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Description("为允许选择多项的控件提供抽象类")]
    public abstract class MultiSelector<T> : ItemsControl<T> where T : UIElement, ISelectableItem
    {
        public MultiSelector()
        {
            selectedIndexs.CollectionChanged += SelectedIndexs_CollectionChanged;
        }

        bool RaiseSelectionChanged = false;

        private void SelectedIndexs_CollectionChanged(object sender, CollectionChangedEventArgs<int> e)
        {

            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                case CollectionChangedAction.Remove:
                case CollectionChangedAction.Replace:
                    if (!RaiseSelectionChanged)
                    {
                        RaiseSelectionChanged = true;
                        Threading.Dispatcher.MainThread.BeginInvoke(() =>
                        {
                            RaiseSelectionChanged = false;
                            isSetSelectIndex = true;
                            var items = Items as ItemCollection;
                            if (selectedIndexs.Count > 0)
                            {
                                SelectedIndex = selectedIndexs[0];
                            }
                            else
                            {
                                SelectedIndex = -1;
                            }
                            isSetSelectIndex = false;
                            isSetSelectValue = true;
                            if (selectedIndexs.Count == 1 && items.Count > selectedIndexs[0])
                            {
                                var p = SelectedValuePath;
                                var item = items[selectedIndexs[0]];
                                if (!string.IsNullOrWhiteSpace(p) && item != null)
                                {
                                    SelectedValue = item.GetPropretyValue(p);
                                }
                                else
                                {
                                    SelectedValue = item;
                                }
                            }
                            else if (selectedIndexs.Count > 1 && items.Count > 0 && !selectedIndexs.Any(a => a >= items.Count))
                            {
                                SelectedValue = new SelectValues { selectIndexs = selectedIndexs, items = items, SelectedValuePath = SelectedValuePath };
                            }
                            else
                            {
                                SelectedValue = null;
                                if (items.Count == 0)
                                {
                                    isSetSelectIndex = true;
                                    selectedIndexs.Clear();
                                    SelectedIndex = -1;
                                    isSetSelectIndex = false;
                                }
                            }
                            isSetSelectValue = false;
                            SelectedItems = selectedIndexs.Select(a => Items[a]);
                            OnSelectionChanged(EventArgs.Empty);
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 获取当前选定的项。
        /// </summary>
        [Description("获取当前选定的项")]
        public IEnumerable<object> SelectedItems
        {
            get { return GetValue<IEnumerable<object>>(); }
            private set { SetValue(value); }
        }
        Collection<int> selectedIndexs = new Collection<int>();
        /// <summary>
        /// 获取或者设置当前选定的项索引
        /// </summary>
        [NotCpfProperty]
        public Collection<int> SelectedIndexs
        {
            get { return selectedIndexs; }
        }

        int? selectIndex;
        /// <summary>
        /// 获取或者设置当前选定的项的第一个索引
        /// </summary>
        [PropertyMetadata(-1), Description("获取或者设置当前选定的项的第一个索引")]
        public int SelectedIndex
        {
            get { return GetValue<int>(); }
            set
            {
                if (!IsInitialized)
                {
                    selectIndex = value;
                }
                else
                {
                    selectIndex = null;
                    SetValue(value);
                }
            }
        }
        /// <summary>
        /// 获取或设置用于从 SelectedValue 获取 SelectedItem 的路径。
        /// </summary>
        [PropertyMetadata(""), Description("获取或设置用于从 SelectedValue 获取 SelectedItem 的路径。")]
        public string SelectedValuePath
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置通过使用 SelectedItem 而获取的 SelectedValuePath 的值。如果数据量大不建议用这个来设置，如果是多选的时候，类型是IEnumerable数据，可以遍历获取
        /// </summary>
        [Description("获取或设置通过使用 SelectedItem 而获取的 SelectedValuePath 的值。如果数据量大不建议用这个来设置，如果是多选的时候，类型是IEnumerable数据，可以遍历获取")]
        public object SelectedValue
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 选择更改时发生
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            this.RaiseEvent(e, nameof(SelectionChanged));
        }

        bool isSetSelectValue;
        bool isSetSelectIndex;


        [PropertyChanged(nameof(SelectedIndex))]
        void RegisterSelectedIndex(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            //isSetSelectValue = true;

            var index = (int)newValue;
            if (!isSetSelectIndex)
            {
                selectedIndexs.Clear();
                if (index >= 0)
                {
                    selectedIndexs.Add(index);
                }
            }

            //if (index > -1)
            //{
            //    var p = SelectedValuePath;
            //    var item = Items[SelectedIndex];
            //    if (!string.IsNullOrWhiteSpace(p) && item != null)
            //    {
            //        SelectedValue = item.GetPropretyValue(p);
            //    }
            //    else
            //    {
            //        SelectedValue = item;
            //    }
            //}
            //else
            //{
            //    SelectedValue = null;
            //}
            //isSetSelectValue = false;
        }


        [PropertyChanged(nameof(SelectedValue))]
        void RegisterSelectedValue(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (!isSetSelectValue && Items != null)
            {
                var count = Items.Count;
                if (count > 0)
                {
                    selectedIndexs.Clear();
                    var p = SelectedValuePath;
                    if (newValue is IEnumerable<object> enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            if (!string.IsNullOrWhiteSpace(p))
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    if (item.Equal(Items[i].GetPropretyValue(p)))
                                    {
                                        selectedIndexs.Add(i);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    if (item.Equal(Items[i]))
                                    {
                                        selectedIndexs.Add(i);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(p))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                if (newValue.Equal(Items[i].GetPropretyValue(p)))
                                {
                                    selectedIndexs.Add(i);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < count; i++)
                            {
                                if (newValue.Equal(Items[i]))
                                {
                                    selectedIndexs.Add(i);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }


        [PropertyChanged(nameof(SelectedValuePath))]
        void RegisterSelectedValuePath(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var p = (string)newValue;
            isSetSelectValue = true;
            if (!string.IsNullOrWhiteSpace(p) && Items.Count > 0 && SelectedIndex > -1)
            {
                SelectedValue = Items[SelectedIndex].GetPropretyValue(p);
            }
            else
            {
                SelectedValue = SelectedItems.FirstOrDefault();
            }
            isSetSelectValue = false;
        }


        protected override void OnDataCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnDataCollectionChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                //var selectValue = SelectedValue;
                if (e.OldStartingIndex == SelectedIndex)
                {
                    SelectedIndex = -1;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                SelectedIndex = -1;
            }
            else if (e.Action == NotifyCollectionChangedAction.Add && isFirst && ItemsHost && !selectIndex.HasValue)
            {
                for (int i = 0; i < ItemsHost.Children.Count; i++)
                {
                    var item = ItemsHost.Children[i] as ISelectableItem;
                    if (item != null && item.IsSelected)
                    {
                        SelectedIndexs.Add(i);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add && ItemsHost && !isFirst && e.NewStartingIndex >= 0)
            {
                for (int i = 0; i < selectedIndexs.Count; i++)
                {
                    if (selectedIndexs[i] >= e.NewStartingIndex)
                    {
                        selectedIndexs[i]++;
                    }
                }
            }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(SelectedItems), new PropertyMetadataAttribute(new object[0]));
        }

        bool isFirst;
        protected override void OnInitialized()
        {
            isFirst = true;
            base.OnInitialized();
            isFirst = false;
            if (selectIndex.HasValue)
            {
                SelectedIndex = selectIndex.Value;
                selectIndex = null;
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(SelectedIndex))
        //    {
        //        isSetSelectValue = true;

        //        var index = (int)newValue;
        //        if (!isSetSelectIndex)
        //        {
        //            selectedIndexs.Clear();
        //            if (index >= 0)
        //            {
        //                selectedIndexs.Add(index);
        //            }
        //        }

        //        if (index > -1)
        //        {
        //            var p = SelectedValuePath;
        //            var item = Items[SelectedIndex];
        //            if (!string.IsNullOrWhiteSpace(p) && item != null)
        //            {
        //                SelectedValue = item.GetPropretyValue(p);
        //            }
        //            else
        //            {
        //                SelectedValue = item;
        //            }
        //        }
        //        else
        //        {
        //            SelectedValue = null;
        //        }
        //        isSetSelectValue = false;
        //    }
        //    else if (propertyName == nameof(SelectedValue))
        //    {
        //        if (!isSetSelectValue)
        //        {
        //            var count = Items.Count;
        //            if (count > 0)
        //            {
        //                var p = SelectedValuePath;
        //                if (!string.IsNullOrWhiteSpace(p))
        //                {
        //                    for (int i = 0; i < count; i++)
        //                    {
        //                        if (newValue.Equal(Items[i].GetPropretyValue(p)))
        //                        {
        //                            SelectedIndex = i;
        //                            break;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    for (int i = 0; i < count; i++)
        //                    {
        //                        if (newValue.Equal(Items[i]))
        //                        {
        //                            SelectedIndex = i;
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else if (propertyName == nameof(SelectedValuePath))
        //    {
        //        var p = (string)newValue;
        //        isSetSelectValue = true;
        //        if (!string.IsNullOrWhiteSpace(p) && Items.Count > 0 && SelectedIndex > -1)
        //        {
        //            SelectedValue = Items[SelectedIndex].GetPropretyValue(p);
        //        }
        //        else
        //        {
        //            SelectedValue = SelectedItems.FirstOrDefault();
        //        }
        //        isSetSelectValue = false;
        //    }
        //}
    }

    public interface ISelectableItem
    {
        bool IsSetOnOwner { get; set; }
        bool IsSelected { get; set; }

        int Index { get; set; }
    }

    class SelectValues : IEnumerable<object>
    {
        public IList items;
        public Collection<int> selectIndexs;
        public string SelectedValuePath;

        public IEnumerator<object> GetEnumerator()
        {
            return new SelectValuesIEnumerator { items = items, selectIndexs = selectIndexs, SelectedValuePath = SelectedValuePath };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SelectValuesIEnumerator { items = items, selectIndexs = selectIndexs, SelectedValuePath = SelectedValuePath };
        }
    }

    class SelectValuesIEnumerator : IEnumerator<object>
    {
        int position = -1;
        public IList items;
        public Collection<int> selectIndexs;
        public string SelectedValuePath;

        public object Current
        {
            get
            {
                try
                {
                    var item = items[selectIndexs[(byte)position]];
                    if (!string.IsNullOrWhiteSpace(SelectedValuePath) && item != null)
                    {
                        return item.GetPropretyValue(SelectedValuePath);
                    }
                    return item;
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool MoveNext()
        {
            position++;
            return (position < selectIndexs.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose()
        {
            position = -1;
        }
    }
}
