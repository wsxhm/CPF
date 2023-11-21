using System;
using System.Collections.Generic;
using System.Text;
using CPF.Input;
using CPF.Mac.Foundation;
using CPF.Mac.AppKit;
using CPF.Mac.CoreGraphics;
using CPF.Mac.ObjCRuntime;
using System.Linq;
using System.Xml;

namespace CPF.Mac
{
    class DataObject : IDataObject
    {
        public DataObject(NSPasteboard pasteboard)
        {
            this.pasteboard = pasteboard;
        }


        public static readonly IntPtr AppKit_Handle = Dlfcn.dlopen("/System/Library/Frameworks/AppKit.framework/AppKit", 0);
        private static NSString _NSPasteboardTypeString;
        public static NSString NSPasteboardTypeString
        {
            get
            {
                if (_NSPasteboardTypeString == null)
                {
                    _NSPasteboardTypeString = Dlfcn.GetStringConstant(AppKit_Handle, "NSPasteboardTypeString");
                }
                return _NSPasteboardTypeString;
            }
        }

        protected NSPasteboard pasteboard;
        public bool Contains(DataFormat dataFormat)
        {
            string type = NSPasteboardTypeString;
            if (dataFormat == DataFormat.FileNames)
            {
                type = NSPasteboard.NSFilenamesType;
            }
            else if (dataFormat == DataFormat.Html)
            {
                type = NSPasteboard.NSHtmlType;
            }
            else if (dataFormat == DataFormat.Image)
            {
                type = NSPasteboard.NSPictType;
            }
            //var r = pasteboard.CanReadItemWithDataConformingToTypes(new string[] { type });
            return pasteboard.Types.Any(a => a == type);
        }

        public object GetData(DataFormat dataFormat)
        {
            switch (dataFormat)
            {
                case DataFormat.FileNames:
                    List<string> fileNames = new List<string>();
                    //var xml = pasteboard.GetDataForType(NSPasteboard.NSFilenamesType);
                    //XmlDocument xmldoc = new XmlDocument();
                    //xmldoc.LoadXml(xml.ToString());
                    //foreach (XmlNode item in xmldoc.ChildNodes)
                    //{
                    //    if (item.Name == "plist" && item.NodeType == XmlNodeType.Element)
                    //    {
                    //        foreach (XmlNode array in item.ChildNodes)
                    //        {
                    //            if (array.Name == "array" && item.NodeType == XmlNodeType.Element)
                    //            {
                    //                foreach (XmlNode str in array)
                    //                {
                    //                    fileNames.Add(str.InnerText);
                    //                }
                    //                break;
                    //            }
                    //        }
                    //        break;
                    //    }
                    //}
                    var ns = (NSMutableArray)pasteboard.GetPropertyListForType(NSPasteboard.NSFilenamesType);
                    for (int i = 0; i < (int)ns.Count; i++)
                    {
                        fileNames.Add(new NSString(ns.ValueAt((ulong)i)).ToString());
                    }
                    return fileNames;
                case DataFormat.Html:
                    var html= pasteboard.GetDataForType(NSPasteboard.NSHtmlType)?.ToString();
                    return html;
                case DataFormat.Image:
                    return pasteboard.GetDataForType(NSPasteboard.NSPictType);
                case DataFormat.Text:
                    return pasteboard.GetDataForType(NSPasteboard.NSStringType)?.ToString();
                default:
                    break;
            }
            return null;
        }
    }
}
