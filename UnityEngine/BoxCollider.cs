namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>A box-shaped primitive collider.</para>
    /// </summary>
    public sealed class BoxCollider : Collider
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_center(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_size(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_center(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_size(ref Vector3 value);

        /// <summary>
        /// <para>The center of the box, measured in the object's local space.</para>
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

        [Obsolete("use BoxCollider.size instead.")]
        public Vector3 extents
        {
            get
            {
                return (Vector3) (this.size * 0.5f);
            }
            set
            {
                this.size = (Vector3) (value * 2f);
            }
        }

        /// <summary>
        /// <para>The size of the box, measured in the object's local space.</para>
        /// </summary>
        public Vector3 size
        {
            get
            {
                Vector3 vector;
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

