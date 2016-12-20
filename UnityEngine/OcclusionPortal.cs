namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>The portal for dynamically changing occlusion at runtime.</para>
    /// </summary>
    public sealed class OcclusionPortal : Component
    {
        /// <summary>
        /// <para>Gets / sets the portal's open state.</para>
        /// </summary>
        public bool open { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

