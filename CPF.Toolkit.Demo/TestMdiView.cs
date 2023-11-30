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
    public class TestMdiView : Window
    {
        protected override void InitializeComponent()
        {
            this.CanResize = true;
            this.Title = "标题";
            this.Width = 1280;
            this.Height = 720;
            this.Background = null;
            var frame = this.Children.Add(new WindowFrame(this, new Grid
            {
                Size = SizeField.Fill,
                RowDefinitions =
                {
                    new RowDefinition{ Height = 30 },
                    new RowDefinition{ },
                },
                Children =
                {
                    new MdiHost
                    {
                        Size = SizeField.Fill,
                        Attacheds = { { Grid.RowIndex,1 } },
                        TaskBarPlacement = TaskBarPlacement.Top,
                    }.Assign(out var host),
                    new WrapPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Size = SizeField.Fill,
                        Children =
                        {
                            new Button
                            {
                                Height = "100%",
                                Content = "New Window",
                                [nameof(Button.Click)] = new CommandDescribe((s,e) => host.Children.Add(new M{ Title = $"Title{host.Children.Count}", })),
                            },
                            new Button
                            {
                                Height = "100%",
                                Content = "任务栏居上",
                                [nameof(Button.Click)] = new CommandDescribe((s,e) => host.TaskBarPlacement = TaskBarPlacement.Top),
                            },
                            new Button
                            {
                                Height = "100%",
                                Content = "任务栏居下",
                                [nameof(Button.Click)] = new CommandDescribe((s,e) => host.TaskBarPlacement = TaskBarPlacement.Bottom),
                            },
                        },
                    },
                },
            }));
            //frame.CaptionBackgrund = "white";
            //frame.CaptionForeground = "black";
            //frame.ControlBoxStroke = "black";
            frame.MaximizeBox = true;
        }
    }

    internal class M : MdiWindow
    {
        protected override void InitializeComponent()
        {
            var vm = new MV { Dialog = new DialogService(this.Root as Window) };
            this.DataContext = vm;
            this.CommandContext = vm;
            this.Content = new WrapPanel
            {
                Size = SizeField.Fill,
                Children =
                {
                    new Button
                    {
                        Content = "close",
                        [nameof(Button.Click)] = new CommandDescribe((s,e) => vm.TestClose())
                    },
                    new Button
                    {
                        Content = "loading",
                        [nameof(Button.AsyncClick)] = new CommandDescribe(async (s,e) => await vm.LoadingTest()),
                    },
                    new Button
                    {
                        Content = "alert",
                        [nameof(Button.Click)] = new CommandDescribe( (s,e) => vm.TestAlert()),
                    },
                },
            };
        }
    }

    internal class MV : ViewModelBase
    {
        public void TestClose()
        {
            this.Close();
        }

        public void TestAlert()
        {
            this.Dialog.Warn("test");
        }

        public async Task LoadingTest()
        {
            var result = await this.ShowLoading(async () =>
            {
                await Task.Delay(3000);
                return "ok";
            });
            Debug.WriteLine(result);
        }

        protected override void OnClose(ClosingEventArgs e)
        {
            e.Cancel = this.Dialog.Ask("确定要关闭吗") != "确定";
            base.OnClose(e);
        }
    }
}
