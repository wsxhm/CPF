using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CPF.Input;
using Java.Lang;

namespace CPF.Android
{
    class InputConnection : BaseInputConnection
    {
        public InputConnection(ISurfaceView view) : base(view as View, true)
        {
            this.view = view;
        }
        ISurfaceView view;
        public override bool CommitText(ICharSequence text, int newCursorPosition)
        {
            view.Root.InputManager.KeyboardDevice.ProcessEvent(new TextInputEventArgs(view.Root, view.Root.InputManager.KeyboardDevice, text.ToString()), KeyEventType.TextInput);
            return base.CommitText(text, newCursorPosition);
        }

        //public override bool SetComposingText(ICharSequence text, int newCursorPosition)
        //{
        //    return base.SetComposingText(text, newCursorPosition);
        //}

        //public override bool FinishComposingText()
        //{
        //    return false;
        //}

        //ExtractedText extractedText = new ExtractedText() { Text = CharSequence.ArrayFromStringArray(new string[] { "test" })[0] };
        //public override ExtractedText GetExtractedText(ExtractedTextRequest request, [GeneratedEnum] GetTextFlags flags)
        //{
        //    return extractedText;
        //}

        //public override ICharSequence GetTextAfterCursorFormatted(int length, [GeneratedEnum] GetTextFlags flags)
        //{
        //    return CharSequence.ArrayFromStringArray(new string[] { "s" })[0];
        //}

        //public override ICharSequence GetTextBeforeCursorFormatted(int length, [GeneratedEnum] GetTextFlags flags)
        //{
        //    return CharSequence.ArrayFromStringArray(new string[] { "t" })[0];
        //}

    }
}