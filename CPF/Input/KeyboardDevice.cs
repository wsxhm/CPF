using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CPF.Input
{
    public class KeyboardDevice : InputDevice
    {

        public KeyboardDevice(InputManager inputManager) : base(inputManager)
        {
        }

        UIElement focusedElement;

        public UIElement FocusedElement
        {
            get
            {
                return focusedElement;
            }
        }
        public InputModifiers Modifiers { get; set; }
        public void ProcessEvent(InputEventArgs args, KeyEventType type)
        {
            KeyEventArgs e = args as KeyEventArgs;
            switch (type)
            {
                case KeyEventType.KeyDown:
                    var root = args.OriginalSource as UIElement;
                    if (focusedElement != null)
                    {
                        args.OverrideSource(focusedElement);
                        var fe = focusedElement;
                        focusedElement.RaiseDeviceEvent(args, EventType.KeyDown);
                        KeyEvent(e, fe, EventType.KeyDown);
                    }
                    if (!e.Handled && e.Key == Keys.Tab)
                    {
                        //var list = root.Children.OrderByTabIndexList;
                        var r = SetFocusDuiControl(root);
                        if (!r)
                        {
                            SetFocusFirst(root);
                        }
                    }
                    break;
                case KeyEventType.KeyUp:
                    if (focusedElement != null)
                    {
                        args.OverrideSource(focusedElement);
                        var fe = focusedElement;
                        focusedElement.RaiseDeviceEvent(args, EventType.KeyUp);
                        KeyEvent(e, fe, EventType.KeyUp);
                    }
                    break;
                case KeyEventType.TextInput:
                    if (focusedElement != null)
                    {
                        args.OverrideSource(focusedElement);
                        focusedElement.RaiseDeviceEvent(args, EventType.TextInput);
                        KeyEvent(args, focusedElement, EventType.TextInput);
                    }
                    break;
            }
        }

        void KeyEvent(InputEventArgs e, UIElement element, EventType eventName)
        {
            if (!e.Handled && element.Parent != null)
            {
                element.Parent.RaiseDeviceEvent(e, eventName);
                KeyEvent(e, element.Parent, eventName);
            }
        }

        internal void SetFocus(UIElement e)
        {
            focusedElement = e;
            //if (focusedElement != null)
            //{
            //    Console.WriteLine(e.Name);
            //}
        }

        private static bool SetFocusFirst(UIElement root)
        {
            var f = root.Children.OrderByTabIndexList().FirstOrDefault(a => a.Visibility == Visibility.Visible && a.IsEnabled && (a.Focusable || a.Children.Count > 0));
            if (f != null)
            {
                if (f.Focusable)
                {
                    f.Focus(NavigationMethod.Tab);
                    return true;
                }
                else
                {
                    foreach (var item in root.Children.OrderByTabIndexList().Where(a => a.Children.Count > 0 && a.IsEnabled && a.Visibility == Visibility.Visible))
                    {
                        if (SetFocusFirst(item))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool SetFocusDuiControl(UIElement dui)
        {
            var list = dui.Children.OrderByTabIndexList();
            var focused = list.Select((a, b) => new { Item = a, Index = b }).FirstOrDefault(a => a.Item.IsKeyboardFocusWithin);
            if (focused == null)
            {
                foreach (var item in list.Where(a => a.Visibility == Visibility.Visible && a.IsEnabled))
                {
                    if (item.Children.Count > 0 && SetFocusDuiControl(item))
                    {
                        return true;
                    }
                    else if (item.Focusable)
                    {
                        item.Focus(NavigationMethod.Tab);
                        return true;
                    }
                }

                //var canFocuse = list.FirstOrDefault(a => a.Visibility == Visibility.Visible && a.IsEnabled);
                //if (canFocuse != null && canFocuse.Children.Count > 0 && SetFocusDuiControl(canFocuse))
                //{
                //    return true;
                //}
                //if (canFocuse != null && canFocuse.Focusable)
                //{
                //    canFocuse.Focus(NavigationMethod.Tab);
                //    return true;
                //}
            }
            else
            {
                //if (focused.Item.Children.Count > 0)
                //{
                //    if (SetFocusDuiControl(focused.Item))
                //    {
                //        return true;
                //    }
                //}
                //if (focused.Item.IsKeyboardFocused)
                //{
                //    return false;
                //}
                for (int i = focused.Index; i < list.Length; i++)
                {
                    var item = list[i];
                    if (item.Visibility == Visibility.Visible && item.IsEnabled)
                    {
                        if (item.Children.Count > 0 && SetFocusDuiControl(item))
                        {
                            return true;
                        }
                        if (i > focused.Index && item.Focusable)
                        {
                            item.Focus(NavigationMethod.Tab);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}
