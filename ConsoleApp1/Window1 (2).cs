using CPF;
using CPF.Animation;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CPFApp
{
    public class Window1 : Window
    {
        protected override void InitializeComponent()
        {
            Icon = "res://ConsoleApp1/icon.png";
            //需要根据项目修改
            LoadStyleFile("res://ConsoleApp1/Stylesheet.css");
            //需要根据项目修改
            Title = "标题";
            Width = 548;
            Height = 356;
            Background = null;
            Children.Add(new Border
            {
                Child = new Panel
                {
                    Children =
                    {
                        new Panel
                        {
                            MarginRight = 19,
                            MarginTop = 40,
                            IsGroup = false,
                            Classes = "loginBox",
                            Commands =
                            {
                                {
                                    nameof(MouseDown),
                                    (s,e)=>(e as RoutedEventArgs).Handled=true
                                }
                            },
                            Children =
                            {
                                new Panel
                                {
                                    MarginTop = 64,
                                    Classes = "textBox",
                                    Attacheds =
                                    {
                                        {
                                            AttachedExtenstions.IsEmpty,
                                            true,
                                            nameof(TextBox.Text),
                                            a=>a.GetChildren().First(b=>b is TextBox),
                                            BindingMode.OneWay,
                                            (string text)=>string.IsNullOrWhiteSpace(text)
                                        }
                                    },
                                    IsGroup = true,
                                    Children =
                                    {
                                        new TextBox
                                        {
                                            MarginBottom = 3,
                                            MarginRight = 3,
                                            MarginLeft = 31,
                                            MarginTop = 3,
                                            Classes = "singleLine",
                                        },
                                        new TextBlock
                                        {
                                            MarginLeft = 38,
                                            Classes = "placeholder",
                                            Text = "用户名",
                                        },
                                        new SVG
                                        {
                                            MarginLeft = 5,
                                            Height = 20,
                                            IsAntiAlias = true,
                                            Width = 20,
                                            Stretch = Stretch.Uniform,
                                            Source="<svg t=\"1615898331928\" class=\"icon\" viewBox=\"0 0 1059 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"5401\" width=\"128\" height=\"128\"><path d=\"M817.540414 298.619586c0-164.89931-135.768276-298.619586-303.280552-298.619586S210.97931 133.684966 210.97931 298.619586c0 164.89931 135.768276 298.619586 303.315862 298.619586 167.476966 0 303.245241-133.684966 303.245242-298.619586z m-553.807448 0c0-136.22731 112.145655-246.678069 250.526896-246.678069 138.381241 0 250.526897 110.450759 250.526897 246.678069 0 136.22731-112.145655 246.678069-250.526897 246.678069-138.381241 0-250.526897-110.450759-250.526896-246.678069zM0 986.747586v25.953104h1054.896552v-25.953104c0-233.154207-244.065103-441.449931-527.465931-441.449931-17.019586 0-34.003862 0.706207-50.811587 2.01269-14.512552 1.165241 23.304828 13.700414 24.470069 28.001103 15.39531-1.235862 10.734345 21.892414 26.341518 21.892414 255.823448 0 474.712276 186.862345 474.712276 389.543724l26.376827-25.988414H26.376828l26.376827 25.988414c0-137.922207 88.381793-263.980138 230.823724-334.177103a25.776552 25.776552 0 0 0 11.758345-34.851311 26.553379 26.553379 0 0 0-35.380965-11.581793C100.528552 684.667586 0 828.027586 0 986.747586z\" p-id=\"5402\"></path></svg>"
                                        }
                                    },
                                    Height = 36f,
                                    Width = 220f,
                                },
                                new Panel
                                {
                                    MarginTop = 118,
                                    Classes = "textBox",
                                    Attacheds =
                                    {
                                        {
                                            AttachedExtenstions.IsEmpty,
                                            true,
                                            nameof(TextBox.Text),
                                            a=>a.GetChildren().First(b=>b is TextBox),
                                            BindingMode.OneWay,
                                            (string text)=>string.IsNullOrWhiteSpace(text)
                                        }
                                    },
                                    IsGroup = true,
                                    Children =
                                    {
                                        new TextBox
                                        {
                                            PasswordChar = '#',
                                            MarginBottom = 2,
                                            MarginRight = 2,
                                            MarginLeft = 31,
                                            MarginTop = 2,
                                            Classes = "singleLine",
                                        },
                                        new TextBlock
                                        {
                                            MarginLeft = 38,
                                            Classes = "placeholder",
                                            Text = "密码",
                                        },
                                        new SVG
                                        {
                                            MarginLeft = 5,
                                            Height = 20,
                                            IsAntiAlias = true,
                                            Width = 22,
                                            Stretch = Stretch.Uniform,
                                            Source="<svg t=\"1615898493386\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"6181\" width=\"128\" height=\"128\"><path d=\"M819.221333 417.856l-519.253333 0-0.021333-150.336-0.277333-4.309333c-0.170667-2.453333-0.341333-4.906667-0.341333-7.402667s0.170667-4.970667 0.341333-7.445333l0.298667-13.397333c0.490667-1.408 0.832-2.901333 1.024-4.437333 12.650667-105.962667 102.72-185.877333 209.472-185.877333 106.773333 0 196.842667 79.914667 209.365333 184.085333 0 0 1.770667 50.794667 1.792 55.509333 0.042667 11.754667 9.578667 21.290667 21.333333 21.290667 11.477333 0.832 21.333333-9.493333 21.333333-21.312 0-4.416-1.834667-56.981333-1.962667-58.773333C747.114667 98.069333 638.848 1.984 510.485333 1.984c-125.973333 0-232.618667 92.565333-250.901333 216.448-1.450667 2.88-2.261333 6.122667-2.261333 9.557333l0.021333 13.802667-0.234667 3.690667c-0.234667 3.413333-0.448 6.826667-0.448 10.304s0.213333 6.869333 0.448 10.261333l0.192 151.786667L204.757333 417.834667c-53.44 0-96.938667 44.16-96.938667 98.432l0 412.693333c0 53.013333 42.090667 93.013333 97.898667 93.013333l612.565333 0c55.808 0 97.92-40 97.92-93.013333L916.202667 516.309333C916.181333 462.016 872.682667 417.856 819.221333 417.856zM873.514667 929.002667c0 33.066667-27.797333 50.346667-55.253333 50.346667L205.717333 979.349333c-27.456 0-55.232-17.301333-55.232-50.346667L150.485333 516.309333c0-30.741333 24.341333-55.765333 54.272-55.765333l614.485333 0c29.930667 0 54.293333 25.024 54.293333 55.765333L873.536 929.002667z\" p-id=\"6182\"></path></svg>"
                                        }
                                    },
                                    Height = 36f,
                                    Width = 220f,
                                },
                                new CheckBox
                                {
                                    MarginLeft = 26,
                                    MarginTop = 175,
                                    Content = "记住密码",
                                },
                                new CheckBox
                                {
                                    MarginLeft = 181,
                                    MarginTop = 175,
                                    Content = "自动登录",
                                },
                                new Button
                                {
                                    MarginLeft = 27,
                                    MarginTop = 213,
                                    Classes = "primary",
                                    Height = 33,
                                    Width = 226,
                                    Content = "登录",
                                },
                                new TextBlock
                                {
                                    FontSize = 20f,
                                    MarginTop = 13,
                                    Text = "XX管理系统",
                                }
                            },
                            Height = 264,
                            Width = 274,
                        },
                        new Panel
                        {
                            MarginBottom = 0,
                            Background = "#5862E5",
                            Children =
                            {
                                new SVG
                                {
                                    IsAntiAlias = true,
                                    Source = "res://ConsoleApp1/svg1.svg",
                                    Stretch = Stretch.Uniform,
                                    Height = 196,
                                    Width = 167,
                                },
                            },
                            MarginLeft = 0,
                            MarginTop = 0,
                            Width = 222,
                            Commands =
                            {
                                {
                                    nameof(MouseDown),
                                    (s,e)=>DragMove()
                                }
                            }
                        },
                        new SVG
                        {
                            Fill = "#6C6C6C",
                            MarginLeft = 501,
                            MarginTop = 9,
                            Height = 18,
                            IsAntiAlias = true,
                            Width = 18,
                            Stretch = Stretch.Uniform,
                            Source="<svg t=\"1615894804118\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"4632\" width=\"128\" height=\"128\"><path d=\"M523.776 440.832L148.47999998 65.53599997c-18.944-18.944-50.17600001-18.944-69.11999998 4e-8l-2.04799999 2.04800001c-18.944 18.944-18.944 50.17600001 0 69.11999998L453.11999999 512 77.31199998 887.296c-18.944 18.944-18.944 50.17600001 3e-8 69.12000001l2.04799999 2.04799998c18.944 19.45600002 50.17600001 19.45600002 69.12000001 0l375.29599999-375.29599999L899.072 958.46400003c18.944 18.944 50.17600001 18.944 69.12-4e-8l2.048-2.04800001c18.944-18.944 18.944-50.17600001 0-69.11999998L594.94400001 512 970.24 136.704c18.944-18.944 18.944-50.17600001 0-69.12000001l-2.048-2.04799998c-18.944-18.944-50.17600001-18.944-69.12 0L523.776 440.832z\"></path></svg>",
                            Triggers =
                            {
                                {
                                    nameof(IsMouseOver),
                                    Relation.Me,
                                    null,
                                    (nameof(SVG.Fill),"#111")
                                }
                            },
                            Commands =
                            {
                                {
                                    nameof(MouseDown),
                                    (s,e)=>Close()
                                }
                            }
                        },
                    },
                    Height = "100%",
                    Width = "100%",
                },
                BorderStroke = "0,Solid",
                ShadowBlur = 10,
                Background = "#FFFFFF",
                Height = "100%",
                Width = "100%",
                Commands =
                {
                    {
                        nameof(MouseDown),
                        (s,e)=>DragMove()
                    }
                }
            });
        }

    }
}
