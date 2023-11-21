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

namespace ConsoleApp1
{
    [CPF.Design.DesignerLoadStyle("res://ConsoleApp1/Stylesheet1.css")]//用于设计的时候加载样式
    public class Class2 : Expander
    {
        protected override void InitializeComponent()
        {
            Children.Add(new Grid
            {
                MarginLeft = 0,
                MarginTop = 0,
                RowDefinitions =
                {
                    new RowDefinition
                    {
                        Height = "auto"
                    },
                    new RowDefinition
                    {
                        Height = "auto"
                    }
                },
                Children =
                {
                    new Panel
                    {
                        Name="header",
                        MarginLeft=0,
                        Children=
                        {
                            new Panel
                            {
                                MarginLeft=3,
                                Name="btnPanel",
                                Children =
                                {
                                    new Ellipse
                                    {
                                        Width=19,
                                        Height=19,
                                        IsAntiAlias=true,
                                        Fill="#fff",
                                        Name="circle",
                                        PresenterFor=this,
                                        StrokeFill="72,72,72",
                                    },
                                    new Polyline
                                    {
                                        Name="arrow",
                                        Points=
                                        {
                                            new Point(0, 1.5f),
                                            new Point(3.5f, 5),
                                            new Point(7, 1.5f)
                                        },
                                        IsAntiAlias=true,
                                        [nameof(RenderTransform)]=(this,nameof(IsExpanded),a=>(bool)a?null:new RotateTransform(180))
                                    }
                                }
                            },
                            new ContentControl
                            {
                                //MarginBottom = 5,
                                MarginLeft = 25,
                                MarginRight = 5,//MarginTop = 5,
                                Bindings =
                                {
                                    {
                                        nameof(Content),
                                        nameof(Header),
                                        this
                                    },
                                    {
                                        nameof(ContentTemplate),
                                        nameof(HeaderTemplate),
                                        this
                                    }
                                }
                            }
                        },
                        Commands =
                        {
                            {
                                nameof(MouseDown),
                                (s,e)=>
                                {
                                    this.IsExpanded=!this.IsExpanded;
                                    (s as UIElement).CaptureMouse();
                                }
                            },
                            {
                                nameof(MouseUp),
                                (s,e)=>
                                {
                                    (s as UIElement).ReleaseMouseCapture();
                                }
                            }
                        },
                        Triggers =
                        {
                            {
                                nameof(IsMouseOver),
                                Relation.Me.Find(a=>a.Name=="circle"&&a.PresenterFor==this),
                                null,
                                (nameof(Ellipse.StrokeFill),"88,149,255")
                            },
                            {
                                nameof(IsMouseCaptured),
                                Relation.Me.Find(a=>a.Name=="circle"&&a.PresenterFor==this),
                                null,
                                (nameof(Ellipse.StrokeStyle),"2"),
                                (nameof(Ellipse.Fill),"217,236,255")
                            }
                        }
                    },
                    new Border
                    {
                        Name = "contentPresenter",
                        Height = "100%",
                        Width = "100%",
                        BorderFill = null,
                        PresenterFor = this,
                        Attacheds=
                        {
                            {
                                Grid.RowIndex,
                                1
                            }
                        },
                        [nameof(Visibility)]=(this,nameof(IsExpanded),a=>(bool)a?Visibility.Visible:Visibility.Collapsed),
                    }
                }
            });
        }
    }
}
