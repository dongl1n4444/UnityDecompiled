namespace UnityEditor.HolographicEmulation
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    internal sealed class HolographicEmulation
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void Initialize();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void LoadRoom(string id);
        internal static void SetEmulationMode(EmulationMode mode)
        {
            SetEmulationMode_Internal(mode);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetEmulationMode_Internal(EmulationMode mode);
        internal static void SetGestureHand(GestureHand hand)
        {
            SetGestureHand_Internal(hand);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetGestureHand_Internal(GestureHand hand);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void Shutdown();
    }
}

