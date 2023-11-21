using System;
using System.Collections.Generic;
using System.Text;
using CPF.Input;
using System.Threading.Tasks;
using System.Linq;
using static CPF.Linux.XLib;
using System.Runtime.InteropServices;

namespace CPF.Linux
{
    class X11Clipboard : IClipboard
    {
        private readonly X11Info _x11;
        private string _storedString;
        private string _storedHtml;
        private XWindow window;
        private CancelHandle _requestedFormatsHandle;
        private CancelHandle _requestedTextHandle;
        private readonly IntPtr[] _textAtoms;

        public X11Clipboard()
        {
            _x11 = LinuxPlatform.Platform.Info;
            window = new XWindow { EventAction = OnXEvent };
            //_saveTargetsAtom = XInternAtom(_x11.Display, "PENGUIN", false);
            _textAtoms = new[]
            {
                _x11.Atoms.XA_STRING,
                _x11.Atoms.OEMTEXT,
                _x11.Atoms.UTF8_STRING,
                _x11.Atoms.UTF16_STRING
            }.Where(a => a != IntPtr.Zero).ToArray();
        }

        void OnXEvent(ref XEvent ev)
        {
            if (ev.type == XEventName.SelectionRequest)
            {
                var sel = ev.SelectionRequestEvent;
                var resp = new XEvent
                {
                    SelectionEvent =
                    {
                        type = XEventName.SelectionNotify,
                        send_event = true,
                        display = _x11.Display,
                        selection = sel.selection,
                        target = sel.target,
                        requestor = sel.requestor,
                        time = sel.time,
                        property = IntPtr.Zero
                    }
                };
                if (sel.selection == _x11.Atoms.CLIPBOARD)
                {
                    resp.SelectionEvent.property = WriteTargetToProperty(sel.target, sel.requestor, sel.property);
                }

                XSendEvent(_x11.Display, sel.requestor, false, new IntPtr((int)EventMask.NoEventMask), ref resp);
                XFlush(_x11.Display);
            }
        }

        Encoding GetStringEncoding(IntPtr atom)
        {
            return (atom == _x11.Atoms.XA_STRING
                    || atom == _x11.Atoms.OEMTEXT)
                ? Encoding.ASCII
                : atom == _x11.Atoms.UTF8_STRING
                    ? Encoding.UTF8
                    : atom == _x11.Atoms.UTF16_STRING
                        ? Encoding.Unicode
                        : null;
        }

        unsafe IntPtr WriteTargetToProperty(IntPtr target, IntPtr window, IntPtr property)
        {
            Encoding textEnc;
            if (target == _x11.Atoms.TARGETS)
            {
                var atoms = _textAtoms;
                atoms = atoms.Concat(new[] { _x11.Atoms.TARGETS, _x11.Atoms.MULTIPLE })
                    .ToArray();
                if (!string.IsNullOrEmpty(_storedHtml))
                {
                    atoms = atoms.Concat(new[] { DataObject.htmlAtom })
                        .ToArray();
                }
                XChangeProperty(_x11.Display, window, property, _x11.Atoms.XA_ATOM, 32, PropertyMode.Replace, atoms, atoms.Length);
                return property;
            }
            else if (target == _x11.Atoms.SAVE_TARGETS && _x11.Atoms.SAVE_TARGETS != IntPtr.Zero)
            {
                return property;
            }
            else if ((textEnc = GetStringEncoding(target)) != null)
            {

                var data = textEnc.GetBytes(_storedString ?? "");
                //var ptr = Marshal.AllocHGlobal(data.Length);
                //Marshal.Copy(data, 0, ptr, data.Length);
                //XChangeProperty(_x11.Display, window, property, target, 8,
                //    PropertyMode.Replace,
                //    (void*)ptr, data.Length);
                fixed (void* pdata = data)
                    XChangeProperty(_x11.Display, window, property, target, 8,
                        PropertyMode.Replace,
                        pdata, data.Length);
                return property;
            }
            else if (target == _x11.Atoms.MULTIPLE && _x11.Atoms.MULTIPLE != IntPtr.Zero)
            {
                XGetWindowProperty(_x11.Display, window, property, IntPtr.Zero, new IntPtr(0x7fffffff), false,
                    _x11.Atoms.ATOM_PAIR, out _, out var actualFormat, out var nitems, out _, out var prop);
                if (nitems == IntPtr.Zero)
                    return IntPtr.Zero;
                if (actualFormat == 32)
                {
                    var data = (IntPtr*)prop.ToPointer();
                    for (var c = 0; c < nitems.ToInt32(); c += 2)
                    {
                        var subTarget = data[c];
                        var subProp = data[c + 1];
                        var converted = WriteTargetToProperty(subTarget, window, subProp);
                        data[c + 1] = converted;
                    }

                    XChangeProperty(_x11.Display, window, property, _x11.Atoms.ATOM_PAIR, 32, PropertyMode.Replace,
                        prop.ToPointer(), nitems.ToInt32());
                }

                XFree(prop);

                return property;
            }
            else if (target == DataObject.htmlAtom)
            {
                var data = Encoding.UTF8.GetBytes(_storedHtml ?? "");
                fixed (void* pdata = data)
                    XChangeProperty(_x11.Display, window, property, target, 8, PropertyMode.Replace, pdata, data.Length);
                return property;
            }
            else
                return IntPtr.Zero;
        }

        private unsafe bool OnEvent(ref XEvent ev)
        {
            if (ev.type == XEventName.SelectionNotify && ev.SelectionEvent.selection == _x11.Atoms.CLIPBOARD)
            {
                var sel = ev.SelectionEvent;
                if (sel.property == IntPtr.Zero)
                {
                    _requestedFormatsHandle.SetResult(null);
                    _requestedTextHandle.SetResult(null);
                }
                XGetWindowProperty(_x11.Display, window.Handle, sel.property, IntPtr.Zero, new IntPtr(0x7fffffff), true, (IntPtr)Atom.AnyPropertyType,
                    out var actualAtom, out var actualFormat, out var nitems, out var bytes_after, out var prop);
                Encoding textEnc = null;
                if (nitems == IntPtr.Zero)
                {
                    _requestedFormatsHandle.SetResult(null);
                    _requestedTextHandle.SetResult(null);
                }
                else
                {
                    if (sel.property == _x11.Atoms.TARGETS)
                    {
                        if (actualFormat != 32)
                        {
                            _requestedFormatsHandle.SetResult(null);
                        }
                        else
                        {
                            var formats = new IntPtr[nitems.ToInt32()];
                            Marshal.Copy(prop, formats, 0, formats.Length);
                            _requestedFormatsHandle.SetResult(formats);
                            //_requestedFormatsTcs?.TrySetResult(formats);
                        }
                    }
                    else if ((textEnc = GetStringEncoding(sel.property)) != null)
                    {
                        var text = textEnc.GetString((byte*)prop.ToPointer(), nitems.ToInt32());
                        //_requestedTextTcs?.TrySetResult(text);
                        _requestedTextHandle.SetResult(text);
                    }
                    else if (sel.property == DataObject.htmlAtom)
                    {
                        var html = Encoding.UTF8.GetString((byte*)prop.ToPointer(), nitems.ToInt32());
                        _requestedTextHandle.SetResult(html);
                    }
                }

                XFree(prop);
            }
            return false;
        }

        IntPtr[] SendFormatRequest()
        {
            //if (_requestedFormatsTcs == null || _requestedFormatsTcs.Task.IsCompleted)
            //    _requestedFormatsTcs = new TaskCompletionSource<IntPtr[]>();
            if (_requestedFormatsHandle == null || _requestedFormatsHandle.Cancel)
            {
                _requestedFormatsHandle = new CancelHandle();
            }
            XConvertSelection(_x11.Display, _x11.Atoms.CLIPBOARD, _x11.Atoms.TARGETS, _x11.Atoms.TARGETS, window.Handle,
                IntPtr.Zero);
            //while (!_requestedFormatsTcs.Task.IsCompleted)
            //{
            //    XNextEvent(_x11.Display, out var xev);
            //    OnEvent(xev);
            //    LinuxPlatform.Platform.OnEvent(xev);
            //}
            LinuxPlatform.Platform.RunMainLoop(_requestedFormatsHandle, OnEvent);
            //return _requestedFormatsTcs.Task;
            return (IntPtr[])_requestedFormatsHandle.Data;
        }

        string SendTextRequest(IntPtr format)
        {
            //if (_requestedTextTcs == null || _requestedFormatsTcs.Task.IsCompleted)
            //    _requestedTextTcs = new TaskCompletionSource<string>();
            if (_requestedTextHandle == null || _requestedFormatsHandle.Cancel)
            {
                _requestedTextHandle = new CancelHandle();
            }
            XConvertSelection(_x11.Display, _x11.Atoms.CLIPBOARD, format, format, window.Handle, IntPtr.Zero);
            //while (!_requestedTextTcs.Task.IsCompleted)
            //{
            //    XNextEvent(_x11.Display, out var xev);
            //    OnEvent(xev);
            //    LinuxPlatform.Platform.OnEvent(xev);
            //}
            //return _requestedTextTcs.Task;
            LinuxPlatform.Platform.RunMainLoop(_requestedTextHandle, OnEvent);
            return (string)_requestedTextHandle.Data;
        }

        public void Clear()
        {
            SetData((DataFormat.Text, null));
        }

        public bool Contains(DataFormat dataFormat)
        {
            if (dataFormat == DataFormat.Text)
            {
                return GetData(DataFormat.Text) != null;
            }
            else if (dataFormat == DataFormat.Html)
            {
                return GetData(DataFormat.Html) != null;
            }
            return false;
        }

        string GetText()
        {
            if (XGetSelectionOwner(_x11.Display, _x11.Atoms.CLIPBOARD) == IntPtr.Zero)
                return null;
            var res = SendFormatRequest();
            var target = _x11.Atoms.UTF8_STRING;
            if (res != null)
            {
                var preferredFormats = new[] { _x11.Atoms.UTF16_STRING, _x11.Atoms.UTF8_STRING, _x11.Atoms.XA_STRING };
                foreach (var pf in preferredFormats)
                    if (res.Contains(pf))
                    {
                        target = pf;
                        break;
                    }
            }

            return SendTextRequest(target);
        }
        string GetHtml()
        {
            if (XGetSelectionOwner(_x11.Display, _x11.Atoms.CLIPBOARD) == IntPtr.Zero)
                return null;
            var res = SendFormatRequest();

            return SendTextRequest(DataObject.htmlAtom);
        }

        public object GetData(DataFormat dataFormat)
        {
            if (dataFormat == DataFormat.Text)
            {
                return GetText();
            }
            if (dataFormat == DataFormat.Html)
            {
                return GetHtml();
            }
            throw new NotImplementedException("暂时不支持：" + dataFormat);
        }

        public void SetData(params (DataFormat, object)[] data)
        {
            _storedHtml = null;
            _storedString = null;
            foreach (var item in data)
            {
                if (item.Item1 == DataFormat.Text)
                {
                    _storedString = item.Item2 == null ? null : item.Item2.ToString();
                    //break;
                }
                else if (item.Item1 == DataFormat.Html)
                {
                    _storedHtml = item.Item2?.ToString();
                }
            }
            if (data.Length > 0)
            {
                XSetSelectionOwner(_x11.Display, _x11.Atoms.CLIPBOARD, window.Handle, IntPtr.Zero);
            }
        }
    }
}
