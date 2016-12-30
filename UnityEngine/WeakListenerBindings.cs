namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    internal sealed class WeakListenerBindings
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void InvokeCallbacks(object inst, GCHandle gchandle, object[] parameters);
    }
}

