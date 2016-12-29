namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>AvatarMask are used to mask out humanoid body parts and transforms.</para>
    /// </summary>
    public sealed class AvatarMask : Object
    {
        /// <summary>
        /// <para>Creates a new AvatarMask.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AvatarMask();
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
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool GetHumanoidBodyPartActive(AvatarMaskBodyPart index);
        /// <summary>
        /// <para>Returns true if the transform at the given index is active.</para>
        /// </summary>
        /// <param name="index">The index of the transform.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool GetTransformActive(int index);
        /// <summary>
        /// <para>Returns the path of the transform at the given index.</para>
        /// </summary>
        /// <param name="index">The index of the transform.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetTransformPath(int index);
        /// <summary>
        /// <para>Sets the humanoid body part at the given index to active or not.</para>
        /// </summary>
        /// <param name="index">The index of the humanoid body part.</param>
        /// <param name="value">Active or not.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetHumanoidBodyPartActive(AvatarMaskBodyPart index, bool value);
        /// <summary>
        /// <para>Sets the tranform at the given index to active or not.</para>
        /// </summary>
        /// <param name="index">The index of the transform.</param>
        /// <param name="value">Active or not.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetTransformActive(int index, bool value);
        /// <summary>
        /// <para>Sets the path of the transform at the given index.</para>
        /// </summary>
        /// <param name="index">The index of the transform.</param>
        /// <param name="path">The path of the transform.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetTransformPath(int index, string path);

        internal bool hasFeetIK { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [Obsolete("AvatarMask.humanoidBodyPartCount is deprecated. Use AvatarMaskBodyPart.LastBodyPart instead.")]
        private int humanoidBodyPartCount =>
            13;

        /// <summary>
        /// <para>Number of transforms.</para>
        /// </summary>
        public int transformCount { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

