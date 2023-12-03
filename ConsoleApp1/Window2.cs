using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF;
using CPF.Drawing;
using CPF.Controls;
using CPF.Animation;
using System.Data;
using System.Diagnostics;
using CPF.Shapes;
using CPF.Svg;
using System.Threading;
using CPF.Input;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
using System.Reflection.Emit;
using CPF.Styling;
using CPF.Documents;
using System.Threading.Tasks;
//#if !DesignMode&&!Net4
////using System.Reactive.Linq;
////using System.Reactive;
//using UglyToad.PdfPig;
//#endif

namespace ConsoleApp1
{
    public class Window2 : Window
    {
        Model model = new Model();
        ThreeDEffect2 effect = new ThreeDEffect2 { Depth = 3000, Y = 90, };
        DataGridColumn column = new DataGridCheckBoxColumn
        {
            Header = "d1fsd",
            Binding = new DataGridBinding("p2")
            {
                BindingMode = BindingMode.TwoWay
            },
            Width = "100",
            HeaderTemplate = typeof(ColumnTemplate),
        };
        protected override void InitializeComponent()
        {
            //var img = Image.FromFile(@"C:\Users\xhm\Desktop\231.gif");
            //TopMost = true;
            Nodes = new Collection<NodeData>
            {
                new NodeData
                {
                    Text="test1",
                    Nodes=
                    {
                        new NodeData
                        {
                            Text="asda"
                        },
                        new NodeData
                        {
                            Text="asda"
                        },
                        new NodeData
                        {
                            Text="1asda"
                        },
                        new NodeData
                        {
                            Text="2asda"
                        },
                    }
                },
                new NodeData
                {
                    Text="测试",
                    Nodes=
                    {
                        new NodeData
                        {
                            Text="3asda"
                        },
                        new NodeData
                        {
                            Text="4asda",
                            Nodes=
                            {
                                new NodeData
                                {
                                    Text="6asda"
                                },
                                new NodeData
                                {
                                    Text="7asda"
                                },
                            }
                        },
                        new NodeData
                        {
                            Text="6asda"
                        },
                        new NodeData
                        {
                            Text="7asda"
                        },
                        new NodeData
                        {
                            Text="4asda",
                            Nodes=
                            {
                                new NodeData
                                {
                                    Text="6asda"
                                },
                                new NodeData
                                {
                                    Text="7asda"
                                },
                                new NodeData
                                {
                                    Text="6asda"
                                },
                                new NodeData
                                {
                                    Text="7asda"
                                },
                            }
                        },
                        new NodeData
                        {
                            Text="3asda"
                        },
                        new NodeData
                        {
                            Text="4asda",
                            Nodes=
                            {
                                new NodeData
                                {
                                    Text="6asda"
                                },
                                new NodeData
                                {
                                    Text="7asda"
                                },
                                new NodeData
                                {
                                    Text="6asda"
                                },
                                new NodeData
                                {
                                    Text="7asda"
                                },
                            }
                        },
                    }
                }
            };
            List1 = new Collection<TestClass>();
            for (int i = 0;
            i < 10;
            i++)
            {
                List1.Add(new TestClass
                {
                    test = i.ToString()
                });
            }
            List2 = new Collection<TestClass>();
            //Columns = new Collection<DataGridColumn>();
            Title = "CPF演示案例";
            Width = 860;
            Height = 600;
            Background = null;
            CanResize = true;
            MinHeight = 100;
            MinWidth = 200;
            DragThickness = 10;
            Children.Add(new WindowFrame(this, new Panel
            {
                Background = null,
                Width = "100%",
                Height = "100%",
                Children =
                {
                    new Button
                    {
                        MarginTop = 0,
                        Content = "点击生成pdf",
                        [nameof(Button.Content)]= new Obx<MainModel>(a => a.Test1.test.test.test.test.Name),
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                (a,b)=>
                                {
                                    var TabControl = FindPresenterByName<TabControl>("mainTab");
                                    #if !Net4
                                CPF.Skia.SkiaPdf.CreatePdf(TabControl.SelectedItem.ContentElement,"test.pdf");
                                    #endif
                                }
                            }
                        },
                    },//内容元素放这里
                    new TabControl
                    {
                        MarginTop = 20,
                        Name="mainTab",
                        PresenterFor = this,
                        TabStripPlacement= Dock.Left,
                        Width="100%",
                        Height="100%",//SelectedIndex=2,
                        Items=
                        {
                            new TabItemTemplate
                            {
                                Header="基础控件",
                                Content=new Panel
                                {
                                    PresenterFor = this,
                                    Name = nameof(page1),
                                    Width="100%",
                                    Height="100%",
                                    Background="#fff",
                                    Children=
                                    {
                                        new Button
                                        {
                                            PresenterFor = this,
                                            Name = nameof(btn),
                                            Classes="Test",
                                            FontStyle= FontStyles.Bold| FontStyles.Italic,
                                            Width=150,
                                            Height=25,
                                            Content="另外一个演示窗体😍",
                                            MarginTop=20,
                                            MarginLeft=20,
                                            [nameof(Button.Click)]=new CommandDescribe((s,e)=>
                                            {
                                                var w = new Window1();
                                                w.DataContext = model;
                                                w.CommandContext = w.DataContext;
                                                //w.TopMost=true;
                                                w.Show();
                                            }),//Commands=
                                            //{
                                            //    {
                                            //        nameof(Button.Click),
                                            //        (s,e)=>
                                            //        {
                                            //            var w = new Window1();
                                            //            w.DataContext = model;
                                            //            w.CommandContext = w.DataContext;
                                            //            //w.TopMost=true;
                                            //            w.Show();
                                            //        }
                                            //    }
                                            //}
                                        },
                                        new CheckBox
                                        {
                                            IsChecked = null,
                                            Content="复选框1",
                                            MarginTop="64",
                                            MarginLeft="20"
                                        },
                                        new CheckBox
                                        {
                                            Content="复选框2",
                                            MarginTop="90",
                                            MarginLeft="20",
                                            IsThreeState=true
                                        },
                                        new RadioButton
                                        {
                                            IsChecked = true,
                                            Content="单选框1",
                                            MarginTop="120",
                                            MarginLeft="20",
                                            GroupName="gn1"
                                        },
                                        new RadioButton
                                        {
                                            Content="单选框2",
                                            MarginTop=150,
                                            MarginLeft=20,
                                            GroupName="gn1"
                                        },
                                        new Border
                                        {
                                            Name="shadowEffect",
                                            MarginTop=180,
                                            MarginLeft=15,
                                            Width=110,
                                            Height=33,
                                            Background="#fff",
                                            BorderType= BorderType.BorderThickness,
                                            ShadowBlur=5,
                                            ShadowColor="0,0,0,0",
                                            Child=new TextBox
                                            {
                                                Classes=
                                                {
                                                    "Single"
                                                },
                                                PresenterFor=this,
                                                Name="textbox1",
                                                MarginBottom=0,
                                                MarginLeft=0,
                                                MarginRight=0,
                                                MarginTop=0,
                                                Text="dfsfs"
                                            }
                                        },//绑定当前页面里的元素，被绑定的元素需要设置PresenterFor=this
                                        new TextBox
                                        {
                                            Padding = "0,5,0,0",
                                            Name="password",
                                            PresenterFor=this,
                                            Classes=
                                            {
                                                "Single"
                                            },
                                            MarginTop=220,
                                            MarginLeft=20,
                                            Width=100,
                                            Height = 24,
                                            Background="#fff",
                                            Bindings=
                                            {
                                                {
                                                    nameof(TextBox.Text),
                                                    nameof(TextBox.Text),
                                                    this.FindPresenter<TextBox>(a=>a.Name=="textbox1")
                                                }
                                            },//Text="test",
                                            PasswordChar='*',
                                            CornerRadius="8",
                                            IsAntiAlias=true
                                        },
                                        new TextBox
                                        {
                                            Commands =
                                            {
                                                {
                                                    nameof(TextBox.MouseDown),
                                                    nameof(testHandled),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                                {
                                                    nameof(TextBox.IsFocused),
                                                    nameof(textFocus),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                                {
                                                    nameof(TextBox.KeyDown),
                                                    nameof(KeyDownTest),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            Bindings =
                                            {
                                                {
                                                    nameof(TextBox.Document),
                                                    nameof(Document),
                                                    this,
                                                    BindingMode.OneWayToSource
                                                },
                                            },
                                            Padding = "5,5,5,5",
                                            PresenterFor = this,
                                            Name = nameof(textBox),
                                            MarginTop=250,
                                            MarginLeft=20,
                                            Width="40%",
                                            Height=258,
                                            Background="#FFFFC7",
                                            Text="多行文本框😀😁😂😃123haha",
                                            IsAllowPasteImage=true,
                                            AcceptsTab=true,
                                            Styles =
                                            {
                                                new DocumentStyle
                                                {
                                                    Foreground="#f00"
                                                }
                                            }
                                        },
                                        new ScrollBar
                                        {
                                            MarginTop = 89,
                                            Maximum = 10f,
                                            Width = 136,
                                            Value = 0.5f,
                                            Name = "scrollbar",
                                            MarginRight=529,
                                            MarginBottom=434,
                                            MarginLeft=171,
                                            Orientation= Orientation.Horizontal,
                                            Background="url(res://ConsoleApp1/Resources/()1.gif) Tile None 0,0,0,0"
                                        },
                                        new ScrollBar
                                        {
                                            Cursor = Cursors.Cross,
                                            MarginLeft=373.6f,
                                            MarginTop=24.2f,
                                            Width=27f,
                                            Height=154.5f,
                                            Orientation= Orientation.Vertical,
                                            Background="#fff"
                                        },
                                        new Picture
                                        {
                                            Source="res://ConsoleApp1/Resources/主页.png",
                                            MarginTop=215,
                                            MarginLeft=200
                                        },
                                        new Picture
                                        {
                                            Height = 41,
                                            Width = 122,
                                            Source="https://dss0.bdstatic.com/5aV1bjqh_Q23odCf/static/superman/img/logo_top-e3b63a0b1b.png",
                                            MarginTop=168,
                                            MarginLeft=167
                                        },
                                        new ComboBox
                                        {
                                            //IsVirtualizing=true,
                                            PresenterFor = this,
                                            Name = nameof(testCombobox),//IsEditable=true,
                                            //SelectionMode= SelectionMode.Multiple,
                                            //SelectedIndex=2,
                                            MarginTop=138,
                                            MarginLeft=262,
                                            Width=100,
                                            Height=25,
                                            ItemTemplate=new ListBoxItem
                                            {
                                                Width="100%",
                                                FontSize=14,
                                                ContentTemplate=new ContentTemplate
                                                {
                                                    Width="auto",
                                                    MarginLeft=5,
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
                                            DisplayMemberPath="Item1",
                                            SelectedValuePath="Item2",//IsVirtualizing=true
                                            IsEditable=true,
                                        },
                                        new ScrollViewer
                                        {
                                            Background = "url(res://ConsoleApp1/icon.png) Tile None 0,0,0,0",
                                            MarginLeft = 421,//HorizontalScrollBarVisibility= ScrollBarVisibility.Disabled,
                                            //VerticalScrollBarVisibility= ScrollBarVisibility.Visible,
                                            Commands =
                                            {
                                                {
                                                    nameof(ScrollViewer.MouseDown),
                                                    nameof(scrollViewerMouseDown),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                                {
                                                    nameof(ScrollViewer.MouseUp),
                                                    nameof(scrollViewerMouseUp),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            Content=
                                            //new Picture
                                            //{
                                            //    PresenterFor = this,
                                            //    Name = nameof(pic),
                                            //    Source="http://219.239.12.91:5001/bookimage//bookimage3/cate1826979600058c0bd3/file253320e4000582XXXX/253320e4000582XXXX.jpg"
                                            //}
                                            
                                            
                                        #if !Net4&&!NETCOREAPP3_0
                                        new GLView
                                            {
                                                Height = 336,
                                                Width = 421,
                                                IsAntiAlias=true,
                                            },
                                            #else
                                            new WrapPanel
                                            {
                                                Width="100%",
                                                Children =
                                                {
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                    new Button
                                                    {
                                                        Content="123"
                                                    },
                                                }
                                            },
                                            #endif
                                            Height=300,
                                            MarginTop=19,
                                            MarginRight=29
                                        },
                                        new Ellipse
                                        {
                                            StrokeStyle = "10,Solid",
                                            Width=145,
                                            Height=83,
                                            IsAntiAlias=true,
                                            Triggers=
                                            {
                                                new CPF.Styling.Trigger(nameof(IsMouseOver),Relation.Me)
                                                {
                                                    Setters=
                                                    {
                                                        {
                                                            nameof(Ellipse.Fill),
                                                            "#f00"
                                                        }
                                                    }
                                                },
                                            },
                                        },
                                        new Expander
                                        {
                                            MarginTop=350,
                                            Header="test",
                                            Content= new Button
                                            {
                                                Content="test内容"
                                            }
                                        }
                                        //.Bind(this,t=>t.FocusFrameFill,s=>s.Background,BindingMode.OneWay)
                                        ,//                                new Button{ Content="test",Commands={ {nameof(Button.Click),(s,e)=>MessageBox.Show("Test")
        //} } },
                                        //new Calendar{ },
                                        new DatePicker
                                        {
                                            Width=100,
                                            Height=20,
                                            MarginTop=100
                                        },//new DatePicker{ Width=100, Height=30},
                                        new NumericUpDown
                                        {
                                            MarginLeft = 386,
                                            MarginTop = 401,
                                            Width = 83,
                                            Minimum =  0.001,
                                            Increment =  0.001,
                                            Maximum=100,
                                            Value=0.001,
                                            Bindings =
                                            {
                                                
                                            }
                                        },
                                        new Button
                                        {
                                            MarginLeft = 175f,
                                            MarginTop = 138.2f,
                                            Content = "附加样式",
                                            Bindings =
                                            {
                                                {
                                                    nameof(Button.Foreground),
                                                    nameof(MainModel.TestBool),
                                                    null,
                                                    BindingMode.OneWay,
                                                    (bool a)=>a?(ViewFill)"#fff":"#000"
                                                }
                                            },
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=>
                                                    {
                                                        this.LoadStyleFile("res://ConsoleApp1/testApend.css", true);
                                                    }
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Commands =
                                            {
                                                {
                                                    "Click",
                                                    nameof(scrollEnd),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            Height = 30,
                                            Width = 98,
                                            MarginLeft = 495,
                                            MarginTop = 362,
                                            Content = "滚动到最底下",
                                        },
                                        new Button
                                        {
                                            Height = 34,
                                            Width = 110,
                                            MarginLeft = 666,
                                            MarginTop = 349,
                                            Content = "Button",
                                            Commands =
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=>
                                                    {
                                                        pic.Source= this.Screen.Screenshot();
                                                    }
                                                }
                                            }
                                        },
                                        new EditComboBox
                                        {
                                            Width = 91,//Bindings =
                                            //{
                                            //    {
                                            //        nameof(ComboBox.Items),
                                            //        nameof(MainModel.TestItems1)
                                            //    },
                                            //},
                                            [nameof(ComboBox.Items)]=nameof(MainModel.TestItems1),//Commands =
                                            //{
                                            //    {
                                            //        nameof(ComboBox.DoubleClick),
                                            //        nameof(comboBoxtest),
                                            //        this,
                                            //        CommandParameter.EventSender,
                                            //        CommandParameter.EventArgs
                                            //    },
                                            //    {
                                            //        nameof(ComboBox.IsDropDownOpen),
                                            //        nameof(TestComboBox),
                                            //        this,
                                            //        CommandParameter.EventSender,
                                            //        CommandParameter.EventArgs
                                            //    },
                                            //},
                                            PresenterFor = this,
                                            Name = nameof(combox),
                                            MarginLeft = 200,
                                            MarginTop = 25,
                                        },
                                        new Slider
                                        {
                                            TickPlacement = TickPlacement.TopLeft,
                                            MarginLeft = 547,
                                            MarginTop = 427,
                                            Height = 25,
                                            Width = 151,
                                        },
                                        new Panel
                                        {
                                            Children =
                                            {
                                                new SVG
                                                {
                                                    Fill = "#959595",
                                                    Height = 29,
                                                    IsAntiAlias = true,
                                                    Width = 33,
                                                    Stretch = Stretch.Uniform,
                                                    Source="<svg t=\"1631519369186\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"3913\" width=\"16\" height=\"16\"><path d=\"M812.698413 212.746384c79.825044 79.825044 121.362963 180.238448 124.613757 300.517813-3.250794 119.918166-44.788713 219.97037-124.613757 300.156614-79.825044 80.186243-179.877249 121.724162-299.795414 124.252557-120.640564-2.528395-220.692769-44.066314-300.517813-124.252557-79.825044-80.186243-121.362963-180.238448-124.613757-300.156614 3.250794-120.640564 44.788713-220.692769 124.613757-300.517813 79.825044-79.825044 179.877249-121.362963 300.517813-124.613757C633.182363 91.383422 732.873369 132.92134 812.698413 212.746384zM786.692063 791.387654c72.962257-72.962257 110.888183-164.345679 114.138977-274.150265-3.250794-109.804586-41.17672-201.188007-114.138977-274.150265-72.601058-72.962257-163.98448-110.888183-273.789065-113.416578C402.737213 131.837743 311.353792 169.763668 238.391534 243.087125c-72.962257 72.962257-110.888183 164.345679-114.138977 274.150265 3.250794 109.804586 41.17672 201.188007 114.138977 274.150265 72.601058 72.962257 164.345679 110.888183 274.872663 113.416578C622.707584 902.275838 714.091005 864.349912 786.692063 791.387654z\" p-id=\"3914\"></path></svg>"
                                                }
                                            },
                                            MarginLeft = 383,
                                            MarginTop = 450,
                                            Height = 58,
                                            Width = 121,
                                        },
                                        new Button
                                        {
                                            Commands =
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(ShowLayer),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            Height = 21,
                                            Width = 105,
                                            MarginTop = 62,
                                            MarginLeft = 195,
                                            Content = "弹出Window2",
                                        },
                                        new ProgressBar
                                        {
                                            Height = 16,
                                            Width = 142,
                                            MarginTop = 473,
                                            MarginLeft = 532,//[nameof(ProgressBar.Value)]="Value",
                                            //[nameof(ProgressBar.Value)]=("Value",BindingMode.TwoWay),
                                            //[nameof(ProgressBar.Value)]=(null,"Value",BindingMode.TwoWay,a=>a.ToString()),
                                            //[nameof(ProgressBar.Value)]=new BindingDescribe(null,"Value",BindingMode.TwoWay,a=>a.ToString())
                                        },
                                        new TextBlock
                                        {
                                            TextTrimming= TextTrimming.CharacterEllipsis,
                                            Height = 59,
                                            Width = 41,
                                            MarginTop = 397,
                                            MarginLeft = 321,
                                            Text = "Te\nxtBlock",
                                        },
                                        new CPF.Controls.Switch
                                        {
                                            PresenterFor = this,
                                            Name = nameof(_sw),
                                            Height = 30,
                                            Width = 64,
                                            MarginTop = 189,
                                            MarginLeft = 300,
                                        }
                                        .Assign(out var sw),
                                        new Button
                                        {
                                            Commands =
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(PDF),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            MarginLeft = 98,
                                            MarginTop = 69,
                                            Content = "解析PDF"+sw.IsChecked,
                                        },
                                        new Viewbox
                                        {
                                            MarginLeft = 426,
                                            MarginTop = 344,
                                            Child = new Path("M159.375 196.875A9.375 9.375 0 0 1 140.625 196.875V133.8375L113.83125 89.2125A9.375 9.375 0 1 1 129.91875 79.55625L158.04375 126.43125A9.375 9.375 0 0 1 159.375 131.25V196.875zM121.875 300A9.375 9.375 0 0 1 121.875 281.25H131.25V261.1875A131.26875 131.26875 0 0 1 69.88125 27.3L58.59375 16.0125A9.375 9.375 0 0 1 71.85 2.7375L85.8375 16.725A130.6875 130.6875 0 0 1 150 0A130.6875 130.6875 0 0 1 214.1625 16.725L228.15 2.7375A9.375 9.375 0 0 1 241.4062500000001 16.0125L230.1375 27.3A131.26875 131.26875 0 0 1 168.75 261.1875V281.25H178.125A9.375 9.375 0 0 1 178.125 300H121.875zM141.3375 243.4125A114.24375 114.24375 0 0 0 158.6625 243.4125A112.5 112.5 0 1 0 141.3375 243.4125zM0 234.375C0 220.25625 6.24375 207.58125 16.125 198.99375A150.65625 150.65625 0 0 0 82.25625 265.125A46.875 46.875 0 0 1 0 234.375zM253.125 281.25C239.00625 281.25 226.33125 275.00625 217.74375 265.125A150.65625 150.65625 0 0 0 283.875 198.99375A46.875 46.875 0 0 1 253.125 281.25z")
                                            {
                                                IsAntiAlias = true,
                                                StrokeStyle = "10,Solid",
                                            },
                                            Height = 30,
                                            Width = 35,
                                        },
                                    }
                                }
                            },
                            new TabItemTemplate
                            {
                                Header="动画",//IsSelected=true,
                                Content=new Panel
                                {
                                    Width="100%",
                                    Height="100%",
                                    Children=
                                    {
                                        new Button
                                        {
                                            Content="弹窗动画",
                                            Width=100,
                                            Height=25,
                                            MarginLeft=20,
                                            MarginTop=20,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=> ShowDialogForm()
                                                }
                                            }
                                        }
                                        .AfterStyle(a=>
                                        {
                                            a.Background="#0f0";
                                        }),
                                        new Button
                                        {
                                            Content="缓动动画1",
                                            Width=100,
                                            Height=25,
                                            MarginLeft=160,
                                            MarginTop=20,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=> Animation((Button)s,new QuadraticEase())
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content="缓动动画2",
                                            Width=100,
                                            Height=25,
                                            MarginLeft=260,
                                            MarginTop=20,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=> Animation((Button)s,new CubicEase())
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content="缓动动画3",
                                            Width=100,
                                            Height=25,
                                            MarginLeft=360,
                                            MarginTop=20,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=> Animation((Button)s,new ElasticEase())
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content="缓动动画4",
                                            Width=100,
                                            Height=25,
                                            MarginLeft=460,
                                            MarginTop=20,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=> Animation((Button)s,new ExponentialEase())
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content="缓动动画5",
                                            Width=100,
                                            Height=25,
                                            MarginLeft=560,
                                            MarginTop=20,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=> Animation((Button)s,new QuinticEase())
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content="缓动动画6",
                                            Width=100,
                                            Height=25,
                                            MarginLeft=660,
                                            MarginTop=20,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=> Animation((Button)s,new SineEase())
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content=new Panel
                                            {
                                                Children=
                                                {
                                                    new SVG("res://ConsoleApp1/test.svg")
                                                    {
                                                        MarginLeft = 0,
                                                        MarginTop = 0,
                                                        Height = 106,
                                                        Width=178,
                                                        Stretch= Stretch.Fill,
                                                    },
                                                    new Picture
                                                    {
                                                        Stretch = Stretch.Fill,
                                                        Source = "res://ConsoleApp1/Resources/te.gif",
                                                        Height = 60,
                                                        Width =80,
                                                    }
                                                },
                                            },
                                            Width=104,
                                            Height=55,
                                            MarginLeft=60,
                                            MarginTop=130,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=> Animation((Button)s)
                                                }
                                            }
                                        },
                                        new Path("M 150,10 T 250,100 80,280")
                                        {
                                            //ClipToBounds = true,
                                            Height = 98,
                                            IsAntiAlias = true,
                                            StrokeStyle = new Stroke(1,DashStyles.Custom,0,new float[]
                                            {
                                                10,
                                                5,
                                                2,
                                                5
                                            },CapStyles.Round,LineJoins.Round),//StrokeFill="url(res://ConsoleApp1/Resources/icon.png)",
                                        },
                                        new Slider
                                        {
                                            MarginLeft = 602,
                                            MarginTop = 258,
                                            Maximum = 100f,
                                            Width = 179.1f,
                                            Bindings=
                                            {
                                                {
                                                    nameof(Slider.Value),
                                                    nameof(TestValue),
                                                    this
                                                }
                                            }
                                        },
                                        new Picture
                                        {
                                            Stretch = Stretch.Fill,
                                            Source = "res://ConsoleApp1/Resources/loading.gif",
                                            Height = 103,
                                            Width = 114,
                                            MarginLeft = 24,
                                            MarginTop = 299,
                                        },
                                        new Picture
                                        {
                                            Stretch = Stretch.Fill,
                                            Source = "res://ConsoleApp1/Resources/te.gif",
                                            Height = 110,
                                            Width = 230,
                                            MarginLeft = 557,
                                            MarginTop = 310,
                                        },
                                        new Line
                                        {
                                            IsHitTestOnPath=true,
                                            Commands =
                                            {
                                                {
                                                    nameof(Line.MouseDown),
                                                    nameof(lineMouseDown),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                        },
                                        new test
                                        {
                                            Width=793,
                                            Height=81,
                                            MarginLeft=16,
                                            MarginBottom=42,
                                            Valeft = 5,
                                            Commands=
                                            {
                                                {
                                                    nameof(SVG.MouseUp),
                                                    (a,b)=> (a as test).Animation(a as test)
                                                }
                                            }
                                        },
                                        new StackPanel
                                        {
                                            Children =
                                            {
                                                new NativeElement
                                                {
                                                    Height = 142,
                                                    Width = 159,
                                                    BackColor=Color.Blue,
                                                }
                                            },
                                            MarginTop = 56,
                                            Height = 172,
                                            Width = 167,
                                        },
                                        new Path
                                        {
                                            IsAntiAlias = true,
                                            Width = 205,
                                            Height = 130,
                                            MarginLeft = 461,
                                            MarginTop = 78,
                                            Fill = "#FFFF00",
                                            Stretch = Stretch.Uniform,
                                            Data="M224.94 813.36c-6.69 0-13.47-1.41-19.96-4.32-19.04-8.52-30.23-27.44-28.53-48.19 0.19-2.37 0.41-4.08 0.51-4.83 1.36-12.07 7.44-54.68 29.59-112.4 14.46-37.62 32.71-72.58 54.26-103.93 27.38-39.65 59.92-73.91 96.77-101.88 9.34-7.73 76.59-61.43 170.59-86.98v-59.34c0-22.02 12.57-41.09 32.82-49.77s42.72-4.63 58.67 10.56l199.82 190.34c15.82 15.07 24.53 35.4 24.53 57.24 0 21.85-8.71 42.17-24.53 57.24L619.65 747.48c-15.95 15.19-38.43 19.24-58.67 10.56-20.24-8.67-32.82-27.74-32.82-49.77v-47.71c-20.67 4.04-40.97 8.95-60.6 14.66-40.38 11.77-77.42 26.68-110.06 44.3-36.29 19.66-64.79 41.98-92.29 72.3-0.9 1.15-2.82 3.56-5.59 6.47-9.38 9.87-21.85 15.07-34.68 15.07z m39.93-21.15c-0.02 0.02-0.04 0.04-0.06 0.07 0.01-0.03 0.03-0.05 0.06-0.07z m-48.27-30.89s-0.15 1.09-0.29 2.81c-0.42 5.17 2.96 7.49 5.01 8.41 1.99 0.89 5.85 1.84 9.3-1.79 1.6-1.69 2.68-3.05 3.26-3.79 0.36-0.46 0.74-0.94 1.26-1.51 30.39-33.6 63.23-59.37 103.33-81.1 35.14-18.97 74.81-34.96 117.91-47.52 28.51-8.29 58.31-14.98 88.58-19.89 5.79-0.94 11.7 0.71 16.16 4.51 4.47 3.8 7.04 9.37 7.04 15.23v71.6c0 8.42 5.99 11.89 8.57 13 2.58 1.1 9.23 3.05 15.33-2.76l199.82-190.34c7.81-7.44 12.12-17.49 12.12-28.28 0-10.79-4.3-20.83-12.12-28.28L592.06 281.27c-6.09-5.81-12.75-3.86-15.33-2.76s-8.57 4.58-8.57 13v74.93c0 9.32-6.44 17.4-15.52 19.49-94.41 21.72-162.4 76.72-169.84 82.92-0.24 0.2-0.48 0.39-0.73 0.58-33.6 25.44-63.32 56.72-88.34 92.96-19.75 28.73-36.54 60.89-49.86 95.57-20.59 53.67-26.03 92.21-27.2 102.74-0.02 0.21-0.04 0.41-0.07 0.62z m0 0z"
                                        },
                                    }
                                }
                            },
                            new TabItemTemplate
                            {
                                Header="布局",
                                Content=new Panel
                                {
                                    Name = "布局",
                                    PresenterFor = this,
                                    Width="100%",
                                    Height="100%",
                                    Children=
                                    {
                                        new StackPanel
                                        {
                                            MarginLeft=10,
                                            MarginTop=10,
                                            Orientation= Orientation.Vertical,
                                            Children=
                                            {
                                                new Button
                                                {
                                                    Content="StackPanel的Vertical"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                            }
                                        },
                                        new StackPanel
                                        {
                                            BorderStroke = "5,Solid",
                                            BorderFill = "#B4B4B4",
                                            MarginLeft=80,
                                            MarginTop=50,
                                            Orientation= Orientation.Horizontal,
                                            Children=
                                            {
                                                new Button
                                                {
                                                    Content="StackPanel的Horizontal"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="Margin调间距",
                                                    MarginLeft=5
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                            }
                                        },
                                        new WrapPanel
                                        {
                                            MarginRight=10,
                                            MarginTop=10,
                                            Width="50%",
                                            Orientation= Orientation.Horizontal,
                                            Children=
                                            {
                                                new Button
                                                {
                                                    Content="WrapPanel的Horizontal"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="Margin调间距",
                                                    MarginLeft=5
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="宽度不够"
                                                },
                                                new Button
                                                {
                                                    Content="可以自动换行"
                                                },
                                            }
                                        },
                                        new Grid
                                        {
                                            RenderTransform=new RotateTransform(10),
                                            Name="testGrid",
                                            Background="#999",
                                            Width="80%",
                                            Height="60%",
                                            MarginTop=120,
                                            MarginLeft=20,
                                            ColumnDefinitions=
                                            {
                                                new ColumnDefinition
                                                {
                                                    Width="40*"
                                                },
                                                new ColumnDefinition
                                                {
                                                    Width = "30*"
                                                },
                                                new ColumnDefinition
                                                {
                                                    Width="200",
                                                    [nameof(ColumnDefinition.Width)]=nameof(MainModel.ColumnWidth)
                                                },
                                            },
                                            RowDefinitions=
                                            {
                                                new RowDefinition
                                                {
                                                    Height="30*"
                                                },
                                                new RowDefinition
                                                {
                                                    Height="30*"
                                                },
                                                new RowDefinition
                                                {
                                                    Height="30*"
                                                }
                                            },
                                            Children=
                                            {
                                                new WrapPanel
                                                {
                                                    Name="test",
                                                    Background="#a2f",
                                                    Width="100%",
                                                    Height="100%",
                                                    Children=
                                                    {
                                                        new Button
                                                        {
                                                            Content="水平浮动布局231"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮2"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮3"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮4"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮5"
                                                        },
                                                    }
                                                },
                                                {
                                                    new WrapPanel
                                                    {
                                                        Orientation= Orientation.Vertical,
                                                        Background="#27a",
                                                        Width="100%",
                                                        Height="100%",
                                                        Children=
                                                        {
                                                            new Button
                                                            {
                                                                Content="垂直浮动布局"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮2"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮3"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮4"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮5"
                                                            },
                                                        }
                                                    },
                                                    1,
                                                    1
                                                },
                                                {
                                                    new TextBlock
                                                    {
                                                        Background="#ac2",
                                                        Width="100%",
                                                        Height="100%",
                                                        Text="Grid布局。。。"
                                                    },
                                                    2,
                                                    1
                                                },
                                                {
                                                    new Panel
                                                    {
                                                        Background="#b1a",
                                                        MarginLeft=0,
                                                        MarginRight=0,
                                                        Children=
                                                        {
                                                            new Button
                                                            {
                                                                Content="跨列",
                                                                Width="50%"
                                                            }
                                                        }
                                                    },
                                                    0,
                                                    2,
                                                    2
                                                },
                                                {
                                                    new TextBlock
                                                    {
                                                        Background="#186",
                                                        Height="100%",
                                                        Text="跨行"
                                                    },
                                                    2,
                                                    1,
                                                    1,
                                                    2
                                                },
                                                new TextBox
                                                {
                                                    MarginLeft=10,
                                                    Size=SizeField.Fill,
                                                    Text="元素变换，可以旋转，倾斜，缩放等操作",
                                                    Attacheds=
                                                    {
                                                        {
                                                            Grid.ColumnIndex,
                                                            1
                                                        }
                                                    }
                                                },
                                                new Button
                                                {
                                                    Content=new SVG("res://ConsoleApp1/test.svg")
                                                    {
                                                        MarginLeft = 0,
                                                        MarginTop = 0,
                                                        Height = 85,
                                                        Width=170,
                                                        Stretch= Stretch.Uniform,
                                                    },
                                                    Width=104,
                                                    Height=55,
                                                    MarginLeft=60,
                                                    MarginTop=120,
                                                    Commands=
                                                    {
                                                        {
                                                            nameof(Button.Click),
                                                            (s,e)=> Animation((Button)s)
                                                        }
                                                    }
                                                }
                                            },
                                        },
                                        new DockPanel
                                        {
                                            LastChildFill = false,
                                            Width=200,
                                            Height=200,
                                            MarginRight=0,
                                            MarginTop=50,
                                            Background="#f00",
                                            Children =
                                            {
                                                new Button
                                                {
                                                    Content="Right",
                                                    Height="100%",
                                                    Attacheds =
                                                    {
                                                        {
                                                            DockPanel.Dock,
                                                            Dock.Right
                                                        }
                                                    }
                                                },
                                            }
                                        },
                                        new Slider
                                        {
                                            Maximum = 300,
                                            Value = 200,
                                            MarginLeft = 252,
                                            MarginTop = 76,
                                            Height = 23,
                                            Width = 219,//[nameof(Slider.Value)]= new Obx<MainModel>(a => a.Type.Name),
                                            [nameof(Slider.Value)]= new BindingDescribe(null, nameof(MainModel.ColumnWidth),BindingMode.OneWayToSource,null,a=>new GridLength((float)(double)a))
                                        },
                                    }
                                }
                            },
                            new TabItemTemplate
                            {
                                Header="ListBox",
                                Content=new Panel
                                {
                                    Width="100%",
                                    Height="100%",
                                    Children=
                                    {
                                        new ListBox
                                        {
                                            SelectionMode = SelectionMode.Multiple,
                                            AlternationCount = 2,
                                            Width=300,
                                            Height=500,
                                            ItemTemplate=typeof(ListBoxItemTemplate),
                                            Bindings=
                                            {
                                                {
                                                    nameof(ListBox.Items),
                                                    nameof(Items),
                                                    this
                                                }
                                            },
                                        },
                                        new Button
                                        {
                                            Commands =
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(TestClear),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            Height = 25,
                                            Width = 80,
                                            MarginTop = 47,
                                            MarginLeft = 536,
                                            Content = "Button",
                                        },
                                        new DataGrid
                                        {
                                            PresenterFor = this,
                                            Name = nameof(testGrid),
                                            Height = 113,
                                            Width = 132,
                                            MarginTop = 72,
                                            MarginLeft = 47,
                                        },
                                        new Button
                                        {
                                            Commands =
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(addColumnClick),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            MarginTop = 218,
                                            MarginLeft = 47,
                                            Content = "添加列",
                                        },
                                    }
                                }
                            },
                            new TabItemTemplate
                            {
                                Header="DataGrid",
                                Content=new Panel
                                {
                                    Width="100%",
                                    Height="100%",
                                    Children=
                                    {
                                        new DataGrid
                                        {
                                            ContextMenu=new ContextMenu
                                            {
                                                Items=
                                                {
                                                    new MenuItem
                                                    {
                                                        Header="123"
                                                    }
                                                }
                                            },
                                            PresenterFor = this,
                                            Name = nameof(testDataGrid),//IsVirtualizing=false,
                                            Width = 544,
                                            Height = 344,//VirtualizationMode= VirtualizationMode.Standard,
                                            Background = "#fff",//CustomScrollData=customScrollData,
                                            ItemTemplate=typeof(DataGridRowTemplate),
                                            AlternationCount=2,
                                            SelectionUnit= DataGridSelectionUnit.Cell,//SelectionMode= DataGridSelectionMode.Single,
                                            Columns =
                                            {
                                                new DataGridComboBoxColumn
                                                {
                                                    Header="dfsd",
                                                    Binding=new DataGridBinding("p1",BindingMode.TwoWay),
                                                    Width="100",
                                                    Items=
                                                    {
                                                        "0",
                                                        "1",
                                                        "2",
                                                        "3"
                                                    },//Visibility= Visibility.Collapsed
                                                },
                                                column,
                                                new DataGridTextColumn
                                                {
                                                    Header="3dfsd",
                                                    Binding=new DataGridBinding("p3")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="输入类型验证",
                                                    Binding=new DataGridBinding("p4")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="3dfsd",
                                                    Binding=new DataGridBinding("p3")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="输入类型验证",
                                                    Binding=new DataGridBinding("p4")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="3dfsd",
                                                    Binding=new DataGridBinding("p3")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="输入类型验证",
                                                    Binding=new DataGridBinding("p4")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="3dfsd",
                                                    Binding=new DataGridBinding("p3")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="输入类型验证",
                                                    Binding=new DataGridBinding("p4")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="3dfsd",
                                                    Binding=new DataGridBinding("p3")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="输入类型验证",
                                                    Binding=new DataGridBinding("p4")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="3dfsd",
                                                    Binding=new DataGridBinding("p3")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="输入类型验证",
                                                    Binding=new DataGridBinding("p4")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="输入类型验证",
                                                    Binding=new DataGridBinding("p4")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="3dfsd",
                                                    Binding=new DataGridBinding("p3")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="输入类型验证",
                                                    Binding=new DataGridBinding("p4")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="3dfsd",
                                                    Binding=new DataGridBinding("p3")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                },
                                                new DataGridTextColumn
                                                {
                                                    Header="输入类型验证",
                                                    Binding=new DataGridBinding("p4")
                                                    {
                                                        BindingMode= BindingMode.TwoWay
                                                    },
                                                    Width="100",
                                                }
                                                //new DataGridTemplateColumn
                                                //{
                                                //    Header="自定义模板",
                                                //    Binding=new DataGridBinding("p5"),
                                                //    Width="*",
                                                //    CellTemplate=typeof(CellTemplate)
                                                //},
                                            },
                                            Bindings =
                                            {
                                                {
                                                    nameof(DataGrid.Items),
                                                    nameof(Data),
                                                    this
                                                },//{
                                                //    nameof(DataGrid.Items),
                                                //    nameof(ItemCollection),
                                                //    this,
                                                //    BindingMode.OneWayToSource
                                                //},//{ nameof(DataGrid.Columns), nameof(Columns),this }
                                            }
                                        },
                                        new Button
                                        {
                                            Commands =
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(AddTest),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            MarginLeft = 245,
                                            MarginTop = 21,
                                            Content = "添加",
                                        },
                                        new Button
                                        {
                                            Commands =
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(ClearData),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            MarginLeft = 355,
                                            MarginTop = 21,
                                            Content = "清除",
                                        },
                                        new Button
                                        {
                                            Commands =
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(addColumn),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            MarginLeft = 442,
                                            MarginTop = 17,
                                            Content = "addColumn",
                                        },
                                    }
                                }
                            },
                            new TabItemTemplate
                            {
                                Header="TreeView",
                                Content=new Panel
                                {
                                    Width="100%",
                                    Height="100%",
                                    Children=
                                    {
                                        new TreeView
                                        {
                                            PresenterFor = this,
                                            Name = nameof(testTreeVIew),
                                            Bindings =
                                            {
                                                {
                                                    "SelectedValue",
                                                    nameof(SelectNode),
                                                    this,
                                                    BindingMode.OneWayToSource
                                                },
                                                {
                                                    "Items",
                                                    "Nodes",
                                                    this
                                                },
                                            },
                                            Width=300,
                                            Height=500,
                                            DisplayMemberPath=nameof(NodeData.Text),
                                            ItemsMemberPath=nameof(NodeData.Nodes),//HeaderTemplate=typeof(TreeViewItemContentTemplate),
                                            //ItemTemplate=typeof(TreeViewItemTemplate),
                                            //Items=new TreeViewItem[]{ new TreeViewItem { Header="24" } }
                                        },
                                        new Button
                                        {
                                            Commands =
                                            {
                                                {
                                                    "Click",
                                                    nameof(addItem),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            MarginLeft = 599,
                                            MarginTop = 24,
                                            Content = "添加子节点",
                                        },
                                        new Button
                                        {
                                            MarginLeft = 597,
                                            MarginTop = 66,
                                            Commands =
                                            {
                                                {
                                                    "Click",
                                                    nameof(RemoveItem),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            Content = "删除节点",
                                        },
                                    }
                                }
                            },
                            new TabItemTemplate
                            {
                                //[nameof(Name)]=nameof(Button.Content),
                                //[nameof(Name)]=(nameof(Button.Content),BindingMode.TwoWay),
                                //[nameof(MouseDown)]=new Action<CpfObject,object>((a,b)=>{ }),
                                Header="test",
                                Content= new Panel
                                {
                                    Commands =
                                    {
                                        {
                                            nameof(Button.TouchMove),
                                            nameof(TouchDownTest),
                                            this,
                                            CommandParameter.EventSender,
                                            CommandParameter.EventArgs
                                        },
                                    },
                                    Size=SizeField.Fill,
                                    Children =
                                    {
                                        new SVG("res://ConsoleApp1/test.svg")
                                        {
                                            Commands =
                                            {
                                                {
                                                    nameof(SVG.MouseUp),
                                                    nameof(svgMouseUP),
                                                    this,
                                                    CommandParameter.EventSender,
                                                    CommandParameter.EventArgs
                                                },
                                            },
                                            MarginLeft = 442,
                                            MarginTop = 50,
                                            Height = 199,
                                            Width=245,
                                            Stretch= Stretch.Uniform,
                                        }
                                        //new Border
                                        //{
                                        //    BorderFill="#f00",
                                        //    BorderStroke="1",
                                        //    Child=new Border
                                        //    {
                                        //        Size=SizeField.Fill,
                                        //        BorderFill="#0f0",
                                        //        BorderStroke="1",
                                        //        Child=new StackPanel
                                        //        {
                                        //            Children =
                                        //            {
                                        //                new TextBlock{Text="432sd"},
                                        //                new TextBlock{Text="gdfsa"},
                                        //                new TextBlock{Text="gsdsd"},
                                        //                new TextBlock{Text="gfdfs"},
                                        //            }
                                        //        }
                                        //    }
                                        //}
                                    ,
                                        new Viewbox
                                        {
                                            Stretch = Stretch.Uniform,
                                            Child = new Button
                                            {
                                                Commands =
                                                {
                                                    {
                                                        nameof(Button.MouseUp),
                                                        nameof(ShowPopup),
                                                        this,
                                                        CommandParameter.EventSender,
                                                        CommandParameter.EventArgs
                                                    },
                                                },
                                                Height = 79,
                                                Width = 88,
                                                Content = "弹窗测试",
                                            },
                                            MarginLeft = 16,
                                            MarginTop = 84,
                                            Height = 116,
                                            Width = 190,
                                        },
                                        new  WrapPanel
                                        {
                                            Height = 66,
                                            Width = 78,
                                        }
                                        .LoopCreate(10,i=>new Button
                                        {
                                            Content=i
                                        }),
                                        new Border
                                        {
                                            Child = new Button
                                            {
                                                MarginTop = 17,
                                                MarginLeft = 21,
                                                Content = "Button",
                                            },
                                            Height = 69,
                                            Width = 131,
                                            MarginTop = 42,
                                            MarginLeft = 275,
                                        },
                                        new Button
                                        {
                                            Height = 30,
                                            Width = 95,
                                            MarginTop = 152,
                                            MarginLeft = 295,
                                            Content = "Button",
                                        },
                                        new CheckBox
                                        {
                                            MarginTop = 26.2f,
                                            MarginLeft = 101.8f,
                                            Content = "CheckBox",
                                        },
                                        new Panel
                                        {
                                            MarginLeft = 30,
                                            MarginTop = 281,
                                            IsGroup = true,
                                            Children =
                                            {
                                                new Button
                                                {
                                                    Width = 26,
                                                    Height = "100%",
                                                    MarginRight = 0,
                                                    Content = new Polyline
                                                    {
                                                        Points =
                                                        {
                                                            {
                                                                0,
                                                                0
                                                            },
                                                            {
                                                                5,
                                                                5
                                                            },
                                                            {
                                                                10,
                                                                0
                                                            }
                                                        },
                                                        IsAntiAlias=true
                                                    },
                                                    ContextMenu=new ContextMenu
                                                    {
                                                        Items =
                                                        {
                                                            new MenuItem
                                                            {
                                                                StaysOpenOnClick=true,
                                                                Header="123"
                                                            },
                                                            new Separator
                                                            {
                                                                
                                                            },
                                                            new MenuItem
                                                            {
                                                                Header="1233423",
                                                                Items =
                                                                {
                                                                    new MenuItem
                                                                    {
                                                                        Header="45242"
                                                                    }
                                                                }
                                                            },
                                                        }
                                                    },
                                                    Commands =
                                                    {
                                                        {
                                                            nameof(Button.MouseUp),
                                                            (s,e)=>
                                                            {
                                                                var E=e as MouseButtonEventArgs;
                                                                if (E.MouseButton== MouseButton.Left)
                                                                {
                                                                    var cm = (s as UIElement).ContextMenu;
                                                                    if (cm != null)
                                                                    {
                                                                        cm.DataContext = DataContext;
                                                                        cm.CommandContext = CommandContext;
                                                                        cm.PlacementTarget = s as UIElement;
                                                                        cm.Placement=  PlacementMode.Padding;
                                                                        cm.PopupMarginLeft=0;
                                                                        cm.PopupMarginTop="100%";
                                                                        cm.IsOpen = true;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    E.Handled=true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                },
                                                new TextBlock
                                                {
                                                    MarginLeft = 11,
                                                    Text = "带下拉菜单",
                                                    Commands =
                                                    {
                                                        {
                                                            nameof(MouseDown),
                                                            (s,e)=>
                                                            {
                                                                (s as UIElement).Parent.GetChildren()[0].ContextMenu.Items.Add(new MenuItem
                                                                {
                                                                    Header="test"
                                                                });
                                                            }
                                                        }
                                                    }
                                                },
                                            },
                                            Height = 31,
                                            Width = 129,
                                        }
                                    }
                                }
                            },
                            new TabItemTemplate
                            {
                                Content = new Page1
                                {
                                    Height = "100%",
                                    Width = "100%",
                                },
                                Header = "TabItem",
                            },
                            new TestTabItem
                            {
                                Content = new Panel
                                {
                                    Children =
                                    {
                                        new Grid
                                        {
                                            Children =
                                            {
                                                new TextBox
                                                {
                                                    MarginLeft=10,
                                                    Size=SizeField.Fill,
                                                    Text="元素变换，可以旋转，倾斜，缩放等操作1111111111111111111111111111111111111111111111111111111111111112",
                                                    Attacheds=
                                                    {
                                                        {
                                                            Grid.ColumnIndex,
                                                            1
                                                        }
                                                    }
                                                }
                                            },
                                            Height = 228,
                                            Width = "100%",
                                            ColumnDefinitions =
                                            {
                                                new ColumnDefinition
                                                {
                                                    Width=250
                                                },
                                                new ColumnDefinition
                                                {
                                                    Width="*"
                                                },
                                            }
                                        },
                                    },
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
                                        new SVG
                                        {
                                            Fill = "#959595",
                                            Height = 28,
                                            IsAntiAlias = true,
                                            Width = 28,
                                            Stretch = Stretch.Uniform,
                                            Source="<svg t=\"1631519369186\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"3913\" width=\"16\" height=\"16\"><path d=\"M812.698413 212.746384c79.825044 79.825044 121.362963 180.238448 124.613757 300.517813-3.250794 119.918166-44.788713 219.97037-124.613757 300.156614-79.825044 80.186243-179.877249 121.724162-299.795414 124.252557-120.640564-2.528395-220.692769-44.066314-300.517813-124.252557-79.825044-80.186243-121.362963-180.238448-124.613757-300.156614 3.250794-120.640564 44.788713-220.692769 124.613757-300.517813 79.825044-79.825044 179.877249-121.362963 300.517813-124.613757C633.182363 91.383422 732.873369 132.92134 812.698413 212.746384zM786.692063 791.387654c72.962257-72.962257 110.888183-164.345679 114.138977-274.150265-3.250794-109.804586-41.17672-201.188007-114.138977-274.150265-72.601058-72.962257-163.98448-110.888183-273.789065-113.416578C402.737213 131.837743 311.353792 169.763668 238.391534 243.087125c-72.962257 72.962257-110.888183 164.345679-114.138977 274.150265 3.250794 109.804586 41.17672 201.188007 114.138977 274.150265 72.601058 72.962257 164.345679 110.888183 274.872663 113.416578C622.707584 902.275838 714.091005 864.349912 786.692063 791.387654z\" p-id=\"3914\"></path></svg>"
                                        },
                                        new TextBlock
                                        {
                                            MarginLeft = 5f,
                                            Classes = "text",
                                            Text = "文字",
                                        },
                                    },
                                },
                                Triggers =
                                {
                                    new Trigger(nameof(TestTabItem.IsMouseOver), Relation.Me)
                                    {
                                        Setters =
                                        {
                                            {
                                                nameof(TestTabItem.Foreground),
                                                "#419EFF"
                                            },
                                            {
                                                nameof(TestTabItem.Background),
                                                "#ecf5ff"
                                            }
                                        }
                                    },
                                    new Trigger(nameof(TestTabItem.IsSelected), Relation.Me)
                                    {
                                        Setters =
                                        {
                                            {
                                                nameof(TestTabItem.Foreground),
                                                "white"
                                            },
                                            {
                                                nameof(TestTabItem.Background),
                                                "#3a8ee6"
                                            }
                                        }
                                    }
                                }
                            },
                            new TabItemTemplate
                            {
                                Header = "生成pdf",
                                Content = new Panel
                                {
                                    Size = new SizeField("100%","100%"),
                                },
                            },
                            new TabItemTemplate
                            {
                                Header = "TextBox",
                                Content = new Panel
                                {
                                    Size = new SizeField("100%","100%"),
                                    Children =
                                    {
                                        //                                        new CodeTextBox
//                                        {
//                                            MarginTop = 0,
//                                            MarginLeft = 0,
//                                            Width="70%",
//                                            Height=519,
//                                            FontSize = 20,
//                                            WordWarp = false,
//                                            FontFamily = "新宋体",
//                                            HScrollBarVisibility = ScrollBarVisibility.Visible,
//                                            VScrollBarVisibility = ScrollBarVisibility.Visible,
//                                            SelectionFill = "153,201,239",
//                                            Text = @"new TabItemTemplate{
//    Header = ""生成pdf"",
//    Content = new Panel{
//        Size = new SizeField(""100%"",""100%""),
//    },

//",
//                                            Background="255,255,255",
//                                            Styles=
//                                            {
//                                                new DocumentStyle
//                                                {
//                                                    Foreground = "#008000"
//                                                },
//                                                new DocumentStyle
//                                                {
//                                                    Foreground = "128,0,0"
//                                                },
//                                                new DocumentStyle
//                                                {
//                                                    Foreground = "DarkBlue"
//                                                },
//                                                new DocumentStyle
//                                                {
//                                                    Foreground = "143,8,196"
//                                                },
//                                                new DocumentStyle
//                                                {
//                                                    Foreground = "31,86,173"
//                                                },
//                                                new DocumentStyle
//                                                {
//                                                    Foreground = "#8000FF"
//                                                },
//                                                new DocumentStyle
//                                                {
//                                                    Foreground = "#c3c3fd"
//                                                },
//                                            },
//                                            KeywordsStyles=
//                                            {
//                                                new KeywordsStyle
//                                                {
//                                                    Keywords = @"(\/\/.*|\/\*[\s\S]*?\*\/)",// 匹配注释
//                                                    IsRegex =true,
//                                                    StyleId = 0
//                                                },
//                                                new KeywordsStyle
//                                                {
//                                                    Keywords = @"(""(?:[^""\\]|\\[\s\S])*""|'(?:[^'\\]|\\[\s\S])*')",// 匹配字符串
//                                                    IsRegex=true,
//                                                    StyleId = 1
//                                                },
//                                                new KeywordsStyle
//                                                {
//                                                    Keywords = @"\b(var|for|if|else|return|this|while|new|function|switch|case|typeof|do|in|throw|try|catch|finally|with|instance|delete|void|break|continue)\b",//匹配关键词，关键词顺序改了下
//                                                    IsRegex = true,
//                                                    StyleId = 2
//                                                },/*new KeywordsStyle
//                                                {
//                                                    Keywords = @"(?:[^\W\d]|\$)[\$\w]*",// 匹配普通的变量名
//                                                    IsRegex =true,
//                                                    StyleId = 4
//                                                },*/
//                                                //new KeywordsStyle
//                                                //{
//                                                //    Keywords = @"(0[xX][0-9a-fA-F]+|\d+(?:\.\d+)?(?:[eE][+-]?\d+)?|\.\d+(?:[eE][+-]?\d+)?)",// 匹配数字，修复了匹配
//                                                //    IsRegex =true,
//                                                //    StyleId = -1
//                                                //},
//                                                //new KeywordsStyle
//                                                //{
//                                                //    Keywords = @"(?:^|[^\)\]\}])(\/(?!\*)(?:\\.|[^\\\/\n])+?\/[gim]*)",//匹配正则
//                                                //    IsRegex =true,
//                                                //    StyleId = -1
//                                                //},
//                                            }
//                                        },
                                        new CodeTextBox
                                        {
                                            //FontSize=16,
                                            AcceptsTab=true,
                                            MarginBottom = 22,
                                            MarginRight = 208,
                                            Background = "#FFFFFF",
                                            MarginLeft = 3,
                                            MarginTop = -1,
                                            IsUndoEnabled=true,//Text="231231\n3\n4\n5\n6\n7\n8\n4\n5\n6\n7\n8\n7\n8\n4\n5\n6\n7\n81dfssssssssssssssssssssssssa\n2\n3\n4\n5\n6\n7\n8\n9asddffsfsfs\n10\n11\n12",
                                            Styles=
                                            {
                                                new CodeStyle
                                                {
                                                    Foreground = "#008000",//Background="#00ff00",
                                                },
                                                new CodeStyle
                                                {
                                                    Foreground = "128,0,0",//Background="#00ffff",
                                                },
                                                new CodeStyle
                                                {
                                                    Foreground = "DarkBlue",//Background="#0f0fff",
                                                },
                                                new CodeStyle
                                                {
                                                    Foreground = "143,8,196",// Background="#ff00ff",
                                                },
                                                new CodeStyle
                                                {
                                                    Foreground = "31,86,173"
                                                },
                                                new CodeStyle
                                                {
                                                    Foreground = "#8000FF"
                                                },
                                                new CodeStyle
                                                {
                                                    Foreground = "#c3c3fd"
                                                },
                                            },
                                            KeywordsStyles=
                                            {
                                                new KeywordsStyle
                                                {
                                                    Keywords = @"\b(var|for|if|else|return|this|while|new|function|switch|case|typeof|do|in|throw|try|catch|finally|with|instance|delete|void|break|continue)\b",//匹配关键词，关键词顺序改了下
                                                    IsRegex = true,
                                                    StyleId = 2
                                                },
                                                new KeywordsStyle
                                                {
                                                    Keywords = @"(\/\/.*|\/\*[\s\S]*?\*\/)",// 匹配注释
                                                    IsRegex =true,
                                                    StyleId = 0
                                                },
                                                new KeywordsStyle
                                                {
                                                    Keywords = @"(""(?:[^""\\]|\\[\s\S])*""|'(?:[^'\\]|\\[\s\S])*')",// 匹配字符串
                                                    IsRegex=true,
                                                    StyleId = 1
                                                },/*new KeywordsStyle
                                                {
                                                    Keywords = @"(?:[^\W\d]|\$)[\$\w]*",// 匹配普通的变量名
                                                    IsRegex =true,
                                                    StyleId = 4
                                                },*/
                                                //new KeywordsStyle
                                                //{
                                                //    Keywords = @"(0[xX][0-9a-fA-F]+|\d+(?:\.\d+)?(?:[eE][+-]?\d+)?|\.\d+(?:[eE][+-]?\d+)?)",// 匹配数字，修复了匹配
                                                //    IsRegex =true,
                                                //    StyleId = -1
                                                //},
                                                //new KeywordsStyle
                                                //{
                                                //    Keywords = @"(?:^|[^\)\]\}])(\/(?!\*)(?:\\.|[^\\\/\n])+?\/[gim]*)",//匹配正则
                                                //    IsRegex =true,
                                                //    StyleId = -1
                                                //},
                                            }
                                        },
                                        new TextBox
                                        {
                                            MarginBottom = 22,
                                            MarginTop = 0,
                                            MarginRight = 0,
                                            Width=207,
                                            FontSize = 13,
                                            SelectionFill = "153,201,239",
                                            Text = @"
new TabItemTemplate{
    Header = ""生成pdf"",
    Content = new Panel{
        Size = new SizeField(""100%"",""100%""),
    },
}
",
                                            Background="255,255,255",
                                            Styles=
                                            {
                                                new DocumentStyle
                                                {
                                                    Foreground = "#008000"
                                                },
                                                new DocumentStyle
                                                {
                                                    Foreground = "128,0,0"
                                                },
                                                new DocumentStyle
                                                {
                                                    Foreground = "DarkBlue"
                                                },
                                                new DocumentStyle
                                                {
                                                    Foreground = "143,8,196"
                                                },
                                                new DocumentStyle
                                                {
                                                    Foreground = "31,86,173"
                                                },
                                                new DocumentStyle
                                                {
                                                    Foreground = "#8000FF"
                                                },
                                                new DocumentStyle
                                                {
                                                    Foreground = "#c3c3fd"
                                                },
                                            },
                                            KeywordsStyles=
                                            {
                                                new KeywordsStyle
                                                {
                                                    Keywords = @"\b(var|for|if|else|return|this|while|new|function|switch|case|typeof|do|in|throw|try|catch|finally|with|instance|delete|void|break|continue)\b",//匹配关键词，关键词顺序改了下
                                                    IsRegex = true,
                                                    StyleId = 2
                                                },
                                                new KeywordsStyle
                                                {
                                                    Keywords = @"(\/\/.*|\/\*[\s\S]*?\*\/)",// 匹配注释
                                                    IsRegex =true,
                                                    StyleId = 0
                                                },
                                                new KeywordsStyle
                                                {
                                                    Keywords = @"(""(?:[^""\\]|\\[\s\S])*""|'(?:[^'\\]|\\[\s\S])*')",// 匹配字符串
                                                    IsRegex=true,
                                                    StyleId = 1
                                                },//new KeywordsStyle
                                                //{
                                                //    Keywords = @"(?:[^\W\d]|\$)[\$\w]*",// 匹配普通的变量名
                                                //    IsRegex =true,
                                                //    StyleId = 4
                                                //},
                                                //new KeywordsStyle
                                                //{
                                                //    Keywords = @"(0[xX][0-9a-fA-F]+|\d+(?:\.\d+)?(?:[eE][+-]?\d+)?|\.\d+(?:[eE][+-]?\d+)?)",// 匹配数字，修复了匹配
                                                //    IsRegex =true,
                                                //    StyleId = -1
                                                //},
                                                //new KeywordsStyle
                                                //{
                                                //    Keywords = @"(?:^|[^\)\]\}])(\/(?!\*)(?:\\.|[^\\\/\n])+?\/[gim]*)",//匹配正则
                                                //    IsRegex =true,
                                                //    StyleId = -1
                                                //},
                                            }
                                        }
                                    },
                                },
                            },
                            new TabItemTemplate
                            {
                                Header="多级绑定",
                                Content=new Panel
                                {
                                    Name = "多级绑定",
                                    PresenterFor = this,
                                    Width="100%",
                                    Height="100%",
                                    Children=
                                    {
                                        new StackPanel
                                        {
                                            MarginLeft=10,
                                            MarginTop=10,
                                            Orientation= Orientation.Vertical,
                                            Children=
                                            {
                                                new TextBlock
                                                {
                                                    [nameof(TextBlock.Text)]= new Obx<MainModel>(a => a.Test1.test.test.test.test.Name,
                                                    BindingMode.OneWay),
                                                    Name = "hmbb"
                                                },//new TextBox
                                                //{
                                                //    Width = 130,
                                                //    Height= 60,
                                                //    Background =Color.Gray,
                                                //    [nameof(TextBox.Text)]= new Obx<MainModel>(a => a.Test1.test.test.test.test.Name,
                                                //    BindingMode.OneWayToSource),
                                                //},
                                                new Button
                                                {
                                                    Content="创建对象",
                                                    [nameof(Button.Click)]=new CommandDescribe((s,e)=>
                                                    {
                                                        data a = new data();
                                                        a.test.test.test.Name = "666666";
                                                        (DataContext as MainModel).Test1.test = a;
                                                    })
                                                },
                                                new Button
                                                {
                                                    Content="删除对象",
                                                    [nameof(Button.Click)]=new CommandDescribe((s,e)=>
                                                    {
                                                        (DataContext as MainModel).Test1.test.test = null;
                                                    })
                                                },
                                                new Button
                                                {
                                                    Content="添加对象",
                                                    [nameof(Button.Click)]=new CommandDescribe((s,e)=>
                                                    {
                                                        data a = new data();
                                                        a.test.test.Name = "8888";
                                                        (DataContext as MainModel).Test1.test.test = a;
                                                    })
                                                },
                                            }
                                        },
                                    }
                                }
                            },
                            new TabItemTemplate
                            {
                                Header="布局",
                                Content=new Panel
                                {
                                    Name = "布局",
                                    PresenterFor = this,
                                    Width="100%",
                                    Height="100%",
                                    Children=
                                    {
                                        new StackPanel
                                        {
                                            MarginLeft=10,
                                            MarginTop=10,
                                            Orientation= Orientation.Vertical,
                                            Children=
                                            {
                                                new Button
                                                {
                                                    Content="StackPanel的Vertical"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                            }
                                        },
                                        new StackPanel
                                        {
                                            BorderStroke = "5,Solid",
                                            BorderFill = "#B4B4B4",
                                            MarginLeft=80,
                                            MarginTop=50,
                                            Orientation= Orientation.Horizontal,
                                            Children=
                                            {
                                                new Button
                                                {
                                                    Content="StackPanel的Horizontal"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="Margin调间距",
                                                    MarginLeft=5
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                            }
                                        },
                                        new WrapPanel
                                        {
                                            MarginRight=10,
                                            MarginTop=10,
                                            Width="50%",
                                            Orientation= Orientation.Horizontal,
                                            Children=
                                            {
                                                new Button
                                                {
                                                    Content="WrapPanel的Horizontal"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="Margin调间距",
                                                    MarginLeft=5
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="宽度不够"
                                                },
                                                new Button
                                                {
                                                    Content="可以自动换行"
                                                },
                                            }
                                        },
                                        new Grid
                                        {
                                            RenderTransform=new RotateTransform(10),
                                            Name="testGrid",
                                            Background="#999",
                                            Width="80%",
                                            Height="60%",
                                            MarginTop=120,
                                            MarginLeft=20,
                                            ColumnDefinitions=
                                            {
                                                new ColumnDefinition
                                                {
                                                    Width="40*"
                                                },
                                                new ColumnDefinition
                                                {
                                                    Width = "30*"
                                                },
                                                new ColumnDefinition
                                                {
                                                    Width="200",
                                                    [nameof(ColumnDefinition.Width)]=nameof(MainModel.ColumnWidth)
                                                },
                                            },
                                            RowDefinitions=
                                            {
                                                new RowDefinition
                                                {
                                                    Height="30*"
                                                },
                                                new RowDefinition
                                                {
                                                    Height="30*"
                                                },
                                                new RowDefinition
                                                {
                                                    Height="30*"
                                                }
                                            },
                                            Children=
                                            {
                                                new WrapPanel
                                                {
                                                    Name="test",
                                                    Background="#a2f",
                                                    Width="100%",
                                                    Height="100%",
                                                    Children=
                                                    {
                                                        new Button
                                                        {
                                                            Content="水平浮动布局231"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮2"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮3"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮4"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮5"
                                                        },
                                                    }
                                                },
                                                {
                                                    new WrapPanel
                                                    {
                                                        Orientation= Orientation.Vertical,
                                                        Background="#27a",
                                                        Width="100%",
                                                        Height="100%",
                                                        Children=
                                                        {
                                                            new Button
                                                            {
                                                                Content="垂直浮动布局"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮2"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮3"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮4"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮5"
                                                            },
                                                        }
                                                    },
                                                    1,
                                                    1
                                                },
                                                {
                                                    new TextBlock
                                                    {
                                                        Background="#ac2",
                                                        Width="100%",
                                                        Height="100%",
                                                        Text="Grid布局。。。"
                                                    },
                                                    2,
                                                    1
                                                },
                                                {
                                                    new Panel
                                                    {
                                                        Background="#b1a",
                                                        MarginLeft=0,
                                                        MarginRight=0,
                                                        Children=
                                                        {
                                                            new Button
                                                            {
                                                                Content="跨列",
                                                                Width="50%"
                                                            }
                                                        }
                                                    },
                                                    0,
                                                    2,
                                                    2
                                                },
                                                {
                                                    new TextBlock
                                                    {
                                                        Background="#186",
                                                        Height="100%",
                                                        Text="跨行"
                                                    },
                                                    2,
                                                    1,
                                                    1,
                                                    2
                                                },
                                                new TextBox
                                                {
                                                    MarginLeft=10,
                                                    Size=SizeField.Fill,
                                                    Text="元素变换，可以旋转，倾斜，缩放等操作",
                                                    Attacheds=
                                                    {
                                                        {
                                                            Grid.ColumnIndex,
                                                            1
                                                        }
                                                    }
                                                },
                                                new Button
                                                {
                                                    Content=new SVG("res://ConsoleApp1/test.svg")
                                                    {
                                                        MarginLeft = 0,
                                                        MarginTop = 0,
                                                        Height = 85,
                                                        Width=170,
                                                        Stretch= Stretch.Uniform,
                                                    },
                                                    Width=104,
                                                    Height=55,
                                                    MarginLeft=60,
                                                    MarginTop=120,
                                                    Commands=
                                                    {
                                                        {
                                                            nameof(Button.Click),
                                                            (s,e)=> Animation((Button)s)
                                                        }
                                                    }
                                                }
                                            },
                                        },
                                        new DockPanel
                                        {
                                            LastChildFill = false,
                                            Width=200,
                                            Height=200,
                                            MarginRight=0,
                                            MarginTop=50,
                                            Background="#f00",
                                            Children =
                                            {
                                                new Button
                                                {
                                                    Content="Right",
                                                    Height="100%",
                                                    Attacheds =
                                                    {
                                                        {
                                                            DockPanel.Dock,
                                                            Dock.Right
                                                        }
                                                    }
                                                },
                                            }
                                        },
                                        new Slider
                                        {
                                            Maximum = 300,
                                            Value = 200,
                                            MarginLeft = 252,
                                            MarginTop = 76,
                                            Height = 23,
                                            Width = 219,
                                            [nameof(Slider.Value)]=new BindingDescribe(null, nameof(MainModel.ColumnWidth),BindingMode.OneWayToSource,null,a=>new GridLength((float)(double)a))
                                        },
                                    }
                                }
                            },
                            new TabItemTemplate
                            {
                                Header="布局",
                                Content=new Panel
                                {
                                    Name = "布局",
                                    PresenterFor = this,
                                    Width="100%",
                                    Height="100%",
                                    Children=
                                    {
                                        new StackPanel
                                        {
                                            MarginLeft=10,
                                            MarginTop=10,
                                            Orientation= Orientation.Vertical,
                                            Children=
                                            {
                                                new Button
                                                {
                                                    Content="StackPanel的Vertical"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                            }
                                        },
                                        new StackPanel
                                        {
                                            BorderStroke = "5,Solid",
                                            BorderFill = "#B4B4B4",
                                            MarginLeft=80,
                                            MarginTop=50,
                                            Orientation= Orientation.Horizontal,
                                            Children=
                                            {
                                                new Button
                                                {
                                                    Content="StackPanel的Horizontal"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="Margin调间距",
                                                    MarginLeft=5
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                            }
                                        },
                                        new WrapPanel
                                        {
                                            MarginRight=10,
                                            MarginTop=10,
                                            Width="50%",
                                            Orientation= Orientation.Horizontal,
                                            Children=
                                            {
                                                new Button
                                                {
                                                    Content="WrapPanel的Horizontal"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="Margin调间距",
                                                    MarginLeft=5
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="宽度不够"
                                                },
                                                new Button
                                                {
                                                    Content="可以自动换行"
                                                },
                                            }
                                        },
                                        new Grid
                                        {
                                            RenderTransform=new RotateTransform(10),
                                            Name="testGrid",
                                            Background="#999",
                                            Width="80%",
                                            Height="60%",
                                            MarginTop=120,
                                            MarginLeft=20,
                                            ColumnDefinitions=
                                            {
                                                new ColumnDefinition
                                                {
                                                    Width="40*"
                                                },
                                                new ColumnDefinition
                                                {
                                                    Width = "30*"
                                                },
                                                new ColumnDefinition
                                                {
                                                    Width="200",
                                                    [nameof(ColumnDefinition.Width)]=nameof(MainModel.ColumnWidth)
                                                },
                                            },
                                            RowDefinitions=
                                            {
                                                new RowDefinition
                                                {
                                                    Height="30*"
                                                },
                                                new RowDefinition
                                                {
                                                    Height="30*"
                                                },
                                                new RowDefinition
                                                {
                                                    Height="30*"
                                                }
                                            },
                                            Children=
                                            {
                                                new WrapPanel
                                                {
                                                    Name="test",
                                                    Background="#a2f",
                                                    Width="100%",
                                                    Height="100%",
                                                    Children=
                                                    {
                                                        new Button
                                                        {
                                                            Content="水平浮动布局231"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮2"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮3"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮4"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮5"
                                                        },
                                                    }
                                                },
                                                {
                                                    new WrapPanel
                                                    {
                                                        Orientation= Orientation.Vertical,
                                                        Background="#27a",
                                                        Width="100%",
                                                        Height="100%",
                                                        Children=
                                                        {
                                                            new Button
                                                            {
                                                                Content="垂直浮动布局"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮2"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮3"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮4"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮5"
                                                            },
                                                        }
                                                    },
                                                    1,
                                                    1
                                                },
                                                {
                                                    new TextBlock
                                                    {
                                                        Background="#ac2",
                                                        Width="100%",
                                                        Height="100%",
                                                        Text="Grid布局。。。"
                                                    },
                                                    2,
                                                    1
                                                },
                                                {
                                                    new Panel
                                                    {
                                                        Background="#b1a",
                                                        MarginLeft=0,
                                                        MarginRight=0,
                                                        Children=
                                                        {
                                                            new Button
                                                            {
                                                                Content="跨列",
                                                                Width="50%"
                                                            }
                                                        }
                                                    },
                                                    0,
                                                    2,
                                                    2
                                                },
                                                {
                                                    new TextBlock
                                                    {
                                                        Background="#186",
                                                        Height="100%",
                                                        Text="跨行"
                                                    },
                                                    2,
                                                    1,
                                                    1,
                                                    2
                                                },
                                                new TextBox
                                                {
                                                    MarginLeft=10,
                                                    Size=SizeField.Fill,
                                                    Text="元素变换，可以旋转，倾斜，缩放等操作",
                                                    Attacheds=
                                                    {
                                                        {
                                                            Grid.ColumnIndex,
                                                            1
                                                        }
                                                    }
                                                },
                                                new Button
                                                {
                                                    Content=new SVG("res://ConsoleApp1/test.svg")
                                                    {
                                                        MarginLeft = 0,
                                                        MarginTop = 0,
                                                        Height = 85,
                                                        Width=170,
                                                        Stretch= Stretch.Uniform,
                                                    },
                                                    Width=104,
                                                    Height=55,
                                                    MarginLeft=60,
                                                    MarginTop=120,
                                                    Commands=
                                                    {
                                                        {
                                                            nameof(Button.Click),
                                                            (s,e)=> Animation((Button)s)
                                                        }
                                                    }
                                                }
                                            },
                                        },
                                        new DockPanel
                                        {
                                            LastChildFill = false,
                                            Width=200,
                                            Height=200,
                                            MarginRight=0,
                                            MarginTop=50,
                                            Background="#f00",
                                            Children =
                                            {
                                                new Button
                                                {
                                                    Content="Right",
                                                    Height="100%",
                                                    Attacheds =
                                                    {
                                                        {
                                                            DockPanel.Dock,
                                                            Dock.Right
                                                        }
                                                    }
                                                },
                                            }
                                        },
                                        new Slider
                                        {
                                            Maximum = 300,
                                            Value = 200,
                                            MarginLeft = 252,
                                            MarginTop = 76,
                                            Height = 23,
                                            Width = 219,
                                            [nameof(Slider.Value)]=new BindingDescribe(null, nameof(MainModel.ColumnWidth),BindingMode.OneWayToSource,null,a=>new GridLength((float)(double)a))
                                        },
                                    }
                                }
                            },
                            new TabItemTemplate
                            {
                                Header="布局",
                                Content=new Panel
                                {
                                    Name = "布局",
                                    PresenterFor = this,
                                    Width="100%",
                                    Height="100%",
                                    Children=
                                    {
                                        new StackPanel
                                        {
                                            MarginLeft=10,
                                            MarginTop=10,
                                            Orientation= Orientation.Vertical,
                                            Children=
                                            {
                                                new Button
                                                {
                                                    Content="StackPanel的Vertical"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                            }
                                        },
                                        new StackPanel
                                        {
                                            BorderStroke = "5,Solid",
                                            BorderFill = "#B4B4B4",
                                            MarginLeft=80,
                                            MarginTop=50,
                                            Orientation= Orientation.Horizontal,
                                            Children=
                                            {
                                                new Button
                                                {
                                                    Content="StackPanel的Horizontal"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="Margin调间距",
                                                    MarginLeft=5
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                            }
                                        },
                                        new WrapPanel
                                        {
                                            MarginRight=10,
                                            MarginTop=10,
                                            Width="50%",
                                            Orientation= Orientation.Horizontal,
                                            Children=
                                            {
                                                new Button
                                                {
                                                    Content="WrapPanel的Horizontal"
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="Margin调间距",
                                                    MarginLeft=5
                                                },
                                                new Button
                                                {
                                                    Content="按钮"
                                                },
                                                new Button
                                                {
                                                    Content="宽度不够"
                                                },
                                                new Button
                                                {
                                                    Content="可以自动换行"
                                                },
                                            }
                                        },
                                        new Grid
                                        {
                                            RenderTransform=new RotateTransform(10),
                                            Name="testGrid",
                                            Background="#999",
                                            Width="80%",
                                            Height="60%",
                                            MarginTop=120,
                                            MarginLeft=20,
                                            ColumnDefinitions=
                                            {
                                                new ColumnDefinition
                                                {
                                                    Width="40*"
                                                },
                                                new ColumnDefinition
                                                {
                                                    Width = "30*"
                                                },
                                                new ColumnDefinition
                                                {
                                                    Width="200",
                                                    [nameof(ColumnDefinition.Width)]=nameof(MainModel.ColumnWidth)
                                                },
                                            },
                                            RowDefinitions=
                                            {
                                                new RowDefinition
                                                {
                                                    Height="30*"
                                                },
                                                new RowDefinition
                                                {
                                                    Height="30*"
                                                },
                                                new RowDefinition
                                                {
                                                    Height="30*"
                                                }
                                            },
                                            Children=
                                            {
                                                new WrapPanel
                                                {
                                                    Name="test",
                                                    Background="#a2f",
                                                    Width="100%",
                                                    Height="100%",
                                                    Children=
                                                    {
                                                        new Button
                                                        {
                                                            Content="水平浮动布局231"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮2"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮3"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮4"
                                                        },
                                                        new Button
                                                        {
                                                            Content="按钮5"
                                                        },
                                                    }
                                                },
                                                {
                                                    new WrapPanel
                                                    {
                                                        Orientation= Orientation.Vertical,
                                                        Background="#27a",
                                                        Width="100%",
                                                        Height="100%",
                                                        Children=
                                                        {
                                                            new Button
                                                            {
                                                                Content="垂直浮动布局"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮2"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮3"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮4"
                                                            },
                                                            new Button
                                                            {
                                                                Content="按钮5"
                                                            },
                                                        }
                                                    },
                                                    1,
                                                    1
                                                },
                                                {
                                                    new TextBlock
                                                    {
                                                        Background="#ac2",
                                                        Width="100%",
                                                        Height="100%",
                                                        Text="Grid布局。。。"
                                                    },
                                                    2,
                                                    1
                                                },
                                                {
                                                    new Panel
                                                    {
                                                        Background="#b1a",
                                                        MarginLeft=0,
                                                        MarginRight=0,
                                                        Children=
                                                        {
                                                            new Button
                                                            {
                                                                Content="跨列",
                                                                Width="50%"
                                                            }
                                                        }
                                                    },
                                                    0,
                                                    2,
                                                    2
                                                },
                                                {
                                                    new TextBlock
                                                    {
                                                        Background="#186",
                                                        Height="100%",
                                                        Text="跨行"
                                                    },
                                                    2,
                                                    1,
                                                    1,
                                                    2
                                                },
                                                new TextBox
                                                {
                                                    MarginLeft=10,
                                                    Size=SizeField.Fill,
                                                    Text="元素变换，可以旋转，倾斜，缩放等操作",
                                                    Attacheds=
                                                    {
                                                        {
                                                            Grid.ColumnIndex,
                                                            1
                                                        }
                                                    }
                                                },
                                                new Button
                                                {
                                                    Content=new SVG("res://ConsoleApp1/test.svg")
                                                    {
                                                        MarginLeft = 0,
                                                        MarginTop = 0,
                                                        Height = 85,
                                                        Width=170,
                                                        Stretch= Stretch.Uniform,
                                                    },
                                                    Width=104,
                                                    Height=55,
                                                    MarginLeft=60,
                                                    MarginTop=120,
                                                    Commands=
                                                    {
                                                        {
                                                            nameof(Button.Click),
                                                            (s,e)=> Animation((Button)s)
                                                        }
                                                    }
                                                }
                                            },
                                        },
                                        new DockPanel
                                        {
                                            LastChildFill = false,
                                            Width=200,
                                            Height=200,
                                            MarginRight=0,
                                            MarginTop=50,
                                            Background="#f00",
                                            Children =
                                            {
                                                new Button
                                                {
                                                    Content="Right",
                                                    Height="100%",//Attacheds =
                                                    //{
                                                    //    {
                                                    //        DockPanel.Dock,
                                                    //        Dock.Right
                                                    //    }
                                                    //},
                                                    [DockPanel.Dock] = Dock.Right,
                                                }
                                                .Assign(out var testBtn),
                                            }
                                        },
                                        new Slider
                                        {
                                            Maximum = 300,
                                            Value = 200,
                                            MarginLeft = 252,
                                            MarginTop = 76,
                                            Height = 23,
                                            Width = 219,
                                            [nameof(Slider.Value)]=new BindingDescribe(null, nameof(MainModel.ColumnWidth),BindingMode.OneWayToSource,null,a=>new GridLength((float)(double)a))
                                        },
                                    }
                                }
                            }
                        },//SelectedIndex=1
                    }
                }
            })
            {
                MaximizeBox = true,
                ShadowBlur = 10,
                #if !DesignMode
                //Effect = effect
#endif
            });
            //#if !Net4 && !NETCOREAPP3_0
//            Children.Add(new GLView
//            {
//                Height = "30%",
//                Width = "30%",
//                IsAntiAlias = true,
//            });
//#endif
            LoadStyleFile("res://ConsoleApp1/Stylesheet3.css");
            //加载样式文件，文件需要设置为内嵌资源
            Console.WriteLine(testBtn[DockPanel.Dock]);
            if (!DesignMode)//设计模式下不执行
            {
                //var type = Grid.ColumnIndex.GetType();
                //var f = typeof(Grid).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                //var attached = f[0].GetValue(null, null);
                //var pt = typeof(CPF.OptionalParameter<>).MakeGenericType(f[0].PropertyType.GetGenericArguments());
                //var v = (attached as Delegate).DynamicInvoke(this, Activator.CreateInstance(pt));
                //var p = typeof(Grid).GetProperty("Grid.RowIndex", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            }
        }
        CPF.Controls.Switch _sw;
        DataGrid testDataGrid;
        DataGrid testGrid;
        TreeView testTreeVIew;
        Button btn;
        Panel page1;
        ComboBox combox;
        ComboBox testCombobox;
        Picture pic;
        StackPanel stackPanel;
        TextBox textBox;

        public float TestValue
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }

        //PathGeometry pathGeometry = "M159.375 196.875A9.375 9.375 0 0 1 140.625 196.875V133.8375L113.83125 89.2125A9.375 9.375 0 1 1 129.91875 79.55625L158.04375 126.43125A9.375 9.375 0 0 1 159.375 131.25V196.875zM121.875 300A9.375 9.375 0 0 1 121.875 281.25H131.25V261.1875A131.26875 131.26875 0 0 1 69.88125 27.3L58.59375 16.0125A9.375 9.375 0 0 1 71.85 2.7375L85.8375 16.725A130.6875 130.6875 0 0 1 150 0A130.6875 130.6875 0 0 1 214.1625 16.725L228.15 2.7375A9.375 9.375 0 0 1 241.4062500000001 16.0125L230.1375 27.3A131.26875 131.26875 0 0 1 168.75 261.1875V281.25H178.125A9.375 9.375 0 0 1 178.125 300H121.875zM141.3375 243.4125A114.24375 114.24375 0 0 0 158.6625 243.4125A112.5 112.5 0 1 0 141.3375 243.4125zM0 234.375C0 220.25625 6.24375 207.58125 16.125 198.99375A150.65625 150.65625 0 0 0 82.25625 265.125A46.875 46.875 0 0 1 0 234.375zM253.125 281.25C239.00625 281.25 226.33125 275.00625 217.74375 265.125A150.65625 150.65625 0 0 0 283.875 198.99375A46.875 46.875 0 0 1 253.125 281.25z";
        //Stopwatch stopwatch = new Stopwatch();
        //int frameCount;
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            //frameCount++;
            //if (stopwatch.ElapsedMilliseconds >= 500)
            //{
            //    Debug.WriteLine(frameCount * 1000 / stopwatch.ElapsedMilliseconds);
            //    stopwatch.Restart();
            //    frameCount = 0;
            //}
            //else if (!stopwatch.IsRunning)
            //{
            //    stopwatch.Start();
            //}
            //dc.DrawPath("#f00", new Stroke(1), pathGeometry);
        }

        protected override void OnElementInitialize(UIElement element)
        {
            if (element is Button button)
            {
                button.SetTemplate((s, c) =>
                {
                    c.Add(new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new Ellipse { Width = 15, Height = 15,IsAntiAlias=true,Fill="#f00" },
                            new Border
                            {
                                Name = "contentPresenter",
                                Height = "100%",
                                BorderFill = null,
                                PresenterFor = s,
                            },
                        }
                    });
                });
            }
            if (element.Tag == null)
            {
                element.Tag = 1;
            }
            else
            {
                element.Tag = 1 + (int)element.Tag;
            }
            base.OnElementInitialize(element);
        }

        [PropertyMetadata(-1)]
        public int SelectIndex
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public Collection<TestClass> List1
        {
            get { return GetValue<Collection<TestClass>>(); }
            set { SetValue(value); }
        }
        public Collection<TestClass> List2
        {
            get { return GetValue<Collection<TestClass>>(); }
            set { SetValue(value); }
        }

        void test()
        {
            //var panel = this.FindPresenter<StackPanel>().First(a => a.Name == "testPanel");
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //panel.Children.DisposeChildren();
            //Debug.WriteLine("清除：" + stopwatch.ElapsedMilliseconds);
            //System.Threading.Thread.Sleep(1000);
            //stopwatch.Restart();
            //for (int i = 0; i < 1000; i++)
            //{
            //    panel.Children.Add(new Button { Content = "test测试", Width = 100, Height = 50 });
            //}
            //stopwatch.Stop();
            //Debug.WriteLine("添加：" + stopwatch.ElapsedMilliseconds);
        }

        protected async override void OnInitialized()
        {
            //testTreeVIew.Items = Nodes;
            //(DataContext as MainModel).TestItems1 = new Collection<string>();
            //for (int i = 0; i < 10; i++)
            //{
            //    (DataContext as MainModel).TestItems1.Add(i.ToString());
            //}

            _sw = FindPresenterByName<CPF.Controls.Switch>(nameof(_sw));
            _sw.IsChecked = true;
            base.OnInitialized();
            Items = new Collection<ItemData>();
            for (int i = 0; i < 100; i++)
            {
                Items.Add(new ItemData { Name = "马大云" + i, Introduce = "哈哈---" + i });
            }
            //Columns.Add(new DataGridTextColumn { Header = "任务名称", Width = "60*", CanUserSort = false });
            //Columns.Add(new DataGridTextColumn { Header = "", Width = "20*", CanUserSort = false });
            CPF.Styling.ResourceManager.GetImage("res://ConsoleApp1/Resources/主页.png", a =>
            {
                var data = new DataTable();
                for (int i = 0; i < 9; i++)
                {
                    data.Columns.Add("p" + (i + 1).ToString());
                }
                data.Columns[1].DataType = typeof(bool);
                data.Columns[3].DataType = typeof(int);
                data.Columns[5].DataType = typeof(Image);
                data.Columns[7].DataType = typeof(Button);
                for (int i = 0; i < 180; i++)
                {
                    var row = data.NewRow();
                    for (int j = 0; j < 9; j++)
                    {
                        if (j != 1)
                        {
                            if (j == 5)
                            {
                                row[j] = a;
                            }
                            else if (j == 7)
                            {
                                row[7] = new Button { Content = "test" + i, Width = "100%" };
                            }
                            else
                            {
                                row[j] = i;
                            }
                        }
                    }
                    row[0] = i % 3;
                    row[1] = true;

                    data.Rows.Add(row);
                }
                Data = data.ToItems();
            });
            Icon = await CPF.Styling.ResourceManager.GetImage("res://ConsoleApp1/Resources/icon.png");
            textBox = FindPresenterByName<TextBox>(nameof(textBox));
            stackPanel = FindPresenterByName<StackPanel>(nameof(stackPanel));


            //for (int i = 0; i < 50000; i++)
            //{
            //    stackPanel.Children.Add(new Button { Content="test"}); 
            //}
            pic = FindPresenterByName<Picture>(nameof(pic));
            btn = FindPresenterByName<Button>(nameof(btn));

            //textBox.Background = new VisualFill(btn);

#if !Net4
            //var m = Observable.FromEventPattern<MouseButtonEventArgs>(this, nameof(MouseDown));
            //var m = this.Observe<Window, MouseButtonEventArgs>(nameof(MouseDown));
            //m.Subscribe(args =>
            //{
            //    Debug.WriteLine(args.EventArgs.MouseButton);
            //});
#endif

            //var newValueParameter = Expression.Parameter(typeof(string), "value");
            ////var instanceParameter = Expression.Parameter(Type, "instance");
            //var method = Type.GetMethod(nameof(TestSetValue), BindingFlags.Instance | BindingFlags.NonPublic);
            //var methodCall = Expression.Call(Expression.Constant(this), method, newValueParameter, Expression.Constant((byte)1));
            //var lambda = Expression.Lambda<Action<string>>(methodCall, newValueParameter);

            //var tpSet = Type.GetProperty(nameof(TestProperty)).GetSetMethod();
            ////var tpSet = Type.GetMethod(nameof(TestSet1), BindingFlags.Static | BindingFlags.NonPublic);
            ////var set = Type.GetMethod(nameof(TestSet), BindingFlags.Instance | BindingFlags.NonPublic);
            //var action = lambda.Compile();
            //var set = action.Method;
            ////var h = GetDynamicMethodRuntimeHandle(set);

            //var setValue = Type.GetMethod(nameof(TestSetValue), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { typeof(object), typeof(byte).Assembly.GetType("System.Byte&") }, null);





            ////因为你是作为Instance 方法,那么对于DynamicMethod 第一个参数就应该是This
            //DynamicMethod mt = new DynamicMethod(string.Empty, typeof(void), new Type[] { typeof(object), typeof(string) });
            //var il = mt.GetILGenerator();

            //LocalBuilder loc = il.DeclareLocal(typeof(byte), true);
            //il.Emit(OpCodes.Ldc_I4, 83);
            //il.Emit(OpCodes.Conv_Ovf_U1);
            //il.Emit(OpCodes.Stloc, loc.LocalIndex);
            ////SetValue(Value, PropertyIndex);
            ////Stack
            ////CpfObject This
            ////Value
            ////Index ref
            //il.Emit(OpCodes.Ldarg_0);
            //il.Emit(OpCodes.Ldarg_1);
            //il.Emit(OpCodes.Ldloca, loc.LocalIndex);

            //il.Emit(OpCodes.Callvirt, setValue);
            //if (setValue.ReturnType != typeof(void))
            //    il.Emit(OpCodes.Pop);
            //il.Emit(OpCodes.Ret);

            //var ha = mt.CreateDelegate(typeof(Action<string>), this);
            ////((Action<string>)ha)("231");
            ////var m_scope = il.GetType().GetField("m_scope", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(il);
            ////var m_tokens = m_scope.GetType().GetField("m_tokens", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(m_scope);
            ////RuntimeMethodHandle handle = (RuntimeMethodHandle)(m_tokens as IList)[2];
            //var h = GetDynamicMethodRuntimeHandle(mt);
            //RedirectTo(tpSet, h);
            //TestProperty = "test";
            //TestSet1("test");


            testCombobox = FindPresenterByName<ComboBox>(nameof(testCombobox));

            //for (int i = 0; i < 300; i++)
            //{
            //    testCombobox.Items.Add((i.ToString(), i));
            //}
            combox = FindPresenterByName<ComboBox>(nameof(combox));
            page1 = FindPresenterByName<Panel>(nameof(page1));

            //for (int i = 0; i < 10; i++)
            //{
            //    combox.Items.Add(i.ToString());
            //}

#if !Net4
            if (!DesignMode)
            {
                effect.TransitionValue(nameof(ThreeDEffect2.Y), 0f, TimeSpan.FromSeconds(0.6), null, AnimateMode.Linear, () =>
                {
                    Find<WindowFrame>().First().Effect = null;
                });
            }

#endif
            testTreeVIew = FindPresenterByName<TreeView>(nameof(testTreeVIew));
            //WindowState = WindowState.Maximized;
            testGrid = FindPresenterByName<DataGrid>(nameof(testGrid));
            testDataGrid = FindPresenterByName<DataGrid>(nameof(testDataGrid));
            textBox.Document.Children[2].StyleId = 0;

#if !DesignMode && !Net4
            //PdfDocument doc = PdfReader.Open(System.IO.Path.Combine(CPF.Platform.Application.StartupPath, "test.pdf"), PdfDocumentOpenMode.ReadOnly);
            //foreach (var page in doc.Pages)
            //{
            //    foreach (var item in Find(page))
            //    {
            //        Debug.WriteLine(item.Value.GetType());
            //        if (item.Value is PdfSharpCore.Pdf.Advanced.PdfReference reff)
            //        {

            //        }
            //        else if (item.Value is PdfSharpCore.Pdf.PdfDictionary dic)
            //        {

            //        }
            //    }
            //}

#endif
        }

        //private void ExistValue(string strName, string strFieldValue)
        //{
        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    if (dic.ContainsKey(strName))
        //        dic[strName] = strFieldValue;
        //}

        async void PDF(CpfObject obj, RoutedEventArgs eventArgs)
        {
#if !DesignMode && !Net4

            Debug.WriteLine((obj as Button).PointToView(new Point()).ToString());
            //var file = await new BrowseFileWindow { Title="文件选择" }.ShowDialog();
            //if (file is string str)
            //{
            //    Console.WriteLine(str);
            //}
            //else
            //{
            //    Console.WriteLine("未选中");
            //}
            //System.Diagnostics.Process p = new System.Diagnostics.Process();
            //p.StartInfo.FileName = "cmd.exe";
            //p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            //p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            //p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            //p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            ////p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            //p.Start();//启动程序

            ////向cmd窗口发送输入信息
            //p.StandardInput.WriteLine("dotnet \"D:\\xhm\\Documents\\Visual Studio 2019\\ConsoleApp1\\ConsoleApp1\\bin\\Debug\\netcoreapp3.0\\ConsoleApp1.dll\"");

            //p.StandardInput.AutoFlush = true;

            //Process.Start(new ProcessStartInfo
            //{
            //    UseShellExecute = true,
            //    FileName = "D:/xhm/Documents/Visual Studio 2019/ConsoleApp1/ConsoleApp1/bin/Debug/netcoreapp3.0/test.pdf"
            //});
            //using (var dialog = new OpenFileDialog { Title = "打开PDF", Filters = { new FileDialogFilter { Extensions = "pdf", Name = "PDF" } } })
            //{
            //    var file = await dialog.ShowAsync(this);
            //    if (file != null && file.Length > 0)
            //    {
            //        using (PdfDocument document = PdfDocument.Open(file[0]))
            //        {
            //            //foreach (var page in document.GetPages())
            //            var page = document.GetPage(1);
            //            {
            //                using (var bmp = new Bitmap((int)page.Width, (int)page.Height))
            //                {
            //                    using (var dc = DrawingContext.FromBitmap(bmp))
            //                    {
            //                        dc.Clear(Color.White);
            //                        dc.AntialiasMode = AntialiasMode.AntiAlias;
            //                        using (var sb = new SolidColorBrush("#000"))
            //                        {
            //                            //string pageText = page.Text;

            //                            foreach (var item in page.Letters)
            //                            {
            //                                dc.DrawString(new Point((float)item.Location.X, (float)page.Height - (float)item.Location.Y), sb, item.Value, new Font(item.FontName, (float)item.PointSize));
            //                            }
            //                        }
            //                    }
            //                    using (var stream = System.IO.File.OpenWrite(System.IO.Path.Combine(CPF.Platform.Application.StartupPath, "test.png")))
            //                    {
            //                        bmp.SaveToStream(ImageFormat.Png, stream);
            //                    }

            //                    //Debug.WriteLine(pageText);
            //                    //foreach (var word in page.GetWords())
            //                    //{
            //                    //    Console.WriteLine(word.Text);
            //                    //}
            //                }
            //            }
            //        }
            //    }
            //}

            //        using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={System.IO.Path.Combine(CPF.Platform.Application.StartupPath, "hello.db")}"))
            //        {
            //            connection.Open();
            //            //var dataAdapter = new SqliteDataAdapter("",connection);

            ////            var command = connection.CreateCommand();
            ////            command.CommandText =
            ////            @"
            ////    SELECT name
            ////    FROM user
            ////    WHERE id = $id
            ////";
            ////            command.Parameters.AddWithValue("$id", id);

            ////            using (var reader = command.ExecuteReader())
            ////            {
            ////                while (reader.Read())
            ////                {
            ////                    var name = reader.GetString(0);

            ////                    Console.WriteLine($"Hello, {name}!");
            ////                }
            ////            }
            //        }
#endif
        }
        bool TestSetValue(object value, in byte index)
        {
            MessageBox.Show(index + value.ToString());
            return true;
        }
        void TestSet(string text)
        {
            MessageBox.Show(text);
            //throw new Exception(text);
            //Debug.WriteLine(text);
        }
        static void TestSet1(string text)
        {
            Debug.WriteLine(text + "|test");
        }

        public string TestProperty
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        //public string TestProperty1
        //{
        //    get { return (string)GetValue(1); }
        //    set { SetValue(value, 1); }
        //}
        private static IntPtr GetDynamicMethodRuntimeHandle(MethodBase method)
        {
            if (!(method is DynamicMethod))
                return method.MethodHandle.Value;
            var fieldInfo = typeof(DynamicMethod).GetField("m_methodHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                var m_methodHandle = fieldInfo.GetValue(method);
                if (m_methodHandle != null)
                {
                    var value = m_methodHandle.GetType().GetField("m_value", BindingFlags.Public | BindingFlags.Instance).GetValue(m_methodHandle);
                    return (IntPtr)value.GetType().GetField("m_handle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(value);
                }
            }
            return method.MethodHandle.Value;
        }
        public static void RedirectTo(MethodInfo origin, IntPtr newMethod)
        {
            IntPtr ori = GetMethodAddress(origin);
            //IntPtr tar = GetMethodAddress(newMethod);

            Marshal.Copy(new IntPtr[] { Marshal.ReadIntPtr(newMethod) }, 0, ori, 1);
        }

        private static IntPtr GetMethodAddress(MethodInfo mi)
        {
            const ushort SLOT_NUMBER_MASK = 0xfff; // 3 bytes
            const int MT_OFFSET_32BIT = 0x28;      // 40 bytes
            const int MT_OFFSET_64BIT = 0x40;      // 64 bytes

            IntPtr address;

            // JIT compilation of the method
            RuntimeHelpers.PrepareMethod(mi.MethodHandle);

            IntPtr md = mi.MethodHandle.Value;             // MethodDescriptor address
            IntPtr mt = mi.DeclaringType.TypeHandle.Value; // MethodTable address
            if (mi.IsVirtual)
            {
                // The fixed-size portion of the MethodTable structure depends on the process type
                int offset = IntPtr.Size == 4 ? MT_OFFSET_32BIT : MT_OFFSET_64BIT;

                // First method slot = MethodTable address + fixed-size offset
                // This is the address of the first method of any type (i.e. ToString)
                IntPtr ms = Marshal.ReadIntPtr(mt + offset);

                // Get the slot number of the virtual method entry from the MethodDesc data structure
                // Remark: the slot number is represented on 3 bytes
                long shift = Marshal.ReadInt64(md) >> 32;
                int slot = (int)(shift & SLOT_NUMBER_MASK);

                // Get the virtual method address relative to the first method slot
                address = ms + (slot * IntPtr.Size);
            }
            else
            {
                // Bypass default MethodDescriptor padding (8 bytes) 
                // Reach the CodeOrIL field which contains the address of the JIT-compiled code
                address = md + 8;
            }

            //var data = mi.GetMethodBody().GetILAsByteArray();
            //var data1 = new byte[data.Length];
            //Marshal.Copy(Marshal.ReadIntPtr(address), data1, 0, data1.Length);
            return address;
        }

        Control mask;
        Storyboard storyboard;
        void ShowDialogForm()
        {
            if (mask == null)
            {
                mask = new Control { Width = "100%", Height = "100%", Background = "0,0,0,0", Commands = { { nameof(Control.MouseDown), (s, e) => DragMove() } } };
                //淡入效果
                storyboard = new Storyboard
                {
                    Timelines =
                    {
                        new Timeline(1)
                        {
                            KeyFrames =
                            {
                                new KeyFrame<SolidColorFill>{ Property=nameof(Control.Background), Value="0,0,0,100" }
                            }
                        }
                    }
                };
            }
            this.Children.Add(mask);
            storyboard.Start(mask, TimeSpan.FromSeconds(0.3), 1, EndBehavior.Reservations);
#if !DesignMode
            var dv = new DialogView(this);
            dv.MarginTop = -100;
            dv.TransitionValue(nameof(MarginTop), (FloatField)100, TimeSpan.FromSeconds(0.3), new PowerEase { }, AnimateMode.EaseOut);
            Children.Add(dv);
#endif
        }

        public void CloseDialogForm(DialogView dialogView)
        {
            //采用过渡属性的写法定义淡出效果
            mask.TransitionValue(nameof(Control.Background), (SolidColorFill)"0,0,0,0", TimeSpan.FromSeconds(0.3), null, AnimateMode.Linear, () =>
            {
                this.Children.Remove(mask);
            });

            dialogView.TransitionValue(nameof(MarginTop), (FloatField)(-100), TimeSpan.FromSeconds(0.3), new PowerEase { }, AnimateMode.EaseIn, () =>
            {
                this.Children.Remove(dialogView);
            });
        }

        void Animation(Button button, IEase ease)
        {
            var old = button.MarginTop;
            button.TransitionValue(nameof(MarginTop), (FloatField)(button.MarginTop.Value + 150), TimeSpan.FromSeconds(0.8), ease, AnimateMode.EaseIn, () =>
            {
                button.MarginTop = old;
            });
        }
        /// <summary>
        /// 微秒级延迟,会稍有偏差
        /// </summary>
        /// <param name="time">延迟时间，1/毫秒，0.0500/500微秒</param>
        /// <returns></returns>
        public static double delayUs(double time)
        {
            System.Diagnostics.Stopwatch stopTime = new System.Diagnostics.Stopwatch();
            stopTime.Start();
            while (stopTime.Elapsed.TotalMilliseconds < time) { }
            stopTime.Stop();
            return stopTime.Elapsed.TotalMilliseconds;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWaitableTimer(Microsoft.Win32.SafeHandles.SafeWaitHandle hTimer,
            [In] ref long pDueTime, int lPeriod, int pfnCompletionRoutine, int lpArgToCompletionRoutine, int fResume);
        [DllImport("kernel32.dll")]
        public static extern Microsoft.Win32.SafeHandles.SafeWaitHandle CreateWaitableTimerW(int lpTimerAttributes, int bManualReset, int lpTimerName);
        static void Ex_Sleep(int us)
        {
            long duetime;
            duetime = -10 * us;
            Microsoft.Win32.SafeHandles.SafeWaitHandle hTimer = CreateWaitableTimerW(0, 0, 0);
            if (SetWaitableTimer(hTimer, ref duetime, 0, 0, 0, 0))
            {
                using (EventWaitHandle wh = new EventWaitHandle(false, EventResetMode.AutoReset))
                {
                    wh.SafeWaitHandle = hTimer;
                    wh.WaitOne();
                }
            };

            //while (MsgWaitForMultipleObjects(1, &hTimer, 0, -1, 255) != 0)
            //{
            //    MSG msg;
            //    while (PeekMessageW(&msg, 0, 0, 0, 1))
            //    {
            //        TranslateMessage(&msg);
            //        DispatchMessageW(&msg);
            //    }
            //}
            //CloseHandle(hTimer);
        }
        public static int GetTickCount64()
        {
            return System.Environment.TickCount;
        }

        public class Animation_easing
        {
            /// <summary>
            /// 创建缓动动画
            /// </summary>
            /// <param name="uIElement">控件</param>
            /// <param name="nTotalTime">总用时</param>
            /// <param name="nInterval">间隔</param>
            /// <param name="nStart">开始值</param>
            /// <param name="nStop">结束值</param>
            public Animation_easing(UIElement uIElement,
              int nTotalTime, int nInterval, int nStart, int nStop)
            {
                var timeThread = new Thread(() =>
                {
                    var Easing = Animator.Tween.Easing_create(Animator.Tween.AnimateMode.Bounce);


                    var index = 1f;
                    double nFrameStep = nInterval / nTotalTime;
                    var nFrameCount = nTotalTime / nInterval;
                    while (true)
                    {
                        var nProcessTime = GetTickCount64();
                        var i = 1;

                        List<float> list = new List<float>();
                        //b初始值
                        //c终止值
                        //d 任意应该是用时
                        //t时间
                        int b = 60, c = 700, d = 60; float t = 1f;

                        while (i <= nFrameCount)
                        {
                            Ex_Sleep((nInterval - (GetTickCount64() - nProcessTime)) * 1000);
                            var v = Easing[0](t, b, c, d);
                            list.Add((float)v);
                            uIElement.Invoke(() =>
                            {

                                /*uIElement.RenderTransform = new GeneralTransform
                                { 
                                     OffsetX = (float)v,
                                };*/

                                uIElement.MarginLeft = v;
                                if (t < d)
                                {
                                    t += 0.5f;
                                }
                                //if (uIElement.MarginLeft.Value >= 700)
                                //{
                                //    index = -3f;
                                //}
                                //if (uIElement.MarginLeft.Value <= 60f)
                                //{
                                //    index = 3f;
                                //}
                                //uIElement.MarginLeft = v;
                            });
                            i++;
                            nProcessTime = GetTickCount64();
                        }
                        //break;
                        //Debug.WriteLine(list);
                    }
                })
                { IsBackground = true, Name = "测试" };
                timeThread.Start();
            }

        };
        Storyboard storyboard1;
        void Animation(UIElement button)
        {
            new Animation_easing(button, 1000, 5, 60, 700);

            //Task.Factory.StartNew(()=> {
            //    var text = ResourceManager.GetText("res://ConsoleApp1/Points.json");
            //    text.Wait();

            //    var json = CPF.Json.JsonSerializer.ToObject<List<Point>>(text.Result);

            //    foreach (var item in json)
            //    {
            //        Invoke(() => {
            //            //操作代码
            //            button.MarginLeft = item.X;
            //            button.MarginTop = item.Y;
            //        });
            //    }
            //});
            //Task.Factory.StartNew(() => {
            //    List<Point> points = new List<Point>();

            //    轨迹模拟.BezierTool bezier = new 轨迹模拟.BezierTool();
            //    var size = new Size(826, 548);
            //    int width1 = (int)size.Width;
            //    int height1 = (int)size.Height;
            //    Random rd = new Random();
            //    Point head = new Point();
            //    for (int i = 0; i < 20; i++)
            //    {
            //        List<Point> MyBezierpoints = new List<Point>();
            //        if (head.IsEmpty)
            //        {
            //            MyBezierpoints.Add(new Point(0, 0));
            //            MyBezierpoints.Add(new Point(rd.Next(1, width1), rd.Next(1, height1)));
            //            MyBezierpoints.Add(new Point(rd.Next(1, width1), rd.Next(1, height1)));
            //            head.X = MyBezierpoints[2].X;
            //            head.Y = MyBezierpoints[2].Y;

            //        }
            //        else
            //        {
            //            MyBezierpoints.Add(head);
            //            MyBezierpoints.Add(new Point(rd.Next(1, width1), rd.Next(1, height1)));
            //            MyBezierpoints.Add(new Point(rd.Next(1, width1), rd.Next(1, height1)));
            //            head.X = MyBezierpoints[2].X;
            //            head.Y = MyBezierpoints[2].Y;
            //        }
            //        bezier.DrawMyBezier(MyBezierpoints.ToArray(), button,ref points);

            //        //bezier.Delay(rd.Next(1, 2) * 1000);
            //    }
            //    List<Point> MyBezierpoints1 = new List<Point>();
            //    MyBezierpoints1.Add(head);
            //    MyBezierpoints1.Add(new Point(rd.Next(1, width1), rd.Next(1, height1)));
            //    MyBezierpoints1.Add(new Point(713, 12));
            //    bezier.DrawMyBezier(MyBezierpoints1.ToArray(), button, ref points);
            //    var json = JsonSerializer.Serialize<List<Point>>(points);
            //});
        }
        void Animation(Button button)
        {
            //button.RenderTransform = new GeneralTransform();

            //var timeThread = new Thread(()=> {
            //    float i = 1;

            //    while (true)
            //    {
            //        Thread.Sleep(2);
            //        Invoke(() => {
            //            if (button.MarginLeft == 700)
            //            {
            //                i = -1f;
            //            }
            //            if (button.MarginLeft == 60)
            //            {
            //                i = 1f;
            //            }
            //            button.MarginLeft += i;
            //        });
            //    }
            //    
            //}) { IsBackground = true, Name = "测试" };
            //timeThread.Start();

            if (storyboard1 == null)
            {
                storyboard1 = new Storyboard
                {
                    Timelines =
                    {
                        new Timeline(0.5f)
                        {
                            KeyFrames =
                            {
                                new KeyFrame<FloatField>{ Property=nameof(button.MarginLeft), Value=700, AnimateMode= AnimateMode.EaseIn, Ease=new PowerEase() },
                                //new KeyFrame<GeneralTransform>{ Property=nameof(button.RenderTransform),Value=new GeneralTransform{ OffsetX=700 }, AnimateMode= AnimateMode.EaseIn, Ease=new PowerEase() },
                            }
                        },
                        new Timeline(1)
                        {
                            KeyFrames =
                            {
                                new KeyFrame<FloatField>{ Property=nameof(button.MarginLeft), Value=60, AnimateMode= AnimateMode.EaseInOut, Ease=new QuadraticEase() },
                                //new KeyFrame<GeneralTransform>{ Property=nameof(button.RenderTransform),Value=new GeneralTransform{ OffsetX=60 }, AnimateMode= AnimateMode.EaseIn, Ease=new QuadraticEase() },
                            }
                        },
                        //new Timeline(1)
                        //{
                        //    KeyFrames =
                        //    {
                        //        new KeyFrame<GeneralTransform>{ Property=nameof(button.RenderTransform),Value=new GeneralTransform{ Angle=130,SkewX=0,ScaleY=3 }, AnimateMode= AnimateMode.EaseIn, Ease=new ElasticEase() },
                        //    }
                        //},
                    }
                };
            }
            storyboard1.Start(button, TimeSpan.FromSeconds(2), 0);
        }

        Collection<ItemData> Items
        {
            get { return GetValue<Collection<ItemData>>(); }
            set { SetValue(value); }
        }

        IList Data
        {
            get { return GetValue<IList>(); }
            set { SetValue(value); }
        }
        ItemCollection ItemCollection
        {
            get { return GetValue<ItemCollection>(); }
            set { SetValue(value); }
        }

        public Collection<DataGridColumn> Columns
        {
            get { return GetValue<Collection<DataGridColumn>>(); }
            set { SetValue(value); }
        }
        //[PropertyChanged(nameof(IsKeyboardFocusWithin))]
        //void OnIsKeyboardFocusWithin(object newValue,object oldValue,PropertyMetadataAttribute attribute)
        //{

        //}

        //public B<string> Text
        //{
        //    get { return GetValue<string>(); }
        //    set { SetValue(value); }
        //}

        //void TestBinding()
        //{
        //    Text = (nameof(Model.SelectValue), BindingMode.OneWay);
        //    string text = Text;
        //}

        static void WritePrivateProfileString(string section, string key,
                    string val, string filePath)
        {
            IList<string> text;
            if (System.IO.File.Exists(filePath))
            {
                text = System.IO.File.ReadAllLines(filePath);
                int hasSection = -1;
                var hasKey = false;
                for (int i = 0; i < text.Count; i++)
                {
                    var line = text[i];
                    if (hasSection < 0 && line.StartsWith("[" + section + "]"))
                    {
                        hasSection = i;
                    }
                    else if (hasSection >= 0 && line.StartsWith(key + "="))
                    {
                        text[i] = key + "=" + val;
                        hasKey = true;
                        break;
                    }
                }
                if (hasSection < 0)
                {
                    text = new List<string>(text);
                    text.Add("[" + section + "]");
                    text.Add(key + "=" + val);
                }
                else if (!hasKey)
                {
                    text = new List<string>(text);
                    text.Insert(hasSection + 1, key + "=" + val);
                }
            }
            else
            {
                text = new string[] { "[" + section + "]", key + "=" + val };
            }
            System.IO.File.WriteAllLines(filePath, text);
        }

        static string GetPrivateProfileString(string section, string key, string def, string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                var text = System.IO.File.ReadAllLines(filePath);
                var hasSection = false;
                for (int i = 0; i < text.Length; i++)
                {
                    var line = text[i];
                    if (!hasSection && line.StartsWith("[" + section + "]"))
                    {
                        hasSection = true;
                    }
                    else if (hasSection && line.StartsWith(key + "="))
                    {
                        return line.Substring(key.Length + 1).Trim();
                    }
                }
            }
            return def;
        }
        void addItem(CpfObject obj, RoutedEventArgs eventArgs)
        {
            SelectNode.Nodes.Add(new NodeData { Text = "dfsfs" });
            //testTreeVIew.Items = new Collection<TreeViewItem> { new TreeViewItem { Header = "4532" } };
        }

        public NodeData SelectNode { get { return GetValue<NodeData>(); } set { SetValue(value); } }

        public Collection<NodeData> Nodes { get { return GetValue<Collection<NodeData>>(); } set { SetValue(value); } }
        void RemoveItem(CpfObject obj, RoutedEventArgs eventArgs)
        {
            //SelectNode.Parent.Nodes.Remove(SelectNode);
            SelectNode.Text = "12313131";
        }
        void scrollEnd(CpfObject obj, RoutedEventArgs eventArgs)
        {
            textBox.SelectAll();
        }
        void TouchDownTest(CpfObject obj, TouchEventArgs eventArgs)
        {
            Debug.WriteLine(string.Join("、", eventArgs.TouchDevice.GetPositions(obj as UIElement)));
        }
        void scrollViewerMouseDown(CpfObject obj, MouseButtonEventArgs eventArgs)
        {
            (obj as UIElement).CaptureMouse();
        }
        void scrollViewerMouseUp(CpfObject obj, MouseButtonEventArgs eventArgs)
        {
            (obj as UIElement).ReleaseMouseCapture();
        }

        public CPF.Documents.Document Document
        {
            get { return GetValue<CPF.Documents.Document>(); }
            set { SetValue(value); }
        }

        void comboBoxtest(CpfObject obj, RoutedEventArgs eventArgs)
        {
            page1.ShowLoading("加载。。。", a =>
            {
                Thread.Sleep(10000);
                a.Message = "加载123";
                Thread.Sleep(10000);
            });
            var combobox = obj as ComboBox;
            combobox.Items.Clear();
            for (int i = 2; i < 10; i++)
            {
                combobox.Items.Add(i.ToString());
            }
            combobox.SelectedIndex = 0;
        }
        void TestComboBox(CpfObject obj, CPFPropertyChangedEventArgs eventArgs)
        {
            if ((bool)eventArgs.NewValue)
            {
                var p = typeof(ComboBox).GetProperty("Popup", BindingFlags.Static | BindingFlags.NonPublic);
                var popue = p.GetValue(null, null) as Popup;
                //popue.LayoutManager.ExecuteLayoutPass();
            }
        }
        CustomScrollData customScrollData = new CustomScrollData
        {
            DefaultSize = 18,
            Custom = new (int, float)[]
            {
                (8, 180),
                (38, 180),
                (68, 180)
            }
        };
        void AddTest(CpfObject obj, RoutedEventArgs eventArgs)
        {
            var data = Data.GetDataTable();
            var row = data.NewRow();
            for (int j = 0; j < 9; j++)
            {
                if (j != 1)
                {
                    if (j == 5)
                    {
                        row[j] = (Image)"res://ConsoleApp1/Resources/主页.png";
                    }
                    else if (j == 7)
                    {
                        row[7] = new Button { Content = "test" + data.Rows.Count, Width = "100%" };
                    }
                    else
                    {
                        row[j] = data.Rows.Count;
                    }
                }
            }
            row[0] = data.Rows.Count % 3;
            row[1] = true;


            //var customData = new List<(object obj, float size)>();
            ////先获取原来索引对应的数据
            //foreach (var item in customScrollData.Custom)
            //{
            //    var _item = ItemCollection[item.index];
            //    customData.Add((_item, item.size));
            //}

            data.Rows.Add(row);

            ////添加完数据后再获取新的索引，更新到customScrollData
            //List<(int index, float size)> list = new List<(int index, float size)>();
            //foreach (var item in customData)
            //{
            //    var index = ItemCollection.IndexOf(item.obj);
            //    if (index > -1)
            //    {
            //        list.Add((index, item.size));
            //    }
            //}
            //customScrollData.Custom = list.OrderBy(a => a.index);

            //if (column.Width.IsStar)
            //{
            //    column.Width = 100;
            //}
            //column.Width = column.Width.Value + 1;
        }



        void testHandled(CpfObject obj, MouseButtonEventArgs eventArgs)
        {

        }
        void TestHandled1(CpfObject obj, MouseButtonEventArgs eventArgs)
        {

        }
        void svgMouseUP(CpfObject obj, MouseButtonEventArgs eventArgs)
        {
            MessageBox.Show("test");
        }
        void lineMouseDown(CpfObject obj, MouseButtonEventArgs eventArgs)
        {

        }
        Popup popup;
        void ShowPopup(CpfObject obj, MouseButtonEventArgs eventArgs)
        {
            Console.WriteLine(Root.InputManager.KeyboardDevice.Modifiers);
            Debug.WriteLine(Root.InputManager.KeyboardDevice.Modifiers);
            if (popup == null)
            {
                popup = new Popup
                {
                    CanActivate = true,
                    StaysOpen = true,
                    Placement = PlacementMode.Padding,
                    PlacementTarget = obj as UIElement,
                    MarginTop = -10,
                    Children =
                    {
                        new Component1{ }
                    },
                    Commands =
                    {
                        {nameof(UIElement.LostFocus),(s,e)=>{
                            popup.Hide();
                        } },
                        {nameof(UIElement.GotFocus),(s,e)=>{
                            popup.Width="auto";
                            popup.Height="auto";
                        } }
                    }
                };
            }
            popup.Show();
        }
        void textFocus(CpfObject obj, CPFPropertyChangedEventArgs eventArgs)
        {
            //textBox.Text = "";
        }
        void ShowLayer(CpfObject obj, RoutedEventArgs eventArgs)
        {
            new Window2 { DataContext = DataContext, CommandContext = CommandContext }.Show();
            //new LayerDialog { Content =  }.ShowDialog(this);
        }
        void TestClear(CpfObject obj, RoutedEventArgs eventArgs)
        {
            //Items = new Collection<ItemData>() { new ItemData { Name = "231", Introduce = "fgsa" } };
            Items.Clear();
            Items.Add(new ItemData { Name = "231", Introduce = "fgsa" });
            Items.Add(new ItemData { Name = "2341", Introduce = "f2gsa" });
            Items.Add(new ItemData { Name = "2331", Introduce = "fg54sa" });
        }



        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }


        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
        void addColumnClick(CpfObject obj, RoutedEventArgs eventArgs)
        {
            testGrid.Columns.Add(new DataGridTextColumn { Width = 100, Header = "test" });
        }
        void ClearData(CpfObject obj, RoutedEventArgs eventArgs)
        {
            Data.GetDataTable().Clear();
        }
        void addColumn(CpfObject obj, RoutedEventArgs eventArgs)
        {
            testDataGrid.Columns.Add(new DataGridTextColumn { Binding = "p" + (testDataGrid.Columns.Count + 1), Header = "p" + (testDataGrid.Columns.Count + 1), Width = "100" });
        }
        void KeyDownTest(CpfObject obj, KeyEventArgs eventArgs)
        {
            //Close();
        }

        //protected override void OnClosing(ClosingEventArgs e)
        //{
        //    base.OnClosing(e);
        //    var f = new Window5();
        //    f.ShowDialogSync();
        //    e.Cancel = true;
        //}

    }


}
