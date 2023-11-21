using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CPF.Controls;
using CPF.Input;
using CPF.Platform;
using System.Collections.Concurrent;
using CPF.Drawing;

namespace CPF.Design
{
    public class DesignPlatform : RuntimePlatform
    {
        public RuntimePlatform Platform { get; set; }

        public override PixelPoint MousePosition => new PixelPoint();

        public override TimeSpan DoubleClickTime => throw new NotImplementedException("不能在设计模式下调用");

        public override INativeImpl CreateNative()
        {
            return null;
        }

        public override INotifyIconImpl CreateNotifyIcon()
        {
            return null;
        }

        public override IPopupImpl CreatePopup()
        {
            return new DesignPopupImpl();
        }

        public override IWindowImpl CreateWindow()
        {
            return new DesignWindow();
        }

        public override DragDropEffects DoDragDrop(DragDropEffects allowedEffects, params (DataFormat, object)[] data)
        {
            throw new NotImplementedException("不能在设计模式下调用");
        }

        Screen[] screens = new Screen[] { new Screen(new Rect(0, 0, 1920, 1080), new Rect(0, 0, 1920, 1080), true) };
#if Net4
        public override IList<Screen> GetAllScreen()
        {
            return screens;
        }
#else
        public override IReadOnlyList<Screen> GetAllScreen()
        {
            return screens;
        }
#endif
        public override IClipboard GetClipboard()
        {
            if (Platform != null)
            {
                return Platform.GetClipboard();
            }
            throw new NotImplementedException("不能在设计模式下调用");
        }

        public override object GetCursor(Cursors cursorType)
        {
            return null;
        }

        public override SynchronizationContext GetSynchronizationContext()
        {
            return new DesignSynchronizationContext { virtualPlatform = this };
        }

        public override PlatformHotkey Hotkey(KeyGesture keyGesture)
        {
            throw new NotImplementedException("不能在设计模式下调用");
        }

        internal ConcurrentQueue<SendOrPostData> asyncQueue = new ConcurrentQueue<SendOrPostData>();
        internal ConcurrentQueue<SendOrPostData> invokeQueue = new ConcurrentQueue<SendOrPostData>();
        ManualResetEvent messageInvoke = new ManualResetEvent(false);

        public void Exit()
        {
            IsRun = false;
            messageInvoke.Set();
        }
        bool IsRun = true;

        public override void Run()
        {
            while (IsRun)
            {
                if (invokeQueue.Count == 0 && asyncQueue.Count == 0)
                {
                    messageInvoke.WaitOne();
                    messageInvoke.Reset();
                }
                if (!IsRun)
                {
                    return;
                }
                if (invokeQueue.TryDequeue(out SendOrPostData result))
                {
                    try
                    {
                        result.SendOrPostCallback(result.Data);
                    }
                    catch (Exception e)
                    {
                        result.ManualResetEvent.Set();
                        throw new InvalidOperationException("Invoke操作异常", e);
                    }
                    finally
                    {
                        result.ManualResetEvent.Set();
                    }
                }
                while (asyncQueue.TryDequeue(out SendOrPostData data))
                {
                    data.SendOrPostCallback(data.Data);
                }
            }
        }

        public void SendMessage()
        {
            messageInvoke.Set();
        }

        public override void Run(CancellationToken cancellation)
        {
            throw new NotImplementedException("不能在设计模式下调用");
        }

        public override Task<string[]> ShowFileDialogAsync(FileDialog dialog, IWindowImpl parent)
        {
            throw new NotImplementedException("不能在设计模式下调用");
        }

        public override Task<string> ShowFolderDialogAsync(OpenFolderDialog dialog, IWindowImpl parent)
        {
            throw new NotImplementedException("不能在设计模式下调用");
        }
    }
}
