﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace CPF.Razor
{
    public static class CpfExtensions
    {
        /// <summary>
        /// 创建视图 <typeparamref name="TComponent"/> 添加到 <paramref name="parent"/>.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="host"></param>
        /// <param name="parent"></param>
        public static void AddComponent<TComponent>(this IHost host, CPF.UIElement parent) where TComponent : IComponent
        {
            if (host is null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (parent is null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            var services = host.Services;
            var renderer = new CpfRenderer(services, services.GetRequiredService<ILoggerFactory>());

            //// TODO: This call is an async call, but is called as "fire-and-forget," which is not ideal.
            //// We need to figure out how to get Xamarin.Forms to run this startup code asynchronously, which
            //// is how this method should be called.
            renderer.AddComponent<TComponent>(new ElementHandler(renderer, parent)).ConfigureAwait(false);
        }
    }
}
