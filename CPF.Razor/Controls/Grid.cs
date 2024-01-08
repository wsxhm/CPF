using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Razor.Controls
{
    public partial class Grid
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        protected override RenderFragment GetChild() => ChildContent;
    }
}
