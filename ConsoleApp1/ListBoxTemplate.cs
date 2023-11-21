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
    [CPF.Design.DesignerLoadStyle("res://ConsoleApp1/Stylesheet1.css")]//用于设计的时候加载样式
    public class ListBoxTemplate : ListBoxItem
    {
        protected override void InitializeComponent()
        {
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
                FontSize = 16,
                Bindings =
                {
                    {
                        nameof(TextBlock.Text),
                        nameof(ItemData.Name)
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
