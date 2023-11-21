using CPF.Mac.AppKit;
using CPF.Mac.Foundation;
using CPF.Mac.CoreGraphics;
using CPF.Controls;
using CPF;
using CPF.Drawing;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CPF.Mac
{
    //[Register("AppDelegate",true)]
    //[Model]
    public class AppDelegate : NSApplicationDelegate, NotMonoMac
    {
        public AppDelegate()
        {
        }
        //NSWindow window;
        public override void DidFinishLaunching(NSNotification notification)
        {
            //window = new NSWindow(new CGRect(0, 0, 400, 400), (NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Resizable | NSWindowStyle.Titled), NSBackingStore.Retained, false);
            //window.AcceptsMouseMovedEvents = true;
            //window.WindowShouldClose = delegate
            //{
            //    return true;
            //};
            //window.MakeKeyAndOrderFront(this);
        }

        //public override void WillTerminate(NSNotification notification)
        //{
        //    // Insert code here to tear down your application
        //}

        public static List<WindowImpl> windows = new List<WindowImpl>();

        public override void ScreenParametersChanged(NSNotification notification)
        {
            //base.ScreenParametersChanged(notification);
            //System.Diagnostics.Debug.WriteLine(notification.Description);
            foreach (var item in windows)
            {
                item.InvalidateDpi();
            }
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            Console.WriteLine("ApplicationShouldTerminateAfterLastWindowClosed");
            var main = windows.FirstOrDefault(a => a.IsMain);
            if (main != null && main.isMainVisible)
            {
                return false;
            }
            return true;
        }
    }

    interface NotMonoMac
    {

    }

}
