using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using CPF.Controls;
using CPF;

namespace ConsoleApp1
{
    public class Component1 : Control
    {
        [Browsable(false)]
        public string test1
        {
            set { }
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        protected override void InitializeComponent()
        {
            Children.Add(new StackPanel
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = "test"
                    },
                    new Button
                    {
                        Content = "自定义模板就是这样做啊"
                    },
                    new CheckBox
                    {
                        Content = "测试",
                        IsChecked=true
                    },
                    new RadioButton
                    {
                        Content="单选1"
                    },
                    new RadioButton
                    {
                        Content="单选2",
                        IsChecked=true
                    },
                    new Picture
                    {
                        Source="http://tb2.bdstatic.com/tb/static-puser/widget/celebrity/img/single_member_100_0b51e9e.png"
                    },
                    new ScrollBar
                    {
                        Orientation= Orientation.Horizontal,
                        Width=200,
                        Height=20,
                        Value=0.8f
                    },
                    new CPF.Shapes.Ellipse
                    {
                        Width=60,
                        Height=40,
                        IsAntiAlias=true,
                        Fill="#f00",
                        StrokeStyle=new CPF.Drawing.Stroke(2, CPF.Drawing.DashStyles.Dash)
                    },
                    new TextBox
                    {
                        Height = 22,
                        Width = 142,
                    },
                }
            });
            if (DesignMode)
            {
                Children.Add(new Template());
            }
        }
        #endregion
    }
}
