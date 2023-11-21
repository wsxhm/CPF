using System;
using System.Collections.Generic;
using System.Text;
using CPF.Input;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using CPF.Drawing;
//using IOleDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace CPF.Windows
{
    class OleDataObject : Input.IDataObject
    {
        private IOleDataObject _wrapped;

        public OleDataObject(IOleDataObject wrapped)
        {
            _wrapped = wrapped;
        }

        public bool Contains(DataFormat dataFormat)
        {
            return GetDataFormatsCore().Any(df => df == dataFormat);
        }

        //public IEnumerable<string> GetDataFormats()
        //{
        //    return GetDataFormatsCore().Distinct();
        //}


        public object GetData(DataFormat dataFormat)
        {
            return GetDataFromOleHGLOBAL(dataFormat, DVASPECT.DVASPECT_CONTENT);
        }

        private object GetDataFromOleHGLOBAL(DataFormat format, DVASPECT aspect)
        {
            List<short> fs = new List<short>();
            fs.Add((short)ClipboardImpl.GetFormatId(format));
            if (format == DataFormat.Text)
            {
                fs.Add((short)UnmanagedMethods.ClipboardFormat.CF_TEXT);
                fs.Add((short)UnmanagedMethods.ClipboardFormat.CF_OEMTEXT);
            }
            foreach (var item in fs)
            {
                FORMATETC formatEtc = new FORMATETC();
                formatEtc.cfFormat = item;
                formatEtc.dwAspect = aspect;
                formatEtc.lindex = -1;
                formatEtc.tymed = TYMED.TYMED_HGLOBAL;
                if (_wrapped.QueryGetData(ref formatEtc) == 0)
                {
                    _wrapped.GetData(ref formatEtc, out STGMEDIUM medium);
                    try
                    {
                        if (medium.unionmember != IntPtr.Zero && medium.tymed == (int)TYMED.TYMED_HGLOBAL)
                        {
                            if (format == DataFormat.Text)
                                return ReadStringFromHGlobal(medium.unionmember);
                            if (format == DataFormat.FileNames)
                                return ReadFileNamesFromHGlobal(medium.unionmember);
                            if (format == DataFormat.Image)
                            {
                                IntPtr ptr = UnmanagedMethods.GlobalLock(medium.unionmember);
                                //默认是24位的图，需要绘制到32位的位图里
#if Net4
                                var bmp = (UnmanagedMethods.BITMAPINFOHEADER)Marshal.PtrToStructure(ptr, typeof(UnmanagedMethods.BITMAPINFOHEADER));
#else
                                var bmp = Marshal.PtrToStructure<UnmanagedMethods.BITMAPINFOHEADER>(ptr);
#endif
                                IntPtr screenDC = UnmanagedMethods.GetDC(IntPtr.Zero);
                                IntPtr memDc = UnmanagedMethods.CreateCompatibleDC(screenDC);
                                UnmanagedMethods.BITMAPINFOHEADER info = new UnmanagedMethods.BITMAPINFOHEADER();
                                info.biSize = (uint)Marshal.SizeOf(typeof(UnmanagedMethods.BITMAPINFOHEADER));
                                info.biBitCount = 32;
                                info.biHeight = bmp.biHeight;
                                info.biWidth = bmp.biWidth;
                                info.biPlanes = 1;
                                var hBitmap = UnmanagedMethods.CreateDIBSection(memDc, ref info, 0, out IntPtr ppvBits, IntPtr.Zero, 0);
                                var oldBits = UnmanagedMethods.SelectObject(memDc, hBitmap);//将位图载入上下文

                                _ = UnmanagedMethods.StretchDIBits(memDc, 0, 0, bmp.biWidth, bmp.biHeight, 0, 0, bmp.biWidth, bmp.biHeight, (ptr + 40), ref bmp, 0, (uint)TernaryRasterOperations.SRCCOPY);

                                var img = ClipboardImpl.ImageFormHBitmap(hBitmap);
                                //var img = new Bitmap(bmp.biWidth, Math.Abs(bmp.biHeight), bmp.biWidth * 4, PixelFormat.PRgba, ppvBits).Clone();

                                UnmanagedMethods.SelectObject(memDc, oldBits);
                                UnmanagedMethods.ReleaseDC(IntPtr.Zero, screenDC);
                                UnmanagedMethods.DeleteDC(memDc);
                                UnmanagedMethods.DeleteObject(hBitmap);
                                UnmanagedMethods.GlobalUnlock(medium.unionmember);
                                return img;
                            }
                            byte[] data = ReadBytesFromHGlobal(medium.unionmember);

                            if (format == DataFormat.Html)
                            {
                                var html= Encoding.UTF8.GetString(data);
                                if (!string.IsNullOrWhiteSpace(html))
                                {
                                    var start = html.IndexOf("<!--StartFragment");
                                    var end = html.IndexOf("<!--EndFragment");
                                    html = html.Substring(start + 17, end - start - 17);
                                    html = html.TrimStart('-', ' ', '>');
                                }
                                return html;
                            }
                            if (IsSerializedObject(data))
                            {
                                using (var ms = new MemoryStream(data))
                                {
                                    ms.Position = DataObject.SerializedObjectGUID.Length;
                                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                                    return binaryFormatter.Deserialize(ms);
                                }
                            }
                            return data;
                        }
                    }
                    finally
                    {
                        UnmanagedMethods.ReleaseStgMedium(ref medium);
                    }
                }
            }

            return null;
        }

        private bool IsSerializedObject(byte[] data)
        {
            if (data.Length < DataObject.SerializedObjectGUID.Length)
                return false;
            for (int i = 0; i < DataObject.SerializedObjectGUID.Length; i++)
                if (data[i] != DataObject.SerializedObjectGUID[i])
                    return false;
            return true;
        }

        private static IEnumerable<string> ReadFileNamesFromHGlobal(IntPtr hGlobal)
        {
            List<string> files = new List<string>();
            int fileCount = UnmanagedMethods.DragQueryFile(hGlobal, -1, null, 0);
            if (fileCount > 0)
            {
                for (int i = 0; i < fileCount; i++)
                {
                    int pathLen = UnmanagedMethods.DragQueryFile(hGlobal, i, null, 0);
                    StringBuilder sb = new StringBuilder(pathLen + 1);

                    if (UnmanagedMethods.DragQueryFile(hGlobal, i, sb, sb.Capacity) == pathLen)
                    {
                        files.Add(sb.ToString());
                    }
                }
            }
            return files;
        }

        private static string ReadStringFromHGlobal(IntPtr hGlobal)
        {
            IntPtr ptr = UnmanagedMethods.GlobalLock(hGlobal);
            try
            {
                return Marshal.PtrToStringAuto(ptr);
            }
            finally
            {
                UnmanagedMethods.GlobalUnlock(hGlobal);
            }
        }

        private static byte[] ReadBytesFromHGlobal(IntPtr hGlobal)
        {
            IntPtr source = UnmanagedMethods.GlobalLock(hGlobal);
            try
            {
                int size = (int)UnmanagedMethods.GlobalSize(hGlobal).ToInt64();
                byte[] data = new byte[size];
                Marshal.Copy(source, data, 0, size);
                return data;
            }
            finally
            {
                UnmanagedMethods.GlobalUnlock(hGlobal);
            }
        }

        private IEnumerable<DataFormat> GetDataFormatsCore()
        {
            var enumFormat = _wrapped.EnumFormatEtc(DATADIR.DATADIR_GET);
            if (enumFormat != null)
            {
                enumFormat.Reset();
                FORMATETC[] formats = new FORMATETC[1];
                int[] fetched = { 1 };
                while (fetched[0] > 0)
                {
                    fetched[0] = 0;
                    if (enumFormat.Next(1, formats, fetched) == 0 && fetched[0] > 0)
                    {
                        if (formats[0].ptd != IntPtr.Zero)
                            Marshal.FreeCoTaskMem(formats[0].ptd);

                        yield return ClipboardImpl.GetFormat(formats[0].cfFormat);
                    }
                }
            }
        }
    }

    [ComImport]
    [Guid("0000010E-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleDataObject
    {

        /// <summary>
        ///     Called by a data consumer to obtain data from a source data object.
        ///     The GetData method renders the data described in the specified FORMATETC
        ///     structure and transfers it through the specified STGMEDIUM structure.
        ///     The caller then assumes responsibility for releasing the STGMEDIUM structure.
        /// </summary>
        void GetData([In] ref FORMATETC format, out STGMEDIUM medium);

        /// <summary>
        ///     Called by a data consumer to obtain data from a source data object.
        ///     This method differs from the GetData method in that the caller must
        ///     allocate and free the specified storage medium.
        /// </summary>
        void GetDataHere([In] ref FORMATETC format, ref STGMEDIUM medium);

        /// <summary>
        ///     Determines whether the data object is capable of rendering the data
        ///     described in the FORMATETC structure. Objects attempting a paste or
        ///     drop operation can call this method before calling IDataObject::GetData
        ///     to get an indication of whether the operation may be successful.
        /// </summary>
        [PreserveSig]
        int QueryGetData([In] ref FORMATETC format);

        /// <summary>
        ///     Provides a standard FORMATETC structure that is logically equivalent to one that is more
        ///     complex. You use this method to determine whether two different
        ///     FORMATETC structures would return the same data, removing the need
        ///     for duplicate rendering.
        /// </summary>
        [PreserveSig]
        int GetCanonicalFormatEtc([In] ref FORMATETC formatIn, out FORMATETC formatOut);

        /// <summary>
        ///     Called by an object containing a data source to transfer data to
        ///     the object that implements this method.
        /// </summary>
        void SetData([In] ref FORMATETC formatIn, [In] ref STGMEDIUM medium, [MarshalAs(UnmanagedType.Bool)] bool release);

        /// <summary>
        ///     Creates an object for enumerating the FORMATETC structures for a
        ///     data object. These structures are used in calls to IDataObject::GetData
        ///     or IDataObject::SetData.
        /// </summary>
        IEnumFORMATETC EnumFormatEtc(DATADIR direction);

        /// <summary>
        ///     Called by an object supporting an advise sink to create a connection between
        ///     a data object and the advise sink. This enables the advise sink to be
        ///     notified of changes in the data of the object.
        /// </summary>
        [PreserveSig]
        int DAdvise([In] ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection);

        /// <summary>
        ///     Destroys a notification connection that had been previously set up.
        /// </summary>
        void DUnadvise(int connection);

        /// <summary>
        ///     Creates an object that can be used to enumerate the current advisory connections.
        /// </summary>
        [PreserveSig]
        int EnumDAdvise(out IEnumSTATDATA enumAdvise);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STGMEDIUM
    {
        [MarshalAs(UnmanagedType.I4)]
        internal int tymed;
        internal IntPtr unionmember;
        internal IntPtr pUnkForRelease;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FORMATETC
    {
        [MarshalAs(UnmanagedType.U2)]
        public short cfFormat;
        public IntPtr ptd;
        [MarshalAs(UnmanagedType.U4)]
        public DVASPECT dwAspect;
        public int lindex;
        [MarshalAs(UnmanagedType.U4)]
        public TYMED tymed;
    }
    [ComImport()]
    [Guid("00000103-0000-0000-C000-000000000046")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumFORMATETC
    {

        /// <devdoc>
        ///     Retrieves the next celt items in the enumeration sequence. If there are 
        ///     fewer than the requested number of elements left in the sequence, it 
        ///     retrieves the remaining elements. The number of elements actually 
        ///     retrieved is returned through pceltFetched (unless the caller passed 
        ///     in NULL for that parameter).
        /// </devdoc>
        [PreserveSig]
        int Next(int celt, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] FORMATETC[] rgelt, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);

        /// <devdoc>
        ///     Skips over the next specified number of elements in the enumeration sequence.
        /// </devdoc>
        [PreserveSig]
        int Skip(int celt);

        /// <devdoc>
        ///     Resets the enumeration sequence to the beginning.
        /// </devdoc>
        [PreserveSig]
        int Reset();

        /// <devdoc>
        ///     Creates another enumerator that contains the same enumeration state as 
        ///     the current one. Using this function, a client can record a particular 
        ///     point in the enumeration sequence and then return to that point at a 
        ///     later time. The new enumerator supports the same interface as the original one.
        /// </devdoc>
        void Clone(out IEnumFORMATETC newEnum);
    }
}
