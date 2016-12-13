using System;

namespace Unity.IL2CPP.Common.CFG
{
	public class InstructionData
	{
		public readonly int StackBefore;

		public readonly int StackAfter;

		public InstructionData(int before, int after)
		{
			this.StackBefore = before;
			this.StackAfter = after;
		}
	}
}
