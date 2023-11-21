using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using static System.Runtime.InteropServices.ComWrappers;

namespace CPF.Windows.ComWrapper
{
    public readonly struct IStreamWrapper : IDisposable
    {
        // Token: 0x06000C19 RID: 3097 RVA: 0x00099D8C File Offset: 0x00098F8C
        public IStreamWrapper(IntPtr ptr)
        {
            this.Ptr = ptr;
        }

        // Token: 0x06000C1A RID: 3098 RVA: 0x00099D98 File Offset: 0x00098F98
        public void Dispose()
        {
            Marshal.Release(this.Ptr);
        }

        // Token: 0x040007F7 RID: 2039
        public readonly IntPtr Ptr;
    }
}
