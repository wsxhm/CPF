using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.AppKit
{
	public enum NSOpenGLGlobalOption
	{
		FormatCacheSize = 501,
		ClearFormatCache = 502,
		RetainRenderers = 503,
		[Lion]
		UseBuildCache = 506,
		[Obsolete]
		ResetLibrary = 504
	}
}
