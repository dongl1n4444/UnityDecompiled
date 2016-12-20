namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class EditorUserSettings : Object
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetConfigValue(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetConfigValue(string name, string value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetPrivateConfigValue(string name, string value);

        public static bool AutomaticAdd { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static bool DebugCmd { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static bool DebugCom { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static bool DebugOut { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static SemanticMergeMode semanticMergeMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static bool showFailedCheckout { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static bool WorkOffline { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

