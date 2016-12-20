namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>A capsule-shaped primitive collider.</para>
    /// </summary>
    public sealed class CapsuleCollider2D : Collider2D
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_size(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_size(ref Vector2 value);

        /// <summary>
        /// <para>The direction that the capsule sides can extend.</para>
        /// </summary>
        public CapsuleDirection2D direction { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The width and height of the capsule area.</para>
        /// </summary>
        public Vector2 size
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_size(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_size(ref value);
            }
        }
    }
}

