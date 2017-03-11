namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The Skinned Mesh filter.</para>
    /// </summary>
    public class SkinnedMeshRenderer : Renderer
    {
        /// <summary>
        /// <para>Creates a snapshot of SkinnedMeshRenderer and stores it in mesh.</para>
        /// </summary>
        /// <param name="mesh">A static mesh that will receive the snapshot of the skinned mesh.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void BakeMesh(Mesh mesh);
        /// <summary>
        /// <para>Returns weight of BlendShape on this renderer.</para>
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern float GetBlendShapeWeight(int index);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_localBounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_localBounds(ref Bounds value);
        /// <summary>
        /// <para>Sets the weight in percent of a BlendShape on this Renderer.</para>
        /// </summary>
        /// <param name="index">The index of the BlendShape to modify.</param>
        /// <param name="value">The weight in percent for this BlendShape.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetBlendShapeWeight(int index, float value);

        internal Transform actualRootBone { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The bones used to skin the mesh.</para>
        /// </summary>
        public Transform[] bones { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>AABB of this Skinned Mesh in its local space.</para>
        /// </summary>
        public Bounds localBounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_localBounds(out bounds);
                return bounds;
            }
            set
            {
                this.INTERNAL_set_localBounds(ref value);
            }
        }

        /// <summary>
        /// <para>The maximum number of bones affecting a single vertex.</para>
        /// </summary>
        public SkinQuality quality { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public Transform rootBone { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The mesh used for skinning.</para>
        /// </summary>
        public Mesh sharedMesh { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Specifies whether skinned motion vectors should be used for this renderer.</para>
        /// </summary>
        public bool skinnedMotionVectors { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If enabled, the Skinned Mesh will be updated when offscreen. If disabled, this also disables updating animations.</para>
        /// </summary>
        public bool updateWhenOffscreen { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

