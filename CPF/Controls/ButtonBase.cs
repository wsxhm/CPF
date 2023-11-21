using System;
using System.Collections.Generic;
using System.Text;
using CPF.Input;
using CPF.Drawing;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示所有 Button 控件的基类。
    /// </summary>
    [Description("表示所有 Button 控件的基类。"), Browsable(false)]
    public abstract class ButtonBase : ContentControl
    {
        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Focusable), new PropertyMetadataAttribute(true));
        }

        bool isMouseLeftButtonPressed;
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!e.Handled)
            {
                if (e.MouseButton == MouseButton.Left)
                {
                    isMouseLeftButtonPressed = true;
                    // Ignore when in hover-click mode.
                    if (ClickMode != ClickMode.Hover)
                    {
                        e.Handled = true;

                        // Always set focus on itself
                        // In case ButtonBase is inside a nested focus scope we should restore the focus OnLostMouseCapture
                        if (Focusable)
                        {
                            Focus(NavigationMethod.Click);
                        }

                        // It is possible that the mouse state could have changed during all of
                        // the call-outs that have happened so far.
                        if (e.ButtonState == MouseButtonState.Pressed)
                        {
                            // Capture the mouse, and make sure we got it.
                            // WARNING: callout
                            CaptureMouse();
                            if (IsMouseCaptured)
                            {
                                // Though we have already checked this state, our call to CaptureMouse
                                // could also end up changing the state, so we check it again.
                                if (e.ButtonState == MouseButtonState.Pressed)
                                {
                                    if (!IsPressed)
                                    {
                                        IsPressed = true;
                                    }
                                }
                                else
                                {
                                    // Release capture since we decided not to press the button.
                                    ReleaseMouseCapture();
                                }
                            }
                        }

                        if (ClickMode == ClickMode.Press)
                        {
                            bool exceptionThrown = true;
                            try
                            {
                                OnClick();
                                exceptionThrown = false;
                            }
                            finally
                            {
                                if (exceptionThrown)
                                {
                                    // Cleanup the buttonbase state
                                    IsPressed = false;
                                    ReleaseMouseCapture();
                                }
                            }
                        }
                    }
                }
            }

        }
        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);
        //    if ((ClickMode != ClickMode.Hover) &&
        //        ((IsMouseCaptured && (e.LeftButton == MouseButtonState.Pressed) && !IsSpaceKeyDown)))
        //    {
        //        UpdateIsPressed();

        //        e.Handled = true;
        //    }
        //}
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.MouseButton == MouseButton.Left)
            {
                isMouseLeftButtonPressed = false;
            }
            if (!e.Handled)
            {
                if (e.MouseButton == MouseButton.Left)
                {
                    if (ClickMode != ClickMode.Hover)
                    {
                        e.Handled = true;
                        bool shouldClick = !IsSpaceKeyDown && IsPressed && ClickMode == ClickMode.Release;

                        if (IsMouseCaptured && !IsSpaceKeyDown)
                        {
                            ReleaseMouseCapture();
                        }

                        if (shouldClick)
                        {
                            var l = e.Location;
                            var r = GetContentBounds();
                            if (l.X >= 0 && l.Y >= 0 && l.X <= r.Width && l.Y <= r.Height)
                            {
                                OnClick();
                            }
                        }
                    }
                }
            }
        }

        bool IsSpaceKeyDown = false;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (ClickMode == ClickMode.Hover || e.Handled)
            {
                // Ignore when in hover-click mode.
                return;
            }

            if (e.Key == Keys.Space)
            {
                // Alt+Space should bring up system menu, we shouldn't handle it.
                if ((Root.InputManager.KeyboardDevice.Modifiers & (InputModifiers.Control | InputModifiers.Alt)) != InputModifiers.Alt)
                {
                    if ((!IsMouseCaptured) && (e.OriginalSource == this))
                    {
                        IsSpaceKeyDown = true;
                        IsPressed = true;
                        CaptureMouse();

                        if (ClickMode == ClickMode.Press)
                        {
                            OnClick();
                        }

                        e.Handled = true;
                    }
                }
            }
            else if (e.Key == Keys.Enter)//&& (bool)GetValue(KeyboardNavigation.AcceptsReturnProperty)
            {
                if (e.OriginalSource == this)
                {
                    IsSpaceKeyDown = false;
                    IsPressed = false;
                    if (IsMouseCaptured)
                    {
                        ReleaseMouseCapture();
                    }

                    OnClick();
                    e.Handled = true;
                }
            }
            else
            {
                // On any other key we set IsPressed to false only if Space key is pressed
                if (IsSpaceKeyDown)
                {
                    IsPressed = false;
                    IsSpaceKeyDown = false;
                    if (IsMouseCaptured)
                    {
                        ReleaseMouseCapture();
                    }
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (ClickMode == ClickMode.Hover || e.Handled)
            {
                // Ignore when in hover-click mode.
                return;
            }

            if ((e.Key == Keys.Space) && IsSpaceKeyDown)
            {
                // Alt+Space should bring up system menu, we shouldn't handle it.
                if ((Root.InputManager.KeyboardDevice.Modifiers & (InputModifiers.Control | InputModifiers.Alt)) != InputModifiers.Alt)
                {
                    IsSpaceKeyDown = false;
                    if (!isMouseLeftButtonPressed)
                    {
                        bool shouldClick = IsPressed && ClickMode == ClickMode.Release;

                        // Release mouse capture if left mouse button is not pressed
                        if (IsMouseCaptured)
                        {
                            // OnLostMouseCapture set IsPressed to false
                            ReleaseMouseCapture();
                        }

                        if (shouldClick)
                            OnClick();
                    }
                    else
                    {
                        // IsPressed state is updated only if mouse is captured (bugfix 919349)
                        //if (IsMouseCaptured)
                        //    UpdateIsPressed();
                    }

                    e.Handled = true;
                }
            }
        }
        protected override void OnReleaseMouseCapture()
        {
            base.OnReleaseMouseCapture();
            if ((ClickMode != ClickMode.Hover) && !IsSpaceKeyDown)
            {
                // If we are inside a nested focus scope - we should restore the focus to the main focus scope
                // This will cover the scenarios like ToolBar buttons
                //if (IsKeyboardFocused && !IsInMainFocusScope)
                //    Keyboard.Focus(null);

                // When we lose capture, the button should not look pressed anymore
                // -- unless the spacebar is still down, in which case we are still pressed.
                IsPressed = false;
            }
        }
        protected void OnClick()
        {
            //var p1 = PointToScreen(new Point());
            //var p4 = PointToScreen(new Point(ActualSize.Width, ActualSize.Height));
            //var rect = new Rect(p1, p4);
            //if (rect.Contains(Root.InputManager.MouseDevice.Location))
            //{
            OnClick(new RoutedEventArgs(this));
            //}
        }

        protected virtual void OnClick(RoutedEventArgs e)
        {
            RaiseEvent(e, nameof(Click));
        }

        /// <summary>
        /// 获取或设置 Click 事件何时发生。
        /// </summary>
        [PropertyMetadata(ClickMode.Release)]
        public ClickMode ClickMode
        {
            get
            {
                return (ClickMode)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        /// <summary>
        /// 是否按下
        /// </summary>
        public bool IsPressed
        {
            get { return (bool)GetValue(); }
            private set { SetValue(value); }
        }

        public event EventHandler<RoutedEventArgs> Click
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
    }

    /// <summary>
    /// ClickMode specifies when the Click event should fire
    /// </summary>
    public enum ClickMode : byte
    {
        /// <summary>
        /// Used to specify that the Click event will fire on the
        /// normal down->up semantics of Button interaction.
        /// Escaping mechanisms work, too. Capture is taken by the
        /// Button while it is down and released after the
        /// Click is fired.
        /// </summary>
        Release,

        /// <summary>
        /// Used to specify that the Click event should fire on the
        /// down of the Button.  Basically, Click will fire as
        /// soon as the IsPressed property on Button becomes true.
        /// Even if the mouse is held down on the Button, capture
        /// is not taken.
        /// </summary>
        Press,

        /// <summary>
        /// Used to specify that the Click event should fire when the
        /// mouse hovers over a Button.
        /// </summary>
        Hover,

        // NOTE: if you add or remove any values in this enum, be sure to update ButtonBase.IsValidClickMode()
    }
}
