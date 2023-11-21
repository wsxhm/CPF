using System;

namespace CPF.Mac.Foundation
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class OutletAttribute : ExportAttribute
	{
		public OutletAttribute()
		{
		}

		public OutletAttribute(string name)
			: base(name)
		{
		}
	}
}
