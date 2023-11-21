using System;
using System.Collections.Generic;
using System.Text;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF;

namespace ConsoleApp1
{
    public class TabItemTemplate : TabItem
    {
        protected override void InitializeComponent()
        {//模板定义
            Width = "100%";
            Children.Add(new Border
            {
                Background = null,
                BorderFill = null,
                MarginLeft = 0,
                Width = "100%",
                Child =
                new ContentControl
                {
                    MarginBottom = 5,
                    MarginLeft = 30,
                    MarginRight = 5,
                    MarginTop = 5,
                    Bindings = {
                    { nameof(Content), nameof(Header), this },
                    { nameof(ContentTemplate), nameof(HeaderTemplate), this } }
                }
            });
            Children.Add(new Picture { Source = "res://ConsoleApp1/Resources/主页.png", Width = 14, Height = 14, MarginLeft = 10, Stretch= Stretch.Fill });
            Children.Add(new Polygon { Points = { { 0, 5 }, { 5, 0 }, { 5, 10 } }, StrokeFill = null, Fill = "#fff", MarginRight = 0, Bindings = { { nameof(Visibility), nameof(IsSelected), this, BindingMode.OneWay, a => (bool)a ? Visibility.Visible : Visibility.Collapsed } } });
        }
    }
}
