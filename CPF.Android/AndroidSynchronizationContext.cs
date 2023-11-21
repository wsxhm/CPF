using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CPF.Android
{
    class AndroidSynchronizationContext : SynchronizationContext
    {
        private Handler _handler;
        public AndroidSynchronizationContext()
        {
            _handler = new Handler(Application.Context.MainLooper);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            _handler.Post(() =>
            {
                d(state);
            });
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            if (CPF.Threading.Dispatcher.MainThread.CheckAccess())
            {
                d(state);
            }
            else
            {
                ManualResetEvent manual = new ManualResetEvent(false);
                _handler.Post(() =>
                {
                    d(state);
                    manual.Set();
                });
                manual.WaitOne();
            }
        }
    }
}