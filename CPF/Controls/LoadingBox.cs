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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 定义一个加载动画的弹出层
    /// </summary>
    [Browsable(false)]
    public class LoadingBox : Control
    {
        //模板定义
        protected override void InitializeComponent()
        {
            Children.Add(new Picture
            {
                Stretch = Stretch.Uniform,//Stretch = Stretch.Uniform,
                Source = "res://CPF/loading.gif",
                MarginTop = 0,
                Height = 124,
                Width = 124,
            });
            Children.Add(new TextBlock
            {
                Name = "message",
                FontStyle = FontStyles.Bold,
                FontSize = 14f,
                Foreground = "#FFF",
                MarginTop = 165,
                Text = "TextBlock",
                Bindings =
                {
                    {
                        nameof(TextBlock.Text),
                        nameof(Message),
                        this
                    }
                }
            });
        }
        /// <summary>
        /// 提示消息
        /// </summary>
        public string Message
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
