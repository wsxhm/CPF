using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CIEightfoldReflectedTile : CITileFilter
	{
		public CIEightfoldReflectedTile()
			: base("CIEightfoldReflectedTile")
		{
		}

		public CIEightfoldReflectedTile(IntPtr handle)
			: base(handle)
		{
		}
	}
}
