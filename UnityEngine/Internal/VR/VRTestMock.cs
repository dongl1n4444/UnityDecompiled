namespace UnityEngine.Internal.VR
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    public static class VRTestMock
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void AddController(string controllerName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void AddTrackedDevice(string deviceName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_UpdateLeftEye(ref Vector3 position, ref Quaternion rotation);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_UpdateLeftHand(ref Vector3 position, ref Quaternion rotation);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_UpdateRightEye(ref Vector3 position, ref Quaternion rotation);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_UpdateRightHand(ref Vector3 position, ref Quaternion rotation);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_UpdateTrackedDevice(string deviceName, ref Vector3 position, ref Quaternion rotation);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Reset();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void UpdateControllerAxis(string controllerName, int axis, float value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void UpdateControllerButton(string controllerName, int button, bool pressed);
        public static void UpdateLeftEye(Vector3 position, Quaternion rotation)
        {
            INTERNAL_CALL_UpdateLeftEye(ref position, ref rotation);
        }

        public static void UpdateLeftHand(Vector3 position, Quaternion rotation)
        {
            INTERNAL_CALL_UpdateLeftHand(ref position, ref rotation);
        }

        public static void UpdateRightEye(Vector3 position, Quaternion rotation)
        {
            INTERNAL_CALL_UpdateRightEye(ref position, ref rotation);
        }

        public static void UpdateRightHand(Vector3 position, Quaternion rotation)
        {
            INTERNAL_CALL_UpdateRightHand(ref position, ref rotation);
        }

        public static void UpdateTrackedDevice(string deviceName, Vector3 position, Quaternion rotation)
        {
            INTERNAL_CALL_UpdateTrackedDevice(deviceName, ref position, ref rotation);
        }
    }
}

