using CPF.Mac.Foundation;
using System;

namespace CPF.Mac.AppKit
{
	public class NSDataEventArgs : EventArgs
	{
		public NSData DeviceToken
		{
			get;
			set;
		}

		public NSDataEventArgs(NSData deviceToken)
		{
			DeviceToken = deviceToken;
		}
	}
}
