namespace Unity.IL2CPP
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class MethodBodyWriterDebugOptions
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <EmitBlockInfo>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <EmitIlCode>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <EmitInputAndOutputs>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <EmitLineNumbers>k__BackingField;

        public bool EmitBlockInfo { get; set; }

        public bool EmitIlCode { get; set; }

        public bool EmitInputAndOutputs { get; set; }

        public bool EmitLineNumbers { get; set; }
    }
}

