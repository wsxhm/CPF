using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using static System.Runtime.InteropServices.ComWrappers;

namespace CPF.Windows.ComWrapper
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    unsafe partial class IDropTargetProxy
    {
        [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
        public static int DragEnter(IntPtr thisPtr, IntPtr pDataObj, uint grfKeyState, POINT pt, uint* pdwEffect)
        {
            try
            {
                var inst = ComInterfaceDispatch.GetInstance<IDropTarget>((ComInterfaceDispatch*)thisPtr);
                var local_0 = pDataObj == System.IntPtr.Zero ? null : (object)Marshal.GetObjectForIUnknown(pDataObj);
                var effect = pdwEffect[0];
                var r = (int)inst.DragEnter((IOleDataObject)local_0, grfKeyState, pt, ref effect);
                pdwEffect[0] = effect;
                return r;
            }
            catch (System.Exception __e)
            {
                Console.WriteLine(__e);
                return __e.HResult;
            }
        }
        [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
        public static int DragOver(System.IntPtr thisPtr, uint grfKeyState, POINT pt, uint* pdwEffect)
        {
            try
            {
                var effect = pdwEffect[0];
                var inst = ComInterfaceDispatch.GetInstance<IDropTarget>((ComInterfaceDispatch*)thisPtr);
                var r = (int)inst.DragOver(grfKeyState, pt, ref effect);
                pdwEffect[0] = effect;
                return r;
            }
            catch (System.Exception __e)
            {
                Console.WriteLine(__e);
                return __e.HResult;
            }
        }
        [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
        public static int DragLeave(System.IntPtr thisPtr)
        {
            try
            {
                var inst = ComInterfaceDispatch.GetInstance<IDropTarget>((ComInterfaceDispatch*)thisPtr);
                return (int)inst.DragLeave();
            }
            catch (System.Exception __e)
            {
                Console.WriteLine(__e);
                return __e.HResult;
            }
        }
        [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
        public static int Drop(System.IntPtr thisPtr, System.IntPtr pDataObj, uint grfKeyState, POINT pt, uint* pdwEffect)
        {
            try
            {
                var inst = ComInterfaceDispatch.GetInstance<IDropTarget>((ComInterfaceDispatch*)thisPtr);
                //var local_0 = pDataObj == System.IntPtr.Zero ? null : (object)Marshal.GetObjectForIUnknown(pDataObj);
                var local_0 = pDataObj == System.IntPtr.Zero ? null : CpfComWrappers.Instance.GetOrCreateObjectForComInstance(pDataObj, CreateObjectFlags.None);
                var effect = pdwEffect[0];
                var r = (int)inst.Drop((IOleDataObject)local_0, grfKeyState, pt, ref effect);
                pdwEffect[0] = effect;
                return r;
            }
            catch (System.Exception __e)
            {
                Console.WriteLine(__e);
                return __e.HResult;
            }
        }
    }
}
