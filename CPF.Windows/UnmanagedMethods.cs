using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using CPF.Drawing;
//using IOleDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
// ReSharper disable InconsistentNaming
#pragma warning disable 169, 649

namespace CPF.Windows
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using Win32 naming for consistency.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Using Win32 naming for consistency.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Using Win32 naming for consistency.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements must be documented", Justification = "Look in Win32 docs.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items must be documented", Justification = "Look in Win32 docs.")]
    public static class UnmanagedMethods
    {
        static IntPtr initToken;
        static IntPtr shcore;
        static UnmanagedMethods()
        {
            StartupInput input = StartupInput.GetDefault();
            StartupOutput output;

            // GDI+ ref counts multiple calls to Startup in the same process, so calls from multiple
            // domains are ok, just make sure to pair each w/GdiplusShutdown
            int status = GdiplusStartup(out initToken, ref input, out output);

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.ProcessExit += new EventHandler(OnProcessExit);

            // Also sync to DomainUnload for non-default domains since they will not get a ProcessExit if
            // they are unloaded prior to ProcessExit (and this object's static fields are scoped to AppDomains, 
            // so we must cleanup on AppDomain shutdown)
            if (!currentDomain.IsDefaultAppDomain())
            {
                currentDomain.DomainUnload += new EventHandler(OnProcessExit);
            }

            shcore = LoadLibrary("shcore.dll");
            var addr = GetProcAddress(shcore, nameof(GetDpiForMonitor));
            if (addr != IntPtr.Zero)
            {
                GetDpiForMonitor = (GetDpiForMonitor)Marshal.GetDelegateForFunctionPointer(addr, typeof(GetDpiForMonitor));
            }
        }
        private static void OnProcessExit(object sender, EventArgs e)
        {
            LocalDataStoreSlot slot = Thread.GetNamedDataSlot("system.drawing.threaddata");
            Thread.SetData(slot, null);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.ProcessExit -= new EventHandler(OnProcessExit);
            if (!currentDomain.IsDefaultAppDomain())
            {
                currentDomain.DomainUnload -= new EventHandler(OnProcessExit);
            }
        }

        public static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }

        public static int LOWORD(int n)
        {
            return n & 0xffff;
        }

        public const int GMEM_DDESHARE = 0x2000;
        public const int GMEM_ZEROINIT = 0x0040;
        public const int GMEM_MOVEABLE = 0x0002;
        public const int CW_USEDEFAULT = unchecked((int)0x80000000);

        public delegate void TimerProc(IntPtr hWnd, uint uMsg, IntPtr nIDEvent, uint dwTime);

        public delegate void TimeCallback(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void WaitOrTimerCallback(IntPtr lpParameter, bool timerOrWaitFired);

        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public static readonly IntPtr DPI_AWARENESS_CONTEXT_UNAWARE = new IntPtr(-1);
        public static readonly IntPtr DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = new IntPtr(-2);
        public static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = new IntPtr(-3);
        public static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);

        public enum Cursor
        {
            IDC_ARROW = 32512,
            IDC_IBEAM = 32513,
            IDC_WAIT = 32514,
            IDC_CROSS = 32515,
            IDC_UPARROW = 32516,
            IDC_SIZE = 32640,
            IDC_ICON = 32641,
            IDC_SIZENWSE = 32642,
            IDC_SIZENESW = 32643,
            IDC_SIZEWE = 32644,
            IDC_SIZENS = 32645,
            IDC_SIZEALL = 32646,
            IDC_NO = 32648,
            IDC_HAND = 32649,
            IDC_APPSTARTING = 32650,
            IDC_HELP = 32651
        }

        public enum MouseActivate : int
        {
            MA_ACTIVATE = 1,
            MA_ACTIVATEANDEAT = 2,
            MA_NOACTIVATE = 3,
            MA_NOACTIVATEANDEAT = 4
        }

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            SWP_ASYNCWINDOWPOS = 0x4000,
            SWP_DEFERERASE = 0x2000,
            SWP_DRAWFRAME = 0x0020,
            SWP_FRAMECHANGED = 0x0020,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOACTIVATE = 0x0010,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOMOVE = 0x0002,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOREDRAW = 0x0008,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_NOSIZE = 0x0001,
            SWP_NOZORDER = 0x0004,
            SWP_SHOWWINDOW = 0x0040,

            SWP_RESIZE = SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOZORDER
        }

        public static class WindowPosZOrder
        {
            public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
            public static readonly IntPtr HWND_TOP = new IntPtr(0);
            public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
            public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        }

        public enum SizeCommand
        {
            Restored,
            Minimized,
            Maximized,
            MaxShow,
            MaxHide,
        }

        public enum ShowWindowCommand
        {
            Hide = 0,
            Normal = 1,
            ShowMinimized = 2,
            Maximize = 3,
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }

        public enum SystemMetric
        {
            SM_CXSCREEN = 0,  // 0x00
            SM_CYSCREEN = 1,  // 0x01
            SM_CXVSCROLL = 2,  // 0x02
            SM_CYHSCROLL = 3,  // 0x03
            SM_CYCAPTION = 4,  // 0x04
            SM_CXBORDER = 5,  // 0x05
            SM_CYBORDER = 6,  // 0x06
            SM_CXDLGFRAME = 7,  // 0x07
            SM_CXFIXEDFRAME = 7,  // 0x07
            SM_CYDLGFRAME = 8,  // 0x08
            SM_CYFIXEDFRAME = 8,  // 0x08
            SM_CYVTHUMB = 9,  // 0x09
            SM_CXHTHUMB = 10, // 0x0A
            SM_CXICON = 11, // 0x0B
            SM_CYICON = 12, // 0x0C
            SM_CXCURSOR = 13, // 0x0D
            SM_CYCURSOR = 14, // 0x0E
            SM_CYMENU = 15, // 0x0F
            SM_CXFULLSCREEN = 16, // 0x10
            SM_CYFULLSCREEN = 17, // 0x11
            SM_CYKANJIWINDOW = 18, // 0x12
            SM_MOUSEPRESENT = 19, // 0x13
            SM_CYVSCROLL = 20, // 0x14
            SM_CXHSCROLL = 21, // 0x15
            SM_DEBUG = 22, // 0x16
            SM_SWAPBUTTON = 23, // 0x17
            SM_CXMIN = 28, // 0x1C
            SM_CYMIN = 29, // 0x1D
            SM_CXSIZE = 30, // 0x1E
            SM_CYSIZE = 31, // 0x1F
            SM_CXSIZEFRAME = 32, // 0x20
            SM_CXFRAME = 32, // 0x20
            SM_CYSIZEFRAME = 33, // 0x21
            SM_CYFRAME = 33, // 0x21
            SM_CXMINTRACK = 34, // 0x22
            SM_CYMINTRACK = 35, // 0x23
            SM_CXDOUBLECLK = 36, // 0x24
            SM_CYDOUBLECLK = 37, // 0x25
            SM_CXICONSPACING = 38, // 0x26
            SM_CYICONSPACING = 39, // 0x27
            SM_MENUDROPALIGNMENT = 40, // 0x28
            SM_PENWINDOWS = 41, // 0x29
            SM_DBCSENABLED = 42, // 0x2A
            SM_CMOUSEBUTTONS = 43, // 0x2B
            SM_SECURE = 44, // 0x2C
            SM_CXEDGE = 45, // 0x2D
            SM_CYEDGE = 46, // 0x2E
            SM_CXMINSPACING = 47, // 0x2F
            SM_CYMINSPACING = 48, // 0x30
            SM_CXSMICON = 49, // 0x31
            SM_CYSMICON = 50, // 0x32
            SM_CYSMCAPTION = 51, // 0x33
            SM_CXSMSIZE = 52, // 0x34
            SM_CYSMSIZE = 53, // 0x35
            SM_CXMENUSIZE = 54, // 0x36
            SM_CYMENUSIZE = 55, // 0x37
            SM_ARRANGE = 56, // 0x38
            SM_CXMINIMIZED = 57, // 0x39
            SM_CYMINIMIZED = 58, // 0x3A
            SM_CXMAXTRACK = 59, // 0x3B
            SM_CYMAXTRACK = 60, // 0x3C
            SM_CXMAXIMIZED = 61, // 0x3D
            SM_CYMAXIMIZED = 62, // 0x3E
            SM_NETWORK = 63, // 0x3F
            SM_CLEANBOOT = 67, // 0x43
            SM_CXDRAG = 68, // 0x44
            SM_CYDRAG = 69, // 0x45
            SM_SHOWSOUNDS = 70, // 0x46
            SM_CXMENUCHECK = 71, // 0x47
            SM_CYMENUCHECK = 72, // 0x48
            SM_SLOWMACHINE = 73, // 0x49
            SM_MIDEASTENABLED = 74, // 0x4A
            SM_MOUSEWHEELPRESENT = 75, // 0x4B
            SM_XVIRTUALSCREEN = 76, // 0x4C
            SM_YVIRTUALSCREEN = 77, // 0x4D
            SM_CXVIRTUALSCREEN = 78, // 0x4E
            SM_CYVIRTUALSCREEN = 79, // 0x4F
            SM_CMONITORS = 80, // 0x50
            SM_SAMEDISPLAYFORMAT = 81, // 0x51
            SM_IMMENABLED = 82, // 0x52
            SM_CXFOCUSBORDER = 83, // 0x53
            SM_CYFOCUSBORDER = 84, // 0x54
            SM_TABLETPC = 86, // 0x56
            SM_MEDIACENTER = 87, // 0x57
            SM_STARTER = 88, // 0x58
            SM_SERVERR2 = 89, // 0x59
            SM_MOUSEHORIZONTALWHEELPRESENT = 91, // 0x5B
            SM_CXPADDEDBORDER = 92, // 0x5C
            SM_DIGITIZER = 94, // 0x5E
            SM_MAXIMUMTOUCHES = 95, // 0x5F

            SM_REMOTESESSION = 0x1000, // 0x1000
            SM_SHUTTINGDOWN = 0x2000, // 0x2000
            SM_REMOTECONTROL = 0x2001, // 0x2001

            SM_CONVERTABLESLATEMODE = 0x2003,
            SM_SYSTEMDOCKED = 0x2004,
        }

        [Flags]
        public enum ModifierKeys
        {
            MK_CONTROL = 0x0008,

            MK_LBUTTON = 0x0001,

            MK_MBUTTON = 0x0010,

            MK_RBUTTON = 0x0002,

            MK_SHIFT = 0x0004,

            MK_ALT = 0x0020,

            MK_XBUTTON1 = 0x0020,

            MK_XBUTTON2 = 0x0040
        }

        public enum WindowActivate
        {
            WA_INACTIVE,
            WA_ACTIVE,
            WA_CLICKACTIVE,
        }

        public enum HitTestValues : int
        {
            HTERROR = -2,
            HTTRANSPARENT = -1,
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21
        }

        public enum SystemCommand
        {
            SW_SCROLLCHILDREN = 0x0001,
            SW_INVALIDATE = 0x0002,
            SW_ERASE = 0x0004,
            SW_SMOOTHSCROLL = 0x0010,
            SC_SIZE = 0xF000,
            SC_MINIMIZE = 0xF020,
            SC_MAXIMIZE = 0xF030,
            SC_CLOSE = 0xF060,
            SC_KEYMENU = 0xF100,
            SC_RESTORE = 0xF120,
            SC_MOVE = 0xF010,
            SS_LEFT = 0x00000000,
            SS_CENTER = 0x00000001,
            SS_RIGHT = 0x00000002,
            SS_OWNERDRAW = 0x0000000D,
            SS_NOPREFIX = 0x00000080,
            SS_SUNKEN = 0x00001000,
            SC_MOUSEMOVE = SC_MOVE + 0x02,
        }

        [Flags]
        public enum WindowStyles : int
        {
            WS_BORDER = 0x800000,
            WS_CAPTION = 0xc00000,
            WS_CHILD = 0x40000000,
            WS_CLIPCHILDREN = 0x2000000,
            WS_CLIPSIBLINGS = 0x4000000,
            WS_DISABLED = 0x8000000,
            WS_DLGFRAME = 0x400000,
            WS_GROUP = 0x20000,
            WS_HSCROLL = 0x100000,
            WS_MAXIMIZE = 0x1000000,
            WS_MAXIMIZEBOX = 0x10000,
            WS_MINIMIZE = 0x20000000,
            WS_MINIMIZEBOX = 0x20000,
            WS_OVERLAPPED = 0x0,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUP = unchecked((int)0x80000000),
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_SIZEFRAME = 0x40000,
            WS_SYSMENU = 0x80000,
            WS_TABSTOP = 0x10000,
            WS_VISIBLE = 0x10000000,
            WS_VSCROLL = 0x200000,
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_NOINHERITLAYOUT = 0x00100000,
            WS_EX_LAYOUTRTL = 0x00400000,
            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000,
            WS_THICKFRAME = 0x00040000,
        }

        [Flags]
        public enum ClassStyles : uint
        {
            CS_VREDRAW = 0x0001,
            CS_HREDRAW = 0x0002,
            CS_DBLCLKS = 0x0008,
            CS_OWNDC = 0x0020,
            CS_CLASSDC = 0x0040,
            CS_PARENTDC = 0x0080,
            CS_NOCLOSE = 0x0200,
            CS_SAVEBITS = 0x0800,
            CS_BYTEALIGNCLIENT = 0x1000,
            CS_BYTEALIGNWINDOW = 0x2000,
            CS_GLOBALCLASS = 0x4000,
            CS_IME = 0x00010000,
            CS_DROPSHADOW = 0x00020000
        }

        public enum WindowsMessage : uint
        {
            WM_NULL = 0x0000,
            WM_CREATE = 0x0001,
            WM_DESTROY = 0x0002,
            WM_MOVE = 0x0003,
            WM_SIZE = 0x0005,
            WM_ACTIVATE = 0x0006,
            WM_SETFOCUS = 0x0007,
            WM_KILLFOCUS = 0x0008,
            WM_ENABLE = 0x000A,
            WM_SETREDRAW = 0x000B,
            WM_SETTEXT = 0x000C,
            WM_GETTEXT = 0x000D,
            WM_GETTEXTLENGTH = 0x000E,
            WM_PAINT = 0x000F,
            WM_CLOSE = 0x0010,
            WM_QUERYENDSESSION = 0x0011,
            WM_QUERYOPEN = 0x0013,
            WM_ENDSESSION = 0x0016,
            WM_QUIT = 0x0012,
            WM_ERASEBKGND = 0x0014,
            WM_SYSCOLORCHANGE = 0x0015,
            WM_SHOWWINDOW = 0x0018,
            WM_WININICHANGE = 0x001A,
            WM_SETTINGCHANGE = WM_WININICHANGE,
            WM_DEVMODECHANGE = 0x001B,
            WM_ACTIVATEAPP = 0x001C,
            WM_FONTCHANGE = 0x001D,
            WM_TIMECHANGE = 0x001E,
            WM_CANCELMODE = 0x001F,
            WM_SETCURSOR = 0x0020,
            WM_MOUSEACTIVATE = 0x0021,
            WM_CHILDACTIVATE = 0x0022,
            WM_QUEUESYNC = 0x0023,
            WM_GETMINMAXINFO = 0x0024,
            WM_PAINTICON = 0x0026,
            WM_ICONERASEBKGND = 0x0027,
            WM_NEXTDLGCTL = 0x0028,
            WM_SPOOLERSTATUS = 0x002A,
            WM_DRAWITEM = 0x002B,
            WM_MEASUREITEM = 0x002C,
            WM_DELETEITEM = 0x002D,
            WM_VKEYTOITEM = 0x002E,
            WM_CHARTOITEM = 0x002F,
            WM_SETFONT = 0x0030,
            WM_GETFONT = 0x0031,
            WM_SETHOTKEY = 0x0032,
            WM_GETHOTKEY = 0x0033,
            WM_QUERYDRAGICON = 0x0037,
            WM_COMPAREITEM = 0x0039,
            WM_GETOBJECT = 0x003D,
            WM_COMPACTING = 0x0041,
            WM_WINDOWPOSCHANGING = 0x0046,
            WM_WINDOWPOSCHANGED = 0x0047,
            WM_COPYDATA = 0x004A,
            WM_CANCELJOURNAL = 0x004B,
            WM_NOTIFY = 0x004E,
            WM_INPUTLANGCHANGEREQUEST = 0x0050,
            WM_INPUTLANGCHANGE = 0x0051,
            WM_TCARD = 0x0052,
            WM_HELP = 0x0053,
            WM_USERCHANGED = 0x0054,
            WM_NOTIFYFORMAT = 0x0055,
            WM_CONTEXTMENU = 0x007B,
            WM_STYLECHANGING = 0x007C,
            WM_STYLECHANGED = 0x007D,
            WM_DISPLAYCHANGE = 0x007E,
            WM_GETICON = 0x007F,
            WM_SETICON = 0x0080,
            WM_NCCREATE = 0x0081,
            WM_NCDESTROY = 0x0082,
            WM_NCCALCSIZE = 0x0083,
            WM_NCHITTEST = 0x0084,
            WM_NCPAINT = 0x0085,
            WM_NCACTIVATE = 0x0086,
            WM_NCUAHDRAWCAPTION = 0x00AE,
            WM_NCUAHDRAWFRAME = 0x00AF,
            WM_GETDLGCODE = 0x0087,
            WM_SYNCPAINT = 0x0088,
            WM_NCMOUSEMOVE = 0x00A0,
            WM_NCLBUTTONDOWN = 0x00A1,
            WM_NCLBUTTONUP = 0x00A2,
            WM_NCLBUTTONDBLCLK = 0x00A3,
            WM_NCRBUTTONDOWN = 0x00A4,
            WM_NCRBUTTONUP = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6,
            WM_NCMBUTTONDOWN = 0x00A7,
            WM_NCMBUTTONUP = 0x00A8,
            WM_NCMBUTTONDBLCLK = 0x00A9,
            WM_NCXBUTTONDOWN = 0x00AB,
            WM_NCXBUTTONUP = 0x00AC,
            WM_NCXBUTTONDBLCLK = 0x00AD,
            WM_INPUT_DEVICE_CHANGE = 0x00FE,
            WM_INPUT = 0x00FF,
            WM_KEYFIRST = 0x0100,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_CHAR = 0x0102,
            WM_DEADCHAR = 0x0103,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_SYSCHAR = 0x0106,
            WM_SYSDEADCHAR = 0x0107,
            WM_UNICHAR = 0x0109,
            WM_KEYLAST = 0x0109,
            WM_IME_STARTCOMPOSITION = 0x010D,
            WM_IME_ENDCOMPOSITION = 0x010E,
            WM_IME_COMPOSITION = 0x010F,
            WM_IME_KEYLAST = 0x010F,
            WM_INITDIALOG = 0x0110,
            WM_COMMAND = 0x0111,
            WM_SYSCOMMAND = 0x0112,
            WM_TIMER = 0x0113,
            WM_HSCROLL = 0x0114,
            WM_VSCROLL = 0x0115,
            WM_INITMENU = 0x0116,
            WM_INITMENUPOPUP = 0x0117,
            WM_MENUSELECT = 0x011F,
            WM_MENUCHAR = 0x0120,
            WM_ENTERIDLE = 0x0121,
            WM_MENURBUTTONUP = 0x0122,
            WM_MENUDRAG = 0x0123,
            WM_MENUGETOBJECT = 0x0124,
            WM_UNINITMENUPOPUP = 0x0125,
            WM_MENUCOMMAND = 0x0126,
            WM_CHANGEUISTATE = 0x0127,
            WM_UPDATEUISTATE = 0x0128,
            WM_QUERYUISTATE = 0x0129,
            WM_CTLCOLORMSGBOX = 0x0132,
            WM_CTLCOLOREDIT = 0x0133,
            WM_CTLCOLORLISTBOX = 0x0134,
            WM_CTLCOLORBTN = 0x0135,
            WM_CTLCOLORDLG = 0x0136,
            WM_CTLCOLORSCROLLBAR = 0x0137,
            WM_CTLCOLORSTATIC = 0x0138,
            WM_MOUSEFIRST = 0x0200,
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDBLCLK = 0x0209,
            WM_MOUSEWHEEL = 0x020A,
            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C,
            WM_XBUTTONDBLCLK = 0x020D,
            WM_MOUSEHWHEEL = 0x020E,
            WM_MOUSELAST = 0x020E,
            WM_PARENTNOTIFY = 0x0210,
            WM_ENTERMENULOOP = 0x0211,
            WM_EXITMENULOOP = 0x0212,
            WM_NEXTMENU = 0x0213,
            WM_SIZING = 0x0214,
            WM_CAPTURECHANGED = 0x0215,
            WM_MOVING = 0x0216,
            WM_POWERBROADCAST = 0x0218,
            WM_DEVICECHANGE = 0x0219,
            WM_MDICREATE = 0x0220,
            WM_MDIDESTROY = 0x0221,
            WM_MDIACTIVATE = 0x0222,
            WM_MDIRESTORE = 0x0223,
            WM_MDINEXT = 0x0224,
            WM_MDIMAXIMIZE = 0x0225,
            WM_MDITILE = 0x0226,
            WM_MDICASCADE = 0x0227,
            WM_MDIICONARRANGE = 0x0228,
            WM_MDIGETACTIVE = 0x0229,
            WM_MDISETMENU = 0x0230,
            WM_ENTERSIZEMOVE = 0x0231,
            WM_EXITSIZEMOVE = 0x0232,
            WM_DROPFILES = 0x0233,
            WM_MDIREFRESHMENU = 0x0234,
            WM_IME_SETCONTEXT = 0x0281,
            WM_IME_NOTIFY = 0x0282,
            WM_IME_CONTROL = 0x0283,
            WM_IME_COMPOSITIONFULL = 0x0284,
            WM_IME_SELECT = 0x0285,
            WM_IME_CHAR = 0x0286,
            WM_IME_REQUEST = 0x0288,
            WM_IME_KEYDOWN = 0x0290,
            WM_IME_KEYUP = 0x0291,
            WM_MOUSEHOVER = 0x02A1,
            WM_MOUSELEAVE = 0x02A3,
            WM_NCMOUSEHOVER = 0x02A0,
            WM_NCMOUSELEAVE = 0x02A2,
            WM_WTSSESSION_CHANGE = 0x02B1,
            WM_TABLET_FIRST = 0x02c0,
            WM_TABLET_LAST = 0x02df,
            WM_DPICHANGED = 0x02E0,
            WM_CUT = 0x0300,
            WM_COPY = 0x0301,
            WM_PASTE = 0x0302,
            WM_CLEAR = 0x0303,
            WM_UNDO = 0x0304,
            WM_RENDERFORMAT = 0x0305,
            WM_RENDERALLFORMATS = 0x0306,
            WM_DESTROYCLIPBOARD = 0x0307,
            WM_DRAWCLIPBOARD = 0x0308,
            WM_PAINTCLIPBOARD = 0x0309,
            WM_VSCROLLCLIPBOARD = 0x030A,
            WM_SIZECLIPBOARD = 0x030B,
            WM_ASKCBFORMATNAME = 0x030C,
            WM_CHANGECBCHAIN = 0x030D,
            WM_HSCROLLCLIPBOARD = 0x030E,
            WM_QUERYNEWPALETTE = 0x030F,
            WM_PALETTEISCHANGING = 0x0310,
            WM_PALETTECHANGED = 0x0311,
            WM_HOTKEY = 0x0312,
            WM_PRINT = 0x0317,
            WM_PRINTCLIENT = 0x0318,
            WM_APPCOMMAND = 0x0319,
            WM_THEMECHANGED = 0x031A,
            WM_CLIPBOARDUPDATE = 0x031D,
            WM_DWMCOMPOSITIONCHANGED = 0x031E,
            WM_DWMNCRENDERINGCHANGED = 0x031F,
            WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320,
            WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321,
            WM_GETTITLEBARINFOEX = 0x033F,
            WM_HANDHELDFIRST = 0x0358,
            WM_HANDHELDLAST = 0x035F,
            WM_AFXFIRST = 0x0360,
            WM_AFXLAST = 0x037F,
            WM_PENWINFIRST = 0x0380,
            WM_PENWINLAST = 0x038F,
            WM_APP = 0x8000,
            WM_USER = 0x0400,
            WM_DISPATCH_WORK_ITEM = WM_USER,
            WM_TOUCH = 0x0240,
            WM_GESTURE = 0x0119,
            //WM_PARENTNOTIFY = 0x0210,
            WM_NCPOINTERUPDATE = 0x0241,
            WM_NCPOINTERDOWN = 0x0242,
            WM_NCPOINTERUP = 0x0243,
            WM_POINTERUPDATE = 0x0245,
            WM_POINTERDOWN = 0x0246,
            WM_POINTERUP = 0x0247,
            WM_POINTERENTER = 0x0249,
            WM_POINTERLEAVE = 0x024A,
            WM_POINTERACTIVATE = 0x024B,
            WM_POINTERCAPTURECHANGED = 0x024C,
            WM_POINTERWHEEL = 0x024E,
            WM_POINTERHWHEEL = 0x024F,
        }

        public enum BitmapCompressionMode : uint
        {
            BI_RGB = 0,
            BI_RLE8 = 1,
            BI_RLE4 = 2,
            BI_BITFIELDS = 3,
            BI_JPEG = 4,
            BI_PNG = 5
        }

        public enum DIBColorTable
        {
            DIB_RGB_COLORS = 0,     /* color table in RGBs */
            DIB_PAL_COLORS          /* color table in palette indices */
        }

        public enum WindowLongParam
        {
            GWL_WNDPROC = -4,
            GWL_HINSTANCE = -6,
            GWL_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_EXSTYLE = -20,
            GWL_USERDATA = -21
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        //[StructLayout(LayoutKind.Sequential)]
        //public struct BITMAPINFOHEADER
        //{
        //    public int biSize;    // ndirect.DllLib.sizeOf( this );
        //    public int biWidth;
        //    public int biHeight;
        //    public short biPlanes;
        //    public short biBitCount;
        //    public int biCompression;
        //    public int biSizeImage;
        //    public int biXPelsPerMeter;
        //    public int biYPelsPerMeter;
        //    public int biClrUsed;
        //    public int biClrImportant;
        //}
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct BITMAPFILEHEADER
        {
            public ushort bfType;
            public uint bfSize;
            public ushort bfReserved1;
            public ushort bfReserved2;
            public uint bfOffBits;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            // C# cannot inlay structs in structs so must expand directly here
            //
            //[StructLayout(LayoutKind.Sequential)]
            //public struct BITMAPINFOHEADER
            //{
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public BitmapCompressionMode biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
            //}

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            //public uint[] cols;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] cols;
        }


        [StructLayout(LayoutKind.Sequential)]
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

        //[StructLayout(LayoutKind.Sequential)]
        //public struct BITMAPINFO_FLAT
        //{
        //    public int bmiHeader_biSize;// = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
        //    public int bmiHeader_biWidth;
        //    public int bmiHeader_biHeight;
        //    public short bmiHeader_biPlanes;
        //    public short bmiHeader_biBitCount;
        //    public int bmiHeader_biCompression;
        //    public int bmiHeader_biSizeImage;
        //    public int bmiHeader_biXPelsPerMeter;
        //    public int bmiHeader_biYPelsPerMeter;
        //    public int bmiHeader_biClrUsed;
        //    public int bmiHeader_biClrImportant;

        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256 * 4)]
        //    public byte[] bmiColors; // RGBQUAD structs... Blue-Green-Red-Reserved, repeat...
        //}


        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        public const int SizeOf_BITMAPINFOHEADER = 40;
        [StructLayout(LayoutKind.Sequential)]
        public struct COMPOSITIONFORM
        {
            public uint dwStyle;
            public POINT ptCurrentPos;
            public RECT rcArea;
        }

        [DllImport("Imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImmGetOpenStatus(IntPtr hIMC);
        [DllImport("Imm32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImmSetConversionStatus(IntPtr hIMC, int conversion, int sentence);

        [DllImport("Imm32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImmSetOpenStatus(IntPtr hIMC, bool open);
        [DllImport("Imm32.dll")]
        public static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("Imm32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImmGetConversionStatus(IntPtr hIMC, ref int conversion, ref int sentence);

        [DllImport("Imm32.dll")]
        public static extern bool ImmAssociateContextEx(IntPtr hWnd, IntPtr hIMC, int dwFlags);
        [DllImport("imm32.dll")]
        public static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, StringBuilder lPBuf, int dwBufLen);
        [DllImport("imm32.dll")]
        public static extern int ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("imm32.dll")]
        public static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompForm);

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImmDestroyContext(IntPtr hIMC);

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ImmCreateContext();

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip,
                                                      MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);
        /// <summary>
        ///        Retrieves the bits of the specified compatible bitmap and copies them into a buffer as a DIB using the specified format.
        /// </summary>
        /// <param name="hdc">A handle to the device context.</param>
        /// <param name="hbmp">A handle to the bitmap. This must be a compatible bitmap (DDB).</param>
        /// <param name="uStartScan">The first scan line to retrieve.</param>
        /// <param name="cScanLines">The number of scan lines to retrieve.</param>
        /// <param name="lpvBits">A pointer to a buffer to receive the bitmap data. If this parameter is <see cref="IntPtr.Zero"/>, the function passes the dimensions and format of the bitmap to the <see cref="BITMAPINFO"/> structure pointed to by the <paramref name="lpbi"/> parameter.</param>
        /// <param name="lpbi">A pointer to a <see cref="BITMAPINFO"/> structure that specifies the desired format for the DIB data.</param>
        /// <param name="uUsage">The format of the bmiColors member of the <see cref="BITMAPINFO"/> structure. It must be one of the following values.</param>
        /// <returns>If the lpvBits parameter is non-NULL and the function succeeds, the return value is the number of scan lines copied from the bitmap.
        /// If the lpvBits parameter is NULL and GetDIBits successfully fills the <see cref="BITMAPINFO"/> structure, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// This function can return the following value: ERROR_INVALID_PARAMETER (87 (0×57))</returns>
        [DllImport("gdi32.dll", EntryPoint = "GetDIBits")]
        public static extern int GetDIBits([In] IntPtr hdc, [In] IntPtr hbmp, uint uStartScan, uint cScanLines, [Out] byte[] lpvBits, ref BITMAPINFOHEADER lpbi, DIB_Color_Mode uUsage);
        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true, EntryPoint = "CreateRectRgn", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);
        [DllImport("gdi32.dll")]
        public static extern IntPtr GetCurrentObject(IntPtr hdc, ObjectType uObjectType);
        // GetObject stuff
        [DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetObject(IntPtr hObject, int nSize, [In, Out] BITMAP bm);
        [DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetObject(IntPtr hObject, int nSize, [In, Out] BITMAPINFO bm);

        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        public static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);
        [DllImport("gdi32.dll", EntryPoint = "GdiAlphaBlend")]
        public static extern bool AlphaBlend(
IntPtr hdcDest, // handle to destination DC
int nXOriginDest, // x-coord of upper-left corner
int nYOriginDest, // y-coord of upper-left corner
int nWidthDest, // destination width
int nHeightDest, // destination height
IntPtr hdcSrc, // handle to source DC
int nXOriginSrc, // x-coord of upper-left corner
int nYOriginSrc, // y-coord of upper-left corner
int nWidthSrc, // source width
int nHeightSrc, // source height
BLENDFUNCTION blendFunction // alpha-blending function
);
        [DllImport("gdi32.dll")]
        public static extern int StretchDIBits(IntPtr hdc, int XDest, int YDest,
   int nDestWidth, int nDestHeight, int XSrc, int YSrc, int nSrcWidth,
   int nSrcHeight, byte[] lpBits, [In] ref BITMAPINFOHEADER lpBitsInfo, uint iUsage,
   uint dwRop);

        [DllImport("gdi32.dll")]
        public static extern int StretchDIBits(IntPtr hdc, int XDest, int YDest,
   int nDestWidth, int nDestHeight, int XSrc, int YSrc, int nSrcWidth,
   int nSrcHeight, IntPtr lpBits, [In] ref BITMAPINFOHEADER lpBitsInfo, uint iUsage,
   uint dwRop);
        [DllImport("gdi32.dll")]
        public static extern int SetDIBitsToDevice(IntPtr hdc, int XDest, int YDest,
            uint dwWidth, uint dwHeight,
            int XSrc, int YSrc,
            uint uStartScan, uint cScanLines,
           IntPtr lpvBits, [In] ref BITMAPINFOHEADER lpbmi, uint fuColorUse);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr DeleteDC(IntPtr hDC);
        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true, EntryPoint = "CreateSolidBrush", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateSolidBrush(int crColor);
        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int SetBkColor(IntPtr hDC, int clr);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool AdjustWindowRectEx(ref RECT lpRect, uint dwStyle, bool bMenu, uint dwExStyle);

        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateWindowEx(int dwExStyle, string lpszClassName, string lpszWindowName, uint style, int x, int y, int width, int height, IntPtr hWndParent, IntPtr hMenu, IntPtr hInst, [MarshalAs(UnmanagedType.AsAny)] object pvParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
           int dwExStyle,
           uint lpClassName,
           string lpWindowName,
           uint dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string msg);
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref POINT pt, int cPoints);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void PostQuitMessage(int nExitCode);
        [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "DispatchMessageW")]
        public static extern IntPtr DispatchMessage(ref MSG lpmsg);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern void FillRect(IntPtr hdc, ref RECT rect, IntPtr brush);

        [DllImport("user32.dll")]
        public static extern uint GetCaretBlinkTime();

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern uint GetDoubleClickTime();

        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMessage(ref MSG msg, IntPtr hwnd, int minFilter, int maxFilter);
        [DllImport("user32.dll", EntryPoint = "GetMessageW")]
        public static extern sbyte GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        public static extern int GetMessageTime();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLong")]
        public static extern uint GetWindowLong32b(IntPtr hWnd, int nIndex);

        public static uint GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLong32b(hWnd, nIndex);
            }
            else
            {
                return GetWindowLongPtr(hWnd, nIndex);
            }
        }

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLong32b(IntPtr hWnd, int nIndex, uint value);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLong64b(IntPtr hWnd, int nIndex, IntPtr value);

        public static uint SetWindowLong(IntPtr hWnd, int nIndex, uint value)
        {
            if (IntPtr.Size == 4)
            {
                return (uint)SetWindowLong32b(hWnd, nIndex, value);
            }
            else
            {
                return (uint)SetWindowLong64b(hWnd, nIndex, new IntPtr((uint)value)).ToInt32();
            }
        }

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr handle)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLong32b(hWnd, nIndex, (uint)handle.ToInt32());
            }
            else
            {
                return SetWindowLong64b(hWnd, nIndex, handle);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool GetUpdateRect(IntPtr hwnd, out RECT lpRect, bool bErase);

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hWnd, ref RECT lpRect, bool bErase);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool RedrawWindow(IntPtr hwnd, ref RECT rcUpdate, IntPtr hrgnUpdate, int flags);

        [DllImport("user32.dll")]
        public static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowUnicode(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool KillTimer(IntPtr hWnd, IntPtr uIDEvent);
        [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)] // 3 = Unicode
        private static extern int GdiplusStartup(out IntPtr token, ref StartupInput input, out StartupOutput output);
        [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)] // 3 = Unicode
        private static extern void GdiplusShutdown(IntPtr token);
        [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, EntryPoint = "GdipDisposeImage", CharSet = CharSet.Unicode)] // 3 = Unicode
        public static extern int GdipDisposeImage(IntPtr image);
        [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)] // 3 = Unicode
        public static extern int GdipCreateBitmapFromStream(IStream stream, out IntPtr bitmap);
        [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)] // 3 = Unicode
        public static extern int GdipCreateBitmapFromStream(IntPtr stream, out IntPtr bitmap);

        [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)] // 3 = Unicode
        public static extern int GdipCreateHICONFromBitmap(IntPtr nativeBitmap, out IntPtr hicon);
        [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)] // 3 = Unicode
        public static extern int GdipCreateHBITMAPFromBitmap(IntPtr nativeBitmap, out IntPtr hbitmap, int argbBackground);

        [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)] // 3 = Unicode
        public static extern int GdipCreateBitmapFromHBITMAP(IntPtr hbitmap, IntPtr hpalette, out IntPtr bitmap);
        [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)] // 3 = Unicode
        internal static extern int GdipSaveImageToStream(IntPtr image, IStream stream,
                                                 ref Guid classId, IntPtr encoderParams);
        [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)] // 3 = Unicode
        internal static extern int GdipGetImageDecodersSize(out int numDecoders, out int size);

        [DllImport("gdiplus.dll", SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)] // 3 = Unicode
        internal static extern int GdipGetImageDecoders(int numDecoders, int size, IntPtr decoders);

        [DllImport("user32.dll", SetLastError = true, ExactSpelling = true, EntryPoint = "DestroyIcon", CharSet = CharSet.Auto)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateIconFromResourceEx")]
        public unsafe static extern IntPtr CreateIconFromResourceEx(byte* pbIconBits, int cbIconBits, bool fIcon, int dwVersion, int csDesired, int cyDesired, int flags);
        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

        [DllImport("user32.dll")]
        static extern IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

        [DllImport("user32.dll")]
        public static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegisterClassExW")]
        public static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern ushort RegisterClass(ref WNDCLASS lpWndClass);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetTimer(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);
        [DllImport("user32.dll")]
        public static extern bool SetFocus(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool SetParent(IntPtr hWnd, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);
        [DllImport("user32.dll", ExactSpelling = true, EntryPoint = "SetWindowRgn", CharSet = CharSet.Auto)]
        public static extern int SetWindowRgn(IntPtr hwnd, IntPtr hrgn, bool fRedraw);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pptSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateTimerQueue();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DeleteTimerQueueEx(IntPtr TimerQueue, IntPtr CompletionEvent);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateTimerQueueTimer(
            out IntPtr phNewTimer,
            IntPtr TimerQueue,
            WaitOrTimerCallback Callback,
            IntPtr Parameter,
            uint DueTime,
            uint Period,
            uint Flags);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DeleteTimerQueueTimer(IntPtr TimerQueue, IntPtr Timer, IntPtr CompletionEvent);

        [DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint virtualKeyCode,
            uint scanCode,
            byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder receivingBuffer,
            int bufferSize,
            uint flags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetWindowTextW")]
        public static extern bool SetWindowText(IntPtr hwnd, string lpString);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterTouchWindow(IntPtr hWnd, RegisterTouchFlags flags);
        [DllImport("user32")]
        public static extern unsafe bool GetTouchInputInfo(IntPtr hTouchInput, uint cInputs, TOUCHINPUT* pInputs, int cbSize);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerInfo(int pointerID, ref POINTER_INFO pointerInfo);

        [DllImport("user32")]
        public static extern bool CloseTouchInputHandle(IntPtr hTouchInput);
        [DllImport("user32.dll")]
        public static extern uint GetMessageExtraInfo();
        public enum ClassLongIndex : int
        {
            GCLP_MENUNAME = -8,
            GCLP_HBRBACKGROUND = -10,
            GCLP_HCURSOR = -12,
            GCLP_HICON = -14,
            GCLP_HMODULE = -16,
            GCL_CBWNDEXTRA = -18,
            GCL_CBCLSEXTRA = -20,
            GCLP_WNDPROC = -24,
            GCL_STYLE = -26,
            GCLP_HICONSM = -34,
            GCW_ATOM = -32
        }

        [DllImport("user32.dll", EntryPoint = "SetClassLongPtr")]
        private static extern IntPtr SetClassLong64(IntPtr hWnd, ClassLongIndex nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetClassLong")]
        private static extern IntPtr SetClassLong32(IntPtr hWnd, ClassLongIndex nIndex, IntPtr dwNewLong);

        public static IntPtr SetClassLong(IntPtr hWnd, ClassLongIndex nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetClassLong32(hWnd, nIndex, dwNewLong);
            }

            return SetClassLong64(hWnd, nIndex, dwNewLong);
        }

        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return GetClassLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
        }

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetCursor")]
        internal static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        internal const uint COINIT_APARTMENTTHREADED = 0x2;
        [DllImport("ole32.dll")]
        internal static extern int CoInitializeEx([In] IntPtr pvReserved, [In] uint dwCoInit);

        [DllImport("ole32.dll", PreserveSig = true)]
        internal static extern int CoCreateInstance(ref Guid clsid,
            IntPtr ignore1, int ignore2, ref Guid iid, [MarshalAs(UnmanagedType.IUnknown), Out] out object pUnkOuter);
        [DllImport("ole32.dll", ExactSpelling = true)]
        internal static extern int CoCreateInstance([In] ref Guid clsid,
                                            [MarshalAs(UnmanagedType.Interface)] object punkOuter,
                                            int context,
                                            [In] ref Guid iid,
                                            [MarshalAs(UnmanagedType.Interface)] out object punk);
        [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] OPENFILENAME_I ofn);


        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int Shell_NotifyIcon(int message, NOTIFYICONDATA pnid);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);
        //[DllImport("Ole32", ExactSpelling = true, CharSet = CharSet.Auto)]
        //public static extern int OleGetClipboard(ref IDataObject data);
        //[DllImport("Ole32", ExactSpelling = true, CharSet = CharSet.Auto)]
        //public static extern int OleSetClipboard(IDataObject pDataObj);
        [DllImport("Ole32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int OleFlushClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool OpenClipboard(IntPtr hWndOwner);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        public static extern bool EmptyClipboard();
        [DllImport("user32.dll")]
        public static extern uint EnumClipboardFormats(uint format);
        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(ClipboardFormat uFormat);
        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(int uFormat);

        [DllImport("user32.dll")]
        public static extern IntPtr SetClipboardData(ClipboardFormat uFormat, IntPtr hMem);
        [DllImport("user32.dll")]
        public static extern IntPtr SetClipboardData(int uFormat, IntPtr hMem);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GlobalLock(IntPtr handle);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern bool GlobalUnlock(IntPtr handle);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GlobalFree(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        [DllImport("kernel32.dll")]
        public static extern unsafe int WideCharToMultiByte(uint cp, uint flags, string pwzSource, int cchSource, byte[] pbDestBuffer, int cbDestBuffer, IntPtr null1, IntPtr null2);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, EntryPoint = "RtlMoveMemory", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern void CopyMemory(IntPtr destData, byte[] srcData, int size);


        [DllImport("comdlg32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetSaveFileNameW")]
        public static extern bool GetSaveFileName(IntPtr lpofn);

        [DllImport("comdlg32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetOpenFileNameW")]
        public static extern bool GetOpenFileName(IntPtr lpofn);

        [DllImport("comdlg32.dll")]
        public static extern int CommDlgExtendedError();

        public static bool ShCoreAvailable => shcore != IntPtr.Zero;

        [DllImport("shcore.dll")]
        public static extern void SetProcessDpiAwareness(PROCESS_DPI_AWARENESS value);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetProcessDpiAwarenessContext(IntPtr dpiAWarenessContext);

        //[DllImport("shcore.dll")]
        //public static extern long GetDpiForMonitor(IntPtr hmonitor, MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY);
        public static GetDpiForMonitor GetDpiForMonitor;

        [DllImport("shcore.dll")]
        public static extern void GetScaleFactorForMonitor(IntPtr hMon, out uint pScale);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromPoint(POINT pt, MONITOR dwFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromRect(RECT rect, MONITOR dwFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, MONITOR dwFlags);

        [DllImport("user32", EntryPoint = "GetMonitorInfoW", ExactSpelling = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo([In] IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("User32.dll")]
        public static extern int SendMessage(
     IntPtr hWnd,     // handle to destination window   
     int Msg,         // message  
     IntPtr wParam,    // first message parameter   
     ref COPYDATASTRUCT pcd // second message parameter   
 );

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]

        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int[] lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]

        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int[] wParam, int[] lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]

        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, ref int wParam, ref int lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]

        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]

        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, StringBuilder lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "PostMessageW")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        //[DllImport("gdi32.dll")]
        //public static extern int SetDIBitsToDevice(IntPtr hdc, int XDest, int YDest, uint
        //        dwWidth, uint dwHeight, int XSrc, int YSrc, uint uStartScan, uint cScanLines,
        //    IntPtr lpvBits, [In] ref BITMAPINFOHEADER lpbmi, uint fuColorUse);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateBitmapIndirect(ref BITMAP bitmap);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDIBitmap(IntPtr hdc, [In] ref BITMAPINFOHEADER
   lpbmih, uint fdwInit, byte[] lpbInit, [In] ref BITMAPINFO lpbmi,
   uint fuUsage);
        [DllImport("gdi32")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFOHEADER bmi, int iUsage, ref int ppvBits, IntPtr hSection, int dwOffset);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateDIBSection(IntPtr hDC, ref BITMAPINFOHEADER pBitmapInfo, int un, out IntPtr lplpVoid, IntPtr handle, int dw);
        [DllImport("gdi32.dll")]
        public static extern int DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile,
            IntPtr lpFileMappingAttributes,
            uint flProtect,
            uint dwMaximumSizeHigh,
            uint dwMaximumSizeLow,
            string lpName);

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CopyMemory(IntPtr dest, IntPtr src, UIntPtr count);
        [DllImport("ole32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int RevokeDragDrop(IntPtr hwnd);
        [DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern HRESULT RegisterDragDrop(IntPtr hwnd, IDropTarget target);
        [DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern HRESULT RegisterDragDrop(IntPtr hwnd, IntPtr target);

        [DllImport("ole32.dll", EntryPoint = "OleInitialize")]
        public static extern HRESULT OleInitialize(IntPtr val);

        [DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern void ReleaseStgMedium(ref STGMEDIUM medium);

        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetClipboardFormatName(int format, StringBuilder lpString, int cchMax);

        [DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int RegisterClipboardFormat(string format);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GlobalSize(IntPtr hGlobal);

        [DllImport("shell32.dll", BestFitMapping = false, CharSet = CharSet.Auto)]
        public static extern int DragQueryFile(IntPtr hDrop, int iFile, StringBuilder lpszFile, int cch);

        [DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true, PreserveSig = false)]
        internal static extern void DoDragDrop(IOleDataObject dataObject, IDropSource dropSource, int allowedEffects, int[] finalEffect);

        //[DllImport("shell32.dll", CharSet = CharSet.Auto)]
        //public static extern int DragQueryFile(HandleRef hDrop, int iFile, StringBuilder lpszFile, int cch);

        //[DllImport("dwmapi.dll", PreserveSig = false)]
        //public static extern bool DwmIsCompositionEnabled();
        [DllImport("dwmapi.dll", BestFitMapping = false)]
        public static extern int DwmIsCompositionEnabled(out Int32 enabled);

        [DllImport("dwmapi.dll")]
        public static extern int DwmEnableBlurBehindWindow(IntPtr hWnd, ref DWM_BLURBEHIND pBlurBehind);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        private const int Win32RedShift = 0;
        private const int Win32GreenShift = 8;
        private const int Win32BlueShift = 16;
        public static int ToWin32(Color c)
        {
            return c.R << Win32RedShift | c.G << Win32GreenShift | c.B << Win32BlueShift;
        }
        public static int DragQueryFileLongPath(IntPtr hDrop, int iFile, StringBuilder lpszFile)
        {
            if (null != lpszFile && 0 != lpszFile.Capacity && iFile != unchecked((int)0xFFFFFFFF))
            {
                int resultValue = 0;

                // iterating by allocating chunk of memory each time we find the length is not sufficient.
                // Performance should not be an issue for current MAX_PATH length due to this
                if ((resultValue = DragQueryFile(hDrop, iFile, lpszFile, lpszFile.Capacity)) == lpszFile.Capacity)
                {
                    // passing null for buffer will return actual number of charectors in the file name.
                    // So, one extra call would be suffice to avoid while loop in case of long path.
                    int capacity = DragQueryFile(hDrop, iFile, null, 0);
                    if (capacity < short.MaxValue)
                    {
                        lpszFile.EnsureCapacity(capacity);
                        resultValue = DragQueryFile(hDrop, iFile, lpszFile, capacity);
                    }
                    else
                    {
                        resultValue = 0;
                    }
                }

                lpszFile.Length = resultValue;
                return resultValue;  // what ever the result.
            }
            else
            {
                return DragQueryFile(hDrop, iFile, lpszFile, lpszFile.Capacity);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            /// <summary>
            /// 不使用
            /// </summary>
            public IntPtr dwData;
            /// <summary>
            /// 数据长度
            /// </summary>
            public int cbData;
            //[MarshalAs(UnmanagedType.LPStr)]
            /// <summary>
            /// 数据地址
            /// </summary>
            public IntPtr lpData;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class NOTIFYICONDATA
        {
            public int cbSize = Marshal.SizeOf(typeof(NOTIFYICONDATA));
            public IntPtr hWnd;
            public int uID;
            public int uFlags;
            public int uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;
            public int dwState = 0;
            public int dwStateMask = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szInfo;
            public int uTimeoutOrVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szInfoTitle;
            public int dwInfoFlags;
        }

        public const uint DWM_BB_ENABLE = 0x00000001;
        public const uint DWM_BB_BLURREGION = 0x00000002;
        public const uint DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;

        public const int RPC_E_CHANGED_MODE = unchecked((int)0x80010106),
    RPC_E_CANTCALLOUT_ININPUTSYNCCALL = unchecked((int)0x8001010D),
    RGN_AND = 1,
    RGN_XOR = 3,
    RGN_DIFF = 4,
    RDW_INVALIDATE = 0x0001,
    RDW_ERASE = 0x0004,
    RDW_ALLCHILDREN = 0x0080,
    RDW_ERASENOW = 0x0200,
    RDW_UPDATENOW = 0x0100,
    RDW_FRAME = 0x0400,
    RB_INSERTBANDA = (0x0400 + 1),
    RB_INSERTBANDW = (0x0400 + 10);

        public const int NIM_ADD = 0x00000000,
      NIM_MODIFY = 0x00000001,
      NIM_DELETE = 0x00000002,
      NIF_MESSAGE = 0x00000001,
      NIM_SETVERSION = 0x00000004,
      NIF_ICON = 0x00000002,
      NIF_INFO = 0x00000010,
      NIF_TIP = 0x00000004,
      NIIF_NONE = 0x00000000,
      NIIF_INFO = 0x00000001,
      NIIF_WARNING = 0x00000002,
      NIIF_ERROR = 0x00000003,
      NIN_BALLOONSHOW = ((int)WindowsMessage.WM_USER + 2),
      NIN_BALLOONHIDE = ((int)WindowsMessage.WM_USER + 3),
      NIN_BALLOONTIMEOUT = ((int)WindowsMessage.WM_USER + 4),
      NIN_BALLOONUSERCLICK = ((int)WindowsMessage.WM_USER + 5),
      NFR_ANSI = 1,
      NFR_UNICODE = 2,
      NM_CLICK = ((0 - 0) - 2),
      NM_DBLCLK = ((0 - 0) - 3),
      NM_RCLICK = ((0 - 0) - 5),
      NM_RDBLCLK = ((0 - 0) - 6),
      NM_CUSTOMDRAW = ((0 - 0) - 12),
      NM_RELEASEDCAPTURE = ((0 - 0) - 16),
      NONANTIALIASED_QUALITY = 3,
            MAX_PATH = 260,
            OFN_READONLY = 0x00000001,
        OFN_OVERWRITEPROMPT = 0x00000002,
        OFN_HIDEREADONLY = 0x00000004,
        OFN_NOCHANGEDIR = 0x00000008,
        OFN_SHOWHELP = 0x00000010,
        OFN_ENABLEHOOK = 0x00000020,
        OFN_NOVALIDATE = 0x00000100,
        OFN_ALLOWMULTISELECT = 0x00000200,
        OFN_PATHMUSTEXIST = 0x00000800,
        OFN_FILEMUSTEXIST = 0x00001000,
        OFN_CREATEPROMPT = 0x00002000,
        OFN_EXPLORER = 0x00080000,
        OFN_NODEREFERENCELINKS = 0x00100000,
        OFN_ENABLESIZING = 0x00800000,
        OFN_USESHELLITEM = 0x01000000
            ;

        public enum MONITOR
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;

            public static MONITORINFO Create()
            {
#if Net4
                return new MONITORINFO() { cbSize = Marshal.SizeOf(typeof(MONITORINFO)) };
#else
                return new MONITORINFO() { cbSize = Marshal.SizeOf<MONITORINFO>() };
#endif
            }

            public enum MonitorOptions : uint
            {
                MONITOR_DEFAULTTONULL = 0x00000000,
                MONITOR_DEFAULTTOPRIMARY = 0x00000001,
                MONITOR_DEFAULTTONEAREST = 0x00000002
            }
        }

        public enum PROCESS_DPI_AWARENESS
        {
            PROCESS_DPI_UNAWARE = 0,
            PROCESS_SYSTEM_DPI_AWARE = 1,
            PROCESS_PER_MONITOR_DPI_AWARE = 2
        }

        public enum MONITOR_DPI_TYPE
        {
            MDT_EFFECTIVE_DPI = 0,
            MDT_ANGULAR_DPI = 1,
            MDT_RAW_DPI = 2,
            MDT_DEFAULT = MDT_EFFECTIVE_DPI
        }

        public enum ClipboardFormat
        {
            /// <summary>
            /// Text format. Each line ends with a carriage return/linefeed (CR-LF) combination. A null character signals the end of the data. Use this format for ANSI text.
            /// </summary>
            CF_TEXT = 1,
            /// <summary>
            /// A handle to a bitmap
            /// </summary>
            CF_BITMAP = 2,
            CF_METAFILEPICT = 3,
            CF_SYLK = 4,
            CF_DIF = 5,

            CF_TIFF = 6,
            CF_OEMTEXT = 7,
            /// <summary>
            /// A memory object containing a BITMAPINFO structure followed by the bitmap bits.
            /// </summary>
            CF_DIB = 8,
            CF_PALETTE = 9,
            CF_PENDATA = 10,
            CF_RIFF = 11,
            CF_WAVE = 12,
            /// <summary>
            /// Unicode text format. Each line ends with a carriage return/linefeed (CR-LF) combination. A null character signals the end of the data.
            /// </summary>
            CF_UNICODETEXT = 13,
            CF_ENHMETAFILE = 14,
            /// <summary>
            /// A handle to type HDROP that identifies a list of files. 
            /// </summary>
            CF_HDROP = 15,
            CF_LOCALE = 16,
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        public struct DWM_BLURBEHIND
        {
            public uint dwFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fEnable;
            public IntPtr hRegionBlur;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fTransitionOnMaximized;
        }

        public struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }


        [StructLayout(LayoutKind.Sequential)]
        public class MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor; // The display monitor rectangle  
            public RECT rcWork; // The working area rectangle  
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public char[] szDevice;
        }

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public int Width => right - left;
            public int Height => bottom - top;
            public RECT(Rect rect)
            {
                left = (int)rect.X;
                top = (int)rect.Y;
                right = (int)(rect.X + rect.Width);
                bottom = (int)(rect.Y + rect.Height);
            }
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
            public void Offset(POINT pt)
            {
                left += pt.X;
                right += pt.X;
                top += pt.Y;
                bottom += pt.Y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NCCALCSIZE_PARAMS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public RECT[] rgrc;
            public WINDOWPOS lppos;
        }

        public struct TRACKMOUSEEVENT
        {
            public int cbSize;
            public uint dwFlags;
            public IntPtr hwndTrack;
            public int dwHoverTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            /// <summary>
            /// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
            /// <para>
            /// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
            /// </para>
            /// </summary>
            public int Length;

            /// <summary>
            /// Specifies flags that control the position of the minimized window and the method by which the window is restored.
            /// </summary>
            public int Flags;

            /// <summary>
            /// The current show state of the window.
            /// </summary>
            public ShowWindowCommand ShowCmd;

            /// <summary>
            /// The coordinates of the window's upper-left corner when the window is minimized.
            /// </summary>
            public POINT MinPosition;

            /// <summary>
            /// The coordinates of the window's upper-left corner when the window is maximized.
            /// </summary>
            public POINT MaxPosition;

            /// <summary>
            /// The window's coordinates when the window is in the restored position.
            /// </summary>
            public RECT NormalPosition;

            /// <summary>
            /// Gets the default (empty) value.
            /// </summary>
            public static WINDOWPLACEMENT Default
            {
                get
                {
                    WINDOWPLACEMENT result = new WINDOWPLACEMENT();
                    result.Length = Marshal.SizeOf(result);
                    return result;
                }
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_INFO
        {
            public POINTER_INPUT_TYPE pointerType;
            public int PointerID;
            public int FrameID;
            public POINTER_FLAGS PointerFlags;
            public IntPtr SourceDevice;
            public IntPtr WindowTarget;
            [MarshalAs(UnmanagedType.Struct)]
            public POINT PtPixelLocation;
            [MarshalAs(UnmanagedType.Struct)]
            public POINT PtPixelLocationRaw;
            [MarshalAs(UnmanagedType.Struct)]
            public POINT PtHimetricLocation;
            [MarshalAs(UnmanagedType.Struct)]
            public POINT PtHimetricLocationRaw;
            public uint Time;
            public uint HistoryCount;
            public uint InputData;
            public VIRTUAL_KEY_STATES KeyStates;
            public long PerformanceCount;
            public int ButtonChangeType;
        }
        [Flags]
        public enum VIRTUAL_KEY_STATES
        {
            NONE = 0x0000,
            LBUTTON = 0x0001,
            RBUTTON = 0x0002,
            SHIFT = 0x0004,
            CTRL = 0x0008,
            MBUTTON = 0x0010,
            XBUTTON1 = 0x0020,
            XBUTTON2 = 0x0040
        }
        public enum POINTER_INPUT_TYPE
        {
            POINTER = 0x00000001,
            TOUCH = 0x00000002,
            PEN = 0x00000003,
            MOUSE = 0x00000004
        }

        [Flags]
        public enum POINTER_FLAGS
        {
            NONE = 0x00000000,
            NEW = 0x00000001,
            INRANGE = 0x00000002,
            INCONTACT = 0x00000004,
            FIRSTBUTTON = 0x00000010,
            SECONDBUTTON = 0x00000020,
            THIRDBUTTON = 0x00000040,
            FOURTHBUTTON = 0x00000080,
            FIFTHBUTTON = 0x00000100,
            PRIMARY = 0x00002000,
            CONFIDENCE = 0x00004000,
            CANCELED = 0x00008000,
            DOWN = 0x00010000,
            UPDATE = 0x00020000,
            UP = 0x00040000,
            WHEEL = 0x00080000,
            HWHEEL = 0x00100000,
            CAPTURECHANGED = 0x00200000,
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WNDCLASSEX
        {
            public int cbSize;
            public int style;
            public WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WNDCLASS
        {
            public uint style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszClassName;
        }
        public enum RegisterTouchFlags
        {
            TWF_NONE = 0x00000000,
            TWF_FINETOUCH = 0x00000001, //Specifies that hWnd prefers noncoalesced touch input.

            TWF_WANTPALM = 0x00000002 //Setting this flag disables palm rejection which reduces delays for getting WM_TOUCH messages.
        }
        [Flags]
        public enum OpenFileNameFlags
        {
            OFN_ALLOWMULTISELECT = 0x00000200,
            OFN_EXPLORER = 0x00080000,
            OFN_HIDEREADONLY = 0x00000004,
            OFN_NOREADONLYRETURN = 0x00008000,
            OFN_OVERWRITEPROMPT = 0x00000002
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
        public enum Icons
        {
            ICON_SMALL = 0,
            ICON_BIG = 1
        }

        public const uint SIGDN_FILESYSPATH = 0x80058000;

        [Flags]
        public enum FOS : uint
        {
            FOS_OVERWRITEPROMPT = 0x00000002,
            FOS_STRICTFILETYPES = 0x00000004,
            FOS_NOCHANGEDIR = 0x00000008,
            FOS_PICKFOLDERS = 0x00000020,
            FOS_FORCEFILESYSTEM = 0x00000040, // Ensure that items returned are filesystem items.
            FOS_ALLNONSTORAGEITEMS = 0x00000080, // Allow choosing items that have no storage.
            FOS_NOVALIDATE = 0x00000100,
            FOS_ALLOWMULTISELECT = 0x00000200,
            FOS_PATHMUSTEXIST = 0x00000800,
            FOS_FILEMUSTEXIST = 0x00001000,
            FOS_CREATEPROMPT = 0x00002000,
            FOS_SHAREAWARE = 0x00004000,
            FOS_NOREADONLYRETURN = 0x00008000,
            FOS_NOTESTFILECREATE = 0x00010000,
            FOS_HIDEMRUPLACES = 0x00020000,
            FOS_HIDEPINNEDPLACES = 0x00040000,
            FOS_NODEREFERENCELINKS = 0x00100000,
            FOS_DONTADDTORECENT = 0x02000000,
            FOS_FORCESHOWHIDDEN = 0x10000000,
            FOS_DEFAULTNOMINIMODE = 0x20000000
        }

        public static class ShellIds
        {
            public static readonly Guid OpenFileDialog = Guid.Parse("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7");
            public static readonly Guid SaveFileDialog = Guid.Parse("C0B4E2F3-BA21-4773-8DBA-335EC946EB8B");
            public static readonly Guid IFileDialog = Guid.Parse("42F85136-DB7E-439C-85F1-E4075D135FC8");
            public static readonly Guid IShellItem = Guid.Parse("43826D1E-E718-42EE-BC55-A1E261C37BFE");
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;

            public BLENDFUNCTION(byte alpha)
            {
                BlendOp = 0;
                BlendFlags = 0;
                AlphaFormat = 0;
                SourceConstantAlpha = alpha;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOUCHINPUT
        {
            public int X;
            public int Y;
            public IntPtr Source;
            public uint Id;
            public TouchInputFlags Flags;
            public int Mask;
            public uint Time;
            public IntPtr ExtraInfo;
            public int CxContact;
            public int CyContact;
        }
    }


    public delegate long GetDpiForMonitor(IntPtr hmonitor, UnmanagedMethods.MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY);

    public enum HRESULT : int
    {
        S_OK = 0,
        S_FALSE = 1,
        DRAGDROP_S_DROP = 0x00040100,
        DRAGDROP_S_CANCEL = 0x00040101,
        DRAGDROP_S_USEDEFAULTCURSORS = 0x00040102,
        DISP_E_MEMBERNOTFOUND = unchecked((int)0x80020003),
        DISP_E_PARAMNOTFOUND = unchecked((int)0x80020004),
        DISP_E_UNKNOWNNAME = unchecked((int)0x80020006),
        DISP_E_EXCEPTION = unchecked((int)0x80020009),
        DISP_E_UNKNOWNLCID = unchecked((int)0x8002000C),
        TYPE_E_BADMODULEKIND = unchecked((int)0x800288BD),
        E_NOTIMPL = unchecked((int)0x80004001),
        E_NOINTERFACE = unchecked((int)0x80004002),
        E_POINTER = unchecked((int)0x80004003),
        E_ABORT = unchecked((int)0x80004004),
        E_FAIL = unchecked((int)0x80004005),
        OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003),
        OLE_E_NOCONNECTION = unchecked((int)0x80040004),
        OLE_E_PROMPTSAVECANCELLED = unchecked((int)0x8004000C),
        OLE_E_INVALIDRECT = unchecked((int)0x8004000D),
        DV_E_FORMATETC = unchecked((int)0x80040064),
        DV_E_TYMED = unchecked((int)0x80040069),
        DV_E_DVASPECT = unchecked((int)0x8004006B),
        DRAGDROP_E_NOTREGISTERED = unchecked((int)0x80040100),
        DRAGDROP_E_ALREADYREGISTERED = unchecked((int)0x80040101),
        VIEW_E_DRAW = unchecked((int)0x80040140),
        INPLACE_E_NOTOOLSPACE = unchecked((int)0x800401A1),
        STG_E_INVALIDFUNCTION = unchecked((int)0x80030001L),
        STG_E_FILENOTFOUND = unchecked((int)0x80030002),
        STG_E_ACCESSDENIED = unchecked((int)0x80030005),
        STG_E_INVALIDPARAMETER = unchecked((int)0x80030057),
        STG_E_INVALIDFLAG = unchecked((int)0x800300FF),
        E_OUTOFMEMORY = unchecked((int)0x8007000E),
        E_ACCESSDENIED = unchecked((int)0x80070005L),
        E_INVALIDARG = unchecked((int)0x80070057),
        ERROR_CANCELLED = unchecked((int)0x800704C7),
        RPC_E_CHANGED_MODE = unchecked((int)0x80010106),
        STG_E_INVALIDPOINTER = -2147287031,
    }

    [Flags]
    public enum TouchInputFlags
    {
        /// <summary>
        /// Movement has occurred. Cannot be combined with TOUCHEVENTF_DOWN.
        /// </summary>
        TOUCHEVENTF_MOVE = 0x0001,

        /// <summary>
        /// The corresponding touch point was established through a new contact. Cannot be combined with TOUCHEVENTF_MOVE or TOUCHEVENTF_UP.
        /// </summary>
        TOUCHEVENTF_DOWN = 0x0002,

        /// <summary>
        /// A touch point was removed.
        /// </summary>
        TOUCHEVENTF_UP = 0x0004,

        /// <summary>
        /// A touch point is in range. This flag is used to enable touch hover support on compatible hardware. Applications that do not want support for hover can ignore this flag.
        /// </summary>
        TOUCHEVENTF_INRANGE = 0x0008,

        /// <summary>
        /// Indicates that this TOUCHINPUT structure corresponds to a primary contact point. See the following text for more information on primary touch points.
        /// </summary>
        TOUCHEVENTF_PRIMARY = 0x0010,

        /// <summary>
        /// When received using GetTouchInputInfo, this input was not coalesced.
        /// </summary>
        TOUCHEVENTF_NOCOALESCE = 0x0020,

        /// <summary>
        /// The touch event came from the user's palm.
        /// </summary>
        TOUCHEVENTF_PALM = 0x0080
    }
    [Flags]
    public enum DropEffect : int
    {
        None = 0,
        Copy = 1,
        Move = 2,
        Link = 4,
        Scroll = -2147483648,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct COMDLG_FILTERSPEC
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszSpec;
    }

    internal struct COMDLG_FILTERSPEC_native
    {
        // Token: 0x04000008 RID: 8
        public IntPtr pszName;

        // Token: 0x04000009 RID: 9
        public IntPtr pszSpec;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
        public POINT(int dw)
        {
            unchecked
            {
                this.X = (short)LOWORD(dw);
                this.Y = (short)HIWORD(dw);
            }
        }
        private static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }

        private static int LOWORD(int n)
        {
            return n & 0xffff;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public int cx;
        public int cy;

        public SIZE(Int32 x, Int32 y)
        {
            cx = x;
            cy = y;
        }

    }

    [ComImport(), Guid("42F85136-DB7E-439C-85F1-E4075D135FC8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileDialog
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig()]
        uint Show([In, Optional] IntPtr hwndOwner); //IModalWindow


        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetFileTypes(uint cFileTypes, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] COMDLG_FILTERSPEC[] rgFilterSpec);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetFileTypeIndex([In] uint iFileType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint GetFileTypeIndex(out uint piFileType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint Advise([In, MarshalAs(UnmanagedType.Interface)] IntPtr pfde, out uint pdwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint Unadvise([In] uint dwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetOptions([In] uint fos);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint GetOptions(out uint fos);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, uint fdap);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint Close([MarshalAs(UnmanagedType.Error)] uint hr);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetClientGuid([In] ref Guid guid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint ClearClientData();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

    }

    [ComImport, Guid("d57c7288-d4ad-4768-be02-9d969532d960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileOpenDialog
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig()]
        uint Show([In, Optional] IntPtr hwndOwner); //IModalWindow 

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetFileTypes([In] uint cFileTypes, COMDLG_FILTERSPEC[] rgFilterSpec);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypeIndex([In] uint iFileType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileTypeIndex(out uint piFileType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint Advise([In, MarshalAs(UnmanagedType.Interface)] IntPtr pfde, out uint pdwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Unadvise([In] uint dwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint SetOptions([In] uint fos);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint GetOptions(out uint fos);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, uint fdap);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Close([MarshalAs(UnmanagedType.Error)] int hr);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetClientGuid([In] ref Guid guid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ClearClientData();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
    }

    [ComImport, Guid("B63EA76D-1F85-456F-A19C-48159EFA858B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItemArray
    {
        [PreserveSig]
        HRESULT BindToHandler(
            IntPtr pbc,
            ref Guid rbhid,
            ref Guid riid,
            out IntPtr ppvOut);

        [PreserveSig]
        HRESULT GetPropertyStore(
            GETPROPERTYSTOREFLAGS flags,
            ref Guid riid,
            out IntPtr ppv);

        [PreserveSig]
        HRESULT GetPropertyDescriptionList(
            ref PROPERTYKEY keyType,
            ref Guid riid,
            out IntPtr ppv);

        [PreserveSig]
        HRESULT GetAttributes(
            SIATTRIBFLAGS dwAttribFlags,
            uint sfgaoMask,
            out uint psfgaoAttribs);

        void GetCount(
            out uint pdwNumItems);

        void GetItemAt(
            uint dwIndex,
            out IShellItem ppsi);

        [PreserveSig]
        HRESULT EnumItems(
            out IntPtr ppenumShellItems);
    }
    [Flags]
    public enum GETPROPERTYSTOREFLAGS : uint
    {
        DEFAULT = 0x00000000,
        HANDLERPROPERTIESONLY = 0x00000001,
        READWRITE = 0x00000002,
        TEMPORARY = 0x00000004,
        FASTPROPERTIESONLY = 0x00000008,
        OPENSLOWITEM = 0x00000010,
        DELAYCREATION = 0x00000020,
        BESTEFFORT = 0x00000040,
        NO_OPLOCK = 0x00000080,
        PREFERQUERYPROPERTIES = 0x00000100,
        EXTRINSICPROPERTIES = 0x00000200,
        EXTRINSICPROPERTIESONLY = 0x00000400,
        VOLATILEPROPERTIES = 0x00000800,
        VOLATILEPROPERTIESONLY = 0x00001000,
        MASK_VALID = 0x00001FFF
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PROPERTYKEY
    {
        public Guid fmtid;
        public uint pid;
    }

    public enum SIATTRIBFLAGS
    {
        SIATTRIBFLAGS_AND = 1,
        SIATTRIBFLAGS_APPCOMPAT = 3,
        SIATTRIBFLAGS_OR = 2
    }

    [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItem
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint BindToHandler([In] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IntPtr ppvOut);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint GetDisplayName([In] uint sigdnName, out IntPtr ppszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        uint Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);

    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000122-0000-0000-C000-000000000046")]
    public interface IDropTarget
    {
        [PreserveSig]
        HRESULT DragEnter([MarshalAs(UnmanagedType.Interface)][In] IOleDataObject pDataObj, uint grfKeyState, POINT pt, ref uint pdwEffect);
        [PreserveSig]
        HRESULT DragOver(uint grfKeyState, POINT pt, ref uint pdwEffect);
        [PreserveSig]
        HRESULT DragLeave();
        [PreserveSig]
        HRESULT Drop([MarshalAs(UnmanagedType.Interface)][In] IOleDataObject pDataObj, uint grfKeyState, POINT pt, ref uint pdwEffect);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000121-0000-0000-C000-000000000046")]
    public interface IDropSource
    {
        [PreserveSig]
        int QueryContinueDrag(int fEscapePressed, [MarshalAs(UnmanagedType.U4)][In] int grfKeyState);
        [PreserveSig]
        int GiveFeedback([MarshalAs(UnmanagedType.U4)][In] int dwEffect);
    }


    //[ComImport]
    //[Guid("0000010E-0000-0000-C000-000000000046")]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //public interface IOleDataObject
    //{
    //    void GetData([In] ref FORMATETC format, out STGMEDIUM medium);
    //    void GetDataHere([In] ref FORMATETC format, ref STGMEDIUM medium);
    //    [PreserveSig]
    //    int QueryGetData([In] ref FORMATETC format);
    //    [PreserveSig]
    //    int GetCanonicalFormatEtc([In] ref FORMATETC formatIn, out FORMATETC formatOut);
    //    void SetData([In] ref FORMATETC formatIn, [In] ref STGMEDIUM medium, [MarshalAs(UnmanagedType.Bool)] bool release);
    //    IEnumFORMATETC EnumFormatEtc(DATADIR direction);
    //    [PreserveSig]
    //    int DAdvise([In] ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection);
    //    void DUnadvise(int connection);
    //    [PreserveSig]
    //    int EnumDAdvise(out IEnumSTATDATA enumAdvise);
    //}


    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct _DROPFILES
    {
        public Int32 pFiles;
        public Int32 X;
        public Int32 Y;
        public bool fNC;
        public bool fWide;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct ICONDIR
    {
        public short idReserved;
        public short idType;
        public short idCount;
        public ICONDIRENTRY idEntries;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ICONDIRENTRY
    {
        public byte bWidth;
        public byte bHeight;
        public byte bColorCount;
        public byte bReserved;
        public short wPlanes;
        public short wBitCount;
        public int dwBytesInRes;
        public int dwImageOffset;
    }

    [ComImport(), Guid("0000000C-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IStream
    {
        // Token: 0x06000C70 RID: 3184
        unsafe void Read(byte* pv, uint cb, uint* pcbRead);

        // Token: 0x06000C71 RID: 3185
        unsafe void Write(byte* pv, uint cb, uint* pcbWritten);

        // Token: 0x06000C72 RID: 3186
        unsafe void Seek(long dlibMove, SeekOrigin dwOrigin, ulong* plibNewPosition);

        // Token: 0x06000C73 RID: 3187
        void SetSize(ulong libNewSize);

        // Token: 0x06000C74 RID: 3188
        unsafe HRESULT CopyTo(IntPtr pstm, ulong cb, ulong* pcbRead, ulong* pcbWritten);

        // Token: 0x06000C75 RID: 3189
        void Commit(uint grfCommitFlags);

        // Token: 0x06000C76 RID: 3190
        void Revert();

        // Token: 0x06000C77 RID: 3191
        HRESULT LockRegion(ulong libOffset, ulong cb, uint dwLockType);

        // Token: 0x06000C78 RID: 3192
        HRESULT UnlockRegion(ulong libOffset, ulong cb, uint dwLockType);

        // Token: 0x06000C79 RID: 3193
        unsafe void Stat(STATSTG* pstatstg, STATFLAG grfStatFlag);

        // Token: 0x06000C7A RID: 3194
        unsafe HRESULT Clone(IntPtr* ppstm);
    }

    /// <summary>
    /// Stat flags for <see cref="IStream.Stat(out STATSTG, STATFLAG)"/>.
    /// <see href="https://docs.microsoft.com/en-us/windows/desktop/api/wtypes/ne-wtypes-tagstatflag"/>
    /// </summary>
    public enum STATFLAG : uint
    {
        /// <summary>
        /// Stat includes the name.
        /// </summary>
        STATFLAG_DEFAULT = 0,

        /// <summary>
        /// Stat doesn't include the name.
        /// </summary>
        STATFLAG_NONAME = 1
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct StartupInput
    {
        public int GdiplusVersion;             // Must be 1

        // public DebugEventProc DebugEventCallback; // Ignored on free builds
        public IntPtr DebugEventCallback;

        public bool SuppressBackgroundThread;     // FALSE unless you're prepared to call 
                                                  // the hook/unhook functions properly

        public bool SuppressExternalCodecs;       // FALSE unless you want GDI+ only to use
                                                  // its internal image codecs.

        public static StartupInput GetDefault()
        {
            StartupInput result = new StartupInput();
            result.GdiplusVersion = 1;
            // result.DebugEventCallback = null;
            result.SuppressBackgroundThread = false;
            result.SuppressExternalCodecs = false;
            return result;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OPENFILENAME_I
    {
        public int lStructSize = Marshal.SizeOf(typeof(OPENFILENAME_I)); //ndirect.DllLib.sizeOf(this);
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;   // use embedded nulls to separate filters
        public IntPtr lpstrCustomFilter = IntPtr.Zero;
        public int nMaxCustFilter = 0;
        public int nFilterIndex;
        public IntPtr lpstrFile;
        public int nMaxFile = UnmanagedMethods.MAX_PATH;
        public IntPtr lpstrFileTitle = IntPtr.Zero;
        public int nMaxFileTitle = UnmanagedMethods.MAX_PATH;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset = 0;
        public short nFileExtension = 0;
        public string lpstrDefExt;
        public IntPtr lCustData = IntPtr.Zero;
        public UnmanagedMethods.WndProc lpfnHook;
        public string lpTemplateName = null;
        public IntPtr pvReserved = IntPtr.Zero;
        public int dwReserved = 0;
        public int FlagsEx;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct StartupOutput
    {
        // The following 2 fields won't be used.  They were originally intended 
        // for getting GDI+ to run on our thread - however there are marshalling
        // dealing with function *'s and what not - so we make explicit calls
        // to gdi+ after the fact, via the GdiplusNotificationHook and 
        // GdiplusNotificationUnhook methods.
        public IntPtr hook;//not used
        public IntPtr unhook;//not used.
    }
    public enum TernaryRasterOperations : uint
    {
        SRCCOPY = 0x00CC0020,
        SRCPAINT = 0x00EE0086,
        SRCAND = 0x008800C6,
        SRCINVERT = 0x00660046,
        SRCERASE = 0x00440328,
        NOTSRCCOPY = 0x00330008,
        NOTSRCERASE = 0x001100A6,
        MERGECOPY = 0x00C000CA,
        MERGEPAINT = 0x00BB0226,
        PATCOPY = 0x00F00021,
        PATPAINT = 0x00FB0A09,
        PATINVERT = 0x005A0049,
        DSTINVERT = 0x00550009,
        BLACKNESS = 0x00000042,
        WHITENESS = 0x00FF0062,
        CAPTUREBLT = 0x40000000 //only if WinVer >= 5.0.0 (see wingdi.h)
    }
    public enum DIB_Color_Mode : uint
    {
        DIB_RGB_COLORS = 0,
        DIB_PAL_COLORS = 1
    }
}
