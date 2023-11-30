using CPF.Controls;
using CPF.Drawing;
using CPF.Platform;
using CPF.Shapes;
using CPF.Toolkit.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CPF.Toolkit.Controls
{
    public class MdiHost : Grid
    {
        public MdiHost()
        {
            this.TaskBarList = new Collection<UIElement>();
            this.Size = SizeField.Fill;
            this.Background = "204,204,204";
            base.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            base.RowDefinitions.Add(new RowDefinition { Height = 35 });
            base.Children.Add(this.host);
            taskBar = base.Children.Add(new ListBox
            {
                Size = SizeField.Fill,
                Background = "white",
                BorderFill = new SolidColorFill { Color = Color.Silver },
                BorderThickness = new Thickness(0, 1, 0, 0),
                BorderType = BorderType.BorderThickness,
                ItemsPanel = new StackPanel { Orientation = Orientation.Horizontal },
                ItemTemplate = new ListBoxItem
                {
                    Width = 100,
                    MarginRight = 1,
                    FontSize = 16f,
                    BorderFill = "Silver",
                    BorderThickness = new Thickness(1),
                    Margin = new ThicknessField(1),
                    CornerRadius = new CornerRadius(2),
                    IsAntiAlias = true,
                    UseLayoutRounding = true,
                    BorderType = BorderType.BorderThickness,
                    ContentTemplate = new ContentTemplate
                    {
                        Size = SizeField.Fill,
                        Content = "test"
                        //Content = new StackPanel
                        //{
                        //    Orientation = Orientation.Horizontal,
                        //    Size = SizeField.Fill,
                        //    Children =
                        //    {
                        //        new TextBlock
                        //        {
                        //            //[nameof(TextBlock.Text)] = new BindingDescribe("Title",BindingMode.OneWay)
                        //            Text = "test"
                        //        }
                        //    }
                        //},
                    },
                    //Content = new StackPanel
                    //{
                    //    Size = SizeField.Fill,
                    //    Orientation = Orientation.Horizontal,
                    //    Children =
                    //    {
                    //        new TextBlock
                    //        {
                    //            [nameof(TextBlock.Text)] = new BindingDescribe("Title",BindingMode.OneWay)
                    //        }
                    //    }
                    //},
                    //[nameof(ListBoxItem.Content)] = new BindingDescribe("Title")
                },
                Items = this.TaskBarList,
                [nameof(ListBox.SelectedValue)] = new BindingDescribe(this, nameof(this.SelectWindow), BindingMode.TwoWay),
            }, row: 1);
            this.host.PropertyChanged += Host_PropertyChanged;
            this.host.UIElementAdded += Host_UIElementAdded;
            this.host.UIElementRemoved += Host_UIElementRemoved;
        }
        readonly Dictionary<UIElement, MdiWindowRect> normalRect = [];
        readonly Panel host = new() { Size = SizeField.Fill };
        readonly ListBox taskBar;
        Collection<UIElement> TaskBarList { get => GetValue<Collection<UIElement>>(); set => SetValue(value); }
        public new UIElementCollection Children => host.Children;
        public MdiWindow SelectWindow { get => GetValue<MdiWindow>(); set => SetValue(value); }

        [PropertyMetadata(TaskBarPlacement.Bottom)]
        public TaskBarPlacement TaskBarPlacement { get => GetValue<TaskBarPlacement>(); set => SetValue(value); }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            if (propertyName == nameof(this.SelectWindow) && this.SelectWindow != null)
            {
                this.Topping(this.SelectWindow);
                this.SelectWindow.WindowState = this.normalRect[this.SelectWindow].OldState;
            }
            else if (propertyName == nameof(this.TaskBarPlacement) && this.taskBar != null)
            {
                this.RowDefinitions.Clear();
                this.ColumnDefinitions.Clear();
                switch ((TaskBarPlacement)newValue)
                {
                    case TaskBarPlacement.Top:
                        this.RowDefinitions.Add(new RowDefinition { Height = 35 });
                        this.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                        RowIndex(this.taskBar, 0);
                        RowIndex(this.host, 1);
                        break;
                    case TaskBarPlacement.Bottom:
                        this.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                        this.RowDefinitions.Add(new RowDefinition { Height = 35 });
                        RowIndex(this.host, 0);
                        RowIndex(this.taskBar, 1);
                        break;
                }
            }
            base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        }

        private void Host_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            if (e.PropertyName.Or(nameof(ActualSize), nameof(Size), nameof(Width), nameof(Height)))
            {
                foreach (MdiWindow mdi in this.host.Children)
                {
                    if (mdi.WindowState == WindowState.Maximized) continue;

                    if (mdi.MarginLeft.Value + mdi.ActualSize.Width > this.ActualSize.Width)
                    {
                        mdi.MarginLeft = this.ActualSize.Width - mdi.ActualSize.Width;
                    }

                    if (mdi.MarginTop.Value + mdi.ActualSize.Height > this.ActualSize.Height)
                    {
                        mdi.MarginTop = this.ActualSize.Height - mdi.ActualSize.Height;
                    }
                }
            }
        }

        private void Host_UIElementRemoved(object sender, UIElementRemovedEventArgs e)
        {
            var view = e.Element as MdiWindow;
            view.PropertyChanged -= Element_PropertyChanged;
            view.PreviewMouseDown -= Element_PreviewMouseDown;
            view.Closing -= View_Closing;
            this.TaskBarList.Remove(e.Element);
            this.normalRect.Remove(e.Element);
        }

        private void Host_UIElementAdded(object sender, UIElementAddedEventArgs e)
        {
            var view = e.Element as MdiWindow;
            this.normalRect.Add(e.Element, new MdiWindowRect { Left = 0, Top = 0, Height = 500, Width = 500 });
            view.PropertyChanged += Element_PropertyChanged;
            view.PreviewMouseDown += Element_PreviewMouseDown;
            view.Closing += View_Closing;
            this.TaskBarList.Add(view);
            e.Element.ZIndex = this.host.Children.Max(x => x.ZIndex) + 1;
            this.Topping(view);
        }

        private void View_Closing(object sender, ClosingEventArgs e)
        {
            if (e.Cancel) return;
            UIElement mdiWindow = null;
            if (sender is IClosable closable)
            {
                mdiWindow = this.host.Children.FirstOrDefault(x => x.DataContext == closable);
            }
            else if (sender is MdiWindow mdi)
            {
                mdiWindow = mdi;
            }
            if (mdiWindow != null)
            {
                if (mdiWindow == this.SelectWindow)
                {
                    this.BeginInvoke(() =>
                    {
                        if (this.host.Children.Count == 0) return;
                        var index = this.host.Children.Where(x => x.Visibility == Visibility.Visible).Max(x => x.ZIndex);
                        if (index != -1)
                        {
                            this.SelectWindow = this.host.Children.Find(x => x.ZIndex == index) as MdiWindow;
                        }
                    });
                }
                this.host.Children.Remove(mdiWindow);
                mdiWindow.Dispose();
            }
        }

        private void Element_PreviewMouseDown(object sender, Input.MouseButtonEventArgs e)
        {
            var ele = (MdiWindow)sender;
            this.Topping(ele);
        }

        private void Element_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            var view = sender as MdiWindow;
            switch (e.PropertyName)
            {
                case nameof(MdiWindow.WindowState):

                    switch ((WindowState)e.NewValue)
                    {
                        case WindowState.Normal:
                            var rect = this.normalRect[view];
                            view.Size = new SizeField(rect.Width, rect.Height);
                            view.MarginLeft = rect.Left;
                            view.MarginTop = rect.Top;
                            break;
                        case WindowState.Minimized:
                            view.Visibility = Visibility.Collapsed;
                            this.SelectWindow = this.host.Children.FindLast(x => x.Visibility == Visibility.Visible) as MdiWindow;
                            this.normalRect[view].OldState = (WindowState)e.OldValue;
                            break;
                        case WindowState.Maximized:
                        case WindowState.FullScreen:
                            view.Size = SizeField.Fill;
                            view.MarginLeft = 0;
                            view.MarginTop = 0;
                            break;
                    }
                    break;

                case nameof(ZIndex):
                    this.SelectWindow = view;
                    this.SelectWindow.Visibility = Visibility.Visible;
                    break;

                case nameof(MarginLeft):
                    if (view.WindowState == WindowState.Normal)
                    {
                        var left = (FloatField)e.NewValue;
                        if (left.Value <= 0) view.MarginLeft = 0;
                        this.normalRect[view].Left = view.MarginLeft.Value;
                    }
                    break;
                case nameof(MarginTop):
                    if (view.WindowState == WindowState.Normal)
                    {
                        var top = (FloatField)e.NewValue;
                        if (top.Value <= 0) view.MarginTop = 0;
                        this.normalRect[view].Top = view.MarginTop.Value;
                    }
                    break;

                case nameof(Width):
                    if (view.WindowState == WindowState.Normal)
                    {
                        var size = (FloatField)e.NewValue;
                        this.normalRect[view].Width = size.Value;
                    }
                    break;

                case nameof(Height):
                    if (view.WindowState == WindowState.Normal)
                    {
                        var size = (FloatField)e.NewValue;
                        this.normalRect[view].Height = size.Value;
                    }
                    break;
            }
        }

        public void Topping(MdiWindow ele)
        {
            if (ele == null) return;
            ele.Focus();
            var index = this.host.Children.Max(x => x.ZIndex);
            if (ele.ZIndex == index)
            {
                ele.Visibility = Visibility.Visible;
                return;
            }
            ele.ZIndex = index + 1;
        }
    }
}
