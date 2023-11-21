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

    public class EditComboBox : ComboBox
    {
        private List<string> Temp_Items = new List<string>();
        private List<string> Temp_ItemsAll = new List<string>();
        private TextBox input_text;

        public string Version1
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            //this.Commands.Add(nameof(ComboBox.IsDropDownOpen), nameof(evetns_IsDropDownOpen), this, CommandParameter.EventSender);
            input_text = this.Find<TextBox>().First();
            input_text.TextChanged += EditComboBox_TextInput;
            this.IsEditable = true;
            this.ItemReset();

        }

        public void ItemReset()
        {
            Temp_Items = new List<string>();
            foreach (var item in this.Items.ToItems())
            {
                this.Temp_Items.Add(item.ToString());
            }
        }


        private void EditComboBox_TextInput(object sender, EventArgs e)
        {
            Changed();
        }

        private void evetns_IsDropDownOpen()
        {
            this.BeginInvoke(() => {

                if (!this.IsDropDownOpen)
                {
                    string search = input_text.Text.ToUpper().ToString().Trim();
                    if (string.IsNullOrWhiteSpace(search))
                    {
                        input_text.Text = "";
                        return;
                    }

                    if (this.Temp_Items.Where(t => t == search).ToList().Count == 0)
                    {
                        this.Items.Clear();
                        var results = this.Temp_Items.Where(t => t.Contains(search)).ToArray();
                        foreach (var item in this.Temp_Items.ToList())
                        {
                            this.Items.Add(item);

                        }
                        this.Cursor = Cursors.Arrow;
                        input_text.Text = "";
                    }
                }



            });
        }

        private void Changed()
        {
            this.BeginInvoke(() => {
                foreach (var item in this.ElementItems)
                {
                    item.Visibility = item.DataContext.ToString().ToLower().Contains(input_text.Text.ToLower()) ? Visibility.Visible : Visibility.Collapsed;
                }
            });
        }

    }

}
