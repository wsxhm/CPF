namespace CPF.Mac.AppKit
{
	public enum NSApplicationPresentationOptions : ulong
	{
		Default = 0uL,
		AutoHideDock = 1uL,
		HideDock = 2uL,
		AutoHideMenuBar = 4uL,
		HideMenuBar = 8uL,
		DisableAppleMenu = 0x10,
		DisableProcessSwitching = 0x20,
		DisableForceQuit = 0x40,
		DisableSessionTermination = 0x80,
		DisableHideApplication = 0x100,
		DisableMenuBarTransparency = 0x200,
		FullScreen = 0x400,
		AutoHideToolbar = 0x800
	}
}
