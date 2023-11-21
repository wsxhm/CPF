using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using CPF.Drawing;
using System.Globalization;
using CPF.Controls;
using CPF.Input;
using CPF.Platform;

namespace CPF.Windows
{
    public sealed class NotifyIcon : CpfObject, INotifyIconImpl
    {
        private const int WM_TRAYMOUSEMESSAGE = (int)UnmanagedMethods.WindowsMessage.WM_USER + 1024;
        private static int WM_TASKBARCREATED = UnmanagedMethods.RegisterWindowMessage("TaskbarCreated");

        private object syncObj = new object();

        IntPtr iconHandle;
        //private Image icon = null;
        private string text = "";
        private int id = 0;
        private bool added = false;
        private NotifyIconNativeWindow window = null;
        private static int nextId = 0;
        //private object userData;
        private bool doubleClick = false; // checks if doubleclick is fired

        // Visible defaults to false, but the NotifyIconDesigner makes it seem like the default is 
        // true.  We do this because while visible is the more common case, if it was a true default,
        // there would be no way to create a hidden NotifyIcon without being visible for a moment.
        private bool visible = false;

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.NotifyIcon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.NotifyIcon'/> class.
        ///    </para>
        /// </devdoc>
        public NotifyIcon()
        {
            id = ++nextId;
            window = new NotifyIconNativeWindow(this);
            UpdateIcon(visible);
        }

        /// <include file='doc\NotifyIcon.uex' path='docs/doc[@for="NotifyIcon.BalloonTipClicked"]/*' />
        /// <devdoc>
        ///    <para>[This event is raised on the NIN_BALLOONUSERCLICK message.]</para>
        /// </devdoc>
        public event EventHandler BalloonTipClicked
        {
            add
            {
                AddHandler(value);
            }

            remove
            {
                RemoveHandler(value);
            }
        }

        /// <include file='doc\NotifyIcon.uex' path='docs/doc[@for="NotifyIcon.BalloonTipClosed"]/*' />
        /// <devdoc>
        ///    <para>[This event is raised on the NIN_BALLOONTIMEOUT message.]</para>
        /// </devdoc>
        public event EventHandler BalloonTipClosed
        {
            add
            {
                AddHandler(value);
            }

            remove
            {
                RemoveHandler(value);
            }
        }

        /// <include file='doc\NotifyIcon.uex' path='docs/doc[@for="NotifyIcon.BalloonTipShown"]/*' />
        /// <devdoc>
        ///    <para>[This event is raised on the NIN_BALLOONSHOW or NIN_BALLOONHIDE message.]</para>
        /// </devdoc>
        public event EventHandler BalloonTipShown
        {
            add
            {
                AddHandler(value);
            }
            remove
            {
                RemoveHandler(value);
            }
        }
        [PropertyMetadata("")]
        public string BalloonTipText
        {
            get
            {
                return GetValue<string>();
            }
            set
            {
                SetValue(value);
            }
        }
        public static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue)
        {
            bool valid = (value >= minValue) && (value <= maxValue);
            return valid;

        }
        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.BalloonTipIcon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the BalloonTip icon displayed when
        ///       the mouse hovers over a system tray icon.
        ///    </para>
        /// </devdoc>
        public ToolTipIcon BalloonTipIcon
        {
            get
            {
                return GetValue<ToolTipIcon>();
            }
            set
            {
                //valid values are 0x0 to 0x3
                if (!IsEnumValid(value, (int)value, (int)ToolTipIcon.None, (int)ToolTipIcon.Error))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ToolTipIcon));
                }
                SetValue(value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.BalloonTipTitle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the BalloonTip title displayed when
        ///       the mouse hovers over a system tray icon.
        ///    </para>
        /// </devdoc>
        [PropertyMetadata("")]
        public string BalloonTipTitle
        {
            get
            {
                return GetValue<string>();
            }
            set
            {
                SetValue(value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.ContextMenu"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets context menu
        ///       for the tray icon.
        ///    </para>
        /// </devdoc>
        public ContextMenu ContextMenu
        {
            get
            {
                return GetValue<ContextMenu>();
            }

            set
            {
                SetValue(value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.Icon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the current
        ///       icon.
        ///    </para>
        /// </devdoc>
        public Image Icon
        {
            get
            {
                return GetValue<Image>();
            }
            set
            {
                SetValue(value);
            }
        }
        [PropertyChanged(nameof(Icon))]
        void OnIcon(object newValue, object oldValue, CPF.PropertyMetadataAttribute attribute)
        {
            UpdateIcon(visible);
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.Text"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the ToolTip text displayed when
        ///       the mouse hovers over a system tray icon.
        ///    </para>
        /// </devdoc>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (value == null) value = "";
                if (value != null && !value.Equals(this.text))
                {
                    if (value != null && value.Length > 63)
                    {
                        throw new ArgumentOutOfRangeException("Text", value, "文字太长，不能超过63个字符");
                    }
                    this.text = value;
                    if (added)
                    {
                        UpdateIcon(true);
                    }
                }
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.Visible"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the icon is visible in the Windows System Tray.
        ///    </para>
        /// </devdoc>
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                if (visible != value)
                {
                    UpdateIcon(value);
                    visible = value;
                }
            }
        }


        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.Click"]/*' />
        /// <devdoc>
        ///     Occurs when the user clicks the icon in the system tray.
        /// </devdoc>
        public event EventHandler Click
        {
            add
            {
                AddHandler(value);
            }
            remove
            {
                RemoveHandler(value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.DoubleClick"]/*' />
        /// <devdoc>
        ///     Occurs when the user double-clicks the icon in the system tray.
        /// </devdoc>
        public event EventHandler DoubleClick
        {
            add
            {
                AddHandler(value);
            }
            remove
            {
                RemoveHandler(value);
            }
        }

        public event EventHandler MouseClick
        {
            add
            {
                AddHandler(value);
            }
            remove
            {
                RemoveHandler(value);
            }
        }

        public event EventHandler MouseDoubleClick
        {
            add
            {
                AddHandler(value);
            }
            remove
            {
                RemoveHandler(value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.MouseDown"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the
        ///       user presses a mouse button while the pointer is over the icon in the system tray.
        ///    </para>
        /// </devdoc>
        public event EventHandler<CPF.Input.NotifyIconMouseEventArgs> MouseDown
        {
            add
            {
                AddHandler(value);
            }
            remove
            {
                RemoveHandler(value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.MouseMove"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs
        ///       when the user moves the mouse pointer over the icon in the system tray.
        ///    </para>
        /// </devdoc>
        public event EventHandler<NotifyIconMouseEventArgs> MouseMove
        {
            add
            {
                AddHandler(value);
            }
            remove
            {
                RemoveHandler(value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.MouseUp"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the
        ///       user releases the mouse button while the pointer
        ///       is over the icon in the system tray.
        ///    </para>
        /// </devdoc>
        public event EventHandler<CPF.Input.NotifyIconMouseEventArgs> MouseUp
        {
            add
            {
                AddHandler(value);
            }
            remove
            {
                RemoveHandler(value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.Dispose"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Disposes of the resources (other than memory) used by the
        ///    <see cref='System.Windows.Forms.NotifyIcon'/>.
        ///    </para>
        /// </devdoc>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (window != null)
                {
                    this.Icon = null;
                    this.Text = String.Empty;
                    UpdateIcon(false);
                    window.DestroyHandle();
                    window = null;
                    ContextMenu = null;
                    if (iconHandle != IntPtr.Zero)
                    {
                        UnmanagedMethods.DestroyIcon(iconHandle);
                    }
                    iconHandle = IntPtr.Zero;
                }
            }
            //else
            //{
            //    // This same post is done in ControlNativeWindow's finalize method, so if you change
            //    // it, change it there too.
            //    //
            //    if (window != null && window.Handle != IntPtr.Zero)
            //    {
            //        UnmanagedMethods.PostMessage(new HandleRef(window, window.Handle), UnmanagedMethods.WindowsMessage.WM_CLOSE, 0, 0);
            //        window.ReleaseHandle();
            //    }
            //}
            base.Dispose(disposing);
        }

        private void OnBalloonTipClicked()
        {
            this.RaiseEvent(EventArgs.Empty, nameof(BalloonTipClicked));
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnBalloonTipClosed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This method raised the BalloonTipClosed event. 
        ///    </para>
        /// </devdoc>
        private void OnBalloonTipClosed()
        {
            this.RaiseEvent(EventArgs.Empty, nameof(BalloonTipClosed));
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnBalloonTipShown"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This method raised the BalloonTipShown event. 
        ///    </para>
        /// </devdoc>
        private void OnBalloonTipShown()
        {
            this.RaiseEvent(EventArgs.Empty, nameof(BalloonTipShown));
        }


        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnClick"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This method actually raises the Click event. Inheriting classes should
        ///       override this if they wish to be notified of a Click event. (This is far
        ///       preferable to actually adding an event handler.) They should not,
        ///       however, forget to call base.onClick(e); before exiting, to ensure that
        ///       other recipients do actually get the event.
        ///    </para>
        /// </devdoc>
        private void OnClick(EventArgs e)
        {
            RaiseEvent(e, nameof(Click));
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnDoubleClick"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onDoubleClick to send this event to any registered event listeners.
        /// </devdoc>
        private void OnDoubleClick(EventArgs e)
        {
            RaiseEvent(e, nameof(DoubleClick));
        }


        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnMouseClick"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.OnMouseClick to send this event to any registered event listeners.
        /// </devdoc>
        private void OnMouseClick(NotifyIconMouseEventArgs mea)
        {
            RaiseEvent(mea, nameof(MouseClick));
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnMouseDoubleClick"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.OnMouseDoubleClick to send this event to any registered event listeners.
        /// </devdoc>
        private void OnMouseDoubleClick(NotifyIconMouseEventArgs mea)
        {
            RaiseEvent(mea, nameof(MouseDoubleClick));
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnMouseDown"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.NotifyIcon.MouseDown'/> event.
        ///       Inheriting classes should override this method to handle this event.
        ///       Call base.onMouseDown to send this event to any registered event listeners.
        ///       
        ///    </para>
        /// </devdoc>
        private void OnMouseDown(NotifyIconMouseEventArgs e)
        {
            RaiseEvent(new Input.NotifyIconMouseEventArgs(e.Button), nameof(MouseDown));
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnMouseMove"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Inheriting classes should override this method to handle this event.
        ///       Call base.onMouseMove to send this event to any registered event listeners.
        ///       
        ///    </para>
        /// </devdoc>
        private void OnMouseMove(NotifyIconMouseEventArgs e)
        {
            RaiseEvent(e, nameof(MouseMove));
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnMouseUp"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Inheriting classes should override this method to handle this event.
        ///       Call base.onMouseUp to send this event to any registered event listeners.
        ///    </para>
        /// </devdoc>
        private void OnMouseUp(NotifyIconMouseEventArgs e)
        {
            RaiseEvent(new Input.NotifyIconMouseEventArgs(e.Button), nameof(MouseUp));
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.ShowBalloonTip"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a balloon tooltip in the taskbar.
        /// 
        ///       The system enforces minimum and maximum timeout values. Timeout 
        ///       values that are too large are set to the maximum value and values 
        ///       that are too small default to the minimum value. The operating system's 
        ///       default minimum and maximum timeout values are 10 seconds and 30 seconds, 
        ///       respectively.
        ///       
        ///       No more than one balloon ToolTip at at time is displayed for the taskbar. 
        ///       If an application attempts to display a ToolTip when one is already being displayed, 
        ///       the ToolTip will not appear until the existing balloon ToolTip has been visible for at 
        ///       least the system minimum timeout value. For example, a balloon ToolTip with timeout 
        ///       set to 30 seconds has been visible for seven seconds when another application attempts 
        ///       to display a balloon ToolTip. If the system minimum timeout is ten seconds, the first 
        ///       ToolTip displays for an additional three seconds before being replaced by the second ToolTip.
        ///    </para>
        /// </devdoc>
        public void ShowBalloonTip(int timeout)
        {
            ShowBalloonTip(timeout, this.BalloonTipTitle, this.BalloonTipText, this.BalloonTipIcon);
        }


        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.ShowBalloonTip"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a balloon tooltip in the taskbar with the specified title,
        ///       text, and icon for a duration of the specified timeout value.
        /// 
        ///       The system enforces minimum and maximum timeout values. Timeout 
        ///       values that are too large are set to the maximum value and values 
        ///       that are too small default to the minimum value. The operating system's 
        ///       default minimum and maximum timeout values are 10 seconds and 30 seconds, 
        ///       respectively.
        ///       
        ///       No more than one balloon ToolTip at at time is displayed for the taskbar. 
        ///       If an application attempts to display a ToolTip when one is already being displayed, 
        ///       the ToolTip will not appear until the existing balloon ToolTip has been visible for at 
        ///       least the system minimum timeout value. For example, a balloon ToolTip with timeout 
        ///       set to 30 seconds has been visible for seven seconds when another application attempts 
        ///       to display a balloon ToolTip. If the system minimum timeout is ten seconds, the first 
        ///       ToolTip displays for an additional three seconds before being replaced by the second ToolTip.
        ///    </para>
        /// </devdoc>
        public void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon)
        {

            if (timeout < 0)
            {
                throw new ArgumentOutOfRangeException("timeout", "ShowBalloonTip超时" + timeout);
            }

            if (string.IsNullOrEmpty(tipText))
            {
                throw new ArgumentException("tipText不能为空");
            }

            //valid values are 0x0 to 0x3
            if (!IsEnumValid(tipIcon, (int)tipIcon, (int)ToolTipIcon.None, (int)ToolTipIcon.Error))
            {
                throw new InvalidEnumArgumentException("tipIcon", (int)tipIcon, typeof(ToolTipIcon));
            }


            if (added)
            {
                // Bail if in design mode...
                //if (DesignMode)
                //{
                //    return;
                //}
                //IntSecurity.UnrestrictedWindows.Demand();

                UnmanagedMethods.NOTIFYICONDATA data = new UnmanagedMethods.NOTIFYICONDATA();
                if (window.Handle == IntPtr.Zero)
                {
                    window.CreateHandle(new CreateParams());
                }
                data.hWnd = window.Handle;
                data.uID = id;
                data.uFlags = UnmanagedMethods.NIF_INFO;
                data.uTimeoutOrVersion = timeout;
                data.szInfoTitle = tipTitle;
                data.szInfo = tipText;
                switch (tipIcon)
                {
                    case ToolTipIcon.Info: data.dwInfoFlags = UnmanagedMethods.NIIF_INFO; break;
                    case ToolTipIcon.Warning: data.dwInfoFlags = UnmanagedMethods.NIIF_WARNING; break;
                    case ToolTipIcon.Error: data.dwInfoFlags = UnmanagedMethods.NIIF_ERROR; break;
                    case ToolTipIcon.None: data.dwInfoFlags = UnmanagedMethods.NIIF_NONE; break;
                }
                UnmanagedMethods.Shell_NotifyIcon(UnmanagedMethods.NIM_MODIFY, data);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.ShowContextMenu"]/*' />
        /// <devdoc>
        ///     Shows the context menu for the tray icon.
        /// </devdoc>
        /// <internalonly/>
        private void ShowContextMenu()
        {
            UnmanagedMethods.SetForegroundWindow(window.Handle);
            var contextMenu = ContextMenu;
            if (contextMenu != null)
            {
                //UnmanagedMethods.POINT pt;
                //UnmanagedMethods.GetCursorPos(out pt);

                // VS7 #38994
                // The solution to this problem was found in MSDN Article ID: Q135788.
                // Summary: the current window must be made the foreground window
                // before calling TrackPopupMenuEx, and a task switch must be
                // forced after the call.

                if (contextMenu != null)
                {
                    contextMenu.Placement = PlacementMode.Mouse;
                    contextMenu.PopupMarginBottm = 1;
                    contextMenu.PopupMarginTop = "auto";
                    contextMenu.IsOpen = true;
                    //contextMenu.OnPopup(EventArgs.Empty);

                    //SafeNativeMethods.TrackPopupMenuEx(new HandleRef(contextMenu, contextMenu.Handle),
                    //                         UnmanagedMethods.TPM_VERTICAL | UnmanagedMethods.TPM_RIGHTALIGN,
                    //                         pt.x,
                    //                         pt.y,
                    //                         new HandleRef(window, window.Handle),
                    //                         null);

                    //// Force task switch (see above)
                    //UnmanagedMethods.PostMessage(new HandleRef(window, window.Handle), UnmanagedMethods.WindowsMessage.WM_NULL, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.UpdateIcon"]/*' />
        /// <devdoc>
        ///     Updates the icon in the system tray.
        /// </devdoc>
        /// <internalonly/>
        private void UpdateIcon(bool showIconInTray)
        {
            lock (syncObj)
            {

                // Bail if in design mode...
                //
                //if (DesignMode)
                //{
                //    return;
                //}

                //IntSecurity.UnrestrictedWindows.Demand();

                window.LockReference(showIconInTray);

                UnmanagedMethods.NOTIFYICONDATA data = new UnmanagedMethods.NOTIFYICONDATA();
                data.uCallbackMessage = WM_TRAYMOUSEMESSAGE;
                data.uFlags = UnmanagedMethods.NIF_MESSAGE;
                if (showIconInTray)
                {
                    if (window.Handle == IntPtr.Zero)
                    {
                        window.CreateHandle(new CreateParams());
                    }
                }
                data.hWnd = window.Handle;
                data.uID = id;
                data.hIcon = IntPtr.Zero;
                data.szTip = null;
                if (iconHandle != IntPtr.Zero)
                {
                    UnmanagedMethods.DestroyIcon(iconHandle);
                }
                iconHandle = IntPtr.Zero;
                if (Icon != null)
                {
                    data.uFlags |= UnmanagedMethods.NIF_ICON;
                    var stream = Icon.SaveToStream(ImageFormat.Png);
                    var states = UnmanagedMethods.GdipCreateBitmapFromStream(new GPStream(stream), out IntPtr bitmap);
                    stream.Dispose();
                    UnmanagedMethods.GdipCreateHICONFromBitmap(bitmap, out IntPtr hIcon);
                    UnmanagedMethods.GdipDisposeImage(bitmap);
                    iconHandle = hIcon;
                    data.hIcon = iconHandle;
                }
                data.uFlags |= UnmanagedMethods.NIF_TIP;
                data.szTip = text;

                if (showIconInTray && Icon != null)
                {
                    if (!added)
                    {
                        UnmanagedMethods.Shell_NotifyIcon(UnmanagedMethods.NIM_ADD, data);
                        added = true;
                    }
                    else
                    {
                        UnmanagedMethods.Shell_NotifyIcon(UnmanagedMethods.NIM_MODIFY, data);
                    }
                }
                else if (added)
                {
                    UnmanagedMethods.Shell_NotifyIcon(UnmanagedMethods.NIM_DELETE, data);
                    added = false;
                }
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.WmMouseDown"]/*' />
        /// <devdoc>
        ///     Handles the mouse-down event
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseDown(ref Message m, MouseButton button, int clicks)
        {
            if (clicks == 2)
            {
                OnDoubleClick(new NotifyIconMouseEventArgs(button, 2));
                OnMouseDoubleClick(new NotifyIconMouseEventArgs(button, 2));
                doubleClick = true;
            }
            OnMouseDown(new NotifyIconMouseEventArgs(button, clicks));
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.WmMouseMove"]/*' />
        /// <devdoc>
        ///     Handles the mouse-move event
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseMove(ref Message m)
        {
            OnMouseMove(new NotifyIconMouseEventArgs(MouseButton.None, 0));
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.WmMouseUp"]/*' />
        /// <devdoc>
        ///     Handles the mouse-up event
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseUp(ref Message m, MouseButton button)
        {
            OnMouseUp(new NotifyIconMouseEventArgs(button, 0));
            //subhag
            if (!doubleClick)
            {
                OnClick(new NotifyIconMouseEventArgs(button, 0));
                OnMouseClick(new NotifyIconMouseEventArgs(button, 0));
            }
            doubleClick = false;
        }

        private void WmTaskbarCreated(ref Message m)
        {
            added = false;
            UpdateIcon(visible);
        }

        private void WndProc(ref Message msg)
        {

            switch ((UnmanagedMethods.WindowsMessage)msg.Msg)
            {
                case (UnmanagedMethods.WindowsMessage)WM_TRAYMOUSEMESSAGE:
                    switch ((UnmanagedMethods.WindowsMessage)msg.LParam)
                    {
                        case UnmanagedMethods.WindowsMessage.WM_LBUTTONDBLCLK:
                            WmMouseDown(ref msg, MouseButton.Left, 2);
                            break;
                        case UnmanagedMethods.WindowsMessage.WM_LBUTTONDOWN:
                            WmMouseDown(ref msg, MouseButton.Left, 1);
                            break;
                        case UnmanagedMethods.WindowsMessage.WM_LBUTTONUP:
                            WmMouseUp(ref msg, MouseButton.Left);
                            break;
                        case UnmanagedMethods.WindowsMessage.WM_MBUTTONDBLCLK:
                            WmMouseDown(ref msg, MouseButton.Middle, 2);
                            break;
                        case UnmanagedMethods.WindowsMessage.WM_MBUTTONDOWN:
                            WmMouseDown(ref msg, MouseButton.Middle, 1);
                            break;
                        case UnmanagedMethods.WindowsMessage.WM_MBUTTONUP:
                            WmMouseUp(ref msg, MouseButton.Middle);
                            break;
                        case UnmanagedMethods.WindowsMessage.WM_MOUSEMOVE:
                            WmMouseMove(ref msg);
                            break;
                        case UnmanagedMethods.WindowsMessage.WM_RBUTTONDBLCLK:
                            WmMouseDown(ref msg, MouseButton.Right, 2);
                            break;
                        case UnmanagedMethods.WindowsMessage.WM_RBUTTONDOWN:
                            WmMouseDown(ref msg, MouseButton.Right, 1);
                            break;
                        case UnmanagedMethods.WindowsMessage.WM_RBUTTONUP:
                            //if (ContextMenu != null)
                            {
                                ShowContextMenu();
                            }
                            WmMouseUp(ref msg, MouseButton.Right);
                            break;
                        case (UnmanagedMethods.WindowsMessage)UnmanagedMethods.NIN_BALLOONSHOW:
                            OnBalloonTipShown();
                            break;
                        case (UnmanagedMethods.WindowsMessage)UnmanagedMethods.NIN_BALLOONHIDE:
                            OnBalloonTipClosed();
                            break;
                        case (UnmanagedMethods.WindowsMessage)UnmanagedMethods.NIN_BALLOONTIMEOUT:
                            OnBalloonTipClosed();
                            break;
                        case (UnmanagedMethods.WindowsMessage)UnmanagedMethods.NIN_BALLOONUSERCLICK:
                            OnBalloonTipClicked();
                            break;
                    }
                    break;
                case UnmanagedMethods.WindowsMessage.WM_COMMAND:
                    if (IntPtr.Zero == msg.LParam)
                    {
                        //if (Command.DispatchID((int)msg.WParam & 0xFFFF)) return;
                    }
                    else
                    {
                        window.DefWndProc(ref msg);
                    }
                    break;
                case UnmanagedMethods.WindowsMessage.WM_DRAWITEM:
                    // If the wparam is zero, then the message was sent by a menu.
                    // See WM_DRAWITEM in MSDN.
                    if (msg.WParam == IntPtr.Zero)
                    {
                        //WmDrawItemMenuItem(ref msg);
                    }
                    break;
                case UnmanagedMethods.WindowsMessage.WM_MEASUREITEM:
                    // If the wparam is zero, then the message was sent by a menu.
                    if (msg.WParam == IntPtr.Zero)
                    {
                        //WmMeasureMenuItem(ref msg);
                    }
                    break;

                case UnmanagedMethods.WindowsMessage.WM_INITMENUPOPUP:
                    WmInitMenuPopup(ref msg);
                    break;

                case UnmanagedMethods.WindowsMessage.WM_DESTROY:
                    // Remove the icon from the taskbar
                    UpdateIcon(false);
                    break;

                default:
                    if (msg.Msg == WM_TASKBARCREATED)
                    {
                        WmTaskbarCreated(ref msg);
                    }
                    window.DefWndProc(ref msg);
                    break;
            }
        }

        private void WmInitMenuPopup(ref Message m)
        {
            //if (contextMenu != null)
            //{
            //    if (contextMenu.ProcessInitMenuPopup(m.WParam))
            //    {
            //        return;
            //    }
            //}

            window.DefWndProc(ref m);
        }

        //private void WmMeasureMenuItem(ref Message m)
        //{
        //    // Obtain the menu item object
        //    UnmanagedMethods.MEASUREITEMSTRUCT mis = (UnmanagedMethods.MEASUREITEMSTRUCT)m.GetLParam(typeof(UnmanagedMethods.MEASUREITEMSTRUCT));

        //    Debug.Assert(m.LParam != IntPtr.Zero, "m.lparam is null");

        //    // A pointer to the correct MenuItem is stored in the measure item
        //    // information sent with the message.
        //    // (See MenuItem.CreateMenuItemInfo)
        //    MenuItem menuItem = MenuItem.GetMenuItemFromItemData(mis.itemData);
        //    Debug.Assert(menuItem != null, "UniqueID is not associated with a menu item");

        //    // Delegate this message to the menu item
        //    if (menuItem != null)
        //    {
        //        menuItem.WmMeasureItem(ref m);
        //    }
        //}

        //private void WmDrawItemMenuItem(ref Message m)
        //{
        //    // Obtain the menu item object
        //    UnmanagedMethods.DRAWITEMSTRUCT dis = (UnmanagedMethods.DRAWITEMSTRUCT)m.GetLParam(typeof(UnmanagedMethods.DRAWITEMSTRUCT));

        //    // A pointer to the correct MenuItem is stored in the draw item
        //    // information sent with the message.
        //    // (See MenuItem.CreateMenuItemInfo)
        //    MenuItem menuItem = MenuItem.GetMenuItemFromItemData(dis.itemData);

        //    // Delegate this message to the menu item
        //    if (menuItem != null)
        //    {
        //        menuItem.WmDrawItem(ref m);
        //    }
        //}

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.NotifyIconNativeWindow"]/*' />
        /// <devdoc>
        ///     Defines a placeholder window that the NotifyIcon is attached to.
        /// </devdoc>
        /// <internalonly/>
        private class NotifyIconNativeWindow : NativeWindow
        {
            internal NotifyIcon reference;
            private GCHandle rootRef;   // We will root the control when we do not want to be elligible for garbage collection.

            /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.NotifyIconNativeWindow.NotifyIconNativeWindow"]/*' />
            /// <devdoc>
            ///     Create a new NotifyIcon, and bind the window to the NotifyIcon component.
            /// </devdoc>
            /// <internalonly/>
            internal NotifyIconNativeWindow(NotifyIcon component)
            {
                reference = component;
            }

            ~NotifyIconNativeWindow()
            {
                // This same post is done in Control's Dispose method, so if you change
                // it, change it there too.
                //
                if (Handle != IntPtr.Zero)
                {
                    UnmanagedMethods.PostMessage(Handle, (uint)UnmanagedMethods.WindowsMessage.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }

                // This releases the handle from our window proc, re-routing it back to
                // the system.
            }

            public void LockReference(bool locked)
            {
                if (locked)
                {
                    if (!rootRef.IsAllocated)
                    {
                        rootRef = GCHandle.Alloc(reference, GCHandleType.Normal);
                    }
                }
                else
                {
                    if (rootRef.IsAllocated)
                    {
                        rootRef.Free();
                    }
                }
            }

            /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.NotifyIconNativeWindow.WndProc"]/*' />
            /// <devdoc>
            ///     Pass messages on to the NotifyIcon object's wndproc handler.
            /// </devdoc>
            /// <internalonly/>
            protected override void WndProc(ref Message m)
            {
                Debug.Assert(reference != null, "NotifyIcon was garbage collected while it was still visible.  How did we let that happen?");
                reference.WndProc(ref m);
            }
        }
    }

    /// <include file='doc\ToolTipIcon.uex' path='docs/doc[@for="ToolTipIcon"]/*' />
    public enum ToolTipIcon : byte
    {

        /// <include file='doc\ToolTipIcon.uex' path='docs/doc[@for="ToolTipIcon.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No Icon.
        ///    </para>
        /// </devdoc>
        None = 0,

        /// <include file='doc\ToolTipIcon.uex' path='docs/doc[@for="ToolTipIcon.InfoIcon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A Information Icon.
        ///    </para>
        /// </devdoc>
        Info = 1,

        /// <include file='doc\ToolTipIcon.uex' path='docs/doc[@for="ToolTipIcon.WarningIcon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A Warning Icon.
        ///    </para>
        /// </devdoc>
        Warning = 2,


        /// <include file='doc\ToolTipIcon.uex' path='docs/doc[@for="ToolTipIcon.ErrorIcon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A Error Icon.
        ///    </para>
        /// </devdoc>
        Error = 3

    }

    public class NotifyIconMouseEventArgs : EventArgs
    {
        public NotifyIconMouseEventArgs(MouseButton button, int clicks)
        {
            Button = button;
            Clicks = clicks;
        }
        public MouseButton Button { get; set; }
        public int Clicks { get; set; }
    }
}
