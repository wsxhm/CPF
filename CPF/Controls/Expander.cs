using CPF.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;
using CPF.Styling;

namespace CPF.Controls
{
    /// <summary>
    /// 表示一种控件，该控件显示具有可折叠内容显示窗口的标题。
    /// </summary>
    [Description("表示一种控件，该控件显示具有可折叠内容显示窗口的标题。")]
    public class Expander : ContentControl
    {

        /// <summary>
        /// 获取或设置每个控件的标题所用的数据。
        /// </summary>
        [TypeConverter(typeof(StringConverter))]
        public object Header
        {
            get { return GetValue<object>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置用于显示控件标头的内容的模板。
        /// </summary>
        [Browsable(false)]
        public UIElementTemplate<ContentTemplate> HeaderTemplate
        {
            get { return GetValue<UIElementTemplate<ContentTemplate>>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置 Expander 内容窗口是否可见
        /// </summary>
        public bool IsExpanded
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

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
                        Height = "*"
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

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(HeaderTemplate), new PropertyMetadataAttribute((UIElementTemplate<ContentTemplate>)new ContentTemplate()));
        }
    }
}
