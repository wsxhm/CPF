using CPF.Controls;
using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Toolkit.Controls
{
    internal class MdiFrame : Control
    {
        public MdiFrame()
        {
            
        }

        protected override void InitializeComponent()
        {
            this.Children.Add(new Grid
            {
                Size = SizeField.Fill,
                RowDefinitions =
                {
                    new RowDefinition{ Height = 35 },
                    new RowDefinition{ },
                },
                Children =
                {
                    new Thumb
                    {
                        Size = SizeField.Fill,
                        Background = new LinearGradientFill
                        {
                            EndPoint = "0,100%",
                            GradientStops =
                            {
                                new GradientStop("152,180,208",0),
                                new GradientStop("181,206,231",1),
                            }
                        },
                        Child = new Panel
                        {
                            Size = SizeField.Fill,
                            Children =
                            {
                                new StackPanel
                                {
                                    Orientation = Orientation.Horizontal,
                                    MarginLeft = 0,
                                    Children =
                                    {
                                        new TextBlock
                                        {
                                            [nameof(TextBlock.Text)] = new BindingDescribe(this,nameof(this.Title),BindingMode.OneWay),
                                            FontSize = 14,
                                            MarginLeft = 10,
                                        },
                                    }
                                },
                                new StackPanel
                                {
                                    Orientation = Orientation.Horizontal,
                                    MarginRight = 0,
                                    Height = "100%",
                                    Children =
                                    {
                                        new SystemButton
                                        {
                                            Name = "min",
                                            Size = new SizeField(30,"100%"),
                                            Content = new Line
                                            {
                                                MarginTop = 5,
                                                MarginLeft = "auto",
                                                StartPoint = new Point(1,13),
                                                EndPoint = new Point(14,13),
                                                StrokeStyle = "2",
                                                IsAntiAlias = true,
                                                StrokeFill = "black"
                                            }
                                        },
                                        new Panel
                                        {
                                            Height = "100%",
                                            Bindings =
                                            {
                                                {
                                                    nameof(Visibility),
                                                    nameof(MaximizeBox),
                                                    this,
                                                    BindingMode.OneWay,
                                                    a => (bool)a ? Visibility.Visible : Visibility.Collapsed
                                                }
                                            },
                                            Children =
                                            {
                                                new SystemButton
                                                {
                                                    Name = "max",
                                                    Size = new SizeField(30,"100%"),
                                                    Content = new Rectangle
                                                    {
                                                        Size = new SizeField(14,12),
                                                        MarginTop = 10,
                                                        StrokeStyle = "2",
                                                    },
                                                    Commands =
                                                    {
                                                        { nameof(Button.Click),(s,e) => this.WindowState = WindowState.Maximized }
                                                    },
                                                    Bindings =
                                                    {
                                                         {
                                                            nameof(Border.Visibility),
                                                            nameof(this.WindowState),
                                                            this,
                                                            BindingMode.OneWay,
                                                            a => (WindowState)a == WindowState.Maximized || (WindowState)a == WindowState.FullScreen ? Visibility.Collapsed : Visibility.Visible
                                                         }
                                                    }
                                                },
                                                new SystemButton
                                                {
                                                    Name = "nor",
                                                    Visibility = Visibility.Collapsed,
                                                    Size = new SizeField(30,"100%"),
                                                    Content = new Panel
                                                    {
                                                        Size = SizeField.Fill,
                                                        Children =
                                                        {
                                                            new Rectangle
                                                            {
                                                                MarginTop = 15,
                                                                MarginLeft =8,
                                                                Size = new SizeField(11,8),
                                                                StrokeStyle = "1.5",
                                                            },
                                                            new Polyline
                                                            {
                                                                MarginTop =11,
                                                                MarginLeft = 12,
                                                                Points =
                                                                {
                                                                    new Point(0,3),
                                                                    new Point(0,0),
                                                                    new Point(9,0),
                                                                    new Point(9,7),
                                                                    new Point(6,7)
                                                                },
                                                                StrokeStyle = "2"
                                                            }
                                                        }
                                                    },
                                                    Commands =
                                                    {
                                                        { nameof(Button.Click),(s,e) => this.WindowState = WindowState.Normal }
                                                    },
                                                    Bindings =
                                                    {
                                                        {
                                                            nameof(Border.Visibility),
                                                            nameof(Window.WindowState),
                                                            this,
                                                            BindingMode.OneWay,
                                                            a => (WindowState)a == WindowState.Normal ? Visibility.Collapsed : Visibility.Visible
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        new SystemButton
                                        {
                                            Name = "close",
                                            Size = new SizeField(30,"100%"),
                                            Content = new Panel
                                            {
                                                Size = SizeField.Fill,
                                                Children =
                                                {
                                                    new Line
                                                    {
                                                        MarginTop=10,
                                                        MarginLeft=8,
                                                        StartPoint = new Point(1, 1),
                                                        EndPoint = new Point(14, 13),
                                                        StrokeStyle = "2",
                                                        IsAntiAlias=true,
                                                    },
                                                    new Line
                                                    {
                                                        MarginTop=10,
                                                        MarginLeft=8,
                                                        StartPoint = new Point(14, 1),
                                                        EndPoint = new Point(1, 13),
                                                        StrokeStyle = "2",
                                                        IsAntiAlias=true,
                                                    }
                                                }
                                            },
                                        }
                                    }
                                },
                            },
                        },
                    }.Assign(out var thumb),
                    new Panel
                    {
                        Attacheds = { { Grid.RowIndex,1 } },
                        BorderFill = "186,210,234",
                        BorderType = BorderType.BorderThickness,
                        BorderThickness = new Thickness(5,0,5,5),
                        Size = SizeField.Fill,
                        Children =
                        {

                        },
                    },
                }
            });

        }
    }
}
