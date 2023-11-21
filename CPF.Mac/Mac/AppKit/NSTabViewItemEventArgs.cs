using System;

namespace CPF.Mac.AppKit
{
	public class NSTabViewItemEventArgs : EventArgs
	{
		public NSTabViewItem Item
		{
			get;
			set;
		}

		public NSTabViewItemEventArgs(NSTabViewItem item)
		{
			Item = item;
		}
	}
}
