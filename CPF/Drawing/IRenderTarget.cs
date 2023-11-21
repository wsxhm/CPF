using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    /// <summary>
    /// 定义一个渲染目标
    /// </summary>
    public interface IRenderTarget
    {
        //object Target { get; }

        int Width { get; }

        int Height { get; }

        bool CanUseGPU { get; }
    }
}
