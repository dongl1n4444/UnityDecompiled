namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Struct | AttributeTargets.Class)]
    internal class NativeConditionalAttribute : Attribute, IBindingsAttribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Condition>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <Enabled>k__BackingField;

        public NativeConditionalAttribute()
        {
        }

        public NativeConditionalAttribute(bool enabled)
        {
            this.Enabled = enabled;
        }

        public NativeConditionalAttribute(string condition)
        {
            this.Condition = condition;
            this.Enabled = true;
        }

        public NativeConditionalAttribute(string condition, bool enabled) : this(condition)
        {
            this.Enabled = enabled;
        }

        public string Condition { get; set; }

        public bool Enabled { get; set; }
    }
}

