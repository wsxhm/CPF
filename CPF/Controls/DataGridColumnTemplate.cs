using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using CPF.Input;
using System.Linq;
using CPF.Styling;
using CPF.Shapes;

namespace CPF.Controls
{
    /// <summary>
    /// DataGridColumn模板
    /// </summary>
    [Description("DataGridColumn模板"), Browsable(false)]
    public class DataGridColumnTemplate : ContentControl
    {
        /// <summary>
        /// 当前所在的列
        /// </summary>
        [NotCpfProperty]
        public DataGridColumn Column
        {
            get; internal set;
        }

        protected override void InitializeComponent()
        {
            BorderType = BorderType.BorderThickness;
            BorderFill = "#bbb";
            BorderThickness = new Thickness(0, 0, 1, 1);
            Background = new LinearGradientFill { EndPoint = new PointField(0, "100%"), GradientStops = { new GradientStop(Color.White, 0), new GradientStop("#eee", 1) } };
            Height = 25;
            Children.Add(new Border
            {
                BorderFill = null,
                Width = "100%",
                Height = "100%",
                Name = "contentPresenter",
                PresenterFor = this,
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
            Children.Add(new Polygon
            {
                MarginTop = 1,
                Points = { new Point(), new Point(8, 0), new Point(4, 4) },
                Fill = "linear-gradient(0 0,10 10,#5B86A0,#E4F1FF)",
                StrokeFill = "#8BC6E0",
                Bindings =
                {
                    { nameof(Visibility), nameof(Column.SortDirection), Column, BindingMode.OneWay, a => ((ListSortDirection?)a) == null ? Visibility.Collapsed : Visibility.Visible },
                    { nameof(RenderTransform), nameof(Column.SortDirection), Column, BindingMode.OneWay, a => ((ListSortDirection?)a) == ListSortDirection.Ascending ? new RotateTransform(180) : null },
                    { nameof(MarginTop), nameof(Column.SortDirection), Column, BindingMode.OneWay, a => ((ListSortDirection?)a) == ListSortDirection.Ascending ? (FloatField)0 : (FloatField)1 },
                },
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

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!e.Handled && e.MouseButton == MouseButton.Left)
            {
                if (Column.CanUserSort)
                {
                    Column.SortDirection = Column.SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }
            }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ClipToBounds), new UIPropertyMetadataAttribute(true, UIPropertyOptions.AffectsRender));
        }
        /// <summary>
        /// 用于绑定拖拽调整大小的Thumb.DragDelta
        /// </summary>
        /// <param name="data"></param>
        public void ThumbDragDelta(CpfObject sender, object data)
        {
            var hc = (data as DragDeltaEventArgs).HorizontalChange;
            if (Column.DataGridOwner.Columns.Any(a => a.Width.UnitType == DataGridLengthUnitType.Star))
            {
                DataGridColumn next = Column.DisplayIndex < Column.DataGridOwner.Columns.Count - 1 ? Column.DataGridOwner.Columns[Column.DisplayIndex + 1] : null;
                if (hc < 0)
                {
                    if (Column.ActualWidth + hc < Column.MinWidth)
                    {
                        hc = Column.MinWidth - Column.ActualWidth;
                    }
                    if (next != null)
                    {
                        if (next.ActualWidth - hc > next.MaxWidth)
                        {
                            hc = next.ActualWidth - next.MaxWidth;
                        }
                    }
                }
                else
                {
                    if (Column.ActualWidth + hc > Column.MaxWidth)
                    {
                        hc = Column.MaxWidth - Column.ActualWidth;
                    }
                    if (next != null)
                    {
                        if (next.ActualWidth - hc < next.MinWidth)
                        {
                            hc = next.ActualWidth - next.MinWidth;
                        }
                    }
                }
                if (next != null)
                {
                    var aw = next.ActualWidth - hc;
                    if (next.Width.UnitType == DataGridLengthUnitType.Star)
                    {
                        next.Width = new DataGridLength(next.Width.Value / next.ActualWidth * aw, DataGridLengthUnitType.Star);
                        next.ActualWidth = aw;
                    }
                    else
                    {
                        next.ActualWidth = aw;
                        next.Width = aw;
                    }
                }
                var caw = hc + Column.ActualWidth;
                if (Column.Width.UnitType == DataGridLengthUnitType.Star)
                {
                    Column.Width = new DataGridLength(Column.Width.Value / Column.ActualWidth * caw, DataGridLengthUnitType.Star);
                    Column.ActualWidth = caw;
                }
                else
                {
                    Column.ActualWidth = caw;
                    Column.Width = caw;
                }
            }
            else
            {
                Column.ActualWidth = hc + Column.ActualWidth;
            }
        }
    }
}
