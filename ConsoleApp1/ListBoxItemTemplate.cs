using System;
using System.Collections.Generic;
using System.Text;
using CPF.Controls;
using CPF.Drawing;
using CPF.Styling;
using CPF.Shapes;
using CPF;

namespace ConsoleApp1
{
    public class ListBoxItemTemplate : ListBoxItem
    {
        protected override void InitializeComponent()
        {
            //模板定义
            //CornerRadius="15";
            //BorderFill = "#000";
            //BorderStroke = "1";
            if (DesignMode)
            {
                Width = 200;
            }
            else
            {
                Width = "100%";
            }
            Height = 40;
            Background = "#fff";
            Children.Add(new Ellipse
            {
                IsAntiAlias = true,
                Fill = new TextureFill("url(https://tva1.sinaimg.cn/crop.0.0.180.180.180/7fde8b93jw1e8qgp5bmzyj2050050aa8.jpg)")
                {
                    Stretch = Stretch.Fill
                },
                Width = 30,
                Height = 30,
                MarginLeft = 5,
                StrokeFill = null,
            });
            Children.Add(new TextBlock
            {
                Text = "马大云",
                MarginLeft = 40,
                MarginTop = 5,
                Bindings =
                {
                    {
                        nameof(TextBlock.Text),
                        nameof(ItemData.Name)
                    }
                }
            });
            Children.Add(new TextBlock
            {
                Text = "哈哈",
                MarginLeft = 40,
                MarginTop = 20,
                Foreground = "#666",
                Bindings =
                {
                    {
                        nameof(TextBlock.Text),
                        nameof(ItemData.Introduce)
                    }
                }
            });
            Triggers.Add(new Trigger
            {
                Property = nameof(IsMouseOver),
                PropertyConditions = a => (bool)a && !IsSelected,
                Setters =
                {
                    {
                        nameof(Background),
                        "229,243,251"
                    }
                }
            });
            Triggers.Add(new Trigger
            {
                Property = nameof(IsSelected),
                PropertyConditions = a => (bool)a,
                Setters =
                {
                    {
                        nameof(Background),
                        "203,233,246"
                    }
                }
            });
        }
    }
}
