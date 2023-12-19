using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    public class DataGridComboBoxColumn : DataGridColumn
    {
        public DataGridComboBoxColumn()
        {
            Items = new ItemCollection { owner = this };
        }
        /// <summary>
        /// 单元格ComboBox的下拉列表数据
        /// </summary>
        public IList Items
        {
            get { return GetValue<IList>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 单元格ComboBox的Item的模板
        /// </summary>
        public UIElementTemplate<ListBoxItem> ItemTemplate
        {
            get { return GetValue<UIElementTemplate<ListBoxItem>>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置指向源对象上的值的路径以提供该对象的可视化表示形式。
        /// </summary>
        public string DisplayMemberPath
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置用于从 SelectedValue 获取 SelectedItem 的路径。
        /// </summary>
        public string SelectedValuePath
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }


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
                c.Add(new ComboBox
                {
                    Width = "100%",
                    Bindings =
                        {
                            { nameof(ComboBox.ItemTemplate), nameof(ItemTemplate), column },
                            { nameof(ComboBox.Items), nameof(Items), column },
                            { nameof(ComboBox.DisplayMemberPath), nameof(DisplayMemberPath), column },
                            { nameof(ComboBox.SelectedValuePath), nameof(SelectedValuePath), column },
                            { nameof(ComboBox.SelectedValue), column.Binding.SourcePropertyName, null, t.Cell.Column.Binding.BindingMode, t.Convert( column.Binding.Convert), t.ConvertBack(t.Cell.Column.Binding.ConvertBack),t.Error,t.Error },
                        }
                }
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
                c.Add(new ContentControl { Bindings = { { nameof(ContentControl.Content), t.Cell.Column.Binding.SourcePropertyName, null, t.Cell.Column.Binding.BindingMode, t.Convert(t.Cell.Column.Binding.Convert), t.ConvertBack(t.Cell.Column.Binding.ConvertBack), t.Error, t.Error } } }
                );
                t.Commands.Add(nameof(t.DoubleClick), (s, e) =>
                {
                    t.Cell.IsEditing = true;
                });
                t.Commands.Add(nameof(t.MouseDown), (s, e) =>
                {
                    t.Focus();
                });

            };
            return template;
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ItemTemplate), new PropertyMetadataAttribute((UIElementTemplate<ListBoxItem>)new ListBoxItem { Width = "100%" }));
        }
    }


}
