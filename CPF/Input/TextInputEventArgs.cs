using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Input
{
    public class TextInputEventArgs : InputEventArgs
    {
        public TextInputEventArgs(UIElement source, InputDevice input, string text) : base(input)
        {
            Text = text;
            OriginalSource = source;
        }
        public string Text { get; }
    }
}
