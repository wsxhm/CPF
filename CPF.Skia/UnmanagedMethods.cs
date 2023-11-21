using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CPF.Skia
{
    class UnmanagedMethods
    {
        [DllImport("gdi32.dll")]
        public static extern IntPtr GetCurrentObject(IntPtr hdc, ObjectType uObjectType);
        // GetObject stuff
        [DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetObject(IntPtr hObject, int nSize, [In, Out] BITMAP bm);
    }

    public enum ObjectType
    {
        OBJ_PEN = 1,
        OBJ_BRUSH = 2,
        OBJ_DC = 3,
        OBJ_METADC = 4,
        OBJ_PAL = 5,
        OBJ_FONT = 6,
        OBJ_BITMAP = 7,
        OBJ_REGION = 8,
        OBJ_METAFILE = 9,
        OBJ_MEMDC = 10,
        OBJ_EXTPEN = 11,
        OBJ_ENHMETADC = 12,
        OBJ_ENHMETAFILE = 13
    }

    [StructLayout(LayoutKind.Sequential)]
    // This is not our convention for managed resources.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
    public class BITMAP
    {
        public int bmType = 0;
        public int bmWidth = 0;
        public int bmHeight = 0;
        public int bmWidthBytes = 0;
        public short bmPlanes = 0;
        public short bmBitsPixel = 0;
        public IntPtr bmBits = IntPtr.Zero;
    }
}
