using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureLinguisticRearrangementConnection : CTFontFeatureSelectors
	{
		public enum Selector
		{
			LinguisticRearrangementOn,
			LinguisticRearrangementOff
		}

		public Selector Feature => Feature;

		public CTFontFeatureLinguisticRearrangementConnection(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
