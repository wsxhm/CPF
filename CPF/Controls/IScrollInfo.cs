using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// It exposes scrolling properties, methods for logical scrolling, computing
    /// which children are visible, and measuring/drawing/offsetting/clipping content.    
    /// </summary>
    public interface IScrollInfo
    {
        #region Public Methods

        /// <summary>
        /// Scroll content by one line to the top.
        /// </summary>
        void LineUp();

        /// <summary>
        /// Scroll content by one line to the bottom.
        /// </summary>
        void LineDown();

        /// <summary>
        /// Scroll content by one line to the left.
        /// </summary>
        void LineLeft();

        /// <summary>
        /// Scroll content by one line to the right.
        /// </summary>
        void LineRight();


        /// <summary>
        /// Scroll content by one page to the top.
        /// </summary>
        void PageUp();

        /// <summary>
        /// Scroll content by one page to the bottom.
        /// </summary>
        void PageDown();

        /// <summary>
        /// Scroll content by one page to the left.
        /// </summary>
        void PageLeft();

        /// <summary>
        /// Scroll content by one page to the right.
        /// </summary>
        void PageRight();


        /// <summary>
        /// Scroll content by one page to the top.
        /// </summary>
        void MouseWheelUp();

        /// <summary>
        /// Scroll content by one page to the bottom.
        /// </summary>
        void MouseWheelDown();

        /// <summary>
        /// Scroll content by one page to the left.
        /// </summary>
        void MouseWheelLeft();

        /// <summary>
        /// Scroll content by one page to the right.
        /// </summary>
        void MouseWheelRight();

        /// <summary>
        /// Set the HorizontalOffset to the passed value.  
        /// An implementation may coerce this value into a valid range, typically inclusively between 0 and <see cref="ExtentWidth" /> less <see cref="ViewportWidth" />.
        /// </summary>
        void SetHorizontalOffset(float offset);

        /// <summary>
        /// Set the VerticalOffset to the passed value.  
        /// An implementation may coerce this value into a valid range, typically inclusively between 0 and <see cref="ExtentHeight" /> less <see cref="ViewportHeight" />.
        /// </summary>
        void SetVerticalOffset(float offset);
        
        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //-------------------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// This property indicates to the IScrollInfo whether or not it can scroll in the vertical given dimension.
        /// </summary>
        bool CanVerticallyScroll { get; set; }

        /// <summary>
        /// This property indicates to the IScrollInfo whether or not it can scroll in the horizontal given dimension.
        /// </summary>
        bool CanHorizontallyScroll { get; set; }

        /// <summary>
        /// ExtentWidth contains the full horizontal range of the scrolled content.
        /// </summary>
        float ExtentWidth { get; }

        /// <summary>
        /// ExtentHeight contains the full vertical range of the scrolled content.
        /// </summary>
        float ExtentHeight { get; }

        /// <summary>
        /// ViewportWidth contains the currently visible horizontal range of the scrolled content.
        /// </summary>
        float ViewportWidth { get; }

        /// <summary>
        /// ViewportHeight contains the currently visible vertical range of the scrolled content.
        /// </summary>
        float ViewportHeight { get; }

        /// <summary>
        /// HorizontalOffset is the horizontal offset into the scrolled content that represents the first unit visible.
        /// </summary>
        float HorizontalOffset { get; }

        /// <summary>
        /// VerticalOffset is the vertical offset into the scrolled content that represents the first unit visible.
        /// </summary>
        float VerticalOffset { get; }

        /// <summary>
        /// ScrollOwner is the container that controls any scrollbars, headers, etc... that are dependant
        /// on this IScrollInfo's properties.  Implementers of IScrollInfo should call InvalidateScrollInfo()
        /// on this object when properties change.
        /// </summary>
        ScrollViewer ScrollOwner { get; set; }
        #endregion
        /// <summary>
        /// 滚动或者视口变化
        /// </summary>
        event EventHandler ScrollChanged;
    }
}
