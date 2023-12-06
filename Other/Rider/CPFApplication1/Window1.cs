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

namespace CPFApplication1
{
    public class Window1 : Window
    {
        protected override void InitializeComponent()
        {
            LoadStyleFile("res://CPFApplication1/Stylesheet1.css");//加载样式文件，文件需要设置为内嵌资源

            Title = "标题";
            Width = 500;
            Height = 400;
            Background = null;
            Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Children =
                {
                    new Button{ Content="按钮" }
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
