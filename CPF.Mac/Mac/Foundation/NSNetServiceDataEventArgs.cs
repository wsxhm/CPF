using System;

namespace CPF.Mac.Foundation
{
	public class NSNetServiceDataEventArgs : EventArgs
	{
		public NSData Data
		{
			get;
			set;
		}

		public NSNetServiceDataEventArgs(NSData data)
		{
			Data = data;
		}
	}
}
