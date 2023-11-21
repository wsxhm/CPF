using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using System.Reflection.Emit;

namespace CPF.Reflection
{
    /// <summary>
    /// 用于实现快速反射调用
    /// </summary>
    public static class FastReflectionExtensions
    {
        /// <summary>
        /// 保存当前的类型缓存。 一般是先调用SetTypeCache，再使用一些可卸载程序集，程序集卸载之后，再调用 RecoveryTypeCache
        /// </summary>
        public static void SetTypeCache()
        {
            methodCache.SetTypeCache();
            propertySetCache.SetTypeCache();
            propertyGetCache.SetTypeCache();
            fieldGetCache.SetTypeCache();
            fieldSetCache.SetTypeCache();
            constructorCache.SetTypeCache();
            saveMethods = methods.ToArray();
            saveSetValue = setValues.ToArray();
            saveGetValue = getValues.ToArray();
        }
        /// <summary>
        /// 恢复之前保存的类型缓存。 一般是先调用SetTypeCache，再使用一些可卸载程序集，程序集卸载之后，再调用 RecoveryTypeCache
        /// </summary>
        public static void RecoveryTypeCache()
        {
            methodCache.RecoveryTypeCache();
            propertySetCache.RecoveryTypeCache();
            propertyGetCache.RecoveryTypeCache();
            fieldGetCache.RecoveryTypeCache();
            fieldSetCache.RecoveryTypeCache();
            constructorCache.RecoveryTypeCache();
            if (saveMethods != null)
            {
                methods.Clear();
                foreach (var item in saveMethods)
                {
                    methods.TryAdd(item.Key, item.Value);
                }
                saveMethods = null;
            }
            if (saveSetValue != null)
            {
                setValues.Clear();
                foreach (var item in saveSetValue)
                {
                    setValues.TryAdd(item.Key, item.Value);
                }
                saveSetValue = null;
            }
            if (saveGetValue != null)
            {
                getValues.Clear();
                foreach (var item in saveGetValue)
                {
                    getValues.TryAdd(item.Key, item.Value);
                }
                saveGetValue = null;
            }
        }
        static ConcurrentDictionary<Type, ConcurrentDictionary<string, InvokeHandler>> methods = new ConcurrentDictionary<Type, ConcurrentDictionary<string, InvokeHandler>>();
        static KeyValuePair<Type, ConcurrentDictionary<string, InvokeHandler>>[] saveMethods;

        static ConcurrentDictionary<Type, ConcurrentDictionary<string, SetHandler<object>>> setValues = new ConcurrentDictionary<Type, ConcurrentDictionary<string, SetHandler<object>>>();
        static KeyValuePair<Type, ConcurrentDictionary<string, SetHandler<object>>>[] saveSetValue;

        static ConcurrentDictionary<Type, ConcurrentDictionary<string, GetHandler<object>>> getValues = new ConcurrentDictionary<Type, ConcurrentDictionary<string, GetHandler<object>>>();
        static KeyValuePair<Type, ConcurrentDictionary<string, GetHandler<object>>>[] saveGetValue;

        /// <summary>
        /// 快速动态调用对象的方法
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object Invoke(this object instance, string methodName, params object[] parameters)
        {
            var type = instance.GetType();

            if (!methods.TryGetValue(type, out var list))
            {
                list = new ConcurrentDictionary<string, InvokeHandler>();
                methods.TryAdd(type, list);
            }
            if (!list.TryGetValue(methodName, out var fun))
            {
                var minfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (minfo == null)
                {
                    throw new Exception(type + "未找到方法：" + methodName);
                }
                fun = MethodCache.CreateInvokeDelegate(minfo);
                list.TryAdd(methodName, fun);
            }
            return fun(instance, parameters);
        }

        /// <summary>
        /// 反射快速调用
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="instance"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object FastInvoke(this MethodInfo methodInfo, object instance, params object[] parameters)
        {
            //return FastReflectionCaches.MethodInvokerCache.Get(methodInfo).Invoke(instance, parameters);
            return methodCache.Get(methodInfo).Invoke(instance, parameters);
        }
        static MethodCache methodCache = new MethodCache();

        /// <summary>
        /// 快速动态设置对象的属性值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetValue(this object instance, string propertyName, object value)
        {
            var type = instance.GetType();

            if (!setValues.TryGetValue(type, out var list))
            {
                list = new ConcurrentDictionary<string, SetHandler<object>>();
                setValues.TryAdd(type, list);
            }
            if (!list.TryGetValue(propertyName, out var fun))
            {
                var minfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (minfo == null)
                {
                    throw new Exception(type + "未找到属性：" + propertyName);
                }
                fun = PropertySetCache.CreateInvokeDelegate(minfo);
                list.TryAdd(propertyName, fun);
            }
            fun(instance, value);
        }

        public static void FastSetValue(this PropertyInfo propertyInfo, object instance, object value)
        {
            //FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).SetValue(instance, value);
            propertySetCache.Get(propertyInfo).Invoke(instance, value);
        }
        static PropertySetCache propertySetCache = new PropertySetCache();
        /// <summary>
        /// 快速动态获取对象的属性值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetValue(this object instance, string propertyName)
        {
            var type = instance.GetType();

            if (!getValues.TryGetValue(type, out var list))
            {
                list = new ConcurrentDictionary<string, GetHandler<object>>();
                getValues.TryAdd(type, list);
            }
            if (!list.TryGetValue(propertyName, out var fun))
            {
                var minfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (minfo == null)
                {
                    if (type.IsValueType)
                    {
                        var f = type.GetField(propertyName);
                        if (f != null)
                        {
                            fun = fieldGetCache.Get(f);
                            //return f.FastGetValue(instance);
                        }
                        else
                        {
                            if (type.Name.StartsWith("ValueTuple`"))
                            {
                                var rest = type.GetField("Rest");
                                if (rest != null)
                                {
                                    var i = int.Parse(propertyName.Substring(4));
                                    var pn = "Item" + (i - 7);
                                    f = rest.FieldType.GetField(pn);
                                    if (f != null)
                                    {
                                        //var fu = fieldGetCache.Get(f);
                                        fun = obj => obj.GetValue("Rest").GetValue(pn);
                                    }
                                }
                            }
                        }
                    }
                    if (fun == null)
                    {
                        throw new Exception(type + "未找到属性：" + propertyName);
                    }
                }
                else
                {
                    fun = PropertyGetCache.CreateInvokeDelegate(minfo);
                }
                list.TryAdd(propertyName, fun);
            }
            return fun(instance);
        }
        public static object FastGetValue(this PropertyInfo propertyInfo, object instance)
        {
            //return FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).GetValue(instance);
            return propertyGetCache.Get(propertyInfo)(instance);
        }
        static PropertyGetCache propertyGetCache = new PropertyGetCache();
        public static object FastGetValue(this FieldInfo fieldInfo, object instance)
        {
            //return FastReflectionCaches.FieldAccessorCache.Get(fieldInfo).GetValue(instance);
            return fieldGetCache.Get(fieldInfo).Invoke(instance);
        }
        static FieldGetCache fieldGetCache = new FieldGetCache();

        static FieldSetCache fieldSetCache = new FieldSetCache();
        public static void FastSetValue(this FieldInfo fieldInfo, object instance, object value)
        {
            fieldSetCache.Get(fieldInfo).Invoke(instance, value);
        }

        public static object FastInvoke(this ConstructorInfo constructorInfo, params object[] parameters)
        {
            //return FastReflectionCaches.ConstructorInvokerCache.Get(constructorInfo).Invoke(parameters);
            return constructorCache.Get(constructorInfo).Invoke(parameters);
        }

        static ConstructorCache constructorCache = new ConstructorCache();
    }

    class ConstructorCache : FastReflectionCache<ConstructorInfo, Func<object[], object>>
    {

        protected override Func<object[], object> Create(ConstructorInfo key)
        {
            return InitializeInvoker(key);
        }

        private Func<object[], object> InitializeInvoker(ConstructorInfo constructorInfo)
        {
            // Target: (object)new T((T0)parameters[0], (T1)parameters[1], ...)

            // parameters to execute
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            var parameterExpressions = new List<Expression>();
            var paramInfos = constructorInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                // (Ti)parameters[i]
                var valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                var valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            // new T((T0)parameters[0], (T1)parameters[1], ...)
            var instanceCreate = Expression.New(constructorInfo, parameterExpressions);

            // (object)new T((T0)parameters[0], (T1)parameters[1], ...)
            var instanceCreateCast = Expression.Convert(instanceCreate, typeof(object));

            var lambda = Expression.Lambda<Func<object[], object>>(instanceCreateCast, parametersParameter);

            return lambda.Compile();
        }
    }

    class FieldGetCache : FastReflectionCache<FieldInfo, GetHandler<object>>
    {
        protected override GetHandler<object> Create(FieldInfo key)
        {
            return InitializeGet(key);
        }

        private GetHandler<object> InitializeGet(FieldInfo fieldInfo)
        {
            // target: (object)(((TInstance)instance).Field)

            // preparing parameter, object type
            var instance = Expression.Parameter(typeof(object), "instance");

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = fieldInfo.IsStatic ? null :
                Expression.Convert(instance, fieldInfo.ReflectedType);

            // ((TInstance)instance).Field
            var fieldAccess = Expression.Field(instanceCast, fieldInfo);

            // (object)(((TInstance)instance).Field)
            var castFieldValue = Expression.Convert(fieldAccess, typeof(object));

            // Lambda expression
            var lambda = Expression.Lambda<GetHandler<object>>(castFieldValue, instance);

            return lambda.Compile();


            //DynamicMethod dm = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object) }, fieldInfo.DeclaringType.Module, true);
            //ILGenerator il = dm.GetILGenerator();
            //if (!fieldInfo.IsStatic)
            //{
            //    il.PushThis(fieldInfo.DeclaringType);
            //}
            //il.PushField(fieldInfo);
            //il.Convert(fieldInfo.FieldType, typeof(object));
            //il.Ret();
            //return (GetHandler<object>)dm.CreateDelegate(typeof(GetHandler<object>));
        }
    }

    class FieldSetCache : FastReflectionCache<FieldInfo, SetHandler<object>>
    {
        protected override SetHandler<object> Create(FieldInfo key)
        {
            return InitializeGet(key);
        }

        private SetHandler<object> InitializeGet(FieldInfo fieldInfo)
        {
            // preparing parameter, object type
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = fieldInfo.IsStatic ? null :
                Expression.Convert(instance, fieldInfo.ReflectedType);

            // ((TInstance)instance).Field
            var fieldAccess = Expression.Field(instanceCast, fieldInfo);

            // (FieldType)value
            var castPropertyValue = Expression.Convert(value, fieldInfo.FieldType);
            //((TInstance)instance).Field=(FieldType)value
            var assign = Expression.Assign(fieldAccess, castPropertyValue);

            // Lambda expression
            var lambda = Expression.Lambda<SetHandler<object>>(assign, instance, value);

            return lambda.Compile();


            //DynamicMethod dm = new DynamicMethod(string.Empty, typeof(void), new Type[] { typeof(object), typeof(object) }, fieldInfo.DeclaringType.Module, true);
            //ILGenerator il = dm.GetILGenerator();
            //if (!fieldInfo.IsStatic)
            //{
            //    LoadThis(il, fieldInfo.DeclaringType);
            //}
            //il.PushArgument(1);
            //il.Convert(typeof(object), fieldInfo.FieldType);
            //il.PopField(fieldInfo);
            //il.Ret();
            //return dm.CreateDelegate(typeof(SetHandler<object>)) as SetHandler<object>;
        }
    }


    class MethodCache : FastReflectionCache<MethodInfo, InvokeHandler>
    {
        protected override InvokeHandler Create(MethodInfo key)
        {
            return CreateInvokeDelegate(key);
        }

        public static InvokeHandler CreateInvokeDelegate(MethodInfo methodInfo)
        {
            // Target: ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)

            // parameters to execute
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            var parameterExpressions = new List<Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                // (Ti)parameters[i]
                BinaryExpression valueObj = Expression.ArrayIndex(
                    parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(
                    valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = methodInfo.IsStatic ? null :
                Expression.Convert(instanceParameter, methodInfo.ReflectedType);

            // static invoke or ((TInstance)instance).Method
            var methodCall = Expression.Call(instanceCast, methodInfo, parameterExpressions);

            // ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)
            if (methodCall.Type == typeof(void))
            {
                var lambda = Expression.Lambda<Action<object, object[]>>(
                        methodCall, instanceParameter, parametersParameter);

                Action<object, object[]> execute = lambda.Compile();
                return (instance, parameters) =>
                {
                    execute(instance, parameters);
                    return null;
                };
            }
            else
            {
                var castMethodCall = Expression.Convert(methodCall, typeof(object));
                var lambda = Expression.Lambda<InvokeHandler>(
                    castMethodCall, instanceParameter, parametersParameter);

                return lambda.Compile();
            }

            //DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module, true);
            //ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
            //ParameterInfo[] parameters = methodInfo.GetParameters();
            //var parametersTypes = new Type[parameters.Length];
            //LocalBuilder[] locclBuilders = new LocalBuilder[parametersTypes.Length];
            //for (int i = 0; i < parametersTypes.Length; i++)
            //{
            //    Type ptype = parameters[i].ParameterType;
            //    if (ptype.IsByRef)
            //        ptype = ptype.GetElementType();
            //    locclBuilders[i] = iLGenerator.DeclareLocal(ptype, true);
            //    parametersTypes[i] = ptype;
            //}
            //for (int i = 0; i < parametersTypes.Length; i++)
            //{
            //    iLGenerator.Emit(OpCodes.Ldarg_1);
            //    iLGenerator.PushNumber(i);
            //    iLGenerator.Emit(OpCodes.Ldelem_Ref);
            //    iLGenerator.UnBox(parametersTypes[i]);
            //    iLGenerator.Emit(OpCodes.Stloc, locclBuilders[i]);
            //}
            //if (!methodInfo.IsStatic)
            //{
            //    LoadThis(iLGenerator, methodInfo.DeclaringType);
            //}

            //for (int i = 0; i < parametersTypes.Length; i++)
            //{
            //    if (parameters[i].ParameterType.IsByRef)
            //        iLGenerator.Emit(OpCodes.Ldloca_S, locclBuilders[i]);
            //    else
            //        iLGenerator.Emit(OpCodes.Ldloc, locclBuilders[i]);
            //}
            //iLGenerator.Call(methodInfo);

            //if (methodInfo.ReturnType == typeof(void))
            //    iLGenerator.Emit(OpCodes.Ldnull);
            //else
            //    iLGenerator.BoxIfNeeded(methodInfo.ReturnType);

            //for (int i = 0; i < parametersTypes.Length; i++)
            //{
            //    if (parameters[i].ParameterType.IsByRef)
            //    {
            //        iLGenerator.Emit(OpCodes.Ldarg_1);
            //        iLGenerator.PushNumber(i);
            //        iLGenerator.Emit(OpCodes.Ldloc, locclBuilders[i]);
            //        iLGenerator.BoxIfNeeded(locclBuilders[i]);
            //        iLGenerator.Emit(OpCodes.Stelem_Ref);
            //    }
            //}
            //iLGenerator.Ret();
            //return (InvokeHandler)dynamicMethod.CreateDelegate(typeof(InvokeHandler));
        }
    }

    class PropertyGetCache : FastReflectionCache<PropertyInfo, GetHandler<object>>
    {
        protected override GetHandler<object> Create(PropertyInfo key)
        {
            return CreateInvokeDelegate(key);
        }

        public static GetHandler<object> CreateInvokeDelegate(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead) return null;

            // Target: (object)(((TInstance)instance).Property)

            // preparing parameter, object type
            var instance = Expression.Parameter(typeof(object), "instance");

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = propertyInfo.GetGetMethod(true).IsStatic ? null :
                Expression.Convert(instance, propertyInfo.ReflectedType);

            // ((TInstance)instance).Property
            var propertyAccess = Expression.Property(instanceCast, propertyInfo);

            // (object)(((TInstance)instance).Property)
            var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));

            // Lambda expression
            var lambda = Expression.Lambda<GetHandler<object>>(castPropertyValue, instance);

            return lambda.Compile();

            //if (!propertyInfo.CanRead)
            //    return null;
            //DynamicMethod dm = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object) }, propertyInfo.DeclaringType.Module, true);
            //ILGenerator il = dm.GetILGenerator();
            //MethodInfo mt = propertyInfo.GetGetMethod(true);
            //if (!mt.IsStatic)
            //{
            //    il.PushThis(mt.DeclaringType);
            //}
            //il.Call(mt);
            //il.Convert(mt.ReturnType, typeof(object));
            //il.Ret();
            //return (GetHandler<object>)dm.CreateDelegate(typeof(GetHandler<object>));
        }
    }

    class PropertySetCache : FastReflectionCache<PropertyInfo, SetHandler<object>>
    {
        protected override SetHandler<object> Create(PropertyInfo propertyInfo)
        {
            return CreateInvokeDelegate(propertyInfo);
        }

        public static SetHandler<object> CreateInvokeDelegate(PropertyInfo propertyInfo)
        {
            // preparing parameter, object type
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = propertyInfo.GetSetMethod(true).IsStatic ? null :
                Expression.Convert(instance, propertyInfo.ReflectedType);

            // ((TInstance)instance).Property
            var propertyAccess = Expression.Property(instanceCast, propertyInfo);

            // (PropertyType)value
            var castPropertyValue = Expression.Convert(value, propertyInfo.PropertyType);
            //((TInstance)instance).Property=(PropertyType)value
            var assign = Expression.Assign(propertyAccess, castPropertyValue);
            // Lambda expression
            var lambda = Expression.Lambda<SetHandler<object>>(assign, instance, value);

            return lambda.Compile();

            //DynamicMethod dm = new DynamicMethod(string.Empty, typeof(void), new Type[] { typeof(object), typeof(object) }, propertyInfo.DeclaringType.Module, true);
            //ILGenerator il = dm.GetILGenerator();
            //MethodInfo mt = propertyInfo.GetSetMethod(true);
            //if (!mt.IsStatic)
            //{
            //    LoadThis(il, mt.DeclaringType);
            //}
            //il.PushArgument(1);
            //il.Convert(typeof(object), propertyInfo.PropertyType);
            //il.Call(mt);
            //il.Ret();
            //return (SetHandler<object>)dm.CreateDelegate(typeof(SetHandler<object>));
        }
    }

    public delegate void SetHandler<T>(object target, T value);
    public delegate T GetHandler<T>(object target);
    public delegate object InvokeHandler(object target, object[] paramters);
    public delegate void SetRefHandler<T, V>(ref T target, V value);
}
