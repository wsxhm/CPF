using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureFractions : CTFontFeatureSelectors
	{
		public enum Selector
		{
			NoFractions,
			VerticalFractions,
			DiagonalFractions
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureFractions(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
