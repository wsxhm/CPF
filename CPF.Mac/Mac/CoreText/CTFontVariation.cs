using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	public class CTFontVariation
	{
		public NSDictionary Dictionary
		{
			get;
			private set;
		}

		public CTFontVariation()
			: this(new NSMutableDictionary())
		{
		}

		public CTFontVariation(NSDictionary dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			Dictionary = dictionary;
		}
	}
}
