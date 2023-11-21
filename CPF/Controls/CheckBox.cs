using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Input;
using CPF.Shapes;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 表示用户可以选择和清除的控件。
    /// </summary>
    [Description("表示用户可以选择和清除的控件。")]
    public class CheckBox : ToggleButton
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Add aditional keys "+" and "-" when we are not in IsThreeState mode
            if (!IsThreeState)
            {
                if (e.Key == Keys.OemPlus || e.Key == Keys.Add)
                {
                    e.Handled = true;
                    IsChecked = true;
                }
                else if (e.Key == Keys.OemMinus || e.Key == Keys.Subtract)
                {
                    e.Handled = true;
                    IsChecked = false;
                }
            }
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
                            new Border
                            {
                                Name="checkBoxBorder",
                                Width = 13,
                                Height = 13,
                                BorderFill = "124,124,124",
                                BorderStroke= "1",
                                Background = "255,255,255",
                                UseLayoutRounding=true,//IsAntiAlias=true,
                                Child=new Polyline
                                {
                                    Points=
                                    {
                                        new Point(2,6),
                                        new Point(6,10),
                                        new Point(12,2)
                                    },
                                    IsAntiAlias=true,
                                    StrokeStyle="1.5",
                                    Bindings =
                                    {
                                        {
                                            nameof(Visibility),
                                            nameof(IsChecked),
                                            4,
                                            BindingMode.OneWay,
                                            a=>(bool?)a==true?Visibility.Visible:Visibility.Collapsed
                                        }
                                    }
                                },
                            },
                            new Rectangle
                            {
                                Name="indeterminateMark",
                                Width = 9,
                                Height = 9,
                                Fill = "124,124,124",
                                StrokeFill=null,
                                IsAntiAlias=true,
                                Bindings =
                                {
                                    {
                                        nameof(Visibility),
                                        nameof(IsChecked),
                                        3,
                                        BindingMode.OneWay,
                                        a=>a==null?Visibility.Visible:Visibility.Collapsed
                                    }
                                }
                            },
                        },
                        Height="100%"
                    },
                    new Border
                    {
                        Name = "contentPresenter",
                        MarginLeft=2,
                        BorderFill=null,
                        PresenterFor=this,
                        Height="100%"
                    }
                }
            });
        }
    }
}
