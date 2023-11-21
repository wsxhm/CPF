using System;
using System.Collections.Generic;
using System.Text;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF.Input;
using CPF;
using CPF.Svg;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace ConsoleApp1
{
    [CPF.Design.DesignerLoadStyle("res://ConsoleApp1/Stylesheet3.css")]
    public class DialogView : Control
    {
        public DialogView(Window2 window)
        {
            this.window = window;
        }
        Window2 window;
        protected override void InitializeComponent()
        {
            Commands.Add("DoubleClick", nameof(doubleClick), this, CommandParameter.EventSender, CommandParameter.EventArgs);
            Commands.Add("MouseDown", nameof(mousedown), this, CommandParameter.EventSender, CommandParameter.EventArgs);
            // this[nameof(Name)]=nameof(Name);
            //模板定义
            IsAntiAlias = true;
            Background = "#fff";
            CornerRadius = "8";
            Width = 746;
            Height = 415;
            ViewFill color = "#888";
            ViewFill hoverColor = "255,255,255,40";
            Children.Add(new TextBlock
            {
                TextDecoration = "Underline 1 Solid #BE0C0C",
                Commands =
                {
                    {
                        nameof(TextBlock.MouseDown),
                        nameof(test123),
                        this,
                        CommandParameter.EventSender,
                        CommandParameter.EventArgs
                    },
                },
                Text = "🇨🇳牛运当头㊗️8881",
                FontSize = 16,
                MarginTop = 58,
                MarginLeft = 402
            });
            Children.Add(new Panel
            {
                //[nameof(Name)]=nameof(Name),
                Name = "close",
                ToolTip = "关闭",
                MarginRight = 5,
                MarginTop = 5,
                Width = 30,
                Height = 30,
                Children =
                {
                    new Line
                    {
                        MarginTop=8,
                        MarginLeft=8,
                        StartPoint = new Point(1, 1),
                        EndPoint = new Point(14, 13),
                        StrokeStyle = "2",
                        IsAntiAlias=true,
                        StrokeFill=color
                    },
                    new Line
                    {
                        MarginTop=8,
                        MarginLeft=8,
                        StartPoint = new Point(14, 1),
                        EndPoint = new Point(1, 13),
                        StrokeStyle = "2",
                        IsAntiAlias=true,
                        StrokeFill=color
                    }
                },
                Commands =
                {
                    {
                        nameof(Button.MouseDown),
                        (s, e) => window.CloseDialogForm(this)
                    }
                },
                Triggers =
                {
                    new Trigger(nameof(Panel.IsMouseOver), Relation.Me)
                    {
                        Setters =
                        {
                            {
                                nameof(Panel.Background),
                                hoverColor
                            }
                        }
                    }
                },
            });
            Children.Add(new Button
            {
                PresenterFor = this,
                Name = nameof(按钮),
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Classes = "imgAndText",
                    IsGroup = true,
                    Children =
                    {
                        new Picture
                        {
                            Stretch = Stretch.Uniform,
                            Source = "res://ConsoleApp1/Resources/icon.png",
                            Classes = "img",
                            Height = 16,
                            Width = 16,
                        },
                        new TextBlock
                        {
                            MarginLeft = 5f,
                            Classes = "text",
                            Text = "文字",
                        },
                    },
                },
                MarginTop = 10,
                MarginLeft = 420,
                IsAntiAlias = true,
                CornerRadius = new CornerRadius(12),
                ToolTip = "32342",
                Width = 87,
                Height = 34,
                Classes = "testButton",
            });
            Children.Add(new SVG
            {
                MarginRight = 7,
                PresenterFor = this,
                Name = nameof(svg11),
                Commands =
                {
                    {
                        "MouseDown",
                        "svgClick",
                        this,
                        CommandParameter.EventSender,
                        CommandParameter.EventArgs
                    },
                },
                Source = "res://ConsoleApp1/test2.svg",
                MarginTop = 48,
                Width = 123,
                Stretch = Stretch.Uniform,
            });
            Children.Add(new Picture
            {
                PresenterFor = this,
                Name = "pic",
                Commands =
                {
                    {
                        "MouseDown",
                        nameof(picClick),
                        this,
                        CommandParameter.EventSender,
                        CommandParameter.EventArgs
                    },
                },
                Stretch = Stretch.Fill,
                Width = 59,
                MarginLeft = 23,
                Height = 57,
                Source = "res://ConsoleApp1/Resources/icon.png"
            });
            Children.Add(new RadioButton
            {
                MarginLeft = 185,
                MarginTop = 10,
                Background = "linear-gradient(0 0,100% 0,#EF1515 0,#15D4EF 0.3630542,#FF0000 0.5029557,#FFFFFF 1)",
                Content = "RadioButton",
            });
            Children.Add(new TextBlock
            {
                MarginLeft = 48,
                MarginTop = 42,
                Text = @"😉 ❁҉҉҉҉҉҉҉҉",
                Name = "TextBlock123",
                PresenterFor = this,
            });
            Children.Add(new Grid
            {
                LineFill = "#E4E4E4",
                LineStroke = "1,Solid",
                ColumnDefinitions =
                {
                    new ColumnDefinition
                    {
                        Width="*"
                    },
                    new ColumnDefinition
                    {
                        Width="*"
                    }
                },
                Children =
                {
                    new Button
                    {
                        Height = 70,
                        Width = 89,
                        Commands =
                        {
                            {
                                "Click",
                                nameof(按钮事件),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        PresenterFor = this,
                        Name = nameof(按钮字段),
                        Content = new CheckBox
                        {
                            Content = "CheckBox",
                        },
                        Attacheds =
                        {
                            {
                                Grid.ColumnIndex,
                                1
                            },
                        }
                    },
                    new ListBox
                    {
                        Height = 68,
                        Width = 73,
                        Items =
                        {
                            "1233",
                            "34242"
                        }
                    },
                },
                MarginLeft = 48,
                MarginTop = 315,
                Height = 81,
                Width = 216,
            });
            Children.Add(new StackPanel
            {
                MarginLeft = 48,
                MarginTop = 58,
                Orientation = Orientation.Horizontal,
                Classes = "imgAndText",
                IsGroup = true,
                Children =
                {
                    new Picture
                    {
                        Classes = "img",
                        Height = 16,
                        Width = 17,
                    },
                    new TextBlock
                    {
                        MarginLeft = 5f,
                        Classes = "text",
                        Text = "文字",
                        Foreground="#f00",
                    },
                },
            });
            Children.Add(new Panel
            {
                MarginLeft = 172,
                MarginTop = 39,
                Classes = "oneLine",
                IsGroup = true,
                Children =
                {
                    new TextBox
                    {
                        MarginLeft = 2,
                        MarginTop = 2,
                        MarginBottom = 2,
                        MarginRight = 2,
                        Classes = "singleLine",
                    },
                    new TextBlock
                    {
                        Classes = "placeholde",
                        MarginLeft = 7,
                        Text = "水印",
                    },
                },
                Height = 31,
                Width = 148,
            });
            Children.Add(new TabControl
            {
                Items =
                {
                    new TabItem
                    {
                        Content = new Panel
                        {
                            Children =
                            {
                                new Panel
                                {
                                    MarginLeft = 128,
                                    MarginTop = 20,
                                    Classes = "textBox,searchBox",
                                    IsGroup = true,
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
                                    Children =
                                    {
                                        new TextBox
                                        {
                                            MarginTop = 3,
                                            MarginBottom = 3,
                                            MarginRight = 33,
                                            MarginLeft = 3,
                                            Classes = "singleLine",
                                        },
                                        new Button
                                        {
                                            Commands =
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(test3424),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            MarginTop = 0,
                                            MarginBottom = 0,
                                            MarginRight = 0,
                                            Width = 30,
                                            Content = new SVG
                                            {
                                                IsAntiAlias = true,
                                                Stretch= Stretch.Uniform,
                                                Width=16,
                                                Source = "<svg><path d=\"M903.744 813.248L760.768 670.272A381.952 381.952 0 0 0 832 448a384 384 0 1 0-384 384 381.952 381.952 0 0 0 222.272-71.232l142.976 142.976a63.936 63.936 0 1 0 90.496-90.496zM192 448a256 256 0 1 1 512 0 256 256 0 0 1-512 0z\"></path></svg>",
                                            },
                                        },
                                        new TextBlock
                                        {
                                            MarginLeft = 8,
                                            Classes = "placeholder",
                                            Text = "placeholder",
                                        }
                                    },
                                    Height = 30,
                                    Width = 200,
                                },
                                new DockPanel
                                {
                                    MarginLeft = 274,
                                    MarginTop = 50,
                                    Children =
                                    {
                                        new Button
                                        {
                                            Commands =
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(addElem),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            Attacheds =
                                            {
                                                {
                                                    DockPanel.Dock,
                                                    Dock.Top
                                                },
                                            },
                                            Content = "Button",
                                        },
                                    },
                                    Height = 88,
                                    Width = 111,
                                },
                                new StackPanel
                                {
                                    Background = "url(res://ConsoleApp1/icon.png) Tile Fill 0,0,0,0",
                                    RenderTransform = new GeneralTransform
                                    {
                                        Angle = 18.1f,
                                    },
                                    Children =
                                    {
                                        
                                    },
                                    BorderType = BorderType.BorderThickness,
                                    MarginLeft = 4,
                                    MarginTop = 45,
                                    Height = 76,
                                    Width = 135,
                                    Bindings =
                                    {
                                        {
                                            nameof(Panel.Children),
                                            nameof(MainModel.UIElements),
                                            null,
                                            BindingMode.OneWayToSource
                                        }
                                    }
                                },
                                new TextBlock
                                {
                                    PresenterFor = this,
                                    Name = nameof(tex1231),
                                    Background = "#84FFA2",
                                    Text = "   Text     Block  ",
                                },
                            },
                            Height = "100%",
                            Width = "100%",
                        },
                        Header = "TabItem",
                    },
                    new TabItem
                    {
                        Content = new Panel
                        {
                            BorderThickness = "50,10,20,30",
                            BorderType = BorderType.BorderThickness,
                            BorderStroke = "8,Solid",
                            BorderFill = "#9D9D9D",
                            Children =
                            {
                                new Label
                                {
                                    MarginLeft = 351,
                                    MarginTop = 9,
                                    Text = "Label",
                                },
                                new Button
                                {
                                    Height = 37,
                                    Width = 91,
                                    MarginTop = 42,
                                    MarginLeft = 35,
                                    Content = "Button",
                                },
                                new CheckBox
                                {
                                    MarginTop = 9,
                                    MarginLeft = 242,
                                    Content = "CheckBox",
                                },
                            },
                            Height = "100%",
                            Width = "100%",
                        },
                        Header = "TabItem",
                    },
                    new TabItem
                    {
                        Content = new Panel
                        {
                            Children =
                            {
                                new TextBlock
                                {
                                    Height = 72,
                                    Width = 105,
                                    MarginLeft = 7,
                                    MarginTop = 6,
                                    Text = "TextBlock裁剪测试啊啊啊啊啊",
                                },
                                new Grid
                                {
                                    MarginLeft = 129,
                                    MarginTop = 22,
                                    IsGroup = true,
                                    ColumnDefinitions =
                                    {
                                        new ColumnDefinition
                                        {
                                            
                                        },
                                        new ColumnDefinition
                                        {
                                            
                                        },
                                    },
                                    Children =
                                    {
                                        new GridSplitter
                                        {
                                            Height = "100%",
                                            MarginLeft = 0f,
                                            Attacheds =
                                            {
                                                {
                                                    Grid.ColumnIndex,
                                                    1
                                                },
                                            },
                                        },
                                        new Button
                                        {
                                            Attacheds =
                                            {
                                                {
                                                    Grid.ColumnIndex,
                                                    1
                                                },
                                            },
                                            Content = "Button",
                                        },
                                        new Button
                                        {
                                            Height = "100%",
                                            Width = "100%",
                                            Content = "Button",
                                        },
                                    },
                                    Height = 117,
                                    Width = 165,
                                }
                            },
                            Height = "100%",
                            Width = "100%",
                        },
                        Header = new Button
                        {
                            Content = "Button",
                        },
                    },
                    new TabItem
                    {
                        Content = new Panel
                        {
                            Children =
                            {
                                new Button
                                {
                                    MarginTop = 78,
                                    MarginLeft = 241,
                                    Height = 39,
                                    Width = 102,
                                    Content = "Button1",
                                },
                                new Button
                                {
                                    ZIndex = -1,
                                    Height = 35,
                                    Width = 79,
                                    MarginRight = 292,
                                    MarginBottom = 20,
                                    Content = "Button2",
                                },
                                new Button
                                {
                                    MarginLeft = 52,
                                    MarginTop = 25,
                                    Content = "Button3",
                                },
                                new Button
                                {
                                    Height = 35,
                                    Width = 99,
                                    ZIndex = 1,
                                    MarginLeft = 266,
                                    MarginTop = 21,
                                    Content = "Button4",
                                },
                                new Border
                                {
                                    Child = new Ellipse
                                    {
                                        Fill = "url(res://ConsoleApp1/Resources/icon.png) Clamp Fill 0,0,0,0",
                                        Height = "100%",
                                        Width = "100%",
                                    },
                                    ShadowBlur = 10,
                                    MarginLeft = 128,
                                    MarginTop = 10,
                                    Height = 78,
                                    Width = 122,
                                }
                            },
                            Height = "100%",
                            Width = "100%",
                        },
                        Header = "TabItem",
                    },
                    new TabItem
                    {
                        Content = new Panel
                        {
                            Children =
                            {
                                new Grid
                                {
                                    Children =
                                    {
                                        new TextBlock
                                        {
                                            Text = "          TextBlock",
                                        },
                                        new TextBox
                                        {
                                            Background = "#C3C3C3",
                                            Text = "1231",
                                            MarginLeft = 18,
                                            MarginTop = 10,
                                            Height = 69,
                                            Width = 152,
                                            Padding="5",
                                        },
                                    },
                                    Height = 117,
                                    Width = 272,
                                },
                            },
                            Height = "100%",
                            Width = "100%",
                        },
                        Header = "TabItem",
                    },
                    new TabItem
                    {
                        Content = new Panel
                        {
                            Children =
                            {
                                new Panel
                                {
                                    MarginLeft = 251,
                                    MarginTop = 10,
                                    Children =
                                    {
                                        new StackPanel
                                        {
                                            Height = 106,
                                            MarginLeft = 12,
                                            MarginTop = 9,
                                            Children =
                                            {
                                                new TextBlock
                                                {
                                                    Text = "Tex3424222tBddk",
                                                    MaxWidth="100%",
                                                    Name="test"
                                                },
                                                new ListBox
                                                {
                                                    MarginTop=5,
                                                    ItemsPanel=new StackPanel
                                                    {
                                                        Orientation= Orientation.Horizontal
                                                    },
                                                    Items =
                                                    {
                                                        new Button
                                                        {
                                                            Content="23"
                                                        },
                                                        new Button
                                                        {
                                                            Content="213"
                                                        },
                                                    }
                                                },
                                            },
                                            Width = 86,
                                        }
                                    },
                                },
                                new Grid
                                {
                                    Children =
                                    {
                                        new Label
                                        {
                                            MarginTop = 10,
                                            MarginLeft = 19,
                                            Text = "Label",
                                        },
                                        new Button
                                        {
                                            Height = 28,
                                            Width = 73,
                                            MarginTop = 4,
                                            MarginLeft = 80,
                                            Content = "Button",
                                        },
                                        new TextBlock
                                        {
                                            MarginTop = 70,
                                            MarginLeft = 65,
                                            Text = "TextBlock",
                                        },
                                    },
                                    Height = 104,
                                    Width = 138,
                                    MarginTop = 16,
                                    MarginLeft = 18,
                                },
                            },
                            Height = "100%",
                            Width = "100%",
                        },
                        Header = "TabItem",
                    },
                },
                Height = 183,
                Width = 402,
            });
            Children.Add(new ComboBox
            {
                PresenterFor = this,
                Name = nameof(comBox),
                MarginLeft = 171,
                MarginTop = 76,
            });
            Children.Add(new ComboBox
            {
                //IsVirtualizing=true,
                //IsEditable=true,
                //SelectionMode= SelectionMode.Multiple,
                SelectedIndex = 2,
                MarginTop = 96,
                MarginLeft = 48,
                Width = 100,
                Height = 25,
                ItemTemplate = new ListBoxItem
                {
                    Width = "100%",
                    FontSize = 14,
                    ContentTemplate = new ContentTemplate
                    {
                        Width = "auto",
                        MarginLeft = 5,
                    }
                },
                Bindings =
                {
                    {
                        nameof(ComboBox.Items),
                        nameof(MainModel.TestItems),
                        null,
                        BindingMode.TwoWay
                    },
                    {
                        nameof(ComboBox.SelectedValue),
                        nameof(MainModel.SelectValue),
                        null,
                        BindingMode.TwoWay
                    },
                },
                DisplayMemberPath = "Item1",
                SelectedValuePath = "Item2",//IsVirtualizing=true
            });
        }
        TextBlock tex1231;
        ComboBox comBox;
        Button 按钮;
        Button 按钮字段;
        SVG svg11;
#if !DesignMode //用户代码写到这里，设计器下不执行，防止设计器出错
        protected override void OnInitialized()
        {
            base.OnInitialized();
            svg11 = FindPresenterByName<SVG>(nameof(svg11));
            按钮字段 = FindPresenterByName<Button>(nameof(按钮字段));
            按钮 = FindPresenterByName<Button>(nameof(按钮));
            comBox = FindPresenterByName<ComboBox>(nameof(comBox));
            tex1231 = FindPresenterByName<TextBlock>(nameof(tex1231));


            for (int i = 0; i < 10; i++)
            {
                comBox.Items.Add(i.ToString());
            }
        }





        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            using (SolidColorBrush sb = new SolidColorBrush(Color.Red))
            {
                using (var b = new SolidColorBrush(Color.Green))
                {
                    using (var path = new PathGeometry())
                    {
                        path.BeginFigure(0, 0);
                        path.LineTo(100, 100);
                        path.LineTo(0, 100);
                        path.EndFigure(false);
                        var m = Matrix.Identity;
                        m.Rotate(20);
                        //m.Translate(120, 120);
                        path.Transform(m);
                        using (var p = path.CreateStrokePath())
                        {
                            var mm = dc.Transform;
                            //var old = mm;
                            //mm.Scale(3, 3);
                            //mm.Translate(-100, -100);
                            //dc.Transform = mm;
                            dc.DrawPath(sb, new Stroke(1, DashStyles.DashDot), p);
                            //dc.DrawPath(b, new Stroke(1), path);
                            //dc.Transform = old;
                        }
                    }
                }
            }
        }

        void Test()
        {
            MessageBox.Show("//if ");
            //if (Root.Styles.Count == 0)
            //{
            //Root.Styles.Add(new Style(new ClassSelector("testButton").Descendant().OfType<TextBlock>())
            //{
            //    Setters =
            //    {
            //        { nameof(Button.Background), "#f00" }
            //    }
            //});
            //Root.Styles.Add(new Style(new ClassSelector("testButton").Descendant().OfType<TextBlock>()));
            //Root.Styles.Add(new Style(new TypeSelector(typeof(Button))));
            //Root.Styles.Add(new Style(new TypeSelector(typeof(CheckBox)).Descendant().Name("indeterminateMark")));
            //Root.Styles.Add(new Style(new TypeSelector(typeof(TabItem)).PropertyEquals("IsSelected", true)));
            //}

            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //foreach (var item in Root.Find<UIElement>())
            //{
            //    foreach (var selector in Root.Styles)
            //    {
            //        if (selector.Selector.Select(item))
            //        {
            //            var prev = selector.Selector.Prev;
            //            if (prev == null)
            //            {
            //                //System.Diagnostics.Debug.WriteLine(item);
            //                continue;
            //            }
            //            var element = item;
            //            while (prev != null)
            //            {
            //                if (prev is Selector selector1)
            //                {
            //                    if (selector1.Select(element))
            //                    {
            //                        if (prev.Prev == null)
            //                        {
            //                            //System.Diagnostics.Debug.WriteLine(element);
            //                        }
            //                    }
            //                    else
            //                    {
            //                        break;
            //                    }
            //                }
            //                else if (prev is ChildSelector)
            //                {
            //                    element = element.Parent;
            //                }
            //                else
            //                {
            //                    var p = element.Parent;
            //                    var select = prev.Prev as Selector;
            //                    while (p != null)
            //                    {
            //                        if (select.Select(p))
            //                        {
            //                            if (select.Prev == null)
            //                            {
            //                                //System.Diagnostics.Debug.WriteLine(element);
            //                            }
            //                            break;
            //                        }
            //                        p = p.Parent;
            //                    }
            //                    if (p == null)
            //                    {
            //                        break;
            //                    }
            //                    prev = prev.Prev;
            //                }
            //                prev = prev.Prev;
            //            }

            //        }
            //    }
            //}

            //stopwatch.Stop();
            //Debug.WriteLine(stopwatch.ElapsedMilliseconds);
        }
        void picClick(CpfObject obj, MouseButtonEventArgs eventArgs)
        {

        }
        void mousedown(CpfObject obj, MouseButtonEventArgs eventArgs)
        {

        }
        void doubleClick(CpfObject obj, RoutedEventArgs eventArgs)
        {

        }
        void 按钮事件(CpfObject obj, RoutedEventArgs eventArgs)
        {

        }
        void test3424(CpfObject obj, RoutedEventArgs eventArgs)
        {

        }
        void addElem(CpfObject obj, RoutedEventArgs eventArgs)
        {
            (DataContext as MainModel).UIElements.Add(new Button { Content = "刀斧手" });
        }
#endif
        void test123(CpfObject obj, MouseButtonEventArgs eventArgs)
        {

        }
    }
}
