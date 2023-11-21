using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 定义一个区域，从中可以按相对位置水平或垂直排列各个子元素。
    /// </summary>
    [Description("定义一个区域，从中可以按相对位置水平或垂直排列各个子元素。")]
    public class DockPanel : Panel
    {
        /// <summary>
        /// 获取或设置一个值，该值指示一个子元素在父级 DockPanel 中的位置。 附加属性
        /// </summary>
        [Description("获取或设置一个值，该值指示一个子元素在父级 DockPanel 中的位置。 附加属性")]
        public static Attached<Dock> Dock
        {
            get
            {
                return RegisterAttached(Controls.Dock.Left, typeof(DockPanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                 {
                     if (obj is UIElement element && element.Parent != null)
                     {
                         element.Parent.InvalidateMeasure();
                     }
                 });
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示 DockPanel 中的最后一个子元素是否拉伸以填充剩余的可用空间
        /// </summary>
        [UIPropertyMetadata(true, UIPropertyOptions.AffectsMeasure),Description("获取或设置一个值，该值指示 DockPanel 中的最后一个子元素是否拉伸以填充剩余的可用空间")]
        public bool LastChildFill
        {
            get { return (bool)GetValue(); }
            set { SetValue(value); }
        }

        protected override Size MeasureOverride(in Size constraint)
        {
            UIElementCollection children = Children;

            float parentWidth = 0;   // Our current required width due to children thus far.
            float parentHeight = 0;   // Our current required height due to children thus far.
            float accumulatedWidth = 0;   // Total width consumed by children.
            float accumulatedHeight = 0;   // Total height consumed by children.

            for (int i = 0, count = children.Count; i < count; ++i)
            {
                UIElement child = children[i];
                Size childConstraint;             // Contains the suggested input constraint for this child.
                Size childDesiredSize;            // Contains the return size from child measure.

                if (child == null) { continue; }

                // Child constraint is the remaining size; this is total size minus size consumed by previous children.
                childConstraint = new Size(Math.Max(0.0f, constraint.Width - accumulatedWidth),
                                           Math.Max(0.0f, constraint.Height - accumulatedHeight));

                // Measure child.
                child.Measure(childConstraint);
                childDesiredSize = child.DesiredSize;

                // Now, we adjust:
                // 1. Size consumed by children (accumulatedSize).  This will be used when computing subsequent
                //    children to determine how much space is remaining for them.
                // 2. Parent size implied by this child (parentSize) when added to the current children (accumulatedSize).
                //    This is different from the size above in one respect: A Dock.Left child implies a height, but does
                //    not actually consume any height for subsequent children.
                // If we accumulate size in a given dimension, the next child (or the end conditions after the child loop)
                // will deal with computing our minimum size (parentSize) due to that accumulation.
                // Therefore, we only need to compute our minimum size (parentSize) in dimensions that this child does
                //   not accumulate: Width for Top/Bottom, Height for Left/Right.
                switch (DockPanel.Dock(child))
                {
                    case Controls.Dock.Left:
                    case Controls.Dock.Right:
                        parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                        accumulatedWidth += childDesiredSize.Width;
                        break;

                    case Controls.Dock.Top:
                    case Controls.Dock.Bottom:
                        parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                        accumulatedHeight += childDesiredSize.Height;
                        break;
                }
            }

            // Make sure the final accumulated size is reflected in parentSize.
            parentWidth = Math.Max(parentWidth, accumulatedWidth);
            parentHeight = Math.Max(parentHeight, accumulatedHeight);

            return (new Size(parentWidth, parentHeight));
        }

        /// <summary>
        /// DockPanel computes a position and final size for each of its children based upon their
        /// <see cref="Controls.Dock" /> enum and sizing properties.
        /// </summary>
        /// <param name="arrangeSize">Size that DockPanel will assume to position children.</param>
        protected override Size ArrangeOverride(in Size arrangeSize)
        {
            UIElementCollection children = Children;
            int totalChildrenCount = children.Count;
            int nonFillChildrenCount = totalChildrenCount - (LastChildFill ? 1 : 0);

            float accumulatedLeft = 0;
            float accumulatedTop = 0;
            float accumulatedRight = 0;
            float accumulatedBottom = 0;

            for (int i = 0; i < totalChildrenCount; ++i)
            {
                UIElement child = children[i];
                if (child == null) { continue; }

                Size childDesiredSize = child.DesiredSize;
                Rect rcChild = new Rect(
                    accumulatedLeft,
                    accumulatedTop,
                    Math.Max(0.0f, arrangeSize.Width - (accumulatedLeft + accumulatedRight)),
                    Math.Max(0.0f, arrangeSize.Height - (accumulatedTop + accumulatedBottom)));

                if (i < nonFillChildrenCount)
                {
                    switch (DockPanel.Dock(child))
                    {
                        case Controls.Dock.Left:
                            accumulatedLeft += childDesiredSize.Width;
                            rcChild.Width = childDesiredSize.Width;
                            break;

                        case Controls.Dock.Right:
                            accumulatedRight += childDesiredSize.Width;
                            rcChild.X = Math.Max(0.0f, arrangeSize.Width - accumulatedRight);
                            rcChild.Width = childDesiredSize.Width;
                            break;

                        case Controls.Dock.Top:
                            accumulatedTop += childDesiredSize.Height;
                            rcChild.Height = childDesiredSize.Height;
                            break;

                        case Controls.Dock.Bottom:
                            accumulatedBottom += childDesiredSize.Height;
                            rcChild.Y = Math.Max(0.0f, arrangeSize.Height - accumulatedBottom);
                            rcChild.Height = childDesiredSize.Height;
                            break;
                    }
                }

                child.Arrange(rcChild);
            }

            return (arrangeSize);
        }

    }
}
