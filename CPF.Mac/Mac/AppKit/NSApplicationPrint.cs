using CPF.Mac.Foundation;

namespace CPF.Mac.AppKit
{
	public delegate NSApplicationPrintReply NSApplicationPrint(NSApplication application, string[] fileNames, NSDictionary printSettings, bool showPrintPanels);
}
