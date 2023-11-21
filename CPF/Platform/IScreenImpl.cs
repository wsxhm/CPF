using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Platform
{
    public interface IScreenImpl
    {
        Rect Bounds { get; }

        Rect WorkingArea { get; }

        bool Primary { get; }

        //IReadOnlyList<Screen> GetAllScreens();
    }
}
