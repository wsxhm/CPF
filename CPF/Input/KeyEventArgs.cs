using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Input
{
    public class KeyEventArgs : InputEventArgs
    {
        //public KeyEventArgs(UIElement source, KeyboardDevice device, Keys key, InputModifiers modifiers) : 
        //{

        //}

        public KeyEventArgs(UIElement source, Keys key, int keycode, InputModifiers modifiers, KeyboardDevice keyboardDevice) : base(keyboardDevice)
        {
            this.Key = key;
            this.Modifiers = modifiers;
            //this.Type = type;
            this.KeyCode = keycode;
            this.OriginalSource = source;
        }
        /// <summary>
        /// 解析后的按键类型
        /// </summary>
        public Keys Key { get; private set; }
        /// <summary>
        /// 平台键值
        /// </summary>
        public int KeyCode { get; private set; }

        public InputModifiers Modifiers { get; private set; }

        //public KeyEventType Type { get; private set; }
    }
    public enum KeyEventType : byte
    {
        KeyDown,
        KeyUp,
        TextInput,
        //LostFocus
    }
    [Flags]
    public enum InputModifiers : byte
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8,
        /// <summary>
        /// MacOS  Command
        /// </summary>
        Command = 8,
        LeftMouseButton = 16,
        RightMouseButton = 32,
        MiddleMouseButton = 64
    }


}
