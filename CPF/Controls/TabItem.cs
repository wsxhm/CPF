using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;
using CPF.Styling;

namespace CPF.Controls
{
    /// <summary>
    /// 表示 TabControl 内某个可选择的项
    /// </summary>
    [Description("表示 TabControl 内某个可选择的项"), Browsable(false)]
    public class TabItem : ContentControl, IHeadered
    {
        /// <summary>
        /// 获取，该值指示是否选择 TabItem。
        /// </summary>
        public bool IsSelected
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        internal bool InnerSetIsSelected;

        /// <summary>
        /// 获取或设置每个控件的标题所用的数据。
        /// </summary>
        [TypeConverter(typeof(StringConverter))]
        public object Header
        {
            get { return GetValue<object>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置用于显示控件标头的内容的模板。
        /// </summary>
        [Browsable(false)]
        public UIElementTemplate<ContentTemplate> HeaderTemplate
        {
            get { return GetValue<UIElementTemplate<ContentTemplate>>(); }
            set { SetValue(value); }
        }

        protected override void InitializeComponent()
        {
            Children.Add(new Border
            {
                Background = null,
                BorderFill = null,
                BorderType = BorderType.BorderThickness,
                BorderThickness = new Thickness(1, 1, 1, 0),
                Child =
                new ContentControl
                {
                    MarginBottom = 5,
                    MarginLeft = 5,
                    MarginRight = 5,
                    MarginTop = 5,
                    Bindings = {
                    { nameof(Content), nameof(Header), this },
                    { nameof(ContentTemplate), nameof(HeaderTemplate), this } }
                }
            });

            Foreground = "#666";
            //this.Triggers.Add(new Trigger { Property = nameof(IsMouseOver), PropertyConditions = a => (bool)a && !IsSelected, Setters = { { nameof(Background), "232,242,252" }, { nameof(BorderFill), "126,180,234" } } });
            this.Triggers.Add(new Trigger { Property = nameof(IsSelected), TargetRelation = Relation.Me.Children(a => a is Border), Setters = { { nameof(Border.BorderFill), "230,230,230" }, { nameof(Border.Background), "#fff" } } });
            this.Triggers.Add(new Trigger { Property = nameof(IsSelected), TargetRelation = Relation.Me, Setters = { { nameof(Foreground), "#000" } } });
        }

        /// <summary>
        /// 内容的UI元素对象
        /// </summary>
        public UIElement ContentElement
        {
            get { return GetValue<UIElement>(); }
            private set { SetValue(value); }
        }
        ///// <summary>
        ///// 所在的TabControl
        ///// </summary>
        //public TabControl TabControl
        //{
        //    get;private set;
        //}

        protected override void OnContentChanged(object oldValue, object newValue)
        {
            if (newValue is UIElement)
            {
                ContentElement = newValue as UIElement;
            }
            else
            {
                var old = ContentElement;
                var element = ContentElement = ContentTemplate.CreateElement();
                element.DataContext = Content;
                if (element.HasProperty(nameof(Content)))
                {
                    element.SetValue(Content, nameof(Content));
                }
                if (old)
                {
                    old.Dispose();
                }
            }
            //base.OnContentChanged(oldValue, newValue);
        }

        protected override void OnContentTemplateChanged(UIElementTemplate<ContentTemplate> oldValue, UIElementTemplate<ContentTemplate> newValue)
        {
            var old = ContentElement;
            var element = ContentElement = newValue.CreateElement();
            element.DataContext = Content;
            if (element.HasProperty(nameof(Content)))
            {
                element.SetValue(Content, nameof(Content));
            }
            if (old)
            {
                old.Dispose();
            }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(HeaderTemplate), new PropertyMetadataAttribute((UIElementTemplate<ContentTemplate>)new ContentTemplate()));
        }

    }
}
