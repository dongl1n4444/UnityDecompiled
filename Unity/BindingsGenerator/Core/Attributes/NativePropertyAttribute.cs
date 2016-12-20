namespace Unity.BindingsGenerator.Core.Attributes
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property)]
    internal class NativePropertyAttribute : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <IsThreadSafe>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <Name>k__BackingField;

        public bool IsThreadSafe { get; set; }

        public string Name { get; set; }
    }
}

