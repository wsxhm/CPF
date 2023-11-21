using System;

namespace CPF.Mac.AppKit
{
	public class NSPathCellDisplayPanelEventArgs : EventArgs
	{
		public NSOpenPanel OpenPanel
		{
			get;
			set;
		}

		public NSPathCellDisplayPanelEventArgs(NSOpenPanel openPanel)
		{
			OpenPanel = openPanel;
		}
	}
}
