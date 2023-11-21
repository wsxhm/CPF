using System;
using System.Collections.Generic;
using System.Text;
using CPF.Controls;
using CPF.Shapes;

namespace ConsoleApp1
{
    public class Template : CPF.Controls.Control
    {
        protected override void InitializeComponent()
        {
            Children.Add(new TextBlock { Text = "测试" });
            Children.Add(new Line { StartPoint = new CPF.Drawing.Point(), EndPoint = new CPF.Drawing.Point(20,20), StrokeFill="#f00" });
        }
    }
}
