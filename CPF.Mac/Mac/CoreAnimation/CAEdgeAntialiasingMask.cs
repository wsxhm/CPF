using System;

namespace CPF.Mac.CoreAnimation
{
	[Flags]
	public enum CAEdgeAntialiasingMask
	{
		LeftEdge = 0x1,
		RightEdge = 0x2,
		BottomEdge = 0x4,
		TopEdge = 0x8,
		All = 0xF,
		LeftRightEdges = 0x3,
		TopBottomEdges = 0xC
	}
}
