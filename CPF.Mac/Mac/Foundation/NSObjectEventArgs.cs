using System;

namespace CPF.Mac.Foundation
{
	public class NSObjectEventArgs : EventArgs
	{
		public NSObject Obj
		{
			get;
			set;
		}

		public NSObjectEventArgs(NSObject obj)
		{
			Obj = obj;
		}
	}
}
