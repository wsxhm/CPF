using System;
using System.Collections.Generic;
using System.Text;
using CPF.Controls;
using CPF.Drawing;
using CPF;

namespace ConsoleApp1
{
    public class TreeViewItemContentTemplate : TreeViewContentTemplate
    {
        protected override void InitializeComponent()
        {//模板定义
            Children.Add(new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new CheckBox
                    {
                        Content = "test",
                        Bindings =
                        {
                            { nameof(Content), nameof(Content), this },
                            { nameof(CheckBox.IsChecked), nameof(NodeData.IsChecked)  }
                        },
                        Commands =
                        {
                            { nameof(CheckBox.IsChecked), (s, e) => SetCheckBox((s as CheckBox).IsChecked.Value) } ,
                            { nameof(CheckBox.MouseDown), (s, e) => TreeViewItem.SingleSelect() }
                        }
                    },
                    new Button { Content = "自定义模板，可以任意设计", }
                }
            });
        }


        void SetCheckBox(bool isChecked)
        {
            foreach (NodeData item in TreeViewItem.Items)
            {
                item.IsChecked = isChecked;
            }
        }
    }
}
