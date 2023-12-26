using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Reflection;
using CPF.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using CPF.Styling;
using CPF.Animation;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Collections;

namespace CPF
{
    /// <summary>
    /// 默认所有属性都是依赖属性，如果不想作为依赖属性，属性上加上[NotCpfProperty]特性。不能使用new来覆盖已经定义为依赖属性的属性，最多255个依赖属性
    /// </summary>
    public class CpfObject : INotifyPropertyChanged, IDisposable, IDisposed, ICloneable, Design.ISerializerCode, IObservable<EventObserver<EventArgs, CpfObject>>
    {
        static ConcurrentDictionary<Type, ObjInfo> typeCache = new ConcurrentDictionary<Type, ObjInfo>();
        static ConcurrentDictionary<Type, PropertyMetadataAttribute[]> typePropertyCache = new ConcurrentDictionary<Type, PropertyMetadataAttribute[]>();
        static ConcurrentDictionary<Type, HashSet<string>> inheritsProperties = new ConcurrentDictionary<Type, HashSet<string>>();
        static ConcurrentDictionary<Type, HashSet<string>> breakInheritsProperties = new ConcurrentDictionary<Type, HashSet<string>>();
        static ConcurrentDictionary<Type, List<MethodInfo>> propertyChangedMethods = new ConcurrentDictionary<Type, List<MethodInfo>>();

        static KeyValuePair<Type, ObjInfo>[] saveTypeCache;
        static KeyValuePair<Type, PropertyMetadataAttribute[]>[] saveTypePropertyCache;
        static KeyValuePair<Type, HashSet<string>>[] saveInheritsProperties;
        static KeyValuePair<Type, HashSet<string>>[] saveBreakInheritsProperties;
        static KeyValuePair<Type, List<MethodInfo>>[] savePropertyChangedMethods;
        public static void SetTypeCache()
        {
            saveTypeCache = typeCache.ToArray();
            saveTypePropertyCache = typePropertyCache.ToArray();
            saveInheritsProperties = inheritsProperties.ToArray();
            saveBreakInheritsProperties = breakInheritsProperties.ToArray();
            FastReflectionExtensions.SetTypeCache();
            savePropertyChangedMethods = propertyChangedMethods.ToArray();
        }
        public static void RecoveryTypeCache()
        {
            if (saveTypeCache != null)
            {
                typeCache.Clear();
                foreach (var item in saveTypeCache)
                {
                    typeCache.TryAdd(item.Key, item.Value);
                }
                saveTypeCache = null;
            }
            if (saveTypePropertyCache != null)
            {
                typePropertyCache.Clear();
                foreach (var item in saveTypePropertyCache)
                {
                    typePropertyCache.TryAdd(item.Key, item.Value);
                }
                saveTypePropertyCache = null;
            }
            if (saveInheritsProperties != null)
            {
                inheritsProperties.Clear();
                foreach (var item in saveInheritsProperties)
                {
                    inheritsProperties.TryAdd(item.Key, item.Value);
                }
                saveInheritsProperties = null;
            }
            if (saveBreakInheritsProperties != null)
            {
                breakInheritsProperties.Clear();
                foreach (var item in saveBreakInheritsProperties)
                {
                    breakInheritsProperties.TryAdd(item.Key, item.Value);
                }
                saveBreakInheritsProperties = null;
            }
            if (savePropertyChangedMethods != null)
            {
                propertyChangedMethods.Clear();
                foreach (var item in savePropertyChangedMethods)
                {
                    propertyChangedMethods.TryAdd(item.Key, item.Value);
                }
                savePropertyChangedMethods = null;
            }
            FastReflectionExtensions.RecoveryTypeCache();
        }

        ObjInfo objInfo;
        PropertyMetadataAttribute[] propertyInfos;
        //EffectiveValue[] values;
        byte[] valueIndexs;
        _List<EffectiveValue> valueList = new _List<EffectiveValue>();

        internal HybridDictionary<string, object> attachedValues;
        /// <summary>
        /// 继承属性，不同继承类型分支下来的属性名可能相同但是属性ID不同，只能保存属性名
        /// </summary>
        internal HashSet<string> inheritsPropertyName;
        /// <summary>
        /// 终止继承属性，不同继承类型分支下来的属性名可能相同但是属性ID不同，只能保存属性名
        /// </summary>
        internal HashSet<string> breakInheritsPropertyName;

        AttachedProperties attached;
        /// <summary>
        /// 用于设置附加属性，和绑定附加属性
        /// </summary>
        [Category("绑定")]
        [Description("用于设置附加属性，一般控件上会有附加属性分组，那边设置就行，可以不需要在这里设置。")]
        [NotCpfProperty]
        public AttachedProperties Attacheds
        {
            get
            {
                if (attached == null)
                {
                    attached = new AttachedProperties(this);
                }
                return attached;
            }
        }

        AttachedNotify attachedNotify;
        [NotCpfProperty]
        internal AttachedNotify AttachedNotify
        {
            get
            {
                if (attachedNotify == null)
                {
                    attachedNotify = new AttachedNotify(this);
                }
                return attachedNotify;
            }
        }

        internal Bindings bindings;
        /// <summary>
        /// 设置绑定
        /// </summary>
        [NotCpfProperty]
        [Category("绑定")]
        [Description("设置数据绑定")]
        public Bindings Bindings
        {
            get
            {
                if (bindings == null)
                {
                    bindings = new Bindings(this);
                }
                return bindings;
            }
        }

        internal Commands commands;
        /// <summary>
        /// 设置命令
        /// </summary>
        [NotCpfProperty]
        [Category("绑定")]
        [Description("设置命令绑定")]
        public Commands Commands
        {
            get
            {
                if (commands == null)
                {
                    commands = new Commands(this);
                }
                return commands;
            }
        }

        Type type;
        /// <summary>
        /// 设置绑定
        /// </summary>
        /// <param name="propertyName">需要绑定的属性名</param>
        ///// <param name="convert">绑定的属性值转换到源对象的属性值</param>
        /// <returns></returns>
        [NotCpfProperty]
        public BindingDescribe this[string propertyName]
        {
            get { return new BindingDescribe { Source = this, PropertyName = propertyName }; }
            set
            {
                if (value.Command != null)
                {
                    if (value.Command.Action != null)
                    {
                        Commands.Add(propertyName, value.Command.Action);
                    }
                    else if (!string.IsNullOrWhiteSpace(value.Command.MethodName))
                    {
                        if (value.Command.Find != null)
                        {
                            Commands.Add(propertyName, value.Command.MethodName, value.Command.Find, value.Command.Parameters);
                        }
                        else
                        {
                            Commands.Add(propertyName, value.Command.MethodName, value.Command.Target, value.Command.Parameters);
                        }
                    }
                }
                else if (value.Trigger != null)
                {
                    OnAddTriggerDescribe(propertyName, value.Trigger);
                }
                else
                {
                    if (value.Source is int layer)
                    {
                        Bindings.Add(propertyName, value.PropertyName, (byte)layer, value.BindingMode, value.Convert, value.ConvertBack, value.SourceToTargetError, value.TargetToSourceError);
                    }
                    if (value.Source is byte layer1)
                    {
                        Bindings.Add(propertyName, value.PropertyName, layer1, value.BindingMode, value.Convert, value.ConvertBack, value.SourceToTargetError, value.TargetToSourceError);
                    }
                    else if (value.Source is Func<UIElement, UIElement> find)
                    {
                        Bindings.Add(propertyName, value.PropertyName, find, value.BindingMode, value.Convert, value.ConvertBack, value.SourceToTargetError, value.TargetToSourceError);
                    }
                    else
                    {
                        Bindings.Add(propertyName, value.PropertyName, value.Source, value.BindingMode, value.Convert, value.ConvertBack, value.SourceToTargetError, value.TargetToSourceError);
                    }
                }
            }
        }

        internal protected virtual void OnAddTriggerDescribe(string property, TriggerDescribe trigger)
        {

        }

        /// <summary>
        /// 读取或者设置附加属性，参数必须是附加属性
        /// </summary>
        /// <param name="attached"></param>
        /// <returns></returns>
        [NotCpfProperty]
        public object this[MulticastDelegate attached]
        {
            get
            {
                var type = attached.GetType();
                try
                {
                    var p = typeof(OptionalParameter<>).MakeGenericType(type.GetGenericArguments()[0]);
                    //return attached(this);
                    return attached.Method.FastInvoke(attached.Target, this, Activator.CreateInstance(p));
                }
                catch (Exception e)
                {
                    if (type.GetGenericTypeDefinition() != typeof(Attached<>))
                    {
                        throw new Exception(attached + "必须是附加属性", e);
                    }
                    throw e;
                }
            }
            set
            {
                var type = attached.GetType();
                try
                {
                    if (value is AttachedDescribe describe)
                    {
                        var p = typeof(OptionalParameter<>).MakeGenericType(type.GetGenericArguments()[0]);
                        attached.Method.FastInvoke(attached.Target, this, Activator.CreateInstance(p, describe.Value));
                        var targetType = attached.Target.GetType();
                        var field = targetType.GetField("propertyName");
                        var name = field.FastGetValue(attached.Target).ToString();
                        AttachedNotify.Bindings.Add(name, describe.PropertyName, describe.Source, describe.BindingMode, describe.Convert, describe.ConvertBack, describe.SourceToTargetError, describe.TargetToSourceError);
                    }
                    else
                    {
                        var p = typeof(OptionalParameter<>).MakeGenericType(type.GetGenericArguments()[0]);
                        attached.Method.FastInvoke(attached.Target, this, Activator.CreateInstance(p, value));
                    }
                }
                catch (Exception e)
                {
                    if (type.GetGenericTypeDefinition() != typeof(Attached<>))
                    {
                        throw new Exception(attached + "必须是附加属性", e);
                    }
                    throw e;
                }
                //attached(this, value);
            }
        }

        public CpfObject()
        {
            type = this.GetType();
            //Threading.Dispatcher.MainThread.VerifyAccess();
            if (!typeCache.TryGetValue(type, out objInfo))
            {
                //typeNames.Add(type.Name, type);
                objInfo = new ObjInfo();
                typeCache.TryAdd(type, objInfo);
                List<PropertyMetadataAttribute> propertyList = new List<PropertyMetadataAttribute>();
                var list = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                list = list.OrderBy(a => a, new TypeCompar()).ThenBy(a => a.Name).ToArray();

                OverrideMetadata om = new OverrideMetadata();
                OnOverrideMetadata(om);
                var not = typeof(NotCpfProperty);
                var pm = typeof(PropertyMetadataAttribute);
                var cm = typeof(ComputedAttribute);
                byte id = 0;
                //List<(PropertyInfo, ComputedAttribute[])> computeProterty = new List<(PropertyInfo, ComputedAttribute[])>();
                List<ComputeProtertyInfo> computeProterty = new List<ComputeProtertyInfo>();
                foreach (var item in list)
                {
                    var nots = item.GetCustomAttributes(not, true);
                    if (nots.Length == 0)
                    {
                        var ass = item.GetCustomAttributes(pm, true);

                        PropertyMetadataAttribute a;
                        if (!om.list.TryGetValue(item.Name, out a))
                        {
                            if (ass.Length > 0)
                            {
                                a = (PropertyMetadataAttribute)ass[0];
                            }
                            else
                            {
                                a = new PropertyMetadataAttribute();
                                a.DefaultValue = item.PropertyType.IsValueType ? Activator.CreateInstance(item.PropertyType) : null; ;
                            }
                        }
                        a.PropertyType = item.PropertyType;
                        a.PropertyName = item.Name;
                        if (a.DefaultValue != null)
                        {
                            var t = a.DefaultValue.GetType();
                            if (t != a.PropertyType && !a.PropertyType.IsAssignableFrom(t))
                            {
                                try
                                {
                                    if (a.PropertyType.IsEnum && !t.IsEnum && t.IsValueType)
                                    {
                                        a.DefaultValue = Enum.ToObject(a.PropertyType, a.DefaultValue);
                                    }
                                    else
                                    {
                                        a.DefaultValue = Convert.ChangeType(a.DefaultValue, a.PropertyType);
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw new InvalidCastException($"{type.Name}的属性{item.Name}的默认值类型不对", e);
                                }
                            }
                        }
                        a.Id = id;
                        id++;
                        /*
                         备注一下把，如果自定义的组件有多个
                        public Char this[int place]
                        public string this[int place]
                        多个会导致item重复
                         */
                        if (objInfo.ContainsKey(item.Name))
                        {
                            string msg = $"\n重复的字段:{item.Name},或检查组件是否定义多个索引器\n" +
                                "例如:public string this[string index]";
                            throw new Exception(msg);
                        }
                        objInfo.Add(item.Name, a);
                        propertyList.Add(a);
                        var ca = item.GetCustomAttributes(cm, true);
                        if (ca.Length > 0)
                        {
                            a.Compute = true;
                            //computeProterty.Add((item, (ComputedAttribute[])ca));
                            computeProterty.Add(new ComputeProtertyInfo { Property = item, NoticeProperties = ((ComputedAttribute[])ca)[0].Properties });
                        }
                    }
                }

                typePropertyCache.TryAdd(type, propertyList.ToArray());

                var l = new HashSet<string>();
                var b = new HashSet<string>();
                foreach (var item in objInfo)
                {
                    if (item.Value is UIPropertyMetadataAttribute attribute)
                    {
                        if (attribute.Inherits)
                        {
                            l.Add(attribute.PropertyName);
                        }
                        else
                        {
                            b.Add(attribute.PropertyName);
                        }
                    }
                }
                if (l.Count == 0)
                {
                    l = null;
                }
                if (b.Count == 0)
                {
                    b = null;
                }
                inheritsProperties.TryAdd(type, l);
                breakInheritsProperties.TryAdd(type, b);

                //Type tt = typeof(PropertyChangedAttribute);
                //var ms = type.FindMembers(MemberTypes.Method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, (a, b) => a.CustomAttributes.Any(c => c.AttributeType == tt), null);
                foreach (MethodInfo item in Methods(type))
                {
                    try
                    {
                        var attrs = item.GetCustomAttributes(typeof(PropertyChangedAttribute), true);
                        foreach (PropertyChangedAttribute attr in attrs)
                        {
                            if (!objInfo.TryGetValue(attr.PropertyName, out PropertyMetadataAttribute attribute))
                            {
                                throw new Exception("不存在属性：" + attr.PropertyName);
                            }
                            if (attribute.actions == null)
                            {
                                attribute.actions = new List<PropertyChangedCallback>();
                            }
                            var instanceParameter = Expression.Parameter(typeof(CpfObject), "instance");
                            var newValueParameter = Expression.Parameter(typeof(object), "newValue");
                            var oldValueParameter = Expression.Parameter(typeof(object), "oldValue");
                            var attributeParameter = Expression.Parameter(typeof(PropertyMetadataAttribute), "attribute");
                            var instanceCast = Expression.Convert(instanceParameter, item.ReflectedType);
                            var methodCall = Expression.Call(instanceCast, item, new ParameterExpression[] { newValueParameter, oldValueParameter, attributeParameter });

                            var lambda = Expression.Lambda<PropertyChangedCallback>(
        methodCall, instanceParameter, newValueParameter, oldValueParameter, attributeParameter);

                            var execute = lambda.Compile();

                            attribute.actions.Add(execute);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("绑定属性通知出错，检查方法参数类型和返回值类型：" + item.Name, e);
                    }
                }

                OnInitializeComputeProterty(computeProterty.Where(a => a.NoticeProperties == null));

                if (computeProterty.Count > 0)
                {
                    objInfo.Computed = computeProterty;
                }
                foreach (var item in computeProterty)
                {
                    //if (item.Item2[0].Properties != null && item.Item2[0].Properties.Length > 0)
                    if (item.NoticeProperties == null && item.Tokens != null)
                    {
                        List<string> ps = new List<string>();
                        foreach (var t in item.Tokens)
                        {
                            var p = list.FirstOrDefault(a => a.MetadataToken == t);
                            if (p != null)
                            {
                                ps.Add(p.Name);
                            }
                        }
                        item.NoticeProperties = ps.ToArray();
                    }
                    if (item.NoticeProperties != null && item.NoticeProperties.Length > 0)
                    {
                        //foreach (var p in item.Item2[0].Properties)
                        foreach (var p in item.NoticeProperties)
                        {
                            if (!string.IsNullOrWhiteSpace(p))
                            {
                                if (!objInfo.TryGetValue(p, out PropertyMetadataAttribute attribute))
                                {
                                    throw new Exception("不存在属性：" + p);
                                }
                                if (attribute.actions == null)
                                {
                                    attribute.actions = new List<PropertyChangedCallback>();
                                }
                                var instanceParameter = Expression.Parameter(typeof(CpfObject), "instance");
                                var newValueParameter = Expression.Parameter(typeof(object), "newValue");
                                var oldValueParameter = Expression.Parameter(typeof(object), "oldValue");
                                var attributeParameter = Expression.Parameter(typeof(PropertyMetadataAttribute), "attribute");
                                //var instanceCast = Expression.Convert(instanceParameter, item.Item1.ReflectedType);
                                var instanceCast = Expression.Convert(instanceParameter, item.Property.ReflectedType);
                                var propValue = Expression.Property(instanceCast, item.Property);
                                var methodCall = Expression.Call(instanceCast, setValue, Expression.Convert(propValue, typeof(object)), Expression.Constant(item.Property.Name));
                                var lambda = Expression.Lambda<PropertyChangedCallback>(
            methodCall, instanceParameter, newValueParameter, oldValueParameter, attributeParameter);

                                var execute = lambda.Compile();

                                attribute.actions.Add(execute);
                            }
                        }
                    }
                }
            }
            //values = new EffectiveValue[objInfo.Count];
            propertyInfos = typePropertyCache[type];

            if (objInfo.Count > 255)
            {
                throw new Exception(type + "类型属性数量不能超过255");
            }
            //valueIndexs = new ByteArray((byte)objInfo.Count);
            valueIndexs = new byte[objInfo.Count];
            inheritsPropertyName = inheritsProperties[type];
            breakInheritsPropertyName = breakInheritsProperties[type];

            if (objInfo.Computed != null)
            {
                foreach (var item in objInfo.Computed)
                {
                    SetValue(FastReflectionExtensions.GetValue(this, item.Property.Name), item.Property.Name);
                }
            }
        }
        class TypeCompar : IComparer<PropertyInfo>
        {
            public int Compare(PropertyInfo a, PropertyInfo b)
            {
                var x = a.DeclaringType;
                var y = b.DeclaringType;
                var gx = a.GetGetMethod();
                if (gx != null)
                {
                    var baseXD = gx.GetBaseDefinition();
                    var x1 = baseXD.DeclaringType;
                    while (x1 != x)
                    {
                        x = x1;
                        baseXD = baseXD.GetBaseDefinition();
                        x1 = baseXD.DeclaringType;
                    }
                    x = x1;
                }
                else
                {
                    var sx = a.GetSetMethod();
                    if (sx != null)
                    {
                        var baseXD = sx.GetBaseDefinition();
                        var x1 = baseXD.DeclaringType;
                        while (x1 != x)
                        {
                            x = x1;
                            baseXD = baseXD.GetBaseDefinition();
                            x1 = baseXD.DeclaringType;
                        }
                        x = x1;
                    }
                }
                var gy = b.GetGetMethod();
                if (gy != null)
                {
                    var baseYD = gy.GetBaseDefinition();
                    var y1 = baseYD.DeclaringType;
                    while (y1 != y)
                    {
                        y = y1;
                        baseYD = baseYD.GetBaseDefinition();
                        y1 = baseYD.DeclaringType;
                    }
                    y = y1;
                }
                else
                {
                    var sy = b.GetSetMethod();
                    if (sy != null)
                    {
                        var baseYD = sy.GetBaseDefinition();
                        var y1 = baseYD.DeclaringType;
                        while (y1 != y)
                        {
                            y = y1;
                            baseYD = baseYD.GetBaseDefinition();
                            y1 = baseYD.DeclaringType;
                        }
                        y = y1;
                    }
                }

                if (x == y)
                {
                    return 0;
                }
                if (x.IsAssignableFrom(y))
                {
                    return -1;
                }
                return 1;
            }
        }
        static MethodInfo setValue = typeof(CpfObject).GetMethod(nameof(InnerSetValue), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        static IEnumerable<MethodInfo> Methods(Type type)
        {
            if (!propertyChangedMethods.TryGetValue(type, out List<MethodInfo> methods))
            {
                methods = new List<MethodInfo>();
                Type t = typeof(PropertyChangedAttribute);
                //var ms = type.FindMembers(MemberTypes.Method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, (a, b) => a.CustomAttributes.Any(c => c.AttributeType == t), null);
                var ms = type.FindMembers(MemberTypes.Method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, (a, b) =>
                {
                    var alist = a.GetCustomAttributes(t, true);
                    return alist != null && alist.Length > 0;
                }, null);
                foreach (MethodInfo item in ms)
                {
                    if (!item.IsStatic)
                    {
                        methods.Add(item);
                    }
                }
                propertyChangedMethods.TryAdd(type, methods);
            }
            foreach (var item in methods)
            {
                yield return item;
            }
            if (type.BaseType != typeof(object))
            {
                foreach (var item in Methods(type.BaseType))
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// 用于初始化计算属性，请不要调用和重写，内部使用
        /// </summary>
        /// <param name="computeProterties"></param>
        protected virtual void OnInitializeComputeProterty(IEnumerable<ComputeProtertyInfo> computeProterties)
        {
            //foreach (var item in computeProterties)
            //{
            //    if (item.Property.Name == "通知属性1")
            //    {
            //        item.NoticeProperties = new string[] { "属性1", "属性2" };

            //        item.Tokens = new int[] { };//如果没有解析到属性名，NoticeProperties不要设置
            //    }
            //}
        }

        /// <summary>
        /// 该类型的第一个对象构造的时候调用，重写属性元数据，一般重写属性的代码写在base.OnOverrideMetadata后面
        /// </summary>
        /// <param name="overridePropertys"></param>
        protected virtual void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {

        }

        //[PropertyMetadata(null)]
        //public string Name
        //{
        //    get { return GetValue<string>(); }
        //    set { SetValue(value); }
        //}
        /// <summary>
        /// 绑定的数据上下文
        /// </summary>
        [Category("绑定")]
        [Description("绑定的数据上下文")]
        [UIPropertyMetadata(null, true)]
        public virtual object DataContext
        {
            get { return GetValue<object>(1); }
            set { SetValue(value, 1); }
        }

        /// <summary>
        /// 绑定的命令上下文
        /// </summary>
        [Category("绑定")]
        [Description("绑定的命令上下文")]
        [UIPropertyMetadata(null, true)]
        public virtual object CommandContext
        {
            get { return GetValue<object>(0); }
            set { SetValue(value, 0); }
        }

        /// <summary>
        /// 当前对象的类型
        /// </summary>
        [NotCpfProperty]
        [Browsable(false)]
        public Type Type
        {
            get
            {
                //if (type == null)
                //{
                //    type = this.GetType();
                //}
                return type;
            }
        }
        /// <summary>
        /// 附加属性更改时发生
        /// </summary>
        /// <param name="ownerType">所注册在的类型</param>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnAttachedChanged(Type ownerType, string propertyName, object defaultValue, object oldValue, object newValue)
        {
            if (attachedNotify != null)
            {
                attachedNotify.OnPropertyChanged(propertyName, oldValue, newValue, attachedNotify.GetPropertyMetadata(propertyName));
            }
        }

        /// <summary>
        /// 注册附加属性
        /// </summary>
        /// <typeparam name="Value"></typeparam>
        /// <param name="defaultValue">默认值</param>
        /// <param name="ownerType">所注册在的类型</param>
        /// <param name="propertyChanged">属性变化回调</param>
        /// <param name="propertyName">属性名一般不用设置，VS自动设置</param>
        /// <returns></returns>
        public static Attached<Value> RegisterAttached<Value>(Value defaultValue, Type ownerType, AttachedPropertyChanged propertyChanged = null, [CallerMemberName] string propertyName = null)
        {
            var data = new AttachedData<Value> { defaultValue = defaultValue, ownerType = ownerType, propertyChanged = propertyChanged, propertyName = propertyName };
            var func = new Attached<Value>(data.Attached);
            return func;
        }

        class AttachedData<Value>
        {
            public Value defaultValue;
            public Type ownerType;
            public AttachedPropertyChanged propertyChanged = null;
            public string propertyName = null;
            public Value Attached<Value>(CpfObject obj, OptionalParameter<Value> value = default)
            {
                obj.AttachedNotify.AddAttached(propertyName, new Attached<Value>(Attached));
                if (obj.attachedValues == null)
                {
                    obj.attachedValues = new HybridDictionary<string, object>();
                }
                if (obj.attachedValues.TryGetValue(ownerType.Name + "." + propertyName, out object v))
                {
                    if (value.HasValue)
                    {
                        obj.attachedValues.Remove(ownerType.Name + "." + propertyName);
                    }
                }
                else
                {
                    v = defaultValue;
                }
                if (value.HasValue)
                {
                    object newValue = value.Value;
                    propertyChanged?.Invoke(obj, propertyName, defaultValue, v, ref newValue);
                    obj.attachedValues.Add(ownerType.Name + "." + propertyName, value.Value);
                    obj.OnAttachedChanged(ownerType, propertyName, defaultValue, v, newValue);
                    v = newValue;
                }
                return (Value)v;
            }
        }

        ///// <summary>
        ///// 当修改附加属性的时候
        ///// </summary>
        ///// <param name="object">被附加的对象</param>
        ///// <param name="propertyName"></param>
        ///// <param name="defaultValue"></param>
        ///// <param name="oldValue"></param>
        ///// <param name="newValue"></param>
        //protected virtual void OnAttachedChanged(CPFObject @object, string propertyName, object defaultValue, object oldValue, ref object newValue)
        //{

        //}

        bool TryGetValue(string propertyName, out PropertyMetadataAttribute attribute, out EffectiveValue value)
        {
            //if (disposedValue)
            //{
            //    value = null;
            //    attribute = null;
            //    return false;
            //}
            if (!objInfo.TryGetValue(propertyName, out attribute))
            {
                value = null;
                return false;
            }
            //value = values[attribute.Id];
            value = GetEffectiveValue(attribute);
            return value != null;
        }

        internal EffectiveValue GetEffectiveValue(PropertyMetadataAttribute attribute)
        {
            var id = valueIndexs[attribute.Id] - 1;//因为默认值就是0，所以要-1
            if (id > -1 && id < valueList.Count)
            {
                return valueList[id];
            }
            return null;
        }

        void SetValue(PropertyMetadataAttribute attribute, EffectiveValue value)
        {
            var id = valueIndexs[attribute.Id] - 1;
            if (id > -1 && id < valueList.Count)
            {
                valueList[id] = value;
            }
            else
            {
                valueList.Add(value);
                valueIndexs[attribute.Id] = valueList.Count;
            }
        }

        /// <summary>
        /// 获取有LocalValue的属性和值
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(string, object)> GetHasLocalValueProperties()
        {
            foreach (var item in objInfo)
            {
                //var value = values[item.Value.Id];
                var value = GetEffectiveValue(item.Value);
                if (value != null && value.LocalValue.HasValue)
                {
                    yield return (item.Key, value.LocalValue.Value);
                }
            }
        }

        internal bool InnerSetValue(object value, string property)
        {
            return SetValue(value, property);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns>设置属性值是否成功</returns>
        public virtual bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new Exception("propertyName不能为空");
            }
            //Threading.Dispatcher.MainThread.VerifyAccess();
            object v = value;
            if (OnSetValue(propertyName, ref v))
            {
                //bool ex = true;
                object oldValue;
                EffectiveValue oValue;
                PropertyMetadataAttribute p;

                if (!TryGetValue(propertyName, out p, out oValue))
                {
                    if (p == null)
                    {
                        //throw new Exception("未找到该属性的元数据：" + propertyName);
                        return false;
                    }
                    oldValue = OnGetDefaultValue(p);
                }
                else
                {
                    //p = oValue.PropertyMetadata;
                    if (!oValue.GetLocalValue(out oldValue))
                    {
                        oldValue = OnGetDefaultValue(p);
                    }
                }
                if (v != null)
                {
                    var t = v.GetType();
                    if (t != p.PropertyType && !p.PropertyType.IsAssignableFrom(t))
                    {
                        v = value.ConvertTo(p.PropertyType);
                    }
                }
                //if ((oldValue == null && v != null) || (oldValue != null && v == null) || (oldValue != null && !oldValue.Equals(v)))
                if (!oldValue.Equal(v))
                {
                    if (oValue == null)
                    {
                        oValue = new EffectiveValue();
                        //values[p.Id] = oValue;
                        SetValue(p, oValue);
                    }
                    oValue.LocalValue.HasValue = true;
                    oValue.LocalValue.Value = v;
                    oValue.ClearStyleValues();
                    if (!oValue.HasStyleValue())
                    {
                        OnSetValue(propertyName, p, ValueFrom.Property, v);
                        OnPropertyChanged(propertyName, oldValue, v, p);
                    }
                }
                else if (oldValue == null && oValue == null && v == null && (propertyName == nameof(DataContext) || propertyName == nameof(CommandContext)))
                {
                    oValue = new EffectiveValue();
                    //values[p.Id] = oValue;
                    SetValue(p, oValue);
                    oValue.LocalValue.HasValue = true;
                }
                else if (p is UIPropertyMetadataAttribute ui && ui.Inherits && propertyName != nameof(DataContext) && propertyName != nameof(CommandContext))
                {
                    if (oValue == null)
                    {
                        oValue = new EffectiveValue();
                        //values[p.Id] = oValue;
                        SetValue(p, oValue);
                    }
                    oValue.LocalValue.HasValue = true;
                    oValue.LocalValue.Value = v;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 内部使用，请勿调用
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyIndex"></param>
        /// <returns></returns>
        protected virtual bool SetValue(object value, in byte propertyIndex)
        {
            var p = propertyInfos[propertyIndex];
            object v = value;
            var propertyName = p.PropertyName;
            if (OnSetValue(propertyName, ref v))
            {
                //bool ex = true;
                object oldValue;
                EffectiveValue oValue;

                if (!TryGetValue(propertyName, out p, out oValue))
                {
                    if (p == null)
                    {
                        //throw new Exception("未找到该属性的元数据：" + propertyName);
                        return false;
                    }
                    oldValue = OnGetDefaultValue(p);
                }
                else
                {
                    //p = oValue.PropertyMetadata;
                    if (!oValue.GetLocalValue(out oldValue))
                    {
                        oldValue = OnGetDefaultValue(p);
                    }
                }
                if (value != null)
                {
                    var t = value.GetType();
                    if (t != p.PropertyType && !p.PropertyType.IsAssignableFrom(t))
                    {
                        v = value.ConvertTo(p.PropertyType);
                    }
                }
                //if ((oldValue == null && v != null) || (oldValue != null && v == null) || (oldValue != null && !oldValue.Equals(v)))
                if (!oldValue.Equal(v))
                {
                    if (oValue == null)
                    {
                        oValue = new EffectiveValue();
                        //values[p.Id] = oValue;
                        SetValue(p, oValue);
                    }
                    oValue.LocalValue.HasValue = true;
                    oValue.LocalValue.Value = v;
                    oValue.ClearStyleValues();
                    if (!oValue.HasStyleValue())
                    {
                        OnSetValue(propertyName, p, ValueFrom.Property, v);
                        OnPropertyChanged(propertyName, oldValue, v, p);
                    }
                }
                else if (oldValue == null && oValue == null && v == null && (propertyName == nameof(DataContext) || propertyName == nameof(CommandContext)))
                {
                    oValue = new EffectiveValue();
                    //values[p.Id] = oValue;
                    SetValue(p, oValue);
                    oValue.LocalValue.HasValue = true;
                }
                else if (p is UIPropertyMetadataAttribute ui && ui.Inherits && propertyName != nameof(DataContext) && propertyName != nameof(CommandContext))
                {
                    if (oValue == null)
                    {
                        oValue = new EffectiveValue();
                        //values[p.Id] = oValue;
                        SetValue(p, oValue);
                    }
                    oValue.LocalValue.HasValue = true;
                    oValue.LocalValue.Value = v;
                }
                return true;
            }
            return false;
        }

        internal void SetStyleValue(string propertyName, CPF.Styling.StyleValue styleValue)
        {
            var value = styleValue.Value;
            var style = styleValue.Style;
            if (OnSetValue(propertyName, ref value))
            {
                EffectiveValue oValue;
                PropertyMetadataAttribute p;
                if (!TryGetValue(propertyName, out p, out oValue))
                {
                    if (p == null)
                    {
                        throw new Exception("未找到该属性的元数据：" + propertyName);
                    }
                    //oValue.PropertyMetadata = p;
                    oValue = new EffectiveValue();
                    //values[p.Id] = oValue;
                    SetValue(p, oValue);
                }
                try
                {
                    if (oValue.styleValues == null || oValue.styleValues.Count == 0 ||
                       (oValue.styleValues[oValue.styleValues.Count - 1].StyleValue.IsImportant == styleValue.IsImportant && oValue.styleValues[oValue.styleValues.Count - 1].StyleValue.Style.Index < style.Index) ||
                       (styleValue.IsImportant && !oValue.styleValues[oValue.styleValues.Count - 1].StyleValue.IsImportant))
                    {
                        object oldValue;
                        if (!oValue.GetValue(out oldValue))
                        {
                            oldValue = OnGetDefaultValue(p);
                        }

                        oValue.SetStyleValue(styleValue, value);
                        if ((oldValue == null && value != null) || (oldValue != null && value == null) || (oldValue != null && !oldValue.Equals(value)))
                        {
                            OnSetValue(propertyName, p, ValueFrom.Style, value);
                            OnPropertyChanged(propertyName, oldValue, value, p);
                        }
                    }
                    else
                    {
                        oValue.SetStyleValue(styleValue, value);
                        oValue.styleValues.Sort(styleSort);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(this + " " + style.Selector.ToString() + " " + style.Index, e);
                }
            }
        }

        static StyleSort styleSort = new StyleSort();
        class StyleSort : IComparer<SavedStyleValue>
        {
            public int Compare(SavedStyleValue x, SavedStyleValue y)
            {
                return x.StyleValue.IsImportant == y.StyleValue.IsImportant ? (x.StyleValue.Style.Index - y.StyleValue.Style.Index) : (x.StyleValue.IsImportant ? 1 : -1);
            }
        }
        static TriggerSort triggerSort = new TriggerSort();
        class TriggerSort : IComparer<TriggerValue>
        {
            public int Compare(TriggerValue x, TriggerValue y)
            {
                if (x.StyleValue != null && y.StyleValue != null && x.StyleValue.IsImportant != y.StyleValue.IsImportant)
                {
                    if (x.StyleValue.IsImportant)
                    {
                        return 1;
                    }
                    else if (y.StyleValue.IsImportant)
                    {
                        return -1;
                    }
                }
                else if (x.StyleValue != y.StyleValue)
                {
                    if (x.StyleValue != null && x.StyleValue.IsImportant)
                    {
                        return 1;
                    }
                    else if (y.StyleValue != null && y.StyleValue.IsImportant)
                    {
                        return -1;
                    }
                }
                return 0;
            }
        }

        internal void SetAnimationValue(string propertyName, Storyboard Storyboard, object value)
        {
            if (OnSetValue(propertyName, ref value))
            {
                object oldValue;
                EffectiveValue oValue;
                PropertyMetadataAttribute p;
                if (!TryGetValue(propertyName, out p, out oValue))
                {
                    if (p == null)
                    {
                        throw new Exception("未找到该属性的元数据：" + propertyName);
                    }
                    //oValue.PropertyMetadata = p;
                    oValue = new EffectiveValue();
                    //values[p.Id] = oValue;
                    SetValue(p, oValue);
                }
                //else
                //{
                //    p = oValue.PropertyMetadata;
                //}
                if (!oValue.GetValue(out oldValue))
                {
                    oldValue = OnGetDefaultValue(p);
                }

                oValue.SetAnimationValue(Storyboard, value);
                if ((oldValue == null && value != null) || (oldValue != null && value == null) || (oldValue != null && !oldValue.Equals(value)))
                {
                    OnSetValue(propertyName, p, ValueFrom.Animation, value);
                    OnPropertyChanged(propertyName, oldValue, value, p);
                }
            }
        }
        internal void SetTriggerValue(string propertyName, Trigger Trigger, object value)
        {
            if (value is StyleValue styleValue)
            {
                if (!styleValue.HasValue)
                {
                    value = styleValue.CssValue;
                }
                else
                {
                    value = styleValue.Value;
                }
            }
            else
            {
                styleValue = null;
            }
            if (OnSetValue(propertyName, ref value))
            {
                object oldValue;
                EffectiveValue oValue;
                PropertyMetadataAttribute p;
                if (!TryGetValue(propertyName, out p, out oValue))
                {
                    if (p == null)
                    {
                        throw new Exception("未找到该属性的元数据：" + propertyName);
                    }
                    //oValue.PropertyMetadata = p;
                    oValue = new EffectiveValue();
                    //values[p.Id] = oValue;
                    SetValue(p, oValue);
                }
                //else
                //{
                //    p = oValue.PropertyMetadata;
                //}
                if (value != null)
                {
                    var vType = value.GetType();
                    if (vType != p.PropertyType && !p.PropertyType.IsAssignableFrom(vType))
                    {
                        value = value.ConvertTo(p.PropertyType);
                    }
                }
                if (!oValue.GetValue(out oldValue))
                {
                    oldValue = OnGetDefaultValue(p);
                }
                oValue.SetTriggerValue(Trigger, value, styleValue);
                if (oValue.TriggerValue.Count > 0)
                {
                    oValue.TriggerValue.Sort(triggerSort);
                    value = oValue.TriggerValue[oValue.TriggerValue.Count - 1].Value;
                }
                if (Trigger.SetPropertys == null)
                {
                    Trigger.SetPropertys = new HybridDictionary<CpfObject, List<string>>();
                }
                if (!Trigger.SetPropertys.TryGetValue(this, out List<string> ps))
                {
                    ps = new List<string>();
                    Trigger.SetPropertys.Add(this, ps);
                }
                ps.Add(propertyName);
                if ((oldValue == null && value != null) || (oldValue != null && value == null) || (oldValue != null && !oldValue.Equals(value)))
                {
                    OnSetValue(propertyName, p, ValueFrom.Trigger, value);
                    OnPropertyChanged(propertyName, oldValue, value, p);
                }
            }
        }

        [PropertyChanged(nameof(DataContext))]
        void OnRenderDataContext(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (oldValue != null)
            {
                if (bindings != null)
                {
                    foreach (var item in bindings.binds)
                    {
                        foreach (var i in item.Value)
                        {
                            if (i.IsDataContext && i.Source != null && i.Source.IsAlive)
                            {
                                INotifyPropertyChanged n = i.Source.Target as INotifyPropertyChanged;
                                if (n != null)
                                {
                                    //n.PropertyChanged -= i.PropertyChanged;
                                    i.CancellationPropertyChanged(n);
                                }
                                i.Source = null;
                            }
                        }
                    }
                }
            }
            //if (newValue != null)
            {
                if (bindings != null)
                {
                    foreach (var item in bindings.binds)
                    {
                        foreach (var i in item.Value)
                        {
                            if (i.IsDataContext)
                            {
                                if (newValue != null)
                                {
                                    i.Source = new WeakReference(newValue);
                                }
                                else
                                {
                                    i.Source = null;
                                }
                                if (i.BindingMode == BindingMode.OneWay || i.BindingMode == BindingMode.TwoWay)
                                {
                                    INotifyPropertyChanged no = newValue as INotifyPropertyChanged;
                                    if (no != null)
                                    {
                                        i.RegisterPropertyChanged(no);
                                    }
                                }
                                if (i.BindingMode != BindingMode.OneWayToSource)
                                {
                                    i.SourceToTarget();
                                }
                                else
                                {
                                    if (newValue != null)
                                    {
                                        i.TargetToSource();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 一般不建议在这里处理属性通知，建议用PropertyChanged特性来注册属性通知。
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyMetadata"></param>
        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            if (propertyMetadata.actions != null)
            {
                //foreach (var item in propertyMetadata.actions)
                //{
                //    item(this, newValue, oldValue, propertyMetadata);
                //}
                for (int i = 0; i < propertyMetadata.actions.Count; i++)
                {
                    propertyMetadata.actions[i](this, newValue, oldValue, propertyMetadata);
                }
            }
            //if (propertyName == nameof(DataContext))
            //{
            //    if (oldValue != null)
            //    {
            //        if (bindings != null)
            //        {
            //            foreach (var item in bindings.binds)
            //            {
            //                foreach (var i in item.Value)
            //                {
            //                    if (i.IsDataContext && i.Source != null && i.Source.IsAlive)
            //                    {
            //                        INotifyPropertyChanged n = i.Source.Target as INotifyPropertyChanged;
            //                        if (n != null)
            //                        {
            //                            //n.PropertyChanged -= i.PropertyChanged;
            //                            i.CancellationPropertyChanged(n);
            //                        }
            //                        i.Source = null;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    if (newValue != null)
            //    {
            //        if (bindings != null)
            //        {
            //            foreach (var item in bindings.binds)
            //            {
            //                foreach (var i in item.Value)
            //                {
            //                    if (i.IsDataContext)
            //                    {
            //                        i.Source = new WeakReference(newValue);
            //                        if (i.BindingMode == BindingMode.OneWay || i.BindingMode == BindingMode.TwoWay)
            //                        {
            //                            INotifyPropertyChanged no = newValue as INotifyPropertyChanged;
            //                            if (no != null)
            //                            {
            //                                i.RegisterPropertyChanged(no);
            //                            }
            //                        }
            //                        if (i.BindingMode != BindingMode.OneWayToSource)
            //                        {
            //                            i.SourceToTarget();
            //                        }
            //                        else
            //                        {
            //                            i.TargetToSource();
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //var cpc = new CPFPropertyChangedEventArgs { NewValue = newValue, OldValue = oldValue, PropertyMetadata = propertyMetadata, PropertyName = propertyName };
            using (var cpc = CPFPropertyChangedEventArgs.Create(propertyName, oldValue, newValue, propertyMetadata))
            {
                if (bindings != null && bindings.binds.TryGetValue(propertyName, out List<Binding> list))
                {
                    SetBinding(list);
                }
                if (commands != null && commands.commands.TryGetValue(propertyName, out List<Command> list1))
                {
                    SetCommand(cpc, list1);
                }

                RaiseEvent(cpc, strPropertyChanged);

                //PropertyChangedEventHandler handler = (PropertyChangedEventHandler)Events["INotifyPropertyChanged"];
                NotifyPropertyChanged(propertyName);
            }
        }
        const string strPropertyChanged = "PropertyChanged";

        /// <summary>
        /// 触发INotifyPropertyChanged的PropertyChanged事件
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (propertyChangedEventHandler != null)
            {
                propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
                //propertyChangedEventHandler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SetCommand(in CPFPropertyChangedEventArgs eventArgs, List<Command> list1)
        {
            foreach (var item in list1)
            {
                if (item.Action == null)
                {
                    var objs = new List<object>();
                    object v = null;
                    if (item.Target != null)
                    {
                        v = item.Target.Target;
                        objs.Add(v);
                    }
                    //else if (item.Relation != null && this is UIElement)
                    //{
                    //    objs.AddRange(item.Relation.Query(this as UIElement));
                    //}
                    else
                    {
                        v = CommandContext;
                        if (v != null)
                        {
                            objs.Add(v);
                        }
                    }
                    //var v = GetValue(item.PropertyName);
                    foreach (var vv in objs)
                    {
                        object[] ps = new object[item.Params == null ? 0 : item.Params.Length];
                        if (item.Params != null && item.Params.Length > 0)
                        {
                            item.Params.CopyTo(ps, 0);
                            //ps = item.Params;
                            for (int i = 0; i < ps.Length; i++)
                            {
                                var p = ps[i];
                                if (p is CommandParameter)
                                {
                                    if ((CommandParameter)p == CommandParameter.PropertyValue)
                                    {
                                        ps[i] = eventArgs.NewValue;
                                    }
                                    else if ((CommandParameter)p == CommandParameter.OldPropertyValue)
                                    {
                                        ps[i] = eventArgs.OldValue;
                                    }
                                    else if ((CommandParameter)p == CommandParameter.PropertyMetadata)
                                    {
                                        ps[i] = eventArgs.PropertyMetadata;
                                    }
                                    else if ((CommandParameter)p == CommandParameter.EventArgs)
                                    {
                                        ps[i] = eventArgs;
                                    }
                                    else if ((CommandParameter)p == CommandParameter.EventSender)
                                    {
                                        ps[i] = this;
                                    }
                                }
                            }
                        }
                        //var m = vv.GetType().GetMethod(item.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        //if (m == null)
                        //{
                        //    throw new Exception("未找到该方法 " + item.MethodName);
                        //}
                        //m.Invoke(vv, ps);
                        vv.Invoke(item.MethodName, ps);
                    }
                }
                else
                {
                    item.Action(this, eventArgs);
                }
            }
        }

        private static void SetBinding(List<Binding> tlist)
        {
            foreach (var item in tlist)
            {
                if (item.Source != null && (item.BindingMode == BindingMode.TwoWay || item.BindingMode == BindingMode.OneWayToSource))
                {
                    if (item.Source.IsAlive)
                    {
                        item.TargetToSource();
                    }
                }
            }
        }


        /// <summary>
        /// 当要设置属性值的时候，返回值为true的时候将设置值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns>返回值为true的时候将设置值</returns>
        protected virtual bool OnSetValue(string propertyName, ref object value)
        {
            return true;
        }
        ///// <summary>
        ///// 清除对象设置的属性值，恢复默认值
        ///// </summary>
        ///// <param name="propertyName"></param>
        //public void ClearValue(string propertyName)
        //{
        //    if (values.ContainsKey(propertyName))
        //    {
        //        var oldValue = GetValue(propertyName);
        //        values.Remove(propertyName);
        //        var newValue = GetValue(propertyName);
        //        if (!oldValue.Equal(newValue))
        //        {
        //            OnPropertyChanged(propertyName, oldValue, newValue, GetPropertyMetadata(propertyName));
        //        }
        //    }
        //}
        /// <summary>
        /// 清除本地值
        /// </summary>
        /// <param name="propertyName"></param>
        public void ClearLocalValue(string propertyName)
        {
            EffectiveValue v;
            if (TryGetValue(propertyName, out PropertyMetadataAttribute p, out v))
            {
                var oldValue = GetValue(propertyName);
                v.LocalValue = new EffectiveValueEntry();
                var newValue = GetValue(propertyName);
                OnClearLocalValue(propertyName, v);
                if ((oldValue != null && !oldValue.Equals(newValue)) || (oldValue == null && newValue != null))
                {
                    OnSetValue(propertyName, p, ValueFrom.Property, newValue);
                    OnPropertyChanged(propertyName, oldValue, newValue, p);
                }
            }
        }


        internal virtual void OnClearLocalValue(string propertyName, EffectiveValue value)
        {

        }
        internal virtual void ClearAnimationValue(Storyboard Storyboard, string propertyName)
        {
            EffectiveValue value;
            if (TryGetValue(propertyName, out PropertyMetadataAttribute p, out value))
            {
                var oldValue = GetValue(propertyName);
                var r = value.ClearAnimationValue(Storyboard);
                if (r)
                {
                    OnClearAnimationValue(propertyName, value);
                    //var pm = GetPropertyMetadata(propertyName);
                    var newValue = GetValue(propertyName);
                    if (!isDisposing && ((oldValue != null && !oldValue.Equals(newValue)) || (oldValue == null && newValue != null)))
                    {
                        OnSetValue(propertyName, p, ValueFrom.Animation, newValue);
                        OnPropertyChanged(propertyName, oldValue, newValue, p);
                    }
                }
            }
        }

        internal virtual void OnClearAnimationValue(string propertyName, EffectiveValue value)
        {

        }

        internal virtual void ClearTriggerValue(Trigger Trigger, string propertyName)
        {
            EffectiveValue value;
            if (TryGetValue(propertyName, out PropertyMetadataAttribute p, out value))
            {
                var oldValue = GetValue(propertyName);
                var r = value.ClearTriggerValue(Trigger);
                if (r)
                {
                    OnClearTriggerValue(propertyName, value);
                    var newValue = GetValue(propertyName);
                    if (!isDisposing && ((oldValue != null && !oldValue.Equals(newValue)) || (oldValue == null && newValue != null)))
                    {
                        OnSetValue(propertyName, p, ValueFrom.Trigger, newValue);
                        OnPropertyChanged(propertyName, oldValue, newValue, p);
                    }
                }
            }
        }

        internal virtual void OnClearTriggerValue(string propertyName, EffectiveValue value)
        {

        }

        internal virtual void OnSetValue(string propertyName, PropertyMetadataAttribute property, ValueFrom valueForm, object newValue)
        {

        }

        internal void ClearStyleValue(Style style)
        {
            foreach (var item in objInfo)
            {
                EffectiveValue value = GetEffectiveValue(item.Value);
                if (value.HasStyle(style))
                {
                    var oldValue = GetValue(item.Key);
                    var r = value.ClearStyleValue(style);
                    if (r)
                    {
                        OnClearStyleValue(item.Key, value);
                        var newValue = GetValue(item.Key);
                        if ((oldValue != null && !oldValue.Equals(newValue)) || (oldValue == null && newValue != null))
                        {
                            OnSetValue(item.Key, item.Value, ValueFrom.Style, newValue);
                            OnPropertyChanged(item.Key, oldValue, newValue, item.Value);
                        }
                    }
                }
            }
        }

        internal virtual void OnClearStyleValue(string propertyName, EffectiveValue value)
        {

        }

        internal void ClearStyleValues()
        {
            if (isDisposing)
            {
                return;
            }
            foreach (var item in objInfo)
            {
                //EffectiveValue value = values[item.Value.Id];
                EffectiveValue value = GetEffectiveValue(item.Value);
                //if (TryGetValue(item.Key, out PropertyMetadataAttribute p, out value))
                if (value != null)
                {
                    var oldValue = GetValue(item.Key);
                    var r = value.ClearStyleValues();
                    if (r)
                    {
                        OnClearStyleValue(item.Key, value);
                        var newValue = GetValue(item.Key);
                        if (((oldValue != null && !oldValue.Equals(newValue)) || (oldValue == null && newValue != null)))
                        {
                            OnSetValue(item.Key, item.Value, ValueFrom.Style, newValue);
                            OnPropertyChanged(item.Key, oldValue, newValue, item.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new Exception("propertyName不能为空");
            }
            //Threading.Dispatcher.MainThread.VerifyAccess();
            EffectiveValue value;
            if (TryGetValue(propertyName, out PropertyMetadataAttribute p, out value))
            {
                object v;
                if (value.GetValue(out v))
                {
                    return (T)v;
                }
            }
            if (p == null)
            {
                throw new Exception("未找到该属性的元数据：" + propertyName);
            }
            return (T)OnGetDefaultValue(p);
        }
        /// <summary>
        /// 内部使用，请勿调用
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual object GetValue(in byte index)
        {
            var pa = propertyInfos[index];
            var value = GetEffectiveValue(pa);
            object v;
            if (value != null && value.GetValue(out v))
            {
                return v;
            }
            return OnGetDefaultValue(pa);
        }

        /// <summary>
        /// 内部使用，请勿调用
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual T GetValue<T>(in byte index)
        {
            var pa = propertyInfos[index];
            var value = GetEffectiveValue(pa);
            object v;
            if (value != null && value.GetValue(out v))
            {
                return (T)v;
            }
            return (T)OnGetDefaultValue(pa);
        }

        public virtual PropertyMetadataAttribute GetPropertyMetadata(string propertyName)
        {
            PropertyMetadataAttribute p;
            if (!objInfo.TryGetValue(propertyName, out p))
            {
                return null;
            }
            return p;
        }

        public virtual object GetValue([CallerMemberName] string propertyName = null)
        {
            return GetValue<object>(propertyName);
        }
        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected virtual object OnGetDefaultValue(PropertyMetadataAttribute property)
        {
            return property.DefaultValue;
        }
        /// <summary>
        /// 是否已经设置了本地值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool HasLocalValue(string propertyName)
        {
            EffectiveValue v;
            if (TryGetValue(propertyName, out PropertyMetadataAttribute p, out v))
            {
                if (v.LocalValue.HasValue)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasLocalOrStyleValue(string propertyName, out PropertyMetadataAttribute attribute)
        {
            EffectiveValue v;
            if (TryGetValue(propertyName, out attribute, out v))
            {
                if (v.LocalValue.HasValue || v.HasStyleValue())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventArgs">EventArgs类型要和事件的数据类型对应</param>
        /// <param name="eventName"></param>
        public void RaiseEvent<TEventArgs>(in TEventArgs eventArgs, string eventName)
        {
            //if (eventArgs is RoutedEventArgs routed)
            //{
            //    routed.Sender = this;
            //}
            OnRaiseEvent(eventArgs, eventName);

            if (observers != null && eventArgs is EventArgs args)
            {
                EventObserver<EventArgs, CpfObject> eventObserver = new EventObserver<EventArgs, CpfObject>(eventName, args, this);
                foreach (var observer in observers)
                {
                    observer.OnNext(eventObserver);
                }
            }

            if (commands != null)
            {
                List<Command> list;
                if (commands.commands.TryGetValue(eventName, out list))
                {
                    foreach (var item in list)
                    {
                        if (item.Action == null)
                        {
                            var objs = new List<object>();
                            object v = null;
                            if (item.Target != null)
                            {
                                v = item.Target.Target;
                                objs.Add(v);
                            }
                            //else if (item.Relation != null && this is UIElement)
                            //{
                            //    objs.AddRange(item.Relation.Query(this as UIElement));
                            //}
                            else
                            {
                                v = CommandContext;
                                if (v != null)
                                {
                                    objs.Add(v);
                                }
                            }
                            foreach (var obj in objs)
                            {
                                if (obj == null)
                                {
                                    continue;
                                }
                                object[] ps = new object[item.Params == null ? 0 : item.Params.Length];
                                if (item.Params != null && item.Params.Length > 0)
                                {
                                    //ps = item.Params;
                                    item.Params.CopyTo(ps, 0);
                                    for (int i = 0; i < ps.Length; i++)
                                    {
                                        var p = ps[i];
                                        if (p is CommandParameter)
                                        {
                                            if ((CommandParameter)p == CommandParameter.EventArgs)
                                            {
                                                ps[i] = eventArgs;
                                            }
                                            else if ((CommandParameter)p == CommandParameter.EventSender)
                                            {
                                                ps[i] = this;
                                            }
                                        }
                                    }
                                }
                                //v.GetType().GetMethod(item.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FastInvoke(v, ps);

                                //var m = obj.GetType().GetMethod(item.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                //if (m == null)
                                //{
                                //    throw new Exception("未找到该方法 " + item.MethodName);
                                //}
                                //m.Invoke(obj, ps);
                                obj.Invoke(item.MethodName, ps);
                            }
                        }
                        else
                        {
                            item.Action(this, eventArgs);
                        }
                    }
                }
            }

            var handler = Events[eventName];
            if (handler != null)
            {
                handler.Invoke(this, eventArgs);
            }
        }

        protected virtual void OnRaiseEvent<TEventArgs>(in TEventArgs eventArgs, string eventName)
        {

        }

        PropertyChangedEventHandler propertyChangedEventHandler;
        /// <summary>
        /// 当有属性更改之后发生
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                //Events.AddHandler("INotifyPropertyChanged", value);
                propertyChangedEventHandler = (PropertyChangedEventHandler)Delegate.Combine(propertyChangedEventHandler, value);
                //if (propertyChangedEventHandler == null)
                //{
                //    propertyChangedEventHandler = new WeakEvent();
                //}
                //propertyChangedEventHandler.AddHandler(value);
            }
            remove
            {
                //Events.RemoveHandler("INotifyPropertyChanged", value);
                propertyChangedEventHandler = (PropertyChangedEventHandler)Delegate.Remove(propertyChangedEventHandler, value);
                //if (propertyChangedEventHandler != null)
                //{
                //    propertyChangedEventHandler.RemoveHandler(value);
                //}
            }
        }

        public event EventHandler<CPFPropertyChangedEventArgs> PropertyChanged
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        ///// <summary>
        ///// 当有属性更改的时候发生
        ///// </summary>
        //public event PropertyChangingEventHandler PropertyChanging
        //{
        //    add { AddHandler(value); }
        //    remove { RemoveHandler(value); }
        //}

        WeakEventHandlerList events;
        /// <summary>
        /// 事件列表，用于优化事件订阅内存
        /// </summary>
        [NotCpfProperty]
        protected WeakEventHandlerList Events
        {
            get
            {
                if (events == null)
                {
                    events = new WeakEventHandlerList();
                }
                return events;
            }
        }
        /// <summary>
        /// 为指定的事件添加事件处理程序，并将该处理程序添加到当前元素的处理程序集合中。
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="eventName"></param>
        public void AddHandler(Delegate handler, [CallerMemberName] string eventName = null)
        {
            Events.AddHandler(handler, eventName);
        }
        /// <summary>
        /// 从此元素中删除指定的路由事件处理程序。
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="eventName"></param>
        public void RemoveHandler(Delegate handler, [CallerMemberName] string eventName = null)
        {
            Events.RemoveHandler(handler, eventName);
        }


        ///// <summary>
        ///// 移除处理命令
        ///// </summary>
        ///// <param name="eventName">触发的事件名</param>
        ///// <param name="methodName">方法名</param>
        ///// <param name="obj">属性名</param>
        //public void RemoveCommand(string eventName, string methodName, object obj)
        //{
        //    if (commands != null)
        //    {
        //        List<Command> list;
        //        if (commands.TryGetValue(eventName, out list))
        //        {
        //            var f = list.FirstOrDefault(a => a.MethodName == methodName && a.PropertyName == propertyName);
        //            if (f != null)
        //            {
        //                list.Remove(f);
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 是否包含这个依赖属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual bool HasProperty(string propertyName)
        {
            return objInfo.ContainsKey(propertyName);
        }
        /// <summary>
        /// 获取所有注册了的属性元数据
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<PropertyMetadataAttribute> GetProperties()
        {
            return propertyInfos;
        }

        #region IDisposable Support
        private bool disposedValue = false;
        bool isDisposing;

        [NotCpfProperty]
        public bool IsDisposing
        {
            get { return isDisposing; }
        }

        [NotCpfProperty]
        public bool IsDisposed
        {
            get { return disposedValue; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    //values.Clear();
                    if (bindings != null)
                    {
                        foreach (var item in bindings.binds.Select(a => a.Value).ToArray())
                        {
                            foreach (var i in item.ToArray())
                            {
                                i.UnBind();
                            }
                        }
                    }
                    //foreach (var item in values)
                    //{
                    //    if (item != null)
                    //    {
                    //        item.ClearTriggerValues(this);
                    //    }
                    //}


                    for (int i = 0; i < valueList.Count; i++)
                    {
                        var item = valueList[i];
                        if (item != null)
                        {
                            item.ClearTriggerValues(this);
                        }
                    }

                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~CpfObject()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            isDisposing = true;
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            if (events != null)
            {
                events.Dispose(); events = null;
            }
            propertyChangedEventHandler = null;
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            //valueIndexs.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// 不为null而且没有释放，则返回true
        /// </summary>
        /// <param name="CPFObject"></param>
        public static implicit operator bool(CpfObject CPFObject)
        {
            return CPFObject != null && !CPFObject.IsDisposed;
        }
        /// <summary>
        /// 克隆依赖属性和绑定
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            var obj = Type.GetConstructor(new Type[] { }).FastInvoke() as CpfObject;
            CopyTo(obj, true);
            return obj;
        }
        /// <summary>
        /// 将依赖属性本地值和绑定拷贝到另外个对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cover">是否覆盖已经存在的本地值</param>
        public virtual void CopyTo(CpfObject obj, bool cover = false)
        {
            //foreach (var item in values)
            //{
            //    if (!obj.values.TryGetValue(item.Key, out EffectiveValue value))
            //    {
            //        obj.values.Add(item.Key, item.Value);
            //    }
            //    else
            //    {
            //        if (cover || !value.LocalValue.HasValue)
            //        {
            //            value.LocalValue = item.Value.LocalValue;
            //        }
            //    }
            //}
            if (obj.Type != Type)
            {
                throw new Exception("目标类型不一致");
            }
            if (cover)
            {
                //obj.values = values.ToArray();
                obj.valueIndexs = valueIndexs.ToArray();
                obj.valueList = valueList.Clone();
            }
            else
            {
                //for (int i = 0; i < values.Length; i++)
                //{
                //    if (obj.values[i] == null || !obj.values[i].LocalValue.HasValue)
                //    {
                //        obj.values[i] = values[i];
                //    }
                //}
                for (int i = 0; i < obj.valueIndexs.Length; i++)
                {
                    var ti = obj.valueIndexs[(byte)i] - 1;
                    var si = valueIndexs[(byte)i] - 1;
                    var tv = obj.valueList[ti];
                    var sv = valueList[si];
                    if ((si > -1 && sv != null) && (ti < 0 || tv == null || !tv.LocalValue.HasValue))
                    {
                        if (ti < 0)
                        {
                            obj.valueList.Add(new EffectiveValue { LocalValue = sv.LocalValue });
                            obj.valueIndexs[(byte)i] = obj.valueList.Count;
                        }
                        else if (tv != null)
                        {
                            tv.LocalValue = sv.LocalValue;
                        }
                        else
                        {
                            obj.valueList[ti] = new EffectiveValue { LocalValue = sv.LocalValue };
                        }
                    }
                }
            }

            if (bindings != null)
            {
                //obj.Bindings.binds.Clear();
                foreach (var item in bindings.binds)
                {
                    List<Binding> bindings = new List<Binding>();
                    foreach (var b in item.Value)
                    {
                        bindings.Add(new Binding { BindingMode = b.BindingMode, Convert = b.Convert, ConvertBack = b.ConvertBack, IsDataContext = b.IsDataContext, Owner = obj, SourceElementLayer = b.SourceElementLayer, SourcePropertyName = b.SourcePropertyName, TargetPropertyName = b.TargetPropertyName, Source = b.SourceElementLayer.HasValue ? null : b.Source });
                    }
                    obj.Bindings.binds.Add(item.Key, bindings);
                }
            }

            if (commands != null)
            {
                //obj.Commands.commands.Clear();
                foreach (var item in commands.commands)
                {
                    List<Command> commands = new List<Command>();
                    foreach (var c in item.Value)
                    {
                        commands.Add(new Command
                        {
                            MethodName = c.MethodName,
                            Params = c.Params,
                            //Relation = c.Relation,
                            Target = c.Target,
                            Action = c.Action
                        });
                    }
                    obj.Commands.commands.Add(item.Key, commands);
                }
            }

            if (attachedValues != null)
            {
                obj.attachedValues = new HybridDictionary<string, object>();
                foreach (var item in attachedValues)
                {
                    obj.attachedValues.Add(item);
                }
            }

        }

        public virtual string GetCreationCode()
        {
            var ps = GetHasLocalValueProperties().Select(a => $"{a.Item1} = {a.Item2.GetCreationCode()},").ToArray();
            var c = $"new {type.Name}{{ {string.Join(" ", ps)} }}";
            return c;
        }


        List<IObserver<EventObserver<EventArgs, CpfObject>>> observers;
        /// <summary>
        /// 订阅通知
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<EventObserver<EventArgs, CpfObject>> observer)
        {
            if (observers == null)
            {
                observers = new List<IObserver<EventObserver<EventArgs, CpfObject>>>();
            }
            observers.Add(observer);
            return new Unsubscribe(this.observers, observer);
        }
        /// <summary>
        /// 取消订阅类
        /// </summary>
        class Unsubscribe : IDisposable
        {
            List<IObserver<EventObserver<EventArgs, CpfObject>>> observers;
            IObserver<EventObserver<EventArgs, CpfObject>> observer;
            public Unsubscribe(List<IObserver<EventObserver<EventArgs, CpfObject>>> observers
            , IObserver<EventObserver<EventArgs, CpfObject>> observer)
            {
                this.observer = observer;
                this.observers = observers;
            }

            public void Dispose()
            {
                if (this.observers != null)
                {
                    this.observers.Remove(observer);
                }
            }
        }
    }

    //public class PInfo
    //{

    //    public PropertyInfo PropertyInfo { get; internal set; }

    //    public PropertyMetadataAttribute PropertyMetadata { get; internal set; }
    //}
    /// <summary>
    /// 不使用属性管理
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotCpfProperty : Attribute
    {

    }

    public delegate void EventHandlerRef<TEventArgs>(object sender, in TEventArgs e);

    /// <summary>
    /// 属性元数据，设置默认值，必须显示转换
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PropertyMetadataAttribute : Attribute
    {
        internal PropertyMetadataAttribute() { }
        /// <summary>
        /// 设置默认值，必须显示转换
        /// </summary>
        /// <param name="defaultValue"></param>
        public PropertyMetadataAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// 设置默认值，通过设定的类型和字符串转换
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public PropertyMetadataAttribute(Type type, string value)
        {
            // load an otherwise normal class.
            try
            {
                this.DefaultValue = type.Parse(value);
            }
            catch
            {
                throw new InvalidCastException("Default value attribute of type " + type.FullName + " threw converting from the string '" + value + "'.");
            }
        }
        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; internal set; }
        /// <summary>
        /// 属性值类型
        /// </summary>
        public Type PropertyType { get; internal set; }
        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; internal set; }

        internal bool Compute;

        internal byte Id;
        /// <summary>
        /// 属性通知
        /// </summary>
        internal List<PropertyChangedCallback> actions;
    }
    /// <summary>
    /// UI属性元数据，设置默认值，必须显示转换
    /// </summary>
    public class UIPropertyMetadataAttribute : PropertyMetadataAttribute
    {
        /// <summary>
        /// UI属性元数据
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <param name="inherits">属性值是否继承父级容器</param>
        public UIPropertyMetadataAttribute(object defaultValue, bool inherits) : base(defaultValue)
        {
            //Inherits = inherits;
            UIPropertyMetadataOptions = inherits ? UIPropertyOptions.Inherits : UIPropertyOptions.None;
        }
        /// <summary>
        /// UI属性元数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="inherits"></param>
        public UIPropertyMetadataAttribute(Type type, string value, bool inherits) : base(type, value)
        {
            UIPropertyMetadataOptions = inherits ? UIPropertyOptions.Inherits : UIPropertyOptions.None;
            //Inherits = inherits;
        }
        /// <summary>
        /// UI属性元数据
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <param name="options">属性变化之后的操作，支持位运算组合</param>
        public UIPropertyMetadataAttribute(object defaultValue, UIPropertyOptions options) : base(defaultValue)
        {
            UIPropertyMetadataOptions = options;
            //Inherits = inherits;
            //NeedInvalidate = needInvalidate;
            //NeedLayout = needLayout;
        }
        ///// <summary>
        ///// UI属性元数据
        ///// </summary>
        ///// <param name="type">属性值类型</param>
        ///// <param name="value">默认值</param>
        ///// <param name="inherits">属性值是否继承父级容器</param>
        ///// <param name="needLayout">属性变化是否需要重新布局</param>
        ///// <param name="needInvalidate">属性变化是否需要重新绘制</param>
        //public UIPropertyMetadataAttribute(Type type, string value, bool inherits, bool needLayout, bool needInvalidate) : base(type, value)
        //{
        //    //Inherits = inherits;
        //    //NeedInvalidate = needInvalidate;
        //    //NeedLayout = needLayout;
        //}
        /// <summary>
        /// UI属性元数据
        /// </summary>
        /// <param name="type">属性值类型</param>
        /// <param name="value">默认值</param>
        /// <param name="options">属性变化之后的操作，支持位运算组合</param>
        public UIPropertyMetadataAttribute(Type type, string value, UIPropertyOptions options) : base(type, value)
        {
            UIPropertyMetadataOptions = options;
        }

        private static bool IsFlagSet(UIPropertyOptions flag, UIPropertyOptions flags)
        {
            return (flags & flag) != 0;
        }
        /// <summary>
        /// 属性变化之后的操作
        /// </summary>
        public UIPropertyOptions UIPropertyMetadataOptions { get; }

        /// <summary>
        /// 属性值是否继承父级容器
        /// </summary>
        public bool Inherits { get { return IsFlagSet(UIPropertyOptions.Inherits, UIPropertyMetadataOptions); } }
        /// <summary>
        /// 属性变化是否需要重新布局
        /// </summary>
        public bool AffectsArrange { get { return IsFlagSet(UIPropertyOptions.AffectsArrange, UIPropertyMetadataOptions); } }
        /// <summary>
        /// 重新计算元素尺寸
        /// </summary>
        public bool AffectsMeasure { get { return IsFlagSet(UIPropertyOptions.AffectsMeasure, UIPropertyMetadataOptions); } }
        /// <summary>
        /// 属性变化是否需要重新绘制
        /// </summary>
        public bool AffectsRender { get { return IsFlagSet(UIPropertyOptions.AffectsRender, UIPropertyMetadataOptions); } }

    }


    internal class EffectiveValue
    {
        //public PropertyMetadataAttribute PropertyMetadata;
        public _List<AnimationValue> AnimationValue;
        public EffectiveValueEntry LocalValue;
        public _List<TriggerValue> TriggerValue;
        public _List<SavedStyleValue> styleValues;
        //public EffectiveValueEntry TemplateValue;
        public void SetStyleValue(StyleValue style, object value)
        {
            if (styleValues == null)
            {
                styleValues = new _List<SavedStyleValue>();
            }
            else
            {
                if (styleValues.Any(a => a.StyleValue == style))
                {
                    return;
                }
            }
            styleValues.Add(new SavedStyleValue { StyleValue = style, Value = value });
        }

        public void SetTriggerValue(Trigger Trigger, object value, StyleValue styleValue)
        {
            if (TriggerValue == null)
            {
                TriggerValue = new _List<TriggerValue>();
            }
            else
            {
                for (int i = 0; i < TriggerValue.Count; i++)
                {
                    var item = TriggerValue[i];
                    if (item.Trigger == Trigger)
                    {
                        item.Value = value;
                        item.StyleValue = styleValue;
                        if (i != TriggerValue.Count - 1)
                        {
                            TriggerValue.RemoveAt(i);
                            TriggerValue.Add(item);
                        }
                        return;
                    }
                }

            }
            TriggerValue.Add(new TriggerValue { Trigger = Trigger, Value = value, StyleValue = styleValue });
            //if (StyleValue.Count > 1)
            //{
            //    //StyleValue.Sort(new StyleValueSort());
            //    SortStyleValue();
            //}
        }

        ///// <summary>
        ///// 根据Priority属性排序
        ///// </summary>
        //internal void SortStyleValue()
        //{//选择排序
        //    int min;
        //    for (int i = 0; i < StyleValue.Count; i++)
        //    {
        //        min = i;
        //        for (int j = i + 1; j < StyleValue.Count; j++)
        //        {
        //            if (StyleValue[j].Priority - StyleValue[min].Priority < 0)
        //            {
        //                min = j;
        //            }
        //        }
        //        var temp = StyleValue[i];
        //        StyleValue[i] = StyleValue[min];
        //        StyleValue[min] = temp;
        //    }
        //}

        public bool ClearStyleValue(Style style)
        {
            if (styleValues != null)
            {
                var v = styleValues.FirstOrDefault(a => a.StyleValue.Style == style);
                if (v != null)
                {
                    styleValues.Remove(v);
                    if (styleValues.Count == 0)
                    {
                        styleValues = null;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool ClearStyleValues()
        {
            if (styleValues != null && styleValues.Count > 0)
            {
                styleValues = null;
                return true;
            }
            return false;
        }

        public bool ClearTriggerValue(Trigger Trigger)
        {
            if (TriggerValue != null)
            {
                //var v = TriggerValue.ListFirstOrDefault(a => a.Trigger == Trigger);
                //if (v != null)
                //{
                //    TriggerValue.Remove(v);
                //    return true;
                //}
                bool r = false;
                for (int i = TriggerValue.Count - 1; i >= 0; i--)
                {
                    if (TriggerValue[i].Trigger == Trigger)
                    {
                        TriggerValue.RemoveAt(i);
                        r = true;
                    }
                }
                return r;
            }
            return false;
        }

        public void ClearTriggerValues(CpfObject owner)
        {
            if (TriggerValue != null)
            {
                for (int i = TriggerValue.Count - 1; i >= 0; i--)
                {
                    var t = TriggerValue[i].Trigger;
                    if (t != null)
                    {
                        if (t.SetPropertys != null)
                        {
                            t.SetPropertys.Remove(owner);
                        }
                    }
                }
                TriggerValue.Clear();
            }
        }

        public void SetAnimationValue(Storyboard Storyboard, object Value)
        {
            if (AnimationValue == null)
            {
                AnimationValue = new _List<AnimationValue>();
            }
            var v = AnimationValue.FirstOrDefault(a => a.Storyboard == Storyboard);
            if (v != null)
            {
                v.Value = Value;
            }
            else
            {
                AnimationValue.Add(new AnimationValue { Storyboard = Storyboard, Value = Value });
            }
        }

        public bool ClearAnimationValue(Storyboard Storyboard)
        {
            if (AnimationValue != null)
            {
                var v = AnimationValue.FirstOrDefault(a => a.Storyboard == Storyboard);
                if (v != null)
                {
                    AnimationValue.Remove(v);
                    return true;
                }
            }
            return false;
        }

        public bool HasStyleValue()
        {
            if (AnimationValue != null && AnimationValue.Count > 0)
            {
                return true;
            }
            if (TriggerValue != null && TriggerValue.Count > 0)
            {
                return true;
            }
            if (styleValues != null && styleValues.Count > 0)
            {
                return true;
            }
            return false;
        }

        public bool HasStyle(Style style)
        {
            if (styleValues != null)
            {
                return styleValues.Any(a => a.StyleValue.Style == style);
            }
            return false;
        }

        public bool GetLocalValue(out object value)
        {
            if (LocalValue.HasValue)
            {
                value = LocalValue.Value;
                return true;
            }
            value = null;
            return false;
        }

        public bool GetValue(out object value)
        {
            if (AnimationValue != null && AnimationValue.Count > 0)
            {
                value = AnimationValue[AnimationValue.Count - 1].Value;
                return true;
            }
            if (TriggerValue != null && TriggerValue.Count > 0)
            {
                value = TriggerValue[TriggerValue.Count - 1].Value;
                return true;
            }
            if (styleValues != null && styleValues.Count > 0)
            {
                value = styleValues[styleValues.Count - 1].Value;
                return true;
            }
            if (LocalValue.HasValue)
            {
                value = LocalValue.Value;
                return true;
            }
            //if (TemplateValue.HasValue)
            //{
            //    value = TemplateValue.Value;
            //    return true;
            //}
            value = null;
            return false;
        }
    }
    class AnimationValue
    {
        public Storyboard Storyboard;
        public object Value;
    }
    class TriggerValue
    {
        public Trigger Trigger;

        public StyleValue StyleValue;
        //public int Priority;

        public object Value;
    }

    class SavedStyleValue
    {
        public StyleValue StyleValue;

        //public int Priority;

        public object Value;
    }

    internal struct EffectiveValueEntry
    {
        //internal int PropertyIndex { get; set; }
        internal bool HasValue;

        internal object Value;
    }
    /// <summary>
    /// 指示是否被释放了
    /// </summary>
    public interface IDisposed : IDisposable
    {
        bool IsDisposed { get; }
    }

    //class StyleValueSort : IComparer<StyleValue>
    //{
    //    public int Compare(StyleValue x, StyleValue y)
    //    {
    //        return x.Priority - y.Priority;
    //    }
    //}
    /// <summary>
    /// 属性元数据重写
    /// </summary>
    public class OverrideMetadata
    {
        internal Dictionary<string, PropertyMetadataAttribute> list = new Dictionary<string, PropertyMetadataAttribute>();
        /// <summary>
        /// 属性元数据重写
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyMetadata"></param>
        public void Override(string propertyName, PropertyMetadataAttribute propertyMetadata)
        {
            if (list.ContainsKey(propertyName))
            {
                list.Remove(propertyName);
            }
            list.Add(propertyName, propertyMetadata);
        }
    }

    public class ComputeProtertyInfo
    {
        public PropertyInfo Property { get; set; }

        public string[] NoticeProperties { get; set; }

        public int[] Tokens { get; set; }
    }

    /// <summary>
    /// 属性变化之后的操作
    /// </summary>
    [Flags]
    public enum UIPropertyOptions : byte
    {
        /// <summary>No flags</summary>
        None = 0x000,

        /// <summary>This property affects measurement</summary>
        AffectsMeasure = 0x001,

        /// <summary>This property affects arragement</summary>
        AffectsArrange = 0x002,

        ///// <summary>This property affects parent's measurement</summary>
        //AffectsParentMeasure = 0x004,

        ///// <summary>This property affects parent's arrangement</summary>
        //AffectsParentArrange = 0x008,

        /// <summary>This property affects rendering</summary>
        AffectsRender = 0x010,

        /// <summary>This property inherits to children</summary>
        Inherits = 0x020,

        ///// <summary>
        ///// This property causes inheritance and resource lookup to override values 
        ///// of InheritanceBehavior that may be set on any FE in the path of lookup
        ///// </summary>
        //OverridesInheritanceBehavior = 0x040,

        ///// <summary>This property does not support data binding</summary>
        //NotDataBindable = 0x080,

        ///// <summary>Data bindings on this property default to two-way</summary>
        //BindsTwoWayByDefault = 0x100,

        ///// <summary>This property should be saved/restored when journaling/navigating by URI</summary>
        //Journal = 0x400,

        ///// <summary>
        /////     This property's subproperties do not affect rendering.
        /////     For instance, a property X may have a subproperty Y.
        /////     Changing X.Y does not require rendering to be updated.
        ///// </summary>
        //SubPropertiesDoNotAffectRender = 0x800,
    }

    /// <summary>
    /// 获取或者设置附加属性值
    /// </summary>
    /// <typeparam name="Value"></typeparam>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate Value Attached<Value>(CpfObject obj, OptionalParameter<Value> value = default);
    /// <summary>
    /// CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="propertyName"></param>
    /// <param name="defaultValue"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public delegate void AttachedPropertyChanged(CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue);
    /// <summary>
    /// 属性通知回调
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="newValue"></param>
    /// <param name="oldValue"></param>
    /// <param name="attribute"></param>
    delegate void PropertyChangedCallback(CpfObject obj, object newValue, object oldValue, PropertyMetadataAttribute attribute);
    ///// <summary>
    ///// 属性通知注册器
    ///// </summary>
    //public class PropertyChangedRegister
    //{
    //    /// <summary>
    //    /// 注册属性通知
    //    /// </summary>
    //    /// <param name="propertyName"></param>
    //    /// <param name="propertyChangedCallback">CpfObject obj, string propertyName, object newValue, object oldValue, PropertyMetadataAttribute attribute</param>
    //    public void Register(string propertyName, PropertyChangedCallback propertyChangedCallback)
    //    {
    //        if (!propertyChangedCallback.Method.IsStatic)
    //        {
    //            throw new Exception("注册的通知回调必须是静态函数");
    //        }
    //        if (callbacks == null)
    //        {
    //            callbacks = new List<(string, PropertyChangedCallback)>();
    //        }
    //        callbacks.Add((propertyName, propertyChangedCallback));
    //    }

    //    internal List<ValueTuple<string, PropertyChangedCallback>> callbacks;
    //}

    /// <summary>
    /// 定义该方法为属性通知方法，支持绑定多个，方法类型 void Method(object newValue, object oldValue, CPF.PropertyMetadataAttribute attribute)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PropertyChangedAttribute : Attribute
    {
        /// <summary>
        /// 定义该方法为属性通知方法，方法类型 void Method(object newValue, object oldValue, CPF.PropertyMetadataAttribute attribute)
        /// </summary>
        /// <param name="propertyName">属性名</param>
        public PropertyChangedAttribute(string propertyName)
        {
            PropertyName = propertyName;
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new Exception("propertyName不能为空");
            }
        }
        /// <summary>
        /// 通知的属性名
        /// </summary>
        public string PropertyName { get; set; }
    }
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    class _List<T>
    {
        public _List()
        {
            _items = _emptyArray;
        }

        private T[] _items;
        public byte Count;
        private byte maxSize = 255;
        //public byte MaxSize
        //{
        //    get { return maxSize; }
        //    set { maxSize = value; }
        //}

        //public byte Count
        //{
        //    get { return Count; }
        //}

        public T this[in int index]
        {
            get
            {
                // Following trick can reduce the range check by one
                //if ((uint)index >= Count)
                //{
                //    throw new Exception("超出范围" + index);
                //}
                return _items[index];
            }

            set
            {
                //if ((uint)index >= Count)
                //{
                //    throw new Exception("超出范围" + index);
                //}
                _items[index] = value;
            }
        }

        public void Add(T item)
        {
            if (Count == _items.Length)
            {
                EnsureCapacity(Count + 2);
            }
            _items[Count] = item;
            Count++;
        }

        public void Clear()
        {
            if (Count > 0)
            {
                Array.Clear(_items, 0, Count); // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
                Count = 0;
            }
        }
        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, Count);
        }
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if ((uint)index >= Count)
            {
                throw new Exception("超出范围" + index);
            }
            Count--;
            if (index < Count)
            {
                Array.Copy(_items, index + 1, _items, index, Count - index);
            }
            _items[Count] = default(T);
        }
        static readonly T[] _emptyArray = new T[0];
        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int value = _items.Length == 0 ? 1 : _items.Length + 2;
                if (min <= maxSize && value > maxSize)
                {
                    value = maxSize;
                }
                else if (min > maxSize)
                {
                    throw new Exception("超过容量" + maxSize);
                }
                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (Count > 0)
                        {
                            Array.Copy(_items, 0, newItems, 0, Count);
                        }
                        _items = newItems;
                    }
                    else
                    {
                        _items = _emptyArray;
                    }
                }
            }
        }

        public T FirstOrDefault(Func<T, bool> func)
        {
            for (int i = 0; i < Count; i++)
            {
                if (func(_items[i]))
                {
                    return _items[i];
                }
            }
            return default;
        }

        public bool Any(Func<T, bool> func)
        {
            for (int i = 0; i < Count; i++)
            {
                if (func(_items[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public _List<T> Clone()
        {
            var list = new _List<T>();
            list.Count = Count;
            list.maxSize = maxSize;
            list._items = new T[Count];
            if (Count > 0)
            {
                Array.Copy(_items, 0, list._items, 0, Count);
            }
            return list;
        }

        public void Sort(IComparer<T> comparison)
        {
            Array.Sort(_items, 0, Count, comparison);
        }
    }

    //public unsafe class ByteArray : IDisposable, IEnumerable
    //{
    //    public byte Length;
    //    IntPtr intPtr;

    //    public byte this[in byte index]
    //    {
    //        get
    //        {
    //            if (index > Length)
    //            {
    //                throw new IndexOutOfRangeException("ByteArray访问超索引");
    //            }
    //            if (intPtr == IntPtr.Zero)
    //            {
    //                return 0;
    //            }
    //            return ((byte*)intPtr)[index];
    //        }
    //        set
    //        {
    //            if (index > Length)
    //            {
    //                throw new IndexOutOfRangeException("ByteArray访问超索引");
    //            }
    //            ((byte*)intPtr)[index] = value;
    //        }
    //    }

    //    public ByteArray(byte len)
    //    {
    //        Length = len;
    //        intPtr = Marshal.AllocHGlobal(len);
    //        var s = (byte*)intPtr;
    //        for (int i = 0; i < Length; i++)
    //        {
    //            s[i] = 0;
    //        }
    //    }

    //    ByteArray()
    //    {

    //    }

    //    public ByteArray Clone()
    //    {
    //        var by = new ByteArray();
    //        by.Length = Length;
    //        by.intPtr = Marshal.AllocHGlobal(Length);
    //        var t = (byte*)by.intPtr;
    //        var s = (byte*)intPtr;
    //        for (int i = 0; i < Length; i++)
    //        {
    //            t[i] = s[i];
    //        }
    //        return by;
    //    }

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (intPtr != IntPtr.Zero)
    //        {
    //            Marshal.FreeHGlobal((IntPtr)intPtr);
    //            intPtr = IntPtr.Zero;
    //        }
    //    }

    //    // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    //    ~ByteArray()
    //    {
    //        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //        Dispose(disposing: false);
    //    }

    //    public void Dispose()
    //    {
    //        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //        Dispose(disposing: true);
    //        GC.SuppressFinalize(this);
    //    }

    //    public IEnumerator GetEnumerator()
    //    {
    //        return new ByteArrayIEnumerator(this);
    //    }
    //}

    //class ByteArrayIEnumerator : IEnumerator
    //{
    //    ByteArray byteArray;
    //    short position = -1;
    //    public ByteArrayIEnumerator(ByteArray byteArray)
    //    {
    //        this.byteArray = byteArray;
    //    }
    //    public object Current
    //    {
    //        get
    //        {
    //            try
    //            {
    //                return byteArray[(byte)position];
    //            }
    //            catch (IndexOutOfRangeException)
    //            {
    //                throw new InvalidOperationException();
    //            }
    //        }
    //    }

    //    public bool MoveNext()
    //    {
    //        position++;
    //        return (position < byteArray.Length);
    //    }

    //    public void Reset()
    //    {
    //        position = -1;
    //    }
    //}

}
