namespace UnityEditor.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using Unity.DataContract;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Hardware;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal static class ModuleManager
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Assembly, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<Assembly, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<Assembly, System.Type> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, string> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<Assembly, IEnumerable<IPlatformSupportModule>> <>f__mg$cache0;
        [NonSerialized]
        private static IPlatformSupportModule s_ActivePlatformModule;
        [NonSerialized]
        private static List<IEditorModule> s_EditorModules;
        [NonSerialized]
        private static IPackageManagerModule s_PackageManager;
        [NonSerialized]
        private static List<IPlatformSupportModule> s_PlatformModules;
        [NonSerialized]
        private static bool s_PlatformModulesInitialized;

        private static void ChangeActivePlatformModuleTo(string target)
        {
            DeactivateActivePlatformModule();
            foreach (IPlatformSupportModule module in platformSupportModules)
            {
                if (module.TargetName == target)
                {
                    s_ActivePlatformModule = module;
                    module.OnActivate();
                    break;
                }
            }
        }

        private static string CombinePaths(params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }
            if (paths.Length == 1)
            {
                return paths[0];
            }
            StringBuilder builder = new StringBuilder(paths[0]);
            for (int i = 1; i < paths.Length; i++)
            {
                builder.AppendFormat("{0}{1}", "/", paths[i]);
            }
            return builder.ToString();
        }

        private static void DeactivateActivePlatformModule()
        {
            if (s_ActivePlatformModule != null)
            {
                s_ActivePlatformModule.OnDeactivate();
                s_ActivePlatformModule = null;
            }
        }

        internal static IPlatformSupportModule FindPlatformSupportModule(string moduleName)
        {
            foreach (IPlatformSupportModule module in platformSupportModules)
            {
                if (module.TargetName == moduleName)
                {
                    return module;
                }
            }
            return null;
        }

        internal static IBuildAnalyzer GetBuildAnalyzer(string target)
        {
            if (target != null)
            {
                foreach (IPlatformSupportModule module in platformSupportModules)
                {
                    if (module.TargetName == target)
                    {
                        return module.CreateBuildAnalyzer();
                    }
                }
            }
            return null;
        }

        internal static IBuildAnalyzer GetBuildAnalyzer(BuildTarget target) => 
            GetBuildAnalyzer(GetTargetStringFromBuildTarget(target));

        internal static IBuildPostprocessor GetBuildPostProcessor(string target)
        {
            if (target != null)
            {
                foreach (IPlatformSupportModule module in platformSupportModules)
                {
                    if (module.TargetName == target)
                    {
                        return module.CreateBuildPostprocessor();
                    }
                }
            }
            return null;
        }

        internal static IBuildPostprocessor GetBuildPostProcessor(BuildTargetGroup targetGroup, BuildTarget target) => 
            GetBuildPostProcessor(GetTargetStringFrom(targetGroup, target));

        internal static IBuildWindowExtension GetBuildWindowExtension(string target)
        {
            if (!string.IsNullOrEmpty(target))
            {
                foreach (IPlatformSupportModule module in platformSupportModules)
                {
                    if (module.TargetName == target)
                    {
                        return module.CreateBuildWindowExtension();
                    }
                }
            }
            return null;
        }

        internal static ICompilationExtension GetCompilationExtension(string target)
        {
            foreach (IPlatformSupportModule module in platformSupportModules)
            {
                if (module.TargetName == target)
                {
                    return module.CreateCompilationExtension();
                }
            }
            return new DefaultCompilationExtension();
        }

        internal static IDevice GetDevice(string deviceId)
        {
            DevDevice device;
            if (!DevDeviceList.FindDevice(deviceId, out device))
            {
                throw new ApplicationException("Couldn't create device API for device: " + deviceId);
            }
            return FindPlatformSupportModule(device.module)?.CreateDevice(deviceId);
        }

        internal static GUIContent[] GetDisplayNames(string target)
        {
            if (!string.IsNullOrEmpty(target))
            {
                foreach (IPlatformSupportModule module in platformSupportModules)
                {
                    if (module.TargetName == target)
                    {
                        return module.GetDisplayNames();
                    }
                }
            }
            return null;
        }

        internal static ISettingEditorExtension GetEditorSettingsExtension(string target)
        {
            if (!string.IsNullOrEmpty(target))
            {
                foreach (IPlatformSupportModule module in platformSupportModules)
                {
                    if (module.TargetName == target)
                    {
                        return module.CreateSettingsEditorExtension();
                    }
                }
            }
            return null;
        }

        internal static string GetExtensionVersion(string target)
        {
            if (!string.IsNullOrEmpty(target))
            {
                foreach (IPlatformSupportModule module in platformSupportModules)
                {
                    if (module.TargetName == target)
                    {
                        return module.ExtensionVersion;
                    }
                }
            }
            return null;
        }

        internal static List<string> GetJamTargets()
        {
            List<string> list = new List<string>();
            foreach (IPlatformSupportModule module in platformSupportModules)
            {
                list.Add(module.JamTarget);
            }
            return list;
        }

        internal static IPluginImporterExtension GetPluginImporterExtension(string target)
        {
            if (target != null)
            {
                foreach (IPlatformSupportModule module in platformSupportModules)
                {
                    if (module.TargetName == target)
                    {
                        return module.CreatePluginImporterExtension();
                    }
                }
            }
            return null;
        }

        internal static IPluginImporterExtension GetPluginImporterExtension(BuildTarget target) => 
            GetPluginImporterExtension(GetTargetStringFromBuildTarget(target));

        internal static IPluginImporterExtension GetPluginImporterExtension(BuildTargetGroup target) => 
            GetPluginImporterExtension(GetTargetStringFromBuildTargetGroup(target));

        internal static List<IPreferenceWindowExtension> GetPreferenceWindowExtensions()
        {
            List<IPreferenceWindowExtension> list = new List<IPreferenceWindowExtension>();
            foreach (IPlatformSupportModule module in platformSupportModules)
            {
                IPreferenceWindowExtension item = module.CreatePreferenceWindowExtension();
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        private static IScriptingImplementations GetScriptingImplementations(string target)
        {
            if (!string.IsNullOrEmpty(target))
            {
                foreach (IPlatformSupportModule module in platformSupportModules)
                {
                    if (module.TargetName == target)
                    {
                        return module.CreateScriptingImplementations();
                    }
                }
            }
            return null;
        }

        internal static IScriptingImplementations GetScriptingImplementations(BuildTargetGroup target)
        {
            if (target == BuildTargetGroup.Standalone)
            {
                return new DesktopStandalonePostProcessor.ScriptingImplementations();
            }
            return GetScriptingImplementations(GetTargetStringFromBuildTargetGroup(target));
        }

        internal static string GetTargetStringFrom(BuildTargetGroup targetGroup, BuildTarget target)
        {
            if (targetGroup == BuildTargetGroup.Unknown)
            {
                throw new ArgumentException("targetGroup must be valid");
            }
            if (targetGroup != BuildTargetGroup.Facebook)
            {
                if (targetGroup == BuildTargetGroup.Standalone)
                {
                    return GetTargetStringFromBuildTarget(target);
                }
            }
            else
            {
                return "Facebook";
            }
            return GetTargetStringFromBuildTargetGroup(targetGroup);
        }

        internal static string GetTargetStringFromBuildTarget(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return "LinuxStandalone";

                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows:
                    return "WindowsStandalone";

                case BuildTarget.WebGL:
                    return "WebGL";

                case BuildTarget.WSAPlayer:
                    return "Metro";

                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                case BuildTarget.StandaloneOSXIntel:
                    return "OSXStandalone";

                case BuildTarget.Tizen:
                    return "Tizen";

                case BuildTarget.PSP2:
                    return "PSP2";

                case BuildTarget.PS4:
                    return "PS4";

                case BuildTarget.PSM:
                    return "PSM";

                case BuildTarget.XboxOne:
                    return "XboxOne";

                case BuildTarget.SamsungTV:
                    return "SamsungTV";

                case BuildTarget.N3DS:
                    return "N3DS";

                case BuildTarget.WiiU:
                    return "WiiU";

                case BuildTarget.tvOS:
                    return "tvOS";

                case BuildTarget.Switch:
                    return "Switch";

                case BuildTarget.iOS:
                    return "iOS";
            }
            if (target != BuildTarget.Android)
            {
                return null;
            }
            return "Android";
        }

        internal static string GetTargetStringFromBuildTargetGroup(BuildTargetGroup target)
        {
            switch (target)
            {
                case BuildTargetGroup.WebGL:
                    return "WebGL";

                case BuildTargetGroup.WSA:
                    return "Metro";

                case BuildTargetGroup.Tizen:
                    return "Tizen";

                case BuildTargetGroup.PSP2:
                    return "PSP2";

                case BuildTargetGroup.PS4:
                    return "PS4";

                case BuildTargetGroup.PSM:
                    return "PSM";

                case BuildTargetGroup.XboxOne:
                    return "XboxOne";

                case BuildTargetGroup.SamsungTV:
                    return "SamsungTV";

                case BuildTargetGroup.N3DS:
                    return "N3DS";

                case BuildTargetGroup.WiiU:
                    return "WiiU";

                case BuildTargetGroup.tvOS:
                    return "tvOS";

                case BuildTargetGroup.Facebook:
                    return "Facebook";

                case BuildTargetGroup.Switch:
                    return "Switch";

                case BuildTargetGroup.iPhone:
                    return "iOS";

                case BuildTargetGroup.Android:
                    return "Android";
            }
            return null;
        }

        internal static ITextureImportSettingsExtension GetTextureImportSettingsExtension(string targetName)
        {
            foreach (IPlatformSupportModule module in platformSupportModules)
            {
                if (module.TargetName == targetName)
                {
                    return module.CreateTextureImportSettingsExtension();
                }
            }
            return new DefaultTextureImportSettingsExtension();
        }

        internal static ITextureImportSettingsExtension GetTextureImportSettingsExtension(BuildTarget target) => 
            GetTextureImportSettingsExtension(GetTargetStringFromBuildTarget(target));

        internal static IUserAssembliesValidator GetUserAssembliesValidator(string target)
        {
            if (target != null)
            {
                foreach (IPlatformSupportModule module in platformSupportModules)
                {
                    if (module.TargetName == target)
                    {
                        return module.CreateUserAssembliesValidatorExtension();
                    }
                }
            }
            return null;
        }

        internal static bool HaveLicenseForBuildTarget(string targetString)
        {
            BuildTargetGroup group;
            BuildTarget target;
            if (!TryParseBuildTarget(targetString, out group, out target))
            {
                return false;
            }
            return BuildPipeline.LicenseCheck(target);
        }

        internal static void Initialize()
        {
            if (s_PackageManager == null)
            {
                RegisterPackageManager();
                if (s_PackageManager != null)
                {
                    LoadUnityExtensions();
                }
                else
                {
                    UnityEngine.Debug.LogError("Failed to load package manager");
                }
            }
        }

        private static bool InitializePackageManager(Unity.DataContract.PackageInfo package)
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = x => x.Value.type == PackageFileType.Dll;
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = x => x.Key;
            }
            string str = Enumerable.Select<KeyValuePair<string, PackageFileData>, string>(Enumerable.Where<KeyValuePair<string, PackageFileData>>(package.files, <>f__am$cache4), <>f__am$cache5).FirstOrDefault<string>();
            if ((str == null) || !File.Exists(Path.Combine(package.basePath, str)))
            {
                return false;
            }
            InternalEditorUtility.SetPlatformPath(package.basePath);
            return InitializePackageManager(InternalEditorUtility.LoadAssemblyWrapper(Path.GetFileName(str), Path.Combine(package.basePath, str)), package);
        }

        private static bool InitializePackageManager(Assembly assembly, Unity.DataContract.PackageInfo package)
        {
            s_PackageManager = AssemblyHelper.FindImplementors<IPackageManagerModule>(assembly).FirstOrDefault<IPackageManagerModule>();
            if (s_PackageManager == null)
            {
                return false;
            }
            string location = assembly.Location;
            if (package != null)
            {
                InternalEditorUtility.RegisterPrecompiledAssembly(Path.GetFileName(location), location);
            }
            else
            {
                Unity.DataContract.PackageInfo info = new Unity.DataContract.PackageInfo {
                    basePath = Path.GetDirectoryName(location)
                };
                package = info;
            }
            s_PackageManager.moduleInfo = package;
            s_PackageManager.editorInstallPath = EditorApplication.applicationContentsPath;
            s_PackageManager.unityVersion = (string) new PackageVersion(Application.unityVersion);
            s_PackageManager.Initialize();
            foreach (Unity.DataContract.PackageInfo info2 in s_PackageManager.playbackEngines)
            {
                BuildTargetGroup group;
                BuildTarget target;
                if (TryParseBuildTarget(info2.name, out group, out target))
                {
                    object[] arg = new object[] { target, info2.version, info2.unityVersion, info2.basePath, group };
                    Console.WriteLine("Setting {4}:{0} v{1} for Unity v{2} to {3}", arg);
                    if (<>f__am$cache6 == null)
                    {
                        <>f__am$cache6 = f => f.Value.type == PackageFileType.Dll;
                    }
                    foreach (KeyValuePair<string, PackageFileData> pair in Enumerable.Where<KeyValuePair<string, PackageFileData>>(info2.files, <>f__am$cache6))
                    {
                        if (!File.Exists(Path.Combine(info2.basePath, pair.Key).NormalizePath()))
                        {
                            object[] args = new object[] { info2.basePath, info2.name };
                            UnityEngine.Debug.LogWarningFormat("Missing assembly \t{0} for {1}. Player support may be incomplete.", args);
                        }
                        else
                        {
                            InternalEditorUtility.RegisterPrecompiledAssembly(Path.GetFileName(location), location);
                        }
                    }
                    BuildPipeline.SetPlaybackEngineDirectory(target, BuildOptions.CompressTextures, info2.basePath);
                    InternalEditorUtility.SetPlatformPath(info2.basePath);
                    s_PackageManager.LoadPackage(info2);
                }
            }
            return true;
        }

        internal static void InitializePlatformSupportModules()
        {
            if (s_PlatformModulesInitialized)
            {
                Console.WriteLine("Platform modules already initialized, skipping");
            }
            else
            {
                Initialize();
                RegisterPlatformSupportModules();
                foreach (IPlatformSupportModule module in platformSupportModules)
                {
                    foreach (string str in module.NativeLibraries)
                    {
                        EditorUtility.LoadPlatformSupportNativeLibrary(str);
                    }
                    foreach (string str2 in module.AssemblyReferencesForUserScripts)
                    {
                        InternalEditorUtility.RegisterPrecompiledAssembly(Path.GetFileName(str2), str2);
                    }
                    EditorUtility.LoadPlatformSupportModuleNativeDllInternal(module.TargetName);
                    module.OnLoad();
                }
                OnActiveBuildTargetChanged(BuildTarget.NoTarget, EditorUserBuildSettings.activeBuildTarget);
                s_PlatformModulesInitialized = true;
            }
        }

        internal static bool IsPlatformSupported(BuildTarget target) => 
            (GetTargetStringFromBuildTarget(target) != null);

        internal static bool IsPlatformSupportLoaded(string target)
        {
            foreach (IPlatformSupportModule module in platformSupportModules)
            {
                if (module.TargetName == target)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsRegisteredModule(string file) => 
            ((s_PackageManager != null) && (s_PackageManager.GetType().Assembly.Location.NormalizePath() == file.NormalizePath()));

        private static void LoadUnityExtensions()
        {
            foreach (Unity.DataContract.PackageInfo info in s_PackageManager.unityExtensions)
            {
                object[] arg = new object[] { info.name, info.version, info.unityVersion, info.basePath };
                Console.WriteLine("Setting {0} v{1} for Unity v{2} to {3}", arg);
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = f => f.Value.type == PackageFileType.Dll;
                }
                foreach (KeyValuePair<string, PackageFileData> pair in Enumerable.Where<KeyValuePair<string, PackageFileData>>(info.files, <>f__am$cache0))
                {
                    string path = Path.Combine(info.basePath, pair.Key).NormalizePath();
                    if (!File.Exists(path))
                    {
                        object[] args = new object[] { pair.Key, info.name };
                        UnityEngine.Debug.LogWarningFormat("Missing assembly \t{0} for {1}. Extension support may be incomplete.", args);
                    }
                    else
                    {
                        bool flag = !string.IsNullOrEmpty(pair.Value.guid);
                        Console.WriteLine("  {0} ({1}) GUID: {2}", pair.Key, !flag ? "Custom" : "Extension", pair.Value.guid);
                        if (flag)
                        {
                            InternalEditorUtility.RegisterExtensionDll(path.Replace('\\', '/'), pair.Value.guid);
                        }
                        else
                        {
                            InternalEditorUtility.RegisterPrecompiledAssembly(Path.GetFileName(path), path);
                        }
                    }
                }
                s_PackageManager.LoadPackage(info);
            }
        }

        private static void OnActiveBuildTargetChanged(BuildTarget oldTarget, BuildTarget newTarget)
        {
            ChangeActivePlatformModuleTo(GetTargetStringFromBuildTarget(newTarget));
        }

        internal static void RegisterAdditionalUnityExtensions()
        {
            foreach (IPlatformSupportModule module in platformSupportModules)
            {
                module.RegisterAdditionalUnityExtensions();
            }
        }

        private static IEnumerable<IEditorModule> RegisterEditorModulesFromAssembly(Assembly assembly) => 
            AssemblyHelper.FindImplementors<IEditorModule>(assembly);

        private static IEnumerable<T> RegisterModulesFromLoadedAssemblies<T>(Func<Assembly, IEnumerable<T>> processAssembly)
        {
            <RegisterModulesFromLoadedAssemblies>c__AnonStorey0<T> storey = new <RegisterModulesFromLoadedAssemblies>c__AnonStorey0<T> {
                processAssembly = processAssembly
            };
            if (storey.processAssembly == null)
            {
                throw new ArgumentNullException("processAssembly");
            }
            return Enumerable.Aggregate<Assembly, List<T>>(AppDomain.CurrentDomain.GetAssemblies(), new List<T>(), new Func<List<T>, Assembly, List<T>>(storey.<>m__0));
        }

        private static void RegisterPackageManager()
        {
            Unity.DataContract.PackageInfo info;
            s_EditorModules = new List<IEditorModule>();
            try
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = a => null != a.GetType("Unity.PackageManager.PackageManager");
                }
                Assembly assembly = Enumerable.FirstOrDefault<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), <>f__am$cache1);
                if ((assembly != null) && InitializePackageManager(assembly, null))
                {
                    return;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error enumerating assemblies looking for package manager. {0}", exception);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = a => a.GetName().Name == "Unity.Locator";
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = a => a.GetType("Unity.PackageManager.Locator");
            }
            System.Type type = Enumerable.Select<Assembly, System.Type>(Enumerable.Where<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), <>f__am$cache2), <>f__am$cache3).FirstOrDefault<System.Type>();
            try
            {
                object[] args = new object[2];
                string[] textArray1 = new string[] { FileUtil.NiceWinPath(EditorApplication.applicationContentsPath) };
                args[0] = textArray1;
                args[1] = Application.unityVersion;
                type.InvokeMember("Scan", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, args);
            }
            catch (Exception exception2)
            {
                Console.WriteLine("Error scanning for packages. {0}", exception2);
                return;
            }
            try
            {
                string[] textArray2 = new string[] { Application.unityVersion };
                info = type.InvokeMember("GetPackageManager", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, textArray2) as Unity.DataContract.PackageInfo;
                if (info == null)
                {
                    Console.WriteLine("No package manager found!");
                    return;
                }
            }
            catch (Exception exception3)
            {
                Console.WriteLine("Error scanning for packages. {0}", exception3);
                return;
            }
            try
            {
                InitializePackageManager(info);
            }
            catch (Exception exception4)
            {
                Console.WriteLine("Error initializing package manager. {0}", exception4);
            }
            if (s_PackageManager != null)
            {
                s_PackageManager.CheckForUpdates();
            }
        }

        private static void RegisterPlatformSupportModules()
        {
            if (s_PlatformModules != null)
            {
                Console.WriteLine("Modules already registered, not loading");
            }
            else
            {
                Console.WriteLine("Registering platform support modules:");
                Stopwatch stopwatch = Stopwatch.StartNew();
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new Func<Assembly, IEnumerable<IPlatformSupportModule>>(ModuleManager.RegisterPlatformSupportModulesFromAssembly);
                }
                s_PlatformModules = RegisterModulesFromLoadedAssemblies<IPlatformSupportModule>(<>f__mg$cache0).ToList<IPlatformSupportModule>();
                stopwatch.Stop();
                Console.WriteLine("Registered platform support modules in: " + stopwatch.Elapsed.TotalSeconds + "s.");
            }
        }

        internal static IEnumerable<IPlatformSupportModule> RegisterPlatformSupportModulesFromAssembly(Assembly assembly) => 
            AssemblyHelper.FindImplementors<IPlatformSupportModule>(assembly);

        internal static bool ShouldShowMultiDisplayOption()
        {
            GUIContent[] displayNames = GetDisplayNames(EditorUserBuildSettings.activeBuildTarget.ToString());
            return (((BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) == BuildTargetGroup.Standalone) || (BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) == BuildTargetGroup.WSA)) || (displayNames != null));
        }

        internal static void Shutdown()
        {
            if (s_PackageManager != null)
            {
                s_PackageManager.Shutdown(true);
            }
            s_PackageManager = null;
            s_PlatformModules = null;
            s_EditorModules = null;
        }

        internal static void ShutdownPlatformSupportModules()
        {
            DeactivateActivePlatformModule();
            if (s_PlatformModules != null)
            {
                foreach (IPlatformSupportModule module in s_PlatformModules)
                {
                    module.OnUnload();
                }
            }
        }

        private static bool TryParseBuildTarget(string targetString, out BuildTargetGroup buildTargetGroup, out BuildTarget target)
        {
            buildTargetGroup = BuildTargetGroup.Standalone;
            target = BuildTarget.StandaloneWindows;
            try
            {
                BuildTargetGroup facebook = BuildTargetGroup.Facebook;
                if (targetString == facebook.ToString())
                {
                    buildTargetGroup = BuildTargetGroup.Facebook;
                    target = BuildTarget.StandaloneWindows;
                }
                else
                {
                    target = (BuildTarget) Enum.Parse(typeof(BuildTarget), targetString);
                    buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
                }
                return true;
            }
            catch
            {
                UnityEngine.Debug.LogWarning($"Couldn't find build target for {targetString}");
            }
            return false;
        }

        private static List<IEditorModule> editorModules
        {
            get
            {
                if (s_EditorModules == null)
                {
                    return new List<IEditorModule>();
                }
                return s_EditorModules;
            }
        }

        internal static IPackageManagerModule packageManager
        {
            get
            {
                Initialize();
                return s_PackageManager;
            }
        }

        internal static IEnumerable<IPlatformSupportModule> platformSupportModules
        {
            get
            {
                Initialize();
                if (s_PlatformModules == null)
                {
                    RegisterPlatformSupportModules();
                }
                return s_PlatformModules;
            }
        }

        internal static IEnumerable<IPlatformSupportModule> platformSupportModulesDontRegister
        {
            get
            {
                if (s_PlatformModules == null)
                {
                    return new List<IPlatformSupportModule>();
                }
                return s_PlatformModules;
            }
        }

        [CompilerGenerated]
        private sealed class <RegisterModulesFromLoadedAssemblies>c__AnonStorey0<T>
        {
            internal Func<Assembly, IEnumerable<T>> processAssembly;

            internal List<T> <>m__0(List<T> list, Assembly assembly)
            {
                try
                {
                    IEnumerable<T> source = this.processAssembly(assembly);
                    if ((source != null) && source.Any<T>())
                    {
                        list.AddRange(source);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error while registering modules from " + assembly.FullName + ": " + exception.Message);
                }
                return list;
            }
        }

        private class BuildTargetChangedHandler : IActiveBuildTargetChanged, IOrderedCallback
        {
            public void OnActiveBuildTargetChanged(BuildTarget oldTarget, BuildTarget newTarget)
            {
                ModuleManager.OnActiveBuildTargetChanged(oldTarget, newTarget);
            }

            public int callbackOrder =>
                0;
        }
    }
}

