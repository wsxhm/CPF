using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using CPF;
using CPF.Drawing;
using CPF.Input;
using CPF.Platform;
using static CPF.Windows.UnmanagedMethods;
using System.Threading.Tasks;
using CPF.Controls;
using System.Linq;

namespace CPF.Windows
{
    public class WindowsPlatform : RuntimePlatform
    {
        public override PixelPoint MousePosition
        {
            get
            {
                POINT pt = new POINT();
                GetCursorPos(out pt);
                return new PixelPoint(pt.X, pt.Y);
            }
        }

        public override TimeSpan DoubleClickTime
        {
            get
            {
                return TimeSpan.FromSeconds(0.4);
            }
        }

        public override IClipboard GetClipboard()
        {
            return new ClipboardImpl();
        }

        public override IPopupImpl CreatePopup()
        {
            return new PopupImpl();
        }

        //public override IViewImpl CreateView()
        //{
        //    return new WindowImpl();
        //}

        public override IWindowImpl CreateWindow()
        {
            return new WindowImpl();
        }

        public override SynchronizationContext GetSynchronizationContext()
        {
            return new WindowsSynchronizationContext();
        }

        public override void Run()
        {
            MSG msg = new MSG();
            while (Application.Main != null && GetMessage(ref msg, IntPtr.Zero, 0, 0))
            {
                CPF.Threading.DispatcherTimer.SetTimeTick();
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }

        public override void Run(CancellationToken cancellation)
        {
            MSG msg = new MSG();
            while (Application.Main != null && !cancellation.IsCancellationRequested && GetMessage(ref msg, IntPtr.Zero, 0, 0))
            {
                CPF.Threading.DispatcherTimer.SetTimeTick();
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }

        internal static bool registerForMarshalling = true;
        /// <summary>
        /// 初始化Windows平台
        /// </summary>
        /// <param name="RegisterForMarshalling">net5以上才有用的，为了兼容aot，自动注册COM的。 如果你要使用Winform控件出现com问题，你可以把这个改成false</param>
        public WindowsPlatform(bool RegisterForMarshalling)
        {
            registerForMarshalling = RegisterForMarshalling;
#if !Net4
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#endif
            {
                var w = WindowImpl.Window;
                SetDpiAwareness();
            }
        }
        public WindowsPlatform()
        {
#if !Net4
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#endif
            {
                var w = WindowImpl.Window;
                SetDpiAwareness();
            }
        }

        private static void SetDpiAwareness()
        {
            var user32 = LoadLibrary("user32.dll");
            var method = GetProcAddress(user32, nameof(SetProcessDpiAwarenessContext));

            if (method != IntPtr.Zero)
            {
                if (SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2) ||
                    SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE))
                {
                    return;
                }
            }

            var shcore = LoadLibrary("shcore.dll");
            method = GetProcAddress(shcore, nameof(SetProcessDpiAwareness));

            if (method != IntPtr.Zero)
            {
                SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE);
            }
            else
            {
                if (Environment.OSVersion.Version.Major > 5)
                {
                    SetProcessDPIAware();
                }
            }
        }

#if Net4
        public override IList<Screen> GetAllScreen()
#else
        public override IReadOnlyList<Screen> GetAllScreen()
#endif
        {
            //return new ScreenImpl(new CPF.Drawing.Rect(), new CPF.Drawing.Rect(), false, IntPtr.Zero).GetAllScreens();
            var ScreenCount = GetSystemMetrics(SystemMetric.SM_CMONITORS);
            List<Screen> screens = new List<Screen>();
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                (IntPtr monitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr data) =>
                {
                    screens.Add(ScreenImpl.FromMonitor(monitor, hdcMonitor));
                    return true;
                }, IntPtr.Zero);
            return screens;
        }

        public override object GetCursor(Cursors cursorType)
        {
            IntPtr rv;
            if (!Cache.TryGetValue(cursorType, out rv))
            {
                Cache[cursorType] = rv = UnmanagedMethods.LoadCursor(IntPtr.Zero, new IntPtr(CursorTypeMapping[cursorType]));
            }
            return rv;
        }

        private const UnmanagedMethods.FOS DefaultDialogOptions = UnmanagedMethods.FOS.FOS_FORCEFILESYSTEM | UnmanagedMethods.FOS.FOS_NOVALIDATE |
    UnmanagedMethods.FOS.FOS_NOTESTFILECREATE | UnmanagedMethods.FOS.FOS_DONTADDTORECENT;
        const int FILEBUFSIZE = 8192;

        public override Task<string[]> ShowFileDialogAsync(FileDialog dialog, IWindowImpl parent)
        {
            var hWnd = (parent as WindowImpl)?.Handle ?? IntPtr.Zero;
            return Task.Factory.StartNew(() =>
            {
                // var result = Array.Empty<string>();
                var result = new string[0];
                try
                {
                    Guid clsid = dialog is OpenFileDialog ? ShellIds.OpenFileDialog : ShellIds.SaveFileDialog;
                    Guid iid = ShellIds.IFileDialog;

                    //兼容xp
#if Net4
                    CPF.Threading.Dispatcher.MainThread.Invoke(() =>
                    {
                        string filler = "";
                        if (dialog.Filters != null && dialog.Filters.Count > 0)
                        {
                            foreach (var item in dialog.Filters)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Extensions))
                                {
                                    filler += item.Name + "|*." + string.Join(";*.", item.Extensions.Split(',')) + "|";
                                }
                            }
                            filler = filler.TrimEnd('|');
                        }

                        if (dialog is OpenFileDialog open)
                        {
                            using (var openFileDialog = new System.Windows.Forms.OpenFileDialog
                            {
                                Multiselect = open.AllowMultiple,
                                Title = open.Title,
                                FileName = open.InitialFileName,
                                InitialDirectory = open.Directory,
                                Filter = filler
                            })
                            {
                                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    result = openFileDialog.FileNames;
                                }
                            }
                        }
                        else if (dialog is SaveFileDialog save)
                        {
                            using (var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                            {
                                Title = save.Title,
                                FileName = save.InitialFileName,
                                InitialDirectory = save.Directory,
                                Filter = filler
                            })
                            {
                                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    result = new string[] { saveFileDialog.FileName };
                                }
                            }
                        }
                    });
#else
                    CPF.Threading.Dispatcher.MainThread.Invoke(() =>
                    {
                        CoInitializeEx(IntPtr.Zero, COINIT_APARTMENTTHREADED);
                        var r = CoCreateInstance(ref clsid, null, 1, ref iid, out var unk);
                        var frm = (IFileDialog)unk;
                        var openDialog = dialog as OpenFileDialog;

                        uint options;
                        frm.GetOptions(out options);
                        options |= (uint)(DefaultDialogOptions);
                        if (openDialog?.AllowMultiple == true)
                            options |= (uint)UnmanagedMethods.FOS.FOS_ALLOWMULTISELECT;
                        frm.SetOptions(options);

                        var defaultExtension = (dialog as SaveFileDialog)?.DefaultExtension ?? "";
                        frm.SetDefaultExtension(defaultExtension);
                        frm.SetFileName(dialog.InitialFileName ?? "");
                        if (!string.IsNullOrEmpty(dialog.Title))
                        {
                            frm.SetTitle(dialog.Title);
                        }

                        var filters = new List<COMDLG_FILTERSPEC>();
                        if (dialog.Filters != null)
                        {
                            foreach (var filter in dialog.Filters)
                            {
                                if (!string.IsNullOrWhiteSpace(filter.Extensions))
                                {
                                    var extMask = string.Join(";", filter.Extensions.Split(',').Where(a => !string.IsNullOrWhiteSpace(a)).Select(e => "*." + e.Trim()));
                                    filters.Add(new COMDLG_FILTERSPEC { pszName = filter.Name, pszSpec = extMask });
                                }
                            }
                        }
                        if (filters.Count == 0)
                            filters.Add(new COMDLG_FILTERSPEC { pszName = "All files", pszSpec = "*.*" });

                        frm.SetFileTypes((uint)filters.Count, filters.ToArray());
                        frm.SetFileTypeIndex(0);

                        if (dialog.Directory != null)
                        {
                            IShellItem directoryShellItem;
                            Guid riid = UnmanagedMethods.ShellIds.IShellItem;
                            if (UnmanagedMethods.SHCreateItemFromParsingName(dialog.Directory, IntPtr.Zero, ref riid, out directoryShellItem) == (uint)HRESULT.S_OK)
                            {
                                frm.SetFolder(directoryShellItem);
                                frm.SetDefaultFolder(directoryShellItem);
                            }
                        }

                        if (frm.Show(hWnd) == (uint)HRESULT.S_OK)
                        {
                            if (openDialog?.AllowMultiple == true)
                            {
                                IShellItemArray shellItemArray;
                                ((IFileOpenDialog)frm).GetResults(out shellItemArray);
                                uint count;
                                shellItemArray.GetCount(out count);
                                result = new string[count];
                                for (uint i = 0; i < count; i++)
                                {
                                    IShellItem shellItem;
                                    shellItemArray.GetItemAt(i, out shellItem);
                                    result[i] = GetAbsoluteFilePath(shellItem);
                                }
                            }
                            else
                            {
                                IShellItem shellItem;
                                if (frm.GetResult(out shellItem) == (uint)HRESULT.S_OK)
                                {
                                    result = new string[] { GetAbsoluteFilePath(shellItem) };
                                }
                            }
                        }
                    });
#endif

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                return result;
            });
        }

        public override Task<string> ShowFolderDialogAsync(OpenFolderDialog dialog, IWindowImpl parent)
        {
            return Task.Factory.StartNew(() =>
            {
                string result = string.Empty;

                var hWnd = (parent as WindowImpl)?.Handle ?? IntPtr.Zero;
                Guid clsid = ShellIds.OpenFileDialog;
                Guid iid = ShellIds.IFileDialog;

#if Net4
                CPF.Threading.Dispatcher.MainThread.Invoke(() =>
                {
                    using (var folderBrowser = new System.Windows.Forms.FolderBrowserDialog
                    {
                        SelectedPath = dialog.Directory,
                        Description = dialog.Title,
                        ShowNewFolderButton = true,
                    })
                    {
                        if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            result = folderBrowser.SelectedPath;
                        }
                    }
                });
#else
                CPF.Threading.Dispatcher.MainThread.Invoke(() =>
                {
                    var r = CoCreateInstance(ref clsid, IntPtr.Zero, 1, ref iid, out var unk);
                    var frm = (IFileDialog)unk;
                    uint options;
                    frm.GetOptions(out options);
                    options |= (uint)(UnmanagedMethods.FOS.FOS_PICKFOLDERS | DefaultDialogOptions);
                    frm.SetOptions(options);
                    if (!string.IsNullOrEmpty(dialog.Title))
                    {
                        frm.SetTitle(dialog.Title);
                    }

                    if (dialog.Directory != null)
                    {
                        IShellItem directoryShellItem;
                        Guid riid = UnmanagedMethods.ShellIds.IShellItem;
                        if (UnmanagedMethods.SHCreateItemFromParsingName(dialog.Directory, IntPtr.Zero, ref riid, out directoryShellItem) == (uint)HRESULT.S_OK)
                        {
                            frm.SetFolder(directoryShellItem);
                        }
                    }

                    if (dialog.Directory != null)
                    {
                        IShellItem directoryShellItem;
                        Guid riid = UnmanagedMethods.ShellIds.IShellItem;
                        if (UnmanagedMethods.SHCreateItemFromParsingName(dialog.Directory, IntPtr.Zero, ref riid, out directoryShellItem) == (uint)HRESULT.S_OK)
                        {
                            frm.SetDefaultFolder(directoryShellItem);
                        }
                    }

                    if (frm.Show(hWnd) == (uint)HRESULT.S_OK)
                    {
                        IShellItem shellItem;
                        if (frm.GetResult(out shellItem) == (uint)HRESULT.S_OK)
                        {
                            result = GetAbsoluteFilePath(shellItem);
                        }
                    }
                });
#endif
                return result;
            });
        }
        private string GetAbsoluteFilePath(IShellItem shellItem)
        {
            IntPtr pszString;
            if (shellItem.GetDisplayName(UnmanagedMethods.SIGDN_FILESYSPATH, out pszString) == (uint)HRESULT.S_OK)
            {
                if (pszString != IntPtr.Zero)
                {
                    try
                    {
                        return Marshal.PtrToStringAuto(pszString);
                    }
                    finally
                    {
                        Marshal.FreeCoTaskMem(pszString);
                    }
                }
            }
            return "";
        }
        static Dictionary<KeyGesture, PlatformHotkey> keyValuePairs = new Dictionary<KeyGesture, PlatformHotkey>() {
            { new KeyGesture(Keys.C,InputModifiers.Control),PlatformHotkey.Copy},
            { new KeyGesture(Keys.X,InputModifiers.Control),PlatformHotkey.Cut},
            { new KeyGesture(Keys.V,InputModifiers.Control),PlatformHotkey.Paste},
            { new KeyGesture(Keys.Y,InputModifiers.Control),PlatformHotkey.Redo},
            { new KeyGesture(Keys.A,InputModifiers.Control),PlatformHotkey.SelectAll},
            { new KeyGesture(Keys.Z,InputModifiers.Control),PlatformHotkey.Undo},
        };
        public override PlatformHotkey Hotkey(KeyGesture keyGesture)
        {
            keyValuePairs.TryGetValue(keyGesture, out PlatformHotkey platformHotkey);
            return platformHotkey;
        }

        public override DragDropEffects DoDragDrop(DragDropEffects allowedEffects, params (DataFormat, object)[] data)
        {
            Threading.Dispatcher.MainThread.VerifyAccess();
            try
            {
                OleDragSource src = new OleDragSource();
                DataObject dataObject = new DataObject(data);
                int allowed = (int)ConvertDropEffect(allowedEffects);

                int[] finalEffect = new int[1];
                UnmanagedMethods.DoDragDrop(dataObject, src, allowed, finalEffect);

                return ConvertDropEffect((DropEffect)finalEffect[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(e);
            }
            return DragDropEffects.None;
        }

        public static DropEffect ConvertDropEffect(DragDropEffects operation)
        {
            DropEffect result = DropEffect.None;
            if (operation.HasFlag(DragDropEffects.Copy))
                result |= DropEffect.Copy;
            if (operation.HasFlag(DragDropEffects.Move))
                result |= DropEffect.Move;
            if (operation.HasFlag(DragDropEffects.Link))
                result |= DropEffect.Link;
            return result;
        }
        public static DragDropEffects ConvertDropEffect(DropEffect effect)
        {
            DragDropEffects result = DragDropEffects.None;
            if (effect.HasFlag(DropEffect.Copy))
                result |= DragDropEffects.Copy;
            if (effect.HasFlag(DropEffect.Move))
                result |= DragDropEffects.Move;
            if (effect.HasFlag(DropEffect.Link))
                result |= DragDropEffects.Link;
            return result;
        }

        public override INativeImpl CreateNative()
        {
            return new NativeHost();
        }

        public override INotifyIconImpl CreateNotifyIcon()
        {
            return new NotifyIcon();
        }


        private static readonly Dictionary<Cursors, IntPtr> Cache =
    new Dictionary<Cursors, IntPtr>();
        private static readonly Dictionary<Cursors, int> CursorTypeMapping = new Dictionary
    <Cursors, int>
        {
            {Cursors.AppStarting, 32650},
            {Cursors.Arrow, 32512},
            {Cursors.Cross, 32515},
            {Cursors.Hand, 32649},
            {Cursors.Help, 32651},
            {Cursors.Ibeam, 32513},
            {Cursors.No, 32648},
            {Cursors.SizeAll, 32646},
            {Cursors.UpArrow, 32516},
            {Cursors.SizeNorthSouth, 32645},
            {Cursors.SizeWestEast, 32644},
            {Cursors.Wait, 32514},
            //Same as SizeNorthSouth
            {Cursors.TopSide, 32645},
            {Cursors.BottomSide, 32645},
            //Same as SizeWestEast
            {Cursors.LeftSide, 32644},
            {Cursors.RightSide, 32644},
            //Using SizeNorthWestSouthEast
            {Cursors.TopLeftCorner, 32642},
            {Cursors.BottomRightCorner, 32642},
            //Using SizeNorthEastSouthWest
            {Cursors.TopRightCorner, 32643},
            {Cursors.BottomLeftCorner, 32643},

            // Fallback, should have been loaded from ole32.dll
            {Cursors.DragMove, 32516},
            {Cursors.DragCopy, 32516},
            {Cursors.DragLink, 32516},
        };
    }
}
