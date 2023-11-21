using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using CPF.Controls;
using CPF;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF.Animation;
using CPF.Input;
using CPF.Effects;
using CPF.Documents;
using System.Diagnostics;
using System.Threading;
using CPF.Svg;

namespace ConsoleApp1
{
    public class Window1 : Window
    {
        public Window1()
        { }

        public Window1 Window { get; set; }

        Window1 Test()
        {
            return null;
        }

        PiecesEffect effect = new PiecesEffect { Value = 1 };
        protected override void InitializeComponent()
        {
            //Effect = new BlurEffect { BlurRadius = 8 };
            //ResourceManager.GetImage("res://ConsoleApp1.icon.png", a =>
            //{
            //    Icon = a;
            //});
            Icon = "res://ConsoleApp1.icon.png";
            ViewFill color = "#fff";
            ViewFill hoverColor = "255,255,255,40";
            Title = "标题123test";
            CanResize = true;
            Background = null;
            MinWidth = 150;
            MinHeight = 50;
            //AllowDrop = true;
            Width = 810.4f;
            Height = 575.2f;
            DragThickness = 10;
            //var bmp = new Bitmap(750, 500);
            //using (var dc = DrawingContext.FromBitmap(bmp))
            //{
            //    using (var brush = new SolidColorBrush(Color.Beige))
            //    {
            //        dc.FillRectangle(brush, new Rect(20, 20, bmp.Width - 40, bmp.Height - 40));
            //    }
            //}
            //StackBlur.ProcessShadow(bmp, 10, new Rect(40, 40, 0, 0));
            //Background = bmp;

            //窗体阴影
            var frame = Children.Add(new Border
            {
                Width = "100%",
                Height = "100%",
                Background = "255,255,255",
                BorderType = BorderType.BorderStroke,
                BorderStroke = new Stroke(0),//ShadowHorizontal = 10,
                //ShadowVertical = 10,
                Bindings =
                {
                    {
                        nameof(Border.ShadowBlur),
                        nameof(Window.WindowState),
                        this,
                        BindingMode.OneWay,
                        (WindowState a) => a == WindowState.Maximized||a== WindowState.FullScreen ? 0 : 20
                    }
                }
            });
            //用来裁剪内容，不然内容超出阴影
            var clip = new Decorator
            {
                Width = "100%",
                Height = "100%",
                ClipToBounds = true,//Background = "#fff"
            };
            frame.Child = clip;
            var grid = (Grid)(clip.Child = new Grid
            {
                Width = "100%",
                Height = "100%",
                ColumnDefinitions =
                {
                    new ColumnDefinition()
                },
                RowDefinitions =
                {
                    new RowDefinition
                    {
                        Height = 30
                    },
                    new RowDefinition
                    {

                    }
                },
            });
            //标题栏和按钮
            grid.Children.Add(
            new Panel
            {
                Name = "caption",//Background = "#1E9FFF",
                Width = "100%",
                Height = "100%",
                Commands =
                {
                    {
                        nameof(Window.MouseDown),
                        nameof(Window.DragMove),
                        this
                    }
                },
                Children =
                {
                    new TextBlock
                    {
                        MarginLeft=10,
                        Bindings=
                        {
                            {
                                nameof(TextBlock.Text),
                                nameof(Window.Title),
                                this
                            }
                        },
                        Foreground="#fff"
                    },
                    new StackPanel
                    {
                        MarginRight=0,
                        Height = "100%",
                        Orientation= Orientation.Horizontal,
                        Children =
                        {
                            new Panel
                            {
                                ToolTip="最小化",
                                Name="min",
                                Width = 30,
                                Height = "100%",
                                Children =
                                {
                                    new Line
                                    {
                                        MarginLeft=8,
                                        MarginTop=5,
                                        StartPoint = new Point(1, 13),
                                        EndPoint = new Point(14, 13),
                                        StrokeStyle = "2",
                                        IsAntiAlias=true,
                                        StrokeFill=color
                                    },
                                },
                                Commands =
                                {
                                    {
                                        nameof(Button.MouseDown),
                                        (s,e)=>
                                        {
                                            (e as MouseButtonEventArgs).Handled = true;
                                            this.WindowState = WindowState.Minimized;
                                        }
                                    }
                                },
                                Triggers=
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
                            },
                            new Panel
                            {
                                ToolTip="最大化",
                                Name="max",
                                Width = 30,
                                Height = "100%",
                                Children=
                                {
                                    new Rectangle
                                    {
                                        Width=14,
                                        Height=12,
                                        StrokeStyle="2",
                                        StrokeFill = color
                                    }
                                },
                                Commands =
                                {
                                    {
                                        nameof(Button.MouseDown),
                                        (s,e)=>
                                        {
                                            (e as MouseButtonEventArgs).Handled = true;
                                            this.WindowState= WindowState.Maximized;
                                        }
                                    }
                                },
                                Triggers=
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
                                Bindings =
                                {
                                    {
                                        nameof(Border.Visibility),
                                        nameof(Window.WindowState),
                                        this,
                                        BindingMode.OneWay,
                                        a => (WindowState)a == WindowState.Maximized||(WindowState)a == WindowState.FullScreen ? Visibility.Collapsed : Visibility.Visible
                                    }
                                },
                            },//.Bind(
                            //    this,
                            //    t=>t.Visibility== Visibility.Visible?WindowState.Maximized: WindowState.Minimized,
                            //    s=>s.WindowState == WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible,
                            //    BindingMode.TwoWay
                            //    )
                            //.Trigger(a=>a.IsMouseOver,Relation.Me,(nameof(UIElement.FocusFrameFill),"#fff"))
                            //.Attached(Grid.ColumnIndex,1),
                            new Panel
                            {
                                ToolTip="向下还原",
                                Name="nor",
                                Width = 30,
                                Height = "100%",
                                Children=
                                {
                                    new Rectangle
                                    {
                                        MarginTop=15,
                                        MarginLeft=8,
                                        Width=11,
                                        Height=8,
                                        StrokeStyle="1.5",
                                        StrokeFill = color
                                    },
                                    new Polyline
                                    {
                                        MarginTop=11,
                                        MarginLeft=12,
                                        Points=
                                        {
                                            new Point(0,3),
                                            new Point(0,0),
                                            new Point(9,0),
                                            new Point(9,7),
                                            new Point(6,7)
                                        },
                                        StrokeFill = color,
                                        StrokeStyle="2"
                                    }
                                },
                                Commands =
                                {
                                    {
                                        nameof(Button.MouseDown),
                                        (s, e) =>
                                        {
                                            (e as MouseButtonEventArgs).Handled = true;
                                            this.WindowState = WindowState.Normal;
                                        }
                                    }
                                },
                                Bindings =
                                {
                                    {
                                        nameof(Border.Visibility),
                                        nameof(Window.WindowState),
                                        this,
                                        BindingMode.OneWay,
                                        (WindowState a) => a == WindowState.Normal ? Visibility.Collapsed : Visibility.Visible
                                    }
                                },
                                Triggers=
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
                            },
                            new Panel
                            {
                                Name="close",
                                ToolTip="关闭",
                                Width = 30,
                                Height = "100%",
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
                                        (s,e)=>
                                        {
                                            (e as MouseButtonEventArgs).Handled=true;
                                            //关闭播放动画
                                    effect.Value=0;
                                            Effect = effect;
                                            Storyboard storyboard1 = new Storyboard
                                            {
                                                Timelines =
                                                {
                                                    new Timeline(1)
                                                    {
                                                        KeyFrames =
                                                        {
                                                            new KeyFrame<float>
                                                            {
                                                                Value=1,
                                                                Property="Effect.Value",
                                                                AnimateMode= AnimateMode.EaseIn,
                                                                Ease=new PowerEase
                                                                {
                                                                    Power=0.6
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            };
                                            storyboard1.Start(this, TimeSpan.FromSeconds(0.5f));
                                            storyboard1.Completed +=Storyboard_Completed;
                                        }
                                    },
                                    {
                                        nameof(Button.MouseDown),
                                        (s,e)=>
                                        {
                                            (e as MouseButtonEventArgs).Handled=true;
                                        }
                                    }
                                },
                                Triggers=
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
                            }
                        }
                    }
                }
            });
            var pop = new ContextMenu
            {
                Width = "auto",//PlacementTarget = btn,
                //Placement = PlacementMode.Mouse,
                Items = new UIElement[]
                {
                    new TestMenuItem
                    {
                        Header = "2313sdadadaaaaaaa1PlacementMode"
                    },
                    new Separator
                    {

                    },
                    new MenuItem
                    {
                        Header = "2",
                        Items=new MenuItem[]
                        {
                            new MenuItem
                            {
                                Header = "21",
                                        Commands =
                                        {
                                            {nameof(MenuItem.Click),MenuItemClick }
                                        }
                            },
                            new MenuItem
                            {
                                Header = "22",
                                Items=new MenuItem[]
                                {
                                    new MenuItem
                                    {
                                        Header = "221",
                                        Commands =
                                        {
                                            {nameof(MenuItem.Click),MenuItemClick }
                                        }
                                    },
                                    new MenuItem
                                    {
                                        Header = "222",
                                        Commands =
                                        {
                                            {nameof(MenuItem.Click),MenuItemClick }
                                        }
                                    }
                                }
                            },
                        },
                    },
                    new MenuItem
                    {
                        Header = "3",
                        IsCheckable=true,
                        Items = new MenuItem[]
                        {
                            new MenuItem
                            {
                                Header = "31",
                                        Commands =
                                        {
                                            {nameof(MenuItem.Click),MenuItemClick }
                                        }
                            },
                            new MenuItem
                            {
                                Header = "32"
                            },
                        }
                    }
                },
            };
            var datagrid = new DataGrid
            {
                //IsVirtualizing=false,
                Width = "60.000004%",
                Height = 342,//Background = "#fff",
                Columns =
                {
                    new DataGridComboBoxColumn
                    {
                        Header="dfsd",
                        Binding=new DataGridBinding("Item1",BindingMode.TwoWay),
                        Width=100,
                        Items=
                        {
                            "0",
                            "1",
                            "2",
                            "3"
                        },
                        CanUserSort=false
                    },
                    new DataGridCheckBoxColumn
                    {
                        Header="d1fsd",
                        Binding=new DataGridBinding("Item2")
                        {
                            BindingMode= BindingMode.TwoWay
                        },
                        Width=100,
                        CanUserSort=false
                    },
                    new DataGridTextColumn
                    {
                        Header="3dfsd",
                        Binding=new DataGridBinding("Item3")
                        {
                            BindingMode= BindingMode.TwoWay
                        },
                        Width="100"
                    },
                    new DataGridTextColumn
                    {
                        Header="输入类型验证",
                        Binding=new DataGridBinding("Item4")
                        {
                            BindingMode= BindingMode.TwoWay
                        },
                        Width="100"
                    },
                    new DataGridTemplateColumn
                    {
                        Header="自定义模板",
                        Binding=new DataGridBinding("Item5"),
                        Width=100,
                        CellTemplate=typeof(CellTemplate)
                    },
                    new DataGridTextColumn
                    {
                        Header="d1fsd",
                        Binding=new DataGridBinding("Item6"),
                        Width=100
                    },
                    new DataGridTextColumn
                    {
                        Header="3dfsd",
                        Binding=new DataGridBinding("Item7"),
                        Width="100"
                    },
                    new DataGridTextColumn
                    {
                        Header="3dfsd",
                        Binding=new DataGridBinding("Item8"),
                        Width="100"
                    },
                    new DataGridTextColumn
                    {
                        Header="3dfsd",
                        Binding=new DataGridBinding("Item9"),
                        Width="100"
                    },
                },
                Bindings =
                {
                    {
                        nameof(DataGrid.Items),
                        nameof(Model.Data)
                    }
                }
            };
            grid.Children.Add(new TabControl
            {
                Width = "100%",
                Height = "100%",//Background = "10,255,255,255",
                SwitchAction = (oldItem, newItem) =>
                {
                    if (oldItem != null && oldItem.ContentElement != null)
                    {
                        oldItem.ContentElement.TransitionValue(nameof(UIElement.MarginLeft), (FloatField)"-100%", TimeSpan.FromSeconds(0.2), new PowerEase(), AnimateMode.EaseOut, () =>
                        {
                            oldItem.ContentElement.Visibility = Visibility.Collapsed;
                        });
                    }
                    if (newItem != null && newItem.ContentElement != null)
                    {
                        newItem.ContentElement.Visibility = Visibility.Visible;
                        newItem.ContentElement.MarginLeft = "100%";
                        newItem.ContentElement.TransitionValue(nameof(UIElement.MarginLeft), (FloatField)"0%", TimeSpan.FromSeconds(0.2), new PowerEase(), AnimateMode.EaseOut);
                    }
                },
                Items =
                {
                    new TabItem
                    {
                        Header="基础控件",
                        Content=new StackPanel
                        {
                            Width="100%",
                            Height="100%",//Background=Color.FromArgb(100,0,0,0),
                        //Background="linear-gradient(0 0,300 300,#fff,#000,#faa)",
                        Children=
                            {
                                new StackPanel
                                {
                                    Orientation= Orientation.Horizontal,
                                    Children=
                                    {
                                        new Button
                                        {
                                            Content = "打开文件对话框",
                                            Width=100,
                                            Height=20,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    async (s,e)=>
                                                    {
                                                        var f=new OpenFileDialog
                                                        {
                                                            Title="打开文件",
                                                            AllowMultiple=true
                                                        };
                                                        f.Filters.Add(new FileDialogFilter
                                                        {
                                                            Name="*",
                                                            Extensions="bmp,jpeg,png,jpg"
                                                        });
                                                        var sf=await f.ShowAsync(this);
                                                    }
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content = "选择目录对话框",
                                            Width=100,
                                            Height=20,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=>
                                                    {
                                                        ThreadPool.QueueUserWorkItem(async a=>
                                                        {
                                                            var sf=await new OpenFolderDialog
                                                            {
                                                                Title="标题"
                                                            }
                                                            .ShowAsync(this);
                                                        },null);
                                                    }
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content = "保存文件对话框",
                                            Width=100,
                                            Height=20,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    async (s,e)=>
                                                    {
                                                        var f=new SaveFileDialog
                                                        {
                                                            Title="保存文件",
                                                            Filters=
                                                            {
                                                                new FileDialogFilter
                                                                {
                                                                    Extensions="dll",
                                                                    Name="dll文件"
                                                                }
                                                            }
                                                        };
                                                        var sf= await f.ShowAsync(this);
                                                    }
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content = "模态窗体",
                                            Width=100,
                                            Height=20,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    async (s,e)=>
                                                    {
                                                        var f=new Window();
                                                        f.Background="#000";
                                                        f.Width=300;
                                                        f.Height=300;
                                                        f.CanResize=true;
                                                        f.Commands.Add(nameof(f.DoubleClick),(ss,ee)=>f.Close());
                                                        await f.ShowDialog(this);
                                                        //MessageBox.Show("dfs");
                                                        System.Diagnostics.Debug.WriteLine("test");
                                                    }
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content = "切换样式2",
                                            Width=100,
                                            Height=20,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=>
                                                    {
                                                        var b=s as Button;
                                                        if (b.Content.ToString()=="切换样式2")
                                                        {
                                                            LoadStyleFile("res://ConsoleApp1.Stylesheet2.css");
                                                            b.Content="切换样式1";
                                                        }
                                                        else
                                                        {
                                                            LoadStyleFile("res://ConsoleApp1.Stylesheet1.css");
                                                            b.Content="切换样式2";
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content="全屏",
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=>
                                                    {
                                                        this.WindowState=WindowState== WindowState.FullScreen?WindowState.Normal: WindowState.FullScreen;
                                                    }
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content="最前端",
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=>
                                                    {
                                                        this.TopMost = !this.TopMost;
                                                    }
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content="ShowInBar",
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=>
                                                    {
                                                        this.ShowInTaskbar = !this.ShowInTaskbar;
                                                    }
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content="移动窗体",
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    (s,e)=>
                                                    {
                                                        this.Position=new PixelPoint(-100,0);
                                                    }
                                                }
                                            }
                                        },
                                    }
                                },
                                new CheckBox
                                {
                                    Content="TextBlock测试🌐",
                                    MarginTop=2
                                },
                                new CheckBox
                                {
                                    Content="选择2",
                                    MarginTop=2,
                                    IsThreeState=true
                                },
                                new RadioButton
                                {
                                    Content=new Button
                                    {
                                        Content="单选1"
                                    },
                                    Background="#fff",
                                    MarginTop=2,
                                    GroupName="分组"
                                },
                                new RadioButton
                                {
                                    Content="单选2",
                                    MarginTop=2,
                                    GroupName="分组"
                                },
                                new ComboBox
                                {
                                    ItemTemplate=new ListBoxItem
                                    {
                                        FontSize=24,
                                        Width="100%",
                                        ContentTemplate=new ContentTemplate
                                        {
                                            Width="auto",
                                            MarginLeft=0
                                        }
                                    },
                                    Width=100,
                                    Height=25,
                                    Items =
                                    {
                                        new ListBoxItem
                                        {
                                            Content="test"
                                        },
                                        new ListBoxItem
                                        {
                                            Content="test2"
                                        },
                                    }
                                },
                                new TextBox
                                {
                                    Classes=
                                    {
                                        "Single"
                                    },
                                    PasswordChar='*',
                                    Width=100,
                                    Height=25,
                                    Bindings=
                                    {
                                        {
                                            nameof(TextBlock.Text),
                                            nameof(Model.TextSize),
                                            null,
                                            BindingMode.OneWayToSource,
                                            null,
                                            a=>
                                            {
                                                var tb = Binding.Current.Owner as TextBox;
                                                return DrawingFactory.Default.MeasureString(a.ToString(), new Font(tb.FontFamily, tb.FontSize, tb.FontStyle)).ToString();
                                            }
                                        }
                                    }
                                },
                                new TextBlock
                                {
                                    Bindings=
                                    {
                                        {
                                            nameof(TextBlock.Text),
                                            nameof(Model.TestComputedProperty)
                                        },
                                        {
                                            nameof(TextBlock.Width),
                                            nameof(Model.TestWidth)
                                        }
                                    }
                                },
                                new Slider
                                {
                                    Height =40,
                                    Width=200,
                                    PresenterFor=this,
                                    Maximum=100,
                                    Name="slider",
                                    TickPlacement= TickPlacement.Both
                                },//new ProgressBar{Height =20,IsIndeterminate=true,Width=200,Value=30,Bindings={ {nameof(ProgressBar.Value),nameof(Slider.Value),this.FindPresenter<Slider>(a=>a.Name=="slider") } } },
                            new Button
                                {
                                    Content="附加文本",
                                    Commands=
                                    {
                                        {
                                            nameof(Button.Click),
                                            (s,e)=>
                                            {
                                                var textbox= this.FindPresenter<TextBox>().First(a=>a.Name=="textbox");
                                                textbox.AppentText("sadadfadaasda测试啊啊啊啊啊\nasdadfs哈哈哈哈哈哈哈啊啊啊撒");
                                                textbox.ScrollToEnd();
                                            }
                                        }
                                    }
                                },
                                new Button
                                {
                                    Content="format",
                                    Commands=
                                    {
                                        {
                                            nameof(Button.Click),
                                            ClickFormat
                                        }
                                    }
                                },
                                new TextBox
                                {
                                    Width="60%",
                                    Name="textbox",
                                    PresenterFor=this,
                                    Height=250,
                                    IsUndoEnabled=false,
                                    Text="",
                                    Foreground="#f00",//Background="#fff",
                                    IsAllowPasteImage=true,
                                    Styles=
                                    {
                                        new DocumentStyle
                                        {
                                            Foreground = "0,0,255"
                                        },
                                        new DocumentStyle
                                        {
                                            Foreground = "192,0,0"
                                        },
                                        new DocumentStyle
                                        {
                                            Foreground = "100,100,100"
                                        }
                                    },
                                    KeywordsStyles=
                                    {
                                        new KeywordsStyle
                                        {
                                            Keywords = "using |namespace |class |true|new ",
                                            IsRegex=true,
                                            StyleId = 0
                                        },
                                        new KeywordsStyle
                                        {
                                            Keywords = "(\").*(\")",
                                            IsRegex = true,
                                            StyleId = 1
                                        },
                                        new KeywordsStyle
                                        {
                                            Keywords = "(\\\').*(\\\')",
                                            IsRegex = true,
                                            StyleId = 2
                                        }
                                    }
                                },
                                new ScrollBar
                                {
                                    Width=200,
                                    Height=20,
                                    MarginTop=20,
                                    Orientation= Orientation.Horizontal
                                },
                            }
                        }
                    },
                    new TabItem
                    {
                        Header="动画",
                        Content=new Panel
                        {
                            Width="100%",
                            Height="100%",
                            Children=
                            {
                                new Button
                                {
                                    RenderTransform=new RotateTransform(10),
                                    Content = "按住鼠标播放动画",
                                    Width=100,
                                    Height=30,
                                    MarginTop=39,
                                    MarginLeft=0,
                                    Triggers=
                                    {
                                        new Trigger
                                        {
                                            Property=nameof(Button.IsMouseCaptured),
                                            Animation= new Storyboard
                                            {
                                                Timelines =
                                                {
                                                    new Timeline(.5f)
                                                    {
                                                        //定义一个时间线，从上个时间点到这个时间点。0到1，相对整个动画的时间。现在定义的是前一半的时间
                                    KeyFrames =
                                                        {
                                                            new KeyFrame<FloatField>
                                                            {
                                                                Property = nameof(UIElement.MarginLeft),//属性名
                                            Value = 400,//动画目标值
                                            //Ease = new PowerEase(),//缓动方式
                                            AnimateMode = AnimateMode.Linear//线性或者缓动
                                                            },//new KeyFrame<FloatField> {
                                        //    Property = nameof(UIElement.MarginTop),//属性名
                                        //    Value = 200,//动画目标值
                                        //}
                                                        }
                                                    },
                                                    new Timeline(1)
                                                    {
                                                        //从上一个时间点0.5到1，就是后一半的时间
                                    KeyFrames =
                                                        {
                                                            new KeyFrame<GeneralTransform>
                                                            {
                                                                Property = nameof(UIElement.RenderTransform),
                                                                Value = new GeneralTransform()
                                                                {
                                                                    Angle=720,
                                                                    ScaleX=2,
                                                                    ScaleY=2
                                                                },
                                                                Ease = new BackEase(),
                                                                AnimateMode = AnimateMode.Linear
                                                            }
                                                        }
                                                    },
                                                }
                                            },
                                            AnimationDuration = TimeSpan.FromSeconds(1)
                                        }
                                    },
                                },
                                new TextBlock
                                {
                                    MarginTop=111,
                                    MarginLeft=0,
                                    Text="鼠标移入变色CSS定义",
                                    Classes=
                                    {
                                        "testAnimation1"
                                    }
                                },
                                new Picture
                                {
                                    MarginTop=111.3f,
                                    Source="https://dss0.bdstatic.com/5aV1bjqh_Q23odCf/static/superman/img/logo_top-e3b63a0b1b.png",
                                    Triggers=
                                    {
                                        new Trigger(nameof(IsMouseOver), Relation.Me)
                                        {
                                            Animation=new Storyboard
                                            {
                                                Timelines =
                                                {
                                                    new Timeline(1)
                                                    {
                                                        KeyFrames =
                                                        {
                                                            new KeyFrame<GeneralTransform>
                                                            {
                                                                Property = nameof(UIElement.RenderTransform),
                                                                Value = new GeneralTransform()
                                                                {
                                                                    ScaleX=1.5f,
                                                                    ScaleY=1.5f
                                                                },
                                                                Ease = new BackEase(),
                                                                AnimateMode = AnimateMode.EaseIn
                                                            }
                                                        }
                                                    }
                                                }
                                            },
                                            AnimationDuration = TimeSpan.FromSeconds(.5)
                                        }
                                    }
                                },
                                new ScrollBar
                                {
                                    Name="animationTransition",
                                    Orientation= Orientation.Horizontal,
                                    Maximum=1000,
                                    Width=500
                                },
                                new Button
                                {
                                    MarginLeft=5,
                                    MarginTop=180,
                                    Content="动态过渡到某个值",
                                    Commands=
                                    {
                                        {
                                            nameof(MouseDown),
                                            (s,e)=>
                                            {
                                                this.Find<ScrollBar>().FirstOrDefault(a=>a.Name== "animationTransition").TransitionValue(a=>a.Value,new Random().Next(0,1000),TimeSpan.FromSeconds(0.3));
                                            }
                                        }
                                    }
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginLeft = 400,
                                    MarginTop = 271,
                                    Height = 85,
                                    Width=170,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginLeft = 205,
                                    MarginTop = 11,
                                    Height = 85,
                                    Width=170,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginRight = 154,
                                    MarginTop = 11,
                                    Height = 85,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginRight = 10,
                                    MarginTop = 11,
                                    Height = 85,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginRight = 3,
                                    MarginTop = 110,
                                    Height = 85,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginRight = 4,
                                    MarginTop = 195,
                                    Height = 85,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginRight = 4,
                                    MarginTop = 280,
                                    Height = 85,
                                    Stretch= Stretch.Uniform,
                                },
                                new Button
                                {
                                    Commands =
                                    {
                                        {
                                            nameof(Button.Click),
                                            nameof(PlayAnimation),
                                            this,
                                            CommandParameter.EventSender,
                                            CommandParameter.EventArgs
                                        },
                                    },
                                    MarginLeft = 393,
                                    MarginTop = 23,
                                    Height = 27,
                                    Width = 96,
                                    Content = "播放动画",
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginLeft = 172,
                                    MarginTop = 133,
                                    Height = 85,
                                    Width=170,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginLeft = 474,
                                    MarginTop = 148,
                                    Height = 85,
                                    Width=170,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginLeft = 16,
                                    MarginTop = 276,
                                    Height = 85,
                                    Width=170,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginLeft = 16,
                                    MarginTop = 361,
                                    Height = 85,
                                    Width=170,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginLeft = 216,
                                    MarginTop = 276,
                                    Height = 85,
                                    Width=170,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginLeft = 205,
                                    MarginTop = 373,
                                    Height = 85,
                                    Width=170,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginLeft = 409,
                                    MarginTop = 385,
                                    Height = 85,
                                    Width=170,
                                    Stretch= Stretch.Uniform,
                                },
                                new SVG("res://ConsoleApp1/test.svg")
                                {
                                    MarginLeft = 620,
                                    MarginTop = 395,
                                    Height = 85,
                                    Width=170,
                                    Stretch= Stretch.Uniform,
                                }
                            }
                        }
                    },
                    new TabItem
                    {
                        Header="虚拟模式加载大数据",
                        Content=new Panel
                        {
                            Width="100%",
                            Height="100%",
                            IsAntiAlias=true,//Background="url(http://static.tieba.baidu.com/tb/editor/images/client/image_emoticon16.png) no-repeat fill",
                        Children=
                            {
                                new StackPanel
                                {
                                    MarginTop=5,
                                    Orientation= Orientation.Horizontal,
                                    Children=
                                    {
                                        new TextBlock
                                        {
                                            MarginTop=5,
                                            Text="插入数据的索引位置:"
                                        },
                                        new TextBox
                                        {
                                            MarginLeft=5,
                                            Height=25,
                                            Width=50,
                                            Classes=
                                            {
                                                "Single"
                                            },
                                            Bindings=
                                            {
                                                {
                                                    nameof(TextBox.Text),
                                                    nameof(Model.InsertIndex),
                                                    null,
                                                    BindingMode.OneWayToSource,
                                                    null,
                                                    a =>
                                                    {
                                                        int.TryParse(a.ToString(), out int result);
                                                        return result;
                                                    }
                                                }
                                            }
                                        },
                                        new TextBlock
                                        {
                                            MarginTop=5,
                                            Text="插入的数据:"
                                        },
                                        new TextBox
                                        {
                                            MarginLeft=5,
                                            Height=25,
                                            Width=50,
                                            Classes=
                                            {
                                                "Single"
                                            },
                                            Bindings=
                                            {
                                                {
                                                    nameof(TextBox.Text),
                                                    nameof(Model.InsertText),
                                                    null,
                                                    BindingMode.OneWayToSource
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            Content="插入",
                                            Width=60,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(Model.Insert)
                                                }
                                            },
                                            Foreground="#fff"
                                        },
                                        new Button
                                        {
                                            MarginLeft=5,
                                            Content="删除选中",
                                            Width=60,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(Model.RemoveSelect)
                                                }
                                            }
                                        },
                                        new Button
                                        {
                                            MarginLeft=5,
                                            Content="排序",
                                            Width=60,
                                            Commands=
                                            {
                                                {
                                                    nameof(Button.Click),
                                                    nameof(Model.Sort)
                                                }
                                            }
                                        },
                                    }
                                },
                                new ListBox
                                {
                                    //Background="#aaa",
                                Name="listbox",
                                    IsVirtualizing=true,//VirtualizationMode= VirtualizationMode.Recycling,
                                SelectionMode= SelectionMode.Extended,
                                    Width=200,
                                    Height=300,//Items=list,
                                    ItemTemplate=new ListBoxItem
                                    {
                                        Width="100%",
                                        FontSize=22,
                                        Tag=this,
                                    },
                                    Bindings=
                                    {
                                        {
                                            nameof(ListBox.Items),
                                            nameof(Model.List)
                                        },
                                        {
                                            nameof(ListBox.SelectedIndex),
                                            nameof(Model.SelectIndex),
                                            null,
                                            BindingMode.OneWayToSource
                                        }
                                    },
                                    Commands =
                                    {
                                        {
                                            nameof(MouseDown),
                                            (s,e)=>
                                            {
                                                Debug.WriteLine(((ListBox)s).IsInItem(((MouseButtonEventArgs)e).OriginalSource as UIElement));
                                            }
                                        }
                                    }
                                },//new Button{ Content="排序" },
                            }
                        }
                    },
                    new TabItem
                    {
                        Header="TreeView",
                        Content= new Panel
                        {
                            Width="100%",
                            Height="100%",
                            Children=
                            {
                                new Button
                                {
                                    Content="添加节点",
                                    Width=80,
                                    Height=30,
                                    MarginTop=2,
                                    Commands=
                                    {
                                        {
                                            nameof(Button.MouseDown),
                                            nameof(Model.AddNode)
                                        }
                                    }
                                },
                                new TreeView
                                {
                                    Width=150,
                                    Height=200,
                                    DisplayMemberPath="Text",
                                    ItemsMemberPath="Nodes",
                                    Background="#aaa",//Items= nodes
                                Bindings=
                                    {
                                        {
                                            nameof(TreeView.Items),
                                            nameof(Model.Nodes)
                                        }
                                    },
                                }
                            }
                        },
                    },
                    new TabItem
                    {
                        Header = "右键菜单",
                        Content =  new Button
                        {
                            Width = 100,
                            Height = 20,
                            Content = "右键",
                            ContextMenu = pop,
                        },
                    },
                    new TabItem
                    {
                        Header = "DataGrid",
                        Content =
                        new Panel
                        {
                            Width="100%",
                            Height="100%",
                            Children=
                            {
                                datagrid,
                                new Button
                                {
                                    Commands =
                                    {
                                        {
                                            "Click",
                                            nameof(ClearData),
                                            this,
                                            CommandParameter.EventSender,
                                            CommandParameter.EventArgs
                                        },
                                    },
                                    Height = 28,
                                    Width = 96,
                                    MarginLeft = 211,
                                    MarginTop = 36,
                                    Content = "ClearButton",
                                },
                                new Button
                                {
                                    Commands =
                                    {
                                        {
                                            nameof(Button.Click),
                                            nameof(AddRowClick),
                                            this,
                                            CommandParameter.EventSender,
                                            CommandParameter.EventArgs
                                        },
                                    },
                                    Height = 29,
                                    Width = 109,
                                    MarginLeft = 389,
                                    MarginTop = 36,
                                    Content = "AddButton",
                                },
                            }
                        },
                    },
                    new TabItem
                    {
                        Header = "布局",
                        Content =
                        new Panel
                        {
                            Width="100%",
                            Height="100%",
                            Children=
                            {
                                new Grid
                                {
                                    Name="testGrid",
                                    Background="#999",
                                    Width="80%",
                                    Height="80%",
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
                                            Width="300"
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
                                                Text="Grid布局。。。。。。。"
                                            },
                                            2,
                                            1
                                        },
                                        {
                                            new TextBlock
                                            {
                                                Background="#b1a",
                                                Width="100%",
                                                Height="100%",
                                                Text="跨列341231说的sda2131\ndddsad",
                                                TextTrimming= TextTrimming.CharacterEllipsis,
                                            },
                                            0,
                                            2,
                                            2
                                        },
                                        {
                                            new TextBlock
                                            {
                                                Background="#186",
                                                Width="100%",
                                                Height="100%",
                                                Text="跨行"
                                            },
                                            2,
                                            1,
                                            1,
                                            2
                                        },
                                        {
                                            new GridSplitter
                                            {
                                                MarginLeft=0,
                                                ShowsPreview=true
                                            },
                                            1,
                                            0
                                        }
                                    },
                                },
                            }
                        },
                    },
                    new TabItem
                    {
                        Header="拖拽",
                        Content =
                        new Panel
                        {
                            Width="100%",
                            Height="100%",
                            Children=
                            {
                                new TextBlock
                                {
                                    AllowDrop = true,
                                    Width = 462,
                                    MarginLeft=0,
                                    Text="文字拖拽给我",
                                    Triggers=
                                    {
                                        new Trigger(nameof(TextBlock.IsDragOver), Relation.Me)
                                        {
                                            Setters =
                                            {
                                                {
                                                    nameof(Background),
                                                    "#f00"
                                                }
                                            }
                                        }
                                    },
                                    Commands=
                                    {
                                        {
                                            nameof(Drop),
                                            (s,e)=>
                                            {
                                                if ((e as DragEventArgs).Data.Contains(DataFormat.Html))
                                                {
                                                    (s as TextBlock).Text = (e as DragEventArgs).Data.GetData(DataFormat.Html)?.ToString();
                                                }
                                                else if ((e as DragEventArgs).Data.Contains(DataFormat.Text))
                                                {
                                                    (s as TextBlock).Text = (e as DragEventArgs).Data.GetData(DataFormat.Text)?.ToString();
                                                }
                                                else if ((e as DragEventArgs).Data.Contains(DataFormat.FileNames))
                                                {
                                                    (s as TextBlock).Text = ((e as DragEventArgs).Data.GetData(DataFormat.FileNames) as IList<string>)[0];
                                                }
                                            }
                                        },
                                        {
                                            nameof(DragEnter),
                                            (s,e)=>
                                            {
                                                 //(e as DragEventArgs).DragEffects= DragDropEffects.Link;
                                            }
                                        },
                                    }
                                },
                                new TextBlock
                                {
                                    MarginRight=0,
                                    Text="文字拖拽给别人",
                                    Commands=
                                    {
                                        {
                                            nameof(MouseDown),
                                            (s,e)=>
                                            {
                                                Debug.WriteLine(DragDrop.DoDragDrop(DragDropEffects.Link,
                                                    (DataFormat.Text, "拖拽文字")
                                                    ,(DataFormat.Html, "<h1>拖拽文字1</h1>")
                                                    ));
                                            }
                                        }
                                    }
                                },
                            }
                        },
                    },
                    new TabItem
                    {
                        Header="位图特效",
                        Content =
                        new Panel
                        {
                            Width="100%",
                            Height="100%",
                            Children=
                            {
                                new Button
                                {
                                    CornerRadius = "10,10,10,10",
                                    MarginTop=5,
                                    Content="模糊，你撸多了",
                                    Effect=new BlurEffect
                                    {

                                    }
                                },
                                new Button
                                {
                                    MarginTop=35,
                                    Content="马赛克，你撸多了",
                                    Effect=new MosaicEffect
                                    {
                                        Size=2
                                    }
                                },
                                new Button
                                {
                                    MarginTop=65,
                                    Content="半透明",
                                    Effect=new OpacityEffect
                                    {
                                        Opacity=0.5f
                                    }
                                },
                                new Button
                                {
                                    MarginTop=95,
                                    Content="灰色",
                                    Effect=new GrayScaleEffect
                                    {

                                    }
                                },
                                new Picture
                                {
                                    MarginTop=125,
                                    Source="http://tb2.bdstatic.com/tb/img/single_member_100_0b51e9e.png",
                                    Effect= new ReliefEffect()
                                },
                            }
                        },
                    },
                    new TabItem
                    {
                        Header="背景特效",
                        Content =
                        new Panel
                        {
                            Width="100%",
                            Height="100%",
                            Children=
                            {
                                new TextBlock
                                {
                                    MarginTop=5,
                                    Text="渐变背景",
                                    Background="linear-gradient(0 0,30 30,#fff,#0f0,#faa)"
                                },
                                new TextBlock
                                {
                                    MarginTop=35,
                                    Height=50,
                                    Width=50,
                                    Text="径向渐变",
                                    Background=new RadialGradientFill
                                    {
                                        GradientStops=
                                        {
                                            new GradientStop(Color.Black,0),
                                            new GradientStop(Color.White,1)
                                        },
                                        Radius=10
                                    }
                                },
                                new TextBlock
                                {
                                    RenderTransform = new GeneralTransform
                                    {
                                        Angle = -5.5f,
                                    },
                                    MarginTop=95,
                                    Height=150,
                                    Width=150,
                                    Text="图片背景",
                                    Background="url(https://tb1.bdstatic.com/tb/r/image/2019-09-29/c29109a0c0d4fe6832d41fa180ffa8f1.jpg) no-repeat fill",
                                    Tag=1f,
                                    RenderTransformOrigin=new PointField(0,0),
                                    Commands =
                                    {
                                        {
                                            nameof(UIElement.MouseDown),
                                            (s,e)=>
                                            {
                                                var m=e as MouseEventArgs;
                                                var ele=s as UIElement;
                                                Matrix matrix=Matrix.Identity;
                                                if(ele.RenderTransform is MatrixTransform transform)
                                                {
                                                    matrix=transform.Value;
                                                }
                                                var v=(float)ele.Tag;
                                                v+=0.2f;
                                                ele.Tag=v;
                                                matrix.ScaleAtPrepend(v,v,m.Location.X,m.Location.Y);
                                                ele.RenderTransform =new MatrixTransform(matrix);
                                            }
                                        }
                                    }
                                },
                                new TextBlock
                                {
                                    MarginTop=255,
                                    Height=250,
                                    Width=250,
                                    Text="图片背景",
                                    Background="url(https://tb1.bdstatic.com/tb/r/image/2019-09-29/c29109a0c0d4fe6832d41fa180ffa8f1.jpg)"
                                },
                                new Button
                                {
                                    MarginTop = 32,
                                    MarginRight = 30,
                                    Height = 30,
                                    Width = 91,
                                    Content = "测试",
                                    Commands =
                                    {
                                        {
                                            nameof(Button.Click),
                                            (s,e)=>
                                            {
                                                System.Threading.ThreadPool.QueueUserWorkItem(a=>
                                                {
                                                    TestMessage();
                                                },null);
                                            }
                                        },
                                        {
                                            "Click",
                                            nameof(testClick),
                                            this,
                                            CommandParameter.EventSender,
                                            CommandParameter.EventArgs
                                        },
                                    }
                                },
                                new Border
                                {
                                    MarginTop = 353,
                                    BorderType = BorderType.BorderThickness,
                                    Name = "testBorder",
                                    CornerRadius = "0,0,0,0",
                                    Width = 420,
                                    Height = 88,
                                    Background = new RadialGradientFill
                                    {
                                        GradientStops=
                                        {
                                            new GradientStop(Color.Black,0),
                                            new GradientStop(Color.White,1)
                                        },
                                        Radius=10
                                    },
                                    ShadowBlur = 20,
                                    BorderFill = "#4F309EC0",
                                    IsAntiAlias = true,
                                    Triggers =
                                    {
                                        {
                                            nameof(Border.IsMouseOver),
                                            Relation.Me,
                                            null,
                                            (nameof(Border.BorderFill),"#f77da4")
                                        }
                                    }
                                },
                                new Polyline
                                {
                                    RenderTransform = new GeneralTransform
                                    {
                                        Angle = -6.5f,
                                    },
                                    StrokeStyle = "5,Solid",
                                    IsHitTestOnPath = true,
                                    MarginLeft = 599,
                                    MarginTop = 112,
                                    Points=
                                    {
                                        new Point(0,0),
                                        new Point(50,50),
                                        new Point(0,50)
                                    },
                                },
                                new Calendar
                                {
                                    MarginLeft = 29,
                                    MarginTop = 19,
                                },
                                new Panel
                                {
                                    Children =
                                    {
                                        new Button
                                        {
                                            MarginLeft = 24,
                                            MarginTop = 18,
                                            Content = "Button",
                                        },
                                        new Button
                                        {
                                            MarginLeft = 101,
                                            MarginTop = 18,
                                            Content = "Button",
                                        },
                                        new CheckBox
                                        {
                                            PresenterFor = this,
                                            Name = nameof(testCheckBox),
                                            MarginLeft = 22,
                                            MarginTop = 80,
                                            Content = "CheckBox",
                                        },
                                    },
                                    MarginLeft = 606,
                                    MarginTop = 184,
                                    Height = 146,
                                    Width = 166,
                                },
                            }
                        },
                    },
                    new TabItem
                    {
                        Content = new Grid
                        {
                            Size = SizeField.Fill,
                            RowDefinitions =
                            {
                                new RowDefinition
                                {

                                },
                            },
                            Children =
                            {
                                new ListBox
                                {
                                    Size = SizeField.Fill,
                                    ItemsPanel = new WrapPanel
                                    {
                                        Width="100%",
                                        Orientation = Orientation.Horizontal,
                                    },
                                    Items =
                                    {
                                        "",
                                        ""
                                    },
                                    ItemTemplate=new ListBoxItem
                                    {
                                        Width=100,
                                        Height=100,
                                        Background=Color.Red,
                                        MarginRight=2
                                    }
                                },
                            }
                        },
                        Header = "TabItem",
                    },
                }
            }, 0, 1);
            //if (DesignMode)
            //{
            //   Children.Add(new Button { Content = "设计模式" });
            // }
            var name = typeof(Window1).Assembly.GetName().Name;
            LoadStyleFile("res://" + name + "/Stylesheet1.css");
            //Effect = effect;
            //Storyboard storyboard = new Storyboard
            //{
            //    Timelines = {
            //        new Timeline(1)
            //        {
            //            KeyFrames = {
            //                new KeyFrame<float>
            //                {
            //                    Value=0,
            //                    Property="Effect.Value",
            //                    AnimateMode= AnimateMode.EaseOut,
            //                    Ease=new CubicEase{  }
            //                }
            //            }
            //        }
            //    }
            //};
            //storyboard.Start(this, TimeSpan.FromSeconds(1));
            //storyboard.Completed += (s, e) =>
            //{
            //    //System.Diagnostics.Debug.WriteLine("end"); 
            //    this.Effect = null;
            //};
        }
        CheckBox testCheckBox;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            testCheckBox = FindPresenterByName<CheckBox>(nameof(testCheckBox));
        }

        //protected override void OnClosing(ClosingEventArgs e)
        //{
        //    base.OnClosing(e);
        //    Model model = DataContext as Model;
        //    if (model != null)
        //    {
        //        model.SelectValue = 2;
        //    }
        //}

        private void Storyboard_Completed(object sender, StoryboardCompletedEventArgs e)
        {
            //DialogResult = 1;
            this.Close();
        }

        async void TestMessage()
        {
            await MessageBox.Show("test");

        }

        void ClickFormat(CpfObject sender, object eventArgs)
        {
            var textbox = this.FindPresenter<TextBox>().First(a => a.Name == "textbox");
            var text = textbox.Text;
            var str = "InitializeComponent";
            var start = text.IndexOf(str);
            List<Code> codes = new List<Code>();
            Code lastCode = new Code { CodeType = CodeType.Other, Length = str.Length };
            codes.Add(lastCode);
            List<CodeType> doubleType = new List<CodeType>();
            char lastChar = (char)0;
            var len = 0;
            for (int i = str.Length + start; i < text.Length; i++)
            {
                var ct = Code.GetCodeType(text[i]);
                if (lastChar == '\\')
                {
                    ct = CodeType.Other;
                }
                else if (lastChar == '/' && text[i] == '/' && (doubleType.Count == 0 || doubleType[doubleType.Count - 1] != CodeType.DoubleQuotationMarks) && lastCode.CodeType == CodeType.Other)//注释
                {
                    len++;
                    lastCode.Length++;
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        len++;
                        if (text[j] == '\n')
                        {
                            lastCode = new Code { CodeType = CodeType.SpaceWhite, Length = 1 };
                            codes.Add(lastCode);
                            lastChar = text[j];
                            i = j;
                            break;
                        }
                        lastCode.Length++;
                    }
                    continue;
                }
                else if (lastChar == '/' && text[i] == '*' && (doubleType.Count == 0 || doubleType[doubleType.Count - 1] != CodeType.DoubleQuotationMarks) && lastCode.CodeType == CodeType.Other)
                {/*  */
                    len++;
                    lastCode.Length++;
                    for (int j = i + 1; j < text.Length - 1; j++)
                    {
                        len++;
                        if (text[j] == '*' && text[j + 1] == '/')
                        {
                            len++;
                            lastCode.Length += 2;
                            lastChar = text[j + 1];
                            i = j + 1;
                            break;
                        }
                        lastCode.Length++;
                    }
                    continue;
                }
                if (doubleType.Count > 0)
                {
                    var last = doubleType[doubleType.Count - 1];
                    if ((last == CodeType.OpenBraceToken && ct == CodeType.CloseBraceToken) ||
                        (last == CodeType.DoubleQuotationMarks && ct == CodeType.DoubleQuotationMarks) ||
                        (last == CodeType.SingleQuotationMarks && ct == CodeType.SingleQuotationMarks) ||
                        (last == CodeType.OpenBracketToken && ct == CodeType.CloseBracketToken))
                    {
                        doubleType.RemoveAt(doubleType.Count - 1);
                    }
                    else
                    {
                        if ((last == CodeType.SingleQuotationMarks && ct != CodeType.SingleQuotationMarks) || (last == CodeType.DoubleQuotationMarks && ct != CodeType.DoubleQuotationMarks) ||
                            (last == CodeType.OpenBracketToken && ct == CodeType.Commas))
                        {
                            ct = CodeType.Other;
                        }
                        else
                        {
                            if (ct == CodeType.CloseBraceToken || ct == CodeType.DoubleQuotationMarks || ct == CodeType.OpenBraceToken || ct == CodeType.SingleQuotationMarks || ct == CodeType.OpenBracketToken || ct == CodeType.CloseBracketToken)
                            {
                                doubleType.Add(ct);
                            }
                        }
                    }
                }
                else
                {
                    if (ct == CodeType.OpenBraceToken)
                    {
                        doubleType.Add(CodeType.OpenBraceToken);
                    }
                }
                if (lastCode.CodeType == ct && ct != CodeType.CloseBraceToken && ct != CodeType.OpenBraceToken)
                {
                    lastCode.Length++;
                }
                else
                {
                    lastCode = new Code { CodeType = ct, Length = 1 };
                    codes.Add(lastCode);
                }
                len++;
                if (ct == CodeType.CloseBraceToken && doubleType.Count == 0)
                {
                    break;
                }
                lastChar = text[i];
            }
            var code = text.Substring(start, len + str.Length);
            var sb = new StringBuilder(code);
            var wlen = 8;
            var index = 0;
            for (int i = 0; i < codes.Count; i++)
            {
                var item = codes[i];
                if (item.CodeType == CodeType.OpenBraceToken)
                {
                    var s = 0;
                    var e = 0;
                    if ((i > 1 && !(codes[i - 1].CodeType == CodeType.SpaceWhite && (codes[i - 2].CodeType == CodeType.OpenBraceToken || codes[i - 2].CodeType == CodeType.Commas))) || i < 2)
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {//往前找空格和换行
                            var c = codes[j];
                            if (c.CodeType != CodeType.SpaceWhite)
                            {
                                break;
                            }
                            s += c.Length;
                        }
                    }
                    for (int j = i + 1; j < codes.Count; j++)
                    {//往后
                        var c = codes[j];
                        if (c.CodeType != CodeType.SpaceWhite)
                        {
                            i = j - 1;
                            break;
                        }
                        e += c.Length;
                    }
                    sb.Remove(index - s, e + s + 1);
                    index -= s;
                    //index++;
                    sb.Insert(index, '\n');
                    for (int j = 0; j < wlen; j++)
                    {
                        index++;
                        sb.Insert(index, ' ');
                    }
                    index++;
                    sb.Insert(index, '{');
                    if (sb.Length > index + 1 && sb[index + 1] != '{')
                    {
                        index++;
                        sb.Insert(index, '\n');
                    }

                    wlen += 4;
                    if (sb.Length > index + 1 && sb[index + 1] != '{')
                    {
                        for (int j = 0; j < wlen; j++)
                        {
                            index++;
                            sb.Insert(index, ' ');
                        }
                    }
                }
                else if (item.CodeType == CodeType.CloseBraceToken)
                {
                    wlen -= 4;
                    var s = 0;
                    var e = 0;
                    if ((i > 1 && !(codes[i - 1].CodeType == CodeType.SpaceWhite && (codes[i - 2].CodeType == CodeType.CloseBraceToken || codes[i - 2].CodeType == CodeType.Commas || codes[i - 2].CodeType == CodeType.Semicolon || codes[i - 2].CodeType == CodeType.OpenBraceToken))) || i < 2)
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {//往前找空格和换行
                            var c = codes[j];
                            if (c.CodeType != CodeType.SpaceWhite)
                            {
                                break;
                            }
                            s += c.Length;
                        }
                    }
                    for (int j = i + 1; j < codes.Count; j++)
                    {//往后
                        var c = codes[j];
                        if (c.CodeType != CodeType.SpaceWhite)
                        {
                            i = j - 1;
                            break;
                        }
                        e += c.Length;
                    }
                    sb.Remove(index - s, e + s + 1);
                    index -= s;
                    //index++;
                    sb.Insert(index, '\n');
                    for (int j = 0; j < wlen; j++)
                    {
                        index++;
                        sb.Insert(index, ' ');
                    }
                    index++;
                    sb.Insert(index, '}');
                    if (sb.Length > index + 1 && sb[index + 1] != ',' && sb[index + 1] != ';' && sb[index + 1] != ')' && sb[index + 1] != '}')
                    {
                        index++;
                        sb.Insert(index, '\n');
                        for (int j = 0; j < wlen; j++)
                        {
                            index++;
                            sb.Insert(index, ' ');
                        }
                    }
                }
                else if (item.CodeType == CodeType.Commas)
                {
                    var s = 0;
                    var e = 0;

                    if ((i > 1 && !(codes[i - 1].CodeType == CodeType.SpaceWhite && codes[i - 2].CodeType == CodeType.CloseBraceToken)) || i < 2)
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {//往前找空格和换行
                            var c = codes[j];
                            if (c.CodeType != CodeType.SpaceWhite)
                            {
                                break;
                            }
                            s += c.Length;
                        }
                    }

                    for (int j = i + 1; j < codes.Count; j++)
                    {//往后
                        var c = codes[j];
                        if (c.CodeType != CodeType.SpaceWhite)
                        {
                            i = j - 1;
                            break;
                        }
                        e += c.Length;
                    }
                    sb.Remove(index - s, e + s + 1);
                    index -= s;
                    sb.Insert(index, ',');
                    if (sb.Length > index + 1 && sb[index + 1] != '}' && sb[index + 1] != '{')
                    {
                        index++;
                        sb.Insert(index, '\n');
                        for (int j = 0; j < wlen; j++)
                        {
                            index++;
                            sb.Insert(index, ' ');
                        }
                    }
                }
                else if (item.CodeType == CodeType.Semicolon)
                {
                    var s = 0;
                    var e = 0;
                    if ((i > 1 && !(codes[i - 1].CodeType == CodeType.SpaceWhite && codes[i - 2].CodeType == CodeType.CloseBraceToken)) || i < 2)
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {//往前找空格和换行
                            var c = codes[j];
                            if (c.CodeType != CodeType.SpaceWhite)
                            {
                                break;
                            }
                            s += c.Length;
                        }
                    }

                    for (int j = i + 1; j < codes.Count; j++)
                    {//往后
                        var c = codes[j];
                        if (c.CodeType != CodeType.SpaceWhite)
                        {
                            i = j - 1;
                            break;
                        }
                        e += c.Length;
                    }
                    sb.Remove(index - s, e + s + 1);
                    index -= s;
                    sb.Insert(index, ';');
                    if (sb.Length > index + 1 && sb[index + 1] != '}')
                    {
                        index++;
                        sb.Insert(index, '\n');
                        for (int j = 0; j < wlen; j++)
                        {
                            index++;
                            sb.Insert(index, ' ');
                        }
                    }
                }


                index += item.Length;
            }
            textbox.Text = sb.ToString();
        }
        void testClick(CpfObject obj, RoutedEventArgs eventArgs)
        {

        }
        Storyboard storyboard1;
        RotateTransform rotateTransform = new RotateTransform();
        void PlayAnimation(CpfObject obj, RoutedEventArgs eventArgs)
        {
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
                                new KeyFrame<float>{ Property=nameof(RotateTransform.Angle), Value=180, AnimateMode= AnimateMode.EaseIn, Ease=new PowerEase() },
                            }
                        },
                        new Timeline(1)
                        {
                            KeyFrames =
                            {
                                new KeyFrame<float>{ Property=nameof(RotateTransform.Angle), Value=360, AnimateMode= AnimateMode.EaseInOut, Ease=new QuadraticEase() },
                            }
                        },
                    }
                };
            }
            foreach (var item in (obj as UIElement).Parent.GetChildren().Where(a => a is SVG))
            {
                (item as UIElement).RenderTransform = rotateTransform;
            }
            storyboard1.Start(rotateTransform, TimeSpan.FromSeconds(1), 0);

        }

        Stopwatch stopwatch = new Stopwatch();
        int frameCount;
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            frameCount++;
            if (stopwatch.ElapsedMilliseconds >= 500)
            {
                Debug.WriteLine("帧数：" + frameCount * 1000 / stopwatch.ElapsedMilliseconds);
                stopwatch.Restart();
                frameCount = 0;
            }
            else if (!stopwatch.IsRunning)
            {
                stopwatch.Start();
            }
        }
        void AddRowClick(CpfObject obj, RoutedEventArgs eventArgs)
        {
            var model = DataContext as Model;
            //var row = model.Data.GetDataTable().NewRow();
            //for (int j = 0; j < 9; j++)
            //{
            //    if (j != 1)
            //    {
            //        row[j] = 1;
            //    }
            //}
            //row[0] = 1 % 3;
            //row[1] = true;
            //model.Data.GetDataTable().Rows.Add(row);


            //var data = new Collection<(string, bool, string, int, string, string, string, string, string)>();
            Random random = new Random();
            //for (int i = 0; i < 1; i++)
            //{
            var index = random.Next(100);
            var row = (index.ToString(), index % 3 == 1, index.ToString(), index, index.ToString(), index.ToString(), index.ToString(), index.ToString(), index.ToString());
            //    data.Add(row);
            //}
            (model.Data as Collection<(string, bool, string, int, string, string, string, string, string)>).Insert(0, row);
        }
        void ClearData(CpfObject obj, RoutedEventArgs eventArgs)
        {
            var model = DataContext as Model;
            //var row = model.Data.GetDataTable().NewRow();
            //for (int j = 0; j < 9; j++)
            //{
            //    if (j != 1)
            //    {
            //        row[j] = 1;
            //    }
            //}
            //row[0] = 1 % 3;
            //row[1] = true;
            //model.Data.GetDataTable().Rows.Add(row);
            model.Data.Clear();
        }

        void MenuItemClick(CpfObject item, object e)
        {
            //Debug.WriteLine((item as MenuItem).Header);
            //new Window { Width = 100, Height = 100, Background = "#f00" }.ShowDialogSync(this);
            MessageBox.ShowSync("test");
        }
    }
    enum CodeType
    {
        Other = 'A',
        OpenBraceToken = '{',
        CloseBraceToken = '}',
        SpaceWhite = ' ',//\n\t\r
        SingleQuotationMarks = '\'',
        DoubleQuotationMarks = '"',
        //Wrap = '\n',
        Commas = ',',
        Semicolon = ';',
        OpenBracketToken = '(',
        CloseBracketToken = ')',
        //StartComment,// /*
        //EndComment,// */
    }

    class Code
    {
        public static CodeType GetCodeType(char c)
        {
            switch (c)
            {
                case ',':
                    return CodeType.Commas;
                case ';':
                    return CodeType.Semicolon;
                case '"':
                    return CodeType.DoubleQuotationMarks;
                case '\'':
                    return CodeType.SingleQuotationMarks;
                case '\n':
                //return CodeType.Wrap;
                case '\r':
                //return CodeType.Wrap;
                case '\t':
                case ' ':
                    return CodeType.SpaceWhite;
                case '}':
                    return CodeType.CloseBraceToken;
                case '{':
                    return CodeType.OpenBraceToken;
                case ')':
                    return CodeType.CloseBracketToken;
                case '(':
                    return CodeType.OpenBracketToken;
                default:
                    return CodeType.Other;
            }
        }
        public CodeType CodeType { get; set; }
        public int Length { get; set; }

        public override string ToString()
        {
            return CodeType.ToString() + " " + Length;
        }
    }
}
