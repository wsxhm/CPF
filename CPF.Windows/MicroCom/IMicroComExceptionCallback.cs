using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Windows.MicroCom
{
    public interface IMicroComExceptionCallback
    {
        void RaiseException(Exception e);
    }
}
