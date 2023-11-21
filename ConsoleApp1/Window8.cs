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
    public class Window8 : Window
    {
        protected override void InitializeComponent()
        {
            Title = "标题";
            Width = 500;
            Height = 500;
            Background = null;
            Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Children =
                {
                    new DockPanel
                    {
                        LastChildFill = false,
                        Width=200,
                        Height=200,
                        Background="#f00",
                        Children =
                        {
                            new Button
                            {
                                Content="Right",
                                Height="100%",
                                //[nameof(Button.Content)]=nameof(MainModel.Test),
                                [DockPanel.Dock] = Dock.Right//Attacheds =
                                                    //{
                                                    //    {
                                                    //        DockPanel.Dock,
                                                    //        Dock.Right
                                                    //    }
                                                    //},
                            }
                            .Assign(out var testBtn),
                        }
                    }
                }
            }));
            //var name = nameof(MainModel.AddItem);
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
