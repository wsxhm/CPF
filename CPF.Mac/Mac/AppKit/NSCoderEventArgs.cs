using CPF.Mac.Foundation;
using System;

namespace CPF.Mac.AppKit
{
	public class NSCoderEventArgs : EventArgs
	{
		public NSCoder Encoder
		{
			get;
			set;
		}

		public NSCoderEventArgs(NSCoder encoder)
		{
			Encoder = encoder;
		}
	}
}
