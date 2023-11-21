using CPF;
using CPF.Animation;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public class ListBoxTemplate : Control
    {
        protected override void InitializeComponent()
        {
            BorderThickness = "0,0,0,1";
            BorderType = BorderType.BorderThickness;
            BorderFill = "#B4B4B4";
            Height = 37.8f;
            Width = 245.7f;
            //模板定义
            Children.Add(new TextBlock
            {
                MarginLeft = 42.1f,
                MarginRight = 13.7f,
                Width = 190f,
                Text = "CPF控件322",
                Bindings =
                {
                    {
                        "Text",
                        "Item1"
                    },
                },
            });
            Children.Add(new Picture
            {
                MarginLeft = 6.1f,
                Width = 28.2f,
                Height = 26.9f,
                Bindings =
                {
                    {
                        nameof(Picture.Source),
                        "Item2"
                    }
                }
            });
            Bindings.Add("a","b");
            Bindings.Add("a1","b2",null,BindingMode.OneTime);
            Bindings.Add("a3","b4");
        }
    }
}
