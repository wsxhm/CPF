using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.Linq;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示包含一段任意类型内容的控件。
    /// </summary>
    [Description("表示包含一段任意类型内容的控件。")]
    [DefaultProperty(nameof(Content))]
    public class ContentControl : Control
    {
        /// <summary>
        /// 内容可以是字符串，UI元素等等
        /// </summary>
        [Description("内容可以是字符串，UI元素等等")]
        [PropertyMetadata(null), TypeConverter(typeof(StringConverter))]
        public object Content { get { return GetValue(); } set { SetValue(value); } }
        /// <summary>
        /// 内容模板
        /// </summary>
        [Browsable(false)]
        public UIElementTemplate<ContentTemplate> ContentTemplate
        {
            get { return GetValue<UIElementTemplate<ContentTemplate>>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个撰写字符串，该字符串指定如果 Content 属性显示为字符串，应如何设置该属性的格式.String.Format
        /// </summary>
        public string ContentStringFormat
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        //protected override void OnChildDesiredSizeChanged(UIElement child)
        //{
        //    if (Width.IsAuto || Height.IsAuto)
        //    {
        //        InvalidateMeasure();
        //    }
        //    else
        //    {
        //        InvalidateArrange();
        //    }
        //}

        ContentTemplate content;

        [PropertyChanged(nameof(Content))]
        void RegisterContent(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            OnContentChanged(oldValue, newValue);
        }
        [PropertyChanged(nameof(ContentTemplate))]
        void RegisterContentTemplate(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var nv = newValue as UIElementTemplate<ContentTemplate>;
            OnContentTemplateChanged(oldValue as UIElementTemplate<ContentTemplate>, nv);
        }
        [PropertyChanged(nameof(ContentStringFormat))]
        void RegisterContentContentStringFormat(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (content)
            {
                content.ContentStringFormat = ContentStringFormat;
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    if (propertyName == nameof(Content))
        //    {
        //        OnContentChanged(oldValue, newValue);
        //    }
        //    else if (propertyName == nameof(ContentTemplate))
        //    {
        //        var nv = newValue as UIElementTemplate<ContentTemplate>;
        //        OnContentTemplateChanged(oldValue as UIElementTemplate<ContentTemplate>, nv);
        //    }
        //    else if (propertyName == nameof(ContentStringFormat))
        //    {
        //        if (content)
        //        {
        //            content.ContentStringFormat = ContentStringFormat;
        //        }
        //    }
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //}

        protected virtual void OnContentChanged(object oldValue, object newValue)
        {
            if (contentPresenter)
            {
                OnSetContentElement(contentPresenter, oldValue as UIElement, newValue as UIElement);
            }
            if (content != null && !(newValue is UIElement))
            {
                if (content.Parent != contentPresenter)
                {
                    contentPresenter.Children.Add(content);
                }
                content.Content = newValue;
            }
            else
            {
                if (content && contentPresenter)
                {
                    contentPresenter.Children.Remove(content);
                }
            }
        }

        protected virtual void OnSetContentElement(UIElement contentPresenter, UIElement oldValue, UIElement newValue)
        {
            if (oldValue != null)
            {
                contentPresenter.Children.Remove(oldValue);
            }
            if (newValue != null)
            {
                contentPresenter.Children.Add(newValue);
            }
        }

        protected virtual void OnContentTemplateChanged(UIElementTemplate<ContentTemplate> oldValue, UIElementTemplate<ContentTemplate> newValue)
        {
            if (content != null)
            {
                if (contentPresenter)
                {
                    contentPresenter.Children.Remove(content);
                }
                content.Dispose();
                content = null;
            }
            if (newValue != null)
            {
                content = newValue.CreateElement();
                content.ContentStringFormat = ContentStringFormat;
                content.Content = Content;
                if (contentPresenter)
                {
                    contentPresenter.Children.Add(content);
                }
            }
        }

        protected override void InitializeComponent()
        {
            Children.Add(new Border
            {
                Name = "contentPresenter",
                Height = "100%",
                Width = "100%",
                BorderFill = null,
                PresenterFor = this
            });
        }
        UIElement contentPresenter;
        protected override void OnInitialized()
        {
            contentPresenter = FindPresenterByName<UIElement>("contentPresenter");
            if (contentPresenter == null)
            {
                //throw new Exception("未找到contentPresenter");
            }
            else
            {
                if (Content is UIElement element)
                {
                    // contentPresenter.Children.Add(element);
                    OnSetContentElement(contentPresenter, null, element);
                }
                else
                {
                    var ct = ContentTemplate;
                    if (ct != null)
                    {
                        content = ct.CreateElement();
                        content.ContentStringFormat = ContentStringFormat;
                        content.Content = Content;
                    }
                    if (content != null)
                    {
                        contentPresenter.Children.Add(content);
                    }
                }
            }
            base.OnInitialized();
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ContentTemplate), new PropertyMetadataAttribute((UIElementTemplate<ContentTemplate>)typeof(ContentTemplate)));
        }
    }
}
