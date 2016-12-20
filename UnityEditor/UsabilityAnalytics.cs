namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class UsabilityAnalytics
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Event(string category, string action, string label, int value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Track(string page);
    }
}

