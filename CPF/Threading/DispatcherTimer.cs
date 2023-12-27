using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace CPF.Threading
{
    /// <summary>
    /// 主窗体存在的时候才有效
    /// </summary>
    public class DispatcherTimer : IDisposable
    {
        TimeSpan interval;
        public DispatcherTimer()
        {
            interval = TimeSpan.FromMilliseconds(15);//毫秒
        }

        static Thread timeThread;
        static List<DispatcherTimer> timers;
        static List<DispatcherTimer> tempTimers;
        static object _lock = new object();

        public void Stop()
        {
            if (isEnabled)
            {
                lock (_lock)
                {
                    isEnabled = false;
                    timers.Remove(this);
                }
            }
        }

        public void Start()
        {
            if (isEnabled)
            {
                return;
            }
            nextTick = CPF.Platform.Application.Elapsed + Interval;
            if (timeThread == null)
            {
                timers = new List<DispatcherTimer>();
                tempTimers = new List<DispatcherTimer>(); 
                timeThread = new Thread(SetTime) { IsBackground = true, Name = "定时器线程" };
                timeThread.Start();
            }
            lock (_lock)
            {
                timers.Add(this);
                isEnabled = true;
                //cancellationTokenSource.Cancel(true);
            }
        }
        TimeSpan nextTick;
        //static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        //static int delay = 100;
        static ManualResetEvent timeMre = new ManualResetEvent(false);
        /// <summary>
        /// 用于提高定时器精度，主线程有任意消息时候调用
        /// </summary>
        public static void SetTimeTick()
        {
            timeMre.Set();
        }

        static void SetTime()
        {
            while (true)
            {
                timeMre.WaitOne(1);
                timeMre.Reset();
                //Thread.Sleep(1);
                //Thread.SpinWait(10000);
                //#if NET40
                //                var task = TaskEx.Delay(delay, cancellationTokenSource.Token);
                //#else
                //                var task = Task.Delay(delay, cancellationTokenSource.Token);
                //#endif
                //                task.Wait();

                //TimeSpan? nextDelay = null;
                var now = CPF.Platform.Application.Elapsed;
                lock (_lock)
                {
                    foreach (var t in timers)
                    {
                        if (t.isEnabled && t.nextTick <= now)
                        {
                            tempTimers.Add(t);
                            //if (nextDelay == null || t.nextTick < nextDelay.Value)
                            //    nextDelay = t.nextTick;
                        }
                    }
                }
                if (tempTimers.Count > 0)
                {
                    if (CPF.Platform.Application.Main != null)
                    {
                        Dispatcher.MainThread.Invoke(() =>
                        {
                            foreach (var t in tempTimers.ToArray())
                            {
                                if (t.Tick != null)
                                {
                                    t.Tick(t, EventArgs.Empty);
                                }
                                if (t.IsEnabled)
                                {
                                    t.nextTick = CPF.Platform.Application.Elapsed + t.Interval;
                                    //if (nextDelay == null || t.nextTick < nextDelay.Value)
                                    //    nextDelay = t.nextTick;
                                }
                                else
                                {
                                    lock (_lock)
                                    {
                                        timers.Remove(t);
                                    }
                                }
                            }
                        });
                    }
                    tempTimers.Clear();
                }
                //if (nextDelay != null)
                //{
                //    delay = Math.Max(2, (int)(nextDelay.Value - _clock.Elapsed).TotalMilliseconds);
                //}
            }
        }

        public TimeSpan Interval
        {
            get
            {
                return interval;
            }

            set
            {
                interval = value;
            }
        }

        bool isEnabled;
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }

            set
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }
        public object Tag { get; set; }
        public event EventHandler Tick;

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Tick = null;
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    Stop();

                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~DispatcherTimer()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
