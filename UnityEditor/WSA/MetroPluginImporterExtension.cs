namespace UnityEditor.WSA
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditorInternal;
    using UnityEngine;

    internal class MetroPluginImporterExtension : DefaultPluginImporterExtension
    {
        private static bool _isCppFile;
        private static bool _isNative;
        private static string _originalPluginPath;
        internal static PlaceholderProperty _placeholderProperty;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static PlaceholderProperty.GetOrignalPluginPath <>f__mg$cache0;
        private static readonly MetroPluginCPUArchitecture[] nativeArchs = new MetroPluginCPUArchitecture[] { MetroPluginCPUArchitecture.X86 };
        private readonly GUIContent[] nativeArchsGUI;

        static MetroPluginImporterExtension()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new PlaceholderProperty.GetOrignalPluginPath(MetroPluginImporterExtension.GetOriginalPluginPath);
            }
            _placeholderProperty = new PlaceholderProperty(EditorGUIUtility.TextContent("Placeholder|Stub assembly used instead of plugin during script compilation. Only needed if plugin references platform specific types."), Plugin.placeHolderPathTag, BuildPipeline.GetBuildTargetName(BuildTarget.WSAPlayer), <>f__mg$cache0);
        }

        internal MetroPluginImporterExtension() : base(GetProperties())
        {
            this.nativeArchsGUI = new GUIContent[nativeArchs.Length];
            for (int i = 0; i < nativeArchs.Length; i++)
            {
                this.nativeArchsGUI[i] = EditorGUIUtility.TextContent(nativeArchs[i].ToString());
            }
        }

        public override string CalculateFinalPluginPath(string platformName, PluginImporter imp) => 
            CalculateFinalPluginPath(platformName, imp, EditorUserBuildSettings.wsaSDK);

        internal static string CalculateFinalPluginPath(string platformName, PluginImporter imp, WSASDK wsaSDK)
        {
            bool flag5;
            string platformData = imp.GetPlatformData(platformName, Plugin.sdkTag);
            string str2 = imp.GetPlatformData(platformName, Plugin.cpuTag);
            string str3 = imp.GetPlatformData(platformName, Plugin.scriptingBackendTag);
            bool flag = string.IsNullOrEmpty(platformData) || string.Equals(platformData, "AnySDK", StringComparison.InvariantCultureIgnoreCase);
            bool flag2 = string.IsNullOrEmpty(str2) || string.Equals(str2, "AnyCPU", StringComparison.InvariantCultureIgnoreCase);
            if (!string.IsNullOrEmpty(str3) && !string.Equals(str3, "AnyScriptingBackend", StringComparison.InvariantCultureIgnoreCase))
            {
                ScriptingImplementation scriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA);
                bool flag4 = false;
                switch (scriptingBackend)
                {
                    case ScriptingImplementation.WinRTDotNET:
                        flag4 = string.Equals(str3, "DotNet", StringComparison.InvariantCultureIgnoreCase);
                        break;

                    case ScriptingImplementation.IL2CPP:
                        flag4 = string.Equals(str3, "Il2Cpp", StringComparison.InvariantCultureIgnoreCase);
                        break;
                }
                if (!flag4)
                {
                    return null;
                }
            }
            if (flag)
            {
                flag5 = false;
            }
            else
            {
                switch (wsaSDK)
                {
                    case WSASDK.SDK81:
                        if (string.Equals(platformData, "SDK81", StringComparison.InvariantCultureIgnoreCase))
                        {
                            flag5 = false;
                            break;
                        }
                        return null;

                    case WSASDK.PhoneSDK81:
                        if (string.Equals(platformData, "PhoneSDK81", StringComparison.InvariantCultureIgnoreCase))
                        {
                            flag5 = false;
                            break;
                        }
                        return null;

                    case WSASDK.UniversalSDK81:
                        if (string.Equals(platformData, "SDK81", StringComparison.InvariantCultureIgnoreCase) || string.Equals(platformData, "PhoneSDK81", StringComparison.InvariantCultureIgnoreCase))
                        {
                            flag5 = true;
                            break;
                        }
                        return null;

                    case WSASDK.UWP:
                        if (string.Equals(platformData, "UWP", StringComparison.InvariantCultureIgnoreCase))
                        {
                            flag5 = false;
                            break;
                        }
                        return null;

                    default:
                        throw new Exception($"Unknown WSASDK: {wsaSDK}");
                }
            }
            string str5 = (flag5 || !flag2) ? "Plugins" : string.Empty;
            if (flag5)
            {
                str5 = Path.Combine(str5, platformData);
            }
            if (!flag2)
            {
                str5 = Path.Combine(str5, str2);
            }
            string fileName = Path.GetFileName(imp.assetPath);
            return Path.Combine(str5, fileName);
        }

        private static string GetOriginalPluginPath() => 
            _originalPluginPath;

        private static DefaultPluginImporterExtension.Property[] GetProperties()
        {
            string buildTargetName = BuildPipeline.GetBuildTargetName(BuildTarget.WSAPlayer);
            return new DefaultPluginImporterExtension.Property[] { new DefaultPluginImporterExtension.Property(EditorGUIUtility.TextContent("SDK|Is plugin compatible with Windows 8.0 or Windows 8.1 SDK?"), Plugin.sdkTag, MetroPluginSDK.AnySDK, buildTargetName), new DefaultPluginImporterExtension.Property(EditorGUIUtility.TextContent("CPU|Is plugin compatible with x86, x64 or ARM CPU architecture?"), Plugin.cpuTag, MetroPluginCPUArchitecture.AnyCPU, buildTargetName), new DefaultPluginImporterExtension.Property(EditorGUIUtility.TextContent("ScriptingBackend|Is plugin compatible with .NET, Il2Cpp or both scripting backends?"), Plugin.scriptingBackendTag, MetroPluginScriptingBackend.AnyScriptingBackend, buildTargetName), new DefaultPluginImporterExtension.Property(EditorGUIUtility.TextContent("Don't process|Plugin will not be modified in any way. This prevents Unity from making sure that it will work on Windows. Note: this setting have no effect for Il2Cpp builds"), Plugin.dontProcessPluginsTag, false, buildTargetName), _placeholderProperty };
        }

        public override void OnPlatformSettingsGUI(PluginImporterInspector inspector)
        {
            if (!base.propertiesRefreshed)
            {
                this.RefreshProperties(inspector);
            }
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < base.properties.Length; i++)
            {
                if (i == 1)
                {
                    GUI.enabled = _isNative && !_isCppFile;
                    if (_isNative)
                    {
                        int index = 0;
                        while (index < nativeArchs.Length)
                        {
                            if (((MetroPluginCPUArchitecture) base.properties[i].value) == nativeArchs[index])
                            {
                                break;
                            }
                            index++;
                        }
                        index = EditorGUILayout.Popup(base.properties[i].name, index, this.nativeArchsGUI, new GUILayoutOption[0]);
                        if (index >= nativeArchs.Length)
                        {
                            index = 0;
                            base.hasModified = true;
                            object[] args = new object[] { inspector.importer.assetPath, nativeArchs[index].ToString() };
                            Debug.LogWarningFormat("Setting '{0}' CPU value to {1}", args);
                        }
                        base.properties[i].value = nativeArchs[index];
                    }
                    else
                    {
                        base.properties[i].OnGUI(inspector);
                    }
                    GUI.enabled = true;
                    continue;
                }
                base.properties[i].OnGUI(inspector);
            }
            if (EditorGUI.EndChangeCheck())
            {
                base.hasModified = true;
            }
        }

        public static bool PluginRequiresPlaceholder(string path, bool areDotNet4AssembliesAllowed)
        {
            if (areDotNet4AssembliesAllowed)
            {
                return false;
            }
            try
            {
                return (string.Equals(Path.GetExtension(path), ".winmd", StringComparison.InvariantCultureIgnoreCase) || InternalEditorUtility.IsDotNet4Dll(path));
            }
            catch
            {
                return false;
            }
        }

        protected override void RefreshProperties(PluginImporterInspector inspector)
        {
            _originalPluginPath = ((PluginImporter) inspector.target).assetPath;
            _isNative = ((PluginImporter) inspector.target).isNativePlugin;
            string[] strArray = new string[] { ".cpp", ".h", ".c" };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<string, bool>(null, (IntPtr) <RefreshProperties>m__0);
            }
            _isCppFile = Enumerable.Any<string>(strArray, <>f__am$cache0);
            base.RefreshProperties(inspector);
        }

        internal static MetroPluginScriptingBackend ToMetroPluginScriptingBackend(ScriptingImplementation implementation)
        {
            if (implementation != ScriptingImplementation.IL2CPP)
            {
                if (implementation != ScriptingImplementation.WinRTDotNET)
                {
                    throw new Exception("Unsupported scripting implementation: " + implementation);
                }
            }
            else
            {
                return MetroPluginScriptingBackend.Il2Cpp;
            }
            return MetroPluginScriptingBackend.DotNet;
        }

        internal enum MetroPluginCPUArchitecture
        {
            AnyCPU,
            X86,
            ARM,
            X64
        }

        internal enum MetroPluginScriptingBackend
        {
            AnyScriptingBackend,
            DotNet,
            Il2Cpp
        }

        internal enum MetroPluginSDK
        {
            AnySDK,
            SDK80,
            SDK81,
            PhoneSDK81,
            UWP
        }
    }
}

