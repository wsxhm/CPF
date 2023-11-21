using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSPrintPanelOptions : ulong
	{
		ShowsCopies = 0x1,
		ShowsPageRange = 0x2,
		ShowsPaperSize = 0x4,
		ShowsOrientation = 0x8,
		ShowsScaling = 0x10,
		ShowsPrintSelection = 0x20,
		ShowsPageSetupAccessory = 0x100,
		ShowsPreview = 0x20000
	}
}
