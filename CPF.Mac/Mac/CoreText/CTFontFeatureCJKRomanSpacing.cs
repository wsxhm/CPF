using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureCJKRomanSpacing : CTFontFeatureSelectors
	{
		public enum Selector
		{
			HalfWidthCJKRoman,
			ProportionalCJKRoman,
			DefaultCJKRoman,
			FullWidthCJKRoman
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureCJKRomanSpacing(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
