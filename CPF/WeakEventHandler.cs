using System;
using System.Collections.Generic;
using System.Text;
using CPF.Reflection;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

namespace CPF
{
    /// <summary>
    /// 弱引用事件
    /// </summary>
    public class WeakEventHandler : IDisposable
    {
        HashSet<WeakDelegate> delegates;
        public void AddHandler(Delegate value)
        {
            if (delegates == null)
            {
                delegates = new HashSet<WeakDelegate>();
            }
            delegates.Add(new WeakDelegate(value.Target, value.Method));
        }

        public void RemoveHandler(Delegate value)
        {
            if (delegates != null)
            {
                delegates.Remove(new WeakDelegate(value.Target, value.Method));
            }
        }

        public void Invoke(object sender, object e)
        {
            if (delegates != null)
            {
                foreach (var item in delegates.ToArray())
                {
                    if (item.reference.TryGetTarget(out var target) || item.method.IsStatic)
                    {
                        item.method.FastInvoke(target, sender, e);
                    }
                    else
                    {
                        delegates.Remove(item);
                    }
                }
            }
        }

        public async Task AsyncInvoke(object sender, object e)
        {
            if (delegates != null)
            {
                foreach (var item in delegates.ToArray())
                {
                    if (item.reference.TryGetTarget(out var target) || item.method.IsStatic)
                    {
                        await item.method.FastAsyncInvoke(target, sender, e);
                    }
                    else
                    {
                        delegates.Remove(item);
                    }
                }
            }
        }

        public void Dispose()
        {
            delegates = null;
        }

    }

    /// <summary>
    /// 弱引用事件里使用的委托
    /// </summary>
    public struct WeakDelegate
    {
        public WeakDelegate(object obj, MethodInfo method)
        {
            reference = new WeakReference<object>(obj);
            this.method = method;
            hash = (obj == null ? 0 : obj.GetHashCode()) ^ method.GetHashCode();
        }
        public readonly WeakReference<object> reference;
        public readonly MethodInfo method;

        public override bool Equals(object obj)
        {
            if (obj is WeakDelegate weak)
            {
                weak.reference.TryGetTarget(out var t1);
                reference.TryGetTarget(out var t2);
                if (t1 == t2 && method == weak.method)
                {
                    return true;
                }
            }
            return false;
        }
        readonly int hash;

        public override int GetHashCode()
        {
            return hash;
        }
    }
}
