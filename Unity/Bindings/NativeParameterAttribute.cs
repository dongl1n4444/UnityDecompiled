namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Parameter)]
    internal class NativeParameterAttribute : Attribute, IBindingsAttribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <Unmarshalled>k__BackingField;

        public NativeParameterAttribute()
        {
        }

        public NativeParameterAttribute(bool unmarshalled)
        {
            this.Unmarshalled = unmarshalled;
        }

        public bool Unmarshalled { get; set; }
    }
}

