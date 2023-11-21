using CPF.Mac.Foundation;

namespace CPF.Mac.AppKit
{
	public delegate bool NSTextViewSelectionShouldChange(NSTextView textView, NSValue[] affectedRanges, string[] replacementStrings);
}
