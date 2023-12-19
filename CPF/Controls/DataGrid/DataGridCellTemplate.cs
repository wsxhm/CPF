using CPF.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 单元格模板
    /// </summary>
    [Description("单元格模板"), Browsable(false)]
    public class DataGridCellTemplate : Control
    {
        [NotCpfProperty]
        public DataGridCell Cell { get; internal set; }
        /// <summary>
        /// 传递数据的时候是否出错
        /// </summary>
        public bool IsError
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        protected override void InitializeComponent()
        {
            Classes = "DataGridCell";
            BorderFill = "#000";
            Width = "100%";
            Height = "100%";
            BorderType = BorderType.BorderThickness;
            BorderThickness = new Thickness(0, 0, 1, 0);
            Children.Add(new ContentControl { Bindings = { { nameof(ContentControl.Content), this } } });

            Commands.Add(nameof(DoubleClick), (s, e) =>
            {
                Cell.IsEditing = true;
            });
            Commands.Add(nameof(MouseDown), (s, e) =>
            {
                Focus();
            });
        }

        public void Error(Binding binding, object value, Exception e)
        {
            IsError = true;
        }

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Func<object, object> Convert(Func<object, object> func)
        {
            if (func != null)
            {
                return a =>
                {
                    IsError = false;
                    return func(a);
                };
            }
            return a => { IsError = false; return a; };
        }

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Func<object, object> ConvertBack(Func<object, object> func)
        {
            if (func != null)
            {
                return a =>
                {
                    IsError = false;
                    return func(a).ConvertTo(Binding.Current.GetSourcePropertyType());
                };
            }
            return a =>
            {
                IsError = false;
                return a.ConvertTo(Binding.Current.GetSourcePropertyType());
            };
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!e.Handled)
            {
                Cell.Row.DataGridOwner.OnCellMouseDown(new DataGridCellMouseEventArgs(Cell, (UIElement)e.OriginalSource, e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, e.MiddleButton == MouseButtonState.Pressed, e.Location, e.MouseDevice, e.MouseButton, e.IsTouch));
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (!e.Handled)
            {
                Cell.Row.DataGridOwner.OnCellMouseUp(new DataGridCellMouseEventArgs(Cell, (UIElement)e.OriginalSource, e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, e.MiddleButton == MouseButtonState.Pressed, e.Location, e.MouseDevice, e.MouseButton, e.IsTouch));
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (!e.Handled)
            {
                Cell.Row.DataGridOwner.OnCellMouseEnter(new DataGridCellMouseEventArgs(Cell, (UIElement)e.OriginalSource, e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, e.MiddleButton == MouseButtonState.Pressed, e.Location, e.MouseDevice, MouseButton.None, e.IsTouch));
            }
        }

        protected override void OnDoubleClick(RoutedEventArgs e)
        {
            base.OnDoubleClick(e);
            if (!e.Handled)
            {
                Cell.Row.DataGridOwner.OnCellDoubleClick(new DataGridCellEventArgs(Cell));
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                if (e.Key == Keys.Enter)
                {
                    Cell.Row.DataGridOwner.OnCellClick(new DataGridCellEventArgs(Cell));
                }
                else if (e.Key == Keys.Space && ((Root.InputManager.KeyboardDevice.Modifiers & (InputModifiers.Control | InputModifiers.Alt)) != InputModifiers.Alt))
                {
                    if ((!IsMouseCaptured) && (e.OriginalSource == this))
                    {
                        Cell.Row.DataGridOwner.OnCellClick(new DataGridCellEventArgs(Cell));
                    }
                }
            }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ClipToBounds), new UIPropertyMetadataAttribute(true, UIPropertyOptions.AffectsRender));
        }
    }
}
