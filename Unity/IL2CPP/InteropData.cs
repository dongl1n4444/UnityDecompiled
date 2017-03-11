namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public sealed class InteropData
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <HasCreateCCWFunction>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <HasDelegatePInvokeWrapperMethod>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <HasGuid>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <HasPInvokeMarshalingFunctions>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <Index>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TypeReference <Type>k__BackingField;

        public InteropData(TypeReference type, int index)
        {
            this.Type = type;
            this.Index = index;
        }

        public bool HasCreateCCWFunction { get; set; }

        public bool HasDelegatePInvokeWrapperMethod { get; set; }

        public bool HasGuid { get; set; }

        public bool HasPInvokeMarshalingFunctions { get; set; }

        public int Index { get; private set; }

        public TypeReference Type { get; private set; }
    }
}

