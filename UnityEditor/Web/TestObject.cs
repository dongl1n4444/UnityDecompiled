namespace UnityEditor.Web
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class TestObject
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <BoolProperty>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <NumberProperty>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <StringProperty>k__BackingField;

        public bool BoolProperty { get; set; }

        public int NumberProperty { get; set; }

        public string StringProperty { get; set; }
    }
}

