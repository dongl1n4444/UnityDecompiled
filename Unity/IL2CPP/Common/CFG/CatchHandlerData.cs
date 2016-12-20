namespace Unity.IL2CPP.Common.CFG
{
    using Mono.Cecil;
    using System;

    public class CatchHandlerData
    {
        public readonly BlockRange Range;
        public readonly TypeReference Type;

        public CatchHandlerData(TypeReference type, BlockRange range)
        {
            this.Type = type;
            this.Range = range;
        }
    }
}

