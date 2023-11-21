using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using CPF.Reflection;
using System.Timers;
using CPF.Controls;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
using System.Threading;

namespace CPF
{
    /// <summary>
    /// 绑定
    /// </summary>
    public class Binding : IDisposable
    {
        static ConcurrentDictionary<WeakNode, HashSet<WeakDelegate>> notifys = new ConcurrentDictionary<WeakNode, HashSet<WeakDelegate>>();
        static Thread GCTimer;
        //internal static bool runGC = true;
        /// <summary>
        /// 事件弱绑定
        /// </summary>
        /// <param name="notify"></param>
        /// <param name="propertyChanged">不能是静态方法</param>
        public static void RegisterPropertyChanged(INotifyPropertyChanged notify, PropertyChangedEventHandler propertyChanged)
        {
            if (propertyChanged.Method.IsStatic)
            {
                throw new Exception("不能是静态方法，必须关联对象");
            }
            if (GCTimer == null)
            {
                GCTimer = new Thread(GCTimer_Elapsed);
                GCTimer.IsBackground = true;
                GCTimer.Name = "清理无效绑定";
                GCTimer.Start();
            }
            if (!notifys.TryGetValue(new WeakNode(notify), out HashSet<WeakDelegate> list))
            {
                notify.PropertyChanged += Notify_PropertyChanged;
                list = new HashSet<WeakDelegate>();
                notifys.TryAdd(new WeakNode(notify), list);
            }
            list.Add(new WeakDelegate(propertyChanged.Target, propertyChanged.Method));
        }

        private static void GCTimer_Elapsed()
        {
            while (true)
            {
                Thread.Sleep(5000);
                List<WeakNode> remove = new List<WeakNode>();
                foreach (var item in notifys)
                {
                    //if(!item.Key.reference.IsAlive)
                    if (!item.Key.reference.TryGetTarget(out INotifyPropertyChanged notifyProperty) || (notifyProperty is IDisposed cpf && cpf.IsDisposed))
                    {
                        remove.Add(item.Key);
                    }
                }
                if (remove.Count > 0)
                {
                    foreach (var item in remove)
                    {
                        notifys.TryRemove(item, out _);
                    }
                    GC.Collect();
                }
            }
        }

        private static void Notify_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (notifys.TryGetValue(new WeakNode((INotifyPropertyChanged)sender), out HashSet<WeakDelegate> list))
            {
                List<WeakDelegate> ids = new List<WeakDelegate>();
                List<WeakDelegate> invokes = new List<WeakDelegate>();
                foreach (var item in list)
                {
                    item.reference.TryGetTarget(out var t);
                    if (t == null || (t is IDisposed cpf && cpf.IsDisposed))
                    {
                        ids.Add(item);
                    }
                    else
                    {
                        //item.method.FastInvoke(t, sender, e);
                        invokes.Add(item);
                    }
                }
                foreach (var item in invokes)
                {
                    item.reference.TryGetTarget(out var t);
                    if (t == null)
                    {
                        ids.Add(item);
                    }
                    else
                    {
                        item.method.FastInvoke(t, sender, e);
                    }
                }
                for (int i = 0; i < ids.Count; i++)
                {
                    list.Remove(ids[i]);
                }
            }
        }
        /// <summary>
        /// 移除弱事件绑定
        /// </summary>
        /// <param name="notify"></param>
        /// <param name="propertyChanged"></param>
        public static void CancellationPropertyChanged(INotifyPropertyChanged notify, PropertyChangedEventHandler propertyChanged)
        {
            var node = new WeakNode(notify);
            if (notifys.TryGetValue(node, out HashSet<WeakDelegate> list))
            {
                List<WeakDelegate> ids = new List<WeakDelegate>();
                var wd = new WeakDelegate(propertyChanged.Target, propertyChanged.Method);
                if (list.Contains(wd))
                {
                    ids.Add(wd);
                }
                //foreach (var item in list)
                //{
                //    //var t = item.reference.Target;
                //    //if (t == null || (propertyChanged.Target == t && propertyChanged.Method == item.method))
                //    if (!item.reference.IsAlive)
                //    {
                //        ids.Add(item);
                //    }
                //}
                if (ids.Count == list.Count)
                {
                    list.Clear();
                }
                else
                {
                    for (int i = 0; i < ids.Count; i++)
                    {
                        list.Remove(ids[i]);
                    }
                }
                if (list.Count == 0)
                {
                    notifys.TryRemove(node, out _);
                    notify.PropertyChanged -= Notify_PropertyChanged;
                }
            }
        }

        struct WeakNode
        {
            public WeakNode(INotifyPropertyChanged reference)
            {
                this.reference = new WeakReference<INotifyPropertyChanged>(reference);
                hash = reference.GetHashCode();
            }
            public WeakReference<INotifyPropertyChanged> reference;

            public override bool Equals(object obj)
            {
                if (obj is WeakNode n)
                {
                    n.reference.TryGetTarget(out INotifyPropertyChanged t1);
                    reference.TryGetTarget(out INotifyPropertyChanged t2);
                    if (t1 == null)
                    {
                        return t1 == t2;
                    }
                    return t1.Equals(t2);
                }
                return false;
            }

            readonly int hash;

            public override int GetHashCode()
            {
                //if (hash == 0 && reference.TryGetTarget(out INotifyPropertyChanged t))
                //{
                //    hash = t.GetHashCode();
                //    return hash;
                //}
                return hash;
            }
        }

        CpfObject owner;
        /// <summary>
        /// Target对象
        /// </summary>
        public CpfObject Owner
        {
            get { return owner; }
            internal set
            {
                //if (value == null && !unbind)
                //{
                //    throw new Exception("错误");
                //}
                owner = value;
            }
        }
        /// <summary>
        /// 数据源对象
        /// </summary>
        public WeakReference Source { get; internal set; }
        internal Binding(object sObj, string sourcePropertyName, string targetPropertyName, BindingMode bindingMode)
        {
            if (sObj != null)
            {
                Source = new WeakReference(sObj);
                IsDataContext = false;
            }
            TargetPropertyName = targetPropertyName;
            SourcePropertyName = sourcePropertyName;
            BindingMode = bindingMode;
        }
        internal Binding() { }

        internal bool IsDataContext = true;
        /// <summary>
        /// 数据源字段名
        /// </summary>
        public string SourcePropertyName
        {
            get;
            internal set;
        }
        /// <summary>
        /// Owner被绑定的属性名
        /// </summary>
        public string TargetPropertyName
        {
            get;
            internal set;
        }
        /// <summary>
        /// SourceToTarget异常回调
        /// </summary>
        public Action<Binding, object, Exception> SourceToTargetError { get; set; }
        /// <summary>
        /// TargetToSource异常回调
        /// </summary>
        public Action<Binding, object, Exception> TargetToSourceError { get; set; }

        /// <summary>
        /// 数据绑定的转换
        /// </summary>
        public Func<object, object> Convert { get; set; }
        /// <summary>
        /// 数据绑定的转换
        /// </summary>
        public Func<object, object> ConvertBack { get; set; }
        /// <summary>
        /// 绑定模式
        /// </summary>
        public BindingMode BindingMode
        {
            get;
            internal set;
        }
        //bool unbind;
        /// <summary>
        /// 取消数据绑定
        /// </summary>
        public void UnBind()
        {
            //unbind = true;
            if (Owner != null)
            {
                List<Binding> list;
                if (Owner.Bindings.binds.TryGetValue(TargetPropertyName, out list))
                {
                    list.Remove(this);
                }
                if (list != null && list.Count == 0)
                {
                    Owner.Bindings.binds.Remove(TargetPropertyName);
                }
            }
            if (Source != null)
            {
                if (Source.IsAlive)
                {
                    INotifyPropertyChanged n = Source.Target as INotifyPropertyChanged;
                    if (n != null)
                    {
                        //n.PropertyChanged -= PropertyChanged;
                        CancellationPropertyChanged(n);
                    }
                }
            }
            Source = null;
            Owner = null;
            //unbind = false;
        }
        ///// <summary>
        ///// 绑定是否有效
        ///// </summary>
        //public bool IsValid
        //{
        //    get; internal set;
        //}

        static Stack<Binding> current = new Stack<Binding>();
        /// <summary>
        /// 当前执行的绑定对象
        /// </summary>
        public static Binding Current
        {
            get
            {
                if (current.Count == 0)
                {
                    return null;
                }
                return current.Peek();
            }
        }
        bool cancel = false;
        /// <summary>
        /// 取消这次的数据传递
        /// </summary>
        public void Cancel()
        {
            cancel = true;
        }
        /// <summary>
        /// 是否是双向绑定的时候回传状态，一般在转换器里使用，在双向绑定的时候，假如label1和label2的text双向绑定了 label1.Text="1";那label1的Text会传给label2的Text，但是这个同时label2的Text也会因为绑定的缘故往label1传Text值，这个时候IsPostBack为true，你可以判断是否要Cancel。假如回传的label2的Text和label1的Text值不同或者转换器转换到的结果不匹配那可能会出现死循环或者其他错误
        /// </summary>
        public bool IsPostBack
        {
            get { return current.Count == 2 && current.ElementAt(1) == this && current.Peek() == this; }
        }

        bool isSourceToTarget = false;
        /// <summary>
        /// 执行数据传递
        /// </summary>
        public virtual void SourceToTarget()
        {
            isSourceToTarget = true;
            object value = null;
            current.Push(this);
            try
            {
                if (Source == null || !Source.IsAlive)
                {
                    if (Owner.HasProperty(TargetPropertyName))
                    {
                        Owner.ClearLocalValue(TargetPropertyName);
                    }
                    else
                    {
                        var type = GetSourcePropertyType();
                        Owner.SetPropretyValue(TargetPropertyName, type.IsValueType ? Activator.CreateInstance(type) : null);
                    }
                    return;
                }
                //CpfObject s = Source.Target as CpfObject;
                //if (s == null)
                //{
                //    var p = Source.Target.GetType().GetProperty(SourcePropertyName);
                //    if (p == null)
                //    {
                //        throw new Exception("未找到" + Source.Target + "的属性：" + SourcePropertyName);
                //    }
                //    value = p.FastGetValue(Source.Target);
                //}
                //else
                //{
                //    if (s.HasProperty(SourcePropertyName))
                //    {
                //        value = s.GetValue(SourcePropertyName);
                //    }
                //    else
                //    {
                //        var p = s.Type.GetProperty(SourcePropertyName);
                //        value = p.FastGetValue(Source.Target);
                //    }
                //}
                value = Source.Target.GetPropretyValue(SourcePropertyName);
                if (Convert != null)
                {
                    value = Convert(value);
                }
                if (!cancel && !Owner.SetValue(value, TargetPropertyName))
                {
                    //Owner.Type.GetProperty(TargetPropertyName).FastSetValue(Owner, value);
                    Owner.SetValue(TargetPropertyName, value);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"绑定数据执行数据传递时出错：Source:{Source.Target},Property:{SourcePropertyName} Target:{Owner},Property:{TargetPropertyName}----{e}");
                if (SourceToTargetError != null)
                {
                    SourceToTargetError(this, value, e);
                }
                else
                {
                    throw new Exception($"绑定数据执行数据传递时出错：Source:{Source.Target},Property:{SourcePropertyName} Target:{Owner},Property:{TargetPropertyName}", e);
                }
            }
            finally
            {
                cancel = false;
                current.Pop();
                isSourceToTarget = false;
            }
        }
        /// <summary>
        /// 执行数据传递
        /// </summary>
        public virtual void TargetToSource()
        {
            if (!isSourceToTarget)
            {
                if (Source == null || !Source.IsAlive)
                {
                    return;
                }
                current.Push(this);
                object nv = null;
                try
                {
                    //if (Owner.HasProperty(TargetPropertyName))
                    //{
                    //    nv = Owner.GetValue(TargetPropertyName);
                    //}
                    //else
                    //{
                    //    nv = Owner.Type.GetProperty(TargetPropertyName).FastGetValue(Owner);
                    //}
                    nv = Owner.GetPropretyValue(TargetPropertyName);
                    if (ConvertBack != null)
                    {
                        nv = ConvertBack(nv);
                    }
                    var b = Source.Target as CpfObject;
                    if (!cancel)
                    {
                        if (b != null)
                        {
                            if (!b.SetValue(nv, SourcePropertyName))
                            {
                                //b.Type.GetProperty(SourcePropertyName).FastSetValue(b, nv);
                                b.SetValue(SourcePropertyName, nv);
                            }
                        }
                        else
                        {
                            //var t = Source.Target.GetType();
                            //var p = t.GetProperty(SourcePropertyName);
                            //if (p == null)
                            //{
                            //    throw new Exception("未找到属性：" + SourcePropertyName);
                            //}
                            //p.FastSetValue(Source.Target, nv);
                            Source.Target.SetValue(SourcePropertyName, nv);
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"绑定数据执行数据传递时出错：Source:{Source.Target},Property:{SourcePropertyName} Target:{Owner},Property:{TargetPropertyName}----{e}");
                    if (TargetToSourceError != null)
                    {
                        TargetToSourceError(this, nv, e);
                    }
                    else
                    {
                        throw new Exception($"绑定数据执行数据传递时出错：Source:{Source.Target},Property:{SourcePropertyName} Target:{Owner},Property:{TargetPropertyName}", e);
                    }
                }
                finally
                {
                    cancel = false;
                    current.Pop();
                }
            }
        }

        Type sourcePropertyType;
        public Type GetSourcePropertyType()
        {
            if (sourcePropertyType == null)
            {
                var b = Source.Target as CpfObject;
                if (b != null)
                {
                    sourcePropertyType = b.GetPropertyMetadata(SourcePropertyName).PropertyType;
                }
                else
                {
                    var t = Source.Target.GetType();
                    var p = t.GetProperty(SourcePropertyName);
                    if (p == null)
                    {
                        if (t.IsValueType)
                        {
                            throw new Exception(t + "未找到属性:" + SourcePropertyName + " ---- 结构体作为数据源只能只读，不能将值传递回去");
                        }
                        else
                        {
                            throw new Exception(t + "未找到属性:" + SourcePropertyName);
                        }
                    }
                    sourcePropertyType = p.PropertyType;
                }
            }
            return sourcePropertyType;
        }

        Type targetPropertyType;
        public Type GetTargetPropertyType()
        {
            if (targetPropertyType == null)
            {
                var t = Owner.GetType();
                targetPropertyType = t.GetProperty(TargetPropertyName).PropertyType;
            }
            return targetPropertyType;
        }
        void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SourcePropertyName == e.PropertyName)
            {
                //CPFObject s = sender as CPFObject;
                //object value;
                //if (s == null)
                //{
                //    var p = sender.GetType().GetProperty(e.PropertyName);
                //    value = p.FastGetValue(sender);
                //}
                //else
                //{
                //    value = s.GetValue(e.PropertyName);
                //}
                //if (Convert != null)
                //{
                //    value = Convert(value);
                //}
                //Owner.SetValue(value, TargetPropertyName);
                if (BindingMode == BindingMode.OneWay || BindingMode == BindingMode.TwoWay)
                {
                    SourceToTarget();
                }
            }
        }

        internal void RegisterPropertyChanged(INotifyPropertyChanged notify)
        {
            //if (owner == null)
            //{
            //    throw new Exception("错误");
            //}
            RegisterPropertyChanged(notify, PropertyChanged);
        }
        internal void CancellationPropertyChanged(INotifyPropertyChanged notify)
        {
            CancellationPropertyChanged(notify, PropertyChanged);
        }

        /// <summary>
        /// 绑定的UIElement层次，0是自己，1是Parent，2是Parent.Parent....
        /// </summary>
        public byte? SourceElementLayer { get; internal set; }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                UnBind();
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~Binding()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    /// <summary>
    /// This enum describes how the data flows through a given Binding
    /// </summary>
    public enum BindingMode : byte
    {
        /// <summary> Data flows from source to target and vice-versa </summary>
        TwoWay,
        /// <summary> Data flows from source to target, source changes cause data flow </summary>
        OneWay,
        /// <summary> Data flows from source to target once, source changes are ignored 数据从源流到目标一次, 源更改将被忽略</summary>
        OneTime,
        /// <summary> Data flows from target to source, target changes cause data flow </summary>
        OneWayToSource
    }


    //public abstract class LambdaBinding
    //{
    //    protected Delegate Action;
    //    protected string NotifyProperty;
    //    protected bool IsSource;
    //    internal protected LambdaBinding Binding;
    //    internal protected INotifyPropertyChanged Source;
    //}

    ///// <summary>
    ///// Lambda方式绑定，不能使用过于复杂的表达式
    ///// </summary>
    ///// <typeparam name="T">目标类型Owner</typeparam>
    ///// <typeparam name="S">数据源类型</typeparam>
    //public class Binding<T, S> : LambdaBinding where T : CpfObject where S : INotifyPropertyChanged
    //{
    //    /// <summary>
    //    /// 数据源是DataContext，Lambda方式绑定，不能使用过于复杂的表达式
    //    /// </summary>
    //    /// <param name="action"></param>
    //    /// <returns></returns>
    //    public static Binding<T, S> ToTarget(Action<T, S> action)
    //    {
    //        PropertyInfo[] propertyInfos = action.GetMethodInfo().ResolveImpProperties(1);
    //        return new Binding<T, S> { Action = action, NotifyProperty = propertyInfos[0].Name, IsSource = true };
    //    }
    //    /// <summary>
    //    /// 数据源是DataContext，数据往DataContext传递，Lambda方式绑定，不能使用过于复杂的表达式
    //    /// </summary>
    //    /// <param name="action"></param>
    //    /// <returns></returns>
    //    public static Binding<T, S> ToSource(Action<T, S> action)
    //    {
    //        PropertyInfo[] propertyInfos = action.GetMethodInfo().ResolveImpProperties(1);
    //        return new Binding<T, S> { Action = action, NotifyProperty = propertyInfos[0].Name };
    //    }
    //    /// <summary>
    //    /// Lambda方式绑定，不能使用过于复杂的表达式
    //    /// </summary>
    //    /// <param name="source"></param>
    //    /// <param name="action"></param>
    //    /// <returns></returns>
    //    public static Binding<T, S> ToTarget(S source, Action<T, S> action)
    //    {
    //        PropertyInfo[] propertyInfos = action.GetMethodInfo().ResolveImpProperties(1);
    //        return new Binding<T, S> { Action = action, NotifyProperty = propertyInfos[0].Name, IsSource = true, Source = source };
    //    }
    //    /// <summary>
    //    /// Lambda方式绑定，不能使用过于复杂的表达式
    //    /// </summary>
    //    /// <param name="source"></param>
    //    /// <param name="action"></param>
    //    /// <returns></returns>
    //    public static Binding<T, S> ToSource(S source, Action<T, S> action)
    //    {
    //        PropertyInfo[] propertyInfos = action.GetMethodInfo().ResolveImpProperties(1);
    //        return new Binding<T, S> { Action = action, NotifyProperty = propertyInfos[0].Name, Source = source };
    //    }

    //}

    public static class BindHelper
    {
        //public static Binding<T, S> ToSource<T, S>(this Binding<T, S> binding, Action<T, S> action) where T : CpfObject where S : INotifyPropertyChanged
        //{
        //    var b = Binding<T, S>.ToSource(action);
        //    b.Binding = binding;
        //    b.Source = binding.Source;
        //    return b;
        //}
        //public static Binding<T, S> ToTarget<T, S>(this Binding<T, S> binding, Action<T, S> action) where T : CpfObject where S : INotifyPropertyChanged
        //{
        //    var b = Binding<T, S>.ToTarget(action);
        //    b.Binding = binding;
        //    b.Source = binding.Source;
        //    return b;
        //}
        //public static T Bind<T, S>(this T obj, S source, Expression<Func<T, object>> action, Expression<Func<S, object>> action1, BindingMode bindingMode)
        //{
        //    CpfObject target=null;
        //    CpfObject s = null;
        //    target.Bind(s, a => a.Attacheds, a => a.Type.Name, BindingMode.OneWay);

        //    //PropertyInfo[] propertyInfos = action.Method.ResolveImpProperties(1);
        //    //Expression<Action<T, S>> func = Expression.Lambda(;
        //    return obj;
        //}

        //public static T Bind<T, S>(this T obj, Expression<Func<T, object>> action, Expression<Func<S, object>> action1, BindingMode bindingMode) where T:CpfObject
        //{
        //    CpfObject target=null;
        //    target.Bind<CpfObject,CpfObject>(a => a.Attacheds, a => a.Type.Name, BindingMode.OneWay);

        //    return obj;
        //}


        //public static T Trigger<T>(this T obj, Expression<Func<T, bool>> action, Relation relation, params (string, object)[] ps) where T : CpfObject
        //{

        //    return obj;
        //}
        /// <summary>
        /// 设置附加属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="obj"></param>
        /// <param name="attached"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Attached<T, V>(this T obj, Attached<V> attached, V value) where T : CpfObject
        {
            attached(obj, value);
            return obj;
        }

        static bool IsCpfObjectGet(MethodBase source)
        {
            var declaringType = source.DeclaringType;
            if (typeof(CpfObject).IsAssignableFrom(declaringType))
            {
                byte[] ILdata = source.GetMethodBody().GetILAsByteArray();
                var reader = new BytesReader(ILdata);
                while (reader.ReadOpCode(out var opCode))
                {
                    switch (opCode.OperandType)
                    {
                        case OperandType.InlineMethod:
                            if (reader.ReadInt32(out var token))
                            {
                                var b = source.Module.ResolveMethod(token);
                                if (b.Name == nameof(CpfObject.GetValue))
                                    return true;
                            }
                            break;
                        default:
                            reader.Skip(GetOperandSize(opCode.OperandType));
                            break;
                    }
                }
            }
            return false;
        }
        static bool FindPropertyInfo(this MethodBase methodBase, out PropertyInfo value)
        {
            value = null;
            foreach (var propertyInfo in methodBase.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public))
            {
                if (propertyInfo.GetGetMethod().MetadataToken == methodBase.MetadataToken)
                {
                    value = propertyInfo;
                    break;
                }

            }
            return value != null;
        }
        static PropertyInfo[] ResolveImpProperties(this MethodBase source, int deep = 1)
        {
            List<PropertyInfo> tp = new List<PropertyInfo>();
            List<MethodBase> tm = new List<MethodBase>();
            MethodBody methodBody = source.GetMethodBody();
            byte[] ILdata = methodBody.GetILAsByteArray();
            var reader = new BytesReader(ILdata);
            Type currentType = null;

            void readMethod()
            {
                if (reader.ReadInt32(out var token))
                {
                    var b = source.Module.ResolveMethod(token);
                    if (IsCpfObjectGet(b) && b.FindPropertyInfo(out var propertyInfo))
                        tp.Add(propertyInfo);
                }
            }

            //void readType()
            //{
            //    if (reader.ReadInt32(out var token))
            //        currentType = source.Module.ResolveType(token);
            //}

            while (reader.ReadOpCode(out var opCode))
            {
                switch (opCode.OperandType)
                {
                    case OperandType.InlineMethod:
                        readMethod();
                        break;
                    default:
                        reader.Skip(GetOperandSize(opCode.OperandType));
                        break;
                }
            }
            if (deep - 1 > 0)
                tm.ForEach(x => tp.AddRange(ResolveImpProperties(x, deep - 1)));
            return tp.ToArray();
        }
        static int GetOperandSize(OperandType operandType)
        {
            switch (operandType)
            {
                case OperandType.InlineBrTarget:
                    return 4;
                case OperandType.InlineField:
                    return 4;
                case OperandType.InlineI:
                    return 4;
                case OperandType.InlineI8:
                    return 8;
                case OperandType.InlineMethod:
                    return 4;
                case OperandType.InlineR:
                    return 8;
                case OperandType.InlineSig:
                    return 4;
                case OperandType.InlineString:
                    return 4;
                case OperandType.InlineSwitch:
                    return 4;
                case OperandType.InlineTok:
                    return 4;
                case OperandType.InlineType:
                    return 4;
                case OperandType.InlineVar:
                    return 2;
                case OperandType.ShortInlineBrTarget:
                    return 1;
                case OperandType.ShortInlineI:
                    return 1;
                case OperandType.ShortInlineR:
                    return 4;
                case OperandType.ShortInlineVar:
                    return 1;
                default:
                    return 0;
            }
        }
    }
    class BytesReader
    {
        public byte[] Data;
        public int Index = 0;
        public static Dictionary<short, OpCode> OpCodeList = new Dictionary<short, OpCode>();
        static BytesReader()
        {
            foreach (var opcodeFI in typeof(OpCodes).GetFields())
            {
                var opCodeObj = opcodeFI.GetValue(null);
                if (opCodeObj != null)
                {
                    var opCode = (OpCode)opCodeObj;
                    OpCodeList[opCode.Value] = opCode;
                }
            }
        }
        public BytesReader(byte[] data)
        {
            Data = data;
        }
        public bool Skip(int size)
        {
            if (Index + size <= Data.Length)
            {
                Index += size;
                return true;
            }
            return false;
        }
        public bool ReadBytes(int size, out byte[] value)
        {
            value = null;
            if (Index + size <= Data.Length)
            {
                value = new byte[size];
                Buffer.BlockCopy(Data, Index, value, 0, value.Length);
                Index += size;
                return true;
            }
            return false;
        }
        public bool ReadValue<T>(out T value) where T : struct
        {
            value = default;
#if Net4
            int len = Marshal.SizeOf(typeof(T));
#else
            int len = Marshal.SizeOf<T>();
#endif
            if (ReadBytes(len, out var data))
            {
                IntPtr ptr = Marshal.AllocHGlobal(len);
                Marshal.Copy(data, 0, ptr, len);
#if Net4
                value = (T)Marshal.PtrToStructure(ptr, typeof(T));
#else
                value = Marshal.PtrToStructure<T>(ptr);
#endif
                Marshal.FreeHGlobal(ptr);
                return true;
            }
            return false;
        }
        public bool ReadInt32(out Int32 value)
        {
            value = default;
            if (ReadBytes(4, out var data))
            {
                value = BitConverter.ToInt32(data, 0);
                return true;
            }
            return false;
        }
        public bool ReadInt16(out Int16 value)
        {
            value = default;
            if (ReadBytes(2, out var data))
            {
                value = BitConverter.ToInt16(data, 0);
                return true;
            }
            return false;
        }
        public bool ReadByte(out byte value)
        {
            value = default;
            if (ReadBytes(1, out var data))
            {
                value = data[0];
                return true;
            }
            return false;
        }
        public bool ReadOpCode(out OpCode value)
        {
            value = default;
            if (ReadByte(out var HByte))
            {
                short key = HByte;
                if (key != 0xFE || (Skip(-1) && ReadInt16(out key)))
                {     //key = HByte;
                    return OpCodeList.TryGetValue(key, out value);
                }
            }
            return false;
        }
    }
}
