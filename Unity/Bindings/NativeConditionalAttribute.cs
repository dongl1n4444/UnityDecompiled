namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=false)]
    internal class NativeConditionalAttribute : Attribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <Condition>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <Enabled>k__BackingField;

        public NativeConditionalAttribute(bool enabled)
        {
            this.Condition = null;
            this.Enabled = enabled;
        }

        public NativeConditionalAttribute(string condition)
        {
            this.Condition = condition;
            this.Enabled = true;
        }

        public NativeConditionalAttribute(string condition, bool enabled)
        {
            this.Condition = condition;
            this.Enabled = enabled;
        }

        public string Condition { get; set; }

        public bool Enabled { get; set; }
    }
}

