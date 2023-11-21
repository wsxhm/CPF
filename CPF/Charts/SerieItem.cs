using CPF.Controls;
using CPF.Shapes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Charts
{
    /// <summary>
    /// 图例模板
    /// </summary>
    [System.ComponentModel.Browsable(false)]
    public class SerieItem : Control
    {
        protected override void InitializeComponent()
        {
            MarginLeft = 0;
            Children.Add(new Ellipse
            {
                MarginLeft = 2,
                IsAntiAlias = true,
                Fill = "#000000",
                Height = 8f,
                Width = 8f,
                Bindings =
                {
                    {nameof(Ellipse.Fill),nameof(IChartData.Fill) }
                }
            });
            Children.Add(new StackPanel
            {
                Orientation = Orientation.Horizontal,
                MarginLeft = 12,
                Children =
                {
                    new TextBlock
                    {
                        Bindings =
                        {
                            {nameof(TextBlock.Text),nameof(Name),this}
                        }
                    },
                    new TextBlock
                    {
                        Bindings =
                        {
                            {nameof(TextBlock.Text),nameof(Value),this,BindingMode.OneWay,(double? value)=>value.HasValue? ": "+value.Value.ToString((DataContext as IFormatData).Format):"" }
                        }
                    }
                }
            });
        }

        public double? Value
        {
            get { return GetValue<double?>(); }
            set { SetValue(value); }
        }
    }
}
