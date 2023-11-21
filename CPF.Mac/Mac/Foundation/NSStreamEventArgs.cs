using System;

namespace CPF.Mac.Foundation
{
	public class NSStreamEventArgs : EventArgs
	{
		public NSStreamEvent StreamEvent
		{
			get;
			set;
		}

		public NSStreamEventArgs(NSStreamEvent streamEvent)
		{
			StreamEvent = streamEvent;
		}
	}
}
