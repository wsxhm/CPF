using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPF.Drawing;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// DataGrid文本编辑模板
    /// </summary>
    [Description("DataGrid文本编辑模板")]
    public class DataGridCellTextEditTemplate : DataGridCellTemplate
    {
        protected override void InitializeComponent()
        {
            Width = "100%";
            Height = "100%";
            BorderType = BorderType.BorderThickness;
            BorderThickness = new Thickness(0, 0, 1, 0);
            BorderFill = "#000";
            Children.Add(new Border
            {
                Width = "100%",
                Height = "100%",
                PresenterFor = this,
                BorderFill = "30,159,255",
                BorderStroke = new Stroke(1),
                Bindings = { { nameof(BorderFill), nameof(IsError), this, BindingMode.OneWay, a => (bool)a ? "#f00" : "30,159,255" } },
                Child = new TextBox
                {
                    Width = "100%",
                    AcceptsReturn = false,
                    AcceptsTab = false,
                    HScrollBarVisibility = ScrollBarVisibility.Hidden,
                    VScrollBarVisibility = ScrollBarVisibility.Hidden,
                    TextAlignment = TextAlignment.Center,
                    Commands = { { nameof(LostFocus), (s, e) => { (s as TextBox).SelectionEnd.Clear(); Cell.IsEditing = false; } }, { nameof(IsInitialized), (s, e) => { (s as TextBox).Focus(); } } },
                    Bindings =
                        {
                            { nameof(TextBox.Text), Cell.Column.Binding.SourcePropertyName, null, Cell.Column.Binding.BindingMode, Convert(Cell.Column.Binding.Convert), ConvertBack(Cell.Column.Binding.ConvertBack),Error,Error },
                        }
                }

            });
        }

    }
}
