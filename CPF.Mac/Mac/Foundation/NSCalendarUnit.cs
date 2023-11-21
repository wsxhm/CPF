using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSCalendarUnit : ulong
	{
		Era = 0x2,
		Year = 0x4,
		Month = 0x8,
		Day = 0x10,
		Hour = 0x20,
		Minute = 0x40,
		Second = 0x80,
		Week = 0x100,
		Weekday = 0x200,
		WeekdayOrdinal = 0x400,
		Quarter = 0x800,
		[Since(5, 0)]
		WeekOfMonth = 0x1000,
		[Since(5, 0)]
		WeekOfYear = 0x2000,
		[Since(5, 0)]
		YearForWeakOfYear = 0x4000,
		[Since(4, 0)]
		Calendar = 0x100000,
		[Since(4, 0)]
		TimeZone = 0x200000
	}
}
