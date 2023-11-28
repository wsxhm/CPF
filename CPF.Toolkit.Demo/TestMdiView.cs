using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using CPF.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Toolkit.Demo
{
    public class TestMdiView : Window
    {
        protected override void InitializeComponent()
        {
            this.CanResize = true;
            this.Title = "标题";
            this.Width = 1280;
            this.Height = 720;
            this.Background = null;
            var frame = this.Children.Add(new WindowFrame(this, new MdiHost
            {
                Children =
                {
                    new MdiWindow{ Content = new Grid{ }, Title = "test1"},
                    //new MdiWindow{ Content = new Grid{ }, Title = "test2"},
                    //new MdiWindow{ Content = new Grid{ }, Title = "test3"},
                },
            }));
            frame.CaptionBackgrund = "white";
            frame.CaptionForeground = "black";
            frame.ControlBoxStroke = "black";
            frame.MaximizeBox = true;
        }
    }
}
