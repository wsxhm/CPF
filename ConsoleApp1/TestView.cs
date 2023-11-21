using CPF;
using CPF.Animation;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class TestView : Control
    {
        protected override void InitializeComponent()
        {
            Background = "linear-gradient(0 0,100% 0,#E0BB45 0,#65BFC6 1)";
            Height = 362;
            Width = 644;
            Children.Add(new Button
            {
                Content = new CheckBox
                {
                    Content = "CheckBoxaaaaaaaaaaaaaaa",
                },
                MarginLeft = 0,
                MarginTop = 27,
                Height = 52,
                Width = 220,
            });
            Children.Add(new ScrollBar
            {
                MarginRight = 11,
                MarginTop = 5,
                Height = 158,
                Width = 29,
            });
            Children.Add(new Slider
            {
                MarginLeft = 0,
                MarginTop = 0,
                Width = 176,
                Height = 22,
            });
            Children.Add(new TabControl
            {
                MarginLeft = 307,
                MarginTop = 34,
                Height = 198,
                Width = 256,
                Items =
                {
                    new TabItem
                    {
                        Content = new Panel
                        {
                            Children =
                            {
                                new ListBox
                                {
                                    Height = 117,
                                    Width = 148,
                                },
                            },
                            Height = "100%",
                            Width = "100%",
                        },
                        Header="测试",
                    },
                    new TabItem
                    {
                        Header="测试"
                    },
                }
            });
            Children.Add(new ComboBox
            {
                MarginLeft = 60,
                MarginTop = 113,
            });
        }
    }
}
