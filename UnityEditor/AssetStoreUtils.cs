namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    internal sealed class AssetStoreUtils
    {
        private const string kAssetStoreUrl = "https://shawarma.unity3d.com";

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string CheckDownload(string id, string url, string[] destination, string key);
        [ExcludeFromDocs]
        public static void Download(string id, string url, string[] destination, string key, string jsonData, bool resumeOK)
        {
            DownloadDoneCallback doneCallback = null;
            Download(id, url, destination, key, jsonData, resumeOK, doneCallback);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Download(string id, string url, string[] destination, string key, string jsonData, bool resumeOK, [DefaultValue("null")] DownloadDoneCallback doneCallback);
        public static string GetAssetStoreSearchUrl() => 
            GetAssetStoreUrl().Replace("https", "http");

        public static string GetAssetStoreUrl() => 
            "https://shawarma.unity3d.com";

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetLoaderPath();
        public static string GetOfflinePath() => 
            Uri.EscapeUriString(EditorApplication.applicationContentsPath + "/Resources/offline.html");

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void RegisterDownloadDelegate(ScriptableObject d);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void UnRegisterDownloadDelegate(ScriptableObject d);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void UpdatePreloading();

        public delegate void DownloadDoneCallback(string package_id, string message, int bytes, int total);
    }
}

