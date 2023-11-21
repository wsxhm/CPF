using CPF;
using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using CPF.Shapes;
using CPF.Styling;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace xss_pro.Base
{
    public class BaseWindow : Window
    {
        private Panel _headPanel;
        private Panel _contentPanel;
        private Panel _controlBoxPanel;
        private UIElement _closeBoxPanel;
        private UIElement _maximizeBoxPanel;
        private UIElement _minimizeBoxPanel;
        private UIElement _titleBlock;
        [PropertyMetadata(typeof(ViewFill), "#fff")]
        public ViewFill WindowBackground { get { return GetValue<ViewFill>(); } set { SetValue(value); } }
        public bool ControlBoxToLeft { get { return GetValue<bool>(); } set { SetValue(value); } }
        [PropertyMetadata(true)]
        public bool ShowTitle { get { return GetValue<bool>(); } set { SetValue(value); } }
        [PropertyMetadata(typeof(ViewFill), "#2d2d2d")]
        public ViewFill TitleFontColor { get { return GetValue<ViewFill>(); } set { SetValue(value); } }
        [PropertyMetadata(FontStyles.Bold)]
        public FontStyles TitleFontStyle { get { return GetValue<FontStyles>(); } set { SetValue(value); } }
        [PropertyMetadata(true)]
        public bool ShowCloseBox { get { return GetValue<bool>(); } set { SetValue(value); } }
        [PropertyMetadata(true)]
        public bool ShowMaximizeBox { get { return GetValue<bool>(); } set { SetValue(value); } }
        [PropertyMetadata(true)]
        public bool ShowMinimizeBox { get { return GetValue<bool>(); } set { SetValue(value); } }
        [PropertyMetadata(typeof(ViewFill), "#2b2b2b")]
        public ViewFill IconStrokekFill { get { return GetValue<ViewFill>(); } set { SetValue(value); } }
        [PropertyMetadata(5)]
        public int IconStrokeWidth { get { return GetValue<int>(); } set { SetValue(value); } }
        [PropertyMetadata(14)]
        public int IconBaseSize { get { return GetValue<int>(); } set { SetValue(value); } }
        [PropertyMetadata(13)]
        public int IconMarginTopDown { get { return GetValue<int>(); } set { SetValue(value); } }
        [PropertyMetadata(13)]
        public int IconMarginLeftRight { get { return GetValue<int>(); } set { SetValue(value); } }
        [PropertyMetadata(TextAlignment.Left)]
        public TextAlignment TitleAlignment { get { return GetValue<TextAlignment>(); } set { SetValue(value); } }
        public Collection<Point> ReducePoints { get { return GetValue<Collection<Point>>(); } set { SetValue(value); } }
        [Computed(nameof(IconMarginTopDown), nameof(IconMarginLeftRight))]
        public ThicknessField IconMargin
        {
            get
            {
                return new ThicknessField(IconMarginLeftRight, IconMarginTopDown, IconMarginLeftRight, IconMarginTopDown);
            }
        }
        [Computed(nameof(ShowControlBox), nameof(ControlBoxHeight))]
        public int PanelMarginTop { get { return ShowControlBox ? ControlBoxHeight : 0; } }
        [Computed(nameof(Title), nameof(ShowTitle), nameof(ShowCloseBox), nameof(ShowMaximizeBox), nameof(ShowMinimizeBox))]
        public bool ShowControlBox { get { return (ShowTitle & !string.IsNullOrEmpty(Title)) | ShowCloseBox | ShowMaximizeBox | ShowMinimizeBox; } }
        [Computed(nameof(IconMarginTopDown), nameof(IconBaseSize))]
        public int ControlBoxHeight { get { return IconMarginTopDown * 2 + IconBaseSize; } }
        [Computed(nameof(IconMarginLeftRight), nameof(IconBaseSize))]
        public int ControlBoxWidth { get { return IconMarginLeftRight * 2 + IconBaseSize; } }
        [Computed(nameof(ControlBoxHeight))]
        public float TitleFontSize { get { return ControlBoxHeight / 2; } }
        [Computed(nameof(TitleFontSize), nameof(TitleAlignment) ,nameof(IconMarginLeftRight))]
        public ThicknessField TitleMargin
        {
            get
            {
                switch (TitleAlignment)
                {
                    default:
                    case TextAlignment.Left:
                        return new ThicknessField(IconMarginLeftRight, TitleFontSize / 2, FloatField.Auto, FloatField.Auto);
                    case TextAlignment.Center:
                        return new ThicknessField(FloatField.Auto, TitleFontSize / 2, FloatField.Auto, FloatField.Auto);
                    case TextAlignment.Right:
                        return new ThicknessField(FloatField.Auto, TitleFontSize / 2, IconMarginLeftRight, FloatField.Auto);
                }
            }
        }
        [Computed(nameof(ControlBoxToLeft))]
        public ThicknessField ControlBoxMargin { get { return ControlBoxToLeft ? new ThicknessField(0, 0, FloatField.Auto, FloatField.Auto) : new ThicknessField(FloatField.Auto, 0, 0, FloatField.Auto);  } }
        [Computed(nameof(IconBaseSize), nameof(IconStrokeWidth))]
        public int ReduceBoxOffsetDistance
        {
            get
            {
                int v = IconBaseSize / 10 + IconStrokeWidth; 
                return v > 3 ? v : 3;
            }
        }
        [Computed(nameof(IconBaseSize), nameof(IconStrokeWidth))]
        public int ReduceBoxSize
        {
            get
            {
                return IconBaseSize - IconStrokeWidth - (IconBaseSize / 10);
            }
        }
        public UIElementCollection Content { get { return ContentPanel.Children; } }
        public Panel HeadPanel
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _headPanel ?? (_headPanel = CreateHeadPanel());
            }
            set
            {
                if (_headPanel != null)
                {
                    _headPanel.Attacheds.Add(Grid.RowIndex, 0);
                }
                _headPanel = value;
            }
        }
        public Panel ContentPanel
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _contentPanel ?? (_contentPanel = CreateContentPanel());
            }
            set
            {
                if (_contentPanel != null)
                {
                    _contentPanel.Attacheds.Add(Grid.RowIndex, 1);
                }
                _contentPanel = value;
            }
        }
        public Panel ControlBoxPanel
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _controlBoxPanel ?? (_controlBoxPanel = CreateControlBox());
            }
            set
            {
                if (_controlBoxPanel != null)
                {
                    _controlBoxPanel.Attacheds.Add(Grid.RowIndex, 0);
                }
                _controlBoxPanel = value;
            }
        }
        public UIElement CloseBox
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _closeBoxPanel ?? (_closeBoxPanel = CreateCloseBox());
            }
            set
            {
                _closeBoxPanel = value;
            }
        }
        public UIElement MaximizeBox
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _maximizeBoxPanel ?? (_maximizeBoxPanel = CreateMaximizeBox());
            }
            set
            {
                _maximizeBoxPanel = value;
            }
        }
        public UIElement MinimizeBox
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _minimizeBoxPanel ?? (_minimizeBoxPanel = CreateMinimizeBox());
            }
            set
            {
                _minimizeBoxPanel = value;
            }
        }
        public UIElement TitleBlock
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _titleBlock ?? (_titleBlock = CreateTitleTextBlock());
            }
            set
            {
                if (_titleBlock != null)
                {
                    _titleBlock.Attacheds.Add(Grid.RowIndex, 0);
                }
                _titleBlock = value;
            }
        }

        public Panel CreateCloseBox()
        {
            return new Panel
            {
                Name = "CloseBox",
                ToolTip = "关闭",
                Height = "100%",
                Children =
                {
                    new Line
                    {
                        StartPoint = new Point(1, 1),
                        IsAntiAlias = true,
                        Bindings =
                        {
                            {
                                nameof(Line.StrokeStyle),
                                nameof(IconStrokeWidth),
                                this,
                                BindingMode.OneWay,
                                (x) => new Stroke((int)x)
                            },
                            {
                                nameof(Line.Margin),
                                nameof(IconMargin),
                                this,
                                BindingMode.OneWay
                            },
                            {
                                nameof(Line.EndPoint),
                                nameof(IconBaseSize),
                                this,
                                BindingMode.OneWay,
                                (x) =>
                                {
                                    int h = (int)x;
                                    return new Point(h, h - 1);
                                }
                            },
                            {
                                nameof(Line.StrokeFill),
                                nameof(IconStrokekFill),
                                this,
                                BindingMode.OneWay
                            }
                        }
                    },
                    new Line
                    {
                        IsAntiAlias = true,
                        Bindings =
                        {
                            {
                                nameof(Line.StrokeStyle),
                                nameof(IconStrokeWidth),
                                this,
                                BindingMode.OneWay,
                                (x) => new Stroke((int)x)
                            },
                            {
                                nameof(Line.Margin),
                                nameof(IconMargin),
                                this,
                                BindingMode.OneWay
                            },
                            {
                                nameof(Line.StartPoint),
                                nameof(IconBaseSize),
                                this,
                                BindingMode.OneWay,
                                (x) =>
                                {
                                    int w = (int)x;
                                    return new Point(w, 1);
                                }
                            },
                            {
                                nameof(Line.EndPoint),
                                nameof(IconBaseSize),
                                this,
                                BindingMode.OneWay,
                                (x) =>
                                {
                                    int w = (int)x;
                                    return new Point(1, w - 1);
                                }
                            },
                            {
                                nameof(Line.StrokeFill),
                                nameof(IconStrokekFill),
                                this,
                                BindingMode.OneWay
                            }
                        }
                    },
                },
                Commands =
                {
                    {
                        nameof(Panel.MouseUp),
                        (s, e) =>
                        {
                            if (e is MouseButtonEventArgs mouseButtonEventArgs) mouseButtonEventArgs.Handled = true;
                            if (s is UIElement element && element.IsMouseOver) Close();
                        }
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
                                "#e81123"
                            }
                        }
                    }
                },
                Bindings =
                {
                    {
                        nameof(Panel.Visibility),
                        nameof(ShowCloseBox),
                        this,
                        BindingMode.OneWay,
                        (x) => ((bool)x) ? Visibility.Visible : Visibility.Collapsed
                    },
                    {
                        nameof(Panel.Width),
                        nameof(ControlBoxWidth),
                        this,
                        BindingMode.OneWay
                    }
                }
            };
        }
        public Panel CreateMaximizeBox()
        {
            Polyline polyline = new Polyline
            {
                MarginTop = 0,
                IsAntiAlias = true,
                Bindings =
                {
                    {
                        nameof(Polyline.StrokeStyle),
                        nameof(IconStrokeWidth),
                        this,
                        BindingMode.OneWay,
                        (x) => new Stroke((int)x)
                    },
                    {
                        nameof(Polyline.MarginLeft),
                        nameof(ReduceBoxOffsetDistance),
                        this,
                        BindingMode.OneWay
                    },
                    {
                        nameof(Polyline.Size),
                        nameof(ReduceBoxSize),
                        this,
                        BindingMode.OneWay,
                        (x) =>
                        {
                            return new SizeField((int)x, (int)x);
                        }
                    },
                    {
                        nameof(Polyline.Visibility),
                        nameof(WindowState),
                        this,
                        BindingMode.OneWay,
                        (x) =>
                        {
                            WindowState windowState = (WindowState)x;
                            return windowState != WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
                        }
                    },
                    {
                        nameof(Polyline.StrokeFill),
                        nameof(IconStrokekFill),
                        this,
                        BindingMode.OneWay
                    }
                }
            };
            ReducePoints = polyline.Points;
            polyline.Points.AddRange(GetReducePoints());
            return new Panel
            {
                Name = "MaximizeBox",
                Height = "100%",
                Children =
                {
                    new Rectangle
                    {
                        ToolTip = "最大化",
                        IsAntiAlias = true,
                        Bindings =
                        {
                            {
                                nameof(Rectangle.StrokeStyle),
                                nameof(IconStrokeWidth),
                                this,
                                BindingMode.OneWay,
                                (x) => new Stroke((int)x)
                            },
                            {
                                nameof(Rectangle.Margin),
                                nameof(IconMargin),
                                this,
                                BindingMode.OneWay
                            },
                            {
                                nameof(Rectangle.Size),
                                nameof(IconBaseSize),
                                this,
                                BindingMode.OneWay,
                                (x) =>
                                {
                                    int h = (int)x;
                                    return new SizeField(h, h);
                                }
                            },
                            {
                                nameof(Rectangle.Visibility),
                                nameof(WindowState),
                                this,
                                BindingMode.OneWay,
                                (x) =>
                                {
                                    WindowState windowState = (WindowState)x;
                                    return windowState == WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
                                }
                            },
                            {
                                nameof(Rectangle.StrokeFill),
                                nameof(IconStrokekFill),
                                this,
                                BindingMode.OneWay
                            }
                        }
                    },
                    new Panel
                    {
                        ToolTip = "还原",
                        Children =
                        {
                            new Rectangle
                            {
                                MarginLeft = 0,
                                IsAntiAlias = true,
                                Bindings =
                                {
                                    {
                                        nameof(Rectangle.StrokeStyle),
                                        nameof(IconStrokeWidth),
                                        this,
                                        BindingMode.OneWay,
                                        (x) => new Stroke((int)x)
                                    },
                                    {
                                        nameof(Rectangle.MarginTop),
                                        nameof(ReduceBoxOffsetDistance),
                                        this,
                                        BindingMode.OneWay
                                    },
                                    {
                                        nameof(Rectangle.Size),
                                        nameof(ReduceBoxSize),
                                        this,
                                        BindingMode.OneWay,
                                        (x) =>
                                        {
                                            return new SizeField((int)x, (int)x);
                                        }
                                    },
                                    {
                                        nameof(Rectangle.Visibility),
                                        nameof(WindowState),
                                        this,
                                        BindingMode.OneWay,
                                        (x) =>
                                        {
                                            WindowState windowState = (WindowState)x;
                                            return windowState != WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
                                        }
                                    },
                                    {
                                        nameof(Rectangle.StrokeFill),
                                        nameof(IconStrokekFill),
                                        this,
                                        BindingMode.OneWay
                                    }
                                }
                            },
                            polyline
                        },
                        Bindings =
                        {
                            {
                                nameof(Rectangle.Margin),
                                nameof(IconMargin),
                                this,
                                BindingMode.OneWay
                            },
                            {
                                nameof(Rectangle.Size),
                                nameof(IconBaseSize),
                                this,
                                BindingMode.OneWay,
                                (x) =>
                                {
                                    int h = (int)x;
                                    return new SizeField(h, h);
                                }
                            },
                            {
                                nameof(Rectangle.Visibility),
                                nameof(WindowState),
                                this,
                                BindingMode.OneWay,
                                (x) =>
                                {
                                    WindowState windowState = (WindowState)x;
                                    return windowState != WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
                                }
                            },
                        }
                    },
                },
                Commands =
                {
                    {
                        nameof(Panel.MouseUp),
                        (s, e) =>
                        {
                            if (e is MouseButtonEventArgs mouseButtonEventArgs) mouseButtonEventArgs.Handled = true;
                            if (s is UIElement element && element.IsMouseOver) 
                            {
                                 WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                            }
                        }
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
                                "#ebeced"
                            }
                        }
                    }
                },
                Bindings =
                {
                    {
                        nameof(Panel.Visibility),
                        nameof(ShowMaximizeBox),
                        this,
                        BindingMode.OneWay,
                        (x) => ((bool)x) ? Visibility.Visible : Visibility.Collapsed
                    },
                    {
                        nameof(Panel.Width),
                        nameof(ControlBoxWidth),
                        this,
                        BindingMode.OneWay
                    }
                }
            };
        }
        public Panel CreateMinimizeBox()
        {
            return new Panel
            {
                Name = "MinimizeBox",
                ToolTip = "最小化",
                Height = "100%",
                Children =
                {
                    new Line
                    {
                        IsAntiAlias = true,
                        Bindings =
                        {
                            {
                                nameof(Line.StrokeStyle),
                                nameof(IconStrokeWidth),
                                this,
                                BindingMode.OneWay,
                                (x) => new Stroke((int)x)
                            },
                            {
                                nameof(Line.Margin),
                                nameof(IconMargin),
                                this,
                                BindingMode.OneWay
                            },
                            {
                                nameof(Line.StartPoint),
                                nameof(IconBaseSize),
                                this,
                                BindingMode.OneWay,
                                (x) => new Point(1, (int)x / 2)
                            },
                            {
                                nameof(Line.EndPoint),
                                nameof(IconBaseSize),
                                this,
                                BindingMode.OneWay,
                                (x) => new Point((int)x, (int)x / 2)
                            },
                            {
                                nameof(Line.StrokeFill),
                                nameof(IconStrokekFill),
                                this,
                                BindingMode.OneWay
                            }
                        }
                    }
                },
                Commands =
                {
                    {
                        nameof(Button.MouseDown),
                        (s, e) =>
                        {
                            if (e is MouseButtonEventArgs mouseButtonEventArgs) mouseButtonEventArgs.Handled = true;
                            if (s is UIElement element && element.IsMouseOver) WindowState = WindowState.Minimized;
                        }
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
                                "#ebeced"
                            }
                        }
                    }
                },
                Bindings =
                {
                    {
                        nameof(Panel.Visibility),
                        nameof(ShowMinimizeBox),
                        this,
                        BindingMode.OneWay,
                        (x) => ((bool)x) ? Visibility.Visible : Visibility.Collapsed
                    },
                    {
                        nameof(Panel.Width),
                        nameof(ControlBoxWidth),
                        this,
                        BindingMode.OneWay
                    }
                }
            };
        }
        public Panel CreateHeadPanel()
        {
            return new Panel
            {
                ClipToBounds = true,
                CornerRadius = "5",
                Attacheds =
                {
                    {
                        Grid.RowIndex,
                        0
                    }
                },
                Size = SizeField.Fill,
                Children =
                {
                    TitleBlock,
                    ControlBoxPanel
                },
                Commands =
                {
                    {
                        nameof(Panel.MouseDown),
                        (o,e) =>
                        {
                            if (!ControlBoxPanel.IsMouseOver) DragMove();
                        }
                    },
                    {
                        nameof(Panel.DoubleClick),
                        (o,e) =>
                        {
                            if (!ControlBoxPanel.IsMouseOver)
                            {
                                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                            }
                        }
                    }
                },
                Bindings =
                {
                    {
                        nameof(Panel.CornerRadius),
                        nameof(CornerRadius),
                        this,
                        BindingMode.OneWay,
                        (x) =>
                        {
                            CornerRadius cornerRadius = (CornerRadius)x;
                            return new CornerRadius(CornerRadius.TopLeft ,cornerRadius.TopRight, 0,0);
                        }
                    }
                },
            };
        }
        public Panel CreateContentPanel()
        {
            return new Panel
            {
                Attacheds =
                {
                    {
                        Grid.RowIndex,
                        1
                    }
                },
                Bindings =
                {
                    {
                        nameof(Panel.CornerRadius),
                        nameof(CornerRadius),
                        this,
                        BindingMode.OneWay,
                        (x) =>
                        {
                            CornerRadius cornerRadius = (CornerRadius)x;
                            return new CornerRadius(0,0, cornerRadius.BottomLeft, cornerRadius.BottomRight);
                        }
                    }
                },
                Size = SizeField.Fill
            };
        }
        public StackPanel CreateControlBox()
        {
            StackPanel panel = new StackPanel
            {
                MarginRight = 0,
                Orientation = Orientation.Horizontal,
                MarginTop = 0,
                Children =
                {
                    ControlBoxToLeft ? CloseBox : MinimizeBox,
                    MaximizeBox,
                    ControlBoxToLeft ? MinimizeBox : CloseBox,
                },
                Bindings =
                {
                    {
                        nameof(StackPanel.Visibility),
                        nameof(ShowControlBox),
                        this,
                        BindingMode.OneWay,
                        (x) => (bool)x ? Visibility.Visible : Visibility.Collapsed
                    },
                    {
                        nameof(StackPanel.Height),
                        nameof(ControlBoxHeight),
                        this,
                        BindingMode.OneWay
                    }
                    ,
                    {
                        nameof(StackPanel.Margin),
                        nameof(ControlBoxMargin),
                        this,
                        BindingMode.OneWay
                    }
                },
                Attacheds =
                {
                    {
                        Grid.RowIndex,
                        0
                    }
                }
            };
            UpdateControlBoxCornerRadius(panel);
            return panel;
        }
        public TextBlock CreateTitleTextBlock()
        {
            return new TextBlock()
            {
                Attacheds =
                {
                    {
                        Grid.RowIndex,
                        0
                    }
                },
                Bindings =
                {
                    {
                        nameof(TextBlock.Visibility),
                        nameof(ShowTitle),
                        this,
                        BindingMode.OneWay,
                        (x) => (bool)x ? Visibility.Visible : Visibility.Collapsed
                    },
                    {
                        nameof(TextBlock.Text),
                        nameof(Title),
                        this,
                        BindingMode.OneWay
                    },
                    {
                        nameof(TextBlock.FontSize),
                        nameof(TitleFontSize),
                        this,
                        BindingMode.OneWay
                    },
                    {
                        nameof(TextBlock.FontStyle),
                        nameof(TitleFontStyle),
                        this,
                        BindingMode.OneWay
                    },
                    {
                        nameof(TextBlock.Margin),
                        nameof(TitleMargin),
                        this,
                        BindingMode.OneWay
                    }
                }
            };
        }
        protected override void InitializeComponent()
        {
#if DesignMode
            Width = 600;
            Height = 500;
            IconBaseSize = 13;
#endif
            Background = null;
            //设置窗体阴影
            var frame = Children.Add(new Border
            {
                Size = SizeField.Fill,
                BorderType = BorderType.BorderStroke,
                BorderStroke = new Stroke(0),
                Bindings =
                {
                    {
                        nameof(Border.CornerRadius),
                        nameof(CornerRadius),
                        this,
                        BindingMode.OneWay
                    },
                    {
                        nameof(Border.ShadowBlur),
                        nameof(WindowState),
                        this,
                        BindingMode.OneWay,
                        (x) => (WindowState)x == WindowState.Maximized ? 0 : 5
                    }
                }
            });
            frame.Child = new Grid
            {
                ClipToBounds = true,
                Size = SizeField.Fill,
                Bindings =
                {
                    {
                       nameof(Grid.CornerRadius),
                       nameof(CornerRadius),
                       this,
                       BindingMode.OneWay
                    },
                    {
                        nameof(Grid.Background),
                        nameof(WindowBackground),
                        this,
                        BindingMode.OneWay
                    }
                },
                RowDefinitions =
                {
                    new RowDefinition()
                    {
                        Bindings =
                        {
                            {
                                nameof(RowDefinition.Height),
                                nameof(PanelMarginTop),
                                this,
                                BindingMode.OneWay,
                                (x) => new GridLength((int)x, GridUnitType.Default)
                            }
                        }
                    },
                    new RowDefinition()
                    {
                        Height = GridLength.Star
                    }
                },
                Children =
                {
                    HeadPanel,
                    ContentPanel
                }
            };
            CanResize = true;
            //base.InitializeComponent();
        }
        [PropertyChanged(nameof(ControlBoxToLeft))]
        void RegisterControlBoxToLeft(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (_controlBoxPanel == null)
                return;
            bool newFlag = (bool)newValue;
            bool oldFlag = (bool)oldValue;
            if (newFlag ^ oldFlag)
            {
                ControlBoxComparer comparer = new ControlBoxComparer(new[] { MinimizeBox , MaximizeBox , CloseBox }, newFlag);
                _controlBoxPanel.Children.Sort(comparer);
            }
        }
        [PropertyChanged(nameof(ControlBoxToLeft))]
        [PropertyChanged(nameof(ShowCloseBox))]
        [PropertyChanged(nameof(ShowMinimizeBox))]
        [PropertyChanged(nameof(ShowMaximizeBox))]
        [PropertyChanged(nameof(CornerRadius))]
        void RegisterControlBoxCornerRadius(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (_controlBoxPanel == default)
                return;
            if (CornerRadius == default)
                return;
            UpdateControlBoxCornerRadius(_controlBoxPanel);
        }
        public void UpdateControlBoxCornerRadius(Panel controlBoxPanel)
        {
            if (controlBoxPanel == default)
                return;
            if (CornerRadius == default)
                return;
            Panel pFirst = null;
            Panel pLast = null;
            foreach (UIElement element in controlBoxPanel.Children)
            {
                if (element is Panel panel)
                {
                    if (panel == MinimizeBox && !ShowMinimizeBox) continue;
                    if (panel == MaximizeBox && !ShowMaximizeBox) continue;
                    if (panel == CloseBox && !ShowCloseBox) continue;
                    if (pFirst == null) pFirst = panel;
                    pLast = panel;
                    panel.CornerRadius = default;
                    panel.Background = null;
                }
            }
            if (pLast == null)
                return;
            if (ControlBoxToLeft)
            {
                pFirst.CornerRadius = new CornerRadius(CornerRadius.TopLeft, 0, 0, 0);
            }
            else
            {
                pLast.CornerRadius = new CornerRadius(0, CornerRadius.TopRight, 0, 0);
            }
        }
        [PropertyChanged(nameof(IconBaseSize))]
        [PropertyChanged(nameof(IconStrokeWidth))]
        void RegisterReducePoints(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (ReducePoints == null)
                return;
            ReducePoints.Clear();
            ReducePoints.AddRange(GetReducePoints());
        }
        public IEnumerable<Point> GetReducePoints()
        {
            return new PolylineBuilder(new Point(-IconStrokeWidth / 2, ReduceBoxOffsetDistance))
                .GoUp(ReduceBoxOffsetDistance - IconStrokeWidth)
                .GoRight(ReduceBoxSize - IconStrokeWidth)
                .GoDown(ReduceBoxSize - IconStrokeWidth)
                .GoLeft(ReduceBoxOffsetDistance)
                .Points;
        }
    }
}
