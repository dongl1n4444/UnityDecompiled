namespace Unity.IL2CPP.Common.CFG
{
    using System;

    public class InstructionData
    {
        public readonly int StackAfter;
        public readonly int StackBefore;

        public InstructionData(int before, int after)
        {
            this.StackBefore = before;
            this.StackAfter = after;
        }
    }
}

