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
    [CPF.Design.DesignerLoadStyle("res://ConsoleApp1/Stylesheet1.css")]//用于设计的时候加载样式
    public class DataGridRowTemplate : DataGridRow
    {
        //模板定义
        protected override void InitializeComponent()
        {
            BorderType = BorderType.BorderThickness;
            BorderThickness = new Thickness(0, 0, 0, 1);
            BorderFill = "#000";
            Children.Add(new StackPanel { Orientation = Orientation.Horizontal, Name = "itemsPanel", PresenterFor = this, Size = new SizeField("100%", "100%") });
            Triggers.Add(new Trigger
            {
                Property = nameof(IsSelected),
                Setters = { { nameof(Background), "#ddd" } }
            });
        }
        [PropertyChanged(nameof(DataContext))]
        void OnDataContextChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            //if (newValue != null && (newValue.GetPropretyValue("p4").Equals(8) || newValue.GetPropretyValue("p4").Equals(38) || newValue.GetPropretyValue("p4").Equals(68)))
            //{
            //    Height = 180;
            //}
            //else
            //{
            //    Height = 18;
            //}
        }
    }
}
