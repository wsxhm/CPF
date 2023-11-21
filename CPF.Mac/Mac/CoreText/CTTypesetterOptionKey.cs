using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	public static class CTTypesetterOptionKey
	{
		[Obsolete("Deprecated in iOS 6.0")]
		public static readonly NSString DisableBidiProcessing;

		public static readonly NSString ForceEmbeddingLevel;

		static CTTypesetterOptionKey()
		{
			IntPtr intPtr = Dlfcn.dlopen("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText", 0);
			if (!(intPtr == IntPtr.Zero))
			{
				try
				{
					DisableBidiProcessing = Dlfcn.GetStringConstant(intPtr, "kCTTypesetterOptionDisableBidiProcessing");
					ForceEmbeddingLevel = Dlfcn.GetStringConstant(intPtr, "kCTTypesetterOptionForcedEmbeddingLevel");
				}
				finally
				{
					Dlfcn.dlclose(intPtr);
				}
			}
		}
	}
}
