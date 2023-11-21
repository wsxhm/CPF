using CPF.Mac.Foundation;
using System;

namespace CPF.Mac.AppKit
{
	public class NSWindowCoderEventArgs : EventArgs
	{
		public NSCoder Coder
		{
			get;
			set;
		}

		public NSWindowCoderEventArgs(NSCoder coder)
		{
			Coder = coder;
		}
	}
}
