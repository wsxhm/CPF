using CPF.Mac.Foundation;

namespace CPF.Mac.AppKit
{
	public delegate string[] NSControlTextFilter(NSControl control, NSTextView textView, string[] words, NSRange charRange, long index);
}
