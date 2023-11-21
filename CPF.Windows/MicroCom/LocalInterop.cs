using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Windows.MicroCom
{
    unsafe class LocalInterop
    {
        public static unsafe void CalliStdCallvoid(void* thisObject, void* methodPtr)
        {
            throw null;
        }

        public static unsafe int CalliStdCallint(void* thisObject, Guid* guid, IntPtr* ppv, void* methodPtr)
        {
            throw null;
        }
    }
}
