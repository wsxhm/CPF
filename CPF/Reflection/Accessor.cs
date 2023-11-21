using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CPF.Reflection
{

    public class Accessor
    {
        internal ConcurrentDictionary<string, Delegate> _dic;
        public Accessor(object target) : this(target, target?.GetType() ?? throw new ArgumentNullException(nameof(target)))
        {

        }
        public Accessor(object target, Type elementType)
        {
            Target = target;
            ElementType = elementType;
            _dic = AccessorHelper.s_map.GetOrAdd(elementType, (type) => new ConcurrentDictionary<string, Delegate>());
        }
        public object Target { get; }
        public Type ElementType { get; }
        public object this[string member]
        {
            get
            {
                GetHandler<object> get = AccessorHelper.GetGetHandler<object>(_dic, ElementType, member) ?? throw new ArgumentException(nameof(member));
                return get(Target);
            }
            set
            {
                SetHandler<object> set = AccessorHelper.GetSetHandler<object>(_dic, ElementType, member) ?? throw new ArgumentException(nameof(member));
                set(Target, value);
            }
        }
        public T GetValue<T>(string member)
        {
            return TryGetValue(member, out T value) ? value : throw new ArgumentException(nameof(member));
        }
        public void SetValue<T>(string member, T value)
        {
            if (!TrySetValue(member, value)) throw new ArgumentException(nameof(member));
        }
        public bool TryGetValue<T>(string member, out T value)
        {
            GetHandler<T> get = AccessorHelper.GetGetHandler<T>(_dic, ElementType, member);
            if (get != null)
            {
                value = get(Target);
                return true;
            }
            value = default;
            return false;
        }
        public bool TrySetValue<T>(string member, T value)
        {
            SetHandler<T> set = AccessorHelper.GetSetHandler<T>(_dic, ElementType, member);
            if (set != null)
            {
                set(Target, value);
                return true;
            }
            return false;
        }
    }
}
