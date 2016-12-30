namespace UnityEditor.Facebook
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Xml.Linq;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal class SDKManager
    {
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Action<ProcessStartInfo> <>f__mg$cache0;
        private static string[] availableSDKs = null;
        private static readonly XNamespace extraNs = "http://ant.apache.org/ivy/extra";
        public static readonly string fbUnityPluginName = "Facebook.Unity";
        public static readonly string fbUnitySettingsName = "Facebook.Unity.Settings";
        public static readonly string sdkAsPlugin = "plugin";
        public static readonly string sdkAsPluginIncompatible = "pluginIncompatible";
        private static string[] supportedSDKs = null;
        private static string unityVersion = Application.unityVersion;

        public static bool CheckSDKToUse()
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = p => p == fbUnityPluginName;
            }
            string str = Enumerable.FirstOrDefault<string>(GetCurrentPlugins(), <>f__am$cache2);
            bool flag = false;
            if (!string.IsNullOrEmpty(str))
            {
                <CheckSDKToUse>c__AnonStorey0 storey = new <CheckSDKToUse>c__AnonStorey0();
                flag = true;
                System.Type type = Assembly.LoadFrom(GetFacebookUnityAssemblyPath(fbUnityPluginName)).GetType("Facebook.Unity.FacebookSdkVersion");
                PropertyInfo property = type.GetProperty("Build");
                storey.build = (string) property.GetValue(type, null);
                if (Enumerable.Any<string>(availableSDKs, new Func<string, bool>(storey.<>m__0)))
                {
                    PlayerSettings.Facebook.sdkVersion = sdkAsPlugin;
                    return flag;
                }
                PlayerSettings.Facebook.sdkVersion = sdkAsPluginIncompatible;
                return flag;
            }
            if ((PlayerSettings.Facebook.sdkVersion == sdkAsPlugin) || (PlayerSettings.Facebook.sdkVersion == sdkAsPluginIncompatible))
            {
                PlayerSettings.Facebook.sdkVersion = BuiltinDefaultSDKVersion;
            }
            return flag;
        }

        private static string[] GetCurrentPlugins()
        {
            BuildTarget selectedFacebookTarget = EditorUserBuildSettings.selectedFacebookTarget;
            PluginImporter[] importers = PluginImporter.GetImporters(BuildTargetGroup.Facebook, selectedFacebookTarget);
            List<string> list = new List<string>();
            foreach (PluginImporter importer in importers)
            {
                string assetPath = importer.assetPath;
                if (assetPath.StartsWith("Assets"))
                {
                    list.Add(Path.GetFileNameWithoutExtension(assetPath));
                }
            }
            return list.ToArray();
        }

        internal static string GetFacebookUnityAssemblyPath(string dllName)
        {
            string sdkVersion = PlayerSettings.Facebook.sdkVersion;
            if ((sdkVersion != sdkAsPlugin) && (sdkVersion != sdkAsPluginIncompatible))
            {
                return Path.Combine(ProjectSDKPath, dllName + ".dll");
            }
            PluginImporter[] importers = PluginImporter.GetImporters(BuildTargetGroup.Facebook, EditorUserBuildSettings.selectedFacebookTarget);
            foreach (PluginImporter importer in importers)
            {
                if (importer.assetPath.EndsWith(dllName + ".dll"))
                {
                    return importer.assetPath;
                }
            }
            return null;
        }

        private static void GetLocallyAvailableSDKs()
        {
            GetSupportedSDKs();
            List<string> list = new List<string>();
            foreach (string str in supportedSDKs)
            {
                if (GetSDKPath(str) != null)
                {
                    list.Add(str);
                }
            }
            availableSDKs = list.ToArray();
        }

        public static string GetSDKPath(string sdk)
        {
            string path = Path.Combine(BuiltinSDKLocation, sdk);
            if (Directory.Exists(path))
            {
                return path;
            }
            string str3 = Path.Combine(SDKLocation, sdk);
            if (Directory.Exists(str3))
            {
                return str3;
            }
            return null;
        }

        private static void GetSupportedSDKs()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = path => Path.GetFileName(path);
            }
            supportedSDKs = Enumerable.Select<string, string>(Directory.GetDirectories(BuiltinSDKLocation), <>f__am$cache0).ToArray<string>();
            if (File.Exists(SDKSupportedListLocation))
            {
                string data = File.ReadAllText(SDKSupportedListLocation);
                supportedSDKs = supportedSDKs.Concat<string>(ParseSupportedSDKList(data)).Distinct<string>().ToArray<string>();
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = x => x;
            }
            supportedSDKs = Enumerable.OrderByDescending<string, string>(supportedSDKs, <>f__am$cache1).ToArray<string>();
        }

        public static void Initialize()
        {
            <Initialize>c__AnonStorey1 storey = new <Initialize>c__AnonStorey1();
            GetLocallyAvailableSDKs();
            string[] components = new string[] { PlayerPackage, "SDKUpdater.exe" };
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Action<ProcessStartInfo>(SDKManager.SetupStartInfo);
            }
            storey.p = Utilities.CreateManagedProgram(Paths.Combine(components), "", <>f__mg$cache0);
            storey.p.Start(new EventHandler(storey.<>m__0));
            storey.p.LogProcessStartInfo();
            string sdkVersion = PlayerSettings.Facebook.sdkVersion;
            if (GetSDKPath(sdkVersion) == null)
            {
                Console.WriteLine("Waiting for facebook SDK update, since required SDK version " + sdkVersion + " is not available.");
                storey.p.WaitForExit();
                GetLocallyAvailableSDKs();
            }
        }

        public static bool IsPreReleaseVersion(string version) => 
            (version.Contains("b") || version.Contains("a"));

        public static bool IsSDKOverriden() => 
            PlayerSettings.Facebook.sdkVersion.StartsWith(sdkAsPlugin);

        public static bool IsSDKOverridenIncompatibleVersion() => 
            (PlayerSettings.Facebook.sdkVersion == sdkAsPluginIncompatible);

        private static string[] ParseSupportedSDKList(string data) => 
            JsonUtility.FromJson<AvailableSDKVersions>(data).versions;

        public static void RegisterAdditionalUnityExtensions()
        {
            UpdateProjectSDKIfNeeded();
            GetLocallyAvailableSDKs();
            bool flag = CheckSDKToUse();
            XElement element = XDocument.Load(Path.Combine(ProjectSDKPath, "ivy.xml")).Element("ivy-module").Element("publications");
            foreach (XElement element2 in element.Elements())
            {
                string str = element2.Attribute("name").Value;
                string str2 = element2.Attribute("ext").Value;
                string guid = element2.Attribute((XName) (extraNs + "guid")).Value;
                string dllLocation = Path.Combine(ProjectSDKPath, $"{str}.{str2}");
                InternalEditorUtility.RegisterExtensionDll(dllLocation, guid);
                AssetDatabase.ImportAsset(dllLocation, ImportAssetOptions.ForceUpdate);
                PluginImporter atPath = AssetImporter.GetAtPath(dllLocation) as PluginImporter;
                if (atPath != null)
                {
                    SetupImporterForDll(atPath, !flag);
                }
            }
        }

        public static void SetupImporterForDll(PluginImporter importer, bool enabled)
        {
            if (enabled)
            {
                importer.SetCompatibleWithAnyPlatform(false);
                importer.SetCompatibleWithEditor(true);
                importer.SetCompatibleWithEditor(BuildTargetGroup.Facebook, BuildTarget.StandaloneWindows, true);
                importer.SetCompatibleWithEditor(BuildTargetGroup.Facebook, BuildTarget.StandaloneWindows64, true);
                importer.SetCompatibleWithEditor(BuildTargetGroup.Facebook, BuildTarget.WebGL, true);
                importer.SetCompatibleWithPlatform(BuildTargetGroup.Facebook, BuildTarget.StandaloneWindows, true);
                importer.SetCompatibleWithPlatform(BuildTargetGroup.Facebook, BuildTarget.StandaloneWindows64, true);
                importer.SetCompatibleWithPlatform(BuildTargetGroup.Facebook, BuildTarget.WebGL, true);
            }
            else
            {
                importer.SetCompatibleWithEditor(false);
                importer.SetCompatibleWithAnyPlatform(false);
                importer.SetCompatibleWithEditor(BuildTargetGroup.Facebook, BuildTarget.StandaloneWindows, false);
                importer.SetCompatibleWithEditor(BuildTargetGroup.Facebook, BuildTarget.StandaloneWindows64, false);
                importer.SetCompatibleWithEditor(BuildTargetGroup.Facebook, BuildTarget.WebGL, false);
                importer.SetCompatibleWithPlatform(BuildTargetGroup.Facebook, BuildTarget.StandaloneWindows, false);
                importer.SetCompatibleWithPlatform(BuildTargetGroup.Facebook, BuildTarget.StandaloneWindows64, false);
                importer.SetCompatibleWithPlatform(BuildTargetGroup.Facebook, BuildTarget.WebGL, false);
            }
            importer.SaveAndReimport();
        }

        public static void SetupStartInfo(ProcessStartInfo startInfo)
        {
            string str = "7za";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                str = "7z.exe";
            }
            string[] components = new string[] { EditorApplication.applicationContentsPath, "Tools", str };
            startInfo.EnvironmentVariables["UNITY_7Z_PATH"] = Paths.Combine(components);
            startInfo.EnvironmentVariables["UNITY_VERSION"] = Application.unityVersion;
            startInfo.EnvironmentVariables["UNITY_FACEBOOK_PACKAGE"] = PlayerPackage;
        }

        public static string ShouldShowUpdateMessage()
        {
            string sdkVersion = PlayerSettings.Facebook.sdkVersion;
            if (IsPreReleaseVersion(sdkVersion))
            {
                return null;
            }
            try
            {
                Version version = new Version(sdkVersion);
                foreach (string str3 in AvailableSDKs)
                {
                    if (!IsPreReleaseVersion(str3))
                    {
                        Version version2 = new Version(str3);
                        if (version2.CompareTo(version) > 0)
                        {
                            return str3;
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void UpdateProjectSDKIfNeeded()
        {
            string sdkVersion = PlayerSettings.Facebook.sdkVersion;
            if ((sdkVersion != sdkAsPlugin) && (sdkVersion != sdkAsPluginIncompatible))
            {
                string path = Path.Combine(ProjectSDKPath, "version.txt");
                string str3 = File.ReadAllText(path);
                if ((str3 != sdkVersion) && (GetSDKPath(sdkVersion) != null))
                {
                    Console.WriteLine("Switching Facebook SDK from " + str3 + " to " + sdkVersion);
                    Directory.Delete(ProjectSDKPath, true);
                    FileUtil.CopyDirectoryRecursive(GetSDKPath(sdkVersion), ProjectSDKPath);
                    File.WriteAllText(path, sdkVersion);
                }
            }
        }

        public static string[] AvailableSDKs
        {
            get
            {
                if (availableSDKs == null)
                {
                    GetLocallyAvailableSDKs();
                }
                return availableSDKs;
            }
        }

        public static string BuiltinDefaultSDKVersion =>
            Path.GetFileName(Directory.GetDirectories(BuiltinSDKLocation)[0]);

        private static string BuiltinSDKLocation
        {
            get
            {
                string[] components = new string[] { PlayerPackage, "SDK" };
                return Paths.Combine(components);
            }
        }

        private static string FacebookDataLocation
        {
            get
            {
                string environmentVariable = Environment.GetEnvironmentVariable("UNITY_FACEBOOK_SDK_PATH");
                if (!string.IsNullOrEmpty(environmentVariable))
                {
                    return environmentVariable;
                }
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    string str3 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    string[] textArray1 = new string[] { str3, "Unity", "Facebook" };
                    return Paths.Combine(textArray1);
                }
                if (Application.platform != RuntimePlatform.OSXEditor)
                {
                    throw new NotImplementedException("FacebookDataLocation not implemented for this platform.");
                }
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string[] components = new string[] { folderPath, "Library", "Unity", "Facebook" };
                return Paths.Combine(components);
            }
        }

        private static string PlayerPackage =>
            BuildPipeline.GetPlaybackEngineExtensionDirectory(BuildTargetGroup.Facebook, BuildTarget.StandaloneWindows, BuildOptions.CompressTextures);

        internal static string ProjectSDKPath =>
            Path.Combine("Library", "FacebookSDK");

        private static string SDKLocation
        {
            get
            {
                string[] components = new string[] { FacebookDataLocation, "SDKs" };
                return Paths.Combine(components);
            }
        }

        private static string SDKSupportedListLocation
        {
            get
            {
                string[] components = new string[] { FacebookDataLocation, "SupportedSDKVersions", unityVersion };
                return Paths.Combine(components);
            }
        }

        [CompilerGenerated]
        private sealed class <CheckSDKToUse>c__AnonStorey0
        {
            internal string build;

            internal bool <>m__0(string s) => 
                (s == this.build);
        }

        [CompilerGenerated]
        private sealed class <Initialize>c__AnonStorey1
        {
            internal Program p;

            internal void <>m__0(object sender, EventArgs e)
            {
                Console.WriteLine("SDKUpdater. Exit code:    {0}. Output:    {1}", this.p.ExitCode, this.p.GetAllOutput());
                if (this.p.ExitCode != 0)
                {
                    UnityEngine.Debug.LogError("SDKUpdater failed: " + this.p.GetAllOutput());
                }
            }
        }
    }
}

