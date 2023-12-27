using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.Threading;
using CPF.Input;
using CPF.Controls;
using System.Threading.Tasks;

namespace CPF.Platform
{
    public abstract class RuntimePlatform
    {
        /// <summary>
        /// 获取调度器
        /// </summary>
        /// <returns></returns>
        public abstract SynchronizationContext GetSynchronizationContext();

        public abstract IWindowImpl CreateWindow();

        //public abstract IViewImpl CreateView();

        public abstract IPopupImpl CreatePopup();

        public abstract INotifyIconImpl CreateNotifyIcon();

#if NET40
        public abstract IList<Screen> GetAllScreen();
#else
        public abstract IReadOnlyList<Screen> GetAllScreen();
#endif

        public abstract INativeImpl CreateNative();

        public abstract object GetCursor(Cursors cursorType);
        //public abstract RuntimePlatformInfo GetInfo();
        /// <summary>
        /// 鼠标位置屏幕像素坐标
        /// </summary>
        public abstract PixelPoint MousePosition { get; }
        /// <summary>
        /// 双击最大间隔时间
        /// </summary>
        public abstract TimeSpan DoubleClickTime { get; }

        public abstract IClipboard GetClipboard();

        public abstract void Run();
        /// <summary>
        /// 一般用于模态窗体
        /// </summary>
        /// <param name="cancellation"></param>
        public abstract void Run(CancellationToken cancellation);
        /// <summary>
        /// 文件对话框
        /// </summary>
        /// <param name="dialog">The details of the file dialog to show.</param>
        /// <param name="parent">The parent window.</param>
        /// <returns>A task returning the selected filenames.</returns>
        public abstract Task<string[]> ShowFileDialogAsync(FileDialog dialog, IWindowImpl parent);
        /// <summary>
        /// 目录对话框
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public abstract Task<string> ShowFolderDialogAsync(OpenFolderDialog dialog, IWindowImpl parent);
        /// <summary>
        /// 确定是否是平台的功能热键
        /// </summary>
        /// <param name="keyGesture"></param>
        /// <returns></returns>
        public abstract PlatformHotkey Hotkey(KeyGesture keyGesture);

        public abstract DragDropEffects DoDragDrop(DragDropEffects allowedEffects, params ValueTuple<DataFormat, object>[] data);
    }
}
