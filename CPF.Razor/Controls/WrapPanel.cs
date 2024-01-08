using Microsoft.AspNetCore.Components;
//using Microsoft.MobileBlazorBindings.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Razor.Controls
{
    public partial class WrapPanel
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        protected override RenderFragment GetChild() => ChildContent;
    }
}
