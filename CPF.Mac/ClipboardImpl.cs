using System;
using CPF.Platform;
using CPF.Input;
using CPF.Mac.Foundation;
using CPF.Mac.AppKit;
using CPF.Mac.CoreGraphics;
using CPF.Mac.ObjCRuntime;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using CPF.Drawing;
using System.Collections.Generic;

namespace CPF.Mac
{
    class ClipboardImpl : DataObject, IClipboard
    {
        public ClipboardImpl() : base(NSPasteboard.GeneralPasteboard)
        {
        }

        public void Clear()
        {
            NSPasteboard.GeneralPasteboard.ClearContents();
        }

        public void SetData(params (DataFormat, object)[] data)
        {
            Clear();
            if (data != null)
            {
                foreach (var item in data)
                {
                    switch (item.Item1)
                    {
                        case DataFormat.FileNames:
                            //NSPasteboard.GeneralPasteboard.SetDataForType(,NSPasteboard.NSFilenamesType);
                            var fs = (IEnumerable<string>)item.Item2;
                            var ns = new NSMutableArray();
                            foreach (var s in fs)
                            {
                                ns.Add(new NSString(s));
                            }
                            NSPasteboard.GeneralPasteboard.SetPropertyListForType(ns, NSPasteboard.NSFilenamesType);
                            break;
                        case DataFormat.Html:
                            var l = Encoding.UTF8.GetByteCount(item.Item2.ToString());
                            var h = Marshal.AllocHGlobal(l);
                            Marshal.Copy(Encoding.UTF8.GetBytes(item.Item2.ToString()), 0, h, l);
                            using (var nsdata = NSData.FromBytes(h, (ulong)l))
                            {
                                NSPasteboard.GeneralPasteboard.SetDataForType(nsdata, NSPasteboard.NSHtmlType);
                                Marshal.FreeHGlobal(h);
                            }
                            break;
                        case DataFormat.Image:
                            var img = item.Item2 as Image;
                            var stream = img.SaveToStream(ImageFormat.Png);
                            stream.Position = 0;
                            var im = Marshal.AllocHGlobal((int)stream.Length);
                            var d = new byte[stream.Length];
                            stream.Read(d, 0, (int)stream.Length);
                            Marshal.Copy(d, 0, im, (int)stream.Length);
                            using (var nsdata = NSData.FromBytes(im, (ulong)stream.Length))
                            {
                                NSPasteboard.GeneralPasteboard.SetDataForType(nsdata, NSPasteboard.NSPictType);
                                Marshal.FreeHGlobal(im);
                            }
                            break;
                        case DataFormat.Text:
                            //var l1 = Encoding.UTF8.GetByteCount(item.Item2.ToString());
                            //var h1 = Marshal.AllocHGlobal(l1);
                            //Marshal.Copy(Encoding.UTF8.GetBytes(item.Item2.ToString()), 0, h1, l1);
                            //using (var nsdata = NSData.FromBytes(h1, (ulong)l1))
                            //{
                            //    NSPasteboard.GeneralPasteboard.SetDataForType(nsdata, NSPasteboard.NSStringType);
                            //    Marshal.FreeHGlobal(h1);
                            //}
                            NSPasteboard.GeneralPasteboard.SetStringForType(item.Item2.ToString(), NSPasteboard.NSStringType);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
