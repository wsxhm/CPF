using Microsoft.AspNetCore.Components;
//using Microsoft.MobileBlazorBindings.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Razor.Controls
{
    /// <summary>
    /// 测试
    /// </summary>
    public partial class Panel : Element<CPF.Controls.Panel>
    {
        [Parameter] public string Background { get; set; }

#pragma warning disable CA1721 // Property names should not match get methods
        [Parameter] public RenderFragment ChildContent { get; set; }
#pragma warning restore CA1721 // Property names should not match get methods
        protected override RenderFragment GetChildContent() => ChildContent;
    }
}
