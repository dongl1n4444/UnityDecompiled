namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    public sealed class NativeProfilerTimeline
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Draw(ref NativeProfilerTimeline_DrawArgs args);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetEntryAtPosition(ref NativeProfilerTimeline_GetEntryAtPositionArgs args);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetEntryInstanceInfo(ref NativeProfilerTimeline_GetEntryInstanceInfoArgs args);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetEntryTimingInfo(ref NativeProfilerTimeline_GetEntryTimingInfoArgs args);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Initialize(ref NativeProfilerTimeline_InitializeArgs args);
    }
}

