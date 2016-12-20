namespace UnityEditor.HolographicEmulation
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class PerceptionSimulation
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Initialize();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void LoadRoom(string id);
        internal static void SetEmulationMode(EmulationMode mode)
        {
            SetEmulationMode_Internal(mode);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetEmulationMode_Internal(EmulationMode mode);
        internal static void SetGestureHand(GestureHand hand)
        {
            SetGestureHand_Internal(hand);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetGestureHand_Internal(GestureHand hand);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Shutdown();
    }
}

