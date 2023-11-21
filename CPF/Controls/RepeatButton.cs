using System;
using System.Collections.Generic;
using System.Text;
using CPF.Threading;
using CPF.Input;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示从按下按钮到释放按钮的时间内重复引发其 Click 事件的控件。
    /// </summary>
    [Description("表示从按下按钮到释放按钮的时间内重复引发其 Click 事件的控件。"), Browsable(true)]
    public class RepeatButton : ButtonBase
    {
        /// <summary>
        /// 延迟操作
        /// </summary>
        [PropertyMetadata(250)]
        public int Delay
        {
            get
            {
                return (int)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }


        /// <summary>
        ///     Specifies the amount of time, in milliseconds, between repeats once repeating starts.
        /// Must be non-negative
        /// </summary>
        [PropertyMetadata(100)]
        public int Interval
        {
            get
            {
                return (int)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        #region Private helpers

        //private static bool IsDelayValid(object value) { return ((int)value) >= 0; }
        //private static bool IsIntervalValid(object value) { return ((int)value) > 0; }

        /// <summary>
        /// Starts a _timer ticking
        /// </summary>
        private void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Tick += new EventHandler(OnTimeout);
            }
            else if (_timer.IsEnabled)
                return;

            _timer.Interval = TimeSpan.FromMilliseconds(Delay);
            _timer.Start();
        }

        /// <summary>
        /// Stops a _timer that has already started
        /// </summary>
        private void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// This is the handler for when the repeat _timer expires. All we do
        /// is invoke a click.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void OnTimeout(object sender, EventArgs e)
        {
            TimeSpan interval = TimeSpan.FromMilliseconds(Interval);
            if (_timer.Interval != interval)
                _timer.Interval = interval;
            if (IsPressed)
            {
                if (Root != null)
                {
                    if (isMouseOver)
                    {
                        OnClick();
                    }
                }
                else
                {
                    StopTimer();
                }
            }
        }


        #endregion Private helpers

        #region Override methods


        ///// <summary>
        ///// Raises InvokedAutomationEvent and call the base method to raise the Click event
        ///// </summary>
        ///// <ExternalAPI/>
        //protected override void OnClick()
        //{
        //    System.Diagnostics.Debug.WriteLine("OnClick");
        //    base.OnClick();
        //}

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.MouseButton == MouseButton.Left)
            {
                if (ClickMode != ClickMode.Hover)
                {
                    StartTimer();
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.MouseButton == MouseButton.Left)
            {
                if (ClickMode != ClickMode.Hover)
                {
                    StopTimer();
                }
            }
        }

        protected override void OnReleaseMouseCapture()
        {
            base.OnReleaseMouseCapture();
            StopTimer();
        }

        /// <summary>
        ///     An event reporting the mouse entered this element.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (HandleIsMouseOverChanged())
            {
                e.Handled = true;
            }
        }

        /// <summary>
        ///     An event reporting the mouse left this element.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (HandleIsMouseOverChanged())
            {
                e.Handled = true;
            }
        }

        /// <summary>
        ///     An event reporting that the IsMouseOver property changed.
        /// </summary>
        private bool HandleIsMouseOverChanged()
        {
            if (ClickMode == ClickMode.Hover)
            {
                if (IsMouseOver)
                {
                    StartTimer();
                }
                else
                {
                    StopTimer();
                }

                return true;
            }

            return false;
        }

        bool isMouseOver;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var l = e.Location;
            var r = GetContentBounds();
            isMouseOver = l.X >= 0 && l.Y >= 0 && l.X <= r.Width && l.Y <= r.Height;
        }

        /// <summary>
        /// This is the method that responds to the KeyDown event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if ((e.Key == Keys.Space) && (ClickMode != ClickMode.Hover))
            {
                StartTimer();
            }
        }

        /// <summary>
        /// This is the method that responds to the KeyUp event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if ((e.Key == Keys.Space) && (ClickMode != ClickMode.Hover))
            {
                StopTimer();
            }
            base.OnKeyUp(e);
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Focusable), new PropertyMetadataAttribute(false));
        }

        #endregion

        #region Data

        private DispatcherTimer _timer;

        #endregion
    }
}
