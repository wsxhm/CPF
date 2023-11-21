using CPF.Input;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using CPF.Drawing;
//using IOleDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace CPF.Windows
{
    class DataObject : IOleDataObject
    {
        // Compatibility with WinForms + WPF...
        internal static readonly byte[] SerializedObjectGUID = new Guid("FD9EA796-3B13-4370-A679-56106BB288FB").ToByteArray();

        class FormatEnumerator : IEnumFORMATETC
        {
            internal DataObject parent = null;
            private FORMATETC[] _formats;
            private int _current;

            private FormatEnumerator(FORMATETC[] formats, int current)
            {
                _formats = formats;
                _current = current;
            }

            public FormatEnumerator(DataObject dataobj)
            {
                parent = dataobj;
                List<DataFormat> dataFormats = dataobj.datas.Select(a => a.Key).ToList();
                var fs = dataFormats.Select(ConvertToFormatEtc).ToList();
                if (fs.Any(a => a.cfFormat == 8))
                {
                    var f = ConvertToFormatEtc(DataFormat.Image);
                    f.cfFormat = 2;
                    fs.Add(f);
                }
                _formats = fs.ToArray();
                _current = 0;
            }

            private FORMATETC ConvertToFormatEtc(DataFormat aFormatName)
            {
                FORMATETC result = new FORMATETC();
                result.cfFormat = unchecked((short)ClipboardImpl.GetFormatId(aFormatName));
                result.dwAspect = DVASPECT.DVASPECT_CONTENT;
                result.ptd = IntPtr.Zero;
                result.lindex = -1;
                if (aFormatName == DataFormat.Image)
                {
                    result.tymed = TYMED.TYMED_GDI;
                }
                else
                {
                    result.tymed = TYMED.TYMED_HGLOBAL;
                }

                return result;
            }

            public void Clone(out IEnumFORMATETC newEnum)
            {
                newEnum = new FormatEnumerator(_formats, _current);
            }

            public int Next(int celt, FORMATETC[] rgelt, int[] pceltFetched)
            {
                if (rgelt == null)
                    return unchecked((int)HRESULT.E_INVALIDARG);

                int i = 0;
                while (i < celt && _current < _formats.Length)
                {
                    rgelt[i] = _formats[_current];
                    _current++;
                    i++;
                }
                if (pceltFetched != null)
                    pceltFetched[0] = i;

                if (i != celt)
                    return unchecked((int)HRESULT.S_FALSE);
                return unchecked((int)HRESULT.S_OK);
            }

            public int Reset()
            {
                _current = 0;
                return unchecked((int)HRESULT.S_OK);
            }

            public int Skip(int celt)
            {
                _current += Math.Min(celt, int.MaxValue - _current);
                if (_current >= _formats.Length)
                    return unchecked((int)HRESULT.S_FALSE);
                return unchecked((int)HRESULT.S_OK);
            }
        }

        private const int DV_E_TYMED = unchecked((int)0x80040069);
        private const int DV_E_DVASPECT = unchecked((int)0x8004006B);
        private const int DV_E_FORMATETC = unchecked((int)0x80040064);
        private const int OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003);
        private const int STG_E_MEDIUMFULL = unchecked((int)0x80030070);
        public const int DV_E_CLIPFORMAT = unchecked((int)0x8004006A);

        HybridDictionary<DataFormat, object> datas = new HybridDictionary<DataFormat, object>();
        public DataObject(params (DataFormat, object)[] data)
        {
            if (data == null || data.Length == 0)
            {
                throw new Exception("数据不能为空");
            }
            foreach (var item in data)
            {
                if (item.Item2 == null)
                {
                    throw new Exception("数据不能为空");
                }
                datas.Add(item.Item1, item.Item2);
            }
        }

        public void SetData(DataFormat dataFormat, object data)
        {
            if (data == null)
            {
                throw new Exception("数据不能为空");
            }
            datas.Add(dataFormat, data);
        }

        #region IOleDataObject

        int IOleDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
        {
            connection = 0;
            return OLE_E_ADVISENOTSUPPORTED;
        }

        void IOleDataObject.DUnadvise(int connection)
        {
            Marshal.ThrowExceptionForHR(OLE_E_ADVISENOTSUPPORTED);
        }

        int IOleDataObject.EnumDAdvise(out IEnumSTATDATA enumAdvise)
        {
            enumAdvise = null;
            return OLE_E_ADVISENOTSUPPORTED;
        }

        IEnumFORMATETC IOleDataObject.EnumFormatEtc(DATADIR direction)
        {
            if (direction == DATADIR.DATADIR_GET)
                return new FormatEnumerator(this);
            throw new NotSupportedException();
        }

        int IOleDataObject.GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
        {
            formatOut = new FORMATETC();
            formatOut.ptd = IntPtr.Zero;
            return unchecked((int)HRESULT.E_NOTIMPL);
        }

        void IOleDataObject.GetData(ref FORMATETC format, out STGMEDIUM medium)
        {
            medium = default(STGMEDIUM);
            medium.tymed = (int)TYMED.TYMED_HGLOBAL;
            var fmt = ClipboardImpl.GetFormat(format.cfFormat);
            //System.Diagnostics.Debug.WriteLine(fmt + " -- " + (int)format.cfFormat);
            try
            {
                if (!format.tymed.HasFlag(TYMED.TYMED_HGLOBAL) && !format.tymed.HasFlag(TYMED.TYMED_GDI))
                {
                    Marshal.ThrowExceptionForHR(DV_E_TYMED);
                    return;
                }

                if (format.dwAspect != DVASPECT.DVASPECT_CONTENT)
                {
                    Marshal.ThrowExceptionForHR(DV_E_DVASPECT);
                    return;
                }

                if (!datas.ContainsKey(fmt))
                {
                    Marshal.ThrowExceptionForHR(DV_E_FORMATETC);
                    //Marshal.ThrowExceptionForHR(0);
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }

            int result = WriteDataToHGlobal(fmt, ref medium.unionmember, ref medium, format);
            Marshal.ThrowExceptionForHR(result);
        }

        void IOleDataObject.GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
        {
            if (medium.tymed != (int)TYMED.TYMED_HGLOBAL || !format.tymed.HasFlag(TYMED.TYMED_HGLOBAL))
                Marshal.ThrowExceptionForHR(DV_E_TYMED);

            if (format.dwAspect != DVASPECT.DVASPECT_CONTENT)
                Marshal.ThrowExceptionForHR(DV_E_DVASPECT);

            var fmt = ClipboardImpl.GetFormat(format.cfFormat);
            if (!datas.ContainsKey(fmt))
                Marshal.ThrowExceptionForHR(DV_E_FORMATETC);

            if (medium.unionmember == IntPtr.Zero)
                Marshal.ThrowExceptionForHR(STG_E_MEDIUMFULL);

            int result = WriteDataToHGlobal(fmt, ref medium.unionmember, ref medium, format);
            Marshal.ThrowExceptionForHR(result);
        }
        //Dictionary<DataFormat, short> log = new Dictionary<DataFormat, short>();
        int IOleDataObject.QueryGetData(ref FORMATETC format)
        {
            if (format.dwAspect != DVASPECT.DVASPECT_CONTENT)
                return DV_E_DVASPECT;
            if (!format.tymed.HasFlag(TYMED.TYMED_HGLOBAL))
                return DV_E_TYMED;

            var dataFormat = ClipboardImpl.GetFormat(format.cfFormat);
            //System.Diagnostics.Debug.WriteLine(dataFormat + " " + format.cfFormat);
            //log.Add(dataFormat, format.cfFormat);
            if (datas.ContainsKey(dataFormat))
                return unchecked((int)HRESULT.S_OK);
            return DV_E_FORMATETC;
            //return unchecked((int)HRESULT.S_FALSE);
        }

        void IOleDataObject.SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
        {
            try
            {
                Marshal.ThrowExceptionForHR(unchecked((int)HRESULT.E_NOTIMPL));
            }
            catch (Exception)
            {

            }
        }

        private int WriteDataToHGlobal(DataFormat dataFormat, ref IntPtr hGlobal, ref STGMEDIUM medium, FORMATETC format)
        {
            object data = datas[dataFormat];
            if (dataFormat == DataFormat.Text && data is string)
                return WriteStringToHGlobal(ref hGlobal, Convert.ToString(data), format.cfFormat);
            if (dataFormat == DataFormat.FileNames && data is IEnumerable<string> files)
                return WriteFileListToHGlobal(ref hGlobal, files);
            if (dataFormat == DataFormat.Html)
            {
                var html = data.ToString();
                Encoding enc = Encoding.UTF8;

                string begin = "Version:0.9\r\nStartHTML:{0:000000}\r\nEndHTML:{1:000000}"
                               + "\r\nStartFragment:{2:000000}\r\nEndFragment:{3:000000}\r\nSourceURL:\r\n";

                string html_begin = "<html>\r\n<body>\r\n"
                                    + "<!--StartFragment-->";

                string html_end = "<!--EndFragment-->\r\n</body>\r\n</html>\r\n";

                string begin_sample = string.Format(begin, 0, 0, 0, 0);

                int count_begin = enc.GetByteCount(begin_sample);
                int count_html_begin = enc.GetByteCount(html_begin);
                int count_html = enc.GetByteCount(html);
                int count_html_end = enc.GetByteCount(html_end);

                string html_total = string.Format(
                    begin
                    , count_begin
                    , count_begin + count_html_begin + count_html + count_html_end
                    , count_begin + count_html_begin
                    , count_begin + count_html_begin + count_html
                                        ) + html_begin + html + html_end;
                var l = enc.GetByteCount(html_total);
                if (hGlobal == IntPtr.Zero)
                    hGlobal = UnmanagedMethods.GlobalAlloc(UnmanagedMethods.GMEM_MOVEABLE | UnmanagedMethods.GMEM_ZEROINIT, l);
                IntPtr ptr = UnmanagedMethods.GlobalLock(hGlobal);
                Marshal.Copy(enc.GetBytes(html_total), 0, ptr, l);
                UnmanagedMethods.GlobalUnlock(hGlobal);
                return unchecked((int)HRESULT.S_OK);
            }
            else if (dataFormat == DataFormat.Image)
            {
                if (data is Image img)
                {
                    medium.tymed = (int)TYMED.TYMED_GDI;

                    IntPtr hBitmap;
                    var stream1 = img.SaveToStream(ImageFormat.Png);
                    var states = UnmanagedMethods.GdipCreateBitmapFromStream(new GPStream(stream1), out IntPtr bitmap);
                    stream1.Dispose();
                    UnmanagedMethods.GdipCreateHBITMAPFromBitmap(bitmap, out hBitmap, UnmanagedMethods.ToWin32(Color.White));
                    UnmanagedMethods.GdipDisposeImage(bitmap);
                    //UnmanagedMethods.SetClipboardData(UnmanagedMethods.ClipboardFormat.CF_BITMAP, hBitmap);
                    if (format.cfFormat == 2)
                    {
                        hGlobal = hBitmap;
                        return unchecked((int)HRESULT.S_OK);
                    }

                    IntPtr screenDC = UnmanagedMethods.GetDC(IntPtr.Zero);
                    IntPtr memDc = UnmanagedMethods.CreateCompatibleDC(screenDC);
                    UnmanagedMethods.BITMAPINFOHEADER info = new UnmanagedMethods.BITMAPINFOHEADER();
                    info.biSize = (uint)Marshal.SizeOf(typeof(UnmanagedMethods.BITMAPINFOHEADER));
                    info.biBitCount = 24;
                    info.biHeight = img.Height;
                    info.biWidth = img.Width;
                    info.biPlanes = 1;
                    info.biSizeImage = (uint)(img.Width * img.Height * 3);
                    var dibHbitmap = UnmanagedMethods.CreateDIBSection(memDc, ref info, 0, out IntPtr ppvBits, IntPtr.Zero, 0);
                    var oldBits = UnmanagedMethods.SelectObject(memDc, dibHbitmap);

                    IntPtr sdc = UnmanagedMethods.CreateCompatibleDC(screenDC);
                    var sob = UnmanagedMethods.SelectObject(sdc, hBitmap);

                    UnmanagedMethods.BitBlt(memDc, 0, 0, img.Width, img.Height, sdc, 0, 0, TernaryRasterOperations.SRCCOPY);

                    if (hGlobal == IntPtr.Zero)
                        hGlobal = UnmanagedMethods.GlobalAlloc(UnmanagedMethods.GMEM_MOVEABLE | UnmanagedMethods.GMEM_ZEROINIT, (int)info.biSize + (int)info.biSizeImage);
                    var ptr = UnmanagedMethods.GlobalLock(hGlobal);
                    Marshal.StructureToPtr(info, ptr, true);
                    var d = new byte[info.biSizeImage];
                    Marshal.Copy(ppvBits, d, 0, d.Length);
                    Marshal.Copy(d, 0, ptr + (int)info.biSize, d.Length);

                    UnmanagedMethods.GlobalUnlock(hGlobal);
                    UnmanagedMethods.SelectObject(sdc, sob);
                    UnmanagedMethods.DeleteDC(sdc);
                    UnmanagedMethods.SelectObject(memDc, oldBits);
                    UnmanagedMethods.DeleteDC(memDc);
                    UnmanagedMethods.ReleaseDC(IntPtr.Zero, screenDC);
                    //System.Diagnostics.Debug.WriteLine("bitmap" + hBitmap);
                    return unchecked((int)HRESULT.S_OK);
                }
            }
            if (data is Stream stream)
            {
                byte[] buffer = new byte[stream.Length - stream.Position];
                stream.Read(buffer, 0, buffer.Length);
                return WriteBytesToHGlobal(ref hGlobal, buffer);
            }
            if (data is IEnumerable<byte> bytes)
            {
                var byteArr = bytes is byte[]? (byte[])bytes : bytes.ToArray();
                return WriteBytesToHGlobal(ref hGlobal, byteArr);
            }
            return WriteBytesToHGlobal(ref hGlobal, SerializeObject(data));
        }

        private byte[] SerializeObject(object data)
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(SerializedObjectGUID, 0, SerializedObjectGUID.Length);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(ms, data);
                return ms.ToArray();
            }
        }

        private int WriteBytesToHGlobal(ref IntPtr hGlobal, byte[] data)
        {
            int required = data.Length;
            if (hGlobal == IntPtr.Zero)
                hGlobal = UnmanagedMethods.GlobalAlloc(UnmanagedMethods.GMEM_MOVEABLE | UnmanagedMethods.GMEM_ZEROINIT, required);

            long available = UnmanagedMethods.GlobalSize(hGlobal).ToInt64();
            if (required > available)
                return STG_E_MEDIUMFULL;

            IntPtr ptr = UnmanagedMethods.GlobalLock(hGlobal);
            try
            {
                Marshal.Copy(data, 0, ptr, data.Length);
                return unchecked((int)HRESULT.S_OK);
            }
            finally
            {
                UnmanagedMethods.GlobalUnlock(hGlobal);
            }
        }

        private int WriteFileListToHGlobal(ref IntPtr hGlobal, IEnumerable<string> files)
        {
            if (!files?.Any() ?? false)
                return unchecked((int)HRESULT.S_OK);

            char[] filesStr = (string.Join("\0", files) + "\0\0").ToCharArray();
            _DROPFILES df = new _DROPFILES();
#if Net4
            df.pFiles = Marshal.SizeOf(typeof(_DROPFILES));
#else
            df.pFiles = Marshal.SizeOf<_DROPFILES>();
#endif
            df.fWide = true;
#if Net4
            int required = (filesStr.Length * sizeof(char)) + Marshal.SizeOf(typeof(_DROPFILES));
#else
            int required = (filesStr.Length * sizeof(char)) + Marshal.SizeOf<_DROPFILES>();
#endif
            if (hGlobal == IntPtr.Zero)
                hGlobal = UnmanagedMethods.GlobalAlloc(UnmanagedMethods.GMEM_MOVEABLE | UnmanagedMethods.GMEM_ZEROINIT, required);

            long available = UnmanagedMethods.GlobalSize(hGlobal).ToInt64();
            if (required > available)
                return STG_E_MEDIUMFULL;

            IntPtr ptr = UnmanagedMethods.GlobalLock(hGlobal);
            try
            {
                Marshal.StructureToPtr(df, ptr, false);
#if Net4
                Marshal.Copy(filesStr, 0, ptr + Marshal.SizeOf(typeof(_DROPFILES)), filesStr.Length);
#else
                Marshal.Copy(filesStr, 0, ptr + Marshal.SizeOf<_DROPFILES>(), filesStr.Length);
#endif
                return unchecked((int)HRESULT.S_OK);
            }
            finally
            {
                UnmanagedMethods.GlobalUnlock(hGlobal);
            }
        }

        private int WriteStringToHGlobal(ref IntPtr hGlobal, string data, short format)
        {
            //if (format == (short)UnmanagedMethods.ClipboardFormat.CF_UNICODETEXT)
            {
                int required = (data.Length + 1) * sizeof(char);
                if (hGlobal == IntPtr.Zero)
                    hGlobal = UnmanagedMethods.GlobalAlloc(UnmanagedMethods.GMEM_MOVEABLE | UnmanagedMethods.GMEM_ZEROINIT, required);

                long available = UnmanagedMethods.GlobalSize(hGlobal).ToInt64();
                if (required > available)
                    return STG_E_MEDIUMFULL;

                IntPtr ptr = UnmanagedMethods.GlobalLock(hGlobal);
                try
                {
                    char[] chars = (data + '\0').ToCharArray();
                    Marshal.Copy(chars, 0, ptr, chars.Length);
                    return unchecked((int)HRESULT.S_OK);
                }
                finally
                {
                    UnmanagedMethods.GlobalUnlock(hGlobal);
                }
            }
//            else
//            {
//                Int32 pinvokeSize;
//                byte[] strBytes;
//                IntPtr ptr;

//                // Convert the unicode text to the ansi multi byte in case of the source unicode is available.
//                // WideCharToMultiByte will throw exception in case of passing 0 size of unicode.
//                if (data.Length > 0)
//                {
//                    pinvokeSize = Win32WideCharToMultiByte(data, data.Length, null, 0);
//                }
//                else
//                {
//                    pinvokeSize = 0;
//                }

//                strBytes = new byte[pinvokeSize];

//                if (pinvokeSize > 0)
//                {
//                    Win32WideCharToMultiByte(data, data.Length, strBytes, strBytes.Length);
//                }

//                // Ensure memory allocation and copy multi byte data with the null terminate
//                if (hGlobal == IntPtr.Zero)
//                    hGlobal = UnmanagedMethods.GlobalAlloc(UnmanagedMethods.GMEM_MOVEABLE | UnmanagedMethods.GMEM_ZEROINIT, pinvokeSize + 1);
//                ptr = UnmanagedMethods.GlobalLock(hGlobal);

//                try
//                {
//                    // Win32 CopyMemory return void, so we should disable PreSharp 6523 that
//                    // expects the Win32 exception with the last error.
//#pragma warning disable 6523

//                    UnmanagedMethods.CopyMemory(ptr, strBytes, pinvokeSize);
//                    //Marshal.Copy(strBytes, 0, ptr, strBytes.Length);

//#pragma warning restore 6523

//                    Marshal.Copy(new byte[] { 0 }, 0, (IntPtr)((long)ptr + pinvokeSize), 1);
//                }
//                finally
//                {
//                    UnmanagedMethods.GlobalUnlock(hGlobal);
//                }
//                return unchecked((int)HRESULT.S_OK);
//            }
        }
        internal static int Win32WideCharToMultiByte(string wideString, int wideChars, byte[] bytes, int byteCount)
        {
            int win32Return = UnmanagedMethods.WideCharToMultiByte(0 /*CP_ACP*/, 0 /*flags*/, wideString, wideChars, bytes, byteCount, IntPtr.Zero, IntPtr.Zero);
            int win32Error = Marshal.GetLastWin32Error();
            if (win32Return == 0)
            {
                throw new System.ComponentModel.Win32Exception(win32Error);
            }

            return win32Return;
        }
        #endregion
    }
}
