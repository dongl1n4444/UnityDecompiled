namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Enum)]
    internal class NativeEnumAttribute : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <GenerateNativeType>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Header>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Name>k__BackingField;

        public bool GenerateNativeType { get; set; }

        public string Header { get; set; }

        public string Name { get; set; }
    }
}

