namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>A script interface for the.</para>
    /// </summary>
    public sealed class Skybox : Behaviour
    {
        /// <summary>
        /// <para>The material used by the skybox.</para>
        /// </summary>
        public Material material { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

