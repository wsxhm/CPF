using CPF.Mac.Foundation;
using System;

namespace CPF.Mac.CoreText
{
	[Obsolete("Deprecated")]
	public class CTFontFeatureLetterCase : CTFontFeatureSelectors
	{
		public enum Selector
		{
			UpperAndLowerCase,
			AllCaps,
			AllLowerCase,
			SmallCaps,
			InitialCaps,
			InitialCapsAndSmallCaps
		}

		public Selector Feature => Feature;

		public CTFontFeatureLetterCase(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
