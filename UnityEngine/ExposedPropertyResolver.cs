namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Object that is used to resolve references to an ExposedReference field.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ExposedPropertyResolver
    {
        internal IntPtr table;
        internal static UnityEngine.Object ResolveReferenceInternal(IntPtr ptr, PropertyName name, out bool isValid) => 
            INTERNAL_CALL_ResolveReferenceInternal(ptr, ref name, out isValid);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern UnityEngine.Object INTERNAL_CALL_ResolveReferenceInternal(IntPtr ptr, ref PropertyName name, out bool isValid);
    }
}

