namespace Unity.Bindings
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Struct | AttributeTargets.Class)]
    internal class NativeInstanceAttribute : Attribute, IBindingsAttribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Accessor>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Unity.Bindings.InvocationTargetKind <InvocationTargetKind>k__BackingField;

        public NativeInstanceAttribute()
        {
            this.InvocationTargetKind = Unity.Bindings.InvocationTargetKind.NonPointer;
        }

        internal NativeInstanceAttribute(string accessor)
        {
            this.Accessor = accessor;
            this.InvocationTargetKind = Unity.Bindings.InvocationTargetKind.NonPointer;
        }

        public NativeInstanceAttribute(Unity.Bindings.InvocationTargetKind kind)
        {
            this.InvocationTargetKind = kind;
        }

        public NativeInstanceAttribute(string accessor, Unity.Bindings.InvocationTargetKind kind)
        {
            this.Accessor = accessor;
            this.InvocationTargetKind = kind;
        }

        public string Accessor { get; set; }

        public Unity.Bindings.InvocationTargetKind InvocationTargetKind { get; set; }
    }
}

