namespace UnityEditor.VisualStudioIntegration
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal class UnityVSSupport
    {
        [CompilerGenerated]
        private static Func<Assembly, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Assembly, bool> <>f__am$cache1;
        private static bool m_ShouldUnityVSBeActive;
        private static string s_AboutLabel;
        private static bool? s_IsUnityVSEnabled;
        public static string s_UnityVSBridgeToLoad;

        private static string CalculateAboutWindowLabel()
        {
            if (!IsUnityVSEnabled())
            {
                return "";
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<Assembly, bool>(null, (IntPtr) <CalculateAboutWindowLabel>m__1);
            }
            Assembly assembly = Enumerable.FirstOrDefault<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), <>f__am$cache1);
            if (assembly == null)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder("Microsoft Visual Studio Tools for Unity ");
            builder.Append(assembly.GetName().Version);
            builder.Append(" enabled");
            return builder.ToString();
        }

        public static string GetAboutWindowLabel()
        {
            if (s_AboutLabel == null)
            {
                s_AboutLabel = CalculateAboutWindowLabel();
            }
            return s_AboutLabel;
        }

        private static string GetAssemblyLocation(Assembly a)
        {
            try
            {
                return a.Location;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        private static string GetVstuBridgeAssembly(VisualStudioVersion version)
        {
            try
            {
                string vsVersion = string.Empty;
                switch (version)
                {
                    case VisualStudioVersion.VisualStudio2010:
                        vsVersion = "2010";
                        break;

                    case VisualStudioVersion.VisualStudio2012:
                        vsVersion = "2012";
                        break;

                    case VisualStudioVersion.VisualStudio2013:
                        vsVersion = "2013";
                        break;

                    case VisualStudioVersion.VisualStudio2015:
                        vsVersion = "2015";
                        break;

                    case VisualStudioVersion.VisualStudio15:
                        vsVersion = "15.0";
                        break;
                }
                string vstuBridgePathFromRegistry = GetVstuBridgePathFromRegistry(vsVersion, true);
                if (vstuBridgePathFromRegistry != null)
                {
                    return vstuBridgePathFromRegistry;
                }
                return GetVstuBridgePathFromRegistry(vsVersion, false);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetVstuBridgePathFromRegistry(string vsVersion, bool currentUser) => 
            ((string) Registry.GetValue($"{!currentUser ? "HKEY_LOCAL_MACHINE" : "HKEY_CURRENT_USER"}\Software\Microsoft\Microsoft Visual Studio {vsVersion} Tools for Unity", "UnityExtensionPath", null));

        public static void Initialize()
        {
            Initialize(null);
        }

        public static void Initialize(string editorPath)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                string str;
                VisualStudioVersion version;
                if (editorPath != null)
                {
                    str = editorPath;
                }
                else
                {
                    str = EditorPrefs.GetString("kScriptsDefaultApp");
                }
                if (str.EndsWith("UnityVS.OpenFile.exe"))
                {
                    str = SyncVS.FindBestVisualStudio();
                    if (str != null)
                    {
                        EditorPrefs.SetString("kScriptsDefaultApp", str);
                    }
                }
                if (IsVisualStudio(str, out version))
                {
                    m_ShouldUnityVSBeActive = true;
                    string vstuBridgeAssembly = GetVstuBridgeAssembly(version);
                    if (vstuBridgeAssembly == null)
                    {
                        Console.WriteLine("Unable to find bridge dll in registry for Microsoft Visual Studio Tools for Unity for " + str);
                    }
                    else if (!File.Exists(vstuBridgeAssembly))
                    {
                        Console.WriteLine("Unable to find bridge dll on disk for Microsoft Visual Studio Tools for Unity for " + vstuBridgeAssembly);
                    }
                    else
                    {
                        s_UnityVSBridgeToLoad = vstuBridgeAssembly;
                        InternalEditorUtility.SetupCustomDll(Path.GetFileNameWithoutExtension(vstuBridgeAssembly), vstuBridgeAssembly);
                    }
                }
            }
        }

        public static bool IsUnityVSEnabled()
        {
            if (!s_IsUnityVSEnabled.HasValue)
            {
                if (m_ShouldUnityVSBeActive)
                {
                }
                s_IsUnityVSEnabled = new bool?((<>f__am$cache0 == null) && Enumerable.Any<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), <>f__am$cache0));
            }
            return s_IsUnityVSEnabled.Value;
        }

        private static bool IsVisualStudio(string externalEditor, out VisualStudioVersion vsVersion)
        {
            <IsVisualStudio>c__AnonStorey0 storey = new <IsVisualStudio>c__AnonStorey0 {
                externalEditor = externalEditor
            };
            if (string.IsNullOrEmpty(storey.externalEditor))
            {
                vsVersion = VisualStudioVersion.Invalid;
                return false;
            }
            KeyValuePair<VisualStudioVersion, string>[] pairArray = Enumerable.Where<KeyValuePair<VisualStudioVersion, string>>(SyncVS.InstalledVisualStudios, new Func<KeyValuePair<VisualStudioVersion, string>, bool>(storey, (IntPtr) this.<>m__0)).ToArray<KeyValuePair<VisualStudioVersion, string>>();
            if (pairArray.Length > 0)
            {
                vsVersion = pairArray[0].Key;
                return true;
            }
            if (storey.externalEditor.EndsWith("devenv.exe", StringComparison.OrdinalIgnoreCase) && TryGetVisualStudioVersion(storey.externalEditor, out vsVersion))
            {
                return true;
            }
            vsVersion = VisualStudioVersion.Invalid;
            return false;
        }

        private static Version ProductVersion(string externalEditor)
        {
            try
            {
                return new Version(FileVersionInfo.GetVersionInfo(externalEditor).ProductVersion);
            }
            catch (Exception)
            {
                return new Version(0, 0);
            }
        }

        public static void ScriptEditorChanged(string editorPath)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                Initialize(editorPath);
                InternalEditorUtility.RequestScriptReload();
            }
        }

        public static bool ShouldUnityVSBeActive() => 
            m_ShouldUnityVSBeActive;

        private static bool TryGetVisualStudioVersion(string externalEditor, out VisualStudioVersion vsVersion)
        {
            switch (ProductVersion(externalEditor).Major)
            {
                case 9:
                    vsVersion = VisualStudioVersion.VisualStudio2008;
                    return true;

                case 10:
                    vsVersion = VisualStudioVersion.VisualStudio2010;
                    return true;

                case 11:
                    vsVersion = VisualStudioVersion.VisualStudio2012;
                    return true;

                case 12:
                    vsVersion = VisualStudioVersion.VisualStudio2013;
                    return true;

                case 14:
                    vsVersion = VisualStudioVersion.VisualStudio2015;
                    return true;

                case 15:
                    vsVersion = VisualStudioVersion.VisualStudio15;
                    return true;
            }
            vsVersion = VisualStudioVersion.Invalid;
            return false;
        }

        [CompilerGenerated]
        private sealed class <IsVisualStudio>c__AnonStorey0
        {
            internal string externalEditor;

            internal bool <>m__0(KeyValuePair<VisualStudioVersion, string> kvp) => 
                Paths.AreEqual(kvp.Value, this.externalEditor, true);
        }
    }
}

