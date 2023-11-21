using System;

namespace CPF.Mac.AppKit
{
	public class NSApplicationRegisterEventArgs : EventArgs
	{
		public string[] ReturnTypes
		{
			get;
			set;
		}

		public NSApplicationRegisterEventArgs(string[] returnTypes)
		{
			ReturnTypes = returnTypes;
		}
	}
}
