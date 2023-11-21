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
using System.ComponentModel;
using CPF.Input;

namespace CPF.Controls
{
    /// <summary>
    /// 定义一个弹出层对话框,new LayerDialog { Width = 400, Height = 300, Content = new Button { Content = "测试" } }.ShowDialog(this);
    /// </summary>
    [Browsable(false)]
    public class LayerDialog : ContentControl
    {
        //模板定义
        protected override void InitializeComponent()
        {
            //Height = 222;
            //Width = 390;
            //Background = "#fff";
            Children.Add(new Border
            {
                Name = "contentPresenter",
                Height = "100%",
                Width = "100%",
                BorderFill = null,
                PresenterFor = this,
                BorderStroke = "0",
            });
            Children.Add(new Button
            {
                Bindings =
                {
                    {nameof(Button.Visibility),nameof(ShowCloseButton),this,BindingMode.OneWay,(bool s)=>s?Visibility.Visible:Visibility.Collapsed }
                },
                Name = "dialogClose",
                ToolTip = "关闭",
                MarginRight = 5,
                MarginTop = 5,
                Width = 30,
                Height = 30,
                Content = new Panel
                {
                    Children =
                    {
                        new Line
                        {
                            MarginTop=0,
                            MarginLeft=0,
                            StartPoint = new Point(1, 1),
                            EndPoint = new Point(14, 13),
                            StrokeStyle = "2",
                            IsAntiAlias=true,
                        },
                        new Line
                        {
                            MarginTop=0,
                            MarginLeft=0,
                            StartPoint = new Point(14, 1),
                            EndPoint = new Point(1, 13),
                            StrokeStyle = "2",
                            IsAntiAlias=true,
                        }
                    }
                },
                Commands =
                {
                    {
                        nameof(Button.Click),
                        (s, e) => CloseDialog()
                    }
                }
            });
        }
        /// <summary>
        /// 显示关闭按钮
        /// </summary>
        [Description("显示关闭按钮"), PropertyMetadata(true)]
        public bool ShowCloseButton
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 点击遮罩层是否可以拖拽移动窗体
        /// </summary>
        [PropertyMetadata(true)]
        public bool CanDragMove
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 定义一个弹出层对话框,new LayerDialog { Width = 400, Height = 300, Content = new Button { Content = "测试" } }.ShowDialog(this);
        /// </summary>
        public LayerDialog()
        {
            layer.Children.Add(this);
            layer.MouseDown += Layer_MouseDown;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            e.Handled = true;
        }

        private void Layer_MouseDown(object sender, CPF.Input.MouseButtonEventArgs e)
        {
            if (CanDragMove && layer.Root is Window window)
            {
                window.DragMove();
            }
        }

        Panel layer = new Panel
        {
            Width = "100%",
            Height = "100%",
            Background = "0,0,0,0",
            Name = "maskLayer",
        };
        /// <summary>
        /// 弹出对话框
        /// </summary>
        /// <param name="root"></param>
        public void ShowDialog(UIElement root)
        {
            root.Children.Add(layer);
            if (PlayShowAndCloseAnimation)
            {
                MarginTop = -100;
                this.TransitionValue(nameof(MarginTop), (FloatField)100, TimeSpan.FromSeconds(0.3), new PowerEase { }, AnimateMode.EaseOut);
                layer.TransitionValue(nameof(Background), (ViewFill)"0,0,0,150", TimeSpan.FromSeconds(0.3));
            }
            else
            {
                MarginTop = 100;
                layer.Background = "0,0,0,150";
            }
        }
        /// <summary>
        /// 是否播放弹出层动画
        /// </summary>
        [PropertyMetadata(true)]
        public bool PlayShowAndCloseAnimation
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        public void CloseDialog()
        {
            if (PlayShowAndCloseAnimation)
            {
                //采用过渡属性的写法定义淡出效果
                layer.TransitionValue(nameof(Control.Background), (SolidColorFill)"0,0,0,0", TimeSpan.FromSeconds(0.3), null, AnimateMode.Linear, () =>
                {
                    if (layer.Parent != null)
                    {
                        layer.Parent.Children.Remove(layer);
                    }
                });

                this.TransitionValue(nameof(MarginTop), (FloatField)(-100), TimeSpan.FromSeconds(0.3), new PowerEase { }, AnimateMode.EaseIn);
            }
            else
            {
                if (layer.Parent != null)
                {
                    layer.Parent.Children.Remove(layer);
                }
            }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Background), new UIPropertyMetadataAttribute((ViewFill)"#fff", UIPropertyOptions.AffectsRender));
        }
    }
}
