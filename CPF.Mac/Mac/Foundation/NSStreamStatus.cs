namespace CPF.Mac.Foundation
{
	public enum NSStreamStatus : ulong
	{
		NotOpen,
		Opening,
		Open,
		Reading,
		Writing,
		AtEnd,
		Closed,
		Error
	}
}
