namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Renders particles on to the screen (Shuriken).</para>
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public sealed class ParticleSystemRenderer : Renderer
    {
        /// <summary>
        /// <para>Query whether the particle system renderer uses a particular set of vertex streams.</para>
        /// </summary>
        /// <param name="streams">Streams to query.</param>
        /// <returns>
        /// <para>Whether all the queried streams are enabled or not.</para>
        /// </returns>
        [Obsolete("AreVertexStreamsEnabled is deprecated. Use GetActiveVertexStreams instead.")]
        public bool AreVertexStreamsEnabled(ParticleSystemVertexStreams streams) => 
            (this.Internal_GetEnabledVertexStreams(streams) == streams);

        /// <summary>
        /// <para>Disable a set of vertex shader streams on the particle system renderer.
        /// The position stream is always enabled, and any attempts to remove it will be ignored.</para>
        /// </summary>
        /// <param name="streams">Streams to disable.</param>
        [Obsolete("DisableVertexStreams is deprecated. Use SetActiveVertexStreams instead.")]
        public void DisableVertexStreams(ParticleSystemVertexStreams streams)
        {
            this.Internal_SetVertexStreams(streams, false);
        }

        /// <summary>
        /// <para>Enable a set of vertex shader streams on the particle system renderer.</para>
        /// </summary>
        /// <param name="streams">Streams to enable.</param>
        [Obsolete("EnableVertexStreams is deprecated. Use SetActiveVertexStreams instead.")]
        public void EnableVertexStreams(ParticleSystemVertexStreams streams)
        {
            this.Internal_SetVertexStreams(streams, true);
        }

        public void GetActiveVertexStreams(List<ParticleSystemVertexStream> streams)
        {
            if (streams == null)
            {
                throw new ArgumentNullException("streams");
            }
            this.GetActiveVertexStreamsInternal(streams);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void GetActiveVertexStreamsInternal(object streams);
        /// <summary>
        /// <para>Query whether the particle system renderer uses a particular set of vertex streams.</para>
        /// </summary>
        /// <param name="streams">Streams to query.</param>
        /// <returns>
        /// <para>Returns the subset of the queried streams that are actually enabled.</para>
        /// </returns>
        [Obsolete("GetEnabledVertexStreams is deprecated. Use GetActiveVertexStreams instead.")]
        public ParticleSystemVertexStreams GetEnabledVertexStreams(ParticleSystemVertexStreams streams) => 
            this.Internal_GetEnabledVertexStreams(streams);

        /// <summary>
        /// <para>Set the array of meshes used as particles.</para>
        /// </summary>
        /// <param name="meshes">This array will be populated with the list of meshes being used for particle rendering.</param>
        /// <returns>
        /// <para>The number of meshes actually written to the destination array.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int GetMeshes(Mesh[] meshes);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_pivot(out Vector3 value);
        [Obsolete("Internal_GetVertexStreams is deprecated. Use GetActiveVertexStreams instead.")]
        internal ParticleSystemVertexStreams Internal_GetEnabledVertexStreams(ParticleSystemVertexStreams streams)
        {
            List<ParticleSystemVertexStream> list = new List<ParticleSystemVertexStream>(this.activeVertexStreamsCount);
            this.GetActiveVertexStreams(list);
            ParticleSystemVertexStreams none = ParticleSystemVertexStreams.None;
            if (list.Contains(ParticleSystemVertexStream.Position))
            {
                none |= ParticleSystemVertexStreams.Position;
            }
            if (list.Contains(ParticleSystemVertexStream.Normal))
            {
                none |= ParticleSystemVertexStreams.Normal;
            }
            if (list.Contains(ParticleSystemVertexStream.Tangent))
            {
                none |= ParticleSystemVertexStreams.Tangent;
            }
            if (list.Contains(ParticleSystemVertexStream.Color))
            {
                none |= ParticleSystemVertexStreams.Color;
            }
            if (list.Contains(ParticleSystemVertexStream.UV))
            {
                none |= ParticleSystemVertexStreams.UV;
            }
            if (list.Contains(ParticleSystemVertexStream.UV2))
            {
                none |= ParticleSystemVertexStreams.UV2BlendAndFrame;
            }
            if (list.Contains(ParticleSystemVertexStream.Center))
            {
                none |= ParticleSystemVertexStreams.CenterAndVertexID;
            }
            if (list.Contains(ParticleSystemVertexStream.SizeXYZ))
            {
                none |= ParticleSystemVertexStreams.Size;
            }
            if (list.Contains(ParticleSystemVertexStream.Rotation3D))
            {
                none |= ParticleSystemVertexStreams.Rotation;
            }
            if (list.Contains(ParticleSystemVertexStream.Velocity))
            {
                none |= ParticleSystemVertexStreams.Velocity;
            }
            if (list.Contains(ParticleSystemVertexStream.AgePercent))
            {
                none |= ParticleSystemVertexStreams.Lifetime;
            }
            if (list.Contains(ParticleSystemVertexStream.Custom1XYZW))
            {
                none |= ParticleSystemVertexStreams.Custom1;
            }
            if (list.Contains(ParticleSystemVertexStream.Custom2XYZW))
            {
                none |= ParticleSystemVertexStreams.Custom2;
            }
            if (list.Contains(ParticleSystemVertexStream.StableRandomXYZ))
            {
                none |= ParticleSystemVertexStreams.Random;
            }
            return (none & streams);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern int Internal_GetMeshCount();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_pivot(ref Vector3 value);
        [Obsolete("Internal_SetVertexStreams is deprecated. Use SetActiveVertexStreams instead.")]
        internal void Internal_SetVertexStreams(ParticleSystemVertexStreams streams, bool enabled)
        {
            List<ParticleSystemVertexStream> list = new List<ParticleSystemVertexStream>(this.activeVertexStreamsCount);
            this.GetActiveVertexStreams(list);
            if (enabled)
            {
                if (((streams & ParticleSystemVertexStreams.Position) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.Position))
                {
                    list.Add(ParticleSystemVertexStream.Position);
                }
                if (((streams & ParticleSystemVertexStreams.Normal) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.Normal))
                {
                    list.Add(ParticleSystemVertexStream.Normal);
                }
                if (((streams & ParticleSystemVertexStreams.Tangent) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.Tangent))
                {
                    list.Add(ParticleSystemVertexStream.Tangent);
                }
                if (((streams & ParticleSystemVertexStreams.Color) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.Color))
                {
                    list.Add(ParticleSystemVertexStream.Color);
                }
                if (((streams & ParticleSystemVertexStreams.UV) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.UV))
                {
                    list.Add(ParticleSystemVertexStream.UV);
                }
                if (((streams & ParticleSystemVertexStreams.UV2BlendAndFrame) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.UV2))
                {
                    list.Add(ParticleSystemVertexStream.UV2);
                    list.Add(ParticleSystemVertexStream.AnimBlend);
                    list.Add(ParticleSystemVertexStream.AnimFrame);
                }
                if (((streams & ParticleSystemVertexStreams.CenterAndVertexID) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.Center))
                {
                    list.Add(ParticleSystemVertexStream.Center);
                    list.Add(ParticleSystemVertexStream.VertexID);
                }
                if (((streams & ParticleSystemVertexStreams.Size) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.SizeXYZ))
                {
                    list.Add(ParticleSystemVertexStream.SizeXYZ);
                }
                if (((streams & ParticleSystemVertexStreams.Rotation) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.Rotation3D))
                {
                    list.Add(ParticleSystemVertexStream.Rotation3D);
                }
                if (((streams & ParticleSystemVertexStreams.Velocity) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.Velocity))
                {
                    list.Add(ParticleSystemVertexStream.Velocity);
                }
                if (((streams & ParticleSystemVertexStreams.Lifetime) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.AgePercent))
                {
                    list.Add(ParticleSystemVertexStream.AgePercent);
                    list.Add(ParticleSystemVertexStream.InvStartLifetime);
                }
                if (((streams & ParticleSystemVertexStreams.Custom1) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.Custom1XYZW))
                {
                    list.Add(ParticleSystemVertexStream.Custom1XYZW);
                }
                if (((streams & ParticleSystemVertexStreams.Custom2) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.Custom2XYZW))
                {
                    list.Add(ParticleSystemVertexStream.Custom2XYZW);
                }
                if (((streams & ParticleSystemVertexStreams.Random) != ParticleSystemVertexStreams.None) && !list.Contains(ParticleSystemVertexStream.StableRandomXYZ))
                {
                    list.Add(ParticleSystemVertexStream.StableRandomXYZ);
                    list.Add(ParticleSystemVertexStream.VaryingRandomX);
                }
            }
            else
            {
                if ((streams & ParticleSystemVertexStreams.Position) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.Position);
                }
                if ((streams & ParticleSystemVertexStreams.Normal) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.Normal);
                }
                if ((streams & ParticleSystemVertexStreams.Tangent) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.Tangent);
                }
                if ((streams & ParticleSystemVertexStreams.Color) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.Color);
                }
                if ((streams & ParticleSystemVertexStreams.UV) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.UV);
                }
                if ((streams & ParticleSystemVertexStreams.UV2BlendAndFrame) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.UV2);
                    list.Remove(ParticleSystemVertexStream.AnimBlend);
                    list.Remove(ParticleSystemVertexStream.AnimFrame);
                }
                if ((streams & ParticleSystemVertexStreams.CenterAndVertexID) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.Center);
                    list.Remove(ParticleSystemVertexStream.VertexID);
                }
                if ((streams & ParticleSystemVertexStreams.Size) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.SizeXYZ);
                }
                if ((streams & ParticleSystemVertexStreams.Rotation) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.Rotation3D);
                }
                if ((streams & ParticleSystemVertexStreams.Velocity) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.Velocity);
                }
                if ((streams & ParticleSystemVertexStreams.Lifetime) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.AgePercent);
                    list.Remove(ParticleSystemVertexStream.InvStartLifetime);
                }
                if ((streams & ParticleSystemVertexStreams.Custom1) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.Custom1XYZW);
                }
                if ((streams & ParticleSystemVertexStreams.Custom2) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.Custom2XYZW);
                }
                if ((streams & ParticleSystemVertexStreams.Random) != ParticleSystemVertexStreams.None)
                {
                    list.Remove(ParticleSystemVertexStream.StableRandomXYZW);
                    list.Remove(ParticleSystemVertexStream.VaryingRandomX);
                }
            }
            this.SetActiveVertexStreams(list);
        }

        public void SetActiveVertexStreams(List<ParticleSystemVertexStream> streams)
        {
            if (streams == null)
            {
                throw new ArgumentNullException("streams");
            }
            this.SetActiveVertexStreamsInternal(streams);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SetActiveVertexStreamsInternal(object streams);
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
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetMeshes(Mesh[] meshes, int size);

        /// <summary>
        /// <para>The number of currently active custom vertex streams.</para>
        /// </summary>
        public int activeVertexStreamsCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Control the direction that particles face.</para>
        /// </summary>
        public ParticleSystemRenderSpace alignment { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>How much are the particles stretched depending on the Camera's speed.</para>
        /// </summary>
        public float cameraVelocityScale { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>How much are the particles stretched in their direction of motion.</para>
        /// </summary>
        public float lengthScale { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Clamp the maximum particle size.</para>
        /// </summary>
        public float maxParticleSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Mesh used as particle instead of billboarded texture.</para>
        /// </summary>
        public Mesh mesh { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The number of meshes being used for particle rendering.</para>
        /// </summary>
        public int meshCount =>
            this.Internal_GetMeshCount();

        /// <summary>
        /// <para>Clamp the minimum particle size.</para>
        /// </summary>
        public float minParticleSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>How much are billboard particle normals oriented towards the camera.</para>
        /// </summary>
        public float normalDirection { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

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
        public ParticleSystemRenderMode renderMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Biases particle system sorting amongst other transparencies.</para>
        /// </summary>
        public float sortingFudge { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Sort particles within a system.</para>
        /// </summary>
        public ParticleSystemSortMode sortMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set the material used by the Trail module for attaching trails to particles.</para>
        /// </summary>
        public Material trailMaterial { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>How much are the particles stretched depending on "how fast they move".</para>
        /// </summary>
        public float velocityScale { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

