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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CPF.Toolkit.Controls
{
    public class PageControl : Control
    {
        public PageControl()
        {
            this.Pages = new ObservableCollection<int>();
        }
        public int PageIndex { get => GetValue<int>(); set => SetValue(value); }
        public int PageCount { get => GetValue<int>(); set => SetValue(value); }
        int pageSize = 10;

        public event EventHandler<IndexEventArgs> PageIndexChanged { add => AddHandler(value); remove => RemoveHandler(value); }

        ObservableCollection<int> Pages { get => GetValue<ObservableCollection<int>>(); set => SetValue(value); }

        protected override void InitializeComponent()
        {
            this.Size = SizeField.Fill;
            this.Children.Add(new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Size = SizeField.Fill,
                Children =
                {
                    new Button
                    {
                        Content = "上一页",
                        [nameof(IsEnabled)] = new BindingDescribe(this,nameof(this.PageIndex),BindingMode.OneWay,c => ((int)c) > 1),
                    },
                    new ListBox
                    {
                        ItemTemplate = new PageButton
                        {
                            [nameof(PageIndexChanged)] = new CommandDescribe((s,e) => this.PageIndex = (e as IndexEventArgs).Index)
                        },
                        ItemsPanel = new StackPanel{Orientation = Orientation.Horizontal,},
                        Items = this.Pages,
                    },
                    new TextBlock
                    {
                        Text = "……",
                        [nameof(Visibility)] = new BindingDescribe(this,nameof(this.PageCount),BindingMode.OneWay,c => ((int)c) > this.pageSize ? Visibility.Visible : Visibility.Collapsed)
                    },
                    new Button
                    {
                        Content = "100",
                        [nameof(Visibility)] = new BindingDescribe(this,nameof(this.PageCount),BindingMode.OneWay,c => ((int)c) > this.pageSize ? Visibility.Visible : Visibility.Collapsed)
                    },
                    new Button
                    {
                        Content = "下一页",
                        [nameof(IsEnabled)] = new BindingDescribe(this,nameof(this.PageIndex),BindingMode.OneWay,c => ((int)c) < this.PageCount),
                    },
                    new StackPanel
                    {
                        MarginLeft = 10,
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = "1 ",
                                Foreground = "dodgerblue",
                            },
                            new TextBlock
                            {
                                Text = $"/ 100",
                            },
                        }
                    },
                    new TextBlock
                    {
                        Text = "  到第几  "
                    },
                    new Border
                    {
                        MinHeight = 35,
                        MinWidth = 35,
                        MarginLeft = 2,
                        MarginRight = 2,
                        Child = new TextBox
                        {
                            Width = "100%",
                            Text = "10",
                            TextAlignment = TextAlignment.Center,
                            FontSize = 14,
                        },
                        BorderFill = "silver",
                        BorderStroke = "1",
                        CornerRadius = new CornerRadius(2),
                        IsAntiAlias = true,
                        UseLayoutRounding = true,
                    },
                    new TextBlock
                    {
                        Text = " 页 " ,
                    },
                    new Button
                    {
                        Content = "确定",
                    }
                }
            });

            foreach (var item in this.Find<TextBlock>())
            {
                item.FontSize = 14;
            }

            foreach (var item in Find<Button>())
            {
                item.BorderFill = "dodgerblue";
                item.BorderStroke = "1";
                item.CornerRadius = new CornerRadius(4);
                item.Background = "white";
                item.Foreground = "dodgerblue";
                item.MinWidth = 35;
                item.MinHeight = 35;
                item.MarginRight = 2;
                item.IsAntiAlias = true;
                item.UseLayoutRounding = true;
                if (!int.TryParse(item.Content.ToString(), out var _))
                {
                    item.Width = 60;
                }
            }
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            if (propertyName.Or(nameof(this.PageIndex), nameof(this.PageCount)))
            {
                this.Pages.Clear();
                for (int i = 1; i <= this.PageCount; i++)
                {
                    if (i > this.pageSize)
                    {
                        break;
                    }
                    this.Pages.Add(i);
                }
            }
            base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        }

        class PageButton : ListBoxItem
        {
            public event EventHandler<IndexEventArgs> PageIndexChanged { add => AddHandler(value); remove => RemoveHandler(value); }

            protected override void InitializeComponent()
            {
                this.Children.Add(new RadioButton
                {
                    Size = SizeField.Fill,
                    GroupName = "pageNumber",
                    Template = (ss, ee) =>
                    {
                        var s = ss as RadioButton;
                        s.BorderFill = "dodgerblue";
                        s.BorderStroke = "1";
                        s.CornerRadius = new CornerRadius(4);
                        s.Size = new SizeField(35, 35);
                        s.MarginLeft = 2;
                        s.MarginRight = 2;
                        s.UseLayoutRounding = true;
                        s.IsAntiAlias = true;
                        s.Foreground = "dodgerblue";
                        s.Cursor = Cursors.Hand;
                        ee.Add(new TextBlock
                        {
                            FontSize = 14,
                            [nameof(TextBlock.Text)] = new BindingDescribe(this, nameof(Content)),
                        });
                        s.Triggers.Add(new Trigger
                        {
                            Property = nameof(RadioButton.IsChecked),
                            PropertyConditions = (x) => (bool?)x == true,
                            Setters =
                            {
                                { nameof(Background) , "dodgerblue" },
                                { nameof(Foreground) , "white" },
                            },
                        });
                    },
                    [nameof(Content)] = new BindingDescribe(this, nameof(Content)),
                    [nameof(RadioButton.IsChecked)] = new BindingDescribe(new CommandDescribe((s, e) =>
                    {
                        var btn = s as RadioButton;
                        if (btn.IsChecked == true) this.RaiseEvent(new IndexEventArgs(Convert.ToInt32(btn.Content)), nameof(this.PageIndexChanged));
                    })),
                }.Assign(out var rad));

                //rad.PropertyChanged += Rad_PropertyChanged;
            }

            private void Rad_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
            {
                if (e.PropertyName == "IsChecked" && ((bool?)e.NewValue) == true)
                {
                    this.RaiseEvent(new IndexEventArgs(Convert.ToInt32((sender as RadioButton).Content)), nameof(this.PageIndexChanged));
                }
            }
        }

    }
    public class IndexEventArgs : EventArgs
    {
        public IndexEventArgs(int index)
        {
            this.Index = index;
        }
        public int Index { get; set; }
    }
}
