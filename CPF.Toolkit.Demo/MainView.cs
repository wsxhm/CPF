using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Toolkit.Demo
{
    public class MainView : Window
    {
        protected override void InitializeComponent()
        {
            //LoadStyleFile("res://CPF.Toolkit.Demo/Stylesheet1.css");

            this.Title = "标题";
            this.Width = 500;
            this.Height = 400;
            this.Background = null;
            var vm = new MainViewModel();
            this.DataContext = this.CommandContext = vm;
            this.Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Children =
                {
                    new Button
                    {
                        Content="按钮",
                        [nameof(Button.Click)] = new CommandDescribe((ss,ee) => vm.Test()),
                    }
                },
            }));
            this.Behaviors.Add(new ViewBehavior());
        }
    }
}
