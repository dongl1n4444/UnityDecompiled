namespace UnityEngine
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    [UsedByNativeCode, AttributeUsage(AttributeTargets.Class)]
    public class DefaultExecutionOrder : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <order>k__BackingField;

        public DefaultExecutionOrder(int order)
        {
            this.order = order;
        }

        public int order { get; private set; }
    }
}

