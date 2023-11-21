using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CISixfoldRotatedTile : CITileFilter
	{
		public CISixfoldRotatedTile()
			: base("CISixfoldRotatedTile")
		{
		}

		public CISixfoldRotatedTile(IntPtr handle)
			: base(handle)
		{
		}
	}
}
