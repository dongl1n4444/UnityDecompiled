namespace UnityEngine.VR
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A collection of methods and properties for interacting with the VR tracking system.</para>
    /// </summary>
    public static class InputTracking
    {
        /// <summary>
        /// <para>Gets the position of a specific node.</para>
        /// </summary>
        /// <param name="node">Specifies which node's position should be returned.</param>
        /// <returns>
        /// <para>The position of the node in its local tracking space.</para>
        /// </returns>
        public static Vector3 GetLocalPosition(VRNode node)
        {
            Vector3 vector;
            INTERNAL_CALL_GetLocalPosition(node, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Gets the rotation of a specific node.</para>
        /// </summary>
        /// <param name="node">Specifies which node's rotation should be returned.</param>
        /// <returns>
        /// <para>The rotation of the node in its local tracking space.</para>
        /// </returns>
        public static Quaternion GetLocalRotation(VRNode node)
        {
            Quaternion quaternion;
            INTERNAL_CALL_GetLocalRotation(node, out quaternion);
            return quaternion;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetLocalPosition(VRNode node, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetLocalRotation(VRNode node, out Quaternion value);
        /// <summary>
        /// <para>Center tracking to the current position and orientation of the HMD.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Recenter();

        /// <summary>
        /// <para>Disables positional tracking in VR. If set to true the camera only tracks headset rotation state.</para>
        /// </summary>
        public static bool disablePositionalTracking { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

