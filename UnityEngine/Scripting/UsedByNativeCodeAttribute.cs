namespace UnityEngine.Scripting
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class, Inherited=false)]
    internal class UsedByNativeCodeAttribute : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Name>k__BackingField;

        public UsedByNativeCodeAttribute()
        {
        }

        public UsedByNativeCodeAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}

