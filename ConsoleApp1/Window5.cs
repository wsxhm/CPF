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

namespace ConsoleApp1
{
    public class Window5 : Window
    {
        NotifyIcon notifyIcon = new NotifyIcon();
        protected override void InitializeComponent()
        {
            notifyIcon.Icon = "res://ConsoleApp1/icon.png";
            notifyIcon.Visible = true;
            notifyIcon.Text = "文字提示";
            notifyIcon.MouseDown += NotifyIcon_MouseDown;
            notifyIcon.MouseUp += NotifyIcon_MouseUp;
            notifyIcon.Click += NotifyIcon_Click;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            Icon = "res://ConsoleApp1/icon.png";
            LoadStyleFile("res://ConsoleApp1/Stylesheet.css");
            this.Styles.Add(new Style(new NameSelector("testBtn"))
            {
                //#testBtn .TextBlock
                Setters =
                {
                    {
                        nameof(Button.Width),
                        30
                    }
                }
            });
            Title = "标题";
            Width = 992;
            Height = 543;
            Background = null;
            CanResize = true;
            ContextMenu = new ContextMenu
            {
                Items =
                {
                    new MenuItem
                    {
                        Header="增加一个选项",
                        Commands =
                        {
                            {nameof(MenuItem.MouseDown),(s,e)=>{
                                ((s as MenuItem).OwnerContextMenu.Items[2] as MenuItem).Items.Add(new MenuItem{ Header="测试"});

                            } }
                        }
                    },
                    new MenuItem
                    {
                        Header="清除选项",
                        Commands =
                        {
                            {nameof(MenuItem.MouseDown),(s,e)=>{
                                ((s as MenuItem).OwnerContextMenu.Items[2] as MenuItem).Items.Clear();

                            } }
                        }
                    },
                    new MenuItem
                    {
                        Header="选项2",
                        Items=
                        {
                            new MenuItem
                            {
                                Header="选项1"
                            },
                        }
                    },
                    new Separator
                    {

                    },
                    new MenuItem
                    {
                        Header="选项3"
                    }
                }
            };
            notifyIcon.ContextMenu = ContextMenu;
            Children.Add(new WindowFrame(this, new Panel
            {
                UseLayoutRounding = true,
                Width = "100%",
                Height = "100%",
                Children =
                {
                    new Panel
                    {
                        MarginLeft = 14,
                        MarginTop = 4,
                        Classes = "oneLine",
                        IsGroup = true,
                        Children =
                        {
                            new TextBlock
                            {
                                Classes = "label",
                                MarginLeft = 7,
                                Text = "标签",
                            },
                            new ComboBox
                            {
                                Width = 158,
                                MarginRight = 8,
                                Items =
                                {
                                    "231",
                                    "sdaf",
                                    "dfs",
                                    "fgsfs",
                                    "dfsgd",
                                    "dgds",
                                    "hdfs",
                                    "fgdgssa",
                                    "dgds",
                                    "ghdfs",
                                    "fgd",
                                    "ghdfs",
                                    "fggd",
                                    "sdfs",
                                    "fghdsa"
                                }
                            },
                        },
                        Height = 38,
                        Width = 245,
                    },
                    new Button
                    {
                        Commands =
                        {
                            {
                                "Click",
                                nameof(SetNotify),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        Classes = "primary",
                        MarginLeft = 266,
                        MarginTop = 12,
                        Height = 30,
                        Width = 90,
                        Content = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Classes = "imgAndText",
                            IsGroup = true,
                            Children =
                            {
                                new Picture
                                {
                                    Source = "res://ConsoleApp1/Resources/icon.png",
                                    Classes = "img",
                                    Height = 16,
                                    Width = 16,
                                    Stretch = Stretch.Uniform,
                                },
                                new TextBlock
                                {
                                    Foreground = "linear-gradient(0 0,100% 0,#000000 0,#FF0505 1)",
                                    MarginLeft = 5f,
                                    Classes = "text",
                                    Text = "notifyIcon",
                                },
                            },
                        },
                    },
                    new Button
                    {
                        Commands =
                        {
                            {
                                "Click",
                                nameof(SwitchImage),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        Classes = "success",
                        Height = 53,
                        Width = 64,
                        MarginLeft = 266,
                        MarginTop = 57,
                        Content = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            Classes = "imgAndText",
                            IsGroup = true,
                            Children =
                            {
                                new Picture
                                {
                                    Source = "res://ConsoleApp1/Resources/主页.png",
                                    Classes = "img",
                                    Height = 16,
                                    Width = 16,
                                    Stretch = Stretch.Uniform,
                                },
                                new TextBlock
                                {
                                    MarginTop = 5f,
                                    Classes = "text",
                                    Text = "文字",
                                },
                            },
                        },
                    },
                    new RadioButton
                    {
                        MarginLeft = 14,
                        MarginTop = 83,
                        Content = "RadioButton1",
                    },
                    new RadioButton
                    {
                        MarginTop = 105,
                        MarginLeft = 14,
                        Content = "RadioButton2",
                    },
                    new CheckBox
                    {
                        MarginLeft = 14,
                        MarginTop = 128,
                        Content = "CheckBox",
                    },
                    new Panel
                    {
                        MarginLeft = 365,
                        MarginTop = 14,
                        IsGroup = true,
                        Children =
                        {
                            new TextBox
                            {
                                MarginRight = 42.3f,
                                MarginLeft = 2f,
                                MarginTop = 0f,
                                MarginBottom = 0f,
                                Classes = "singleLine",
                            },
                            new Border
                            {
                                Child = new TextBlock
                                {
                                    Foreground = "#9093A2",
                                    MarginRight = 5f,
                                    MarginLeft = 5f,
                                    Text = ".com",
                                },
                                Classes = "slotLeft",
                            },
                        },
                        Classes = "textBox,groupPanel",
                        Width = 134,
                        Height = 27,
                    },
                    new StackPanel
                    {
                        MarginLeft = 14,
                        MarginTop = 51,
                        IsGroup = true,
                        Orientation= Orientation.Horizontal,
                        Children =
                        {
                            new RadioButton
                            {
                                Content = "上海",
                                GroupName="分组1",
                            },
                            new RadioButton
                            {
                                Content = "北京",
                                GroupName="分组1",
                            },
                            new RadioButton
                            {
                                Content = "广州",
                                GroupName="分组1",
                            },
                            new RadioButton
                            {
                                Content = "RadioButton",
                            },
                        },
                        Classes = "radioGroup",
                    },
                    new Slider
                    {
                        MarginLeft = 14,
                        MarginTop = 156,
                        Height = 118,
                        Width = 22,
                        Orientation= Orientation.Vertical
                    },
                    new ProgressBar
                    {
                        //IsIndeterminate = true,
                        MarginLeft = 528,
                        MarginTop = 17,
                        Value = 10f,
                        Height = 20,
                        Width = 159,
                    },
                    new NumericUpDown
                    {
                        Height = 28,
                        Width = 63,
                        MarginLeft = 379,
                        MarginTop = 61,
                    },
                    new DatePicker
                    {
                        MarginLeft = 499,
                        MarginTop = 61,
                        Height = 23f,
                        Classes = "el-textbox",
                        Width = 110f,
                    },
                    new Panel
                    {
                        MarginLeft = 697,
                        MarginTop = 17,
                        Classes = "widget",
                        IsGroup = true,
                        Children =
                        {
                            new Panel
                            {
                                Classes = "widgetHead",
                                Children =
                                {
                                    new TextBlock
                                    {
                                        Classes = "label",
                                        MarginLeft = 7,
                                        Text = "标题",
                                    }
                                },
                                MarginTop = 0,
                                Height = 30,
                                Width = "100%",
                            },
                            new CheckBox
                            {
                                MarginLeft = 21,
                                MarginTop = 47,
                                Content = "CheckBox",
                            },
                            new Button
                            {
                                Commands =
                                {
                                    {
                                        "Click",
                                        nameof(CancelClosing),
                                        this,
                                        CommandParameter.EventSender,
                                        CommandParameter.EventArgs
                                    },
                                },
                                Height = 26,
                                Width = 74,
                                Content = "Cancel",
                            },
                        },
                        Height = 186,
                        Width = 276,
                    },
                    new TabControl
                    {
                        Bindings =
                        {
                            {
                                "SelectedIndex",
                                "SelectedItem",
                                FindPresenterByName("treeview"),
                                BindingMode.OneWay,
                                (TreeViewItem item)=>(item==null|| item.Tag==null)?0:int.Parse(item.Tag.ToString())
                            },
                        },
                        MarginLeft = 43,
                        MarginTop = 156,
                        TabStripPlacement = Dock.Left,
                        Height = 271,
                        Width = 453,
                        Items =
                        {
                            new TabItem
                            {
                                Header = "选项1",
                                FontSize = 15f,
                                Content=new Panel
                                {
                                    Children =
                                    {
                                        new TabControl
                                        {
                                            Items =
                                            {
                                                new TabItem
                                                {
                                                    Content = new Panel
                                                    {
                                                        Children =
                                                        {
                                                            new Button
                                                            {
                                                                Commands =
                                                                {
                                                                    {
                                                                        nameof(Button.Click),
                                                                        nameof(Click123),
                                                                        this,
                                                                        CommandParameter.EventSender,
                                                                        CommandParameter.EventArgs
                                                                    },
                                                                },
                                                                PresenterFor = this,
                                                                Name = "testBtn",
                                                                MarginLeft = 127,
                                                                MarginTop = 56,
                                                                Height = 41,
                                                                Content = "Button12",
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
                                                            new SVG
                                                            {
                                                                Height = 100,
                                                                IsAntiAlias = true,
                                                                Width = 100,
                                                                Stretch = Stretch.Uniform,
                                                                Source="<svg t=\"1615967524728\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"3197\" width=\"128\" height=\"128\"><path d=\"M784 700h-48V464c0-121.28-100.48-220-224-220S288 342.72 288 464v236h-48V464c0-147.84 122.08-268 272-268S784 316.32 784 464v236z\" fill=\"#323232\" p-id=\"3198\"></path><path d=\"M792 840h-560c-44.16 0-80-35.84-80-80s35.84-80 80-80h97.28c13.28 0 24 10.72 24 24s-10.72 24-24 24H232c-17.6 0-32 14.4-32 32s14.4 32 32 32h560c17.6 0 32-14.4 32-32s-14.4-32-32-32h-97.6c-13.28 0-24-10.72-24-24s10.72-24 24-24h97.6c44.16 0 80 35.84 80 80s-35.84 80-80 80zM512 240c-44.16 0-80-35.84-80-80s35.84-80 80-80 80 35.84 80 80-35.84 80-80 80z m0-112c-17.6 0-32 14.4-32 32s14.4 32 32 32 32-14.4 32-32-14.4-32-32-32z\" fill=\"#323232\" p-id=\"3199\"></path><path d=\"M517.28 944c-61.76 0-112-50.24-112-112 0-13.28 10.72-24 24-24s24 10.72 24 24c0 35.36 28.64 64 64 64s64-28.64 64-64c0-13.28 10.72-24 24-24s24 10.72 24 24c0 61.76-50.24 112-112 112z\" fill=\"#323232\" p-id=\"3200\"></path><path d=\"M653.28 512c-13.28 0-24-10.72-24-24v-32c0-50.72-42.08-92-93.76-92-13.28 0-24-10.72-24-24s10.72-24 24-24c78.24 0 141.76 62.72 141.76 140v32c0 13.28-10.72 24-24 24z\" fill=\"#6167CE\" p-id=\"3201\"></path></svg>"
                                                            }
                                                        },
                                                        Height = "100%",
                                                        Width = "100%",
                                                    },
                                                    Header = new Panel
                                                    {
                                                        IsGroup = true,
                                                        Children =
                                                        {
                                                            new TextBlock
                                                            {
                                                                MarginLeft = 34,
                                                                Text = "sdaa文字",
                                                            },
                                                            new SVG
                                                            {
                                                                Classes = "closeBtn",
                                                                IsAntiAlias = true,
                                                                Fill = "#939393",
                                                                MarginRight = 5,
                                                                Width = 13,
                                                                Stretch = Stretch.Uniform,
                                                                Source = "<svg><path d=\"M512 1024a512 512 0 1 1 512-512 512 512 0 0 1-512 512z m0-68.224A443.776 443.776 0 1 0 68.224 512 443.776 443.776 0 0 0 512 955.776z\"></path><path d=\"M682.048 632.704a34.816 34.816 0 0 1-49.28 49.28L518.912 568.128 405.056 681.984a34.816 34.816 0 0 1-49.28-49.28l113.856-113.856-113.856-113.856a34.816 34.816 0 0 1 49.28-49.28l113.856 113.856 113.856-113.856a34.816 34.816 0 0 1 49.28 49.28L568.192 518.848z\"></path></svg>",
                                                            },
                                                            new Picture
                                                            {
                                                                MarginLeft = 0,
                                                                Source = "res://ConsoleApp1/Resources/icon.png",
                                                                Classes = "img",
                                                                Height = 16,
                                                                Width = 16,
                                                                Stretch = Stretch.Uniform,
                                                            }
                                                        },
                                                        Width = 120,
                                                    },
                                                },
                                                new TabItem
                                                {
                                                    Content = new Panel
                                                    {
                                                        Height = "100%",
                                                        Width = "100%",
                                                    },
                                                    Header = new StackPanel
                                                    {
                                                        Orientation = Orientation.Horizontal,
                                                        Classes = "imgAndText",
                                                        IsGroup = true,
                                                        Children =
                                                        {
                                                            new Picture
                                                            {
                                                                Source = "res://ConsoleApp1/Resources/icon.png",
                                                                Classes = "img",
                                                                Height = 16,
                                                                Width = 16,
                                                                Stretch = Stretch.Uniform,
                                                            },
                                                            new TextBlock
                                                            {
                                                                MarginLeft = 5f,
                                                                Classes = "text",
                                                                Text = "文字",
                                                            },
                                                        },
                                                    },
                                                },
                                                new TabItem
                                                {
                                                    Content = new Panel
                                                    {
                                                        Children =
                                                        {

                                                        },
                                                        Height = "100%",
                                                        Width = "100%",
                                                    },
                                                    Header = "TabItem",
                                                },
                                            },
                                            Height = 172,
                                            Width = 332,
                                        },
                                    },
                                    Height = "100%",
                                    Width = "100%",
                                }
                            },
                            new TabItem
                            {
                                Content = new Panel
                                {
                                    Children =
                                    {
                                        new Label
                                        {
                                            MarginLeft = 31,
                                            MarginTop = 22,
                                            Text = "Label11",
                                        },
                                        new CheckBox
                                        {
                                            MarginLeft = 221,
                                            MarginTop = 22,
                                            Content = "CheckBox",
                                        },
                                        new Button
                                        {
                                            Commands =
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(BtnClick),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            Content="弹窗",
                                            MarginLeft=59,
                                            MarginTop=64,
                                            Height=30,
                                            Width=100,
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
                                        new Button
                                        {
                                            Height = 140,
                                            Width = 204,
                                            Content = new StackPanel
                                            {
                                                Height = 43,
                                                Width = 63,
                                                Orientation = Orientation.Vertical,
                                                Classes = "imgAndText",
                                                IsGroup = true,
                                                Children =
                                                {
                                                    new Picture
                                                    {
                                                        Source = "res://ConsoleApp1/Resources/icon.png",
                                                        Classes = "img",
                                                        Height = 16,
                                                        Width = 16,
                                                        Stretch = Stretch.Uniform,
                                                    },
                                                    new TextBlock
                                                    {
                                                        MarginTop = 5f,
                                                        Classes = "text",
                                                        Text = "文字231",
                                                    },
                                                },
                                            },
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
                                        new NativeElement
                                        {
                                            BackColor = "#FFC080",
                                            Height = 172,
                                            Width = 244,
//                                            #if !Net4//||!DesignMode
//                                            Content=new CPF.Mac.AppKit.NSButton{ Title="按钮",Frame=new CPF.Mac.CoreGraphics.CGRect(-20,0,100,50) }
//#endif
                                        }
                                    },
                                    Background="#0f0",
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
                                            MarginLeft = 65,
                                            MarginTop = 63,
                                            BorderStroke = "1,Solid",
                                            BorderFill = "#959595",
                                            PresenterFor = this,
                                            Name = "testGrid",
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
                                                new Border
                                                {
                                                    BorderThickness = "1,2,1,1",
                                                    BorderType = BorderType.BorderThickness,
                                                    MarginLeft = 69,
                                                    Height = 37,
                                                    Width = 47,
                                                },
                                            },
                                            Height = 147,
                                            Width = 165,
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
                                        new Chart
                                        {
                                            GridShowMode = GridShowMode.Horizontal,
                                            GridFill = "#A5A5A5",
                                            YAxisScaleCount = 3,
                                            ChartFill = "#C7E5F6",
                                            CanScroll = true,
                                            IsAntiAlias = true,
                                            Height = 184,
                                            Width = 245,
                                            XAxis=
                                            {
                                                "1",
                                                "2",
                                                "3",
                                                "4",
                                                "5",
                                                "6",
                                                "7",
                                                "8",
                                                "9",
                                                "1",
                                                "2",
                                                "3",
                                                "4",
                                                "5",
                                                "6",
                                                "7",
                                                "8",
                                                "9",
                                            },
                                            Data =
                                            {
                                                new ChartBarData
                                                {
                                                    Name="test",
                                                    Fill="#faf",
                                                    Format="#'%'",
                                                    Data =
                                                    {
                                                        1,
                                                        2,
                                                        3
                                                    }
                                                },
                                                new ChartBarData
                                                {
                                                    Name="test1",
                                                    Fill="#0af",
                                                    Format="#'%'",
                                                    ShowValueTip=true,
                                                    Data =
                                                    {
                                                        4,
                                                        5,
                                                        2
                                                    }
                                                },
                                                new ChartLineData
                                                {
                                                    Name="test2",
                                                    LineFill="#a0f",
                                                    Format="#'%'",
                                                    LineType= LineTypes.Curve,
                                                    BottomFill="#ff000033",//ShowValueTip=true,
                                                    Data =
                                                    {
                                                        4,
                                                        5,
                                                        2,
                                                        3,
                                                        4,
                                                        3,
                                                        4,
                                                        5,
                                                        2,
                                                        3,
                                                        4,
                                                        3,
                                                    }
                                                },
                                                new ChartLineData
                                                {
                                                    Name="test2测试",
                                                    LineFill="#0a0",
                                                    BottomFill="#00000055",
                                                    Data =
                                                    {
                                                        3,
                                                        4,
                                                        5,
                                                        4,
                                                        3,
                                                        5,
                                                        3,
                                                        4,
                                                        5,
                                                        4,
                                                        3,
                                                        5,
                                                        5,
                                                        3,
                                                        4,
                                                        5,
                                                        4,
                                                        6,
                                                        5
                                                    }
                                                },
                                            }
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
                                        new PieChart
                                        {
                                            RingWidth = "40%",
                                            Height = 219,
                                            Width = 279,
                                            IsAntiAlias=true,
                                            Data =
                                            {
                                                new PieChartData
                                                {
                                                    Name="test1",
                                                    Fill="#ff000055",
                                                    Value=0
                                                },
                                                new PieChartData
                                                {
                                                    Name="test2",
                                                    Fill="#00ff0055",
                                                    Value=0
                                                },
                                                new PieChartData
                                                {
                                                    Name="test3",
                                                    Fill="#0000ff55",
                                                    Value=0
                                                },
                                                new PieChartData
                                                {
                                                    Name="test4",
                                                    Fill="#00ffff55",
                                                    Value=5
                                                },
                                            }
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
                                    Height = "100%",
                                    Width = "100%",
                                    Children =
                                    {
                                        new Button
                                        {
                                            MarginLeft = 108,
                                            MarginTop = 95,
                                            Content="test",
                                        },
                                        new TimePicker
                                        {

                                        }
                                    }
                                },
                                Header = "TabItem",
                            },
                        }
                    },
                    new Panel
                    {
                        MarginLeft = 379,
                        MarginTop = 101,
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
                                PresenterFor = this,
                                Name = nameof(textbox1),
                                MarginTop = 3,
                                MarginBottom = 3,
                                MarginRight = 33,
                                MarginLeft = 3,
                                Classes = "singleLine",
                            },
                            new Button
                            {
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
                    new Panel
                    {
                        MarginLeft = 134,
                        MarginTop = 116,
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
                                MarginLeft = 3,
                                MarginTop = 3,
                                Classes = "singleLine",
                            },
                            new TextBlock
                            {
                                MarginLeft = 8,
                                Classes = "placeholder",
                                Text = "placeholder",
                            },
                        },
                        Height = 30,
                        Width = 200,
                    },
                    new SVG
                    {
                        MarginLeft = 882,
                        MarginTop = 137,
                        Height = 57,
                        IsAntiAlias = true,
                        Width = 55,
                        Stretch = Stretch.Uniform,
                        Source="<svg t=\"1615644793381\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"35470\" width=\"128\" height=\"128\"><path d=\"M137.90246 0.00041a48.573421 48.573421 0 0 0-35.589106 15.293433A53.964778 53.964778 0 0 0 87.0404 50.934149V968.345622a48.706541 48.706541 0 0 0 15.272954 35.640306 49.97118 49.97118 0 0 0 35.589106 15.293434h746.336982a48.639981 48.639981 0 0 0 35.589105-15.293434 50.37054 50.37054 0 0 0 15.272954-35.640306V288.717094L646.727857 0.00041H137.90246z\" fill=\"#8095FF\" p-id=\"35471\"></path><path d=\"M138.24038 83.451256a42.490863 42.490863 0 1 0 84.976606 0 42.490863 42.490863 0 0 0-84.976606 0zM138.24038 253.276468a42.490863 42.490863 0 1 0 84.976606 0 42.490863 42.490863 0 0 0-84.976606 0zM138.24038 423.09656a42.490863 42.490863 0 1 0 84.976606 0.13312A42.490863 42.490863 0 0 0 138.24038 423.09656zM138.24038 592.988332a42.490863 42.490863 0 1 0 42.490863-42.490863 42.424303 42.424303 0 0 0-42.490863 42.495983zM138.24038 762.813544a42.490863 42.490863 0 1 0 84.976606 0.13312A42.490863 42.490863 0 0 0 138.24038 762.813544zM138.24038 932.705317a42.490863 42.490863 0 1 0 84.976606 0 42.490863 42.490863 0 0 0-84.976606 0zM796.928116 423.09656a42.490863 42.490863 0 1 0 84.976606 0.13312 42.490863 42.490863 0 0 0-84.981726-0.13312zM796.928116 592.988332a42.490863 42.490863 0 1 0 42.485743-42.490863 42.424303 42.424303 0 0 0-42.490863 42.495983zM796.928116 762.813544a42.490863 42.490863 0 1 0 84.976606 0.13312 42.490863 42.490863 0 0 0-84.981726-0.13312zM796.928116 932.705317a42.490863 42.490863 0 1 0 84.976606 0 42.490863 42.490863 0 0 0-84.981726 0zM631.091383 505.753807L406.011153 351.155469a16.691193 16.691193 0 0 0-17.817593-0.79872A17.817593 17.817593 0 0 0 378.880284 365.583623v308.331397a18.022393 18.022393 0 0 0 9.308156 15.293434 16.824313 16.824313 0 0 0 17.817593-0.86528l225.08535-155.391938a15.359994 15.359994 0 0 0 7.644157-13.634554 18.687993 18.687993 0 0 0-7.644157-13.296635v-0.26624z\" fill=\"#FFFFFF\" p-id=\"35472\"></path><path d=\"M935.101501 288.717094h-237.445025c-27.822069-0.6656-50.22718-23.075831-50.928619-50.93374V0.00041l288.373644 288.716684z\" fill=\"#FFFFFF\" opacity=\".4\" p-id=\"35473\"></path></svg>"
                    },
                    new TreeView
                    {
                        PresenterFor = this,
                        Name = nameof(treeview),
                        MarginLeft = 784,
                        MarginTop = 202,
                        Height = 209,
                        Width = 189,
                        Items =
                        {
                            new TreeViewNavItem
                            {
                                Header=new TextBlock
                                {
                                    Foreground = "#9093A2",
                                    MarginRight = 5f,
                                    MarginLeft = 5f,
                                    Text = ".com",
                                },
                                Items =
                                {
                                    new TreeViewNavItem
                                    {
                                        Tag = "1",
                                        Header="123"
                                    }
                                }
                            },
                            new TreeViewNavItem
                            {
                                Header=new StackPanel
                                {
                                    Orientation = Orientation.Horizontal,
                                    Classes = "imgAndText",
                                    IsGroup = true,
                                    Children =
                                    {
                                        new Picture
                                        {
                                            Source = "res://ConsoleApp1/Resources/arrow.png",
                                            Classes = "img",
                                            Height = 16,
                                            Width = 16,
                                            Stretch = Stretch.Uniform,
                                        },
                                        new TextBlock
                                        {
                                            MarginLeft = 5f,
                                            Classes = "text",
                                            Text = "文字",
                                        },
                                    },
                                },
                                Items =
                                {
                                    new TreeViewNavItem
                                    {
                                        Header="123",
                                        Items =
                                        {
                                            new TreeViewNavItem
                                            {
                                                Tag = "2",
                                                Header="fgsfs"
                                            }
                                        }
                                    },
                                    new TreeViewNavItem
                                    {
                                        Header="4533"
                                    }
                                }
                            },
                            new TreeViewNavItem
                            {
                                Header=new StackPanel
                                {
                                    Orientation = Orientation.Horizontal,
                                    Classes = "imgAndText",
                                    IsGroup = true,
                                    Children =
                                    {
                                        new Picture
                                        {
                                            Source = "res://ConsoleApp1/Resources/icon.png",
                                            Classes = "img",
                                            Height = 16,
                                            Width = 16,
                                            Stretch = Stretch.Uniform,
                                        },
                                        new TextBlock
                                        {
                                            MarginLeft = 5f,
                                            Classes = "text",
                                            Text = "文字1231",
                                        },
                                    },
                                },
                                Items =
                                {
                                    new TreeViewNavItem
                                    {
                                        Header="123"
                                    },
                                    new TreeViewNavItem
                                    {
                                        Header="4533"
                                    }
                                }
                            },
                        }
                    },
                    new Switch
                    {
                        PresenterFor = this,
                        Name = "switch1",
                        MarginLeft = 621,
                        MarginTop = 128,
                        Height = 21,
                        Width = 39,
                        IsChecked=true,
                    },
                    new Panel
                    {
                        MarginLeft = 507,
                        MarginTop = 202,
                        IsGroup = true,
                        Classes = "loginBox",
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
                                        MarginLeft = 3,
                                        MarginTop = 3,
                                        Classes = "singleLine",
                                    },
                                    new TextBlock
                                    {
                                        MarginLeft = 10,
                                        Classes = "placeholder",
                                        Text = "用户名",
                                    },
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
                                        MarginLeft = 4,
                                        MarginRight = 5,
                                        PasswordChar = '#',
                                        MarginBottom = 2,
                                        MarginTop = 2,
                                        Classes = "singleLine",
                                    },
                                    new TextBlock
                                    {
                                        MarginLeft = 10,
                                        Classes = "placeholder",
                                        Text = "密码",
                                    },
                                },
                                Height = 36,
                                Width = 217,
                            },
                            new CheckBox
                            {
                                MarginLeft = 112,
                                MarginTop = 175,
                                Content = "记住密码",
                            },
                            new TextBlock
                            {
                                FontSize = 20f,
                                MarginTop = 13,
                                Text = "XX管理系统",
                            },
                        },
                        Height = 209,
                        Width = 274,
                    },
                    new SVG
                    {
                        Commands =
                        {
                            {
                                "DoubleClick",
                                nameof(onClick),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        MarginLeft = 679,
                        MarginTop = 89,
                        Height = 136,
                        IsAntiAlias = true,
                        Width = 110,
                        Stretch = Stretch.Uniform,
                        Source="<svg t=\"1616939485177\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"9599\" width=\"128\" height=\"128\"><path d=\"M560.974501 23.395513c-1.852254-1.856375-3.702448-3.710689-5.538218-5.538219l-0.243122-0.245181c-23.990707-23.572456-62.76854-23.475619-86.621202 0.247241C320.999445 164.622151 116.55305 423.892379 116.55305 631.066676v0.016483h0.243121C116.796171 854.32818 294.653759 1023.993819 511.991759 1023.993819c60.776182 0.004121 118.327917-13.639512 169.754234-37.988719z\" fill=\"#7CE3FF\" p-id=\"9600\"></path><path d=\"M907.203829 631.074917h0.243121c0-208.931775-197.191905-457.945584-346.472449-607.677344-95.111902 136.613517-197.179543 321.618456-197.179542 488.813736h0.317293c0 219.161409 131.608928 398.654914 317.635801 473.793791 133.339622-63.139402 225.455776-198.347761 225.455776-354.930183z\" fill=\"#B9F0FF\" p-id=\"9601\"></path><path d=\"M291.18001 790.712837a60.186922 47.929904 0 1 0 120.373844 0 60.186922 47.929904 0 1 0-120.373844 0Z\" fill=\"#FF8E9E\" p-id=\"9602\"></path><path d=\"M735.195573 792.909169a60.186922 47.929904 0 1 0-120.373844 0 60.186922 47.929904 0 1 0 120.373844 0Z\" fill=\"#FF8E9E\" p-id=\"9603\"></path><path d=\"M380.137649 704.726222a15.452619 15.452619 0 0 0-15.45262 15.452619v20.603492a15.452619 15.452619 0 0 0 30.905239 0v-20.603492a15.452619 15.452619 0 0 0-15.452619-15.452619zM643.862351 704.726222a15.452619 15.452619 0 0 0-15.452619 15.452619v20.603492a15.452619 15.452619 0 0 0 30.905239 0v-20.603492a15.452619 15.452619 0 0 0-15.45262-15.452619zM552.660992 733.023058a15.446438 15.446438 0 0 0-20.605553 7.275093c-3.432542 7.178257-10.901308 11.816103-19.025264 11.816103s-15.592723-4.637846-19.025265-11.816103a15.452619 15.452619 0 0 0-27.880646 13.33046c8.538087 17.854987 26.949368 29.390882 46.905911 29.390882s38.369884-11.535895 46.90591-29.390882a15.452619 15.452619 0 0 0-7.275093-20.605553z\" fill=\"#313D40\" p-id=\"9604\"></path></svg>"
                    },
                    new TextBox
                    {
                        Text = "2\r\n3",
                        MarginLeft = 505,
                        MarginTop = 138,
                        Height = 64,
                        Width = 98,
                    },
                    new TimePicker
                    {
                        Height = 20,
                        Width = 63,
                        MarginLeft = 614,
                        MarginTop = 63,
                    },
                }
            }));
            if (!DesignMode)//设计模式下不执行
            {

            }
        }
        TextBox textbox1;

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Console.WriteLine("NotifyIcon_DoubleClick");
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            Console.WriteLine("NotifyIcon_Click");
        }

        private void NotifyIcon_MouseUp(object sender, EventArgs e)
        {
            Console.WriteLine("NotifyIcon_MouseUp");
        }

        private void NotifyIcon_MouseDown(object sender, EventArgs e)
        {
            //Console.WriteLine("NotifyIcon_MouseDown");
            //if (this.WindowState == WindowState.Normal)
            //{
            //    WindowState = WindowState.Minimized;
            //}
            //else if (WindowState == WindowState.Minimized)
            //{
            //    WindowState = WindowState.Normal;
            //}

            if (Visibility == Visibility.Collapsed)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        TreeView treeview;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            treeview = FindPresenterByName<TreeView>(nameof(treeview));
            textbox1 = FindPresenterByName<TextBox>(nameof(textbox1));

            textbox1.Focus();
        }
        void btnClick(CpfObject obj, RoutedEventArgs eventArgs)
        {

        }
        void onClick(CpfObject obj, RoutedEventArgs eventArgs)
        {

        }

        void SetNotify(CpfObject obj, RoutedEventArgs eventArgs)
        {
            notifyIcon.Visible = !notifyIcon.Visible;
        }
        void SwitchImage(CpfObject obj, RoutedEventArgs eventArgs)
        {
            notifyIcon.Icon = "res://ConsoleApp1/Resources/arrow.png";
            notifyIcon.Text = "test" + new Random().Next(0, 100);
        }
        bool cancel = false;
        void CancelClosing(CpfObject obj, RoutedEventArgs eventArgs)
        {
            cancel = !cancel;
        }

        protected override void OnClosing(ClosingEventArgs e)
        {
            notifyIcon.Dispose();
            //e.Cancel = cancel;
            base.OnClosing(e);
        }
        async void BtnClick(CpfObject obj, RoutedEventArgs eventArgs)
        {
            //var f = new Window();
            //f.Background = null;
            //f.Width = 300;
            //f.Height = 300;
            //f.CanResize = true;
            //f.Children.Add(new WindowFrame(f, new Button
            //{
            //    Content = "test",
            //    Commands =
            //    {
            //        {nameof(Button.Click),(s,e)=>{
            //        MessageBox.Show("test");
            //        } }
            //    }
            //}));
            //await f.ShowDialog();
            await MessageBox.Show("Test", "标题", this);
        }
        void Click123(CpfObject obj, RoutedEventArgs eventArgs)
        {
            new LayerDialog { Width = 400, Height = 300, Content = new Button { Content = "测试" } }.ShowDialog(this);
        }
    }
}
