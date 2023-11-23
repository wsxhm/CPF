using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using CPF.Toolkit.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Toolkit.Demo
{
    public class MainView : Window
    {
        public MainView()
        {

        }
        MainViewModel vm = new MainViewModel();
        protected override void InitializeComponent()
        {
            Title = "标题";
            Width = 500;
            Height = 400;
            Background = null;
            this.DataContext = this.CommandContext = vm;

            Children.Add(new WindowFrame(this, new WrapPanel
            {
                Orientation = Orientation.Horizontal,
                Size = SizeField.Fill,
                Children =
                {
                    new Button
                    {
                        Content = "alert",
                        Commands = { { nameof(Button.Click),(s,e) => vm.Dialog.Alert("这是一条测试消息") } }
                    },
                    new Button
                    {
                        Content = "Sucess",
                        Commands = { { nameof(Button.Click),(s,e) => vm.Dialog.Sucess("这是一条测试消息") } }
                    },
                    new Button
                    {
                        Content = "Error",
                        Commands = { { nameof(Button.Click),(s,e) => vm.Dialog.Error("这是一条测试消息") } }
                    },
                    new Button
                    {
                        Content = "Ask",
                        Commands = { { nameof(Button.Click),(s,e) => vm.Dialog.Ask("这是一条测试消息") } }
                    },
                    new Button
                    {
                        Content = "Warn",
                        Commands = { { nameof(Button.Click),(s,e) => vm.Dialog.Warn("这是一条测试消息") } }
                    },
                    new Button
                    {
                        Content = "关闭窗体",
                        Commands = { { nameof(Button.Click),(s,e) => vm.Test() } }
                    },
                    new Button
                    {
                        Content = "loading",
                        Commands = { { nameof(Button.Click),(s,e) => vm.LoadingTest() } }
                    },
                    new Button
                    {
                        Content = "AsyncButton",
                    }.Assign(out var asyncButton),
                }
            }));

            asyncButton.AsyncClick += AsyncButton_AsyncClick;
        }

        private async Task AsyncButton_AsyncClick(object sender, RoutedEventArgs e)
        {
            await this.vm.AsyncClick();
        }
    }
}
