using Microsoft.AspNetCore.Components;
//using Microsoft.MobileBlazorBindings.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Razor.Controls
{
    public abstract class Element<T> : NativeControlComponentBase<T> where T : UIElement, new()
    {
        [Parameter] public string MarginLeft { get; set; }
        [Parameter] public string MarginTop { get; set; }
        [Parameter] public string Width { get; set; }
        [Parameter] public string Height { get; set; }

        //public CPF.UIElement NativeControl => ((ICpfElementHandler)ElementHandler).Element;

        protected override void RenderAttributes(AttributesBuilder builder)
        {
            base.RenderAttributes(builder);

            if (MarginLeft != null)
            {
                builder.AddAttribute(nameof(MarginLeft), MarginLeft);
            }
            if (MarginTop != null)
            {
                builder.AddAttribute(nameof(MarginTop), MarginTop);
            }
            if (Height != null)
            {
                builder.AddAttribute(nameof(Height), Height);
            }
            if (Width != null)
            {
                builder.AddAttribute(nameof(Width), Width);
            }
        }
    }
}
