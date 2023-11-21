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
    [CPF.Design.DesignerLoadStyle("res://$safeprojectname$/Stylesheet1.css")]//用于设计的时候加载样式
    public class DriveTemplate : ListBoxItem
    {
        //模板定义
        protected override void InitializeComponent()
        {
            if (DesignMode)
            {
                Width = 200;
            }
            else
            {
                Width = "100%";
            }
            Height = 40;
            Background = "#fff";
            Children.Add(new Picture
            {
                IsAntiAlias = true,
                Width = 30,
                Height = 30,
                MarginLeft = 5,
                Stretch = Stretch.Fill,
                Source = "res://ConsoleApp1/Icons/Drive.png",
            });
            Children.Add(new TextBlock
            {
                MarginRight = 3,
                MarginLeft = 39,
                MaxHeight="100%",
                Bindings =
                {
                    {nameof(TextBlock.Text),"Item1" },
                    {nameof(TextBlock.ToolTip),"Item1" },
                }
            });
            Triggers.Add(new Trigger
            {
                Property = nameof(IsMouseOver),//PropertyConditions = a => (bool)a && !IsSelected,
                Setters =
                {
                    {
                        nameof(Background),
                        "229,243,251"
                    }
                }
            });
            //Triggers.Add(new Trigger
            //{
            //    Property = nameof(IsSelected),
            //    PropertyConditions = a => (bool)a,
            //    Setters =
            //    {
            //        {
            //            nameof(Background),
            //            "203,233,246"
            //        }
            //    }
            //});
        }

#if !DesignMode //用户代码写到这里，设计器下不执行，防止设计器出错
        protected override void OnInitialized()
        {
            base.OnInitialized();

        }
        //用户代码

#endif
    }

    public class ItemInfo
    {
        public bool IsFile { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Size { get; set; }
        public string DateTime { get; set; }
    }
}
