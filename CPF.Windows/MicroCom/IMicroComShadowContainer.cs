using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Windows.MicroCom
{
    public interface IMicroComShadowContainer
    {
        MicroComShadow Shadow { get; set; }
        void OnReferencedFromNative();
        void OnUnreferencedFromNative();
    }
}
