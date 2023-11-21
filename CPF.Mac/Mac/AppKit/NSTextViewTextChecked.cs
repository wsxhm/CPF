using CPF.Mac.Foundation;

namespace CPF.Mac.AppKit
{
	public delegate NSTextCheckingResult[] NSTextViewTextChecked(NSTextView view, NSRange range, NSTextCheckingTypes checkingTypes, NSDictionary options, NSTextCheckingResult[] results, NSOrthography orthography, long wordCount);
}
