using System;
using System.Collections.Generic;
using System.Text;
using CPF.Controls;
using CPF.Drawing;
using CPF.Styling;
using CPF;

namespace ConsoleApp1
{
    public class ColumnTemplate : DataGridColumnTemplate
    {
        protected override void InitializeComponent()
        {//模板定义
            BorderType = BorderType.BorderThickness;
            BorderFill = "#bbb";
            BorderThickness = new Thickness(0, 0, 1, 1);
            Background = new LinearGradientFill { EndPoint = new PointField(0, "100%"), GradientStops = { new GradientStop(Color.White, 0), new GradientStop("#eee", 1) } };
            Height = 25;
            Children.Add(new CheckBox
            {
                Content = new Border
                {
                    BorderFill = null,
                    Name = "contentPresenter",
                    PresenterFor = this,
                },
                Commands = { { nameof(CheckBox.IsChecked), (s, e) => SetCheck((s as CheckBox).IsChecked) } }
            });
            Children.Add(new Thumb
            {
                ZIndex = 2,
                Width = 6,
                Height = "100%",
                Cursor = Cursors.SizeWestEast,
                MarginRight = -2,
                Background = null,
                Commands = { { nameof(Thumb.DragDelta), ThumbDragDelta } },
                Bindings = { { nameof(Visibility), nameof(Column.CanUserResize), Column, BindingMode.OneWay, a => (bool)a ? Visibility.Visible : Visibility.Collapsed } }
            });

            Commands.Add(nameof(MouseDown), (s, e) => { (s as UIElement).CaptureMouse(); });
            Commands.Add(nameof(MouseUp), (s, e) => { (s as UIElement).ReleaseMouseCapture(); });
            Triggers.Add(new Trigger
            {
                Property = nameof(IsMouseOver),
                Setters = {
                            { nameof(Background), new LinearGradientFill { EndPoint = new PointField(0, "100%"), GradientStops = { new GradientStop("227,247,255", 0),new GradientStop("175,224,245", 1) } } },
                            { nameof(BorderFill),"105,187,227"}
                        }
            });
            Triggers.Add(new Trigger
            {
                Property = nameof(IsMouseCaptured),
                Setters = {
                            { nameof(Background), new LinearGradientFill { EndPoint = new PointField(0, "100%"), GradientStops = { new GradientStop("188,228,249", 0),new GradientStop("140,207,241", 1) } } },
                            { nameof(BorderFill),"105,187,227"}
                        }
            });
        }

        void SetCheck(bool? check)
        {
            var column = Column;
            foreach (var item in column.DataGridOwner.Items)
            {
                item.SetPropretyValue(column.Binding.SourcePropertyName, (bool)check);
            }
        }
    }
}
