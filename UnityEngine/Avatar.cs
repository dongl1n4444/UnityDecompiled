namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Avatar definition.</para>
    /// </summary>
    public sealed class Avatar : UnityEngine.Object
    {
        private Avatar()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern float GetAxisLength(int humanId);
        internal Vector3 GetLimitSign(int humanId)
        {
            Vector3 vector;
            INTERNAL_CALL_GetLimitSign(this, humanId, out vector);
            return vector;
        }

        internal Quaternion GetPostRotation(int humanId)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetPostRotation(this, humanId, out quaternion);
            return quaternion;
        }

        internal Quaternion GetPreRotation(int humanId)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetPreRotation(this, humanId, out quaternion);
            return quaternion;
        }

        internal Quaternion GetZYPostQ(int humanId, Quaternion parentQ, Quaternion q)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetZYPostQ(this, humanId, ref parentQ, ref q, out quaternion);
            return quaternion;
        }

        internal Quaternion GetZYRoll(int humanId, Vector3 uvw)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetZYRoll(this, humanId, ref uvw, out quaternion);
            return quaternion;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetLimitSign(Avatar self, int humanId, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetPostRotation(Avatar self, int humanId, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetPreRotation(Avatar self, int humanId, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetZYPostQ(Avatar self, int humanId, ref Quaternion parentQ, ref Quaternion q, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetZYRoll(Avatar self, int humanId, ref Vector3 uvw, out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SetMuscleMinMax(int muscleId, float min, float max);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SetParameter(int parameterId, float value);

        /// <summary>
        /// <para>Return true if this avatar is a valid human avatar.</para>
        /// </summary>
        public bool isHuman { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Return true if this avatar is a valid mecanim avatar. It can be a generic avatar or a human avatar.</para>
        /// </summary>
        public bool isValid { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

