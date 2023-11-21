using CPF;
using CPF.Animation;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace ClassLibrary1
{
    public class Window1 : Window
    {
        protected override void InitializeComponent()
        {
            Title = "标题";
            Width = 300;
            Height = 400;
            Background = null;
            CanResize = true;
            Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Children = //内容元素放这里
                {
                    new Button
                    {
                        Content = new Button
                        {
                            Commands =
                            {
                                {
                                    nameof(Button.Click),
                                    nameof(MovePosition),
                                    this,
                                    CommandParameter.EventSender,
                                    CommandParameter.EventArgs
                                },
                            },
                            Content = "Button",
                        },
                        MarginTop = 82,
                        Height = 30,
                        Width = 88,
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                (s,e)=>
                                {
                                    this.Position=new PixelPoint(100,100);
                                }
                            }
                        }
                    },
                    new TextBox
                    {
                        Text = "123",
                        MarginLeft = 102,
                        BorderFill = "#9A9A9A",
                        BorderStroke = "1,Solid",
                        MarginTop = 14,
                        Height = 24,
                        Width = 95,
                    },
                    new Button
                    {
                        MarginTop = 148.6f,
                        Height = 30.2f,
                        Width = 87.5f,
                        Content = "打开文件浏览",
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                async (s,e)=>
                                {
                                    OpenFileDialog dialog=new OpenFileDialog
                                    {
                                        Title="cpf文件选择",
                                        Filters=
                                        {
                                            new FileDialogFilter
                                            {
                                                Extensions="jpg,png"
                                            }
                                        }
                                    };
                                    var f= await dialog.ShowAsync(this);
                                    System.Diagnostics.Debug.WriteLine(f);
                                    //var photo = await FilePicker.PickAsync();
                                      //System.Diagnostics.Debug.WriteLine(photo.FileName);
                                    //  if (WindowState== WindowState.FullScreen)
                                    //{
                                    //    WindowState= WindowState.Normal;
                                    //}
                                    //else
                                    //{
                                    //    WindowState= WindowState.FullScreen;
                                    //}
                                }
                            }
                        }
                    },
                    new Button
                    {
                        MarginLeft = 102,
                        MarginTop = 44,
                        Height = 30.2f,
                        Width = 87.5f,
                        Content = "退出程序",
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                (s,e)=>
                                {
                                    CPF.Platform.Application.Exit();
                                }
                            }
                        }
                    },
                    new Panel
                    {
                        Name = "ColorPicker",
                        MarginTop = 178.8f,
                        MarginLeft = 47.6f,
                        Children =
                        {
                            new Slider
                            {
                                MarginBottom = 40f,
                                MarginTop = 5f,
                                MarginRight = 25f,
                                Background = "linear-gradient(0 0,0 100%,#000000 0,#FFffFF 1)",
                                Width = 10f,
                                ZIndex=1,
                                Orientation= Orientation.Vertical
                            },
                            new Control
                            {
                                Background = "linear-gradient(0 0,100% 0,#FF0000 0,#FFFFFF 1)",
                                MarginLeft = 5f,
                                MarginRight = 40f,
                                MarginTop = 5f,
                                MarginBottom = 40f,
                            },
                            new Control
                            {
                                Background = "linear-gradient(0 0,0 100%,#00000000 0,#000000 1)",
                                MarginLeft = 5f,
                                MarginRight = 40f,
                                MarginTop = 5f,
                                MarginBottom = 40f,
                            },
                            new Slider
                            {
                                MarginBottom = 40f,
                                MarginRight = 5f,
                                MarginTop = 5f,
                                Background = "linear-gradient(0 0,0 100%,#FA0000 0,#FAFF00 0.2,#00FF00 0.4,#00FFFF 0.6,#0000FF 0.8,#FF00FF 1)",
                                Width = 10f,
                                ZIndex=1,
                                Orientation= Orientation.Vertical
                            },
                            new Button
                            {
                                Width = 38.1f,
                                MarginBottom = 11.1f,
                                MarginRight = 5f,
                                Content = "OK",
                            },
                            new Button
                            {
                                MarginBottom = 10.9f,
                                MarginRight = 46.5f,
                                Width = 50.5f,
                                Content = "Cancel",
                            },
                            new TextBox
                            {
                                Text = "#FFFFFFFF",
                                MarginBottom = 11.1f,
                                Height = 20.9f,
                                MarginLeft = 5f,
                                Width = 73.5f,
                                HScrollBarVisibility= ScrollBarVisibility.Hidden,
                                VScrollBarVisibility= ScrollBarVisibility.Hidden,
                                AcceptsReturn=false,
                                BorderFill="#B1B1B1",
                                BorderStroke="1",
                            },
                        },
                        Width = 194.9f,
                        Height = 145.4f,
                    },
                    new TextBox
                    {
                        MarginRight = 12,
                        MarginBottom = 14,
                        Text = "#FFFFFFFF",
                        Height = 22,
                        Width = 81,
                        HScrollBarVisibility= ScrollBarVisibility.Hidden,
                        VScrollBarVisibility= ScrollBarVisibility.Hidden,
                        AcceptsReturn=false,
                        BorderFill="#B1B1B1",
                        BorderStroke="1",
                    },
                    new Button
                    {
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                nameof(msg),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        Height = 27,
                        Width = 87,
                        MarginLeft = 102,
                        MarginTop = 115,
                        Content = "弹窗",
                    },
                }
            })
            {
                MaximizeBox = true
            });
            LoadStyleFile("res://ClassLibrary1/Stylesheet1.css");
            //加载样式文件，文件需要设置为内嵌资源

            if (!DesignMode)//设计模式下不执行
            {
                
            }
        }

        void MovePosition(CpfObject obj, RoutedEventArgs eventArgs)
        {
            this.ViewImpl.Position = new PixelPoint();
        }
        void msg(CpfObject obj, RoutedEventArgs eventArgs)
        {
            MessageBox.Show("test");
        }
    }
}
