using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Raindrops.SharedTests")]
namespace Raindrops.Shared.InvokeST
{
    public static class AccessorHelper
    {
        internal static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Delegate>> s_map;
        internal static readonly BindingFlags s_flag;
        static AccessorHelper()
        {
            s_map = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Delegate>>();
            s_flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        }
        public static string CreateKey(string prefix, string member, Type retType, int parameterCount)
        {
            return $"{prefix}.{member}.{retType.FullName}.{parameterCount}";
        }
        public static void ClearCache(Assembly assembly)
        {
            foreach (Type key in s_map.Keys.ToArray())
            {
                if (key.Assembly == assembly)
                {
                    s_map.TryRemove(key, out _);
                }
            }
        }
        public static GetHandler<T> GetGetHandler<T>(ConcurrentDictionary<string, Delegate> dic, Type type, string member)
        {
            string key = CreateKey("GetHandler", member, typeof(T), 0);
            return dic.GetOrAdd(key, (o) =>
            {
                lock (dic)
                {
                    GetHandler<T> getHandler = null;
                    PropertyInfo propertyInfo = type.GetProperty(member, s_flag);
                    if (propertyInfo == null)
                    {
                        FieldInfo fieldInfo = type.GetField(member, s_flag);
                        if (fieldInfo != null)
                        {
                            getHandler = InvokeHandlerHelper.CreateGetHandler<T>(fieldInfo);
                        }
                    }
                    else
                    {
                        getHandler = InvokeHandlerHelper.CreateGetHandler<T>(propertyInfo);
                    }
                    return getHandler;
                }
            }) as GetHandler<T>;
        }
        public static SetHandler<T> GetSetHandler<T>(ConcurrentDictionary<string, Delegate> dic, Type type, string member)
        {
            string key = CreateKey("SetHandler", member, typeof(T), 1);
            return dic.GetOrAdd(key, (o) =>
            {
                lock (dic)
                {
                    SetHandler<T> setHandler = null;
                    PropertyInfo propertyInfo = type.GetProperty(member, s_flag);
                    if (propertyInfo == null)
                    {
                        FieldInfo fieldInfo = type.GetField(member, s_flag);
                        if (fieldInfo != null)
                        {
                            setHandler = InvokeHandlerHelper.CreateSetHandler<T>(fieldInfo);
                        }
                    }
                    else
                    {
                        setHandler = InvokeHandlerHelper.CreateSetHandler<T>(propertyInfo);
                    }
                    return setHandler;
                }
            }) as SetHandler<T>;
        }
        public static bool TryGetValue<T>(this object target, Type type, string member, out T value)
        {
            if (type != null)
            {
                ConcurrentDictionary<string, Delegate> dic = s_map.GetOrAdd(type, (k) => new ConcurrentDictionary<string, Delegate>());
                GetHandler<T> get = GetGetHandler<T>(dic, type, member);
                if (get != null)
                {
                    value = get(target);
                    return true;
                }
            }
            value = default;
            return false;
        }
        public static bool TryGetValue<T>(this object target, string member, out T value)
        {
            if (target != null)
            {
                return TryGetValue(target, target.GetType(), member, out value);
            }
            value = default;
            return false;
        }
        public static bool TrySetValue<T>(this object target, Type type, string member, T value)
        {
            if (type != null)
            {
                ConcurrentDictionary<string, Delegate> dic = s_map.GetOrAdd(type, (k) => new ConcurrentDictionary<string, Delegate>());
                SetHandler<T> get = GetSetHandler<T>(dic, type, member);
                if (get != null)
                {
                    get(target, value);
                    return true;
                }
            }
            value = default;
            return false;
        }
        public static bool TrySetValue<T>(this object target, string member, T value)
        {
            if (target != null)
            {
                return TrySetValue(target, target.GetType(), member, value);
            }
            value = default;
            return false;
        }
        public static T GetValue<T>(this object target, Type type, string member)
        {
            return TryGetValue(target, type, member, out T value) ? value : throw new ArgumentException(nameof(member));
        }
        public static T GetValue<T>(this object target, string member)
        {
            return GetValue<T>(target, target?.GetType(), member);
        }
        public static void SetValue<T>(this object target, Type type, string member, T value)
        {
            if (!TrySetValue(target, type, member, value)) throw new ArgumentException(nameof(member));
        }
        public static void SetValue<T>(this object target, string member, T value)
        {
            SetValue(target, target?.GetType(), member, value);
        }
    }
}
