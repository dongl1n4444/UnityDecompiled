namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>A sphere-shaped primitive collider.</para>
    /// </summary>
    public sealed class SphereCollider : Collider
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_center(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_center(ref Vector3 value);

        /// <summary>
        /// <para>The center of the sphere in the object's local space.</para>
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
        /// <para>The radius of the sphere measured in the object's local space.</para>
        /// </summary>
        public float radius { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

