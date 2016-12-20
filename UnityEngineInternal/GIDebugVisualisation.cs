namespace UnityEngineInternal
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class GIDebugVisualisation
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CycleSkipInstances(int skip);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CycleSkipSystems(int skip);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void PauseCycleMode();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void PlayCycleMode();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ResetRuntimeInputTextures();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void StopCycleMode();

        public static bool cycleMode { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static bool pauseCycleMode { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static GITextureType texType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

