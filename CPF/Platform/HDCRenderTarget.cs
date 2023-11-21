using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Platform
{
    public class HDCRenderTarget : IRenderTarget
    {
        public HDCRenderTarget(IntPtr hdc, int w, int h, bool gpu = false)
        {
            Hdc = hdc;
            Height = h;
            Width = w;
            CanUseGPU = gpu;
        }

        public int Height
        {
            get;
        }

        public int Width
        {
            get;
        }

        public IntPtr Hdc
        {
            get;
        }


        public bool CanUseGPU
        {
            get;
        }
    }
}
