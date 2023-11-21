using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPF.Input;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 选中的内容的模板
    /// </summary>
    public class SelectionItem : ContentControl
    {
        protected override void InitializeComponent()
        {
            MarginLeft = 3;
            MarginRight = 15;
            ClipToBounds = true;
            Children.Add(new Border
            {
                Name = "contentPresenter",
                Height = "100%",
                Width = "100%",
                BorderFill = null,
                PresenterFor = this
            });
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ContentTemplate), new PropertyMetadataAttribute((UIElementTemplate<ContentTemplate>)new SelectionContentTemplate()));
        }
    }

    public class SelectionContentTemplate : ContentTemplate
    {
        protected override void InitializeComponent()
        {
            Width = "100%";
            Action<object, object> action = (content, old) =>
            {
                if (old != null && content != null && old.GetType() == content.GetType())
                {//如果类型不变就不用重新创建元素
                    return;
                }
                if (content != null && !(content is UIElement) && !(content is Image))
                {
                    Child = new TextBlock
                    {
                        MarginLeft = 3,
                        Bindings =
                        {
                            {
                                nameof(TextBlock.Text),
                                nameof(Content),
                                1,
                                BindingMode.OneWay,
                                ConvertString
                            }
                        }
                    };
                }
                else if (content is Image image)
                {
                    Child = new Picture
                    {
                        MaxHeight = "100%",
                        MaxWidth = "100%",
                        StretchDirection = StretchDirection.DownOnly,
                        Stretch = Stretch.Uniform,
                        Bindings =
                         {
                            {
                                nameof(Picture.Source),
                                nameof(Content),
                                1,
                                BindingMode.OneWay
                            }
                         }
                    };
                }
            };
            if (Content != null)
            {
                action(Content, null);
            }
            Commands.Add(nameof(Content), (s, e) => action(((CPFPropertyChangedEventArgs)e).NewValue, ((CPFPropertyChangedEventArgs)e).OldValue));
        }


        protected object ConvertString(object data)
        {
            var d = data as IEnumerable<object>;
            var count = d.Count();
            if (count == 1)
            {
                return ConvertStr(d.First());
            }
            else if (count > 1)
            {
                return string.Join("|", d.Select(ConvertStr));
            }
            return "";
        }

        string ConvertStr(object a)
        {
            if (a is ContentControl contentControl)
            {
                return contentControl.Content == null ? "" : contentControl.Content.ToString();
            }
            else if (a is UIElement element)
            {
                return element.DataContext == null ? "" : element.DataContext.ToString();
            }
            return a.ToString();
        }
    }
}
