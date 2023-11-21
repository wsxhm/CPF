using CPF.Mac.Foundation;
using System;

namespace CPF.Mac.AppKit
{
	public class NSOpenSavePanelUrlEventArgs : EventArgs
	{
		public NSUrl NewDirectoryUrl
		{
			get;
			set;
		}

		public NSOpenSavePanelUrlEventArgs(NSUrl newDirectoryUrl)
		{
			NewDirectoryUrl = newDirectoryUrl;
		}
	}
}
