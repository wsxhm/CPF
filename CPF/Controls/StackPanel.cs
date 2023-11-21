using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 将子元素排列成水平或垂直的一行
    /// </summary>
    [Description("将子元素排列成水平或垂直的一行")]
    public class StackPanel : Panel//, IScrollInfo
    {
        protected override Size MeasureOverride(in Size availableSize)
        {
            if (Children.Count < 1)
            {
                return base.MeasureOverride(availableSize);
            }
            else
            {
                Size contentDesiredSize = new Size();
                if (Orientation == Orientation.Horizontal)
                {
                    Size avSize = new Size(float.PositiveInfinity, availableSize.Height);
                    foreach (UIElement item in Children)
                    {
                        item.Measure(avSize);
                        contentDesiredSize.Width += item.DesiredSize.Width;
                        //contentDesiredSize.Width = Math.Max(contentDesiredSize.Width, item.DesiredSize.Width);
                        contentDesiredSize.Height = Math.Max(contentDesiredSize.Height, item.DesiredSize.Height);
                    }
                }
                else
                {
                    Size avSize = new Size(availableSize.Width, float.PositiveInfinity);
                    foreach (UIElement item in Children)
                    {
                        item.Measure(avSize);
                        contentDesiredSize.Height += item.DesiredSize.Height;
                        contentDesiredSize.Width = Math.Max(contentDesiredSize.Width, item.DesiredSize.Width);
                        //contentDesiredSize.Height = Math.Max(contentDesiredSize.Height, item.DesiredSize.Height);
                    }
                }
                return contentDesiredSize;
            }
        }

        //protected override void OnChildDesiredSizeChanged(UIElement child)
        //{
        //    base.OnChildDesiredSizeChanged(child);
        //}

        protected override Size ArrangeOverride(in Size finalSize)
        {
            var rect = new Rect(0, 0, finalSize.Width, finalSize.Height);

            var fSize = finalSize;
            float ExtentWidth = 0;
            float ExtentHeight = 0;
            if (Orientation == Orientation.Horizontal)
            {
                foreach (UIElement child in Children)
                {
                    rect.Width = child.DesiredSize.Width;
                    child.Arrange(rect);
                    //if (child.Visibility != Visibility.Collapsed)
                    {
                        var w = child.DesiredSize.Width;
                        //var w = child.ActualSize.Width;
                        //if (!child.MarginLeft.IsAuto)
                        //{
                        //    w += child.MarginLeft.GetActualValue(fSize.Width);
                        //}
                        //if (!child.MarginRight.IsAuto)
                        //{
                        //    w += child.MarginRight.GetActualValue(fSize.Width);
                        //}
                        rect.X += w;
                        ExtentWidth += w;
                    }
                    ExtentHeight = Math.Max(ExtentHeight, rect.Height);
                }
                fSize.Width = ExtentWidth;
            }
            else
            {
                foreach (UIElement child in Children)
                {
                    rect.Height = child.DesiredSize.Height;
                    child.Arrange(rect);
                    //if (child.Visibility != Visibility.Collapsed)
                    {
                        var h = child.DesiredSize.Height;
                        //var h = child.ActualSize.Height;
                        //if (!child.MarginTop.IsAuto)
                        //{
                        //    h += child.MarginTop.GetActualValue(fSize.Height);
                        //}
                        //if (!child.MarginBottom.IsAuto)
                        //{
                        //    h += child.MarginBottom.GetActualValue(fSize.Height);
                        //}
                        rect.Y += h;
                        ExtentHeight += h;
                    }
                    ExtentWidth = Math.Max(ExtentWidth, rect.Width);
                }
                fSize.Height = ExtentHeight;
            }
            return fSize;
        }

        /// <summary>
        /// 布局方向
        /// </summary>
        [UIPropertyMetadata(Orientation.Vertical, UIPropertyOptions.AffectsMeasure)]
        [Description("布局方向")]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(); }
            set { SetValue(value); }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //}
    }
}
