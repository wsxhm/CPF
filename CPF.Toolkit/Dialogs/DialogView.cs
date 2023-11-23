using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Toolkit.Dialogs
{
    internal class DialogView : Window
    {
        public DialogView(string text, string title, DialogType dialogType, string defaultButton, params string[] buttons)
        {
            this.Title = title;
            this.Text = text;
            this.Buttons = buttons;
            this.DefaultButton = defaultButton;
            this.DialogType = dialogType;
        }

        public DialogType DialogType { get => GetValue<DialogType>(); set => SetValue(value); }
        public string DefaultButton { get => GetValue<string>(); set => SetValue(value); }
        public string Text { get => GetValue<string>(); set => SetValue(value); }
        public string[] Buttons { get => GetValue<string[]>(); set => SetValue(value); }
        protected override void InitializeComponent()
        {
            this.ShowInTaskbar = false;
            this.MaxWidth = 800;
            this.MinWidth = 400;
            this.MaxHeight = 600;
            this.MinHeight = 250;
            this.CanResize = false;
            this.Width = "auto";
            this.Height = "auto";
            this.Background = null;
            this.Children.Add(new DialogFrame(this, new Grid
            {
                Size = SizeField.Fill,
                RowDefinitions =
                {
                    new RowDefinition{ Height = "auto" },
                    new RowDefinition{ },
                    new RowDefinition{ Height = 35 },
                },
                Children =
                {
                    new Picture
                    {
                        Stretch = Stretch.None,
                        Bindings =
                        {
                            {
                                nameof(Visibility),
                                nameof(DialogType),
                                this,BindingMode.OneWay,
                                (DialogType t) => t == DialogType.None ? Visibility.Collapsed : Visibility.Visible
                            },
                            {
                                nameof(Picture.Source),
                                nameof(DialogType),
                                this,BindingMode.OneWay,
                                (DialogType t) =>
                                {
                                    switch (t)
                                    {
                                        case DialogType.Sucess: return "res://CPF.Toolkit/Images/sucess.png";
                                        case DialogType.Error:return"res://CPF.Toolkit/Images/error.png";
                                        case DialogType.Ask: return"res://CPF.Toolkit/Images/ask.png";
                                        case DialogType.Warn:return "res://CPF.Toolkit/Images/warn.png";
                                            default:return null;
                                    }
                                }
                            }
                        }
                    },
                    new TextBox
                    {
                        Attacheds = { { Grid.RowIndex,1 } },
                        BorderType = BorderType.BorderThickness,
                        BorderStroke = new Stroke(1, DashStyles.Solid),
                        BorderThickness = new Thickness(0,0,0,1),
                        //BorderFill = "Silver",
                        IsReadOnly = true,
                        Size = SizeField.Fill,
                        FontSize = 16,
                        WordWarp = true,
                        TextAlignment = TextAlignment.Center,
                        Bindings =
                        {
                            { nameof(TextBox.Text),nameof(Text),this,BindingMode.OneWay}
                        }
                    }.Assign(out var textBox),
                    new StackPanel
                    {
                        Height = "100%",
                        Attacheds = { { Grid.RowIndex,2 } },
                        MarginBottom = 4,
                        Orientation = Orientation.Horizontal,
                    }
                    .LoopCreate(this.Buttons.Length, i => new Button
                    {
                        Content = this.Buttons[i],
                        MinWidth = this.Buttons.Length <= 1 ? 80 : 65,
                        Background = "white",
                        BorderFill = "236,236,236",
                        Height = "95%",
                        MarginRight = 5,
                        Commands = { { nameof(Button.Click),(s,e) => this.DialogResult = this.Buttons[i] } }
                    }),
                }
            }));
            textBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Document.Lines.Count > 5)
            {
                textBox.TextAlignment = TextAlignment.Left;
                textBox.Height = "100%";
            }
            else
            {
                textBox.TextAlignment = TextAlignment.Center;
                textBox.Height = "auto";
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key.Or(Keys.Enter, Keys.Space))
            {
                var buttons = this.Find<Button>();
                var btn = buttons.FirstOrDefault(x => x.IsFocused) ?? buttons.FirstOrDefault(x => x.Content?.ToString() == this.DefaultButton);
                this.DialogResult = btn.Content.ToString();
                e.Handled = true;
            }
            base.OnKeyUp(e);
        }
    }
}
