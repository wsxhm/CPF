using System;

namespace CPF.Mac.Foundation
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ActionAttribute : ExportAttribute
	{
		public ActionAttribute()
		{
		}

		public ActionAttribute(string selector)
			: base(selector)
		{
		}
	}
}
