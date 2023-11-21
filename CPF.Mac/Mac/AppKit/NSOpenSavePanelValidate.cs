using CPF.Mac.Foundation;

namespace CPF.Mac.AppKit
{
	public delegate bool NSOpenSavePanelValidate(NSSavePanel panel, NSUrl url, out NSError outError);
}
