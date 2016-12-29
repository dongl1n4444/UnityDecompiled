namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Renders particles on to the screen (Shuriken).</para>
    /// </summary>
    public sealed class ParticleSystemRenderer : Renderer
    {
        /// <summary>
        /// <para>Query whether the particle system renderer uses a particular set of vertex streams.</para>
        /// </summary>
        /// <param name="streams">Streams to query.</param>
        /// <returns>
        /// <para>Whether all the queried streams are enabled or not.</para>
        /// </returns>
        public bool AreVertexStreamsEnabled(ParticleSystemVertexStreams streams) => 
            (this.Internal_GetEnabledVertexStreams(streams) == streams);

        /// <summary>
        /// <para>Disable a set of vertex shader streams on the particle system renderer.
        /// The position stream is always enabled, and any attempts to remove it will be ignored.</para>
        /// </summary>
        /// <param name="streams">Streams to disable.</param>
        public void DisableVertexStreams(ParticleSystemVertexStreams streams)
        {
            this.Internal_SetVertexStreams(streams, false);
        }

        /// <summary>
        /// <para>Enable a set of vertex shader streams on the particle system renderer.</para>
        /// </summary>
        /// <param name="streams">Streams to enable.</param>
        public void EnableVertexStreams(ParticleSystemVertexStreams streams)
        {
            this.Internal_SetVertexStreams(streams, true);
        }

        /// <summary>
        /// <para>Query whether the particle system renderer uses a particular set of vertex streams.</para>
        /// </summary>
        /// <param name="streams">Streams to query.</param>
        /// <returns>
        /// <para>Returns the subset of the queried streams that are actually enabled.</para>
        /// </returns>
        public ParticleSystemVertexStreams GetEnabledVertexStreams(ParticleSystemVertexStreams streams) => 
            this.Internal_GetEnabledVertexStreams(streams);

        /// <summary>
        /// <para>Set the array of meshes used as particles.</para>
        /// </summary>
        /// <param name="meshes">This array will be populated with the list of meshes being used for particle rendering.</param>
        /// <returns>
        /// <para>The number of meshes actually written to the destination array.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetMeshes(Mesh[] meshes);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_pivot(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern ParticleSystemVertexStreams Internal_GetEnabledVertexStreams(ParticleSystemVertexStreams streams);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int Internal_GetMeshCount();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_pivot(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_SetVertexStreams(ParticleSystemVertexStreams streams, bool enabled);
        /// <summary>
        /// <para>Set an array of meshes used as particles instead of a billboarded texture.</para>
        /// </summary>
        /// <param name="meshes">Array of meshes to be used.</param>
        /// <param name="size">Number of elements from the mesh array to be applied.</param>
        public void SetMeshes(Mesh[] meshes)
        {
            this.SetMeshes(meshes, meshes.Length);
        }

        /// <summary>
        /// <para>Set an array of meshes used as particles instead of a billboarded texture.</para>
        /// </summary>
        /// <param name="meshes">Array of meshes to be used.</param>
        /// <param name="size">Number of elements from the mesh array to be applied.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetMeshes(Mesh[] meshes, int size);

        /// <summary>
        /// <para>Control the direction that particles face.</para>
        /// </summary>
        public ParticleSystemRenderSpace alignment { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How much are the particles stretched depending on the Camera's speed.</para>
        /// </summary>
        public float cameraVelocityScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal bool editorEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How much are the particles stretched in their direction of motion.</para>
        /// </summary>
        public float lengthScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Clamp the maximum particle size.</para>
        /// </summary>
        public float maxParticleSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Mesh used as particle instead of billboarded texture.</para>
        /// </summary>
        public Mesh mesh { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The number of meshes being used for particle rendering.</para>
        /// </summary>
        public int meshCount =>
            this.Internal_GetMeshCount();

        /// <summary>
        /// <para>Clamp the minimum particle size.</para>
        /// </summary>
        public float minParticleSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How much are billboard particle normals oriented towards the camera.</para>
        /// </summary>
        public float normalDirection { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Modify the pivot point used for rotating particles.</para>
        /// </summary>
        public Vector3 pivot
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_pivot(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_pivot(ref value);
            }
        }

        /// <summary>
        /// <para>How particles are drawn.</para>
        /// </summary>
        public ParticleSystemRenderMode renderMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Biases particle system sorting amongst other transparencies.</para>
        /// </summary>
        public float sortingFudge { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Sort particles within a system.</para>
        /// </summary>
        public ParticleSystemSortMode sortMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Set the material used by the Trail module for attaching trails to particles.</para>
        /// </summary>
        public Material trailMaterial { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How much are the particles stretched depending on "how fast they move".</para>
        /// </summary>
        public float velocityScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

