namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Scripting.Compilers;

    internal class CustomScriptAssembly
    {
        [CompilerGenerated]
        private static Func<CustomScriptAssemblyPlatform, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<CustomScriptAssemblyPlatform, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, CustomScriptAssemblyPlatform> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<string, CustomScriptAssemblyPlatform> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<CustomScriptAssemblyPlatform, string> <>f__am$cache4;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private CustomScriptAssemblyPlatform[] <ExcludePlatforms>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <FilePath>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CustomScriptAssemblyPlatform[] <IncludePlatforms>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private SupportedLanguage <Language>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Name>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <PathPrefix>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private static CustomScriptAssemblyPlatform[] <Platforms>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string[] <References>k__BackingField;

        static CustomScriptAssembly()
        {
            Platforms = new CustomScriptAssemblyPlatform[0x17];
            int num = 0;
            Platforms[num++] = new CustomScriptAssemblyPlatform("Editor", BuildTarget.NoTarget);
            Platforms[num++] = new CustomScriptAssemblyPlatform("OSXtandalone32", BuildTarget.StandaloneOSXIntel);
            Platforms[num++] = new CustomScriptAssemblyPlatform("OSXStandalone64", BuildTarget.StandaloneOSXIntel64);
            Platforms[num++] = new CustomScriptAssemblyPlatform("OSXStandaloneUniversal", BuildTarget.StandaloneOSXUniversal);
            Platforms[num++] = new CustomScriptAssemblyPlatform("WindowsStandalone32", BuildTarget.StandaloneWindows);
            Platforms[num++] = new CustomScriptAssemblyPlatform("WindowsStandalone64", BuildTarget.StandaloneWindows64);
            Platforms[num++] = new CustomScriptAssemblyPlatform("LinuxStandalone32", BuildTarget.StandaloneLinux);
            Platforms[num++] = new CustomScriptAssemblyPlatform("LinuxStandalone64", BuildTarget.StandaloneLinux64);
            Platforms[num++] = new CustomScriptAssemblyPlatform("LinuxStandaloneUniversal", BuildTarget.StandaloneLinuxUniversal);
            Platforms[num++] = new CustomScriptAssemblyPlatform("iOS", BuildTarget.iOS);
            Platforms[num++] = new CustomScriptAssemblyPlatform("Android", BuildTarget.Android);
            Platforms[num++] = new CustomScriptAssemblyPlatform("WebGL", BuildTarget.WebGL);
            Platforms[num++] = new CustomScriptAssemblyPlatform("WSA", BuildTarget.WSAPlayer);
            Platforms[num++] = new CustomScriptAssemblyPlatform("Tizen", BuildTarget.Tizen);
            Platforms[num++] = new CustomScriptAssemblyPlatform("PSVita", BuildTarget.PSP2);
            Platforms[num++] = new CustomScriptAssemblyPlatform("PS4", BuildTarget.PS4);
            Platforms[num++] = new CustomScriptAssemblyPlatform("PSMobile", BuildTarget.PSM);
            Platforms[num++] = new CustomScriptAssemblyPlatform("XboxOne", BuildTarget.XboxOne);
            Platforms[num++] = new CustomScriptAssemblyPlatform("Nintendo3DS", BuildTarget.N3DS);
            Platforms[num++] = new CustomScriptAssemblyPlatform("WiiU", BuildTarget.WiiU);
            Platforms[num++] = new CustomScriptAssemblyPlatform("tvOS", BuildTarget.tvOS);
            Platforms[num++] = new CustomScriptAssemblyPlatform("SamsungTV", BuildTarget.SamsungTV);
            Platforms[num++] = new CustomScriptAssemblyPlatform("Switch", BuildTarget.Switch);
        }

        public static CustomScriptAssembly FromCustomScriptAssemblyData(string path, CustomScriptAssemblyData customScriptAssemblyData)
        {
            if (customScriptAssemblyData == null)
            {
                return null;
            }
            string directoryName = Path.GetDirectoryName(path);
            CustomScriptAssembly assembly2 = new CustomScriptAssembly {
                Name = customScriptAssemblyData.name,
                References = customScriptAssemblyData.references,
                FilePath = path,
                PathPrefix = directoryName,
                Language = ScriptCompilers.GetLanguageFromName(customScriptAssemblyData.language)
            };
            if ((customScriptAssemblyData.includePlatforms != null) && (customScriptAssemblyData.includePlatforms.Length > 0))
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = name => GetPlatformFromName(name);
                }
                assembly2.IncludePlatforms = Enumerable.Select<string, CustomScriptAssemblyPlatform>(customScriptAssemblyData.includePlatforms, <>f__am$cache2).ToArray<CustomScriptAssemblyPlatform>();
            }
            if ((customScriptAssemblyData.excludePlatforms != null) && (customScriptAssemblyData.excludePlatforms.Length > 0))
            {
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = name => GetPlatformFromName(name);
                }
                assembly2.ExcludePlatforms = Enumerable.Select<string, CustomScriptAssemblyPlatform>(customScriptAssemblyData.excludePlatforms, <>f__am$cache3).ToArray<CustomScriptAssemblyPlatform>();
            }
            return assembly2;
        }

        public static CustomScriptAssemblyPlatform GetPlatformFromBuildTarget(BuildTarget buildTarget)
        {
            foreach (CustomScriptAssemblyPlatform platform in Platforms)
            {
                if (platform.BuildTarget == buildTarget)
                {
                    return platform;
                }
            }
            throw new ArgumentException($"No CustomScriptAssemblyPlatform setup for BuildTarget '{buildTarget}'");
        }

        public static CustomScriptAssemblyPlatform GetPlatformFromName(string name)
        {
            foreach (CustomScriptAssemblyPlatform platform in Platforms)
            {
                if (string.Equals(platform.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return platform;
                }
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = p => $"'{p.Name}'";
            }
            string[] strArray = Enumerable.Select<CustomScriptAssemblyPlatform, string>(Platforms, <>f__am$cache4).ToArray<string>();
            string str = string.Join(",", strArray);
            throw new ArgumentException($"Platform name '{name}' not supported. Supported platform names: {str}");
        }

        public bool IsCompatibleWith(BuildTarget buildTarget, BuildFlags buildFlags)
        {
            <IsCompatibleWith>c__AnonStorey0 storey = new <IsCompatibleWith>c__AnonStorey0 {
                buildTarget = buildTarget
            };
            if ((this.IncludePlatforms == null) && (this.ExcludePlatforms == null))
            {
                return true;
            }
            bool flag2 = (buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
            if (flag2)
            {
                return this.IsCompatibleWithEditor();
            }
            if (flag2)
            {
                storey.buildTarget = BuildTarget.NoTarget;
            }
            if (this.ExcludePlatforms != null)
            {
                return Enumerable.All<CustomScriptAssemblyPlatform>(this.ExcludePlatforms, new Func<CustomScriptAssemblyPlatform, bool>(storey.<>m__0));
            }
            return Enumerable.Any<CustomScriptAssemblyPlatform>(this.IncludePlatforms, new Func<CustomScriptAssemblyPlatform, bool>(storey.<>m__1));
        }

        public bool IsCompatibleWithEditor()
        {
            if (this.ExcludePlatforms != null)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = p => p.BuildTarget != BuildTarget.NoTarget;
                }
                return Enumerable.All<CustomScriptAssemblyPlatform>(this.ExcludePlatforms, <>f__am$cache0);
            }
            if (this.IncludePlatforms != null)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = p => p.BuildTarget == BuildTarget.NoTarget;
                }
                return Enumerable.Any<CustomScriptAssemblyPlatform>(this.IncludePlatforms, <>f__am$cache1);
            }
            return true;
        }

        public UnityEditor.Scripting.ScriptCompilation.AssemblyFlags AssemblyFlags
        {
            get
            {
                if (((this.IncludePlatforms != null) && (this.IncludePlatforms.Length == 1)) && (this.IncludePlatforms[0].BuildTarget == BuildTarget.NoTarget))
                {
                    return UnityEditor.Scripting.ScriptCompilation.AssemblyFlags.EditorOnly;
                }
                return UnityEditor.Scripting.ScriptCompilation.AssemblyFlags.None;
            }
        }

        public CustomScriptAssemblyPlatform[] ExcludePlatforms { get; set; }

        public string FilePath { get; set; }

        public CustomScriptAssemblyPlatform[] IncludePlatforms { get; set; }

        public SupportedLanguage Language { get; set; }

        public string Name { get; set; }

        public string PathPrefix { get; set; }

        private static CustomScriptAssemblyPlatform[] Platforms
        {
            [CompilerGenerated]
            get => 
                <Platforms>k__BackingField;
            [CompilerGenerated]
            set
            {
                <Platforms>k__BackingField = value;
            }
        }

        public string[] References { get; set; }

        [CompilerGenerated]
        private sealed class <IsCompatibleWith>c__AnonStorey0
        {
            internal BuildTarget buildTarget;

            internal bool <>m__0(CustomScriptAssemblyPlatform p) => 
                (p.BuildTarget != this.buildTarget);

            internal bool <>m__1(CustomScriptAssemblyPlatform p) => 
                (p.BuildTarget == this.buildTarget);
        }
    }
}

