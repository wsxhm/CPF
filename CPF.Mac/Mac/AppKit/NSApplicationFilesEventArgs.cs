using System;

namespace CPF.Mac.AppKit
{
	public class NSApplicationFilesEventArgs : EventArgs
	{
		public string[] Filenames
		{
			get;
			set;
		}

		public NSApplicationFilesEventArgs(string[] filenames)
		{
			Filenames = filenames;
		}
	}
}
