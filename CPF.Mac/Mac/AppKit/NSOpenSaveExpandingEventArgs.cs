using System;

namespace CPF.Mac.AppKit
{
	public class NSOpenSaveExpandingEventArgs : EventArgs
	{
		public bool Expanding
		{
			get;
			set;
		}

		public NSOpenSaveExpandingEventArgs(bool expanding)
		{
			Expanding = expanding;
		}
	}
}
