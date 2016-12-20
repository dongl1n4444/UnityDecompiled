namespace UnityEditor.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [InitializeOnLoad]
    internal class PrefabLayoutRebuilder
    {
        [CompilerGenerated]
        private static PrefabUtility.PrefabInstanceUpdated <>f__mg$cache0;

        static PrefabLayoutRebuilder()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new PrefabUtility.PrefabInstanceUpdated(PrefabLayoutRebuilder.OnPrefabInstanceUpdates);
            }
            PrefabUtility.prefabInstanceUpdated = (PrefabUtility.PrefabInstanceUpdated) Delegate.Combine(PrefabUtility.prefabInstanceUpdated, <>f__mg$cache0);
        }

        private static void OnPrefabInstanceUpdates(GameObject instance)
        {
            if (instance != null)
            {
                RectTransform rect = instance.transform as RectTransform;
                if (rect != null)
                {
                    LayoutRebuilder.MarkLayoutForRebuild(rect);
                }
            }
        }
    }
}

