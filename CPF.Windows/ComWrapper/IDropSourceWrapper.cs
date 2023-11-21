using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CPF.Windows.ComWrapper
{
    public unsafe class IDropSourceWrapper : IDropSource
    {
        public readonly IntPtr instance;

        public IDropSourceWrapper(IntPtr instance)
        {
            this.instance = instance;
            Marshal.AddRef(instance);
        }

        ~IDropSourceWrapper()
        {
            Marshal.Release(this.instance);
        }

        int IDropSource.QueryContinueDrag(int fEscapePressed, int grfKeyState)
        {
            var targetInterface = new System.Guid("00000121-0000-0000-C000-000000000046");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                int retVal;
                retVal = ((delegate* unmanaged<System.IntPtr, int, int, int>)vtbl[3])(thisPtr, fEscapePressed, grfKeyState);
                return (int)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        int IDropSource.GiveFeedback(int dwEffect)
        {
            var targetInterface = new System.Guid("00000121-0000-0000-C000-000000000046");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                int retVal;
                retVal = ((delegate* unmanaged<System.IntPtr, int, int>)vtbl[4])(thisPtr, dwEffect);
                return (int)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
    }
}
