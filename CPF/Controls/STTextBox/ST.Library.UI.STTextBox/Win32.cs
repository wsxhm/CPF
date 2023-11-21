using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ST.Library.UI.STTextBox
{
    internal class Win32
    {
        //[DllImport("user32.dll")]
        //public static extern IntPtr GetDC(IntPtr hWnd);
        //[DllImport("user32.dll")]
        //public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        //[DllImport("gdi32.dll")]
        //public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        //[DllImport("gdi32.dll")]
        //public static extern bool DeleteDC(IntPtr hDC);
        //[DllImport("gdi32.dll")]
        //public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);
        //[DllImport("gdi32.dll")]
        //public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        //[DllImport("gdi32.dll")]
        //public static extern bool DeleteObject(IntPtr hObject);
        //[DllImport("gdi32.dll")]
        //public static extern int SetBkMode(IntPtr hDC, int iBkMode);
        //[DllImport("gdi32.dll")]
        //public static extern int SetTextCharacterExtra(IntPtr hDC, int nCharExtra);
        //[DllImport("gdi32.dll")]
        //public static extern uint SetTextColor(IntPtr hdc, int color);
        //[DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        //public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);
        //[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        //public static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, ref Size lpSize);

        [DllImport("user32.dll")]
        public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);
        [DllImport("user32.dll")]
        public static extern bool ShowCaret(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern bool HideCaret(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern bool SetCaretPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern bool DestroyCaret();
        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        public static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);
        [DllImport("imm32.dll")]
        public static extern bool ImmSetCandidateWindow(IntPtr hImc, ref CANDIDATEFORM fuck);
        [DllImport("imm32.dll")]
        public static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompForm);
        [DllImport("imm32.dll")]
        public static extern bool ImmSetCompositionFont(IntPtr hIMC, ref LOGFONT logFont);

        public const int SRCCOPY = 0x00CC0020;

        public const int GCS_COMPSTR = 0x0008;
        public const int GCS_RESULTSTR = 0x0800;
        public const int WM_IME_REQUEST = 0x0288;
        public const int WM_IME_COMPOSITION = 0x010F;
        public const int WM_IME_ENDCOMPOSITION = 0x010E;
        public const int WM_IME_STARTCOMPOSITION = 0x010D;
        // bit field for IMC_SETCOMPOSITIONWINDOW, IMC_SETCANDIDATEWINDOW
        public const int CFS_DEFAULT = 0x0000;
        public const int CFS_RECT = 0x0001;
        public const int CFS_POINT = 0x0002;
        public const int CFS_FORCE_POSITION = 0x0020;
        public const int CFS_CANDIDATEPOS = 0x0040;
        public const int CFS_EXCLUDE = 0x0080;

        public struct CANDIDATEFORM
        {
            public int dwIndex;
            public int dwStyle;
            public Point ptCurrentPos;
            public Rectangle rcArea;
        }

        public struct COMPOSITIONFORM
        {
            public int dwStyle;
            public Point ptCurrentPos;
            public Rectangle rcArea;
        }

        public struct LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            public string lfFaceName;
        }

        private static byte[] m_byString = new byte[1024];

        public static string ImmGetCompositionString(IntPtr hIMC, int dwIndex) {
            if (hIMC == IntPtr.Zero) {
                return null;
            }
            int nLen = Win32.ImmGetCompositionString(hIMC, dwIndex, m_byString, m_byString.Length);
            return Encoding.Unicode.GetString(m_byString, 0, nLen);
        }
    }
}
