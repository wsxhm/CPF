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
    public class FileTemplate : DataGridCellTemplate
    {
        //模板定义
        protected override void InitializeComponent()
        {
            if (DesignMode)
            {
                Background = "#fff";
                Width = 200;
            }
            else
            {
                Width = "100%";
            }
            Height = 25;
            Children.Add(new TextBlock
            {
                MarginTop = 4,
                Text = "马大云",
                MarginLeft = 34,
                FontSize = 12,
                Bindings =
                {
                    {
                        nameof(TextBlock.Text),
                        this
                    },
                    {nameof(TextBlock.ToolTip),this },
                }
            });
            Children.Add(new Picture
            {
                IsAntiAlias = true,
                Width = 22,
                Height = 22,
                MarginLeft = 5,
                Stretch = Stretch.Fill,
                Source = "res://ConsoleApp1/Icons/file.png",
                Bindings =
                {
                    {
                        nameof(Picture.Source),
                        nameof(ItemInfo.Name),
                        null,
                        BindingMode.OneWay,
                        IconConvert
                    }
                }
            });
        }

#if !DesignMode //用户代码写到这里，设计器下不执行，防止设计器出错
        protected override void OnInitialized()
        {
            base.OnInitialized();

        }
        //用户代码

#endif

        object IconConvert(object name)
        {
            var info = DataContext as ItemInfo;
            if (info.IsFile)
            {
                return "res://ConsoleApp1/Icons/file.png";
            }
            else
            {
                return "res://ConsoleApp1/Icons/directory.png";
            }
        }
    }
}
