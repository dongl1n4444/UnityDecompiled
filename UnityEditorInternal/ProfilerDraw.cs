namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class ProfilerDraw
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void DrawNative(ref ProfilingDataDrawNativeInfo d);
    }
}

