using CPF.Mac.Foundation;
using System;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureItalicCJKRoman : CTFontFeatureSelectors
	{
		public enum Selector
		{
			[Obsolete("Deprecated. Use CJKItalicRomanOff instead")]
			NoCJKItalicRoman,
			[Obsolete("Deprecated. Use CJKItalicRomanOn instead")]
			CJKItalicRoman,
			CJKItalicRomanOn,
			CJKItalicRomanOff
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureItalicCJKRoman(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
