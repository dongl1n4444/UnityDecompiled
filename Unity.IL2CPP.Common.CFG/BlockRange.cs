using System;

namespace Unity.IL2CPP.Common.CFG
{
	public class BlockRange
	{
		public readonly InstructionBlock Start;

		public readonly InstructionBlock End;

		public BlockRange(InstructionBlock start, InstructionBlock end)
		{
			this.Start = start;
			this.End = end;
		}
	}
}
