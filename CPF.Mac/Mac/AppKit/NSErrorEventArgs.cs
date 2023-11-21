using CPF.Mac.Foundation;
using System;

namespace CPF.Mac.AppKit
{
	public class NSErrorEventArgs : EventArgs
	{
		public NSError Error
		{
			get;
			set;
		}

		public NSErrorEventArgs(NSError error)
		{
			Error = error;
		}
	}
}
