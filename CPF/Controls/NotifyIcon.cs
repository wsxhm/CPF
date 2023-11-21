using CPF.Drawing;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using CPF.Input;

namespace CPF.Controls
{
    /// <summary>
    /// 指定可在通知区域创建图标的组件，部分Linux可能不支持
    /// </summary>
    public class NotifyIcon : CpfObject
    {
        /// <summary>
        /// 指定可在通知区域创建图标的组件，部分Linux可能不支持
        /// </summary>
        public NotifyIcon()
        {
            if (string.IsNullOrEmpty(CPF.Design.DesignerLoadStyleAttribute.ProjectPath) && !Application.DesignMode)
            {
                notifyIcon = Application.GetRuntimePlatform().CreateNotifyIcon();
                if (notifyIcon != null)
                {
                    notifyIcon.MouseDown += NotifyIcon_MouseDown;
                    notifyIcon.MouseUp += NotifyIcon_MouseUp;
                    notifyIcon.Click += NotifyIcon_Click;
                    notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
                }
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.RaiseEvent(e, nameof(DoubleClick));
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            this.RaiseEvent(e, nameof(Click));
        }

        private void NotifyIcon_MouseUp(object sender, Input.NotifyIconMouseEventArgs e)
        {
            var contextMenu = ContextMenu;
            if (ContextMenu != null && e.Button == Input.MouseButton.Right)
            {
                contextMenu.Placement = PlacementMode.Mouse;
                contextMenu.PopupMarginBottm = 1;
                contextMenu.PopupMarginTop = "auto";
                contextMenu.PlacementTarget = null;
                contextMenu.Popup.LoadStyle(Window.Windows.FirstOrDefault(a => a.IsMain));
                contextMenu.IsOpen = true;
            }
            this.RaiseEvent(e, nameof(MouseUp));
        }

        private void NotifyIcon_MouseDown(object sender, EventArgs e)
        {
            this.RaiseEvent(e, nameof(MouseDown));
        }
        /// <summary>
        /// 内部实现
        /// </summary>
        [NotCpfProperty, Browsable(false)]
        public INotifyIconImpl NotifyIconImpl
        {
            get { return notifyIcon; }
        }
        INotifyIconImpl notifyIcon;
        /// <summary>
        /// 鼠标移入显示的文字提示
        /// </summary>
        public string Text
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 显示的图标
        /// </summary>
        public Image Icon
        {
            get { return GetValue<Image>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 右键菜单
        /// </summary>
        public ContextMenu ContextMenu
        {
            get { return GetValue<ContextMenu>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Visible
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        [PropertyChanged(nameof(Text))]
        void OnText(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (notifyIcon != null)
            {
                CPF.Threading.Dispatcher.MainThread.Invoke(() =>
                {
                    notifyIcon.Text = newValue as string;
                });
            }
        }
        [PropertyChanged(nameof(Icon))]
        void OnIcon(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (notifyIcon != null)
            {
                CPF.Threading.Dispatcher.MainThread.Invoke(() =>
                {
                    notifyIcon.Icon = newValue as Image;
                });
            }
        }

        [PropertyChanged(nameof(Visible))]
        void OnVisible(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (notifyIcon != null)
            {
                CPF.Threading.Dispatcher.MainThread.Invoke(() =>
                {
                    notifyIcon.Visible = (bool)newValue;
                });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
            }
            base.Dispose(disposing);
        }

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

        public event EventHandler<NotifyIconMouseEventArgs> MouseDown
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

        public event EventHandler<NotifyIconMouseEventArgs> MouseUp
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

    }
}
