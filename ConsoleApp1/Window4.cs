using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF;
using CPF.Drawing;
using CPF.Controls;
using CPF.Shapes;
using CPF.Styling;
using CPF.Animation;

namespace ConsoleApp1
{
    public class Window4 : Window
    {
        protected override void InitializeComponent()
        {
            Title = "标题";
            Width = 338.4f;
            Height = 205.6f;
            Background = null;
            Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Children =
                {
                    //内容元素放这里
                    new Button
                    {
                        MarginLeft = 223.8f,
                        MarginTop = 25.7f,
                        Height = 28f,
                        Width = 67.4f,
                        Content = "Button",
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                nameof(MainModel.Click)
                            }
                        },
                        Bindings =
                        {
                            {
                                nameof(Button.Content),
                                nameof(TextBox.Text),
                                FindPresenterByName("textBox")
                            }
                        }
                    }
                    .SetTemplate((s,c)=>{ 
                    
                    }),
                    new TextBlock
                    {
                        MarginLeft = 36.7f,
                        MarginTop = 31.6f,
                        Text = "TextBlock",
                        Bindings =
                        {
                            {
                                nameof(TextBlock.Text),
                                nameof(MainModel.Test),
                                null,
                                BindingMode.OneWay,
                                (string a)=>a+"1"
                            }
                        }
                    },
                    new Button
                    {
                        MarginLeft = 223.8f,
                        MarginTop = 90.6f,
                        Height = 28f,
                        Width = 67.4f,
                        Content = "添加Item",
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                nameof(MainModel.AddItem)
                            }
                        }
                    },
                    new ListBox
                    {
                        SelectedValuePath = "Item2",
                        //绑定Item里的Item1属性
                        DisplayMemberPath = "Item1",
                        //绑定Item里的Item2属性
                        BorderStroke = "1,Solid",
                        BorderFill = "#DEDEDE",
                        MarginLeft = 36.7f,
                        MarginTop = 60.8f,
                        Height = 76.5f,
                        Width = 123.2f,
                        Bindings =
                        {
                            {
                                nameof(ListBox.Items),
                                nameof(MainModel.Items)
                            }
                        }
                    },
                    new TextBox
                    {
                        Name="textBox",
                        PresenterFor=this,
                        AcceptsReturn= false,
                        HScrollBarVisibility= ScrollBarVisibility.Hidden,
                        VScrollBarVisibility= ScrollBarVisibility.Hidden,
                        MarginLeft = 144.8f,
                        MarginTop = 28.1f,
                        Width = 74.5f
                    },
                }
            }));
            LoadStyleFile("res://ConsoleApp1/Stylesheet1.css");
            //加载样式文件，文件需要设置为内嵌资源
        }
    }
}
