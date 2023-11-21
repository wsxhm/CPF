using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace CPF.Reflection
{
    public abstract class FastReflectionCache<TKey, TValue>// : IFastReflectionCache<TKey, TValue>
    {
        private Dictionary<TKey, TValue> m_cache = new Dictionary<TKey, TValue>();
        KeyValuePair<TKey, TValue>[] save;
        internal void SetTypeCache()
        {
            save = m_cache.ToArray();
        }

        internal void RecoveryTypeCache()
        {
            if (save != null)
            {
                m_cache.Clear();
                foreach (var item in save)
                {
                    m_cache.Add(item.Key, item.Value);
                }
                save = null;
            }
        }

        public TValue Get(TKey key)
        {
            TValue value;
            if (this.m_cache.TryGetValue(key, out value))
            {
                return value;
            }

            lock (key)
            {
                if (!this.m_cache.TryGetValue(key, out value))
                {
                    value = this.Create(key);
                    this.m_cache[key] = value;
                }
            }

            return value;
        }

        protected abstract TValue Create(TKey key);

        //public static void LoadThis(ILGenerator iLGenerator, Type thisType)
        //{
        //    if (thisType.IsValueType)
        //    {
        //        //生成一个loc.0钉住引用
        //        LocalBuilder fixedloc = iLGenerator.DeclareLocal(typeof(object), true);
        //        iLGenerator.PushArgument(0);
        //        iLGenerator.PopLocal(fixedloc);

        //        //由于传入的是object 则堆栈顶部是指向堆的指针
        //        iLGenerator.PushLocal(fixedloc);
        //        //压入一个IntPtr大小
        //        iLGenerator.PushNumber(IntPtr.Size);

        //        MethodInfo addition = typeof(IntPtr).GetMethod(nameof(IntPtr.Add), BindingFlags.Public | BindingFlags.Static);
        //        //此时堆栈
        //        //IntPtr.Size
        //        //native int to Object
        //        //调用Add方法,让native int 越过一个单位
        //        iLGenerator.Call(addition);
        //        //此时堆栈
        //        //native int to Struct
        //    }
        //    else
        //    {
        //        iLGenerator.PushThis(thisType);
        //    }
        //}
    }
}
