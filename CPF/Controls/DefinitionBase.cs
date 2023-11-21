using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    public abstract class DefinitionBase : CpfObject
    {
        internal abstract GridLength UserSizeValueCache { get; }
    }
}
