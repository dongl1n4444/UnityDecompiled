namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    internal static class VisualStudioUtil
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool CanVS2017BuildCppCode();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string[] FindVisualStudioDevEnvPaths(int visualStudioVersion, string requiredWorkload);
    }
}

