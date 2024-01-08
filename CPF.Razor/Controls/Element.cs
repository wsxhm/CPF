using CPF.Input;
using Microsoft.AspNetCore.Components;
//using Microsoft.MobileBlazorBindings.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Razor.Controls
{
    public abstract partial class Element<T> : NativeControlComponentBase<T> where T : UIElement, new()
    {
        protected override void RenderAttributes(AttributesBuilder builder)
        {
            base.RenderAttributes(builder);

            var type = GetType();
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
                            builder.AddAttribute("on" + item.Name, v);
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
            //if (MarginTop != null)
            //{
            //    builder.AddAttribute(nameof(MarginTop), MarginTop);
            //}
            //if (Height != null)
            //{
            //    builder.AddAttribute(nameof(Height), Height);
            //}
            //if (Width != null)
            //{
            //    builder.AddAttribute(nameof(Width), Width);
            //}
        }
        public override void ApplyAttribute(ulong attributeEventHandlerId, string attributeName, object attributeValue, string attributeEventUpdatesAttributeName)
        {
            var p = Element.GetPropertyMetadata(attributeName);
            if (p != null)
            {
                Element.SetValue(attributeValue.ConvertTo(p.PropertyType), attributeName);
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
        HashSet<string> events = new HashSet<string>();

        protected override T CreateElement()
        {
            var r = base.CreateElement();
            var type = typeof(T);
            var ps = type.GetEvents();
            foreach (var item in ps)
            {
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

            return r;
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
