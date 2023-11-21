using System.Runtime.InteropServices;

namespace CPF.Mac.AppKit
{
	public class AppKitFramework
	{
		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		public static extern void NSBeep();
	}
}
