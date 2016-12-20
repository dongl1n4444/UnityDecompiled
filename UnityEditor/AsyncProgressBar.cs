namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class AsyncProgressBar
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Clear();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Display(string progressInfo, float progress);

        public static bool isShowing { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static float progress { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static string progressInfo { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

