using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CPF
{
    /// <summary>
    /// 弱引用事件，可以防止注册之后忘记取消绑定导致内存泄露问题，以及重复注册问题
    /// </summary>
    public class WeakEventHandlerList : IDisposable
    {
        HybridDictionary<object, WeakEventHandler> keyValues;
        /// <summary>
        /// 弱引用事件，可以防止注册之后忘记取消绑定导致内存泄露问题，以及重复注册问题
        /// </summary>
        public WeakEventHandlerList()
        { }

        public WeakEventHandler this[object key]
        {
            get
            {
                if (keyValues != null)
                {
                    keyValues.TryGetValue(key, out WeakEventHandler value);
                    return value;
                }
                return null;
            }
        }

        public void AddHandler(object key, Delegate handler)
        {
            if (keyValues == null)
            {
                keyValues = new HybridDictionary<object, WeakEventHandler>();
            }
            if (!keyValues.TryGetValue(key, out WeakEventHandler value))
            {
                value = new WeakEventHandler();
                keyValues.Add(key, value);
            }
            value.AddHandler(handler);
        }
        public void AddHandler(Delegate handler, [CallerMemberName] string eventName = null)
        {
            AddHandler(eventName, handler);
        }

        public void Dispose()
        {
            keyValues = null;
        }

        public void RemoveHandler(object key, Delegate handler)
        {
            if (keyValues != null)
            {
                if (keyValues.TryGetValue(key, out WeakEventHandler value))
                {
                    value.RemoveHandler(handler);
                }
            }
        }

        public void RemoveHandler(Delegate handler, [CallerMemberName] string eventName = null)
        {
            RemoveHandler(eventName, handler);
        }
    }
}
