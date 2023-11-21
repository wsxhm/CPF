using CPF.Mac.Foundation;
using System;

namespace CPF.Mac.AppKit
{
	public class NSDictionaryEventArgs : EventArgs
	{
		public NSDictionary UserInfo
		{
			get;
			set;
		}

		public NSDictionaryEventArgs(NSDictionary userInfo)
		{
			UserInfo = userInfo;
		}
	}
}
