using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CPF.Windows
{
    public class WindowsSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object state)
        {
            if (WindowImpl.Window != null)
            {
                WindowImpl.Window.BeginInvoke(d, state);
            }
            else
            {
                Debug.WriteLine("Window未初始化");
            }
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            if (WindowImpl.Window != null)
            {
                WindowImpl.Window.Invoke(d, state);
            }
            else
            {
                d(state);
            }
        }
    }
}
