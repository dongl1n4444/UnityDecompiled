namespace Unity.PackageManager
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Unity.DataContract;
    using UnityEditor;

    public static class Settings
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static string <downloadLocation>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static string <editorInstallPath>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static PackageVersion <unityVersion>k__BackingField;
        private static Uri defaultRepoUri = null;
        internal const string kExternal = "http://update.unity3d.com/";
        internal const string kInternal = "http://pm-test.hq.unity3d.com/repo/split/";
        private static string s_CachedCustomRepoUrl;
        private static string s_CachedRepoType;
        private static UpdateMode s_CachedUpdateMode;
        private static string s_InstallLocation = null;

        static Settings()
        {
            downloadLocation = Path.Combine(Path.GetTempPath(), "unity");
        }

        public static void CacheAllSettings()
        {
            s_CachedCustomRepoUrl = EditorPrefs.GetString("pmCustomRepositoryUrl");
            s_CachedUpdateMode = (UpdateMode) EditorPrefs.GetInt("pmUpdateMode");
            s_CachedRepoType = EditorPrefs.GetString("pmRepositoryType");
        }

        private static string MapRepoTypeToUrl(string repoType) => 
            MapRepoTypeToUrl(repoType, null);

        private static string MapRepoTypeToUrl(string repoType, string repoUrl)
        {
            if (repoType != null)
            {
                if (repoType == "internal")
                {
                    return "http://pm-test.hq.unity3d.com/repo/split/";
                }
                if (repoType == "external")
                {
                    return "http://update.unity3d.com/";
                }
                if (repoType == "custom")
                {
                    return repoUrl;
                }
            }
            return "http://update.unity3d.com/";
        }

        private static string MapUrlToRepoType(string url)
        {
            if (url == "http://pm-test.hq.unity3d.com/repo/split/")
            {
                return "internal";
            }
            if (url == "http://update.unity3d.com/")
            {
                return "external";
            }
            return "custom";
        }

        public static void ResetAllSettings()
        {
            EditorPrefs.SetString("pmCustomRepositoryUrl", null);
            updateMode = UpdateMode.Automatic;
            repoType = null;
            installLocation = null;
        }

        public static string SelectRepo(string repoType, string repoUrl)
        {
            repoUrl = MapRepoTypeToUrl(repoType, repoUrl);
            baseRepoUrl = repoUrl;
            return repoUrl;
        }

        public static string baseRepoUrl
        {
            get
            {
                if (repoType != "custom")
                {
                    return MapRepoTypeToUrl(repoType);
                }
                string str2 = s_CachedCustomRepoUrl;
                if (string.IsNullOrEmpty(str2))
                {
                    return null;
                }
                if (!str2.EndsWith("/"))
                {
                    str2 = str2 + "/";
                }
                return str2;
            }
            private set
            {
                string str = MapUrlToRepoType(value);
                repoType = str;
                if (str != "custom")
                {
                    value = null;
                }
                else if (!ThreadUtils.InMainThread)
                {
                    return;
                }
                if (!string.IsNullOrEmpty(value) && !value.EndsWith("/"))
                {
                    value = value + "/";
                }
                EditorPrefs.SetString("pmCustomRepositoryUrl", value);
                s_CachedCustomRepoUrl = value;
            }
        }

        public static string downloadLocation
        {
            [CompilerGenerated]
            get => 
                <downloadLocation>k__BackingField;
            [CompilerGenerated]
            set
            {
                <downloadLocation>k__BackingField = value;
            }
        }

        public static string editorInstallPath
        {
            [CompilerGenerated]
            get => 
                <editorInstallPath>k__BackingField;
            [CompilerGenerated]
            set
            {
                <editorInstallPath>k__BackingField = value;
            }
        }

        public static string installLocation
        {
            get
            {
                if (s_InstallLocation == null)
                {
                    if (unityVersionPath != null)
                    {
                        s_InstallLocation = Path.Combine(Locator.installLocation, unityVersionPath);
                    }
                    else
                    {
                        s_InstallLocation = Locator.installLocation;
                    }
                    if (!Directory.Exists(s_InstallLocation))
                    {
                        Directory.CreateDirectory(s_InstallLocation);
                    }
                }
                return s_InstallLocation;
            }
            set
            {
                Locator.installLocation = value;
                s_InstallLocation = value;
                if ((s_InstallLocation != null) && !Directory.Exists(s_InstallLocation))
                {
                    Directory.CreateDirectory(s_InstallLocation);
                }
            }
        }

        public static bool inTestMode =>
            (Environment.GetEnvironmentVariable("UNITY_PACKAGEMANAGERTESTMODE") == "1");

        public static string repoType
        {
            get => 
                s_CachedRepoType;
            private set
            {
                if (ThreadUtils.InMainThread)
                {
                    Unity.PackageManager.PackageManager.Instance.RemoteIndexer.FlushCache();
                    EditorPrefs.SetString("pmRepositoryType", value);
                    s_CachedRepoType = value;
                }
            }
        }

        public static Uri RepoUrl
        {
            get
            {
                Uri uri2;
                if (defaultRepoUri == null)
                {
                    defaultRepoUri = new Uri("http://update.unity3d.com/");
                }
                if (baseRepoUrl == null)
                {
                    return defaultRepoUri;
                }
                if (unityVersionPath == null)
                {
                    if (!Uri.TryCreate(baseRepoUrl, UriKind.Absolute, out uri2))
                    {
                        return defaultRepoUri;
                    }
                    return uri2;
                }
                if (!Uri.TryCreate(baseRepoUrl + unityVersionPath + "/", UriKind.Absolute, out uri2))
                {
                    return defaultRepoUri;
                }
                return uri2;
            }
        }

        public static bool teamcity =>
            (Environment.GetEnvironmentVariable("UNITY_THISISABUILDMACHINE") == "1");

        public static PackageVersion unityVersion
        {
            [CompilerGenerated]
            get => 
                <unityVersion>k__BackingField;
            [CompilerGenerated]
            set
            {
                <unityVersion>k__BackingField = value;
            }
        }

        public static string unityVersionPath
        {
            get
            {
                if (unityVersion == null)
                {
                    return null;
                }
                return (unityVersion.major + "." + unityVersion.minor);
            }
        }

        public static UpdateMode updateMode
        {
            get => 
                s_CachedUpdateMode;
            set
            {
                if (ThreadUtils.InMainThread)
                {
                    EditorPrefs.SetInt("pmUpdateMode", (int) value);
                    s_CachedUpdateMode = value;
                }
            }
        }
    }
}

