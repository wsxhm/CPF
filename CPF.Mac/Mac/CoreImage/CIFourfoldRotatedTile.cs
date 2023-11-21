using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CIFourfoldRotatedTile : CITileFilter
	{
		public CIFourfoldRotatedTile()
			: base("CIFourfoldRotatedTile")
		{
		}

		public CIFourfoldRotatedTile(IntPtr handle)
			: base(handle)
		{
		}
	}
}
