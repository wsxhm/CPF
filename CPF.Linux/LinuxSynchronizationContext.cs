using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using CPF.Controls;
using CPF.Platform;

namespace CPF.Linux
{
    class LinuxSynchronizationContext : SynchronizationContext
    {
        public static ConcurrentQueue<SendOrPostData> invokeQueue = new ConcurrentQueue<SendOrPostData>();
        public static ConcurrentQueue<SendOrPostData> asyncQueue = new ConcurrentQueue<SendOrPostData>();
        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="d"></param>
        /// <param name="state"></param>
        public override void Post(SendOrPostCallback d, object state)
        {
            //Console.WriteLine("发送BeginInvoke");
            //lock (X11Window.XlibLock)
            {
                asyncQueue.Enqueue(new SendOrPostData { Data = state, SendOrPostCallback = d });
                //SendMessage(X11Atoms.BeginInvoke, (IntPtr)1);
            }
            LinuxPlatform.Platform.SetFlag();
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            //Console.WriteLine("发送Invoke");
            if (Threading.Dispatcher.MainThread.CheckAccess())
            {
                d(state);
            }
            else
            {
                var invokeMre = new ManualResetEvent(false);
                //lock (X11Window.XlibLock)
                {
                    invokeQueue.Enqueue(new SendOrPostData { Data = state, SendOrPostCallback = d, ManualResetEvent = invokeMre });
                    //SendMessage(X11Atoms.Invoke, (IntPtr)2);
                }
                LinuxPlatform.Platform.SetFlag();
                invokeMre.WaitOne();
            }

        }


        void SendMessage(IntPtr message_type, IntPtr l1)
        {
            var xev = new XEvent
            {
                ClientMessageEvent =
                {
                    type = XEventName.ClientMessage,
                    send_event = true,
                    window = X11Window.main.Handle,
                    message_type = message_type,
                    format = 32,
                    ptr1 = l1,
                }
            };
            XLib.XSendEvent(LinuxPlatform.Platform.Display, X11Window.main.Handle, true,
                 new IntPtr((int)(EventMask.StructureNotifyMask)), ref xev);

            XLib.XFlush(LinuxPlatform.Platform.Display);
        }
    }

    class SendOrPostData
    {
        public SendOrPostCallback SendOrPostCallback;

        public object Data;

        public ManualResetEvent ManualResetEvent;
    }
}
