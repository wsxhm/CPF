using System;
using System.Collections.Generic;
using System.Text;
using CPF.Controls;
using CPF;

namespace ConsoleApp1
{
    /// <summary>
    /// 单元格模板
    /// </summary>
    public class CellTemplate : DataGridCellTemplate
    {
        protected override void InitializeComponent()
        {
            Width = "100%";
            Height = "100%";
            Children.Add(new Border
            {
                Width = "100%",
                Height = "100%",
                BorderType = BorderType.BorderThickness,
                BorderThickness = new Thickness(0, 0, 1, 0),
                Child = new Button
                {
                    Size=new SizeField("90%","90%"),
                    Bindings =
                    {
                        {
                            nameof(Button.Content),
                            Cell.Column.Binding.SourcePropertyName,
                            null,
                            Cell.Column.Binding.BindingMode,
                            Convert(Cell.Column.Binding.Convert),
                            ConvertBack(Cell.Column.Binding.ConvertBack),
                            Error,
                            Error
                        }
                    },
                    Commands =
                    {
                        {
                            nameof(Button.Click),
                            nameof(Model.ClickCell),
                            null,
                            new Func<int>(() =>
                            {
                                return Cell.Row.Index;
                            })
                        }
                    }
                }
            });
            Commands.Add(nameof(MouseDown), (s, e) =>
            {
                Focus();
            });
        }
    }
}