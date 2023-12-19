using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    public class DataGridCheckBoxColumn : DataGridColumn
    {
        public override DataGridCellTemplate GenerateEditingElement()
        {
            var template = new DataGridCellTemplate();
            template.Template = (a, c) =>
            {
                var t = a as DataGridCellTemplate;
                var column = t.Cell.Column as DataGridComboBoxColumn;
                a.Width = "100%";
                a.Height = "100%";
                t.BorderType = BorderType.BorderThickness;
                t.BorderThickness = new Thickness(0, 0, 1, 0);
                t.BorderFill = "#000";
                c.Add(new CheckBox { Bindings = { { nameof(CheckBox.IsChecked), t.Cell.Column.Binding.SourcePropertyName, null, t.Cell.Column.Binding.BindingMode, t.Convert(t.Cell.Column.Binding.Convert), t.ConvertBack(t.Cell.Column.Binding.ConvertBack), t.Error, t.Error } } }
                );
                t.Commands.Add(nameof(t.IsInitialized), (s, e) =>
                {
                    t.Focus();
                });
                t.Commands.Add(nameof(t.LostFocus), (s, e) =>
                {
                    t.Cell.IsEditing = false;
                });

            };
            return template;
        }

        public override DataGridCellTemplate GenerateElement()
        {
            var template = new DataGridCellTemplate();
            template.Template = (a, c) =>
            {
                var t = a as DataGridCellTemplate;
                var column = t.Cell.Column as DataGridComboBoxColumn;
                a.Width = "100%";
                a.Height = "100%";
                t.BorderType = BorderType.BorderThickness;
                t.BorderThickness = new Thickness(0, 0, 1, 0);
                t.BorderFill = "#000";
                c.Add(new CheckBox
                {
                    IsEnabled =false,
                    Bindings =
                    {
                        { nameof(CheckBox.IsChecked), t.Cell.Column.Binding.SourcePropertyName, null, t.Cell.Column.Binding.BindingMode, t.Convert(t.Cell.Column.Binding.Convert), t.ConvertBack(t.Cell.Column.Binding.ConvertBack), t.Error, t.Error
                        }
                    }
                }
                );
                t.Commands.Add(nameof(t.MouseDown), (s, e) =>
                {
                    t.Focus();
                    t.Cell.IsEditing = true;
                });

            };
            return template;
        }
    }
}
