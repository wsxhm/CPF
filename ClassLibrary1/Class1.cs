using System;
using CPF;
using CPF.Controls;

namespace ClassLibrary1
{
    public class Class1 : Control
    {
        protected override void InitializeComponent()
        {
            Bindings.Add("Padding", "333");
            Background = "#FFffff";
            Height = 519;
            Width = 420;
            if (!DesignMode)
            {
                Size = SizeField.Fill;
            }
            Children.Add(new TextBox
            {
                BorderStroke = "1,Solid"  //测试
                ,
                BorderFill = "#3C3C3C",
                MarginTop = 25.9f,
                Height = 20.5f,
                Width = 123.5f,
                HScrollBarVisibility = ScrollBarVisibility.Hidden,
                Text = "231"
            });
            Children.Add(new Button
            {
                Width = 215,
                MarginLeft = -0,
                ToolTip = "test",
                Height = 31,
                MarginTop = 62,
                Content = "Button测试🌐 🍪 🍕🚀123",
                FontFamily = "黑体",
                Commands =
                {
                    {
                        nameof(Button.Click),
                        (s,e)=>
                        {
                            new Window1
                            {

                            }
                            .Show();
                        }
                    }
                }
            });
            Children.Add(new CheckBox
            {
                MarginLeft = 150,
                MarginTop = 122,
                Content = "CheckBox",
            });
            Children.Add(new TextBlock
            {
                FontFamily = "Courier New",
                MarginTop = 143.8f,
                TextStrokeFill = "#F93F00",
                TextStroke = "1,Solid",
                Background = "#0ff",
                Text = "🇨🇳牛运当头㊗️8881",
            });
            Children.Add(new ListBox
            {
                MarginLeft = 28,
                MarginTop = 269,
                Width = 125,
                Height = 168,
                Items =
                {
                    "test1",
                    "test2",
                    "asdad",
                    "on测试🌐 🍪 🍕🚀1",
                    "🍕🚀1",
                    "🚀",
                    "🌐",
                    "🍕",
                    "aa改改",
                    "🚀",
                    "🌐",
                    "🍕",
                    "🍪",
                    "🚀",
                    "🌐",
                    "🍕",
                    "🍪",
                    "🚀",
                    "🌐",
                    "🍕",
                    "🍪",
                }
            });
            Children.Add(new ComboBox
            {
                MarginTop = 441.4f,
                Items =
                {
                    "🚀",
                    "🌐",
                    "🍕",
                    "🍪",
                    "🚀",
                    "🌐",
                    "🍕",
                    "🍪",
                    "🚀",
                    "🌐",
                    "🍕",
                    "🍪",
                    "🚀",
                    "🌐",
                    "🍕",
                    "🍪",
                    "🚀",
                    "🌐",
                    "🍕",
                    "🍪",
                    "🚀",
                    "🌐",
                    "🍕",
                    "🍪",
                }
            });
            Children.Add(new ComboBox
            {
                MarginTop = 469.9f,
                Items =
                {
                    "🚀",
                    "🌐",
                    "🍕",
                    "🍪",
                    "🚀",
                    "🌐",
                    "🍕",
                    "🍪",
                    "🚀",
                    "🌐",
                    "🍕",
                    "🍪",
                }
            });
            Children.Add(new ScrollViewer
            {
                Content = new TextBlock
                {
                    Height = 246.4f,
                    Width = 208.4f,
                    Text = "Button",
                    TextAlignment = CPF.Drawing.TextAlignment.Center,
                    Background = "100,200,100,200"
                },
                Width = 146.5f,
                Height = 169.4f,
                MarginLeft = -0.2f,
                MarginTop = 93f,
            });
            Children.Add(new Panel
            {
                Background= "url(res://ClassLibrary1/icon.png)",
                Children =
                {
                    new Button
                    {
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                nameof(ShowLayered),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        Height = 26,
                        Width = 76,
                        MarginLeft = 4,
                        MarginTop = 7,
                        Content = "显示加载",
                    },
                    new Button
                    {
                        MarginLeft = 105,
                        MarginTop = 13,
                        Content = "Button",
                    },
                    new Label
                    {
                        MarginTop = 40,
                        MarginLeft = 9,
                        Text = "123132",
                    },
                    new CheckBox
                    {
                        MarginLeft = 6,
                        MarginTop = 68,
                        Content = "CheckBox",
                    },
                },
                Bindings =
                {
                    {
                        "Foreground",
                        "dfsfsfe",
                        null,
                        BindingMode.OneTime
                    },
                },
                MarginLeft = 224,
                MarginTop = 52,
                Height = 124,
                Width = 179,
            });
            Children.Add(new DatePicker
            {
                MarginTop = 188,
                MarginRight = 15,
                Height = 26,
                Width = 108,
            });
            Children.Add(new Switch
            {
                Width = 54,
                MarginRight = 45,
                MarginTop = 227,
                Height = 26,
            });
            Children.Add(new TextBox
            {
                Background = "#FFFFFF",
                MarginLeft = 15,
                MarginTop = 461,
                Height = 26,
                Width = 132,
            });
            Children.Add(new TextBox
            {
                Width = 125,
                MarginRight = 6,
                Height = 29,
                MarginBottom = 26,
                Background = "#FFFFFF",
            });
            Children.Add(new TabControl
            {
                MarginLeft = 188,
                MarginTop = 273,
                Items =
                {
                    new TabItem
                    {
                        Content = new Panel
                        {
                            Height = "100%",
                            Width = "100%",
                        },
                        Header = "TabItem",
                    },
                    new TabItem
                    {
                        Content = new Panel
                        {
                            Background = "#FFFFB2",
                            Children =
                            {

                            },
                            Height = "100%",
                            Width = "100%",
                        },
                        Header = "TabItem",
                    },
                },
                Height = 142,
                Width = 156,
            });
            Children.Add(new NativeElement
            {
                MarginLeft = 180,
                MarginTop = 194,
                Height = 68,
                Width = 100,
                Content= CreateNativeControl?.Invoke()
            });
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();

            Root.LoadStyleFile("res://ClassLibrary1/Stylesheet1.css");
        }
        void ShowLayered(CpfObject obj, RoutedEventArgs eventArgs)
        {
            this.ShowLoading("dfsds测试", a =>
            {
                System.Threading.Thread.Sleep(2000);
            });
        }

        public static Func<object> CreateNativeControl;
    }
}
