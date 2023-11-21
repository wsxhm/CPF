using CPF.Mac.Foundation;

namespace CPF.Mac.AppKit
{
	public delegate string[] NSTextViewCompletion(NSTextView textView, string[] words, NSRange charRange, long index);
}
