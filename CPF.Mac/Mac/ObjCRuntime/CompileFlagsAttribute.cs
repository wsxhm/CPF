using System;

namespace CPF.Mac.ObjCRuntime
{
	public class CompileFlagsAttribute : Attribute
	{
		public string Flags;

		public CompileFlagsAttribute(string flags)
		{
			Flags = flags;
		}
	}
}
