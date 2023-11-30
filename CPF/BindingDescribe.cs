using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CPF
{
    /// <summary>
    /// 简化绑定操作
    /// </summary>
    public class BindingDescribe
    {
        internal BindingDescribe() { }

        public BindingDescribe(string sourceProperty)
        {
            PropertyName = sourceProperty;
        }

        public BindingDescribe(string sourceProperty, BindingMode binding)
        {
            PropertyName = sourceProperty;
            BindingMode = binding;
        }
        /// <summary>
        /// 设置绑定
        /// </summary>
        /// <param name="source">如果是int或者byte，0是自己，1是Parent，2是Parent.Parent....</param>
        /// <param name="sourceProperty"></param>
        /// <param name="binding"></param>
        public BindingDescribe(object source, string sourceProperty, BindingMode binding)
        {
            PropertyName = sourceProperty;
            BindingMode = binding;
            Source = source;
        }
        /// <summary>
        /// 设置绑定
        /// </summary>
        /// <param name="source">如果是int或者byte，0是自己，1是Parent，2是Parent.Parent....</param>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        public BindingDescribe(object source, string sourceProperty)
        {
            PropertyName = sourceProperty;
            Source = source;
        }
        /// <summary>
        /// 设置绑定
        /// </summary>
        /// <param name="source">如果是int或者byte，0是自己，1是Parent，2是Parent.Parent....</param>
        /// <param name="sourceProperty"></param>
        /// <param name="convert"></param>
        public BindingDescribe(object source, string sourceProperty, Func<object, object> convert)
        {
            PropertyName = sourceProperty;
            Source = source;
            Convert = convert;
        }
        /// <summary>
        /// 设置绑定
        /// </summary>
        /// <param name="source">如果是int或者byte，0是自己，1是Parent，2是Parent.Parent....</param>
        /// <param name="sourceProperty"></param>
        /// <param name="binding"></param>
        /// <param name="convert"></param>
        public BindingDescribe(object source, string sourceProperty, BindingMode binding, Func<object, object> convert)
        {
            BindingMode = binding;
            PropertyName = sourceProperty;
            Source = source;
            Convert = convert;
        }
        /// <summary>
        /// 设置绑定
        /// </summary>
        /// <param name="source">如果是int或者byte，0是自己，1是Parent，2是Parent.Parent....</param>
        /// <param name="sourceProperty"></param>
        /// <param name="binding"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        public BindingDescribe(object source, string sourceProperty, BindingMode binding, Func<object, object> convert, Func<object, object> convertBack)
        {
            BindingMode = binding;
            PropertyName = sourceProperty;
            Source = source;
            Convert = convert;
            ConvertBack = convertBack;
        }
        /// <summary>
        /// 设置绑定
        /// </summary>
        /// <param name="source">如果是int或者byte，0是自己，1是Parent，2是Parent.Parent....</param>
        /// <param name="sourceProperty"></param>
        /// <param name="binding"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="SourceToTargetError"></param>
        /// <param name="TargetToSourceError"></param>
        public BindingDescribe(object source, string sourceProperty, BindingMode binding, Func<object, object> convert, Func<object, object> convertBack, Action<Binding, object, Exception> SourceToTargetError, Action<Binding, object, Exception> TargetToSourceError)
        {
            BindingMode = binding;
            PropertyName = sourceProperty;
            Source = source;
            Convert = convert;
            ConvertBack = convertBack;
            this.SourceToTargetError = SourceToTargetError;
            this.TargetToSourceError = TargetToSourceError;
        }

        public BindingDescribe(CommandDescribe command)
        {
            Command = command;
        }

        /// <summary>
        /// 数据绑定的转换
        /// </summary>
        public Func<object, object> Convert { get; internal set; }

        public string PropertyName { get; internal set; }
        /// <summary>
        /// 简化绑定命令的命令，如果设置了该属性，则使用命令绑定
        /// </summary>
        public CommandDescribe Command { get; set; }
        /// <summary>
        /// 简化触发器设置
        /// </summary>
        public CPF.Styling.TriggerDescribe Trigger { get; set; }

        //public CpfObject Owner { get; internal set; }

        /// <summary>
        /// 双向绑定，如果加数据转换器，两个转换器要对称，否则可能出现死循环或者其他错误
        /// </summary>
        /// <param name="property1"></param>
        /// <param name="property2"></param>
        /// <returns></returns>
        public static Binding operator ==(BindingDescribe property1, BindingDescribe property2)
        {
            var b = new Binding(property2.Source, property2.PropertyName, property1.PropertyName, BindingMode.TwoWay) { Convert = property2.Convert, ConvertBack = property1.Convert };
            List<Binding> list;
            var target = property1.Source as CpfObject;
            if (target == null)
            {
                throw new Exception("property1.Source必须是CpfObject");
            }
            if (!target.Bindings.binds.TryGetValue(property1.PropertyName, out list))
            {
                list = new List<Binding>();
                target.Bindings.binds.Add(property1.PropertyName, list);
            }
            list.Add(b);
            b.Owner = target;
            var source = property2.Source as CpfObject;
            if (source == null)
            {
                throw new Exception("property2.Source必须是CpfObject");
            }
            b.RegisterPropertyChanged(source);
            return b;
        }



        /// <summary>
        /// 右边数据绑定到左边，只传递一次数据
        /// </summary>
        /// <param name="property1"></param>
        /// <param name="property2"></param>
        /// <returns></returns>
        public static Binding operator !=(BindingDescribe property1, BindingDescribe property2)
        {
            var b = new Binding(property2.Source, property2.PropertyName, property1.PropertyName, BindingMode.OneTime) { Convert = property2.Convert, ConvertBack = property1.Convert };
            List<Binding> list;
            var target = property1.Source as CpfObject;
            if (target == null)
            {
                throw new Exception("property1.Source必须是CpfObject");
            }
            if (!target.Bindings.binds.TryGetValue(property1.PropertyName, out list))
            {
                list = new List<Binding>();
                target.Bindings.binds.Add(property1.PropertyName, list);
            }
            list.Add(b);
            b.Owner = target;
            b.SourceToTarget();
            return b;
        }
        /// <summary>
        /// 右边数据绑定到左边
        /// </summary>
        /// <param name="property1"></param>
        /// <param name="property2"></param>
        /// <returns></returns>
        public static Binding operator <=(BindingDescribe property1, BindingDescribe property2)
        {//可重载一元运算符  +、-、!、~
            var b = new Binding(property2.Source, property2.PropertyName, property1.PropertyName, BindingMode.OneWay) { Convert = property2.Convert, ConvertBack = property1.Convert };
            List<Binding> list; var target = property1.Source as CpfObject;
            if (target == null)
            {
                throw new Exception("property1.Source必须是CpfObject");
            }
            if (!target.Bindings.binds.TryGetValue(property1.PropertyName, out list))
            {
                list = new List<Binding>();
                target.Bindings.binds.Add(property1.PropertyName, list);
            }
            list.Add(b);
            b.Owner = target;
            var source = property2.Source as CpfObject;
            if (source == null)
            {
                throw new Exception("property2.Source必须是CpfObject");
            }
            b.RegisterPropertyChanged(source);
            b.SourceToTarget();
            return b;
        }
        /// <summary>
        /// 左边数据绑定到右边
        /// </summary>
        /// <param name="property1"></param>
        /// <param name="property2"></param>
        /// <returns></returns>
        public static Binding operator >=(BindingDescribe property1, BindingDescribe property2)
        {//可重载一元运算符  +、-、!、~
            var b = new Binding(property2.Source, property2.PropertyName, property1.PropertyName, BindingMode.OneWayToSource) { Convert = property2.Convert, ConvertBack = property1.Convert };
            List<Binding> list;
            var target = property1.Source as CpfObject;
            if (target == null)
            {
                throw new Exception("property1.Source必须是CpfObject");
            }
            if (!target.Bindings.binds.TryGetValue(property1.PropertyName, out list))
            {
                list = new List<Binding>();
                target.Bindings.binds.Add(property1.PropertyName, list);
            }
            list.Add(b);
            b.Owner = target;
            b.TargetToSource();
            //property2.Owner.PropertyChanged += b.PropertyChanged;
            return b;
        }

        /// <summary>
        /// 和DataContext的对象的属性双向绑定
        /// </summary>
        /// <param name="property1"></param>
        /// <param name="property2">DataContext的对象的属性名</param>
        /// <returns></returns>
        public static Binding operator ==(BindingDescribe property1, string property2)
        {
            var b = new Binding(null, property2, property1.PropertyName, BindingMode.TwoWay) { ConvertBack = property1.Convert };
            List<Binding> list;
            var target = property1.Source as CpfObject;
            if (target == null)
            {
                throw new Exception("property1.Source必须是CpfObject");
            }
            if (!target.Bindings.binds.TryGetValue(property1.PropertyName, out list))
            {
                list = new List<Binding>();
                target.Bindings.binds.Add(property1.PropertyName, list);
            }
            list.Add(b);
            b.Owner = target;
            var dc = target.DataContext;
            if (dc != null)
            {
                INotifyPropertyChanged n = dc as INotifyPropertyChanged;
                if (n != null)
                {
                    //n.PropertyChanged += b.PropertyChanged;
                    b.RegisterPropertyChanged(n);
                }
                b.Source = new WeakReference(dc);
                b.SourceToTarget();
            }
            return b;
        }
        /// <summary>
        /// DataContext的对象的属性数据绑定到左边，只传递一次数据
        /// </summary>
        /// <param name="property1"></param>
        /// <param name="property2">DataContext的对象的属性名</param>
        /// <returns></returns>
        public static Binding operator !=(BindingDescribe property1, string property2)
        {
            var b = new Binding(null, property2, property1.PropertyName, BindingMode.OneTime) { ConvertBack = property1.Convert };
            List<Binding> list;
            var target = property1.Source as CpfObject;
            if (target == null)
            {
                throw new Exception("property1.Source必须是CpfObject");
            }
            if (!target.Bindings.binds.TryGetValue(property1.PropertyName, out list))
            {
                list = new List<Binding>();
                target.Bindings.binds.Add(property1.PropertyName, list);
            }
            list.Add(b);
            b.Owner = target;

            var dc = target.DataContext;
            if (dc != null)
            {
                b.Source = new WeakReference(dc);
                b.SourceToTarget();
            }
            return b;
        }
        /// <summary>
        /// DataContext的对象的属性数据绑定到左边
        /// </summary>
        /// <param name="property1"></param>
        /// <param name="property2">DataContext的对象的属性名</param>
        /// <returns></returns>
        public static Binding operator <=(BindingDescribe property1, string property2)
        {
            var b = new Binding(null, property2, property1.PropertyName, BindingMode.OneWay) { ConvertBack = property1.Convert };
            List<Binding> list;
            var target = property1.Source as CpfObject;
            if (target == null)
            {
                throw new Exception("property1.Source必须是CpfObject");
            }
            if (!target.Bindings.binds.TryGetValue(property1.PropertyName, out list))
            {
                list = new List<Binding>();
                target.Bindings.binds.Add(property1.PropertyName, list);
            }
            list.Add(b);
            b.Owner = target;
            var dc = target.DataContext;
            if (dc != null)
            {
                INotifyPropertyChanged n = dc as INotifyPropertyChanged;
                if (n != null)
                {
                    //n.PropertyChanged += b.PropertyChanged;
                    b.RegisterPropertyChanged(n);
                }
                b.Source = new WeakReference(dc);
                b.SourceToTarget();
            }
            return b;
        }
        /// <summary>
        /// 左边数据绑定到DataContext的对象的属性
        /// </summary>
        /// <param name="property1"></param>
        /// <param name="property2">DataContext的对象的属性名</param>
        /// <returns></returns>
        public static Binding operator >=(BindingDescribe property1, string property2)
        {
            var b = new Binding(null, property2, property1.PropertyName, BindingMode.OneWayToSource) { ConvertBack = property1.Convert };
            List<Binding> list;
            var target = property1.Source as CpfObject;
            if (target == null)
            {
                throw new Exception("property1.Source必须是CpfObject");
            }
            if (!target.Bindings.binds.TryGetValue(property1.PropertyName, out list))
            {
                list = new List<Binding>();
                target.Bindings.binds.Add(property1.PropertyName, list);
            }
            list.Add(b);
            b.Owner = target;

            var dc = target.DataContext;
            if (dc != null)
            {
                b.Source = new WeakReference(dc);
                b.TargetToSource();
            }
            return b;
        }
        //public static BindingDescribe operator *(BindingDescribe property1, string property2)
        //{
        //    return null;
        //}
        //public static BindingDescribe operator *(BindingDescribe property1, BindingDescribe property2)
        //{
        //    return null;
        //}
        public BindingMode BindingMode { get; set; } = BindingMode.OneWay;
        public object Source { get; set; }

        public Action<Binding, object, Exception> SourceToTargetError { get; set; }
        public Action<Binding, object, Exception> TargetToSourceError { get; set; }
        public Func<object, object> ConvertBack { get; set; }

        public static implicit operator BindingDescribe(string sourceProperty)
        {
            return new BindingDescribe { PropertyName = sourceProperty };
        }
        public static implicit operator BindingDescribe(CommandDescribe command)
        {
            return new BindingDescribe { Command = command };
        }
        public static implicit operator BindingDescribe(CPF.Styling.TriggerDescribe trigger)
        {
            return new BindingDescribe { Trigger = trigger };
        }
        public static implicit operator BindingDescribe((string sourceProperty, BindingMode binding) item)
        {
            return new BindingDescribe { PropertyName = item.sourceProperty, BindingMode = item.binding };
        }
        public static implicit operator BindingDescribe((object source, string sourceProperty, BindingMode binding) item)
        {
            return new BindingDescribe { PropertyName = item.sourceProperty, Source = item.source, BindingMode = item.binding };
        }
        public static implicit operator BindingDescribe((object source, string sourceProperty) item)
        {
            return new BindingDescribe { Source = item.source, PropertyName = item.sourceProperty };
        }
        public static implicit operator BindingDescribe((object source, string sourceProperty, Func<object, object> convert) item)
        {
            return new BindingDescribe { Source = item.source, PropertyName = item.sourceProperty, Convert = item.convert };
        }
        public static implicit operator BindingDescribe((object source, string sourceProperty, BindingMode binding, Func<object, object> convert) item)
        {
            return new BindingDescribe { PropertyName = item.sourceProperty, Source = item.source, BindingMode = item.binding, Convert = item.convert };
        }
        public static implicit operator BindingDescribe((object source, string sourceProperty, BindingMode binding, Func<object, object> convert, Func<object, object> convertBack) item)
        {
            return new BindingDescribe { PropertyName = item.sourceProperty, Source = item.source, BindingMode = item.binding, Convert = item.convert, ConvertBack = item.convertBack };
        }
        public static implicit operator BindingDescribe((object source, string sourceProperty, BindingMode binding, Func<object, object> convert, Func<object, object> convertBack, Action<Binding, object, Exception> SourceToTargetError, Action<Binding, object, Exception> TargetToSourceError) item)
        {
            return new BindingDescribe { PropertyName = item.sourceProperty, Source = item.source, BindingMode = item.binding, Convert = item.convert, ConvertBack = item.convertBack, SourceToTargetError = item.SourceToTargetError, TargetToSourceError = item.TargetToSourceError };
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    /// <summary>
    /// 命令绑定
    /// </summary>
    public class CommandDescribe
    {
        public Action<CpfObject, object> Action { get; set; }
        public Func<CpfObject, object, Task> AsyncAction { get; set; }

        public string MethodName { get; set; }
        public object[] Parameters { get; set; }

        public object Target { get; set; }

        public Func<UIElement, UIElement> Find { get; set; }
        /// <summary>
        /// 用委托定义个命令绑定
        /// </summary>
        /// <param name="command"></param>
        public CommandDescribe(Action<CpfObject, object> command)
        {
            Action = command;
        }

        public CommandDescribe(Func<CpfObject, object, Task> command)
        {
            this.AsyncAction = command;
        }
        /// <summary>
        /// 定义个命令绑定
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="obj"></param>
        /// <param name="ps"></param>
        public CommandDescribe(string methodName, object obj = null, params object[] ps)
        {
            MethodName = methodName;
            Parameters = ps;
            Target = obj;
        }
        /// <summary>
        /// 查找元素并绑定命令
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="find"></param>
        /// <param name="ps"></param>
        public CommandDescribe(string methodName, Func<UIElement, UIElement> find, params object[] ps)
        {
            MethodName = methodName;
            Parameters = ps;
            Find = find;
        }
    }
}
