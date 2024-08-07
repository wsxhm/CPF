﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace CPF.BlazorBindings
{
    internal class ElementHandlerFactoryContext
    {
        public ElementHandlerFactoryContext(NativeComponentRenderer renderer, IElementHandler parentHandler)
        {
            Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            ParentHandler = parentHandler;
        }

        public IElementHandler ParentHandler { get; }

        public NativeComponentRenderer Renderer { get; }
    }
}
