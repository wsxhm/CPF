using System;

namespace CPF.Mac.CoreFoundation
{
	public class CFSocketException : Exception
	{
		public CFSocketError Error
		{
			get;
			private set;
		}

		public CFSocketException(CFSocketError error)
		{
			Error = error;
		}
	}
}
