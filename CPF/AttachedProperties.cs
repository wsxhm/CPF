using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using CPF.Reflection;
using System.Runtime.CompilerServices;

namespace CPF
{
    public class AttachedProperties : IEnumerable
    {
        public AttachedProperties(CpfObject owner)
        {
            this.owner = owner;
        }
        CpfObject owner;
        /// <summary>
        /// 设置附加属性值
        /// </summary>
        /// <typeparam name="Value"></typeparam>
        /// <param name="attached"></param>
        /// <param name="value"></param>
        public void Add<Value>(Attached<Value> attached, Value value)
        {
            attached(owner, value);
        }
        /// <summary>
        /// 设置附加属性值和绑定
        /// </summary>
        /// <typeparam name="Value"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="attached"></param>
        /// <param name="value"></param>
        /// <param name="sourcePropertyName"></param>
        /// <param name="source"></param>
        /// <param name="bindingMode"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="SourceToTargetError"></param>
        /// <param name="TargetToSourceError"></param>
        public void Add<Value, S, T>(Attached<Value> attached, Value value, string sourcePropertyName, CpfObject source, BindingMode bindingMode = BindingMode.OneWay, Func<S, T> convert = null, Func<T, S> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        {
            var name = attached.GetAttachedPropertyName();
            //owner.AttachedNotify.AddAttached(name, attached);
            attached(owner, value);
            owner.AttachedNotify.Bindings.Add(name, sourcePropertyName, source, bindingMode, convert, convertBack, SourceToTargetError, TargetToSourceError);
        }
        /// <summary>
        /// 设置附加属性值和绑定
        /// </summary>
        /// <typeparam name="Value"></typeparam>
        /// <param name="attached"></param>
        /// <param name="value"></param>
        /// <param name="sourcePropertyName"></param>
        /// <param name="source"></param>
        /// <param name="bindingMode"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="SourceToTargetError"></param>
        /// <param name="TargetToSourceError"></param>
        public void Add<Value>(Attached<Value> attached, Value value, string sourcePropertyName, CpfObject source, BindingMode bindingMode = BindingMode.OneWay, Func<object, object> convert = null, Func<object, object> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        {
            var name = attached.GetAttachedPropertyName();
            //owner.AttachedNotify.AddAttached(name, attached);
            attached(owner, value);
            owner.AttachedNotify.Bindings.Add(name, sourcePropertyName, source, bindingMode, convert, convertBack, SourceToTargetError, TargetToSourceError);
        }
        ///// <summary>
        ///// 设置附加属性值和绑定
        ///// </summary>
        ///// <typeparam name="Value"></typeparam>
        ///// <typeparam name="S"></typeparam>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="attached"></param>
        ///// <param name="value"></param>
        ///// <param name="sourcePropertyName"></param>
        ///// <param name="func"></param>
        ///// <param name="bindingMode"></param>
        ///// <param name="convert"></param>
        ///// <param name="convertBack"></param>
        ///// <param name="SourceToTargetError"></param>
        ///// <param name="TargetToSourceError"></param>
        //public void Add<Value, S, T>(Attached<Value> attached, Value value, string sourcePropertyName, Func<UIElement> func, BindingMode bindingMode = BindingMode.OneWay, Func<S, T> convert = null, Func<T, S> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        //{
        //    var name = attached.GetAttachedPropertyName();
        //    owner.AttachedNotify.AddAttached(name, attached);
        //    attached(owner, value);
        //    owner.AttachedNotify.Bindings.Add(name, sourcePropertyName, func, bindingMode, convert, convertBack, SourceToTargetError, TargetToSourceError);
        //}
        ///// <summary>
        ///// 设置附加属性值和绑定
        ///// </summary>
        ///// <typeparam name="Value"></typeparam>
        ///// <param name="attached"></param>
        ///// <param name="value"></param>
        ///// <param name="sourcePropertyName"></param>
        ///// <param name="func"></param>
        ///// <param name="bindingMode"></param>
        ///// <param name="convert"></param>
        ///// <param name="convertBack"></param>
        ///// <param name="SourceToTargetError"></param>
        ///// <param name="TargetToSourceError"></param>
        //public void Add<Value>(Attached<Value> attached, Value value, string sourcePropertyName, Func<UIElement> func, BindingMode bindingMode = BindingMode.OneWay, Func<object, object> convert = null, Func<object, object> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        //{
        //    var name = attached.GetAttachedPropertyName();
        //    owner.AttachedNotify.AddAttached(name, attached);
        //    attached(owner, value);
        //    owner.AttachedNotify.Bindings.Add(name, sourcePropertyName, func, bindingMode, convert, convertBack, SourceToTargetError, TargetToSourceError);
        //}
        /// <summary>
        /// 设置附加属性值和绑定
        /// </summary>
        /// <typeparam name="Value"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="attached"></param>
        /// <param name="value"></param>
        /// <param name="sourcePropertyName"></param>
        /// <param name="func">初始化的时候查找相对元素</param>
        /// <param name="bindingMode"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="SourceToTargetError"></param>
        /// <param name="TargetToSourceError"></param>
        public void Add<Value, S, T>(Attached<Value> attached, Value value, string sourcePropertyName, Func<UIElement, UIElement> func, BindingMode bindingMode = BindingMode.OneWay, Func<S, T> convert = null, Func<T, S> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        {
            var name = attached.GetAttachedPropertyName();
            //owner.AttachedNotify.AddAttached(name, attached);
            attached(owner, value);
            owner.AttachedNotify.Bindings.Add(name, sourcePropertyName, func, bindingMode, convert, convertBack, SourceToTargetError, TargetToSourceError);
        }
        /// <summary>
        /// 设置附加属性值和绑定
        /// </summary>
        /// <typeparam name="Value"></typeparam>
        /// <param name="attached"></param>
        /// <param name="value"></param>
        /// <param name="sourcePropertyName"></param>
        /// <param name="func">初始化的时候查找相对元素</param>
        /// <param name="bindingMode"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="SourceToTargetError"></param>
        /// <param name="TargetToSourceError"></param>
        public void Add<Value>(Attached<Value> attached, Value value, string sourcePropertyName, Func<UIElement, UIElement> func, BindingMode bindingMode = BindingMode.OneWay, Func<object, object> convert = null, Func<object, object> convertBack = null, Action<Binding, object, Exception> SourceToTargetError = null, Action<Binding, object, Exception> TargetToSourceError = null)
        {
            var name = attached.GetAttachedPropertyName();
            //owner.AttachedNotify.AddAttached(name, attached);
            attached(owner, value);
            owner.AttachedNotify.Bindings.Add(name, sourcePropertyName, func, bindingMode, convert, convertBack, SourceToTargetError, TargetToSourceError);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (owner.attachedValues != null)
            {
                return owner.attachedValues.GetEnumerator();
            }
            return null;
        }
    }

    class AttachedNotify : CpfObject
    {
        public AttachedNotify(CpfObject cpfObject)
        {
            owner = cpfObject;
        }
        internal CpfObject owner;

        HybridDictionary<string, Delegate> attacheds = new HybridDictionary<string, Delegate>();
        public void AddAttached(string name, Delegate attached)
        {
            if (!attacheds.ContainsKey(name))
            {
                attacheds.Add(name, attached);
            }
        }

        public override object GetValue([CallerMemberName] string propertyName = null)
        {
            if (attacheds.TryGetValue(propertyName, out Delegate att))
            {
                return att.Method.FastInvoke(att.Target, owner);
            }
            return null;
        }

        public override PropertyMetadataAttribute GetPropertyMetadata(string propertyName)
        {
            if (attacheds.TryGetValue(propertyName, out Delegate att))
            {
                return new PropertyMetadataAttribute() { PropertyName = propertyName, PropertyType = att.Method.ReturnType };
            }
            return null;
        }

        public override bool HasProperty(string propertyName)
        {
            return attacheds.ContainsKey(propertyName);
        }

        public override bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (attacheds.TryGetValue(propertyName, out Delegate att))
            {
                att.Method.FastInvoke(att.Target, owner, value.ConvertTo(att.Method.GetParameters()[1].ParameterType));
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// 绑定附加属性
    /// </summary>
    public class AttachedDescribe : BindingDescribe
    {
        /// <summary>
        /// 设置和绑定附加属性
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sourceProperty"></param>
        public AttachedDescribe(object value, string sourceProperty) : base(sourceProperty)
        {
            Value = value;
        }
        /// <summary>
        /// 设置和绑定附加属性
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sourceProperty"></param>
        /// <param name="binding"></param>
        public AttachedDescribe(object value, string sourceProperty, BindingMode binding) : base(sourceProperty, binding)
        {
            Value = value;
        }
        /// <summary>
        /// 设置和绑定附加属性
        /// </summary>
        /// <param name="value"></param>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        /// <param name="binding"></param>
        public AttachedDescribe(object value, object source, string sourceProperty, BindingMode binding) : base(source, sourceProperty, binding)
        {
            Value = value;
        }
        /// <summary>
        /// 设置和绑定附加属性
        /// </summary>
        /// <param name="value"></param>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        public AttachedDescribe(object value, object source, string sourceProperty) : base(source, sourceProperty)
        {
            Value = value;
        }
        /// <summary>
        /// 设置和绑定附加属性
        /// </summary>
        /// <param name="value"></param>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        /// <param name="convert"></param>
        public AttachedDescribe(object value, object source, string sourceProperty, Func<object, object> convert) : base(source, sourceProperty, convert)
        {
            Value = value;
        }
        /// <summary>
        /// 设置和绑定附加属性
        /// </summary>
        /// <param name="value"></param>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        /// <param name="binding"></param>
        /// <param name="convert"></param>
        public AttachedDescribe(object value, object source, string sourceProperty, BindingMode binding, Func<object, object> convert) : base(source, sourceProperty, binding, convert)
        {
            Value = value;
        }
        /// <summary>
        /// 设置和绑定附加属性
        /// </summary>
        /// <param name="value"></param>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        /// <param name="binding"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        public AttachedDescribe(object value, object source, string sourceProperty, BindingMode binding, Func<object, object> convert, Func<object, object> convertBack) : base(source, sourceProperty, binding, convert, convertBack)
        {
            Value = value;
        }
        /// <summary>
        /// 设置和绑定附加属性
        /// </summary>
        /// <param name="value"></param>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        /// <param name="binding"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="SourceToTargetError"></param>
        /// <param name="TargetToSourceError"></param>
        public AttachedDescribe(object value, object source, string sourceProperty, BindingMode binding, Func<object, object> convert, Func<object, object> convertBack, Action<Binding, object, Exception> SourceToTargetError, Action<Binding, object, Exception> TargetToSourceError) : base(source, sourceProperty, binding, convert, convertBack, SourceToTargetError, TargetToSourceError)
        {
            Value = value;
        }


        public object Value { get; set; }
    }
}
