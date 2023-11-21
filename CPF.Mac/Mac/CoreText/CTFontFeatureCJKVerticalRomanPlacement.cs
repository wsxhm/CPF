using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureCJKVerticalRomanPlacement : CTFontFeatureSelectors
	{
		public enum Selector
		{
			CJKVerticalRomanCentered,
			CJKVerticalRomanHBaseline
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureCJKVerticalRomanPlacement(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
