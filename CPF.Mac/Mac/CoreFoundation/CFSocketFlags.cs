using System;

namespace CPF.Mac.CoreFoundation
{
	[Flags]
	public enum CFSocketFlags
	{
		AutomaticallyReenableReadCallBack = 0x1,
		AutomaticallyReenableAcceptCallBack = 0x2,
		AutomaticallyReenableDataCallBack = 0x3,
		AutomaticallyReenableWriteCallBack = 0x8,
		LeaveErrors = 0x40,
		CloseOnInvalidate = 0x80
	}
}
