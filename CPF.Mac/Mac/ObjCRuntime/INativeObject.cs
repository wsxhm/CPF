using System;

namespace CPF.Mac.ObjCRuntime
{
	public interface INativeObject
	{
		IntPtr Handle
		{
			get;
		}
	}
}
