// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

//using Microsoft.MobileBlazorBindings.Core;

namespace CPF.Razor
{
    public interface ICpfElementHandler : IElementHandler
    {
        UIElement Element { get; }
        NativeComponentRenderer Renderer { get; set; }
    }
}
