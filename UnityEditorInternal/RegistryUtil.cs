namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    public sealed class RegistryUtil
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetRegistryStringValue(string subKey, string valueName, string defaultValue, RegistryView view);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern uint GetRegistryUInt32Value(string subKey, string valueName, uint defaultValue, RegistryView view);
    }
}

