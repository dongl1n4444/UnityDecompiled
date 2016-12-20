namespace UnityEditor.Advertisements
{
    using System;
    using System.IO;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    internal static class UnityAdsEditor
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EditorOnLoad()
        {
            bool flag = false;
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.iOS:
                case BuildTarget.Android:
                    flag = true;
                    break;
            }
            if (AdvertisementSettings.enabled)
            {
                System.Type type = System.Type.GetType("UnityEngine.Advertisements.Advertisement, UnityEngine.Advertisements");
                if (type != null)
                {
                    object[] parameters = new object[] { Path.GetDirectoryName(Path.GetDirectoryName(typeof(UnityAdsEditor).Assembly.Location)), flag };
                    type.GetMethod("LoadEditor", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, parameters);
                }
            }
        }
    }
}

