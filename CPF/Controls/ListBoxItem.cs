using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Input;

namespace CPF.Controls
{
    /// <summary>
    /// 表示 ListBox 中的可选项。
    /// </summary>
    [Description("表示 ListBox 中的可选项。"), Browsable(false)]
    public class ListBoxItem : ContentControl, ISelectableItem
    {
        /// <summary>
        /// 是否被选中
        /// </summary>
        [Description("是否被选中")]
        public bool IsSelected
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public event EventHandler Selected
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        public event EventHandler Unselected
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        [NotCpfProperty]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsSetOnOwner { get; set; }

        [PropertyChanged(nameof(IsSelected))]
        void OnIsSelected(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var v = (bool)newValue;
            if (!IsSetOnOwner && ListBoxOwner != null)
            {
                if (v)
                {
                    ListBoxOwner.SelectedIndexs.Add(Index);
                }
                else
                {
                    ListBoxOwner.SelectedIndexs.Remove(Index);
                }
            }
            if (v)
            {
                RaiseEvent(EventArgs.Empty, nameof(Selected));
            }
            else
            {
                RaiseEvent(EventArgs.Empty, nameof(Unselected));
            }
        }
        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(IsSelected))
        //    {
        //        var v = (bool)newValue;
        //        if (v)
        //        {
        //            RaiseEvent(EventArgs.Empty, nameof(Selected));
        //        }
        //        else
        //        {
        //            RaiseEvent(EventArgs.Empty, nameof(Unselected));
        //        }
        //    }
        //}

        protected override void InitializeComponent()
        {
            Children.Add(new Border
            {
                Height = "100%",
                Width = "100%",
                BorderFill = null,
                Name = "contentPresenter",
                PresenterFor = this
            });
            //Triggers.Add(new Styling.Trigger { Property = nameof(IsMouseOver), PropertyConditions = a => (bool)a && !IsSelected, Setters = { { nameof(Background), "229,243,251" } } });
            //Triggers.Add(new Styling.Trigger { Property = nameof(IsSelected), Setters = { { nameof(Background), "203,233,246" } } });

            this[nameof(IsMouseOver)] = new Styling.TriggerDescribe(a => (bool)a && !IsSelected, (nameof(Background), "229,243,251"));
            this[nameof(IsSelected)] = new Styling.TriggerDescribe((nameof(Background), "203,233,246"));
        }

        /// <summary>
        /// 获取包含此项目的 ListBox 控件。
        /// </summary>
        [NotCpfProperty]
        public ListBox ListBoxOwner
        {
            get; internal set;
        }

        //protected override void OnContentChanged(object oldValue, object newValue)
        //{
        //    base.OnContentChanged(oldValue, newValue);

        //}

        //protected override void OnUIElementAdded(UIElementAddedEventArgs e)
        //{
        //    base.OnUIElementAdded(e);
        //}

        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    base.OnMouseDown(e);
        //    if (!e.Handled)
        //    {
        //        IsSelected = !IsSelected;
        //    }
        //}
        /// <summary>
        /// 索引ID
        /// </summary>
        //[NotCPFProperty]
        public int Index
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!e.Handled && ListBoxOwner)
            {
                ListBoxOwner.OnItemMouseDown(new ListBoxItemMouseEventArgs(this, (UIElement)e.OriginalSource, e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, e.MiddleButton == MouseButtonState.Pressed, e.Location, e.MouseDevice, e.MouseButton, e.IsTouch));
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (!e.Handled && ListBoxOwner)
            {
                ListBoxOwner.OnItemMouseUp(new ListBoxItemMouseEventArgs(this, (UIElement)e.OriginalSource, e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, e.MiddleButton == MouseButtonState.Pressed, e.Location, e.MouseDevice, e.MouseButton, e.IsTouch));
            }
        }

        protected override void OnDoubleClick(RoutedEventArgs e)
        {
            base.OnDoubleClick(e);
            if (!e.Handled && ListBoxOwner)
            {
                ListBoxOwner.OnItemDoubleClick(new ListBoxItemEventArgs(this));
            }
        }
    }
}
