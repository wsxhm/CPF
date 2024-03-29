using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureVerticalPosition : CTFontFeatureSelectors
	{
		public enum Selector
		{
			NormalPosition,
			Superiors,
			Inferiors,
			Ordinals,
			ScientificInferiors
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureVerticalPosition(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
