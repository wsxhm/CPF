using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using CPF;
using CPF.Drawing;
using CPF.Input;
using CPF.Platform;
using CPF.Mac.AppKit;
using CPF.Mac.Foundation;
using CPF.Mac.CoreGraphics;
using System.Linq;
using System.Threading.Tasks;
using CPF.Controls;
using System.Reflection;
using CPF.Mac.ObjCRuntime;

namespace CPF.Mac
{
    public class MacPlatform : RuntimePlatform
    {
        public MacPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                NSApplication.Init();
                var zero = Class.objc_getClass("NSApplication");
                //zero = Class.objc_allocateClassPair(zero, "NSApplication", IntPtr.Zero);
                var property = typeof(CPFNSApplication).GetProperty(nameof(CPFNSApplication.IsHandlingSendEvent));
                Class.RegisterProperty(property, typeof(CPFNSApplication), zero);
                var method = typeof(CPFNSApplication).GetMethod(nameof(CPFNSApplication.SetHandlingSendEvent));
                Class.RegisterMethod(method, typeof(CPFNSApplication), zero);
                Application = NSApplication.SharedApplication;
                //var selector = new Selector("sharedApplication");
                //Messaging.void_objc_msgSend_IntPtr(appClassHandle, selector.Handle,);
                Application.Delegate = new AppDelegate();
                //customNSApplication.Test();
                //Console.WriteLine((Application as CPFNSApplication).Test());
                //var isHandlingSendEvent= Selector.GetHandle("isHandlingSendEvent");
                //Application.SetValueForUndefinedKey(NSObject.FromObject(true), new NSString("isHandlingSendEvent"));
                //Console.WriteLine(Application.GetType());
                //Class.objc_setAssociatedObject(Application.Handle, new NSString("isHandlingSendEvent").Handle, NSObject.FromObject(true).Handle, (long)Class.objc_AssociationPolicy.OBJC_ASSOCIATION_RETAIN);
            }
        }


        public static NSApplication Application;

        public void HideDockIcon()
        {
            Application.ActivationPolicy = NSApplicationActivationPolicy.Prohibited;
        }

        public NSEvent CurrentKeyEvent
        {
            get; set;
        }

        public override PixelPoint MousePosition
        {
            get
            {
                var p = NSEvent.CurrentMouseLocation;
                var size = NSScreen.MainScreen.Frame;
                return new PixelPoint((int)p.X, (int)size.Height - (int)p.Y);
            }
        }

        public override TimeSpan DoubleClickTime
        {
            get
            {
                return TimeSpan.FromSeconds(0.4);
            }
        }

        public override IPopupImpl CreatePopup()
        {
            return new PopImpl();
        }


        public override IWindowImpl CreateWindow()
        {
            return new WindowImpl();
        }

        public override IReadOnlyList<Screen> GetAllScreen()
        {
            return NSScreen.Screens.Select(a => GetScreen(a)).ToArray();
        }

        public static Screen GetScreen(NSScreen screen)
        {
            if (screen == null)
            {
                screen = NSScreen.MainScreen;
            }
            var b = screen.Frame;
            var w = screen.VisibleFrame;
            return new ScreenImpl(screen, new Rect((float)b.X, (float)b.Y, (float)b.Width, (float)b.Height), new Rect((float)w.X, (float)b.Height - (float)w.Height - (float)w.Y, (float)w.Width, (float)w.Height), NSScreen.MainScreen == screen);
        }

        public override IClipboard GetClipboard()
        {
            return new ClipboardImpl();
        }

        static Assembly assembly = typeof(MacPlatform).Assembly;
        static NSCursor resizeeastwest;
        static NSCursor Resizeeastwest
        {
            get
            {
                if (resizeeastwest == null)
                {
                    var stream = assembly.GetManifestResourceStream("CPF.Mac.resizeeastwest.png");
                    resizeeastwest = new NSCursor(NSImage.FromStream(stream), new CGPoint(10, 10));
                }
                return resizeeastwest;
            }
        }

        static NSCursor resizenorthsouth;
        static NSCursor Resizenorthsouth
        {
            get
            {
                if (resizenorthsouth == null)
                {
                    var stream = assembly.GetManifestResourceStream("CPF.Mac.resizenorthsouth.png");
                    resizenorthsouth = new NSCursor(NSImage.FromStream(stream), new CGPoint(10, 10));
                }
                return resizenorthsouth;
            }
        }

        static NSCursor resizenortheastsouthwest;
        static NSCursor Resizenortheastsouthwest
        {
            get
            {
                if (resizenortheastsouthwest == null)
                {
                    var stream = assembly.GetManifestResourceStream("CPF.Mac.resizenortheastsouthwest.png");
                    resizenortheastsouthwest = new NSCursor(NSImage.FromStream(stream), new CGPoint(10, 10));
                }
                return resizenortheastsouthwest;
            }
        }

        static NSCursor resizenorthwestsoutheast;
        static NSCursor Resizenorthwestsoutheast
        {
            get
            {
                if (resizenorthwestsoutheast == null)
                {
                    var stream = assembly.GetManifestResourceStream("CPF.Mac.resizenorthwestsoutheast.png");
                    resizenorthwestsoutheast = new NSCursor(NSImage.FromStream(stream), new CGPoint(10, 10));
                }
                return resizenorthwestsoutheast;
            }
        }

        public override object GetCursor(Cursors cursorType)
        {
            switch (cursorType)
            {
                case Cursors.Arrow:
                    break;
                case Cursors.Ibeam:
                    return NSCursor.IBeamCursor;
                case Cursors.Wait:
                    return NSCursor.OperationNotAllowedCursor;
                case Cursors.Cross:
                    return NSCursor.CrosshairCursor;
                case Cursors.UpArrow:
                    return NSCursor.ResizeUpCursor;
                case Cursors.SizeWestEast:
                    //return NSCursor.ResizeLeftRightCursor;
                    return Resizeeastwest;
                case Cursors.SizeNorthSouth:
                    //return NSCursor.ResizeUpDownCursor;
                    return Resizenorthsouth;
                case Cursors.SizeAll:
                    return NSCursor.ArrowCursor;
                case Cursors.No:
                    return NSCursor.OperationNotAllowedCursor;
                case Cursors.Hand:
                    return NSCursor.PointingHandCursor;
                case Cursors.AppStarting:
                    return NSCursor.OperationNotAllowedCursor;
                case Cursors.Help:
                    break;
                case Cursors.TopSide:
                    return NSCursor.ResizeUpCursor;
                case Cursors.BottomSide:
                    return NSCursor.ResizeDownCursor;
                case Cursors.LeftSide:
                    return NSCursor.ResizeLeftCursor;
                case Cursors.RightSide:
                    return NSCursor.ResizeRightCursor;
                case Cursors.TopLeftCorner:
                    //return NSCursor.CrosshairCursor;
                    return Resizenorthwestsoutheast;
                case Cursors.TopRightCorner:
                    //return NSCursor.CrosshairCursor;
                    return Resizenortheastsouthwest;
                case Cursors.BottomLeftCorner:
                    // return NSCursor.CrosshairCursor;
                    return Resizenortheastsouthwest;
                case Cursors.BottomRightCorner:
                    // return NSCursor.CrosshairCursor;
                    return Resizenorthwestsoutheast;
                case Cursors.DragMove:
                    return NSCursor.DragCopyCursor;
                case Cursors.DragCopy:
                    return NSCursor.DragCopyCursor;
                case Cursors.DragLink:
                    return NSCursor.DragLinkCursor;
            }
            return NSCursor.ArrowCursor;
        }

        public override SynchronizationContext GetSynchronizationContext()
        {
            if (SynchronizationContext.Current == null)
            {
                System.Diagnostics.Debug.WriteLine("...null");
            }
            return SynchronizationContext.Current;
        }

        public override void Run()
        {
            ////Application.ActivateIgnoringOtherApps(flag: true);
            //Application.Run();
            NSApplication app = Application;
            while (true)
            {
                NSEvent nSEvent = app.NextEvent(NSEventMask.AnyEvent, NSDate.DistantFuture, NSRunLoop.NSRunLoopEventTracking, deqFlag: true);
                if (nSEvent != null)
                {
                    //Console.WriteLine(nSEvent.Type);
                    app.SendEvent(nSEvent);
                    nSEvent.Dispose();
                }
                CPF.Threading.DispatcherTimer.SetTimeTick();
                //Thread.Sleep(10);
            }
        }

        public override Task<string[]> ShowFileDialogAsync(FileDialog dialog, IWindowImpl parent)
        {
            Task<string[]> task = null;
            NSRunLoop.Main.InvokeOnMainThread(() =>
            {
                if (dialog is OpenFileDialog)
                {
                    NSOpenPanel openPanel = new NSOpenPanel();
                    if (dialog.Title != null)
                    {
                        openPanel.Title = dialog.Title;
                    }
                    if (!string.IsNullOrWhiteSpace(dialog.Directory))
                    {
                        openPanel.DirectoryUrl = new NSUrl(dialog.Directory);
                    }
                    var of = (OpenFileDialog)dialog;
                    openPanel.CanChooseDirectories = false;
                    openPanel.CanCreateDirectories = true;
                    openPanel.CanChooseFiles = true;
                    openPanel.AllowsMultipleSelection = of.AllowMultiple;
                    if (of.InitialFileName != null)
                    {
                        openPanel.NameFieldStringValue = of.InitialFileName;
                    }
                    if (of.Filters != null && of.Filters.Count > 0)
                    {
                        var list = new List<string>();
                        foreach (var item in of.Filters)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Extensions))
                            {
                                list.AddRange(item.Extensions.Split(',').Select(a => a.Trim()));
                            }
                        }
                        openPanel.AllowedFileTypes = list.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
                    }

                    task = Task.Factory.StartNew(() =>
                    {
                        var invokeMre = new ManualResetEvent(false);
                        List<string> urls = new List<string>();
                        NSRunLoop.Main.InvokeOnMainThread(delegate
                        {
                            openPanel.BeginSheet(dialog.Directory, of.InitialFileName, openPanel.AllowedFileTypes, (parent as WindowImpl), () =>
                            {
                                if (openPanel.Urls != null)
                                {
                                    foreach (var item in openPanel.Urls)
                                    {
                                        urls.Add(item.Path);
                                    }
                                }
                                invokeMre.Set();
                            });
                        });
                        invokeMre.WaitOne();

                        return urls.ToArray();
                    });
                }
                else if (dialog is SaveFileDialog)
                {
                    NSSavePanel openPanel = new NSSavePanel();
                    if (dialog.Title != null)
                    {
                        openPanel.Title = dialog.Title;
                    }
                    if (!string.IsNullOrWhiteSpace(dialog.Directory))
                    {
                        openPanel.DirectoryUrl = new NSUrl(dialog.Directory);
                    }
                    var sf = dialog as SaveFileDialog;
                    if (sf.Filters != null && sf.Filters.Count > 0)
                    {
                        var list = new List<string>();
                        foreach (var item in sf.Filters)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Extensions))
                            {
                                list.AddRange(item.Extensions.Split(',').Select(a => a.Trim()));
                            }
                        }
                        openPanel.AllowedFileTypes = list.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
                    }
                    if (sf.InitialFileName != null)
                    {
                        openPanel.NameFieldStringValue = sf.InitialFileName;
                    }
                    task = Task.Factory.StartNew(() =>
                    {
                        var invokeMre = new ManualResetEvent(false);
                        List<string> urls = new List<string>();
                        NSRunLoop.Main.InvokeOnMainThread(delegate
                        {
                            openPanel.BeginSheet((parent as WindowImpl), (a) =>
                            {
                                if (openPanel.Url != null && a == 1)
                                {
                                    urls.Add(openPanel.Url.Path);
                                }
                                invokeMre.Set();
                            });
                        });
                        invokeMre.WaitOne();

                        return urls.ToArray();
                    });

                }
            });
            return task;
        }

        public override Task<string> ShowFolderDialogAsync(OpenFolderDialog dialog, IWindowImpl parent)
        {
            Task<string> task = null;
            NSRunLoop.Main.InvokeOnMainThread(() =>
            {
                NSOpenPanel openPanel = new NSOpenPanel();
                openPanel.CanChooseDirectories = true;
                openPanel.CanCreateDirectories = true;
                openPanel.CanChooseFiles = false;
                if (dialog.Title != null)
                {
                    openPanel.Title = dialog.Title;
                }
                if (!string.IsNullOrWhiteSpace(dialog.Directory))
                {
                    openPanel.DirectoryUrl = new NSUrl(dialog.Directory);
                }
                task = Task.Factory.StartNew(() =>
                  {
                      var invokeMre = new ManualResetEvent(false);
                      string url = "";
                      NSRunLoop.Main.InvokeOnMainThread(delegate
                      {
                          openPanel.BeginSheet(dialog.Directory, "", new string[] { }, (parent as WindowImpl), () =>
                        {
                            if (openPanel.Url != null)
                            {
                                url = openPanel.Url.Path;
                            }
                            invokeMre.Set();
                        });
                      });
                      invokeMre.WaitOne();

                      return url;
                  });
            });
            return task;
        }

        internal static Dictionary<KeyGesture, PlatformHotkey> keyValuePairs = new Dictionary<KeyGesture, PlatformHotkey>() {
            { new KeyGesture(Keys.C,InputModifiers.Windows),PlatformHotkey.Copy},
            { new KeyGesture(Keys.X,InputModifiers.Windows),PlatformHotkey.Cut},
            { new KeyGesture(Keys.V,InputModifiers.Windows),PlatformHotkey.Paste},
            { new KeyGesture(Keys.Y,InputModifiers.Windows),PlatformHotkey.Redo},
            { new KeyGesture(Keys.A,InputModifiers.Windows),PlatformHotkey.SelectAll},
            { new KeyGesture(Keys.Z,InputModifiers.Windows),PlatformHotkey.Undo},
        };
        public override PlatformHotkey Hotkey(KeyGesture keyGesture)
        {
            keyValuePairs.TryGetValue(keyGesture, out PlatformHotkey platformHotkey);
            return platformHotkey;
        }

        public override DragDropEffects DoDragDrop(DragDropEffects allowedEffects, params (DataFormat, object)[] data)
        {
            var w = AppDelegate.windows.FirstOrDefault(a => a.LeftMouseDown != null);
            if (w == null)
            {
                throw new Exception("必须是鼠标左键按下事件里调用！");
            }
            var paste = NSPasteboard.FromName(NSPasteboard.NSDragPasteboardName);
            paste.ClearContents();
            foreach (var item in data)
            {
                if (item.Item1 == DataFormat.Text || item.Item1 == DataFormat.Html)
                {
                    paste.SetStringForType(item.Item2.ToString(), DataObject.NSPasteboardTypeString);
                }
                else if (item.Item1 == DataFormat.FileNames)
                {
                    var array = new NSMutableArray();
                    foreach (var fs in (IEnumerable<string>)item.Item2)
                    {
                        array.Add(new NSString(fs));
                    }
                    paste.SetPropertyListForType(array, NSPasteboard.NSFilenamesType);
                }
                else if (item.Item1 == DataFormat.Image)
                {
                    var img = item.Item2 as Image;
                    var stream = img.SaveToStream(ImageFormat.Png);
                    stream.Position = 0;
                    var im = Marshal.AllocHGlobal((int)stream.Length);
                    var d = new byte[stream.Length];
                    stream.Read(d, 0, (int)stream.Length);
                    Marshal.Copy(d, 0, im, (int)stream.Length);
                    paste.SetDataForType(NSData.FromBytes(im, (ulong)stream.Length), NSPasteboard.NSPictType);
                }
            }
            // return w.view.DragPromisedFilesOfTypes(types.ToArray(), new CGRect(0, 0, 32, 32),obj, true, w.LeftMouseDown) ? DragDropEffects.Copy : DragDropEffects.None;

            //var r = paste.WriteObjects(types.ToArray());
            w.view.DragImage(new NSImage(paste), new CGPoint(), new CGSize(32, 32), w.LeftMouseDown, paste, w.view, true);
            return allowedEffects;
        }

        public override INativeImpl CreateNative()
        {
            return new NativeHost();
        }

        public override INotifyIconImpl CreateNotifyIcon()
        {
            return new NotifyIconImpl();
        }

        public override void Run(CancellationToken cancellationToken)
        {
            //Application.ActivateIgnoringOtherApps(flag: true);
            NSApplication app = Application;
            //cancellationToken.Register(delegate
            //{
            //    app.PostEvent(NSEvent.OtherEvent(NSEventType.ApplicationDefined, default(CGPoint), (NSEventModifierMask)0uL, 0.0, 0L, null, 0, 0L, 0L), atStart: true);
            //});
            while (!cancellationToken.IsCancellationRequested)
            {
                NSEvent nSEvent = app.NextEvent(NSEventMask.AnyEvent, NSDate.DistantFuture, NSRunLoop.NSDefaultRunLoopMode, deqFlag: true);
                if (nSEvent != null)
                {
                    //Console.WriteLine(nSEvent.Type);
                    app.SendEvent(nSEvent);
                    nSEvent.Dispose();
                }
                CPF.Threading.DispatcherTimer.SetTimeTick();
                //Thread.Sleep(10);
            }
        }
    }


}
