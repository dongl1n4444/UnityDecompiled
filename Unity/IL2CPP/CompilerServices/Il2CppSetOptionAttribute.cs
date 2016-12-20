namespace Unity.IL2CPP.CompilerServices
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, Inherited=false, AllowMultiple=true)]
    public class Il2CppSetOptionAttribute : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Unity.IL2CPP.CompilerServices.Option <Option>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object <Value>k__BackingField;

        public Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option option, object value)
        {
            this.Option = option;
            this.Value = value;
        }

        public Unity.IL2CPP.CompilerServices.Option Option { get; private set; }

        public object Value { get; private set; }
    }
}

