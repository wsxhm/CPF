using CPF.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CPF.Design
{
    class DesignSynchronizationContext : SynchronizationContext
    {
        internal DesignPlatform virtualPlatform;

        public override void Post(SendOrPostCallback d, object state)
        {
            virtualPlatform.asyncQueue.Enqueue(new SendOrPostData { Data = state, SendOrPostCallback = d,message= "beginInvoke" });
            if (virtualPlatform.asyncQueue.Count <= 1)
            {
                virtualPlatform.SendMessage();
            }
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            if (Dispatcher.MainThread.CheckAccess())
            {
                d(state);
            }
            else
            {
                var invokeMre = new ManualResetEvent(false);
                virtualPlatform.invokeQueue.Enqueue(new SendOrPostData { Data = state, SendOrPostCallback = d, ManualResetEvent = invokeMre,message= "invoke"});
                virtualPlatform.SendMessage();
                invokeMre.WaitOne();
            }
        }
    }

    class SendOrPostData
    {
        public SendOrPostCallback SendOrPostCallback;

        public object Data;

        public ManualResetEvent ManualResetEvent;

        public string message;
    }
}
