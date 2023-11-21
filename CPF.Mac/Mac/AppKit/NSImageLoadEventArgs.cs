using System;

namespace CPF.Mac.AppKit
{
	public class NSImageLoadEventArgs : EventArgs
	{
		public NSImageRep Rep
		{
			get;
			set;
		}

		public NSImageLoadEventArgs(NSImageRep rep)
		{
			Rep = rep;
		}
	}
}
