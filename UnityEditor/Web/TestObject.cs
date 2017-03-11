namespace UnityEditor.Web
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal class TestObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <BoolProperty>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private int <NumberProperty>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <StringProperty>k__BackingField;

        public bool BoolProperty { get; set; }

        public int NumberProperty { get; set; }

        public string StringProperty { get; set; }
    }
}

