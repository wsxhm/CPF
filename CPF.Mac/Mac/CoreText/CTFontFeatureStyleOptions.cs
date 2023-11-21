using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureStyleOptions : CTFontFeatureSelectors
	{
		public enum Selector
		{
			NoStyleOptions,
			DisplayText,
			EngravedText,
			IlluminatedCaps,
			TitlingCaps,
			TallCaps
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureStyleOptions(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
