using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using CPF.Toolkit.Controls;
using CPF.Toolkit.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CPF.Toolkit.Demo
{
    public class MainView : Window
    {
        MainViewModel vm = new MainViewModel();
        protected override void InitializeComponent()
        {
            Title = "标题";
            Width = 1280;
            Height = 720;
            Background = null;
            this.DataContext = this.CommandContext = vm;
            this.CanResize = true;

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
                        Commands =
                        {
                            { nameof(Button.AsyncClick),async (s,e) => await this.vm.AsyncClick() }
                        }
                    },
                    new Button
                    {
                        Content = "Mdi",
                        Commands =
                        {
                            { nameof(Button.Click), (s,e) => new TestMdiView().Show() }
                        }
                    },

                    new Panel
                    {
                    },
                    new PageControl
                    {
                        Height = 35,
                        PageIndex = 1,
                        PageCount = 100,
                        Width = "100%",
                    },
                }
            }));

        }
    }

    internal class MainViewModel : ViewModelBase
    {
        public void Test()
        {
            this.Close();
        }

        protected override void OnClose(ClosingEventArgs e)
        {
            e.Cancel = this.Dialog.Ask("确定要关闭吗") != "确定";
            base.OnClose(e);
        }

        public async void LoadingTest()
        {
            await this.ShowLoading(async () =>
            {
                await Task.Delay(1000);
                Debug.WriteLine(1);
                await Task.Delay(1000);
                Debug.WriteLine(2);
                await Task.Delay(1000);
                Debug.WriteLine(3);
            });
            //await this.ShowLoading(Task.Delay(3000));

            //var result = await this.ShowLoading(async () =>
            //{
            //    await Task.Delay(5000);
            //    return "test";
            //});
            this.Dialog.Sucess("test");
        }

        public async Task AsyncClick()
        {
            await Task.Delay(3000);
            this.Dialog.Alert("test");
        }
    }
}
