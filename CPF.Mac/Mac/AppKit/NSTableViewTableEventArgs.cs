using System;

namespace CPF.Mac.AppKit
{
	public class NSTableViewTableEventArgs : EventArgs
	{
		public NSTableColumn TableColumn
		{
			get;
			set;
		}

		public NSTableViewTableEventArgs(NSTableColumn tableColumn)
		{
			TableColumn = tableColumn;
		}
	}
}
