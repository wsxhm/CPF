using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using ComInterfaceDispatch = System.Runtime.InteropServices.ComWrappers.ComInterfaceDispatch;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace CPF.Windows.ComWrapper
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    unsafe class CpfComWrappers : ComWrappers
    {
        static ComInterfaceEntry* DropSourceEntry;
        static ComInterfaceEntry* StreamEntry;
        static ComInterfaceEntry* DropTargetEntry;
        static ComInterfaceEntry* DataObjectEntry;
        static ComInterfaceEntry* EnumFORMATETCEntry;

        internal static Guid IID_IShellItemArray = new Guid("B63EA76D-1F85-456F-A19C-48159EFA858B");
        internal static Guid IID_ShellItem = new Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE");
        internal static Guid IID_IFileDialog = new Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
        internal static Guid IID_IDropSource = new Guid("00000121-0000-0000-C000-000000000046");
        internal static Guid IID_IDropTarget = new Guid("00000122-0000-0000-C000-000000000046");
        internal static Guid IID_IStream = new Guid("0000000C-0000-0000-C000-000000000046");
        internal static Guid IID_IFileOpenDialog = new Guid("d57c7288-d4ad-4768-be02-9d969532d960");
        internal static Guid IID_IFileSaveDialog = new Guid("84bccd23-5fde-4cdb-aea4-af64b83d78ab");
        internal static Guid IID_IDataObject = new Guid("0000010E-0000-0000-C000-000000000046");
        internal static Guid IID_IEnumFORMATETC = new Guid("00000103-0000-0000-C000-000000000046");

        static CpfComWrappers()
        {
            DropSourceEntry = CreateDropSourceEntry();
            DropTargetEntry = CreateDropTargetEntry();
            StreamEntry = CreateDrawingStreamEntry();
            DataObjectEntry = CreateDataObjectEntry();
            EnumFORMATETCEntry = CreateIEnumFORMATETCProxyVtblEntry();
        }
        public static CpfComWrappers Instance { get; } = new CpfComWrappers();

        private static ComInterfaceEntry* CreateDropSourceEntry()
        {
            CreateIDropSourceProxyVtbl(out var vtbl);

            var comInterfaceEntryMemory = RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(CpfComWrappers), sizeof(ComInterfaceEntry) * 1);
            var wrapperEntry = (ComInterfaceEntry*)comInterfaceEntryMemory.ToPointer();
            wrapperEntry->IID = IID_IDropSource;
            wrapperEntry->Vtable = vtbl;
            return wrapperEntry;
        }
        internal static void CreateIDropSourceProxyVtbl(out System.IntPtr vtbl)
        {
            var vtblRaw = (System.IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(CpfComWrappers), sizeof(System.IntPtr) * 5);
            GetIUnknownImpl(out vtblRaw[0], out vtblRaw[1], out vtblRaw[2]);

            vtblRaw[3] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, int, int, int>)&IDropSourceProxy.QueryContinueDrag;
            vtblRaw[4] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, int, int>)&IDropSourceProxy.GiveFeedback;

            vtbl = (System.IntPtr)vtblRaw;
        }

        private static ComInterfaceEntry* CreateDropTargetEntry()
        {
            CreatePrimitivesIDropTargetProxyVtbl(out var vtbl);

            var comInterfaceEntryMemory = RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(CpfComWrappers), sizeof(ComInterfaceEntry) * 1);
            var wrapperEntry = (ComInterfaceEntry*)comInterfaceEntryMemory.ToPointer();
            wrapperEntry->IID = IID_IDropTarget;
            wrapperEntry->Vtable = vtbl;
            return wrapperEntry;
        }

        internal static void CreatePrimitivesIDropTargetProxyVtbl(out System.IntPtr vtbl)
        {
            var vtblRaw = (System.IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(CpfComWrappers), sizeof(System.IntPtr) * 7);
            GetIUnknownImpl(out vtblRaw[0], out vtblRaw[1], out vtblRaw[2]);

            vtblRaw[3] = (System.IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, uint, POINT, uint*, int>)&IDropTargetProxy.DragEnter;
            vtblRaw[4] = (System.IntPtr)(delegate* unmanaged[Stdcall]<System.IntPtr, uint, POINT, uint*, int>)&IDropTargetProxy.DragOver;
            vtblRaw[5] = (System.IntPtr)(delegate* unmanaged[Stdcall]<System.IntPtr, int>)&IDropTargetProxy.DragLeave;
            vtblRaw[6] = (System.IntPtr)(delegate* unmanaged[Stdcall]<System.IntPtr, System.IntPtr, uint, POINT, uint*, int>)&IDropTargetProxy.Drop;

            vtbl = (System.IntPtr)vtblRaw;
        }

        private static ComInterfaceEntry* CreateDrawingStreamEntry()
        {
            GetIUnknownImpl(out IntPtr fpQueryInterface, out IntPtr fpAddRef, out IntPtr fpRelease);

            IntPtr iStreamVtbl = IStreamVtbl.Create(fpQueryInterface, fpAddRef, fpRelease);

            ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(CpfComWrappers), sizeof(ComInterfaceEntry));
            wrapperEntry->IID = IID_IStream;
            wrapperEntry->Vtable = iStreamVtbl;
            return wrapperEntry;
        }
        private static ComInterfaceEntry* CreateDataObjectEntry()
        {
            CreateIDataObjectProxyVtbl(out var vtbl);

            var comInterfaceEntryMemory = RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(CpfComWrappers), sizeof(ComInterfaceEntry) * 1);
            var wrapperEntry = (ComInterfaceEntry*)comInterfaceEntryMemory;
            wrapperEntry->IID = IID_IDataObject;
            wrapperEntry->Vtable = vtbl;
            return wrapperEntry;
        }

        internal static void CreateIDataObjectProxyVtbl(out System.IntPtr vtbl)
        {
            var vtblRaw = (System.IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(CpfComWrappers), sizeof(System.IntPtr) * 12);
            GetIUnknownImpl(out vtblRaw[0], out vtblRaw[1], out vtblRaw[2]);

            vtblRaw[3] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, FORMATETC*, STGMEDIUM*, int>)&IDataObjectProxy.GetData;
            vtblRaw[4] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, FORMATETC*, STGMEDIUM*, int>)&IDataObjectProxy.GetDataHere;
            vtblRaw[5] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, FORMATETC*, int>)&IDataObjectProxy.QueryGetData;
            vtblRaw[6] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, FORMATETC*, FORMATETC*, int>)&IDataObjectProxy.GetCanonicalFormatEtc;
            vtblRaw[7] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, FORMATETC*, STGMEDIUM*, bool, int>)&IDataObjectProxy.SetData;
            vtblRaw[8] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, int, System.IntPtr*, int>)&IDataObjectProxy.EnumFormatEtc;
            vtblRaw[9] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, FORMATETC*, int, System.IntPtr, int*, int>)&IDataObjectProxy.DAdvise;
            vtblRaw[10] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, int, int>)&IDataObjectProxy.DUnadvise;
            vtblRaw[11] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, System.IntPtr*, int>)&IDataObjectProxy.EnumDAdvise;


            vtbl = (System.IntPtr)vtblRaw;
        }
        private static ComInterfaceEntry* CreateIEnumFORMATETCProxyVtblEntry()
        {
            CreateIEnumFORMATETCProxyVtbl(out var vtbl);

            var comInterfaceEntryMemory = RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(CpfComWrappers), sizeof(ComInterfaceEntry) * 1);
            var wrapperEntry = (ComInterfaceEntry*)comInterfaceEntryMemory;
            wrapperEntry->IID = IID_IEnumFORMATETC;
            wrapperEntry->Vtable = vtbl;
            return wrapperEntry;
        }
        internal static void CreateIEnumFORMATETCProxyVtbl(out System.IntPtr vtbl)
        {
            var vtblRaw = (System.IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(CpfComWrappers), sizeof(System.IntPtr) * 7);
            GetIUnknownImpl(out vtblRaw[0], out vtblRaw[1], out vtblRaw[2]);

            vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, int, FORMATETC*, int*, int>)&IEnumFORMATETCProxy.Next;
            vtblRaw[4] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, int, int>)&IEnumFORMATETCProxy.Skip;
            vtblRaw[5] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, int>)&IEnumFORMATETCProxy.Reset;
            vtblRaw[6] = (System.IntPtr)(delegate* unmanaged<System.IntPtr, System.IntPtr*, int>)&IEnumFORMATETCProxy.Clone;

            vtbl = (System.IntPtr)vtblRaw;
        }

        protected override void ReleaseObjects(System.Collections.IEnumerable objects)
        {
        }

        protected override unsafe ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count)
        {
            if (obj is IDropSource)
            {
                count = 1;
                return DropSourceEntry;
            }
            if (obj is IDropTarget)
            {
                count = 1;
                return DropTargetEntry;
            }
            if (obj is IStream)
            {
                count = 1;
                return StreamEntry;
            }
            if (obj is IOleDataObject)
            {
                count = 1;
                return DataObjectEntry;
            }
            if (obj is IEnumFORMATETC)
            {
                count = 1;
                return EnumFORMATETCEntry;
            }
            throw new NotImplementedException();
        }

        protected override object CreateObject(IntPtr externalComObject, CreateObjectFlags flags)
        {
            if (Marshal.QueryInterface(externalComObject, ref IID_IDropSource, out var objPtr) >= 0)
            {
                Marshal.Release(objPtr);
                return new IDropSourceWrapper(externalComObject);
            }

            if (Marshal.QueryInterface(externalComObject, ref IID_IDropTarget, out objPtr) >= 0)
            {
                Marshal.Release(objPtr);
                return new IDropTargetWrapper(externalComObject);
            }
            if (Marshal.QueryInterface(externalComObject, ref IID_IStream, out objPtr) >= 0)
            {
                Marshal.Release(objPtr);
                return new IStreamWrapper(externalComObject);
            }
            if (Marshal.QueryInterface(externalComObject, ref IID_IFileOpenDialog, out var fileOpenDialogPtr) >= 0)
            {
                Marshal.Release(fileOpenDialogPtr);
                return new IFileOpenDialogWrapper(externalComObject);
            }
            if (Marshal.QueryInterface(externalComObject, ref IID_IFileDialog, out objPtr) >= 0)
            {
                Marshal.Release(objPtr);
                return new IFileDialogWrapper(externalComObject);
            }
            if (Marshal.QueryInterface(externalComObject, ref IID_ShellItem, out objPtr) >= 0)
            {
                Marshal.Release(objPtr);
                return new IShellItemWrapper(externalComObject);
            }
            if (Marshal.QueryInterface(externalComObject, ref IID_IShellItemArray, out objPtr) >= 0)
            {
                Marshal.Release(objPtr);
                return new IShellItemArrayWrapper(externalComObject);
            }

            if (Marshal.QueryInterface(externalComObject, ref IID_IDataObject, out objPtr) >= 0)
            {
                Marshal.Release(objPtr);
                return new IOleDataObjectWrapper(externalComObject);
            }

            if (Marshal.QueryInterface(externalComObject, ref IID_IEnumFORMATETC, out objPtr) >= 0)
            {
                Marshal.Release(objPtr);
                return new IEnumFORMATETCWrapper(externalComObject);
            }

            throw new NotImplementedException();
        }
    }



}
