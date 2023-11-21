using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF.Input;
using CPF.Controls;
using static CPF.Linux.XLib;

namespace CPF.Linux
{
    public unsafe class XI2Manager
    {
        private static readonly XiEventType[] DefaultEventTypes = new XiEventType[]
        {
            XiEventType.XI_Motion,
            XiEventType.XI_ButtonPress,
            XiEventType.XI_ButtonRelease
        };

        private static readonly XiEventType[] MultiTouchEventTypes = new XiEventType[]
        {
            XiEventType.XI_TouchBegin,
            XiEventType.XI_TouchUpdate,
            XiEventType.XI_TouchEnd
        };

        private X11Info _x11;
        private bool _multitouch;
        //private Dictionary<IntPtr, IXI2Client> _clients = new Dictionary<IntPtr, IXI2Client>();

        class DeviceInfo
        {
            public int Id { get; }
            public XIValuatorClassInfo[] Valuators { get; private set; }
            public XIScrollClassInfo[] Scrollers { get; private set; }
            public DeviceInfo(XIDeviceInfo info)
            {
                Id = info.Deviceid;
                Update(info.Classes, info.NumClasses);
            }

            public virtual void Update(XIAnyClassInfo** classes, int num)
            {
                var valuators = new List<XIValuatorClassInfo>();
                var scrollers = new List<XIScrollClassInfo>();
                for (var c = 0; c < num; c++)
                {
                    if (classes[c]->Type == XiDeviceClass.XIValuatorClass)
                        valuators.Add(*((XIValuatorClassInfo**)classes)[c]);
                    if (classes[c]->Type == XiDeviceClass.XIScrollClass)
                        scrollers.Add(*((XIScrollClassInfo**)classes)[c]);
                }

                Valuators = valuators.ToArray();
                Scrollers = scrollers.ToArray();
            }

            public void UpdateValuators(Dictionary<int, double> valuators)
            {
                foreach (var v in valuators)
                {
                    if (Valuators.Length > v.Key)
                        Valuators[v.Key].Value = v.Value;
                }
            }
        }

        class PointerDeviceInfo : DeviceInfo
        {
            public PointerDeviceInfo(XIDeviceInfo info) : base(info)
            {
            }

            public bool HasScroll(ParsedDeviceEvent ev)
            {
                foreach (var val in ev.Valuators)
                    if (Scrollers.Any(s => s.Number == val.Key))
                        return true;

                return false;
            }

            public bool HasMotion(ParsedDeviceEvent ev)
            {
                foreach (var val in ev.Valuators)
                    if (Scrollers.All(s => s.Number != val.Key))
                        return true;

                return false;
            }

        }

        private PointerDeviceInfo _pointerDevice;
        private LinuxPlatform _platform;

        public bool Init(LinuxPlatform platform)
        {
            _platform = platform;
            _x11 = platform.Info;
            _multitouch = true;
            var devices = (XIDeviceInfo*)XIQueryDevice(_x11.Display,
                (int)XiPredefinedDeviceId.XIAllMasterDevices, out int num);
            for (var c = 0; c < num; c++)
            {
                if (devices[c].Use == XiDeviceType.XIMasterPointer)
                {
                    _pointerDevice = new PointerDeviceInfo(devices[c]);
                    break;
                }
            }
            if (_pointerDevice == null)
                return false;
            /*
            int mask = 0;
            
            XISetMask(ref mask, XiEventType.XI_DeviceChanged);
            var emask = new XIEventMask
            {
                Mask = &mask,
                Deviceid = _pointerDevice.Id,
                MaskLen = XiEventMaskLen
            };
            
            if (XISelectEvents(_x11.Display, _x11.RootWindow, &emask, 1) != Status.Success)
                return false;
            return true;
            */
            return XiSelectEvents(_x11.Display, _x11.RootWindow, new Dictionary<int, List<XiEventType>>
            {
                [_pointerDevice.Id] = new List<XiEventType>
                {
                    XiEventType.XI_DeviceChanged
                }
            }) == Status.Success;
        }

        public void AddWindow(IntPtr handle)
        {
            //_clients[xid] = window;
            //System.Threading.Thread.Sleep(5000);

            var eventsLength = DefaultEventTypes.Length;

            if (_multitouch)
                eventsLength += MultiTouchEventTypes.Length;

            var events = new List<XiEventType>(eventsLength);

            events.AddRange(DefaultEventTypes);

            if (_multitouch)
                events.AddRange(MultiTouchEventTypes);

            var s = XiSelectEvents(_x11.Display, handle, new Dictionary<int, List<XiEventType>> { [_pointerDevice.Id] = events });
            //Console.WriteLine(s);

            //// We are taking over mouse input handling from here
            //return XEventMask.PointerMotionMask
            //       | XEventMask.ButtonMotionMask
            //       | XEventMask.Button1MotionMask
            //       | XEventMask.Button2MotionMask
            //       | XEventMask.Button3MotionMask
            //       | XEventMask.Button4MotionMask
            //       | XEventMask.Button5MotionMask
            //       | XEventMask.ButtonPressMask
            //       | XEventMask.ButtonReleaseMask;
        }

        //public void OnWindowDestroyed(IntPtr xid) => _clients.Remove(xid);

        public void OnEvent(XIEvent* xev, X11Window client)
        {
            if (xev->evtype == XiEventType.XI_DeviceChanged)
            {
                var changed = (XIDeviceChangedEvent*)xev;
                _pointerDevice.Update(changed->Classes, changed->NumClasses);
            }


            if ((xev->evtype >= XiEventType.XI_ButtonPress && xev->evtype <= XiEventType.XI_Motion)
                || (xev->evtype >= XiEventType.XI_TouchBegin && xev->evtype <= XiEventType.XI_TouchEnd))
            {
                var dev = (XIDeviceEvent*)xev;
                //if (_clients.TryGetValue(dev->EventWindow, out var client))
                OnDeviceEvent(client, new ParsedDeviceEvent(dev));
            }
        }

        void OnDeviceEvent(X11Window client, ParsedDeviceEvent ev)
        {
            //Console.WriteLine(ev.Type + " Emulated:" + ev.Emulated);
            if (ev.Type == XiEventType.XI_TouchBegin
                || ev.Type == XiEventType.XI_TouchUpdate
                || ev.Type == XiEventType.XI_TouchEnd)
            {
                var type = ev.Type == XiEventType.XI_TouchBegin ?
                    EventType.TouchDown :
                    (ev.Type == XiEventType.XI_TouchUpdate ?
                        EventType.TouchMove :
                        EventType.TouchUp);
                client.root.InputManager.TouchDevice.ProcessEvent(new TouchEventArgs(new TouchPoint { Id = ev.Detail, Position = ev.Position / client.RenderScaling }, client.root.InputManager.TouchDevice, client.root), client.root.LayoutManager.VisibleUIElements, type);
                //client.ScheduleInput(new RawTouchEventArgs(client.TouchDevice, ev.Timestamp, client.InputRoot, type, ev.Position, ev.Modifiers, ev.Detail));
                return;
            }

            //if (_multitouch && ev.Emulated)
            //    return;

            if (ev.Type == XiEventType.XI_Motion)
            {
                Vector scrollDelta = default;
                foreach (var v in ev.Valuators)
                {
                    foreach (var scroller in _pointerDevice.Scrollers)
                    {
                        if (scroller.Number == v.Key)
                        {
                            var old = _pointerDevice.Valuators[scroller.Number].Value;
                            // Value was zero after reset, ignore the event and use it as a reference next time
                            if (old == 0)
                                continue;
                            var diff = (old - v.Value) / scroller.Increment;
                            if (scroller.ScrollType == XiScrollType.Horizontal)
                                scrollDelta = scrollDelta.WithX(scrollDelta.X + (float)diff);
                            else
                                scrollDelta = scrollDelta.WithY(scrollDelta.Y + (float)diff);

                        }
                    }


                }

                if (scrollDelta != default)
                {    //client.ScheduleInput(new MouseWheelEventArgs(client.MouseDevice, ev.Timestamp, client.InputRoot, ev.Position, scrollDelta, ev.Modifiers));
                    //client.root.InputManager.MouseDevice.ProcessEvent(new MouseWheelEventArgs(client.root, ev.Modifiers.HasFlag(InputModifiers.LeftMouseButton), ev.Modifiers.HasFlag(InputModifiers.RightMouseButton), ev.Modifiers.HasFlag(InputModifiers.MiddleMouseButton), ev.Position / client.LayoutScaling, client.root.InputManager.MouseDevice, scrollDelta), client.root.LayoutManager.VisibleUIElements, EventType.MouseWheel);
                    client.MouseEvent(EventType.MouseWheel, ev.Position, ev.Modifiers, scrollDelta, MouseButton.None);
                }
                if (_pointerDevice.HasMotion(ev))
                {
                    //client.ScheduleInput(new MouseEventArgs(client.MouseDevice, ev.Timestamp, client.InputRoot, RawPointerEventType.Move, ev.Position, ev.Modifiers));

                    //client.root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(client.root, ev.Modifiers.HasFlag(InputModifiers.LeftMouseButton), ev.Modifiers.HasFlag(InputModifiers.RightMouseButton), ev.Modifiers.HasFlag(InputModifiers.MiddleMouseButton), ev.Position / client.LayoutScaling, client.root.InputManager.MouseDevice, ev.Emulated), client.root.LayoutManager.VisibleUIElements, EventType.MouseMove);
                    client.MouseEvent(EventType.MouseMove, ev.Position, ev.Modifiers, new Vector(), MouseButton.None, ev.Emulated);
                }
            }

            if (ev.Type == XiEventType.XI_ButtonPress || ev.Type == XiEventType.XI_ButtonRelease)
            {
                if (!client.ActivateTransientChildIfNeeded())
                {
                    var down = ev.Type == XiEventType.XI_ButtonPress;
                    //var type = ev.Button switch
                    //{
                    //    1 => down ? RawPointerEventType.LeftButtonDown : RawPointerEventType.LeftButtonUp,
                    //    2 => down ? RawPointerEventType.MiddleButtonDown : RawPointerEventType.MiddleButtonUp,
                    //    3 => down ? RawPointerEventType.RightButtonDown : RawPointerEventType.RightButtonUp,
                    //    8 => down ? RawPointerEventType.XButton1Down : RawPointerEventType.XButton1Up,
                    //    9 => down ? RawPointerEventType.XButton2Down : RawPointerEventType.XButton2Up,
                    //    _ => (RawPointerEventType?)null
                    //};
                    //if (type.HasValue)
                    //    client.ScheduleInput(new RawPointerEventArgs(client.MouseDevice, ev.Timestamp, client.InputRoot,
                    //        type.Value, ev.Position, ev.Modifiers));
                    MouseButton? mouseButton = null;
                    switch (ev.Button)
                    {
                        case 1:
                            mouseButton = MouseButton.Left;
                            break;
                        case 2:
                            mouseButton = MouseButton.Middle;
                            break;
                        case 3:
                            mouseButton = MouseButton.Right;
                            break;
                        case 8:
                            mouseButton = MouseButton.XButton1;
                            break;
                        case 9:
                            mouseButton = MouseButton.XButton2;
                            break;
                    }
                    if (mouseButton != null)
                    {
                        //client.root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(client.root, ev.Modifiers.HasFlag(InputModifiers.LeftMouseButton), ev.Modifiers.HasFlag(InputModifiers.RightMouseButton), ev.Modifiers.HasFlag(InputModifiers.MiddleMouseButton), ev.Position / client.LayoutScaling, client.root.InputManager.MouseDevice, mouseButton.Value, ev.Emulated), client.root.LayoutManager.VisibleUIElements, down ? EventType.MouseDown : EventType.MouseUp);
                        client.MouseEvent(down ? EventType.MouseDown : EventType.MouseUp, ev.Position, ev.Modifiers, new Vector(), mouseButton.Value, ev.Emulated);
                    }
                }
            }

            _pointerDevice.UpdateValuators(ev.Valuators);
        }
    }

    unsafe class ParsedDeviceEvent
    {
        public XiEventType Type { get; }
        public InputModifiers Modifiers { get; }
        public ulong Timestamp { get; }
        public Point Position { get; }
        public int Button { get; set; }
        public int Detail { get; set; }
        public bool Emulated { get; set; }
        public Dictionary<int, double> Valuators { get; }
        public ParsedDeviceEvent(XIDeviceEvent* ev)
        {
            Type = ev->evtype;
            Timestamp = (ulong)ev->time.ToInt64();
            var state = (XModifierMask)ev->mods.Effective;
            if (state.HasFlag(XModifierMask.ShiftMask))
                Modifiers |= InputModifiers.Shift;
            if (state.HasFlag(XModifierMask.ControlMask))
                Modifiers |= InputModifiers.Control;
            if (state.HasFlag(XModifierMask.Mod1Mask))
                Modifiers |= InputModifiers.Alt;
            //if (state.HasFlag(XModifierMask.Mod4Mask))
            //    Modifiers |= InputModifiers.Meta;

            if (ev->buttons.MaskLen > 0)
            {
                var buttons = ev->buttons.Mask;
                if (XIMaskIsSet(buttons, 1))
                    Modifiers |= InputModifiers.LeftMouseButton;
                if (XIMaskIsSet(buttons, 2))
                    Modifiers |= InputModifiers.MiddleMouseButton;
                if (XIMaskIsSet(buttons, 3))
                    Modifiers |= InputModifiers.RightMouseButton;
                //if (XIMaskIsSet(buttons, 8))
                //    Modifiers |= InputModifiers.XButton1MouseButton;
                //if (XIMaskIsSet(buttons, 9))
                //    Modifiers |= InputModifiers.XButton2MouseButton;
            }

            Valuators = new Dictionary<int, double>();
            Position = new Point((float)ev->event_x, (float)ev->event_y);
            var values = ev->valuators.Values;
            for (var c = 0; c < ev->valuators.MaskLen * 8; c++)
                if (XIMaskIsSet(ev->valuators.Mask, c))
                    Valuators[c] = *values++;
            if (Type == XiEventType.XI_ButtonPress || Type == XiEventType.XI_ButtonRelease)
                Button = ev->detail;
            Detail = ev->detail;
            Emulated = ev->flags.HasFlag(XiDeviceEventFlags.XIPointerEmulated);
        }
    }

    //interface IXI2Client
    //{
    //    View InputRoot { get; }
    //    void ScheduleInput(InputEventArgs args);
    //    MouseDevice MouseDevice { get; }
    //    TouchDevice TouchDevice { get; }
    //}
}
