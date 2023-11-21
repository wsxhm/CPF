namespace CPF.Mac.AppKit
{
	public enum NSTouchPhase : ulong
	{
		Began = 1uL,
		Moved = 2uL,
		Stationary = 4uL,
		Ended = 8uL,
		Cancelled = 0x10,
		Touching = 7uL,
		Any = ulong.MaxValue
	}
}
