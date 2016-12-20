namespace UnityEngine.VR
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>VR Input tracking data.</para>
    /// </summary>
    public static class InputTracking
    {
        /// <summary>
        /// <para>The current position of the requested VRNode.</para>
        /// </summary>
        /// <param name="node">Node index.</param>
        /// <returns>
        /// <para>Position of node local to its tracking space.</para>
        /// </returns>
        public static Vector3 GetLocalPosition(VRNode node)
        {
            Vector3 vector;
            INTERNAL_CALL_GetLocalPosition(node, out vector);
            return vector;
        }

        /// <summary>
        /// <para>The current rotation of the requested VRNode.</para>
        /// </summary>
        /// <param name="node">Node index.</param>
        /// <returns>
        /// <para>Rotation of node local to its tracking space.</para>
        /// </returns>
        public static Quaternion GetLocalRotation(VRNode node)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetLocalRotation(node, out quaternion);
            return quaternion;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetLocalPosition(VRNode node, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetLocalRotation(VRNode node, out Quaternion value);
        /// <summary>
        /// <para>Center tracking to the current position and orientation of the HMD.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Recenter();
    }
}

