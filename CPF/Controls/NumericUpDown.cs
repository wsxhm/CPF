using CPF.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 表示显示数值的 Windows 数字显示框（也称作 up-down 控件）。
    /// </summary>
    [Description("表示显示数值的 Windows 数字显示框（也称作 up-down 控件）。"), Browsable(true)]
    public class NumericUpDown : RangeBase
    {
        protected override void InitializeComponent()
        {
            BorderStroke = "1";
            Background = "#fff";
            BorderFill = "#aaa";
            Children.Add(new Grid
            {
                Size = SizeField.Fill,
                ColumnDefinitions =
                {
                    new ColumnDefinition
                    {
                        Width="auto",
                    },
                    new ColumnDefinition
                    {
                        Width="*",
                    },
                    new ColumnDefinition
                    {
                        Width="auto",
                    },
                },
                Children =
                {
                    new RepeatButton
                    {
                        Height="100%",
                        Name="decreaseBtn",
                        Content="-",
                        Background="221,221,221",
                        Triggers =
                        {
                            { nameof(IsMouseOver),Relation.Me,null,(nameof(Background), "190,230,253") },
                            { nameof(RepeatButton.IsPressed),Relation.Me,null,(nameof(Background), "186,209,226") },
                        },
                        Commands =
                        {
                            {nameof(Button.Click),(s,e)=>Value= Math.Round(Value - Increment, 12) }
                        }
                    },
                    new Border
                    {
                        Name="textBoxBorder",
                        BorderFill="#aaa",
                        BorderType= BorderType.BorderThickness,
                        BorderThickness=new Thickness(1,0),
                        Attacheds =
                        {
                            {Grid.ColumnIndex,1 }
                        },
                        Size= SizeField.Fill,
                        Child= new TextBox
                        {
                            PresenterFor=this,
                            Name="numTextBox",
                            MinWidth=10,
                            AcceptsReturn=false,
                            HScrollBarVisibility= ScrollBarVisibility.Hidden,
                            VScrollBarVisibility= ScrollBarVisibility.Hidden,
                            Commands =
                            {
                                {nameof(LostFocus),(s,e)=>ParseToValue() },
                                {nameof(TextInput),TextBoxTextInput },
                            },
                            Bindings =
                            {
                                {nameof(TextBox.Text),nameof(Value),this,BindingMode.OneWay,(double v)=>ValueToStringConvert==null? v.ToString():ValueToStringConvert(v) }
                            }
                        }
                    },
                    new RepeatButton
                    {
                        Height="100%",
                        Name="increaseBtn",
                        Background="221,221,221",
                        Attacheds =
                        {
                            {Grid.ColumnIndex,2 }
                        },
                        Content="+",
                        Triggers =
                        {
                            { nameof(IsMouseOver),Relation.Me,null,(nameof(Background), "190,230,253") },
                            { nameof(RepeatButton.IsPressed),Relation.Me,null,(nameof(Background), "186,209,226") },
                        },
                        Commands =
                        {
                            {nameof(Button.Click),(s,e)=>Value= Math.Round(Value + Increment, 12) }
                        }
                    }
                }
            });
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            numTextBox = FindPresenterByName<TextBox>("numTextBox");
        }
        TextBox numTextBox;

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!e.Handled)
            {
                if (e.Delta.Y > 0)
                {
                    Value = Math.Round(Value + Increment, 12);
                }
                else if (e.Delta.Y < 0)
                {
                    Value = Math.Round(Value - Increment, 12);
                }
            }
        }
        protected void TextBoxTextInput(CpfObject cpfObject, object e)
        {
            var input = e as TextInputEventArgs;
            if (input.Text.Length != 1 || !(input.Text[0] >= '0' && input.Text[0] <= '9' || input.Text[0] == '.'))
            {
                input.Handled = true;
            }
        }

        protected override void OnAttachedToVisualTree()
        {
            base.OnAttachedToVisualTree();
            Root.PreviewMouseDown += Root_PreviewMouseDown;
        }

        private void Root_PreviewMouseDown(object sender, Input.MouseButtonEventArgs e)
        {
            if (!e.Handled && !numTextBox.IsMouseOver)
            {
                ParseToValue();
            }
        }
        /// <summary>
        /// 控制点击一次向上或者向下小按钮数字输入框值的增减大小
        /// </summary>
        [PropertyMetadata(1d)]
        public double Increment
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 字符串转double转换器
        /// </summary>
        public Func<string, double> StringToValueConvert
        {
            get { return GetValue<Func<string, double>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// double转字符串转换器
        /// </summary>
        public Func<double, string> ValueToStringConvert
        {
            get { return GetValue<Func<double, string>>(); }
            set { SetValue(value); }
        }

        protected void ParseToValue()
        {
            double r;
            if (StringToValueConvert == null)
            {
                double.TryParse(numTextBox.Text, out r);
            }
            else
            {
                r = StringToValueConvert(numTextBox.Text);
            }
            var value = Value;
            if (value == Maximum && r >= value)
            {
                Value = Minimum;
                Value = Maximum;
            }
            else if (value == Minimum && r <= value)
            {
                Value = Maximum;
                Value = Minimum;
            }
            else
            {
                Value = r;
            }
        }

        protected override void OnDetachedFromVisualTree()
        {
            Root.PreviewMouseDown -= Root_PreviewMouseDown;
            base.OnDetachedFromVisualTree();
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Maximum), new PropertyMetadataAttribute(100d));
        }
    }
}
