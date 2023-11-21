using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CPF.Platform;

namespace CPF.Threading
{
    public class Dispatcher : IDispatcher
    {
        internal static int mainId;
        internal static Dispatcher mainThread;
        SynchronizationContext platformDispatcher;

        public static Dispatcher MainThread
        {
            get
            {
                if (mainThread == null)
                {
                    mainThread = new Dispatcher(Application.GetRuntimePlatform().GetSynchronizationContext());
                }
                return mainThread;
            }
        }

        public Dispatcher(SynchronizationContext dispatcher)
        {
            platformDispatcher = dispatcher;
        }

        /// <summary>
        /// 调用线程是否是主线程，否则为false
        /// </summary>
        /// <returns></returns>
        public bool CheckAccess()
        {
            return Thread.CurrentThread.ManagedThreadId == mainId;
        }
        /// <summary>
        /// 验证线程访问权限
        /// </summary>
        public void VerifyAccess()
        {
            if (!CheckAccess() && Application.Main != null)
            {
                throw new Exception("该操作只能在主线程");
            }
        }

        public void Invoke(Action callback)
        {
            platformDispatcher.Send((a) => { callback(); }, null);
        }
        public void Invoke(SendOrPostCallback callback, object data)
        {
            platformDispatcher.Send(callback, data);
        }

        public void BeginInvoke(Action callback)
        {
            platformDispatcher.Post((a) => { callback(); }, null);
        }
        public void BeginInvoke(SendOrPostCallback callback, object data)
        {
            platformDispatcher.Post(callback, data);
        }
    }

    public enum DispatcherPriority
    {
        /// <summary>
        ///     Operations at this priority are processed at normal priority.
        /// </summary>
        Normal,
        /// <summary>
        /// 布局
        /// </summary>
        Layout,
        /// <summary>
        ///     Operations at this priority are processed at the same
        ///     priority as rendering.
        /// </summary>
        Render,

    }
}
