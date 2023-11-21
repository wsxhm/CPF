using CPF;
using CPF.Animation;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    [CPF.Design.DesignerLoadStyle("res://ConsoleApp1/Stylesheet1.css")]//用于设计的时候加载样式
    public class TestMenuItem : MenuItem
    {
        protected override void InitializeComponent()
        {
            Background = "#FFFFFF";
            //模板定义
            if (DesignMode)
            {
                Width = 180;
            }
            var panel = ItemsPanel.CreateElement();
            panel.Name = "itemsPanel";
            panel.PresenterFor = this;
            panel.Width = 150;
            //panel.Background = "#eee";
            PopupPanel.Children.Add(new Border
            {
                Child = panel,
                ShadowBlur = 4,
                Background = "#eee",
                BorderFill = "#999"
            });
            this.Children.Add(new Panel
            {
                MarginLeft = 30,
                MarginRight = 5,
                MarginBottom = 3,
                MarginTop = 3,
                Name = "contentPanel",
                Children =
                {
                    new ContentControl
                    {
                        MarginLeft=0,
                        Height = 25,
                        Bindings =
                        {
                            {
                                nameof(ContentControl.Content),
                                nameof(Header),
                                this
                            },
                            {
                                nameof(ContentControl.ContentTemplate),
                                nameof(HeaderTemplate),
                                this
                            },
                        }
                    }
                }
            });
            this.Children.Add(new Polygon
            {
                Points =
                {
                    new Point(),
                    new Point(0, 6),
                    new Point(3, 3)
                },
                Fill = "#000",
                MarginRight = 5,
                Bindings =
                {
                    {
                        nameof(Visibility),
                        nameof(HasItems),
                        this,
                        BindingMode.OneWay,
                        a => (bool)a ? Visibility.Visible : Visibility.Collapsed
                    }
                }
            });
            MarginLeft = 0;
            MarginRight = 0;
            this.Triggers.Add(new Trigger
            {
                Property = nameof(IsMouseOver),
                PropertyConditions = a => (bool)a,
                Setters =
                {
                    {
                        nameof(Background),
                        "#fff"
                    }
                }
            });
            Children.Add(new Ellipse
            {
                MarginTop = 0,
                IsAntiAlias = true,
                Fill = new TextureFill("url(https://tva1.sinaimg.cn/crop.0.0.180.180.180/7fde8b93jw1e8qgp5bmzyj2050050aa8.jpg)")
                {
                    Stretch = Stretch.Fill
                },
                Width = 30,
                Height = 30,
                MarginLeft = 0,
                StrokeFill = null,
            });
        }
    }
}
