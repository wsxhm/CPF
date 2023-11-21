using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 提供在单个子元素
    /// </summary>
    [Description("提供在单个子元素")]
    public class Decorator : Control
    {
        /// <summary>
        /// 获取或设置 单一子元素。
        /// </summary>
        [Browsable(false)]
        public UIElement Child
        {
            get { return GetValue<UIElement>(); }
            set { SetValue(value); }
        }

        [PropertyChanged(nameof(Child))]
        void RegisterChild(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var o = oldValue as UIElement;
            if (o != null)
            {
                Children.Remove(o);
            }
            var c = newValue as UIElement;
            if (c != null)
            {
                Children.Add(c);
            }
        }
        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(Child))
        //    {
        //        var o = oldValue as UIElement;
        //        if (o != null)
        //        {
        //            Children.Remove(o);
        //        }
        //        var c = newValue as UIElement;
        //        if (c != null)
        //        {
        //            Children.Add(c);
        //        }
        //    }
        //}

        protected override void OnUIElementRemoved(UIElementRemovedEventArgs e)
        {
            base.OnUIElementRemoved(e);
            if (e.Element == Child)
            {
                Child = null;
            }
        }
    }
}
