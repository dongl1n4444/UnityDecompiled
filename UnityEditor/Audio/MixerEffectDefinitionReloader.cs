namespace UnityEditor.Audio
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;

    [InitializeOnLoad]
    internal static class MixerEffectDefinitionReloader
    {
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache0;

        static MixerEffectDefinitionReloader()
        {
            MixerEffectDefinitions.Refresh();
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new EditorApplication.CallbackFunction(MixerEffectDefinitionReloader.OnProjectChanged);
            }
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.projectWindowChanged, <>f__mg$cache0);
        }

        private static void OnProjectChanged()
        {
            MixerEffectDefinitions.Refresh();
        }
    }
}

