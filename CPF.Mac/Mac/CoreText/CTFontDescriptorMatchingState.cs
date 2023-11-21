namespace CPF.Mac.CoreText
{
	public enum CTFontDescriptorMatchingState : uint
	{
		Started,
		Finished,
		WillBeginQuerying,
		Stalled,
		WillBeginDownloading,
		Downloading,
		DownloadingFinished,
		Matched,
		FailedWithError
	}
}
