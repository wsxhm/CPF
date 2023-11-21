using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace CPF.Windows.ComWrapper
{
    unsafe class IDropTargetWrapper : IDropTarget
    {
        public readonly IntPtr instance;

        public IDropTargetWrapper(IntPtr instance)
        {
            this.instance = instance;
            Marshal.AddRef(instance);
        }

        ~IDropTargetWrapper()
        {
            Marshal.Release(this.instance);
        }

        HRESULT IDropTarget.DragEnter(CPF.Windows.IOleDataObject pDataObj, uint grfKeyState, POINT pt, ref uint pdwEffect)
        {
            var targetInterface = new System.Guid("00000122-0000-0000-C000-000000000046");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                var local_0 = pDataObj == null ? System.IntPtr.Zero : Marshal.GetIUnknownForObject(pDataObj);
                int retVal = ((delegate* unmanaged<System.IntPtr, System.IntPtr, uint, POINT, ref uint, int>)vtbl[3])(thisPtr, local_0, grfKeyState, pt, ref pdwEffect);
                return (HRESULT)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        HRESULT IDropTarget.DragOver(uint grfKeyState, POINT pt, ref uint pdwEffect)
        {
            var targetInterface = new System.Guid("00000122-0000-0000-C000-000000000046");
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
                    retVal = ((delegate* unmanaged<System.IntPtr, uint, POINT, ref uint, int>)vtbl[4])(thisPtr, grfKeyState, pt, ref pdwEffect);
                return (HRESULT)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        HRESULT IDropTarget.DragLeave()
        {
            var targetInterface = new System.Guid("00000122-0000-0000-C000-000000000046");
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
                retVal = ((delegate* unmanaged<System.IntPtr, int>)vtbl[5])(thisPtr);
                return (HRESULT)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        HRESULT IDropTarget.Drop(IOleDataObject pDataObj, uint grfKeyState, POINT pt, ref uint pdwEffect)
        {
            var targetInterface = new System.Guid("00000122-0000-0000-C000-000000000046");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                var local_0 = pDataObj == null ? System.IntPtr.Zero : Marshal.GetIUnknownForObject(pDataObj);
                int retVal;
                    retVal = ((delegate* unmanaged<System.IntPtr, System.IntPtr, uint, POINT, ref uint, int>)vtbl[6])(thisPtr, local_0, grfKeyState, pt, ref pdwEffect);
                return (HRESULT)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }

    }
}
