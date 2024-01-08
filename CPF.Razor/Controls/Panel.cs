using Microsoft.AspNetCore.Components;
//using Microsoft.MobileBlazorBindings.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Razor.Controls
{
    public partial class Panel : Element<CPF.Controls.Panel>
    {
        //static Panel()
        //{
        //    ElementHandlerRegistry.RegisterElementHandler<Panel>();
        //}

        [Parameter] public string Background { get; set; }

#pragma warning disable CA1721 // Property names should not match get methods
        [Parameter] public RenderFragment ChildContent { get; set; }
#pragma warning restore CA1721 // Property names should not match get methods
        protected override void RenderAttributes(AttributesBuilder builder)
        {
            base.RenderAttributes(builder);

            if (Background != null)
            {
                builder.AddAttribute(nameof(Background), Background);
            }
        }
        protected override RenderFragment GetChildContent() => ChildContent;

        public override void ApplyAttribute(ulong attributeEventHandlerId, string attributeName, object attributeValue, string attributeEventUpdatesAttributeName)
        {
            //switch (attributeName)
            //{
            //    //case nameof(AutoScroll):
            //    //    AutoScroll = AttributeHelper.GetBool(attributeValue);
            //    //    break;
            //    default:

            //        break;
            //}
            var p = Element.GetPropertyMetadata(attributeName);
            if (p != null)
            {
                Element.SetValue(attributeValue.ConvertTo(p.PropertyType), attributeName);
            }
        }
    }
}
