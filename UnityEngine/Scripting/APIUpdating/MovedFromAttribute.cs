namespace UnityEngine.Scripting.APIUpdating
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class)]
    public class MovedFromAttribute : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Namespace>k__BackingField;

        public MovedFromAttribute(string sourceNamespace)
        {
            this.Namespace = sourceNamespace;
        }

        public string Namespace { get; private set; }
    }
}

