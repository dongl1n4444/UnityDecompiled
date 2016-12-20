namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>A base class for all 2D effectors.</para>
    /// </summary>
    public class Effector2D : Behaviour
    {
        /// <summary>
        /// <para>The mask used to select specific layers allowed to interact with the effector.</para>
        /// </summary>
        public int colliderMask { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal bool designedForNonTrigger { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal bool designedForTrigger { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal bool requiresCollider { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Should the collider-mask be used or the global collision matrix?</para>
        /// </summary>
        public bool useColliderMask { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

