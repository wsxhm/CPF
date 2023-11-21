using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Input;

namespace CPF.Controls
{
    /// <summary>
    /// 可切换状态的控件基类
    /// </summary>
    [Description("可切换状态的控件基类"), Browsable(true)]
    public class ToggleButton : ButtonBase
    {

        /// <summary>
        /// 获取或设置是否选中
        /// </summary>
        [PropertyMetadata(false)]
        public bool? IsChecked
        {
            get { return GetValue<bool?>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 决定控件是支持两种状态还是支持三种状态。
        /// </summary>
        public bool IsThreeState
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public event EventHandler Checked
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        public event EventHandler Indeterminate
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        public event EventHandler Unchecked
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected virtual void OnChecked(EventArgs e)
        {
            RaiseEvent(e, nameof(Checked));
        }

        protected virtual void OnIndeterminate(EventArgs e)
        {
            RaiseEvent(e, nameof(Indeterminate));
        }
        protected virtual void OnUnchecked(EventArgs e)
        {
            RaiseEvent(e, nameof(Unchecked));
        }

        protected override void OnClick(RoutedEventArgs e)
        {
            base.OnClick(e);
            if (!e.Handled)
            {
                OnToggle();
            }
        }

        protected virtual void OnToggle()
        {
            // If IsChecked == true && IsThreeState == true   --->  IsChecked = null
            // If IsChecked == true && IsThreeState == false  --->  IsChecked = false
            // If IsChecked == false                          --->  IsChecked = true
            // If IsChecked == null                           --->  IsChecked = false
            bool? isChecked;
            if (IsChecked == true)
                isChecked = IsThreeState ? (bool?)null : (bool?)false;
            else // false or null
                isChecked = IsChecked.HasValue; // HasValue returns true if IsChecked==false
            IsChecked = isChecked;
        }

        [PropertyChanged(nameof(IsChecked))]
        void RegisterIsChecked(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var newV = (bool?)newValue;
            if (newV == true)
            {
                OnChecked(EventArgs.Empty);
            }
            else if (newV == false)
            {
                OnUnchecked(EventArgs.Empty);
            }
            else
            {
                OnIndeterminate(EventArgs.Empty);
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(IsChecked))
        //    {
        //        var newV = (bool?)newValue;
        //        if (newV == true)
        //        {
        //            OnChecked(EventArgs.Empty);
        //        }
        //        else if (newV == false)
        //        {
        //            OnUnchecked(EventArgs.Empty);
        //        }
        //        else
        //        {
        //            OnIndeterminate(EventArgs.Empty);
        //        }
        //    }
        //}

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Focusable), new PropertyMetadataAttribute(true));
        }
    }
}
