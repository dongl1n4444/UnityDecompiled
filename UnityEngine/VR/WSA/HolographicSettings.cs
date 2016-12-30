namespace UnityEngine.VR.WSA
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The Holographic Settings contain functions which effect the performance and presentation of Holograms on Windows Holographic platforms.</para>
    /// </summary>
    public sealed class HolographicSettings
    {
        /// <summary>
        /// <para>Option to allow developers to achieve higher framerate at the cost of high latency.  By default this option is off.</para>
        /// </summary>
        /// <param name="activated">True to enable or false to disable Low Latent Frame Presentation.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ActivateLatentFramePresentation(bool activated);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_InternalSetFocusPointForFrame(ref Vector3 position);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_InternalSetFocusPointForFrameWithNormal(ref Vector3 position, ref Vector3 normal);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_InternalSetFocusPointForFrameWithNormalVelocity(ref Vector3 position, ref Vector3 normal, ref Vector3 velocity);
        private static void InternalSetFocusPointForFrame(Vector3 position)
        {
            INTERNAL_CALL_InternalSetFocusPointForFrame(ref position);
        }

        private static void InternalSetFocusPointForFrameWithNormal(Vector3 position, Vector3 normal)
        {
            INTERNAL_CALL_InternalSetFocusPointForFrameWithNormal(ref position, ref normal);
        }

        private static void InternalSetFocusPointForFrameWithNormalVelocity(Vector3 position, Vector3 normal, Vector3 velocity)
        {
            INTERNAL_CALL_InternalSetFocusPointForFrameWithNormalVelocity(ref position, ref normal, ref velocity);
        }

        /// <summary>
        /// <para>Sets a point in 3d space that is the focal point of the scene for the user for this frame. This helps improve the visual fidelity of content around this point. This must be set every frame.</para>
        /// </summary>
        /// <param name="position">The position of the focal point in the scene, relative to the camera.</param>
        /// <param name="normal">Surface normal of the plane being viewed at the focal point.</param>
        /// <param name="velocity">A vector that describes how the focus point is moving in the scene at this point in time. This allows the HoloLens to compensate for both your head movement and the movement of the object in the scene.</param>
        public static void SetFocusPointForFrame(Vector3 position)
        {
            InternalSetFocusPointForFrame(position);
        }

        /// <summary>
        /// <para>Sets a point in 3d space that is the focal point of the scene for the user for this frame. This helps improve the visual fidelity of content around this point. This must be set every frame.</para>
        /// </summary>
        /// <param name="position">The position of the focal point in the scene, relative to the camera.</param>
        /// <param name="normal">Surface normal of the plane being viewed at the focal point.</param>
        /// <param name="velocity">A vector that describes how the focus point is moving in the scene at this point in time. This allows the HoloLens to compensate for both your head movement and the movement of the object in the scene.</param>
        public static void SetFocusPointForFrame(Vector3 position, Vector3 normal)
        {
            InternalSetFocusPointForFrameWithNormal(position, normal);
        }

        /// <summary>
        /// <para>Sets a point in 3d space that is the focal point of the scene for the user for this frame. This helps improve the visual fidelity of content around this point. This must be set every frame.</para>
        /// </summary>
        /// <param name="position">The position of the focal point in the scene, relative to the camera.</param>
        /// <param name="normal">Surface normal of the plane being viewed at the focal point.</param>
        /// <param name="velocity">A vector that describes how the focus point is moving in the scene at this point in time. This allows the HoloLens to compensate for both your head movement and the movement of the object in the scene.</param>
        public static void SetFocusPointForFrame(Vector3 position, Vector3 normal, Vector3 velocity)
        {
            InternalSetFocusPointForFrameWithNormalVelocity(position, normal, velocity);
        }

        /// <summary>
        /// <para>Returns true if Holographic rendering is currently running with Latent Frame Presentation.  Default value is false.</para>
        /// </summary>
        public static bool IsLatentFramePresentation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

