using System;
using System.Linq;
using Unity.IL2CPP.Common.CFG;

namespace Unity.IL2CPP
{
	internal static class DeadBlockAnalysis
	{
		public static void MarkBlocksDeadIfNeeded(InstructionBlock[] instructionBlocks)
		{
			if (instructionBlocks.Count<InstructionBlock>() != 1)
			{
				for (int i = 0; i < instructionBlocks.Length; i++)
				{
					InstructionBlock instructionBlock = instructionBlocks[i];
					instructionBlock.MarkIsDead();
				}
				instructionBlocks[0].MarkIsAliveRecursive();
			}
		}
	}
}
