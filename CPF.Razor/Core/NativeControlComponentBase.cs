// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;

namespace CPF.Razor
{
    public abstract class NativeControlComponentBase<T> : ComponentBase, ICpfElementHandler where T : UIElement, new()
    {
        //public IElementHandler ElementHandler { get; private set; }

        UIElement ICpfElementHandler.Element => Element;

        T element;
        public T Element
        {
            get
            {
                if (element == null)
                {
                    element = CreateElement();
                }
                return element;
            }
        }

        public object TargetElement => Element;

        NativeComponentRenderer _Renderer;
        public NativeComponentRenderer Renderer
        {
            get => _Renderer;
            set => _Renderer = value;
        }

        //public void SetElementReference(IElementHandler elementHandler)
        //{
        //    ElementHandler = elementHandler ?? throw new ArgumentNullException(nameof(elementHandler));
        //}

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.OpenElement(0, GetType().FullName);
            RenderAttributes(new AttributesBuilder(builder));

            var childContent = GetChildContent();
            if (childContent != null)
            {
                builder.AddContent(2, childContent);
            }

            builder.CloseElement();
        }

        protected virtual void RenderAttributes(AttributesBuilder builder)
        {
        }

        protected virtual RenderFragment GetChildContent() => null;

        public abstract void ApplyAttribute(ulong attributeEventHandlerId, string attributeName, object attributeValue, string attributeEventUpdatesAttributeName);

        protected virtual T CreateElement()
        {
            return new T();
        }
    }
}
