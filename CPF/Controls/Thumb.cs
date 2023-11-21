using System;
using System.Collections.Generic;
using System.Text;
using CPF.Styling;
using CPF.Input;
using CPF.Drawing;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示可由用户拖动的控件
    /// </summary>
    [Description("表示可由用户拖动的控件")]
    public class Thumb : Decorator
    {
        /// <summary>
        /// 表示可由用户拖动的控件
        /// </summary>
        public Thumb() { }

        protected override void InitializeComponent()
        {

        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.MouseButton == MouseButton.Left)
            {
                if (!IsDragging)
                {
                    e.Handled = true;
                    //Focus();
                    CaptureMouse();
                    IsDragging = true;
                    _originThumbPoint = e.Location;
                    _previousScreenCoordPosition = _originScreenCoordPosition = PointToScreen(_originThumbPoint);
                    bool exceptionThrown = true;
                    try
                    {
                        //RaiseEvent(new DragStartedEventArgs(_originThumbPoint.X, _originThumbPoint.Y), nameof(DragStarted));
                        OnDragStarted(new DragStartedEventArgs(_originThumbPoint.X, _originThumbPoint.Y));
                        exceptionThrown = false;
                    }
                    finally
                    {
                        if (exceptionThrown)
                        {
                            CancelDrag();
                        }
                    }
                }
                else
                {
                    // This is weird, Thumb shouldn't get MouseLeftButtonDown event while dragging.
                    // This may be the case that something ate MouseLeftButtonUp event, so Thumb never had a chance to
                    // reset IsDragging property 
                }
            }
        }

        protected virtual void OnDragStarted(DragStartedEventArgs e)
        {
            RaiseEvent(e, nameof(DragStarted));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsDragging)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point thumbCoordPosition = e.Location;
                    // Get client point then convert to screen point
                    Point screenCoordPosition = PointToScreen(thumbCoordPosition);

                    // We will fire DragDelta event only when the mouse is really moved
                    if (screenCoordPosition != _previousScreenCoordPosition)
                    {
                        _previousScreenCoordPosition = screenCoordPosition;
                        e.Handled = true;
                        OnDragDelta(new DragDeltaEventArgs(thumbCoordPosition.X - _originThumbPoint.X,
                                              thumbCoordPosition.Y - _originThumbPoint.Y));
                    }
                }
                else
                {
                    if (e.Device.InputManager.MouseDevice.Captured == this)
                        ReleaseMouseCapture();
                    IsDragging = false;
                    _originThumbPoint.X = 0;
                    _originThumbPoint.Y = 0;
                }
            }
        }

        protected virtual void OnDragDelta(DragDeltaEventArgs eventArgs)
        {
            RaiseEvent(eventArgs, nameof(DragDelta));
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.MouseButton == MouseButton.Left)
            {
                if (IsMouseCaptured && IsDragging)
                {
                    e.Handled = true;
                    IsDragging = false;
                    ReleaseMouseCapture();
                    Point pt = PointToScreen(e.Location);
                    //RaiseEvent(new DragCompletedEventArgs(pt.X - _originScreenCoordPosition.X, pt.Y - _originScreenCoordPosition.Y, false), nameof(DragCompleted));

                    OnDragCompleted(new DragCompletedEventArgs(pt.X - _originScreenCoordPosition.X, pt.Y - _originScreenCoordPosition.Y, false));
                }
            }
        }

        ///// <summary>
        ///// 是否按下
        ///// </summary>
        //public bool IsPressed
        //{
        //    get { return (bool)GetValue(); }
        //    set { SetValue(value); }
        //}
        /// <summary>
        /// 是否在拖拽中
        /// </summary>
        public bool IsDragging
        {
            get { return (bool)GetValue(); }
            protected set { SetValue(value); }
        }
        /// <summary>
        ///     This method cancels the dragging operation.
        /// </summary>
        public void CancelDrag()
        {
            if (IsDragging)
            {
                if (IsMouseCaptured)
                {
                    ReleaseMouseCapture();
                }
                IsDragging = false;
                //RaiseEvent(new DragCompletedEventArgs(_previousScreenCoordPosition.X - _originScreenCoordPosition.X, _previousScreenCoordPosition.Y - _originScreenCoordPosition.Y, true), nameof(DragCompleted));
                OnDragCompleted(new DragCompletedEventArgs(_previousScreenCoordPosition.X - _originScreenCoordPosition.X, _previousScreenCoordPosition.Y - _originScreenCoordPosition.Y, true));
            }
        }

        protected virtual void OnDragCompleted(DragCompletedEventArgs e)
        {
            RaiseEvent(e, nameof(DragCompleted));
        }

        protected override void OnReleaseMouseCapture()
        {
            CancelDrag();
        }


        /// <summary>
        /// The point where the mouse was clicked down (Thumb's co-ordinate).
        /// </summary>
        private Point _originThumbPoint; //

        /// <summary>
        /// The position of the mouse (screen co-ordinate) where the mouse was clicked down.
        /// </summary>
        private Point _originScreenCoordPosition;

        /// <summary>
        /// The position of the mouse (screen co-ordinate) when the previous DragDelta event was fired
        /// </summary>
        private Point _previousScreenCoordPosition;


        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(IsAntiAlias), new UIPropertyMetadataAttribute(false, UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(Background), new UIPropertyMetadataAttribute((ViewFill)"221,221,221", UIPropertyOptions.AffectsRender));
        }

        /// <summary>
        /// Add / Remove DragStartedEvent handler
        /// </summary>
        public event EventHandler<DragStartedEventArgs> DragStarted
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        /// <summary>
        /// Add / Remove DragDeltaEvent handler
        /// </summary>
        public event EventHandler<DragDeltaEventArgs> DragDelta
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        /// <summary>
        /// Add / Remove DragCompletedEvent handler
        /// </summary>
        public event EventHandler<DragCompletedEventArgs> DragCompleted
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }


    }

    public class DragStartedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// This is an instance constructor for the DragStartedEventArgs class.  It
        /// is constructed with a reference to the event being raised.
        /// </summary>
        /// <returns>Nothing.</returns>
        public DragStartedEventArgs(float horizontalOffset, float verticalOffset) : base()
        {
            _horizontalOffset = horizontalOffset;
            _verticalOffset = verticalOffset;
        }

        /// <value>
        /// Read-only access to the horizontal offset (relative to Thumb's co-ordinate).
        /// </value>
        public float HorizontalOffset
        {
            get { return _horizontalOffset; }
        }

        /// <value>
        /// Read-only access to the vertical offset (relative to Thumb's co-ordinate).
        /// </value>
        public float VerticalOffset
        {
            get { return _verticalOffset; }
        }

        private float _horizontalOffset;
        private float _verticalOffset;
    }

    public class DragDeltaEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// This is an instance constructor for the DragDeltaEventArgs class.  It
        /// is constructed with a reference to the event being raised.
        /// </summary>
        /// <returns>Nothing.</returns>
        public DragDeltaEventArgs(float horizontalChange, float verticalChange) : base()
        {
            _horizontalChange = horizontalChange;
            _verticalChange = verticalChange;
        }

        /// <value>
        /// Read-only access to the horizontal change.
        /// </value>
        public float HorizontalChange
        {
            get { return _horizontalChange; }
        }

        /// <value>
        /// Read-only access to the vertical change.
        /// </value>
        public float VerticalChange
        {
            get { return _verticalChange; }
        }

        private float _horizontalChange;
        private float _verticalChange;
    }

    public class DragCompletedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// This is an instance constructor for the DragCompletedEventArgs class.  It
        /// is constructed with a reference to the event being raised.
        /// </summary>
        /// <returns>Nothing.</returns>
        public DragCompletedEventArgs(float horizontalChange, float verticalChange, bool canceled) : base()
        {
            _horizontalChange = horizontalChange;
            _verticalChange = verticalChange;
            _wasCanceled = canceled;
        }

        /// <value>
        /// Read-only access to the horizontal distance between the point where mouse's left-button
        /// was pressed and the point where mouse's left-button was released
        /// </value>
        public float HorizontalChange
        {
            get { return _horizontalChange; }
        }

        /// <value>
        /// Read-only access to the vertical distance between the point where mouse's left-button
        /// was pressed and the point where mouse's left-button was released
        /// </value>
        public float VerticalChange
        {
            get { return _verticalChange; }
        }

        /// <summary>
        /// Read-only access to boolean states whether the drag operation was canceled or not.
        /// </summary>
        /// <value></value>
        public bool Canceled
        {
            get { return _wasCanceled; }
        }

        private float _horizontalChange;
        private float _verticalChange;
        private bool _wasCanceled;
    }

}
