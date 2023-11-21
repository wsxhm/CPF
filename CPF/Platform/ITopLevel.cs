using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Platform
{
    /// <summary>
    /// 顶级窗口
    /// </summary>
    public interface ITopLevel
    {
        Screen Screen { get; }

        float RenderScaling { get; }

        float LayoutScaling { get; }
    }
}
