namespace Unity.BindingsGenerator.Core.Attributes
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Enum)]
    internal class NativeEnumAttribute : Attribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <GenerateNativeType>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <Header>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <Name>k__BackingField;

        public bool GenerateNativeType { get; set; }

        public string Header { get; set; }

        public string Name { get; set; }
    }
}

