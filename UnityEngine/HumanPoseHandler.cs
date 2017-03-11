namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A handler that lets you read or write a HumanPose from or to a humanoid avatar skeleton hierarchy.</para>
    /// </summary>
    public sealed class HumanPoseHandler : IDisposable
    {
        internal IntPtr m_Ptr = IntPtr.Zero;

        /// <summary>
        /// <para>Creates a human pose handler from an avatar and a root transform.</para>
        /// </summary>
        /// <param name="avatar">The avatar that defines the humanoid rig on skeleton hierarchy with root as the top most parent.</param>
        /// <param name="root">The top most node of the skeleton hierarchy defined in humanoid avatar.</param>
        public HumanPoseHandler(Avatar avatar, Transform root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("HumanPoseHandler root Transform is null");
            }
            if (avatar == null)
            {
                throw new ArgumentNullException("HumanPoseHandler avatar is null");
            }
            if (!avatar.isValid)
            {
                throw new ArgumentException("HumanPoseHandler avatar is invalid");
            }
            if (!avatar.isHuman)
            {
                throw new ArgumentException("HumanPoseHandler avatar is not human");
            }
            this.Internal_HumanPoseHandler(avatar, root);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Dispose();
        public void GetHumanPose(ref HumanPose humanPose)
        {
            humanPose.Init();
            if (!this.Internal_GetHumanPose(ref humanPose.bodyPosition, ref humanPose.bodyRotation, humanPose.muscles))
            {
                Debug.LogWarning("HumanPoseHandler is not initialized properly");
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_Internal_GetHumanPose(HumanPoseHandler self, ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_Internal_SetHumanPose(HumanPoseHandler self, ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles);
        private bool Internal_GetHumanPose(ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles) => 
            INTERNAL_CALL_Internal_GetHumanPose(this, ref bodyPosition, ref bodyRotation, muscles);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_HumanPoseHandler(Avatar avatar, Transform root);
        private bool Internal_SetHumanPose(ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles) => 
            INTERNAL_CALL_Internal_SetHumanPose(this, ref bodyPosition, ref bodyRotation, muscles);

        public void SetHumanPose(ref HumanPose humanPose)
        {
            humanPose.Init();
            if (!this.Internal_SetHumanPose(ref humanPose.bodyPosition, ref humanPose.bodyRotation, humanPose.muscles))
            {
                Debug.LogWarning("HumanPoseHandler is not initialized properly");
            }
        }
    }
}

