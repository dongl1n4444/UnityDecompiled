namespace Unity.IL2CPP.Common.CFG
{
    using System;

    public class BlockRange
    {
        public readonly InstructionBlock End;
        public readonly InstructionBlock Start;

        public BlockRange(InstructionBlock start, InstructionBlock end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}

