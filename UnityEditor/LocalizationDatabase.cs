namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class LocalizationDatabase
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern SystemLanguage[] GetAvailableEditorLanguages();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern SystemLanguage GetCurrentEditorLanguage();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern SystemLanguage GetDefaultEditorLanguage();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern string GetLocalizedString(string original);
        public static string MarkForTranslation(string value) => 
            value;

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void ReadEditorLocalizationResources();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetCurrentEditorLanguage(SystemLanguage lang);
    }
}

