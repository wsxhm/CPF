using System;

namespace CPF.Mac.CoreFoundation
{
	[Flags]
	public enum CFSocketCallBackType
	{
		NoCallBack = 0x0,
		ReadCallBack = 0x1,
		AcceptCallBack = 0x2,
		DataCallBack = 0x3,
		ConnectCallBack = 0x4,
		WriteCallBack = 0x8
	}
}
