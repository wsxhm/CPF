using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 页面管理，Pages里加多个页面，切换显示
    /// </summary>
    [Description("页面管理，Pages里加多个页面，切换显示"), Browsable(true)]
    public class PageManger : Control
    {
        public PageManger()
        {
            pages.CollectionChanged += Pages_CollectionChanged;
        }

        UIElement child;
        private void Pages_CollectionChanged(object sender, CollectionChangedEventArgs<UIElement> e)
        {
            var index = PageIndex;
            if (child != null)
            {
                child.Visibility = Visibility.Collapsed;
            }
            if (index < pages.Count && index >= 0)
            {
                child = pages[index];
                if (child.Parent != this)
                {
                    Children.Add(child);
                }
                child.Visibility = Visibility.Visible;
            }
            else
            {
                child = null;
            }
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    break;
                case CollectionChangedAction.Remove:
                    if (e.OldItem.Parent == this)
                    {
                        Children.Remove(e.OldItem);
                    }
                    break;
                case CollectionChangedAction.Replace:
                    if (e.OldItem.Parent == this)
                    {
                        Children.Remove(e.OldItem);
                    }
                    break;
                case CollectionChangedAction.Sort:
                    break;
            }

            CanBack = index > 0;
            CanForward = index < pages.Count - 1;
        }
        [PropertyChanged(nameof(PageIndex))]
        void OnPageIndex(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var index = (int)newValue;
            if (child != null)
            {
                child.Visibility = Visibility.Collapsed;
            }
            if (index < pages.Count && index >= 0)
            {
                child = pages[index];
                if (child.Parent != this)
                {
                    Children.Add(child);
                }
                child.Visibility = Visibility.Visible;
            }
            else
            {
                child = null;
            }
            CanBack = index > 0;
            CanForward = index < pages.Count - 1;
        }

        /// <summary>
        /// 当前显示的页面索引
        /// </summary>
        public int PageIndex
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 添加的页面
        /// </summary>
        [NotCpfProperty]
        public Collection<UIElement> Pages { get => pages; }

        Collection<UIElement> pages = new Collection<UIElement>();
        /// <summary>
        /// 可以返回
        /// </summary>
        public bool CanBack
        {
            get { return GetValue<bool>(); }
            private set { SetValue(value); }
        }
        /// <summary>
        /// 可以前进
        /// </summary>
        public bool CanForward
        {
            get { return GetValue<bool>(); }
            private set { SetValue(value); }
        }
        /// <summary>
        /// 上一页
        /// </summary>
        public void Previous()
        {
            if (CanBack)
            {
                PageIndex--;
            }
        }
        /// <summary>
        /// 下一页
        /// </summary>
        public void Next()
        {
            if (CanForward)
            {
                PageIndex++;
            }
        }

#if !DesignMode //用户代码写到这里，设计器下不执行，防止设计器出错
        protected override void OnInitialized()
        {
            base.OnInitialized();

        }
        //用户代码

#endif
    }
}
