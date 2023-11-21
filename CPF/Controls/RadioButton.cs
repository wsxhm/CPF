using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;
using CPF.Shapes;

namespace CPF.Controls
{
    /// <summary>
    /// 表示可由用户选择但不能清除的按钮。 可以通过单击来设置 IsChecked 的 RadioButton 属性，但只能以编程方式清除该属性。
    /// </summary>
    [Description("表示可由用户选择但不能清除的按钮。 可以通过单击来设置 IsChecked 的 RadioButton 属性，但只能以编程方式清除该属性。")]
    public class RadioButton : ToggleButton
    {
        /// <summary>
        /// 通过该属性对RadioButton分组，通过Root.GetRadioButtonValue()获取分组的选中值
        /// </summary>
        [PropertyMetadata(""), Description("通过该属性对RadioButton分组，通过Root.GetRadioButtonValue()获取分组的选中值")]
        public string GroupName
        {
            get
            {
                return (string)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        protected override void OnChecked(EventArgs e)
        {
            base.OnChecked(e);
            if (Root != null)
            {
                Root.CheckedRadioButton(this);
            }
        }

        protected override void OnToggle()
        {
            if (IsChecked != true)
                IsChecked = true;
        }

        protected override void OnAttachedToVisualTree()
        {
            base.OnAttachedToVisualTree();
            Root.RegisterRadioButton(this);
        }
        protected override void OnDetachedFromVisualTree()
        {
            base.OnDetachedFromVisualTree();
            Root.UnRegisterRadioButton(this);
        }

        protected override void InitializeComponent()
        {
            Children.Add(new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new Panel
                    {
                        Name="markPanel",
                        Children =
                        {
                            new Ellipse
                            {
                                Name="radioButtonBorder",
                                Width = 15, Height = 15, StrokeFill = "124,124,124", Fill = "255,255,255",IsAntiAlias=true
                            },
                            new Ellipse
                            {
                                Name="optionMark",
                                IsAntiAlias=true,
                                Width = 8,
                                Height = 8,
                                Fill = "124,124,124",
                                Bindings ={ {nameof(Visibility),nameof(IsChecked),3,BindingMode.OneWay,a=>(bool?)a==true?Visibility.Visible:Visibility.Collapsed } }
                            },
                        },
                        Height="100%"
                    },
                        new Border { Name = "contentPresenter",MarginLeft=2, BorderFill=null,PresenterFor=this,Height="100%" }
                }
            });
        }

    }
}
