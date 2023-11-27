using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.Linq;
using CPF.Controls;
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics;

namespace CPF
{
    /// <summary>
    /// 设置绑定
    /// </summary>
    public class Bindings : IEnumerable
    {
        CpfObject owner;
        internal Bindings(CpfObject owner)
        {
            this.owner = owner;
        }

        internal HybridDictionary<string, List<Binding>> binds = new HybridDictionary<string, List<Binding>>();
        /// <summary>
        /// 添加绑定
        /// </summary>
        /// <param name="propertyName">绑定的目标属性名</param>
        /// <param name="sourcePropertyName">绑定的数据源对象的属性名</param>
        /// <param name="source">数据源对象，如果为空则绑定到DataContext对象上</param>
        /// <param name="bindingMode">绑定模式</param>
        /// <param name="convert">源到目标数据转换</param>
        /// <param name="convertBack">目标到源数据转换</param>
        /// <param name="SourceToTargetError ">源到目标数据转换出现异常时</param>
        /// <param name="TargetToSourceError ">目标到源数据转换出现异常时</param>
        public Binding Add(string propertyName, string sourcePropertyName, object source = null, BindingMode bindingMode = BindingMode.OneWay, Func<object, object> convert = null, Func<object, object> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        {
            return AddBinding(propertyName, sourcePropertyName, source, bindingMode, convert, convertBack, SourceToTargetError, TargetToSourceError);
        }

        public Binding Add<S, T>(string propertyName, string sourcePropertyName, object source = null, BindingMode bindingMode = BindingMode.OneWay, Func<S, T> convert = null, Func<T, S> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        {
            Func<object, object> _convert = null;
            if (convert != null)
            {
                _convert = a =>
                {
                    if (a != null && typeof(S).IsValueType && typeof(S) != a.GetType() && ObjectExtenstions.ConvertTypes.Contains(typeof(S)))
                    {
                        a = Convert.ChangeType(a, typeof(S));
                    }
                    return convert((S)a);
                };
            }
            Func<object, object> _cb = null;
            if (convertBack != null)
            {
                _cb = a =>
                {
                    if (a != null && typeof(T).IsValueType && typeof(T) != a.GetType() && ObjectExtenstions.ConvertTypes.Contains(typeof(T)))
                    {
                        a = Convert.ChangeType(a, typeof(T));
                    }
                    return convertBack((T)a);
                };
            }
            return AddBinding(propertyName, sourcePropertyName, source, bindingMode, _convert, _cb, SourceToTargetError, TargetToSourceError);
        }

        private Binding AddBinding(string propertyName, string sourcePropertyName, object source, BindingMode bindingMode, Func<object, object> convert, Func<object, object> convertBack, Action<Binding, object, Exception> SourceToTargetError, Action<Binding, object, Exception> TargetToSourceError)
        {
            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(sourcePropertyName))
            {
                throw new Exception("属性名不能为空！");
            }
            var b = new Binding(source, sourcePropertyName, propertyName, bindingMode) { Convert = convert, ConvertBack = convertBack, SourceToTargetError = SourceToTargetError, TargetToSourceError = TargetToSourceError };
            List<Binding> list;
            if (!owner.Bindings.binds.TryGetValue(propertyName, out list))
            {
                list = new List<Binding>();
                owner.Bindings.binds.Add(propertyName, list);
            }
            list.Add(b);
            b.Owner = owner;
            if (bindingMode == BindingMode.OneWay || bindingMode == BindingMode.TwoWay)
            {
                if (source == null)
                {
                    var dc = owner.DataContext;
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
                }
                else
                {
                    //((INotifyPropertyChanged)source).PropertyChanged += b.PropertyChanged;
                    
                    b.RegisterPropertyChanged((INotifyPropertyChanged)source);
                    b.SourceToTarget();
                }
            }
            else if (bindingMode == BindingMode.OneWayToSource)
            {
                b.TargetToSource();
            }
            else if (bindingMode == BindingMode.OneTime)
            {
                b.SourceToTarget();
            }
            //Debug.WriteLine($"sourcePropertyName:{sourcePropertyName},bindingMode:{bindingMode}");
            return b;
        }
        /// <summary>
        /// 简化 DataGridCellTemplate 绑定
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="cellTemplate"></param>
        public void Add(string propertyName, DataGridCellTemplate cellTemplate)
        {
            if (cellTemplate.Cell.Column.Binding == null)
            {
                return;
            }
            AddBinding(propertyName, cellTemplate.Cell.Column.Binding.SourcePropertyName, null, cellTemplate.Cell.Column.Binding.BindingMode, cellTemplate.Convert(cellTemplate.Cell.Column.Binding.Convert), cellTemplate.ConvertBack(cellTemplate.Cell.Column.Binding.ConvertBack), cellTemplate.Error, cellTemplate.Error);
        }

        //public void Add<T, S>(Binding<T, S> binding) where T : CpfObject where S : INotifyPropertyChanged
        //{

        //}
        ///// <summary>
        ///// 添加绑定到DataContext
        ///// </summary>
        ///// <param name="propertyName"></param>
        ///// <param name="sourcePropertyName"></param>
        ///// <param name="bindingMode"></param>
        ///// <param name="convert"></param>
        ///// <param name="convertBack"></param>
        ///// <returns></returns>
        //public Binding Add(string propertyName, string sourcePropertyName, BindingMode bindingMode = BindingMode.OneWay, Func<object, object> convert = null, Func<object, object> convertBack = null)
        //{
        //    return Add(propertyName, sourcePropertyName, null, bindingMode, convert, convertBack);
        //}
        /// <summary>
        /// 添加绑定到UIElement对象上，定义绑定在对应的Element层次上
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="sourcePropertyName"></param>
        /// <param name="sourceElementLayer">UI元素层，0是自己，1是Parent，2是Parent.Parent....</param>
        /// <param name="bindingMode"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="SourceToTargetError"></param>
        /// <param name="TargetToSourceError"></param>
        public Binding Add(string propertyName, string sourcePropertyName, byte sourceElementLayer, BindingMode bindingMode = BindingMode.OneWay, Func<object, object> convert = null, Func<object, object> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        {
            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(sourcePropertyName))
            {
                throw new Exception("属性名不能为空！");
            }
            var b = new Binding(null, sourcePropertyName, propertyName, bindingMode) { Convert = convert, ConvertBack = convertBack, IsDataContext = false, SourceElementLayer = sourceElementLayer, SourceToTargetError = SourceToTargetError, TargetToSourceError = TargetToSourceError };
            List<Binding> list;
            if (!owner.Bindings.binds.TryGetValue(propertyName, out list))
            {
                list = new List<Binding>();
                owner.Bindings.binds.Add(propertyName, list);
            }
            list.Add(b);
            b.Owner = owner;
            return b;
        }

        public Binding Add<S, T>(string propertyName, string sourcePropertyName, byte sourceElementLayer, BindingMode bindingMode = BindingMode.OneWay, Func<S, T> convert = null, Func<T, S> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        {
            Func<object, object> _convert = null;
            if (convert != null)
            {
                _convert = a =>
                {
                    if (a != null && typeof(S).IsValueType && typeof(S) != a.GetType() && ObjectExtenstions.ConvertTypes.Contains(typeof(S)))
                    {
                        a = Convert.ChangeType(a, typeof(S));
                    }
                    return convert((S)a);
                };
            }
            Func<object, object> _cb = null;
            if (convertBack != null)
            {
                _cb = a =>
                {
                    if (a != null && typeof(T).IsValueType && typeof(T) != a.GetType() && ObjectExtenstions.ConvertTypes.Contains(typeof(T)))
                    {
                        a = Convert.ChangeType(a, typeof(T));
                    }
                    return convertBack((T)a);
                };
            }
            return Add(propertyName, sourcePropertyName, sourceElementLayer, bindingMode, _convert, _cb, SourceToTargetError, TargetToSourceError);
        }
        /// <summary>
        /// 一般用于在设计模板的时候绑定当前页面里的元素，初始化的时候查找相对元素并绑定
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="sourcePropertyName"></param>
        /// <param name="find">初始化的时候查找相对元素并绑定，回调的参数是当前的Owner元素</param>
        /// <param name="bindingMode"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="SourceToTargetError"></param>
        /// <param name="TargetToSourceError"></param>
        public void Add(string propertyName, string sourcePropertyName, Func<UIElement, UIElement> find, BindingMode bindingMode = BindingMode.OneWay, Func<object, object> convert = null, Func<object, object> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        {
            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(sourcePropertyName))
            {
                throw new Exception("属性名不能为空！");
            }
            if (find == null)
            {
                AddBinding(propertyName, sourcePropertyName, null, bindingMode, convert, convertBack, SourceToTargetError, TargetToSourceError);
            }
            else
            {
                Threading.Dispatcher.MainThread.BeginInvoke(() =>
                {
                    UIElement ow;
                    if (owner is AttachedNotify notify)
                    {
                        ow = notify.owner as UIElement;
                    }
                    else
                    {
                        ow = (UIElement)owner;
                    }
                    var ele = find(ow);
                    if (ele != null)
                    {
                        Add(propertyName, sourcePropertyName, ele, bindingMode, convert, convertBack, SourceToTargetError, TargetToSourceError);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(find.ToString() + "未找到元素，无法绑定");
                    }
                });
            }
        }
        /// <summary>
        /// 初始化的时候查找相对元素并绑定
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="sourcePropertyName"></param>
        /// <param name="find"></param>
        /// <param name="bindingMode"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="SourceToTargetError"></param>
        /// <param name="TargetToSourceError"></param>
        public void Add<S, T>(string propertyName, string sourcePropertyName, Func<UIElement, UIElement> find, BindingMode bindingMode = BindingMode.OneWay, Func<S, T> convert = null, Func<T, S> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        {
            Func<object, object> _convert = null;
            if (convert != null)
            {
                _convert = a =>
                {
                    if (a != null && typeof(S).IsValueType && typeof(S) != a.GetType() && ObjectExtenstions.ConvertTypes.Contains(typeof(S)))
                    {
                        a = Convert.ChangeType(a, typeof(S));
                    }
                    return convert((S)a);
                };
            }
            Func<object, object> _cb = null;
            if (convertBack != null)
            {
                _cb = a =>
                {
                    if (a != null && typeof(T).IsValueType && typeof(T) != a.GetType() && ObjectExtenstions.ConvertTypes.Contains(typeof(T)))
                    {
                        a = Convert.ChangeType(a, typeof(T));
                    }
                    return convertBack((T)a);
                };
            }
            Add(propertyName, sourcePropertyName, find, bindingMode, _convert, _cb, SourceToTargetError, TargetToSourceError);
        }
        ///// <summary>
        ///// 一般用于在设计模板的时候绑定当前页面里的元素
        ///// </summary>
        ///// <typeparam name="S"></typeparam>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="propertyName"></param>
        ///// <param name="sourcePropertyName"></param>
        ///// <param name="func"></param>
        ///// <param name="bindingMode"></param>
        ///// <param name="convert"></param>
        ///// <param name="convertBack"></param>
        ///// <param name="SourceToTargetError"></param>
        ///// <param name="TargetToSourceError"></param>
        //public void Add<S, T>(string propertyName, string sourcePropertyName, Func<UIElement> func, BindingMode bindingMode = BindingMode.OneWay, Func<S, T> convert = null, Func<T, S> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        //{
        //    Func<object, object> _convert = null;
        //    if (convert != null)
        //    {
        //        _convert = a => convert((S)a);
        //    }
        //    Func<object, object> _cb = null;
        //    if (convertBack != null)
        //    {
        //        _cb = a => convertBack((T)a);
        //    }
        //    Add(propertyName, sourcePropertyName, func, bindingMode, _convert, _cb, SourceToTargetError, TargetToSourceError);
        //}
        ///// <summary>
        ///// 一般用于在设计模板的时候绑定当前页面里的元素
        ///// </summary>
        ///// <param name="propertyName"></param>
        ///// <param name="sourcePropertyName"></param>
        ///// <param name="func"></param>
        ///// <param name="bindingMode"></param>
        ///// <param name="convert"></param>
        ///// <param name="convertBack"></param>
        ///// <param name="SourceToTargetError"></param>
        ///// <param name="TargetToSourceError"></param>
        ///// <returns></returns>
        //public void Add(string propertyName, string sourcePropertyName, Func<UIElement> func, BindingMode bindingMode = BindingMode.OneWay, Func<object, object> convert = null, Func<object, object> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        //{
        //    if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(sourcePropertyName))
        //    {
        //        throw new Exception("属性名不能为空！");
        //    }
        //    if (func == null)
        //    {
        //        AddBinding(propertyName, sourcePropertyName, null, bindingMode, convert, convertBack, SourceToTargetError, TargetToSourceError);
        //    }
        //    else
        //    {
        //        Threading.Dispatcher.MainThread.BeginInvoke(() =>
        //        {
        //            var ele = func();
        //            if (ele != null)
        //            {
        //                Add(propertyName, sourcePropertyName, ele, bindingMode, convert, convertBack, SourceToTargetError, TargetToSourceError);
        //            }
        //            else
        //            {
        //                System.Diagnostics.Debug.WriteLine(func.ToString() + "未找到元素，无法绑定");
        //            }
        //        });
        //    }
        //}
        ///// <summary>
        ///// 绑定到特定元素名的元素上，元素名必须是唯一的，而且必须是一起添加到可视化树或者元素已经添加到可视化树的，如果eleName为空，就绑定到DataContext
        ///// </summary>
        ///// <param name="propertyName"></param>
        ///// <param name="sourcePropertyName"></param>
        ///// <param name="eleName"></param>
        ///// <param name="bindingMode"></param>
        ///// <param name="convert"></param>
        ///// <param name="convertBack"></param>
        ///// <param name="SourceToTargetError"></param>
        ///// <param name="TargetToSourceError"></param>
        //public Binding Add(string propertyName, string sourcePropertyName, string eleName, BindingMode bindingMode = BindingMode.OneWay, Func<object, object> convert = null, Func<object, object> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        //{
        //    if (string.IsNullOrWhiteSpace(eleName))
        //    {
        //        return AddBinding(propertyName, sourcePropertyName, null, bindingMode, convert, convertBack, SourceToTargetError, TargetToSourceError);
        //    }
        //    if (owner is UIElement element)
        //    {
        //        Threading.Dispatcher.MainThread.BeginInvoke(() =>
        //        {
        //            if (element.Root.nameDic.TryGetValue(eleName, out UIElement ele))
        //            {
        //                Add(propertyName, sourcePropertyName, ele, bindingMode, convert, convertBack);
        //            }
        //            else
        //            {
        //                throw new Exception("未找到元素名为" + eleName + "的元素或者元素未添加到可视化树");
        //            }
        //        });
        //    }
        //    else
        //    {
        //        throw new Exception("UIElement才可以使用该方法");
        //    }
        //    return null;
        //}

        public void Add<S>(string propertyName, Expression<Func<S, object>> source)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// KeyValuePair&lt;string, List&lt;Binding&gt;&gt;
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return binds.GetEnumerator();
        }

        public override string ToString()
        {
            return "Count:" + binds.Count;
        }
    }

    ///// <summary>
    ///// 定义一个简化的绑定类型
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public struct B<T>// : IB
    //{
    //    public B(T value)
    //    {
    //        Value = value;
    //        //HasValue = true;
    //        BindingMode = null;
    //        SourceProperty = null;
    //        Source = null;
    //        Convert = null;
    //    }
    //    public B(string sourceProperty, BindingMode bindingMode)
    //    {
    //        Value = default;
    //        //HasValue = false;
    //        BindingMode = bindingMode;
    //        SourceProperty = sourceProperty;
    //        Source = null;
    //        Convert = null;
    //    }
    //    public B(string sourceProperty, BindingMode bindingMode, Func<object, object> convert)
    //    {
    //        Value = default;
    //        //HasValue = false;
    //        BindingMode = bindingMode;
    //        SourceProperty = sourceProperty;
    //        Source = null;
    //        Convert = convert;
    //    }
    //    public B(object source, string sourceProperty, BindingMode bindingMode)
    //    {
    //        Value = default;
    //        //HasValue = false;
    //        BindingMode = bindingMode;
    //        SourceProperty = sourceProperty;
    //        Source = source;
    //        Convert = null;
    //    }
    //    public B(object source, string sourceProperty, BindingMode bindingMode, Func<object, object> convert)
    //    {
    //        Value = default;
    //        //HasValue = false;
    //        BindingMode = bindingMode;
    //        SourceProperty = sourceProperty;
    //        Source = source;
    //        Convert = convert;
    //    }
    //    //public B(T value, string sourceProperty, BindingMode bindingMode)
    //    //{
    //    //    Value = value;
    //    //    //HasValue = true;
    //    //    BindingMode = bindingMode;
    //    //    SourceProperty = sourceProperty;
    //    //    Source = null;
    //    //}
    //    public Func<object, object> Convert { get; private set; }
    //    public T Value { get; private set; }
    //    public object Source { get; private set; }

    //    //public bool HasValue { get; private set; }

    //    public string SourceProperty { get; private set; }

    //    public BindingMode? BindingMode { get; private set; }

    //    public static implicit operator B<T>(T n)
    //    {
    //        return new B<T>(n);
    //    }
    //    public static implicit operator T(B<T> n)
    //    {
    //        return n.Value;
    //    }
    //    public static implicit operator B<T>((string, BindingMode) n)
    //    {
    //        return new B<T>(n.Item1, n.Item2);
    //    }
    //    public static implicit operator B<T>((string, BindingMode, Func<object, object> convert) n)
    //    {
    //        return new B<T>(n.Item1, n.Item2, n.convert);
    //    }
    //    //public static implicit operator B<T>((T value, string, BindingMode) n)
    //    //{
    //    //    return new B<T>(n.Item1, n.Item2, n.Item3);
    //    //}
    //    public static implicit operator B<T>((object source, string, BindingMode) n)
    //    {
    //        return new B<T>(n.Item1, n.Item2, n.Item3);
    //    }
    //    public static implicit operator B<T>((object source, string, BindingMode, Func<object, object>) n)
    //    {
    //        return new B<T>(n.Item1, n.Item2, n.Item3, n.Item4);
    //    }

    //    //public static B<T> operator !(string property)
    //    //{
    //    //    return new B<T>(property, CPF.BindingMode.OneWay);
    //    //}
    //}

    //public interface IB
    //{

    //}
}

//#if NETSTANDARD2_0||NET40
//namespace System.Runtime.CompilerServices
//{
//    //
//    // 摘要:
//    //     Allows capturing of the expressions passed to a method.
//    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
//    public sealed class CallerArgumentExpressionAttribute : Attribute
//    {
//        //
//        // 摘要:
//        //     Gets the target parameter name of the CallerArgumentExpression.
//        //
//        // 返回结果:
//        //     The name of the targeted parameter of the CallerArgumentExpression.
//        public string ParameterName
//        {
//            get
//            {
//                throw null;
//            }
//        }

//        //
//        // 摘要:
//        //     Initializes a new instance of the System.Runtime.CompilerServices.CallerArgumentExpressionAttribute
//        //     class.
//        //
//        // 参数:
//        //   parameterName:
//        //     The name of the targeted parameter.
//        public CallerArgumentExpressionAttribute(string parameterName)
//        {
//        }
//    }
//}
//#endif