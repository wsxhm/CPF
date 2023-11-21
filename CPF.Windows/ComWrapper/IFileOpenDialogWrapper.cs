using System;
using System.Runtime.InteropServices;
using CPF.Windows;

namespace CPF.Windows.ComWrapper
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    unsafe partial class IFileOpenDialogWrapper : IFileOpenDialog, IFileDialog
    {
        internal readonly IntPtr instance;

        public IFileOpenDialogWrapper(IntPtr instance)
        {
            this.instance = instance;
            Marshal.AddRef(instance);
        }

        ~IFileOpenDialogWrapper()
        {
            Marshal.Release(this.instance);
        }


        uint IFileOpenDialog.Show(global::System.IntPtr parent)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
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
                retVal = ((delegate* unmanaged<System.IntPtr, global::System.IntPtr, int>)vtbl[3])(thisPtr, parent);
                return (uint)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileOpenDialog.SetFileTypes(uint cFileTypes, COMDLG_FILTERSPEC[] rgFilterSpec)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
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
                System.Span<COMDLG_FILTERSPEC_native> local_1_arr = stackalloc COMDLG_FILTERSPEC_native[rgFilterSpec.Length == 0 ? 1 : rgFilterSpec.Length];
                for (int local_1_cnt = 0; local_1_cnt < rgFilterSpec.Length; local_1_cnt++)
                {
                    var arrayItem = rgFilterSpec[local_1_cnt];
                    COMDLG_FILTERSPEC_native local_1_0 = default;
                    var local_1_0_pszName = arrayItem.pszName;
                    var local_1_0_0 = Marshal.StringToCoTaskMemUni(local_1_0_pszName);
                    local_1_0.pszName = local_1_0_0;
                    var local_1_0_pszSpec = arrayItem.pszSpec;
                    var local_1_0_1 = Marshal.StringToCoTaskMemUni(local_1_0_pszSpec);
                    local_1_0.pszSpec = local_1_0_1;
                    local_1_arr[local_1_cnt] = local_1_0;
                }

                fixed (COMDLG_FILTERSPEC_native* local_1 = local_1_arr)
                    retVal = ((delegate* unmanaged<System.IntPtr, uint, COMDLG_FILTERSPEC_native*, int>)vtbl[4])(thisPtr, cFileTypes, local_1);
                return (uint)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.SetFileTypeIndex([In] uint iFileType)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
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
                retVal = ((delegate* unmanaged<System.IntPtr, uint, int>)vtbl[5])(thisPtr, iFileType);
                //return (uint)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.GetFileTypeIndex(out uint piFileType)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                fixed (uint* local_0 = &piFileType)
                    result = ((delegate* unmanaged<System.IntPtr, uint*, int>)vtbl[6])(thisPtr, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileOpenDialog.Advise(IntPtr pfde, out uint pdwCookie)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                if (pfde == IntPtr.Zero)
                {
                    local_0 = System.IntPtr.Zero;
                }
                else
                {
                    //var local_0_unk = Marshal.GetIUnknownForObject(pfde);
                    var local_0_unk = pfde;
                    var local_pfde_IID = new System.Guid("973510DB-7D7F-452B-8975-74A85828D354");
                    result = Marshal.QueryInterface(local_0_unk, ref local_pfde_IID, out local_0);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                fixed (uint* local_1 = &pdwCookie)
                    result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, uint*, int>)vtbl[7])(thisPtr, local_0, local_1);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
                return (uint)result;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.Unadvise(uint dwCookie)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                result = ((delegate* unmanaged<System.IntPtr, uint, int>)vtbl[8])(thisPtr, dwCookie);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileOpenDialog.SetOptions(uint fos)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                result = ((delegate* unmanaged<System.IntPtr, int, int>)vtbl[9])(thisPtr, (int)fos);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
                return (uint)result;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileOpenDialog.GetOptions(out uint pfos)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                int local_0;
                result = ((delegate* unmanaged<System.IntPtr, int*, int>)vtbl[10])(thisPtr, &local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                pfos = (uint)local_0;
                return (uint)result;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.SetDefaultFolder(IShellItem psi)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                if (psi == null)
                {
                    local_0 = System.IntPtr.Zero;
                }
                else
                {
                    var local_0_unk = Marshal.GetIUnknownForObject(psi);
                    var local_psi_IID = new System.Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE");
                    result = Marshal.QueryInterface(local_0_unk, ref local_psi_IID, out local_0);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, int>)vtbl[11])(thisPtr, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.SetFolder(IShellItem psi)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                if (psi == null)
                {
                    local_0 = System.IntPtr.Zero;
                }
                else
                {
                    var local_0_unk = Marshal.GetIUnknownForObject(psi);
                    var local_psi_IID = new System.Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE");
                    result = Marshal.QueryInterface(local_0_unk, ref local_psi_IID, out local_0);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, int>)vtbl[12])(thisPtr, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.GetFolder(out IShellItem ppsi)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr*, int>)vtbl[13])(thisPtr, &local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                ppsi = local_0 == System.IntPtr.Zero ? null : (IShellItem)Marshal.GetObjectForIUnknown(local_0);
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.GetCurrentSelection(out IShellItem ppsi)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                int retVal;
                retVal = ((delegate* unmanaged<System.IntPtr, System.IntPtr*, int>)vtbl[14])(thisPtr, &local_0);
                ppsi = local_0 == System.IntPtr.Zero ? null : (IShellItem)Marshal.GetObjectForIUnknown(local_0);
                //return (uint)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.SetFileName(string pszName)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                var local_0 = Marshal.StringToCoTaskMemUni(pszName);
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, int>)vtbl[15])(thisPtr, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                Marshal.FreeCoTaskMem(local_0);
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.GetFileName(out string pszName)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr*, int>)vtbl[16])(thisPtr, &local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                pszName = Marshal.PtrToStringUni(local_0);
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.SetTitle(string pszTitle)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                var local_0 = Marshal.StringToCoTaskMemUni(pszTitle);
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, int>)vtbl[17])(thisPtr, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                Marshal.FreeCoTaskMem(local_0);
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.SetOkButtonLabel(string pszText)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                var local_0 = Marshal.StringToCoTaskMemUni(pszText);
                int retVal;
                retVal = ((delegate* unmanaged<System.IntPtr, System.IntPtr, int>)vtbl[18])(thisPtr, local_0);
                Marshal.FreeCoTaskMem(local_0);
                //return (uint)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.SetFileNameLabel(string pszLabel)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                var local_0 = Marshal.StringToCoTaskMemUni(pszLabel);
                int retVal;
                retVal = ((delegate* unmanaged<System.IntPtr, System.IntPtr, int>)vtbl[19])(thisPtr, local_0);
                Marshal.FreeCoTaskMem(local_0);
                //return (uint)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileOpenDialog.GetResult(out IShellItem ppsi)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr*, int>)vtbl[20])(thisPtr, &local_0);
                //if (result != 0)
                //{
                //    Marshal.ThrowExceptionForHR(result);
                //}

                ppsi = local_0 == System.IntPtr.Zero ? null : (IShellItem)Marshal.GetObjectForIUnknown(local_0);
                return (uint)result;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileOpenDialog.AddPlace(IShellItem psi, uint fdap)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                if (psi == null)
                {
                    local_0 = System.IntPtr.Zero;
                }
                else
                {
                    var local_0_unk = Marshal.GetIUnknownForObject(psi);
                    var local_psi_IID = new System.Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE");
                    result = Marshal.QueryInterface(local_0_unk, ref local_psi_IID, out local_0);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                int retVal;
                retVal = ((delegate* unmanaged<System.IntPtr, System.IntPtr, int, int>)vtbl[21])(thisPtr, local_0, (int)fdap);
                return (uint)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.SetDefaultExtension(string pszDefaultExtension)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                var local_0 = Marshal.StringToCoTaskMemUni(pszDefaultExtension);
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, int>)vtbl[22])(thisPtr, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                Marshal.FreeCoTaskMem(local_0);
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.Close(int hr)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                result = ((delegate* unmanaged<System.IntPtr, int, int>)vtbl[23])(thisPtr, hr);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.SetClientGuid(ref global::System.Guid guid)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                fixed (global::System.Guid* local_0 = &guid)
                    result = ((delegate* unmanaged<System.IntPtr, global::System.Guid*, int>)vtbl[24])(thisPtr, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.ClearClientData()
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
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
                retVal = ((delegate* unmanaged<System.IntPtr, int>)vtbl[25])(thisPtr);
                //return (uint)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.SetFilter(IntPtr pFilter)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
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
                retVal = ((delegate* unmanaged<System.IntPtr, global::System.IntPtr, int>)vtbl[26])(thisPtr, pFilter);
                //return (uint)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.GetResults(out IShellItemArray ppenum)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr*, int>)vtbl[27])(thisPtr, &local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                ppenum = local_0 == System.IntPtr.Zero ? null : (IShellItemArray)Marshal.GetObjectForIUnknown(local_0);
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileOpenDialog.GetSelectedItems(out IShellItemArray ppsai)
        {
            var targetInterface = new System.Guid("d57c7288-d4ad-4768-be02-9d969532d960");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                int retVal;
                retVal = ((delegate* unmanaged<System.IntPtr, System.IntPtr*, int>)vtbl[28])(thisPtr, &local_0);
                ppsai = local_0 == System.IntPtr.Zero ? null : (IShellItemArray)Marshal.GetObjectForIUnknown(local_0);
                //return (uint)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }


        uint IFileDialog.Show(global::System.IntPtr hwndOwner)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                uint retVal;
                retVal = ((delegate* unmanaged<System.IntPtr, global::System.IntPtr, uint>)vtbl[3])(thisPtr, hwndOwner);
                return (uint)retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.SetFileTypes(uint cFileTypes, COMDLG_FILTERSPEC[] rgFilterSpec)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                uint retVal;
                COMDLG_FILTERSPEC_native[] natives = new COMDLG_FILTERSPEC_native[rgFilterSpec.Length];
                for (int i = 0; i < rgFilterSpec.Length; i++)
                {
                    natives[i] = new COMDLG_FILTERSPEC_native { pszName = Marshal.StringToHGlobalUni(rgFilterSpec[i].pszName), pszSpec = Marshal.StringToHGlobalUni(rgFilterSpec[i].pszSpec) };
                }
                fixed (COMDLG_FILTERSPEC_native* local_1 = natives)
                    result = ((delegate* unmanaged<System.IntPtr, uint, COMDLG_FILTERSPEC_native*, uint*, int>)vtbl[4])(thisPtr, cFileTypes, local_1, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.SetFileTypeIndex(uint iFileType)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, uint, uint*, int>)vtbl[5])(thisPtr, iFileType, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.GetFileTypeIndex(out uint piFileType)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                uint retVal;
                fixed (uint* local_0 = &piFileType)
                    result = ((delegate* unmanaged<System.IntPtr, uint*, uint*, int>)vtbl[6])(thisPtr, local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.Advise(global::System.IntPtr pfde, out uint pdwCookie)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                uint retVal;
                fixed (uint* local_1 = &pdwCookie)
                    result = ((delegate* unmanaged<System.IntPtr, global::System.IntPtr, uint*, uint*, int>)vtbl[7])(thisPtr, pfde, local_1, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.Unadvise(uint dwCookie)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, uint, uint*, int>)vtbl[8])(thisPtr, dwCookie, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.SetOptions(uint fos)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, uint, uint*, int>)vtbl[9])(thisPtr, fos, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.GetOptions(out uint fos)
        {
            var targetInterface = CpfComWrappers.IID_IFileDialog;
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                uint retVal;
                fixed (uint* local_0 = &fos)
                    result = ((delegate* unmanaged<System.IntPtr, uint*, uint*, int>)vtbl[10])(thisPtr, local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        void IFileDialog.SetDefaultFolder(IShellItem psi)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                if (psi == null)
                {
                    local_0 = System.IntPtr.Zero;
                }
                else
                {
                    var local_0_unk = Marshal.GetIUnknownForObject(psi);
                    var local_psi_IID = new System.Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE");
                    result = Marshal.QueryInterface(local_0_unk, ref local_psi_IID, out local_0);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, int>)vtbl[11])(thisPtr, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.SetFolder(IShellItem psi)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                if (psi == null)
                {
                    local_0 = System.IntPtr.Zero;
                }
                else
                {
                    var local_0_unk = Marshal.GetIUnknownForObject(psi);
                    var local_psi_IID = new System.Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE");
                    result = Marshal.QueryInterface(local_0_unk, ref local_psi_IID, out local_0);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, uint*, int>)vtbl[12])(thisPtr, local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.GetFolder(out IShellItem ppsi)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr*, uint*, int>)vtbl[13])(thisPtr, &local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                ppsi = local_0 == System.IntPtr.Zero ? null : (IShellItem)Marshal.GetObjectForIUnknown(local_0);
                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.GetCurrentSelection(out IShellItem ppsi)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr*, uint*, int>)vtbl[14])(thisPtr, &local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                ppsi = local_0 == System.IntPtr.Zero ? null : (IShellItem)Marshal.GetObjectForIUnknown(local_0);
                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.SetFileName(string pszName)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                var local_0 = Marshal.StringToCoTaskMemUni(pszName);
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, uint*, int>)vtbl[15])(thisPtr, local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                Marshal.FreeCoTaskMem(local_0);
                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.GetFileName(out string pszName)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr*, uint*, int>)vtbl[16])(thisPtr, &local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                pszName = Marshal.PtrToStringUni(local_0);
                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.SetTitle(string pszTitle)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                var local_0 = Marshal.StringToCoTaskMemUni(pszTitle);
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, uint*, int>)vtbl[17])(thisPtr, local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                Marshal.FreeCoTaskMem(local_0);
                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.SetOkButtonLabel(string pszText)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                var local_0 = Marshal.StringToCoTaskMemUni(pszText);
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, uint*, int>)vtbl[18])(thisPtr, local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                Marshal.FreeCoTaskMem(local_0);
                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.SetFileNameLabel(string pszLabel)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                var local_0 = Marshal.StringToCoTaskMemUni(pszLabel);
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, uint*, int>)vtbl[19])(thisPtr, local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                Marshal.FreeCoTaskMem(local_0);
                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.GetResult(out IShellItem ppsi)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr*, uint*, int>)vtbl[20])(thisPtr, &local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                ppsi = local_0 == System.IntPtr.Zero ? null : (IShellItem)Marshal.GetObjectForIUnknown(local_0);
                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.AddPlace(IShellItem psi, uint fdap)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                System.IntPtr local_0;
                if (psi == null)
                {
                    local_0 = System.IntPtr.Zero;
                }
                else
                {
                    var local_0_unk = Marshal.GetIUnknownForObject(psi);
                    var local_psi_IID = new System.Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE");
                    result = Marshal.QueryInterface(local_0_unk, ref local_psi_IID, out local_0);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, uint, uint*, int>)vtbl[21])(thisPtr, local_0, fdap, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.SetDefaultExtension(string pszDefaultExtension)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                var local_0 = Marshal.StringToCoTaskMemUni(pszDefaultExtension);
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, System.IntPtr, uint*, int>)vtbl[22])(thisPtr, local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                Marshal.FreeCoTaskMem(local_0);
                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.Close(uint hr)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, uint, uint*, int>)vtbl[23])(thisPtr, hr, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.SetClientGuid(ref global::System.Guid guid)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                uint retVal;
                fixed (global::System.Guid* local_0 = &guid)
                    result = ((delegate* unmanaged<System.IntPtr, global::System.Guid*, uint*, int>)vtbl[24])(thisPtr, local_0, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.ClearClientData()
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, uint*, int>)vtbl[25])(thisPtr, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
        uint IFileDialog.SetFilter(global::System.IntPtr pFilter)
        {
            var targetInterface = new System.Guid("42F85136-DB7E-439C-85F1-E4075D135FC8");
            var result = Marshal.QueryInterface(this.instance, ref targetInterface, out var thisPtr);
            if (result != 0)
            {
                throw new System.InvalidCastException();
            }

            try
            {
                var comDispatch = (System.IntPtr*)thisPtr;
                var vtbl = (System.IntPtr*)comDispatch[0];
                uint retVal;
                result = ((delegate* unmanaged<System.IntPtr, global::System.IntPtr, uint*, int>)vtbl[26])(thisPtr, pFilter, &retVal);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                return retVal;
            }
            finally
            {
                Marshal.Release(thisPtr);
            }
        }
    }
}
