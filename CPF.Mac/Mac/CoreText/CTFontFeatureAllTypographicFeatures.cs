using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureAllTypographicFeatures : CTFontFeatureSelectors
	{
		public enum Selector
		{
			AllTypeFeaturesOn,
			AllTypeFeaturesOff
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureAllTypographicFeatures(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
