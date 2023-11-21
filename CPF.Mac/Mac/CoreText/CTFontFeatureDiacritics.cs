using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureDiacritics : CTFontFeatureSelectors
	{
		public enum Selector
		{
			ShowDiacritics,
			HideDiacritics,
			DecomposeDiacritics
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureDiacritics(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
