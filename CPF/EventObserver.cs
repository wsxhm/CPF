using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    /// <summary>
    /// 描述一个事件用于观察者模式
    /// </summary>
    /// <typeparam name="TArgs"></typeparam>
    /// <typeparam name="TSender"></typeparam>
    public class EventObserver<TArgs, TSender>
    {
        /// <summary>
        /// 描述一个事件用于观察者模式
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="args"></param>
        /// <param name="sender"></param>
        public EventObserver(string eventName, TArgs args, TSender sender)
        {
            EventName = eventName;
            EventArgs = args;
            Sender = sender;
        }

        /// <summary>
        /// 事件名
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        /// 事件数据
        /// </summary>
        public TArgs EventArgs { get; private set; }
        /// <summary>
        /// 事件发起者
        /// </summary>
        public TSender Sender { get; set; }
    }

    class EO<TArgs, TSender> : IObserver<EventObserver<EventArgs, CpfObject>>, IDisposable, IObservable<EventObserver<TArgs, TSender>> where TArgs : EventArgs where TSender : CpfObject
    {
        public CpfObject Observable;
        public string eventName;

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public void OnNext(EventObserver<EventArgs, CpfObject> value)
        {
            if (eventName == value.EventName || (value.EventArgs is CPFPropertyChangedEventArgs eventArgs && eventArgs.PropertyName == eventName))
            {
                EventObserver<TArgs, TSender> eventObserver = new EventObserver<TArgs, TSender>(value.EventName, (TArgs)value.EventArgs, (TSender)value.Sender);
                foreach (var item in obs)
                {
                    item.OnNext(eventObserver);
                }
            }
        }
        List<IObserver<EventObserver<TArgs, TSender>>> obs = new List<IObserver<EventObserver<TArgs, TSender>>>();
        List<IDisposable> disposables = new List<IDisposable>();
        public IDisposable Subscribe(IObserver<EventObserver<TArgs, TSender>> observer)
        {
            disposables.Add(Observable.Subscribe(this));
            obs.Add(observer);
            return this;
        }

        public void Dispose()
        {
            foreach (var item in disposables)
            {
                item.Dispose();
            }
        }
    }
}
