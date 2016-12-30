namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;
    using UnityEngine.Scripting.APIUpdating;

    /// <summary>
    /// <para>AvatarMask are used to mask out humanoid body parts and transforms.</para>
    /// </summary>
    [MovedFrom("UnityEditor.Animations")]
    public sealed class AvatarMask : UnityEngine.Object
    {
        /// <summary>
        /// <para>Creates a new AvatarMask.</para>
        /// </summary>
        public AvatarMask()
        {
            Internal_Create(this);
        }

        [ExcludeFromDocs]
        public void AddTransformPath(Transform transform)
        {
            bool recursive = true;
            this.AddTransformPath(transform, recursive);
        }

        /// <summary>
        /// <para>Adds a transform path into the AvatarMask.</para>
        /// </summary>
        /// <param name="transform">The transform to add into the AvatarMask.</param>
        /// <param name="recursive">Whether to also add all children of the specified transform.</param>
        public void AddTransformPath(Transform transform, [DefaultValue("true")] bool recursive)
        {
            if (transform == null)
            {
                throw new ArgumentNullException("transform");
            }
            this.Internal_AddTransformPath(transform, recursive);
        }

        internal void Copy(AvatarMask other)
        {
            for (AvatarMaskBodyPart part = AvatarMaskBodyPart.Root; part < AvatarMaskBodyPart.LastBodyPart; part += 1)
            {
                this.SetHumanoidBodyPartActive(part, other.GetHumanoidBodyPartActive(part));
            }
            this.transformCount = other.transformCount;
            for (int i = 0; i < other.transformCount; i++)
            {
                this.SetTransformPath(i, other.GetTransformPath(i));
                this.SetTransformActive(i, other.GetTransformActive(i));
            }
        }

        /// <summary>
        /// <para>Returns true if the humanoid body part at the given index is active.</para>
        /// </summary>
        /// <param name="index">The index of the humanoid body part.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool GetHumanoidBodyPartActive(AvatarMaskBodyPart index);
        /// <summary>
        /// <para>Returns true if the transform at the given index is active.</para>
        /// </summary>
        /// <param name="index">The index of the transform.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool GetTransformActive(int index);
        /// <summary>
        /// <para>Returns the path of the transform at the given index.</para>
        /// </summary>
        /// <param name="index">The index of the transform.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern string GetTransformPath(int index);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_AddTransformPath(Transform transform, bool recursive);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_Create([Writable] AvatarMask mono);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_RemoveTransformPath(Transform transform, bool recursive);
        [ExcludeFromDocs]
        public void RemoveTransformPath(Transform transform)
        {
            bool recursive = true;
            this.RemoveTransformPath(transform, recursive);
        }

        /// <summary>
        /// <para>Removes a transform path from the AvatarMask.</para>
        /// </summary>
        /// <param name="transform">The Transform that should be removed from the AvatarMask.</param>
        /// <param name="recursive">Whether to also remove all children of the specified transform.</param>
        public void RemoveTransformPath(Transform transform, [DefaultValue("true")] bool recursive)
        {
            if (transform == null)
            {
                throw new ArgumentNullException("transform");
            }
            this.Internal_RemoveTransformPath(transform, recursive);
        }

        /// <summary>
        /// <para>Sets the humanoid body part at the given index to active or not.</para>
        /// </summary>
        /// <param name="index">The index of the humanoid body part.</param>
        /// <param name="value">Active or not.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetHumanoidBodyPartActive(AvatarMaskBodyPart index, bool value);
        /// <summary>
        /// <para>Sets the tranform at the given index to active or not.</para>
        /// </summary>
        /// <param name="index">The index of the transform.</param>
        /// <param name="value">Active or not.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetTransformActive(int index, bool value);
        /// <summary>
        /// <para>Sets the path of the transform at the given index.</para>
        /// </summary>
        /// <param name="index">The index of the transform.</param>
        /// <param name="path">The path of the transform.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetTransformPath(int index, string path);

        internal bool hasFeetIK { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Obsolete("AvatarMask.humanoidBodyPartCount is deprecated. Use AvatarMaskBodyPart.LastBodyPart instead.")]
        private int humanoidBodyPartCount =>
            13;

        /// <summary>
        /// <para>Number of transforms.</para>
        /// </summary>
        public int transformCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

