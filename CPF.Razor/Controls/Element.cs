using CPF.Input;
using Microsoft.AspNetCore.Components;
//using Microsoft.MobileBlazorBindings.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using CPF.Reflection;
using System.Threading.Tasks;

namespace CPF.Razor.Controls
{
    public abstract partial class Element<T> : NativeControlComponentBase<T> where T : UIElement, new()
    {
        public Element()
        {
            type = GetType();

        }

        Type type;
        protected override void RenderAttributes(AttributesBuilder builder)
        {
            base.RenderAttributes(builder);

            //var type = GetType();
            var ps = type.GetProperties();
            foreach (var item in ps)
            {
                var attr = item.GetCustomAttributes(typeof(ParameterAttribute), true);
                if (attr != null && attr.Length > 0 && item.PropertyType != typeof(RenderFragment))
                {
                    var v = item.GetValue(this);
                    if (v != null)
                    {
                        if (item.PropertyType == typeof(EventCallback) || (item.PropertyType.IsGenericType && item.PropertyType.GetGenericTypeDefinition() == typeof(EventCallback<>)))
                        {//事件注册还必须加小写的on
                            if (cPs.ContainsKey(item.Name))
                            {
                                builder.AddAttribute("on" + item.Name, EventCallback.Factory.Create<ChangeEventArgs>(this, a => HandleChanged(item, a)));
                            }
                            else
                            {
                                builder.AddAttribute("on" + item.Name, v);
                            }
                        }
                        else
                        {
                            builder.AddAttribute(item.Name, v);
                        }
                    }
                }
            }

            //if (MarginLeft != null)
            //{
            //    builder.AddAttribute(nameof(MarginLeft), MarginLeft);
            //}
        }

        private Task HandleChanged(PropertyInfo info, ChangeEventArgs evt)
        {
            return (Task)info.FastGetValue(this).Invoke("InvokeAsync", evt.Value);
        }

        public override void ApplyAttribute(ulong attributeEventHandlerId, string attributeName, object attributeValue, string attributeEventUpdatesAttributeName)
        {
            //var p = Element.GetPropertyMetadata(attributeName);
            //var p = Element.Type.GetProperty(attributeName);
            //if (p != null)
            if (ePs.TryGetValue(attributeName, out var p))
            {
                //Element.SetValue(attributeValue.ConvertTo(p.PropertyType), attributeName);
                //p.FastSetValue(Element, attributeValue.ConvertTo(p.PropertyType));
                //这边传过来的值会变成字符串，那直接读取这边的属性值就好了
                var value = this.GetValue(attributeName);
                p.FastSetValue(Element, value.ConvertTo(p.PropertyType));
            }
            else
            {
                if (events.Contains(attributeName))
                {
                    handlerIds[attributeName] = attributeEventHandlerId;
                    Renderer.RegisterEvent(attributeEventHandlerId, id => { if (id == attributeEventHandlerId) { handlerIds.Remove(attributeName); } });
                }
            }
        }

        Dictionary<string, ulong> handlerIds = new Dictionary<string, ulong>();

        /// <summary>
        /// 事件
        /// </summary>
        HashSet<string> events = new HashSet<string>();
        /// <summary>
        /// 元素属性
        /// </summary>
        Dictionary<string, PropertyInfo> ePs = new Dictionary<string, PropertyInfo>();
        /// <summary>
        /// 依赖属性，用于绑定通知，TextChanged
        /// </summary>
        Dictionary<string, PropertyInfo> cPs = new Dictionary<string, PropertyInfo>();

        protected override T CreateElement()
        {
            var r = base.CreateElement();
            var type = typeof(T);
            var es = type.GetEvents();
            var ps = type.GetProperties();
            foreach (var item in ps)
            {
                if (item.Name != "Item" || item.GetIndexParameters().Length == 0)
                {
                    ePs.Add(item.Name, item);
                    if (r.GetPropertyMetadata(item.Name) != null)
                    {
                        cPs.Add(item.Name + "Changed", item);
                    }
                }
            }
            foreach (var item in es)
            {
                if (cPs.ContainsKey(item.Name))
                {//过滤CPF内置的相同名称事件
                    continue;
                }
                var name = "on" + item.Name;
                events.Add(name);
                r.Commands.Add(item.Name, (s, e) =>
                {
                    if (handlerIds.TryGetValue(name, out var id))
                    {
                        Renderer.Dispatcher.InvokeAsync(() => Renderer.DispatchEventAsync(id, null, e as EventArgs));
                    }
                });
            }
            foreach (var item in cPs)
            {
                var name = "on" + item.Key;
                events.Add(name);
            }
            r.Commands.Add(nameof(r.PropertyChanged), (s, e) =>
            {//属性变换通知事件，给onXXXChanged
                var pe = (CPFPropertyChangedEventArgs)e;
                if (handlerIds.TryGetValue("on" + pe.PropertyName + "Changed", out var id))
                {
                    var newValue = pe.NewValue;
                    Renderer.Dispatcher.InvokeAsync(() =>
                    {
                        if (!handlerIds.ContainsValue(id))
                        {
                            return Task.CompletedTask;
                        }
                        return Renderer.DispatchEventAsync(id, null, new ChangeEventArgs { Value = newValue });
                    });
                }
            });

            return r;
        }
        /// <summary>
        /// 处理标签内部文字
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        public void HandleText(int index, string text)
        {
            if (Element is CPF.Controls.ContentControl control)
            {
                control.Content = text;
            }
        }
        /// <summary>
        /// 实现Razor控件自动转换成CPF控件
        /// </summary>
        /// <param name="element"></param>
        public static implicit operator T(Element<T> element)
        {
            return element.Element;
        }


        ////只要属性和事件自动生成就行
        //[Parameter] public string Name { get; set; }
        //[Parameter] public FloatField? MarginLeft { get; set; }
        //[Parameter] public FloatField? MarginTop { get; set; }
        //[Parameter] public FloatField? MarginBottom { get; set; }
        //[Parameter] public FloatField? MarginRight { get; set; }
        //[Parameter] public CPF.FloatField? Width { get; set; }
        //[Parameter] public CPF.FloatField? Height { get; set; }
        //[Parameter] public EventCallback<MouseButtonEventArgs> MouseDown { get; set; }

    }


}
