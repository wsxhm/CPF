using System;

namespace CPF.Mac.AppKit
{
	public class NSSoundFinishedEventArgs : EventArgs
	{
		public bool Finished
		{
			get;
			set;
		}

		public NSSoundFinishedEventArgs(bool finished)
		{
			Finished = finished;
		}
	}
}
