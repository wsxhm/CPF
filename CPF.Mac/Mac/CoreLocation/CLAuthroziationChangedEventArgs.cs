using System;

namespace CPF.Mac.CoreLocation
{
	[Obsolete("Use CLAuthorizationChangedEventArgs")]
	public class CLAuthroziationChangedEventArgs : CLAuthorizationChangedEventArgs
	{
		public CLAuthroziationChangedEventArgs(CLAuthorizationStatus status)
			: base(status)
		{
		}
	}
}
