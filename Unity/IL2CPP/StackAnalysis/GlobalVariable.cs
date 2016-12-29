namespace Unity.IL2CPP.StackAnalysis
{
    using Mono.Cecil;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class GlobalVariable
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <BlockIndex>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <Index>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TypeReference <Type>k__BackingField;

        public int BlockIndex { get; internal set; }

        public int Index { get; internal set; }

        public TypeReference Type { get; internal set; }

        public string VariableName =>
            $"G_B{this.BlockIndex}_{this.Index}";
    }
}

