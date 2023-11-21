using CPF.Mac.ObjCRuntime;

namespace CPF.Mac.AppKit
{
	[Lion]
	public enum NSTextFinderAction : long
	{
		ShowFindInterface = 1L,
		NextMatch,
		PreviousMatch,
		ReplaceAll,
		Replace,
		ReplaceAndFind,
		SetSearchString,
		ReplaceAllInSelection,
		SelectAll,
		SelectAllInSelection,
		HideFindInterface,
		ShowReplaceInterface,
		HideReplaceInterface
	}
}
