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

namespace ConsoleApp1
{
    public class OpenGLWindow : Window
    {
        protected override void InitializeComponent()
        {
            LoadStyleFile("res://ConsoleApp1/Stylesheet1.css");
            //加载样式文件，文件需要设置为内嵌资源

            Title = "标题";
            Width = 500;
            Height = 400;
            Background = null;
            Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Children = //内容元素放这里
                {
                    new ScrollBar
                    {
                        MarginLeft = 387,
                        MarginTop = 14,
                        Width = 35,
                        Height = 236,
                    },
                    new CPF.Skia.GlView
                    {
                        Height = 252,
                        Width = 337,
                    },
                    new TextBox
                    {
                        MarginLeft = 14,
                        MarginTop = 23,
                        Height = 94,
                        Width = 153,
                    },
                }
            }));
            if (!DesignMode)//设计模式下不执行，也可以用#if !DesignMode
            {
                
            }
        }
#if !DesignMode //用户代码写到这里，设计器下不执行，防止设计器出错
        protected override void OnInitialized()
        {
            base.OnInitialized();

        }
        //用户代码

#endif
    }
}
