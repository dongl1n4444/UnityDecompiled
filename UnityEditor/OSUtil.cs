namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    internal sealed class OSUtil
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetAppFriendlyName(string app);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetDefaultAppPath(string fileType);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string[] GetDefaultApps(string fileType);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetDefaultCachePath();
    }
}

