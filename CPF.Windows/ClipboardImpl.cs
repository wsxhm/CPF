using System;
using System.Collections.Generic;
using System.Text;
using CPF.Input;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.IO;
using CPF.Drawing;
using System.Linq;

namespace CPF.Windows
{
    internal class ClipboardImpl : IClipboard
    {
        static int htmlId;
        static int[] Format;
        static ClipboardImpl()
        {
            //CF_BITMAP 位图的句柄（HBITMAP）。
            //CF_DIB 包含BITMAPINFO结构和位图位的内存对象。

            htmlId = UnmanagedMethods.RegisterClipboardFormat(Html);
            Format = new int[] { (int)UnmanagedMethods.ClipboardFormat.CF_UNICODETEXT, htmlId, (int)UnmanagedMethods.ClipboardFormat.CF_DIB, (int)UnmanagedMethods.ClipboardFormat.CF_HDROP, 0 };
        }
        private void OpenClipboard()
        {
            while (!UnmanagedMethods.OpenClipboard(IntPtr.Zero))
            {
                Thread.Sleep(100);
                //await Task.Delay(100);
            }
        }

        //public void SetText(string text)
        //{
        //    if (text == null)
        //    {
        //        throw new ArgumentNullException(nameof(text));
        //    }

        //    OpenClipboard();

        //    UnmanagedMethods.EmptyClipboard();

        //    try
        //    {
        //        var hGlobal = Marshal.StringToHGlobalUni(text);
        //        UnmanagedMethods.SetClipboardData(UnmanagedMethods.ClipboardFormat.CF_UNICODETEXT, hGlobal);
        //    }
        //    finally
        //    {
        //        UnmanagedMethods.CloseClipboard();
        //    }
        //}

        public void Clear()
        {
            OpenClipboard();
            try
            {
                UnmanagedMethods.EmptyClipboard();
            }
            finally
            {
                UnmanagedMethods.CloseClipboard();
            }
        }

        //List<IntPtr> intPtrs = new List<IntPtr>();

        public void SetData(params ValueTuple<DataFormat, object>[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            //foreach (var item in intPtrs)
            //{
            //    Marshal.FreeHGlobal(item);
            //}
            OpenClipboard();

            UnmanagedMethods.EmptyClipboard();

            try
            {
                IntPtr hGlobal = IntPtr.Zero;
                IntPtr ptr = IntPtr.Zero;
                foreach (var item in data)
                {
                    switch (item.Item1)
                    {
                        case DataFormat.Text:
                            hGlobal = Marshal.StringToHGlobalUni(item.Item2.ToString());
                            UnmanagedMethods.SetClipboardData(UnmanagedMethods.ClipboardFormat.CF_UNICODETEXT, hGlobal);
                            break;
                        case DataFormat.Html:
                            var html = item.Item2.ToString();
                            Encoding enc = Encoding.UTF8;

                            string begin = "Version:0.9\r\nStartHTML:{0:000000}\r\nEndHTML:{1:000000}"
                                           + "\r\nStartFragment:{2:000000}\r\nEndFragment:{3:000000}\r\n";

                            string html_begin = "<html>\r\n<head>\r\n"
                                                + "<meta http-equiv=\"Content-Type\""
                                                + " content=\"text/html; charset=" + enc.WebName + "\">\r\n"
                                                + "<title>HTML clipboard</title>\r\n</head>\r\n<body>\r\n"
                                                + "<!--StartFragment-->";

                            string html_end = "<!--EndFragment-->\r\n</body>\r\n</html>\r\n";

                            string begin_sample = String.Format(begin, 0, 0, 0, 0);

                            int count_begin = enc.GetByteCount(begin_sample);
                            int count_html_begin = enc.GetByteCount(html_begin);
                            int count_html = enc.GetByteCount(html);
                            int count_html_end = enc.GetByteCount(html_end);

                            string html_total = String.Format(
                                begin
                                , count_begin
                                , count_begin + count_html_begin + count_html + count_html_end
                                , count_begin + count_html_begin
                                , count_begin + count_html_begin + count_html
                                                    ) + html_begin + html + html_end;
                            //var h = Marshal.StringToCoTaskMemUTF8(html_total);
                            var l = enc.GetByteCount(html_total);
                            var h = Marshal.AllocHGlobal(l);
                            //intPtrs.Add(h);
                            Marshal.Copy(enc.GetBytes(html_total), 0, h, l);
                            UnmanagedMethods.SetClipboardData(htmlId, h);
                            //Marshal.FreeHGlobal(h);
                            break;
                        case DataFormat.Image:
                            var img = item.Item2 as Image;
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

                            IntPtr hBitmap;
                            var stream = img.SaveToStream(ImageFormat.Png);
                            var states = UnmanagedMethods.GdipCreateBitmapFromStream(new GPStream(stream), out IntPtr bitmap);
                            stream.Dispose();
                            UnmanagedMethods.GdipCreateHBITMAPFromBitmap(bitmap, out hBitmap, UnmanagedMethods.ToWin32(Color.White));
                            UnmanagedMethods.GdipDisposeImage(bitmap);
                            UnmanagedMethods.SetClipboardData(UnmanagedMethods.ClipboardFormat.CF_BITMAP, hBitmap);


                            IntPtr sdc = UnmanagedMethods.CreateCompatibleDC(screenDC);
                            var sob = UnmanagedMethods.SelectObject(sdc, hBitmap);

                            UnmanagedMethods.BitBlt(memDc, 0, 0, img.Width, img.Height, sdc, 0, 0, TernaryRasterOperations.SRCCOPY);

                            var dib = UnmanagedMethods.GlobalAlloc(UnmanagedMethods.GMEM_MOVEABLE | UnmanagedMethods.GMEM_ZEROINIT, (int)info.biSize + (int)info.biSizeImage);
                            ptr = UnmanagedMethods.GlobalLock(dib);
                            Marshal.StructureToPtr(info, ptr, true);
                            var d = new byte[info.biSizeImage];
                            Marshal.Copy(ppvBits, d, 0, d.Length);
                            Marshal.Copy(d, 0, ptr + (int)info.biSize, d.Length);
                            UnmanagedMethods.SetClipboardData(UnmanagedMethods.ClipboardFormat.CF_DIB, dib);

                            UnmanagedMethods.GlobalUnlock(dib);
                            UnmanagedMethods.SelectObject(sdc, sob);
                            UnmanagedMethods.DeleteDC(sdc);
                            UnmanagedMethods.SelectObject(memDc, oldBits);
                            UnmanagedMethods.DeleteDC(memDc);
                            UnmanagedMethods.ReleaseDC(IntPtr.Zero, screenDC);
                            //UnmanagedMethods.DeleteObject(hBitmap);
                            break;
                        case DataFormat.FileNames:
                            var files = item.Item2 as IEnumerable<string>;
                            if (files != null)
                            {
                                char[] filesStr = (string.Join("\0", files) + "\0\0").ToCharArray();
                                _DROPFILES df = new _DROPFILES();
                                df.fWide = true;
#if Net4
                                df.pFiles = Marshal.SizeOf(typeof(_DROPFILES));
                                int required = (filesStr.Length * sizeof(char)) + Marshal.SizeOf(typeof(_DROPFILES));
#else
                                df.pFiles = Marshal.SizeOf<_DROPFILES>();
                                int required = (filesStr.Length * sizeof(char)) + Marshal.SizeOf<_DROPFILES>();
#endif
                                hGlobal = UnmanagedMethods.GlobalAlloc(UnmanagedMethods.GMEM_MOVEABLE | UnmanagedMethods.GMEM_ZEROINIT, required);

                                long available = UnmanagedMethods.GlobalSize(hGlobal).ToInt64();
                                if (required > available)
                                { break; }
                                ptr = UnmanagedMethods.GlobalLock(hGlobal);
                                try
                                {
                                    Marshal.StructureToPtr(df, ptr, false);
#if Net4
                                    Marshal.Copy(filesStr, 0, ptr + Marshal.SizeOf(typeof(_DROPFILES)), filesStr.Length);
#else
                                    Marshal.Copy(filesStr, 0, ptr + Marshal.SizeOf<_DROPFILES>(), filesStr.Length);
#endif
                                    UnmanagedMethods.SetClipboardData(UnmanagedMethods.ClipboardFormat.CF_HDROP, hGlobal);
                                }
                                finally
                                {
                                    UnmanagedMethods.GlobalUnlock(hGlobal);
                                }
                            }
                            break;
                    }
                }
            }
            finally
            {
                UnmanagedMethods.CloseClipboard();
            }
        }
        public bool Contains(DataFormat dataFormat)
        {
            OpenClipboard();
            uint LastRetrievedFormat = 0;
            List<uint> list = new List<uint>();
            while (0 != (LastRetrievedFormat = UnmanagedMethods.EnumClipboardFormats(LastRetrievedFormat)))
            {
                list.Add(LastRetrievedFormat);
            }

            UnmanagedMethods.CloseClipboard();
            //var f = Format[(int)dataFormat - 1];
            var f = GetFormatId(dataFormat);
            if (list.IndexOf((uint)f) < 0)
            {
                if (dataFormat == DataFormat.Text)
                {
                    if (list.IndexOf((uint)UnmanagedMethods.ClipboardFormat.CF_TEXT) >= 0)
                    {
                        return true;
                    }
                    if (list.IndexOf((uint)UnmanagedMethods.ClipboardFormat.CF_OEMTEXT) >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }

        public static DataFormat GetFormat(int format)
        {
            ushort f = (ushort)(format & 0xFFFF);
            if (format == 2 || format == 8)
            {
                return DataFormat.Image;
            }
            for (int i = 0; i < Format.Length; i++)
            {
                if (Format[i] == f)
                {
                    if (i == 0)
                    {
                        return DataFormat.Text;
                    }
                    else if (i == 1)
                    {
                        return DataFormat.Html;
                    }
                    else if (i == 2)
                    {
                        return DataFormat.Image;
                    }
                    else if (i == 3)
                    {
                        return DataFormat.FileNames;
                    }
                }
            }
            if (format == (ushort)UnmanagedMethods.ClipboardFormat.CF_TEXT || format == (ushort)UnmanagedMethods.ClipboardFormat.CF_OEMTEXT)
            {
                return DataFormat.Text;
            }
            return DataFormat.Unknown;
        }

        public static int GetFormatId(DataFormat dataFormat)
        {
            //return Format[(int)dataFormat];
            switch (dataFormat)
            {
                case DataFormat.Text:
                    return Format[0];
                case DataFormat.Html:
                    return Format[1];
                case DataFormat.Image:
                    return Format[2];
                case DataFormat.FileNames:
                    return Format[3];
                default:
                    return 0;
            }
        }

        public static Image ImageFormHBitmap(IntPtr hBitmap)
        {
            var r = UnmanagedMethods.GdipCreateBitmapFromHBITMAP(hBitmap, IntPtr.Zero, out IntPtr b);
            if (r != 0)
            {
                return null;
            }
            MemoryStream ms = new MemoryStream();
            Guid png = new Guid("{b96b3caf-0728-11d3-9d7b-0000f81ef32e}");
            Guid jpg = new Guid("{b96b3cae-0728-11d3-9d7b-0000f81ef32e}");

            int numDecoders;

            int status = UnmanagedMethods.GdipGetImageDecodersSize(out numDecoders, out int size);

            if (status != 0)
            {
                return null;
            }

            IntPtr memory = Marshal.AllocHGlobal(size);

            status = UnmanagedMethods.GdipGetImageDecoders(numDecoders, size, memory);

            int index;
            List<ImageCodecInfoPrivate> codes = new List<ImageCodecInfoPrivate>();

            for (index = 0; index < numDecoders; index++)
            {
                IntPtr curcodec = (IntPtr)((long)memory + (int)Marshal.SizeOf(typeof(ImageCodecInfoPrivate)) * index);
                ImageCodecInfoPrivate codecp = new ImageCodecInfoPrivate();
                Marshal.PtrToStructure(curcodec, codecp);

                codes.Add(codecp);
            }

            var pngCode = codes.Find(a => a.FormatID == png);

            UnmanagedMethods.GdipSaveImageToStream(b, new GPStream(ms), ref pngCode.Clsid, IntPtr.Zero);
            ms.Position = 0;
            var img = new Bitmap(ms);
            UnmanagedMethods.GdipDisposeImage(b);
            //ms.Position = 0;
            //File.WriteAllBytes(Path.Combine(CPF.Platform.Application.StartupPath, "test.png"), ms.ToArray());
            ms.Dispose();
            return img;
        }

        public object GetData(DataFormat dataFormat)
        {
            //if (dataFormat != DataFormat.Image)
            {
                OpenClipboard();
                try
                {
                    int formatId = (int)UnmanagedMethods.ClipboardFormat.CF_UNICODETEXT;
                    switch (dataFormat)
                    {
                        case DataFormat.Text:
                            uint LastRetrievedFormat = 0;
                            while (0 != (LastRetrievedFormat = UnmanagedMethods.EnumClipboardFormats(LastRetrievedFormat)))
                            {
                                if (LastRetrievedFormat == (uint)UnmanagedMethods.ClipboardFormat.CF_OEMTEXT)
                                {
                                    formatId = (int)UnmanagedMethods.ClipboardFormat.CF_OEMTEXT;
                                    break;
                                }
                                else if (LastRetrievedFormat == (uint)UnmanagedMethods.ClipboardFormat.CF_TEXT)
                                {
                                    formatId = (int)UnmanagedMethods.ClipboardFormat.CF_TEXT;
                                    break;
                                }
                            }
                            break;
                        case DataFormat.Html:
                            formatId = htmlId;
                            break;
                        case DataFormat.Image:
                            formatId = (int)UnmanagedMethods.ClipboardFormat.CF_DIB;
                            break;
                        case DataFormat.FileNames:
                            formatId = (int)UnmanagedMethods.ClipboardFormat.CF_HDROP;
                            break;
                    }
                    IntPtr hText = UnmanagedMethods.GetClipboardData(formatId);
                    if (hText == IntPtr.Zero)
                    {
                        return null;
                    }

                    string stringData = null;
                    int size;
                    IntPtr ptr = UnmanagedMethods.GlobalLock(hText);
                    try
                    {
                        switch (dataFormat)
                        {
                            case DataFormat.Text:
                                if (formatId == (int)UnmanagedMethods.ClipboardFormat.CF_TEXT)
                                {
                                    var rv = Marshal.PtrToStringAnsi(hText);
                                    return rv;
                                }
                                else
                                {
                                    var rv = Marshal.PtrToStringUni(hText);
                                    return rv;
                                }
                            case DataFormat.Html:
                                size = (int)UnmanagedMethods.GlobalSize(hText);
                                //if (unicode)
                                //{
                                //stringData = new string((char*)ptr);
                                //}
                                //else
                                //{
                                //    stringData = new string((sbyte*)ptr);
                                //}
                                byte[] bytes = new byte[size];
                                Marshal.Copy(ptr, bytes, 0, size);
                                stringData = Encoding.UTF8.GetString(bytes);
                                var start = stringData.IndexOf("<!--StartFragment");
                                var end = stringData.IndexOf("<!--EndFragment");
                                if (start == -1 || end == -1)
                                {
                                    var s = stringData.IndexOf("StartFragment:");
                                    var e = stringData.IndexOf("EndFragment:");
                                    if (s == -1 || end == -1)
                                    {

                                    }
                                    else
                                    {
                                        start = int.Parse(stringData.Substring(s + 14, 10));
                                        end = int.Parse(stringData.Substring(e + 14, 10));
                                        var by = Encoding.UTF8.GetBytes(stringData);
                                        stringData = Encoding.UTF8.GetString(by, start, end - start);
                                        stringData = stringData.Substring(start, end - start + 1);
                                    }
                                }
                                else
                                {
                                    stringData = stringData.Substring(start + 17, end - start - 17);
                                    stringData = stringData.TrimStart('-', ' ', '>');
                                }
                                return stringData;
                            case DataFormat.Image:
                                if (formatId == (int)UnmanagedMethods.ClipboardFormat.CF_BITMAP)
                                {
                                    var img = ImageFormHBitmap(hText);
                                    return img;
                                }
                                else
                                    unsafe
                                    {
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
                                                                                                    //_ = UnmanagedMethods.GlobalLock(hText);
                                        _ = UnmanagedMethods.StretchDIBits(memDc, 0, 0, bmp.biWidth, bmp.biHeight, 0, 0, bmp.biWidth, bmp.biHeight, (ptr + sizeof(UnmanagedMethods.BITMAPINFOHEADER)), ref info, 0, (uint)TernaryRasterOperations.SRCCOPY);
                                        //sizeof(UnmanagedMethods.BITMAPFILEHEADER) +
                                        //var c = sizeof(UnmanagedMethods.BITMAPINFOHEADER);

                                        var img = ImageFormHBitmap(hBitmap);
                                        //var img = new Bitmap(bmp.biWidth, bmp.biHeight, bmp.biWidth * 4, PixelFormat.PRgba, ppvBits).Clone();

                                        UnmanagedMethods.SelectObject(memDc, oldBits);
                                        UnmanagedMethods.ReleaseDC(IntPtr.Zero, screenDC);
                                        UnmanagedMethods.DeleteDC(memDc);
                                        UnmanagedMethods.DeleteObject(hBitmap);
                                        return img;
                                    }
                            case DataFormat.FileNames:
                                string[] files = null;
                                StringBuilder sb = new StringBuilder(260);

                                int count = UnmanagedMethods.DragQueryFile(hText, unchecked((int)0xFFFFFFFF), null, 0);
                                if (count > 0)
                                {
                                    files = new string[count];


                                    for (int i = 0; i < count; i++)
                                    {
                                        int charlen = UnmanagedMethods.DragQueryFileLongPath(hText, i, sb);
                                        if (0 == charlen)
                                            continue;
                                        string s = sb.ToString(0, charlen);

                                        // SECREVIEW : do we really need to do this?
                                        //
                                        //string fullPath = Path.GetFullPath(s);
                                        //Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileIO(" + fullPath + ") Demanded");
                                        //new FileIOPermission(FileIOPermissionAccess.PathDiscovery, fullPath).Demand();
                                        files[i] = s;
                                    }
                                }

                                return files;
                            default:
                                return null;
                        }
                    }
                    finally
                    {
                        UnmanagedMethods.GlobalUnlock(hText);
                    }

                }
                finally
                {
                    UnmanagedMethods.CloseClipboard();
                }
            }
            //else
            //{
            //    IDataObject dataObject = null;
            //    int hr, retry = 10;
            //    do
            //    {
            //        hr = UnmanagedMethods.OleGetClipboard(ref dataObject);
            //        if (hr != 0)
            //        {
            //            if (retry == 0)
            //            {
            //                //ThrowIfFailed(hr);
            //                throw new Exception("读取剪贴板失败");
            //            }
            //            retry--;
            //            Thread.Sleep(100 /*ms*/);
            //        }
            //    }
            //    while (hr != 0);

            //    FORMATETC formatetc = new FORMATETC();
            //    STGMEDIUM medium = new STGMEDIUM();
            //    var tymed = TYMED.TYMED_GDI;
            //    formatetc.cfFormat = (short)UnmanagedMethods.ClipboardFormat.CF_BITMAP;
            //    formatetc.dwAspect = DVASPECT.DVASPECT_CONTENT;
            //    formatetc.lindex = -1;
            //    formatetc.tymed = tymed;
            //    medium.tymed = tymed;

            //    Object data = null;
            //    if (0 == dataObject.QueryGetData(ref formatetc))
            //    {
            //        try
            //        {
            //            //IntSecurity.UnmanagedCode.Assert();
            //            try
            //            {
            //                dataObject.GetData(ref formatetc, out medium);
            //            }
            //            finally
            //            {
            //                //CodeAccessPermission.RevertAssert();
            //            }

            //            if (medium.unionmember != IntPtr.Zero)
            //            {
            //                var r = UnmanagedMethods.GdipCreateBitmapFromHBITMAP(medium.unionmember, IntPtr.Zero, out IntPtr b);
            //                if (r != 0)
            //                {
            //                    return null;
            //                }
            //                //UnmanagedMethods.BITMAP BITMAP = new UnmanagedMethods.BITMAP();
            //                //UnmanagedMethods.GetObject(medium.unionmember, Marshal.SizeOf(typeof(UnmanagedMethods.BITMAP)), BITMAP);
            //                MemoryStream ms = new MemoryStream();
            //                Guid png = new Guid("{b96b3caf-0728-11d3-9d7b-0000f81ef32e}");

            //                int numDecoders;
            //                int size;

            //                int status = UnmanagedMethods.GdipGetImageDecodersSize(out numDecoders, out size);

            //                if (status != 0)
            //                {
            //                    return null;
            //                }

            //                IntPtr memory = Marshal.AllocHGlobal(size);

            //                status = UnmanagedMethods.GdipGetImageDecoders(numDecoders, size, memory);

            //                int index;
            //                List<ImageCodecInfoPrivate> codes = new List<ImageCodecInfoPrivate>();

            //                for (index = 0; index < numDecoders; index++)
            //                {
            //                    IntPtr curcodec = (IntPtr)((long)memory + (int)Marshal.SizeOf(typeof(ImageCodecInfoPrivate)) * index);
            //                    ImageCodecInfoPrivate codecp = new ImageCodecInfoPrivate();
            //                    Marshal.PtrToStructure(curcodec, codecp);

            //                    codes.Add(codecp);
            //                }

            //                var pngCode = codes.Find(a => a.FormatID == png);

            //                UnmanagedMethods.GdipSaveImageToStream(b, new ComStreamFromDataStream(ms), ref pngCode.Clsid, IntPtr.Zero);
            //                ms.Position = 0;
            //                var img = new CPF.Bitmap(ms);
            //                UnmanagedMethods.GdipDisposeImage(b);
            //                ms.Dispose();
            //                return img;
            //            }
            //        }
            //        catch
            //        {
            //        }
            //    }
            //    return data;
            //}

        }

        /// <para>Specifies the standard ANSI text format. This <see langword='static '/> 
        /// field is read-only.</para>
        /// </devdoc>
        public static readonly string Text = "Text";

        ///    <para>Specifies the standard Windows Unicode text format. This 
        ///    <see langword='static '/>
        ///    field is read-only.</para>
        /// </devdoc>
        public static readonly string UnicodeText = "UnicodeText";

        ///    <para>Specifies the Windows Device Independent Bitmap (DIB) 
        ///       format. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string Dib = "DeviceIndependentBitmap";

        /// <devdoc>
        /// <para>Specifies a Windows bitmap format. This <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string Bitmap = "Bitmap";

        /// <devdoc>
        ///    <para>Specifies the Windows enhanced metafile format. This 
        ///    <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string EnhancedMetafile = "EnhancedMetafile";

        /// <devdoc>
        ///    <para>Specifies the Windows metafile format, which Win Forms 
        ///       does not directly use. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")] // Would be a breaking change to rename this
        public static readonly string MetafilePict = "MetaFilePict";

        /// <devdoc>
        ///    <para>Specifies the Windows symbolic link format, which Win 
        ///       Forms does not directly use. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string SymbolicLink = "SymbolicLink";

        /// <devdoc>
        ///    <para>Specifies the Windows data interchange format, which Win 
        ///       Forms does not directly use. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string Dif = "DataInterchangeFormat";

        /// <devdoc>
        ///    <para>Specifies the Tagged Image File Format (TIFF), which Win 
        ///       Forms does not directly use. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string Tiff = "TaggedImageFileFormat";

        /// <devdoc>
        ///    <para>Specifies the standard Windows original equipment 
        ///       manufacturer (OEM) text format. This <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string OemText = "OEMText";
        /// <devdoc>
        /// <para>Specifies the Windows palette format. This <see langword='static '/> 
        /// field is read-only.</para>
        /// </devdoc>
        public static readonly string Palette = "Palette";

        /// <devdoc>
        ///    <para>Specifies the Windows pen data format, which consists of 
        ///       pen strokes for handwriting software; Win Forms does not use this format. This
        ///    <see langword='static '/> 
        ///    field is read-only.</para>
        /// </devdoc>
        public static readonly string PenData = "PenData";

        /// <devdoc>
        ///    <para>Specifies the Resource Interchange File Format (RIFF) 
        ///       audio format, which Win Forms does not directly use. This <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string Riff = "RiffAudio";

        /// <devdoc>
        ///    <para>Specifies the wave audio format, which Win Forms does not 
        ///       directly use. This <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string WaveAudio = "WaveAudio";

        /// <devdoc>
        ///    <para>Specifies the Windows file drop format, which Win Forms 
        ///       does not directly use. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string FileDrop = "FileDrop";

        /// <devdoc>
        ///    <para>Specifies the Windows culture format, which Win Forms does 
        ///       not directly use. This <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string Locale = "Locale";

        /// <devdoc>
        ///    <para>Specifies text consisting of HTML data. This 
        ///    <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string Html = "HTML Format";

        /// <devdoc>
        ///    <para>Specifies text consisting of Rich Text Format (RTF) data. This 
        ///    <see langword='static '/> field is read-only.</para>
        /// </devdoc>
        public static readonly string Rtf = "Rich Text Format";

        /// <devdoc>
        ///    <para>Specifies a comma-separated value (CSV) format, which is a 
        ///       common interchange format used by spreadsheets. This format is not used directly
        ///       by Win Forms. This <see langword='static '/>
        ///       field is read-only.</para>
        /// </devdoc>
        public static readonly string CommaSeparatedValue = "Csv";
    }

    //[ComImport()]
    //[Guid("0000010E-0000-0000-C000-000000000046")]
    //[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    //public interface IDataObject
    //{

    //    /// <devdoc>
    //    ///     Called by a data consumer to obtain data from a source data object. 
    //    ///     The GetData method renders the data described in the specified FORMATETC 
    //    ///     structure and transfers it through the specified STGMEDIUM structure. 
    //    ///     The caller then assumes responsibility for releasing the STGMEDIUM structure.
    //    /// </devdoc>
    //    void GetData([In] ref FORMATETC format, out STGMEDIUM medium);

    //    /// <devdoc>
    //    ///     Called by a data consumer to obtain data from a source data object. 
    //    ///     This method differs from the GetData method in that the caller must 
    //    ///     allocate and free the specified storage medium.
    //    /// </devdoc>
    //    void GetDataHere([In] ref FORMATETC format, ref STGMEDIUM medium);

    //    /// <devdoc>
    //    ///     Determines whether the data object is capable of rendering the data 
    //    ///     described in the FORMATETC structure. Objects attempting a paste or 
    //    ///     drop operation can call this method before calling IDataObject::GetData 
    //    ///     to get an indication of whether the operation may be successful.
    //    /// </devdoc>
    //    [PreserveSig]
    //    int QueryGetData([In] ref FORMATETC format);

    //    /// <devdoc>
    //    ///     Provides a standard FORMATETC structure that is logically equivalent to one that is more 
    //    ///     complex. You use this method to determine whether two different 
    //    ///     FORMATETC structures would return the same data, removing the need 
    //    ///     for duplicate rendering.
    //    /// </devdoc>
    //    [PreserveSig]
    //    int GetCanonicalFormatEtc([In] ref FORMATETC formatIn, out FORMATETC formatOut);

    //    /// <devdoc>
    //    ///     Called by an object containing a data source to transfer data to 
    //    ///     the object that implements this method.
    //    /// </devdoc>
    //    void SetData([In] ref FORMATETC formatIn, [In] ref STGMEDIUM medium, [MarshalAs(UnmanagedType.Bool)] bool release);

    //    /// <devdoc>
    //    ///     Creates an object for enumerating the FORMATETC structures for a 
    //    ///     data object. These structures are used in calls to IDataObject::GetData 
    //    ///     or IDataObject::SetData. 
    //    /// </devdoc>
    //    IEnumFORMATETC EnumFormatEtc(DATADIR direction);

    //    /// <devdoc>
    //    ///     Called by an object supporting an advise sink to create a connection between 
    //    ///     a data object and the advise sink. This enables the advise sink to be 
    //    ///     notified of changes in the data of the object.
    //    /// </devdoc>
    //    [PreserveSig]
    //    int DAdvise([In] ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection);

    //    /// <devdoc>
    //    ///     Destroys a notification connection that had been previously set up.
    //    /// </devdoc>
    //    void DUnadvise(int connection);

    //    /// <devdoc>
    //    ///     Creates an object that can be used to enumerate the current advisory connections.
    //    /// </devdoc>
    //    [PreserveSig]
    //    int EnumDAdvise(out IEnumSTATDATA enumAdvise);
    //}

    //public struct FORMATETC
    //{
    //    [MarshalAs(UnmanagedType.U2)]
    //    public short cfFormat;
    //    public IntPtr ptd;
    //    [MarshalAs(UnmanagedType.U4)]
    //    public DVASPECT dwAspect;
    //    public int lindex;
    //    [MarshalAs(UnmanagedType.U4)]
    //    public TYMED tymed;
    //}

    //public enum DATADIR
    //{
    //    DATADIR_GET = 1,
    //    DATADIR_SET = 2
    //}

    ///// <devdoc>
    /////     The IEnumFORMATETC interface is used to enumerate an array of FORMATETC 
    /////     structures. IEnumFORMATETC has the same methods as all enumerator interfaces: 
    /////     Next, Skip, Reset, and Clone.
    ///// </devdoc>
    //[ComImport()]
    //[Guid("00000103-0000-0000-C000-000000000046")]
    //[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    //public interface IEnumFORMATETC
    //{

    //    /// <devdoc>
    //    ///     Retrieves the next celt items in the enumeration sequence. If there are 
    //    ///     fewer than the requested number of elements left in the sequence, it 
    //    ///     retrieves the remaining elements. The number of elements actually 
    //    ///     retrieved is returned through pceltFetched (unless the caller passed 
    //    ///     in NULL for that parameter).
    //    /// </devdoc>
    //    [PreserveSig]
    //    int Next(int celt, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] FORMATETC[] rgelt, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);

    //    /// <devdoc>
    //    ///     Skips over the next specified number of elements in the enumeration sequence.
    //    /// </devdoc>
    //    [PreserveSig]
    //    int Skip(int celt);

    //    /// <devdoc>
    //    ///     Resets the enumeration sequence to the beginning.
    //    /// </devdoc>
    //    [PreserveSig]
    //    int Reset();

    //    /// <devdoc>
    //    ///     Creates another enumerator that contains the same enumeration state as 
    //    ///     the current one. Using this function, a client can record a particular 
    //    ///     point in the enumeration sequence and then return to that point at a 
    //    ///     later time. The new enumerator supports the same interface as the original one.
    //    /// </devdoc>
    //    void Clone(out IEnumFORMATETC newEnum);
    //}

    //[Flags]
    //public enum DVASPECT
    //{
    //    DVASPECT_CONTENT = 1,
    //    DVASPECT_THUMBNAIL = 2,
    //    DVASPECT_ICON = 4,
    //    DVASPECT_DOCPRINT = 8
    //}

    //[Flags]
    //public enum TYMED
    //{
    //    TYMED_HGLOBAL = 1,
    //    TYMED_FILE = 2,
    //    TYMED_ISTREAM = 4,
    //    TYMED_ISTORAGE = 8,
    //    TYMED_GDI = 16,
    //    TYMED_MFPICT = 32,
    //    TYMED_ENHMF = 64,
    //    TYMED_NULL = 0
    //}

    //[Flags]
    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1712:DoNotPrefixEnumValuesWithTypeName")]
    //public enum ADVF
    //{
    //    ADVF_NODATA = 1,
    //    ADVF_PRIMEFIRST = 2,
    //    ADVF_ONLYONCE = 4,
    //    ADVF_DATAONSTOP = 64,
    //    ADVFCACHE_NOHANDLER = 8,
    //    ADVFCACHE_FORCEBUILTIN = 16,
    //    ADVFCACHE_ONSAVE = 32
    //}

    ///// <devdoc>
    /////     The IAdviseSink interface enables containers and other objects to 
    /////     receive notifications of data changes, view changes, and compound-document 
    /////     changes occurring in objects of interest. Container applications, for 
    /////     example, require such notifications to keep cached presentations of their 
    /////     linked and embedded objects up-to-date. Calls to IAdviseSink methods are 
    /////     asynchronous, so the call is sent and then the next instruction is executed 
    /////     without waiting for the call's return.
    ///// </devdoc>
    //[ComImport()]
    //[Guid("0000010F-0000-0000-C000-000000000046")]
    //[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    //public interface IAdviseSink
    //{

    //    /// <devdoc>
    //    ///     Called by the server to notify a data object's currently registered 
    //    ///     advise sinks that data in the object has changed.
    //    /// </devdoc>
    //    [PreserveSig]
    //    void OnDataChange([In] ref FORMATETC format, [In] ref STGMEDIUM stgmedium);

    //    /// <devdoc>
    //    ///     Notifies an object's registered advise sinks that its view has changed.
    //    /// </devdoc>
    //    [PreserveSig]
    //    void OnViewChange(int aspect, int index);

    //    /// <devdoc>
    //    ///     Called by the server to notify all registered advisory sinks that 
    //    ///     the object has been renamed.
    //    /// </devdoc>
    //    [PreserveSig]
    //    void OnRename(IMoniker moniker);

    //    /// <devdoc>
    //    ///     Called by the server to notify all registered advisory sinks that 
    //    ///     the object has been saved.
    //    /// </devdoc>
    //    [PreserveSig]
    //    void OnSave();

    //    /// <devdoc>
    //    ///     Called by the server to notify all registered advisory sinks that the 
    //    ///     object has changed from the running to the loaded state.
    //    /// </devdoc>
    //    [PreserveSig]
    //    void OnClose();
    //}

    //public struct STGMEDIUM
    //{
    //    public TYMED tymed;
    //    public IntPtr unionmember;
    //    [MarshalAs(UnmanagedType.IUnknown)]
    //    public object pUnkForRelease;
    //}

    [StructLayout(LayoutKind.Sequential)]

    public struct FILETIME
    {
        public int dwLowDateTime;
        public int dwHighDateTime;
    }

    //[Guid("0000000f-0000-0000-C000-000000000046")]
    //[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    //[ComImport]
    //public interface IMoniker
    //{
    //    // IPersist portion
    //    void GetClassID(out Guid pClassID);

    //    // IPersistStream portion
    //    [PreserveSig]
    //    int IsDirty();
    //    void Load(IStream pStm);
    //    void Save(IStream pStm, [MarshalAs(UnmanagedType.Bool)] bool fClearDirty);
    //    void GetSizeMax(out Int64 pcbSize);

    //    // IMoniker portion
    //    void BindToObject(IBindCtx pbc, IMoniker pmkToLeft, [In()] ref Guid riidResult, [MarshalAs(UnmanagedType.Interface)] out Object ppvResult);
    //    void BindToStorage(IBindCtx pbc, IMoniker pmkToLeft, [In()] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out Object ppvObj);
    //    void Reduce(IBindCtx pbc, int dwReduceHowFar, ref IMoniker ppmkToLeft, out IMoniker ppmkReduced);
    //    void ComposeWith(IMoniker pmkRight, [MarshalAs(UnmanagedType.Bool)] bool fOnlyIfNotGeneric, out IMoniker ppmkComposite);
    //    void Enum([MarshalAs(UnmanagedType.Bool)] bool fForward, out IEnumMoniker ppenumMoniker);
    //    [PreserveSig]
    //    int IsEqual(IMoniker pmkOtherMoniker);
    //    void Hash(out int pdwHash);
    //    [PreserveSig]
    //    int IsRunning(IBindCtx pbc, IMoniker pmkToLeft, IMoniker pmkNewlyRunning);
    //    void GetTimeOfLastChange(IBindCtx pbc, IMoniker pmkToLeft, out FILETIME pFileTime);
    //    void Inverse(out IMoniker ppmk);
    //    void CommonPrefixWith(IMoniker pmkOther, out IMoniker ppmkPrefix);
    //    void RelativePathTo(IMoniker pmkOther, out IMoniker ppmkRelPath);
    //    void GetDisplayName(IBindCtx pbc, IMoniker pmkToLeft, [MarshalAs(UnmanagedType.LPWStr)] out String ppszDisplayName);
    //    void ParseDisplayName(IBindCtx pbc, IMoniker pmkToLeft, [MarshalAs(UnmanagedType.LPWStr)] String pszDisplayName, out int pchEaten, out IMoniker ppmkOut);
    //    [PreserveSig]
    //    int IsSystemMoniker(out int pdwMksys);
    //}

    //[StructLayout(LayoutKind.Sequential)]

    //public struct BIND_OPTS
    //{
    //    public int cbStruct;
    //    public int grfFlags;
    //    public int grfMode;
    //    public int dwTickCountDeadline;
    //}

    //[Guid("0000000e-0000-0000-C000-000000000046")]
    //[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    //[ComImport]
    //public interface IBindCtx
    //{
    //    void RegisterObjectBound([MarshalAs(UnmanagedType.Interface)] Object punk);
    //    void RevokeObjectBound([MarshalAs(UnmanagedType.Interface)] Object punk);
    //    void ReleaseBoundObjects();
    //    void SetBindOptions([In()] ref BIND_OPTS pbindopts);
    //    void GetBindOptions(ref BIND_OPTS pbindopts);
    //    void GetRunningObjectTable(out IRunningObjectTable pprot);
    //    void RegisterObjectParam([MarshalAs(UnmanagedType.LPWStr)] String pszKey, [MarshalAs(UnmanagedType.Interface)] Object punk);
    //    void GetObjectParam([MarshalAs(UnmanagedType.LPWStr)] String pszKey, [MarshalAs(UnmanagedType.Interface)] out Object ppunk);
    //    void EnumObjectParam(out IEnumString ppenum);
    //    [PreserveSig]
    //    int RevokeObjectParam([MarshalAs(UnmanagedType.LPWStr)] String pszKey);
    //}

    //[Guid("00000101-0000-0000-C000-000000000046")]
    //[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    //[ComImport]
    //public interface IEnumString
    //{
    //    [PreserveSig]
    //    int Next(int celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0), Out] String[] rgelt, IntPtr pceltFetched);
    //    [PreserveSig]
    //    int Skip(int celt);
    //    void Reset();
    //    void Clone(out IEnumString ppenum);
    //}

    //[Guid("00000010-0000-0000-C000-000000000046")]
    //[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    //[ComImport]
    //public interface IRunningObjectTable
    //{
    //    int Register(int grfFlags, [MarshalAs(UnmanagedType.Interface)] Object punkObject, IMoniker pmkObjectName);
    //    void Revoke(int dwRegister);
    //    [PreserveSig]
    //    int IsRunning(IMoniker pmkObjectName);
    //    [PreserveSig]
    //    int GetObject(IMoniker pmkObjectName, [MarshalAs(UnmanagedType.Interface)] out Object ppunkObject);
    //    void NoteChangeTime(int dwRegister, ref FILETIME pfiletime);
    //    [PreserveSig]
    //    int GetTimeOfLastChange(IMoniker pmkObjectName, out FILETIME pfiletime);
    //    void EnumRunning(out IEnumMoniker ppenumMoniker);
    //}

    //[Guid("00000102-0000-0000-C000-000000000046")]
    //[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    //[ComImport]
    //public interface IEnumMoniker
    //{
    //    [PreserveSig]
    //    int Next(int celt, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0), Out] IMoniker[] rgelt, IntPtr pceltFetched);
    //    [PreserveSig]
    //    int Skip(int celt);
    //    void Reset();
    //    void Clone(out IEnumMoniker ppenum);
    //}

    //[ComImport()]
    //[Guid("00000103-0000-0000-C000-000000000046")]
    //[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    //public interface IEnumSTATDATA
    //{

    //    /// <devdoc>
    //    ///     Retrieves the next celt items in the enumeration sequence. If there are 
    //    ///     fewer than the requested number of elements left in the sequence, it 
    //    ///     retrieves the remaining elements. The number of elements actually 
    //    ///     retrieved is returned through pceltFetched (unless the caller passed 
    //    ///     in NULL for that parameter).
    //    /// </devdoc>
    //    [PreserveSig]
    //    int Next(int celt, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] STATDATA[] rgelt, [Out, MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] int[] pceltFetched);

    //    /// <devdoc>
    //    ///     Skips over the next specified number of elements in the enumeration sequence.
    //    /// </devdoc>
    //    [PreserveSig]
    //    int Skip(int celt);

    //    /// <devdoc>
    //    ///     Resets the enumeration sequence to the beginning.
    //    /// </devdoc>
    //    [PreserveSig]
    //    int Reset();

    //    /// <devdoc>
    //    ///     Creates another enumerator that contains the same enumeration state as 
    //    ///     the current one. Using this function, a client can record a particular 
    //    ///     point in the enumeration sequence and then return to that point at a 
    //    ///     later time. The new enumerator supports the same interface as the original one.
    //    /// </devdoc>
    //    void Clone(out IEnumSTATDATA newEnum);
    //}

    //public struct STATDATA
    //{
    //    public FORMATETC formatetc;
    //    public ADVF advf;
    //    public IAdviseSink advSink;
    //    public int connection;
    //}

    //internal class ComStreamFromDataStream : IStream
    //{
    //    protected Stream dataStream;

    //    // to support seeking ahead of the stream length...
    //    long virtualPosition = -1;

    //    internal ComStreamFromDataStream(Stream dataStream)
    //    {
    //        if (dataStream == null) throw new ArgumentNullException("dataStream");
    //        this.dataStream = dataStream;
    //    }

    //    private void ActualizeVirtualPosition()
    //    {
    //        if (virtualPosition == -1) return;

    //        if (virtualPosition > dataStream.Length)
    //            dataStream.SetLength(virtualPosition);

    //        dataStream.Position = virtualPosition;

    //        virtualPosition = -1;
    //    }

    //    public virtual IStream Clone()
    //    {
    //        NotImplemented();
    //        return null;
    //    }

    //    public virtual void Commit(int grfCommitFlags)
    //    {
    //        dataStream.Flush();
    //        // Extend the length of the file if needed.
    //        ActualizeVirtualPosition();
    //    }

    //    public virtual long CopyTo(IStream pstm, long cb, long[] pcbRead)
    //    {
    //        int bufsize = 4096; // one page
    //        IntPtr buffer = Marshal.AllocHGlobal(bufsize);
    //        if (buffer == IntPtr.Zero) throw new OutOfMemoryException();
    //        long written = 0;
    //        try
    //        {
    //            while (written < cb)
    //            {
    //                int toRead = bufsize;
    //                if (written + toRead > cb) toRead = (int)(cb - written);
    //                int read = Read(buffer, toRead);
    //                if (read == 0) break;
    //                if (pstm.Write(buffer, read) != read)
    //                {
    //                    throw EFail("Wrote an incorrect number of bytes");
    //                }
    //                written += read;
    //            }
    //        }
    //        finally
    //        {
    //            Marshal.FreeHGlobal(buffer);
    //        }
    //        if (pcbRead != null && pcbRead.Length > 0)
    //        {
    //            pcbRead[0] = written;
    //        }

    //        return written;
    //    }

    //    public virtual Stream GetDataStream()
    //    {
    //        return dataStream;
    //    }

    //    public virtual void LockRegion(long libOffset, long cb, int dwLockType)
    //    {
    //    }

    //    protected static ExternalException EFail(string msg)
    //    {
    //        throw new ExternalException(msg);
    //    }

    //    protected static void NotImplemented()
    //    {
    //        throw new ExternalException("未实现");
    //    }

    //    public virtual int Read(IntPtr buf, /* cpr: int offset,*/  int length)
    //    {
    //        //        System.Text.Out.WriteLine("IStream::Read(" + length + ")");
    //        byte[] buffer = new byte[length];
    //        int count = Read(buffer, length);
    //        Marshal.Copy(buffer, 0, buf, length);
    //        return count;
    //    }

    //    public virtual int Read(byte[] buffer, /* cpr: int offset,*/  int length)
    //    {
    //        ActualizeVirtualPosition();
    //        return dataStream.Read(buffer, 0, length);
    //    }

    //    public virtual void Revert()
    //    {
    //        NotImplemented();
    //    }

    //    public virtual long Seek(long offset, int origin)
    //    {
    //        // Console.WriteLine("IStream::Seek("+ offset + ", " + origin + ")");
    //        long pos = virtualPosition;
    //        if (virtualPosition == -1)
    //        {
    //            pos = dataStream.Position;
    //        }
    //        long len = dataStream.Length;
    //        switch (origin)
    //        {
    //            case GPStream.STREAM_SEEK_SET:
    //                if (offset <= len)
    //                {
    //                    dataStream.Position = offset;
    //                    virtualPosition = -1;
    //                }
    //                else
    //                {
    //                    virtualPosition = offset;
    //                }
    //                break;
    //            case GPStream.STREAM_SEEK_END:
    //                if (offset <= 0)
    //                {
    //                    dataStream.Position = len + offset;
    //                    virtualPosition = -1;
    //                }
    //                else
    //                {
    //                    virtualPosition = len + offset;
    //                }
    //                break;
    //            case GPStream.STREAM_SEEK_CUR:
    //                if (offset + pos <= len)
    //                {
    //                    dataStream.Position = pos + offset;
    //                    virtualPosition = -1;
    //                }
    //                else
    //                {
    //                    virtualPosition = offset + pos;
    //                }
    //                break;
    //        }
    //        if (virtualPosition != -1)
    //        {
    //            return virtualPosition;
    //        }
    //        else
    //        {
    //            return dataStream.Position;
    //        }
    //    }

    //    public virtual void SetSize(long value)
    //    {
    //        dataStream.SetLength(value);
    //    }

    //    public virtual void Stat(IntPtr pstatstg, int grfStatFlag)
    //    {
    //        // GpStream has a partial implementation, but it's so partial rather 
    //        // restrict it to use with GDI+
    //        NotImplemented();
    //    }

    //    public virtual void UnlockRegion(long libOffset, long cb, int dwLockType)
    //    {
    //    }

    //    public virtual int Write(IntPtr buf, /* cpr: int offset,*/ int length)
    //    {
    //        byte[] buffer = new byte[length];
    //        Marshal.Copy(buf, buffer, 0, length);
    //        return Write(buffer, length);
    //    }

    //    public virtual int Write(byte[] buffer, /* cpr: int offset,*/ int length)
    //    {
    //        ActualizeVirtualPosition();
    //        dataStream.Write(buffer, 0, length);
    //        return length;
    //    }
    //}

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal class ImageCodecInfoPrivate
    {
        [MarshalAs(UnmanagedType.Struct)]
        public Guid Clsid;
        [MarshalAs(UnmanagedType.Struct)]
        public Guid FormatID;

        public IntPtr CodecName = IntPtr.Zero;
        public IntPtr DllName = IntPtr.Zero;
        public IntPtr FormatDescription = IntPtr.Zero;
        public IntPtr FilenameExtension = IntPtr.Zero;
        public IntPtr MimeType = IntPtr.Zero;

        public int Flags;
        public int Version;
        public int SigCount;
        public int SigSize;

        public IntPtr SigPattern = IntPtr.Zero;
        public IntPtr SigMask = IntPtr.Zero;
    }
}
