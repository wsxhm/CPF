#if Net4
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CPF
{
    public class WeakReference<T> : IDisposable
    {
        private GCHandle handle;
        private bool trackResurrection;

        public WeakReference(T target) : this(target, false) { }

        public WeakReference(T target, bool trackResurrection)
        {
            this.trackResurrection = trackResurrection;
            this.Target = target;
        }

        ~WeakReference()
        {
            Dispose();
        }

        public void Dispose()
        {
            handle.Free();
            GC.SuppressFinalize(this);
        }

        public virtual bool IsAlive
        {
            get { return (handle.Target != null); }
        }

        public virtual bool TrackResurrection
        {
            get { return this.trackResurrection; }
        }

        public virtual T Target
        {
            get
            {
                if (!handle.IsAllocated)
                {
                    return default(T);
                }
                object o = handle.Target;
                if ((o == null) || (!(o is T)))
                    return default(T);
                else
                    return (T)o;
            }
            set
            {
                handle = GCHandle.Alloc(value, this.trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
            }
        }

        public bool TryGetTarget(out T target)
        {
            // Call the worker method that has more performant but less user friendly signature.
            T o = this.Target;
            target = o;
            return o != null;
        }
    }

}
#endif