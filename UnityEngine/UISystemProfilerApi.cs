namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    public sealed class UISystemProfilerApi
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void AddMarker(string name, UnityEngine.Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void BeginSample(SampleType type);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void EndSample(SampleType type);

        public enum SampleType
        {
            Layout,
            Render
        }
    }
}

