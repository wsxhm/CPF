using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CIGlideReflectedTile : CITileFilter
	{
		public CIGlideReflectedTile()
			: base("CIFourfoldTranslatedTile")
		{
		}

		public CIGlideReflectedTile(IntPtr handle)
			: base(handle)
		{
		}
	}
}
