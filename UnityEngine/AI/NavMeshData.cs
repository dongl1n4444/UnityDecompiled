namespace UnityEngine.AI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Contains and represents NavMesh data.</para>
    /// </summary>
    public sealed class NavMeshData : UnityEngine.Object
    {
        /// <summary>
        /// <para>Constructs a new object for representing a NavMesh for the default agent type.</para>
        /// </summary>
        public NavMeshData()
        {
            Internal_Create(this, 0);
        }

        /// <summary>
        /// <para>Constructs a new object representing a NavMesh for the specified agent type.</para>
        /// </summary>
        /// <param name="agentTypeID">The agent type ID to create a NavMesh for.</param>
        public NavMeshData(int agentTypeID)
        {
            Internal_Create(this, agentTypeID);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_Create([Writable] NavMeshData mono, int agentTypeID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_position(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_rotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_sourceBounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_position(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_rotation(ref Quaternion value);

        internal Vector3 position
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_position(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_position(ref value);
            }
        }

        internal Quaternion rotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_rotation(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_rotation(ref value);
            }
        }

        /// <summary>
        /// <para>Returns the bounding volume of the input geometry used to build this NavMesh (Read Only).</para>
        /// </summary>
        public Bounds sourceBounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_sourceBounds(out bounds);
                return bounds;
            }
        }
    }
}

