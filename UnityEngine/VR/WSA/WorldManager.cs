namespace UnityEngine.VR.WSA
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>This class represents the state of the real world tracking system.</para>
    /// </summary>
    public sealed class WorldManager
    {
        public static  event OnPositionalLocatorStateChangedDelegate OnPositionalLocatorStateChanged;

        /// <summary>
        /// <para>Return the native pointer to Windows::Perception::Spatial::ISpatialCoordinateSystem which was retrieved from an Windows::Perception::Spatial::ISpatialStationaryFrameOfReference object underlying the Unity World Origin.</para>
        /// </summary>
        /// <returns>
        /// <para>Pointer to Windows::Perception::Spatial::ISpatialCoordinateSystem.</para>
        /// </returns>
        public static IntPtr GetNativeISpatialCoordinateSystemPtr()
        {
            IntPtr ptr;
            INTERNAL_CALL_GetNativeISpatialCoordinateSystemPtr(out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetNativeISpatialCoordinateSystemPtr(out IntPtr value);
        [RequiredByNativeCode]
        private static void Internal_TriggerPositionalLocatorStateChanged(PositionalLocatorState oldState, PositionalLocatorState newState)
        {
            if (OnPositionalLocatorStateChanged != null)
            {
                OnPositionalLocatorStateChanged(oldState, newState);
            }
        }

        /// <summary>
        /// <para>The current state of the world tracking systems.</para>
        /// </summary>
        public static PositionalLocatorState state { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Callback on when the world tracking systems state has changed.</para>
        /// </summary>
        /// <param name="oldState">The previous state of the world tracking systems.</param>
        /// <param name="newState">The new state of the world tracking systems.</param>
        public delegate void OnPositionalLocatorStateChangedDelegate(PositionalLocatorState oldState, PositionalLocatorState newState);
    }
}

