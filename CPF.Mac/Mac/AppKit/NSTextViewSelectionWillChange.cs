using CPF.Mac.Foundation;

namespace CPF.Mac.AppKit
{
	public delegate NSValue[] NSTextViewSelectionWillChange(NSTextView textView, NSValue[] oldSelectedCharRanges, NSValue[] newSelectedCharRanges);
}
