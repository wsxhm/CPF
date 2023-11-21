using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF.Drawing;

namespace CPF.Skia
{
    public class SkiaFont : IDisposable
    {
        public string FontFamily { get; set; }

        public float FontSize { get; set; }

        public FontStyles FontStyle { get; set; }

        public void Dispose()
        {
            
        }
    }
}
