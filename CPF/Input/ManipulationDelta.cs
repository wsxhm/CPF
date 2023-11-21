using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Input
{
	/// <summary>
	/// 包含在操作事件发生时累积的转换数据。
	/// </summary>
	public class ManipulationDelta
	{
		public Vector Translation
		{
			get;
			internal set;
		}

		public float Rotation
		{
			get;
			internal set;
		}

		public Vector Scale
		{
			get;
			internal set;
		}


		public ManipulationDelta(Vector translation, float rotation, Vector scale)
		{
			Translation = translation;
			Rotation = rotation;
			Scale = scale;
		}
	}
}
