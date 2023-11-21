using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;
using CPF.Input;

namespace CPF.Controls
{
    /// <summary>
    /// 表示一个处理 Thumb 控件的定位和大小调整的控件基元
    /// </summary>
    [Description("表示一个处理 Thumb 控件的定位和大小调整的控件基元")]
    public class Track : Control
    {
        /// <summary>
        /// 表示一个处理 Thumb 控件的定位和大小调整的控件基元
        /// </summary>
        public Track()
        {

        }
        /// <summary>
        /// Calculate the value from given Point. The input point is relative to TopLeft conner of Track.
        /// </summary>
        /// <param name="pt">Point (in Track's co-ordinate).</param>        
        public virtual float ValueFromPoint(Point pt)
        {
            float val;
            // Find distance from center of thumb to given point.
            if (Orientation == Orientation.Horizontal)
            {
                val = Value + ValueFromDistance(pt.X - ThumbCenterOffset, pt.Y - (ActualSize.Height * 0.5f));
            }
            else
            {
                val = Value + ValueFromDistance(pt.X - (ActualSize.Width * 0.5f), pt.Y - ThumbCenterOffset);
            }
            return Math.Max(Minimum, Math.Min(Maximum, val));
        }

        /// <summary>
        /// This function returns the delta in value that would be caused by moving the thumb the given pixel distances.
        /// The returned delta value is not guaranteed to be inside the valid Value range.
        /// </summary>
        /// <param name="horizontal">Total horizontal distance that the Thumb has moved.</param>
        /// <param name="vertical">Total vertical distance that the Thumb has moved.</param>        
        public virtual float ValueFromDistance(float horizontal, float vertical)
        {
            float scale = IsDirectionReversed ? -1 : 1;
            //
            // Note: To implement 'Snap-Back' feature, we could check whether the point is far away from center of the track.
            // If so, just return current value (this should move the Thumb back to its original localtion).
            //
            if (Orientation == Orientation.Horizontal)
            {
                return scale * horizontal * Density;
            }
            else
            {
                // Increases in y cause decreases in Sliders value
                return -1 * scale * vertical * Density;
            }
        }



        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //-------------------------------------------------------------------

        #region Public Properties

        private void UpdateComponent(UIElement oldValue, UIElement newValue)
        {
            if (oldValue != newValue)
            {

                if (oldValue != null)
                {
                    // notify the visual layer that the old component has been removed.
                    Children.Remove(oldValue);
                }

                Children.Add(newValue);

                //InvalidateMeasure();
                //InvalidateArrange();
            }
        }

        /// <summary>
        /// The RepeatButton used to decrease the Value 减
        /// </summary>
        [NotCpfProperty]
        public RepeatButton DecreaseRepeatButton
        {
            get
            {
                return _decreaseButton;
            }
            set
            {
                //if (_increaseButton == value)
                //{
                //    throw new NotSupportedException(SR.Get(SRID.Track_SameButtons));
                //}
                UpdateComponent(_decreaseButton, value);
                _decreaseButton = value;

                //if (_decreaseButton != null)
                //{
                //    CommandManager.InvalidateRequerySuggested(); // Should post an idle queue item to update IsEnabled on button
                //}
            }
        }

        /// <summary>
        /// The Thumb in the Track
        /// </summary>
        [NotCpfProperty]
        public Thumb Thumb
        {
            get
            {
                return _thumb;
            }
            set
            {
                UpdateComponent(_thumb, value);
                if (_thumb != null)
                {
                    //_thumb.DragStarted -= thumb_DragStarted;
                    _thumb.DragDelta -= thumb_DragDelta;
                    //_thumb.DragCompleted -= thumb_DragCompleted;
                }
                if (value != null)
                {
                    //value.DragStarted += thumb_DragStarted;
                    value.DragDelta += thumb_DragDelta;
                    //value.DragCompleted += thumb_DragCompleted;
                }
                _thumb = value;
            }
        }

        //private void thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        //{

        //}

        private void thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Value += ValueFromDistance(e.HorizontalChange, e.VerticalChange);
        }

        //private void thumb_DragStarted(object sender, DragStartedEventArgs e)
        //{

        //}

        /// <summary>
        /// The RepeatButton used to increase the Value 加
        /// </summary>
        [NotCpfProperty]
        public RepeatButton IncreaseRepeatButton
        {
            get
            {
                return _increaseButton;
            }
            set
            {
                //if (_decreaseButton == value)
                //{
                //    throw new NotSupportedException(SR.Get(SRID.Track_SameButtons));
                //}
                UpdateComponent(_increaseButton, value);
                _increaseButton = value;

                //if (_increaseButton != null)
                //{
                //    CommandManager.InvalidateRequerySuggested(); // Should post an idle queue item to update IsEnabled on button
                //}
            }
        }


        /// <summary>
        /// This property represents the Track layout orientation: Vertical or Horizontal.
        /// On vertical ScrollBars, the thumb moves up and down.  On horizontal bars, the thumb moves left to right.
        /// </summary>
        [UIPropertyMetadata(Orientation.Horizontal, UIPropertyOptions.AffectsMeasure)]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(); }
            set { SetValue(value); }
        }


        /// <summary>
        /// The Minimum value of the Slider or ScrollBar
        /// </summary>
        [UIPropertyMetadata(0f, UIPropertyOptions.AffectsArrange)]
        public float Minimum
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// The Maximum value of the Slider or ScrollBar
        /// </summary>
        [UIPropertyMetadata(1f, UIPropertyOptions.AffectsArrange)]
        public float Maximum
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }


        /// <summary>
        /// The current value of the Slider or ScrollBar
        /// </summary>
        [UIPropertyMetadata(0f, UIPropertyOptions.AffectsArrange)]
        public float Value
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }


        /// <summary>
        /// ViewportSize is the amount of the scrolled extent currently visible.  For most scrolled content, this value
        /// will be bound to one of <see cref="ScrollViewer" />'s ViewportSize properties.
        /// This property is in logical scrolling units.
        /// 
        /// Setting this value to NaN will turn off automatic sizing of the thumb
        /// </summary>
        [UIPropertyMetadata(float.NaN, UIPropertyOptions.AffectsArrange)]
        public float ViewportSize
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }

        private static bool IsValidViewport(object o)
        {
            float d = (float)o;
            return d >= 0.0 || float.IsNaN(d);
        }

        /// <summary>
        /// Indicates if the location of the DecreaseRepeatButton and IncreaseRepeatButton 
        /// should be swapped.
        /// </summary>
        public bool IsDirectionReversed
        {
            get { return (bool)GetValue(); }
            set { SetValue(value); }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //-------------------------------------------------------------------

        #region Protected Methods

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(Value))
            {
                var v = (float)Convert.ChangeType(value, typeof(float));
                var min = Minimum;
                if (v < min)
                {
                    value = min;
                }
                else
                {
                    var max = Maximum;
                    if (v > max)
                    {
                        value = max;
                    }
                }
            }
            else if (propertyName == nameof(Maximum))
            {
                var max = (float)Convert.ChangeType(value, typeof(float));
                if (max < Minimum)
                {
                    value = Minimum;
                }
                if (Value > max)
                {
                    Value = max;
                }
            }
            else if (propertyName == nameof(Minimum))
            {
                var min = (float)Convert.ChangeType(value, typeof(float));
                if (min > Maximum)
                {
                    value = Maximum;
                }
                if (Value < min)
                {
                    Value = min;
                }
            }
            return base.OnSetValue(propertyName, ref value);
        }

        /// <summary>
        /// The desired size of a Track is the width (if vertically oriented) or height (if horizontally
        /// oriented) of the Thumb.  
        ///
        /// When ViewportSize is NaN:
        ///    The thumb is measured to find the other dimension.  
        /// Otherwise:
        ///    Zero size is returned; Track can scale to any size along its children.
        ///    This means that it will occupy no space (and not display) unless made larger by a parent or specified size.
        /// <seealso cref="UIElement.MeasureOverride" />
        /// </summary>
        protected override Size MeasureOverride(in Size availableSize)
        {
            Size desiredSize = new Size(0f, 0f);

            // Only measure thumb
            // Repeat buttons will be sized based on thumb
            if (Thumb != null)
            {
                Thumb.Measure(availableSize);
                desiredSize = Thumb.DesiredSize;
            }

            if (!float.IsNaN(ViewportSize))
            {
                // ScrollBar can shrink to 0 in the direction of scrolling
                if (Orientation == Orientation.Vertical)
                    desiredSize.Height = 0f;
                else
                    desiredSize.Width = 0f;
            }

            return desiredSize;
        }

        // Force length of one of track's pieces to be > 0 and less than tracklength
        private static void CoerceLength(ref float componentLength, float trackLength)
        {
            if (componentLength < 0)
            {
                componentLength = 0f;
            }
            else if (componentLength > trackLength || float.IsNaN(componentLength))
            {
                componentLength = trackLength;
            }
        }

        /// <summary>
        /// Children will be stretched to fit horizontally (if vertically oriented) or vertically (if horizontally 
        /// oriented).
        /// 
        /// There are essentially three possible layout states:
        /// 1. The track is enabled and the thumb is proportionally sizing.
        /// 2. The track is enabled and the thumb has reached its minimum size. 
        /// 3. The track is disabled or there is not enough room for the thumb. 
        ///    Track elements are not displayed, and will not be arranged.
        /// <seealso cref="UIElement.ArrangeOverride" />
        /// </summary>
        protected override Size ArrangeOverride(in Size arrangeSize)
        {
            float decreaseButtonLength, thumbLength, increaseButtonLength;

            bool isVertical = (Orientation == Orientation.Vertical);


            var viewportSize = Math.Max(0f, ViewportSize);

            // If viewport is NaN, compute thumb's size based on its desired size,
            // otherwise compute the thumb base on the viewport and extent properties
            if (float.IsNaN(viewportSize))
            {
                ComputeSliderLengths(arrangeSize, isVertical, out decreaseButtonLength, out thumbLength, out increaseButtonLength);
            }
            else
            {
                // Don't arrange if there's not enough content or the track is too small
                if (!ComputeScrollBarLengths(arrangeSize, viewportSize, isVertical, out decreaseButtonLength, out thumbLength, out increaseButtonLength))
                {
                    return arrangeSize;
                }
            }

            // Layout the pieces of track

            Point offset = new Point();
            Size pieceSize = arrangeSize;
            bool isDirectionReversed = IsDirectionReversed;

            if (isVertical)
            {
                // Vertical Normal   :    |Inc Button |
                //                        |Thumb      |
                //                        |Dec Button | 
                // Vertical Reversed :    |Dec Button |
                //                        |Thumb      |
                //                        |Inc Button | 

                CoerceLength(ref decreaseButtonLength, arrangeSize.Height);
                CoerceLength(ref increaseButtonLength, arrangeSize.Height);
                CoerceLength(ref thumbLength, arrangeSize.Height);

                offset.Y = isDirectionReversed ? decreaseButtonLength + thumbLength : 0f;
                pieceSize.Height = increaseButtonLength;

                if (IncreaseRepeatButton != null)
                    IncreaseRepeatButton.Arrange(new Rect(offset, pieceSize));


                offset.Y = isDirectionReversed ? 0f : increaseButtonLength + thumbLength;
                pieceSize.Height = decreaseButtonLength;

                if (DecreaseRepeatButton != null)
                    DecreaseRepeatButton.Arrange(new Rect(offset, pieceSize));


                offset.Y = isDirectionReversed ? decreaseButtonLength : increaseButtonLength;
                pieceSize.Height = thumbLength;

                if (Thumb != null)
                    Thumb.Arrange(new Rect(offset, pieceSize));

                ThumbCenterOffset = offset.Y + (thumbLength * 0.5f);
            }
            else
            {
                // Horizontal Normal   :    |Dec Button |Thumb| Inc Button| 
                // Horizontal Reversed :    |Inc Button |Thumb| Dec Button|

                CoerceLength(ref decreaseButtonLength, arrangeSize.Width);
                CoerceLength(ref increaseButtonLength, arrangeSize.Width);
                CoerceLength(ref thumbLength, arrangeSize.Width);

                offset.X = isDirectionReversed ? increaseButtonLength + thumbLength : 0f;
                pieceSize.Width = decreaseButtonLength;

                if (DecreaseRepeatButton != null)
                    DecreaseRepeatButton.Arrange(new Rect(offset, pieceSize));


                offset.X = isDirectionReversed ? 0f : decreaseButtonLength + thumbLength;
                pieceSize.Width = increaseButtonLength;

                if (IncreaseRepeatButton != null)
                    IncreaseRepeatButton.Arrange(new Rect(offset, pieceSize));


                offset.X = isDirectionReversed ? increaseButtonLength : decreaseButtonLength;
                pieceSize.Width = thumbLength;

                if (Thumb != null)
                    Thumb.Arrange(new Rect(offset, pieceSize));

                ThumbCenterOffset = offset.X + (thumbLength * 0.5f);
            }

            return arrangeSize;
        }


        // Computes the length of the decrease button, thumb and increase button
        // Thumb's size is based on it's desired size
        private void ComputeSliderLengths(Size arrangeSize, bool isVertical, out float decreaseButtonLength, out float thumbLength, out float increaseButtonLength)
        {
            float min = Minimum;
            float range = Math.Max(0f, Maximum - min);
            float offset = Math.Min(range, Value - min);

            float trackLength;

            // Compute thumb size
            if (isVertical)
            {
                trackLength = arrangeSize.Height;
                thumbLength = Thumb == null ? 0 : Thumb.DesiredSize.Height;
            }
            else
            {
                trackLength = arrangeSize.Width;
                thumbLength = Thumb == null ? 0 : Thumb.DesiredSize.Width;
            }

            CoerceLength(ref thumbLength, trackLength);

            float remainingTrackLength = trackLength - thumbLength;

            decreaseButtonLength = remainingTrackLength * offset / range;
            CoerceLength(ref decreaseButtonLength, remainingTrackLength);

            increaseButtonLength = remainingTrackLength - decreaseButtonLength;
            CoerceLength(ref increaseButtonLength, remainingTrackLength);

            //Debug.Assert(decreaseButtonLength >= 0.0 && decreaseButtonLength <= remainingTrackLength, "decreaseButtonLength is outside bounds");
            //Debug.Assert(increaseButtonLength >= 0.0 && increaseButtonLength <= remainingTrackLength, "increaseButtonLength is outside bounds");

            Density = range / remainingTrackLength;
        }

        // Computes the length of the decrease button, thumb and increase button
        // Thumb's size is based on viewport and extent
        // returns false if the track should be hidden
        private bool ComputeScrollBarLengths(Size arrangeSize, float viewportSize, bool isVertical, out float decreaseButtonLength, out float thumbLength, out float increaseButtonLength)
        {
            var min = Minimum;
            var range = Math.Max(0f, Maximum - min);
            var offset = Math.Min(range, Value - min);

            //Debug.Assert(DoubleUtil.GreaterThanOrClose(offset, 0.0), "Invalid offest (negative value).");

            var extent = Math.Max(0f, range) + viewportSize;

            float trackLength;

            // Compute thumb size
            float thumbMinLength;
            if (isVertical)
            {
                trackLength = arrangeSize.Height;
                var buttonHeight = 18f;
                thumbMinLength = (float)Math.Floor(buttonHeight * 0.5f);
            }
            else
            {
                trackLength = arrangeSize.Width;
                // Try to use the apps resource if it exists, fall back to SystemParameters if it doesn't
                float buttonWidth = 18;
                thumbMinLength = (float)Math.Floor(buttonWidth * 0.5);
            }

            thumbLength = trackLength * viewportSize / extent;
            CoerceLength(ref thumbLength, trackLength);

            thumbLength = Math.Max(thumbMinLength, thumbLength);


            // If we don't have enough content to scroll, disable the track.
            bool notEnoughContentToScroll = DoubleUtil.LessThanOrClose(range, 0.0);
            bool thumbLongerThanTrack = thumbLength > trackLength;

            // if there's not enough content or the thumb is longer than the track, 
            // hide the track and don't arrange the pieces
            if (notEnoughContentToScroll || thumbLongerThanTrack)
            {
                if (Visibility != Visibility.Hidden)
                {
                    Visibility = Visibility.Hidden;
                }

                ThumbCenterOffset = float.NaN;
                Density = float.NaN;
                decreaseButtonLength = 0f;
                increaseButtonLength = 0f;
                return false; // don't arrange
            }
            else if (Visibility != Visibility.Visible)
            {
                Visibility = Visibility.Visible;
            }

            // Compute lengths of increase and decrease button
            var remainingTrackLength = trackLength - thumbLength;
            decreaseButtonLength = remainingTrackLength * offset / range;
            CoerceLength(ref decreaseButtonLength, remainingTrackLength);

            increaseButtonLength = remainingTrackLength - decreaseButtonLength;
            CoerceLength(ref increaseButtonLength, remainingTrackLength);

            Density = range / remainingTrackLength;

            return true;
        }

        #endregion


        //-------------------------------------------------------------------
        //
        //  Private Properties
        //
        //-------------------------------------------------------------------

        #region Private Properties

        private float ThumbCenterOffset
        {
            get { return _thumbCenterOffset; }
            set { _thumbCenterOffset = value; }
        }
        private float Density
        {
            get { return _density; }
            set { _density = value; }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Fields
        //
        //-------------------------------------------------------------------

        #region Private Fields

        private RepeatButton _increaseButton;
        private RepeatButton _decreaseButton;
        private Thumb _thumb;

        // Density of scrolling units present in 1/96" of track (not thumb).  Computed during ArrangeOverride.
        // Note that density default really *is* NaN.  This corresponds to no track having been computed/displayed.
        private float _density = float.NaN;
        private float _thumbCenterOffset = float.NaN;

        #endregion
    }
}
