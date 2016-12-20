namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>A capsule-shaped primitive collider.</para>
    /// </summary>
    public sealed class CapsuleCollider : Collider
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_center(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_center(ref Vector3 value);

        /// <summary>
        /// <para>The center of the capsule, measured in the object's local space.</para>
        /// </summary>
        public Vector3 center
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_center(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_center(ref value);
            }
        }

        /// <summary>
        /// <para>The direction of the capsule.</para>
        /// </summary>
        public int direction { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The height of the capsule meased in the object's local space.</para>
        /// </summary>
        public float height { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The radius of the sphere, measured in the object's local space.</para>
        /// </summary>
        public float radius { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

