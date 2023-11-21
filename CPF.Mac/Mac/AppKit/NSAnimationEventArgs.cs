using System;

namespace CPF.Mac.AppKit
{
	public class NSAnimationEventArgs : EventArgs
	{
		public double Progress
		{
			get;
			set;
		}

		public NSAnimationEventArgs(double progress)
		{
			Progress = progress;
		}
	}
}
