namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class RenderDoc
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsInstalled();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsLoaded();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsSupported();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Load();
    }
}

