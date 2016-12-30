namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A mesh collider allows you to do between meshes and primitives.</para>
    /// </summary>
    public sealed class MeshCollider : Collider
    {
        /// <summary>
        /// <para>Use a convex collider from the mesh.</para>
        /// </summary>
        public bool convex { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Allow the physics engine to increase the volume of the input mesh in attempt to generate a valid convex mesh.</para>
        /// </summary>
        public bool inflateMesh { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The mesh object used for collision detection.</para>
        /// </summary>
        public Mesh sharedMesh { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Used when set to inflateMesh to determine how much inflation is acceptable.</para>
        /// </summary>
        public float skinWidth { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Uses interpolated normals for sphere collisions instead of flat polygonal normals.</para>
        /// </summary>
        [Obsolete("Configuring smooth sphere collisions is no longer needed. PhysX3 has a better behaviour in place.")]
        public bool smoothSphereCollisions
        {
            get => 
                true;
            set
            {
            }
        }
    }
}

