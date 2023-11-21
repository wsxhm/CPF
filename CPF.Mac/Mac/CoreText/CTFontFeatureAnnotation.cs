using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureAnnotation : CTFontFeatureSelectors
	{
		public enum Selector
		{
			NoAnnotation,
			BoxAnnotation,
			RoundedBoxAnnotation,
			CircleAnnotation,
			InvertedCircleAnnotation,
			ParenthesisAnnotation,
			PeriodAnnotation,
			RomanNumeralAnnotation,
			DiamondAnnotation,
			InvertedBoxAnnotation,
			InvertedRoundedBoxAnnotation
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureAnnotation(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
