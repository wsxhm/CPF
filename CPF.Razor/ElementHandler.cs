using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Razor
{
    public class ElementHandler : ICpfElementHandler
    {
        public ElementHandler(NativeComponentRenderer renderer, CPF.UIElement elementControl)
        {
            Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            Element = elementControl ?? throw new ArgumentNullException(nameof(elementControl));
        }

        //protected void RegisterEvent(string eventName, Action<ulong> setId, Action<ulong> clearId)
        //{
        //    RegisteredEvents[eventName] = new EventRegistration(eventName, setId, clearId);
        //}
        //private Dictionary<string, EventRegistration> RegisteredEvents { get; } = new Dictionary<string, EventRegistration>();

        public NativeComponentRenderer Renderer { get; }
        public CPF.UIElement Element { get; }
        public object TargetElement => Element;

        NativeComponentRenderer ICpfElementHandler.Renderer { get; set; }

        public virtual void ApplyAttribute(ulong attributeEventHandlerId, string attributeName, object attributeValue, string attributeEventUpdatesAttributeName)
        {
            //switch (attributeName)
            //{
            //    case nameof(XF.Element.AutomationId):
            //        ElementControl.AutomationId = (string)attributeValue;
            //        break;
            //    case nameof(XF.Element.ClassId):
            //        ElementControl.ClassId = (string)attributeValue;
            //        break;
            //    case nameof(XF.Element.StyleId):
            //        ElementControl.StyleId = (string)attributeValue;
            //        break;
            //    default:
            //        if (!TryRegisterEvent(attributeName, attributeEventHandlerId))
            //        {
            //            throw new NotImplementedException($"{GetType().FullName} doesn't recognize attribute '{attributeName}'");
            //        }
            //        break;
            //}
            var p = Element.GetPropertyMetadata(attributeName);
            if (p != null)
            {
                Element.SetValue(attributeValue.ConvertTo(p.PropertyType), attributeName);
            }
        }

        //private bool TryRegisterEvent(string eventName, ulong eventHandlerId)
        //{
        //    if (RegisteredEvents.TryGetValue(eventName, out var eventRegistration))
        //    {
        //        Renderer.RegisterEvent(eventHandlerId, eventRegistration.ClearId);
        //        eventRegistration.SetId(eventHandlerId);

        //        return true;
        //    }
        //    return false;
        //}
    }
}
