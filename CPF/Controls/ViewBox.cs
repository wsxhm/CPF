using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 定义一个内容修饰器，以便拉伸或缩放单一子项使其填满可用的控件。
    /// </summary>
    [Description("定义一个内容修饰器，以便拉伸或缩放单一子项使其填满可用的控件。")]
    public class Viewbox : Decorator
    {
        /// <summary>
        /// 定义一个内容修饰器，以便拉伸或缩放单一子项使其填满可用的控件。
        /// </summary>
        public Viewbox() : base()
        {
        }

        #region Public Properties
        /// <summary>
        /// 获取或设置 ViewboxStretch 模式，该模式确定内容适应可用空间的方式。
        /// </summary>
        [Description("获取或设置 ViewboxStretch 模式，该模式确定内容适应可用空间的方式。")]
        [UIPropertyMetadata(Stretch.Uniform, UIPropertyOptions.AffectsMeasure)]
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置 StretchDirection，它确定缩放如何应用 Viewbox 的内容。
        /// </summary>
        [Description("获取或设置 StretchDirection，它确定缩放如何应用 Viewbox 的内容。")]
        [UIPropertyMetadata(StretchDirection.Both, UIPropertyOptions.AffectsMeasure)]
        public StretchDirection StretchDirection
        {
            get { return (StretchDirection)GetValue(); }
            set { SetValue(value); }
        }

        #endregion Public Properties

        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //-------------------------------------------------------------------

        #region Protected Methods

        protected override Size MeasureOverride(in Size constraint)
        {
            Size parentSize = new Size();

            foreach (var child in Children)
            {
                // Initialize child constraint to infinity.  We need to get a "natural" size for the child in absence of constraint.
                // Note that an author *can* impose a constraint on a child by using Height/Width, &c... properties 
                Size infinteConstraint = new Size(float.PositiveInfinity, float.PositiveInfinity);

                child.Measure(infinteConstraint);
                Size childSize = child.DesiredSize;

                Size scalefac = ComputeScaleFactor(constraint, childSize, this.Stretch, this.StretchDirection);

                parentSize.Width = Math.Max(parentSize.Width, scalefac.Width * childSize.Width);
                parentSize.Height = Math.Max(parentSize.Height, scalefac.Height * childSize.Height);
            }

            return parentSize;

        }

        protected override Size ArrangeOverride(in Size arrangeSize)
        {
            var size = arrangeSize;
            foreach (var child in Children)
            {
                Size childSize = child.DesiredSize;

                // Compute scaling factors from arrange size and the measured child content size
                Size scalefac = ComputeScaleFactor(arrangeSize, childSize, this.Stretch, this.StretchDirection);

                child.RenderTransformOrigin = new PointField(0, 0);
                child.RenderTransform = new ScaleTransform(scalefac.Width, scalefac.Height);

                // Arrange the child to the desired size 
                child.Arrange(new Rect(new Point(), child.DesiredSize));

                //return the size oocupied by scaled child
                size.Width = Math.Max(size.Width, scalefac.Width * childSize.Width);
                size.Height = Math.Max(size.Height, scalefac.Height * childSize.Height);
            }
            return size;
        }



        /// <summary>
        /// This is a helper function that computes scale factors depending on a target size and a content size
        /// </summary>
        /// <param name="availableSize">Size into which the content is being fitted.</param>
        /// <param name="contentSize">Size of the content, measured natively (unconstrained).</param>
        /// <param name="stretch">Value of the Stretch property on the element.</param>
        /// <param name="stretchDirection">Value of the StretchDirection property on the element.</param>
        internal static Size ComputeScaleFactor(Size availableSize,
                                                Size contentSize,
                                                Stretch stretch,
                                                StretchDirection stretchDirection)
        {
            // Compute scaling factors to use for axes
            var scaleX = 1.0f;
            var scaleY = 1.0f;

            bool isConstrainedWidth = !float.IsPositiveInfinity(availableSize.Width);
            bool isConstrainedHeight = !float.IsPositiveInfinity(availableSize.Height);

            if ((stretch == Stretch.Uniform || stretch == Stretch.UniformToFill || stretch == Stretch.Fill)
                 && (isConstrainedWidth || isConstrainedHeight))
            {
                // Compute scaling factors for both axes
                scaleX = (FloatUtil.IsZero(contentSize.Width)) ? 0.0f : availableSize.Width / contentSize.Width;
                scaleY = (FloatUtil.IsZero(contentSize.Height)) ? 0.0f : availableSize.Height / contentSize.Height;

                if (!isConstrainedWidth) scaleX = scaleY;
                else if (!isConstrainedHeight) scaleY = scaleX;
                else
                {
                    // If not preserving aspect ratio, then just apply transform to fit
                    switch (stretch)
                    {
                        case Stretch.Uniform:       //Find minimum scale that we use for both axes
                            var minscale = scaleX < scaleY ? scaleX : scaleY;
                            scaleX = scaleY = minscale;
                            break;

                        case Stretch.UniformToFill: //Find maximum scale that we use for both axes
                            var maxscale = scaleX > scaleY ? scaleX : scaleY;
                            scaleX = scaleY = maxscale;
                            break;

                        case Stretch.Fill:          //We already computed the fill scale factors above, so just use them
                            break;
                    }
                }

                //Apply stretch direction by bounding scales.
                //In the uniform case, scaleX=scaleY, so this sort of clamping will maintain aspect ratio
                //In the uniform fill case, we have the same result too.
                //In the fill case, note that we change aspect ratio, but that is okay
                switch (stretchDirection)
                {
                    case StretchDirection.UpOnly:
                        if (scaleX < 1.0) scaleX = 1.0f;
                        if (scaleY < 1.0) scaleY = 1.0f;
                        break;

                    case StretchDirection.DownOnly:
                        if (scaleX > 1.0) scaleX = 1.0f;
                        if (scaleY > 1.0) scaleY = 1.0f;
                        break;

                    case StretchDirection.Both:
                        break;

                    default:
                        break;
                }
            }
            //Return this as a size now
            return new Size(scaleX, scaleY);
        }

        #endregion Protected Methods



    }
}
