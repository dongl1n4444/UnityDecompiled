namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Scripting.Compilers;
    using UnityEngine;

    internal class IL2CPPBuilder
    {
        [CompilerGenerated]
        private static Predicate<string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache2;
        private readonly bool m_DebugBuild;
        private readonly LinkXmlReader m_linkXmlReader = new LinkXmlReader();
        private readonly Action<string> m_ModifyOutputBeforeCompile;
        private readonly IIl2CppPlatformProvider m_PlatformProvider;
        private readonly RuntimeClassRegistry m_RuntimeClassRegistry;
        private readonly string m_StagingAreaData;
        private readonly string m_TempFolder;

        public IL2CPPBuilder(string tempFolder, string stagingAreaData, IIl2CppPlatformProvider platformProvider, Action<string> modifyOutputBeforeCompile, RuntimeClassRegistry runtimeClassRegistry, bool debugBuild)
        {
            this.m_TempFolder = tempFolder;
            this.m_StagingAreaData = stagingAreaData;
            this.m_PlatformProvider = platformProvider;
            this.m_ModifyOutputBeforeCompile = modifyOutputBeforeCompile;
            this.m_RuntimeClassRegistry = runtimeClassRegistry;
            this.m_DebugBuild = debugBuild;
        }

        private void ConvertPlayerDlltoCpp(ICollection<string> userAssemblies, string outputDirectory, string workingDirectory)
        {
            if (userAssemblies.Count != 0)
            {
                List<string> arguments = new List<string> { "--convert-to-cpp" };
                if (this.m_PlatformProvider.emitNullChecks)
                {
                    arguments.Add("--emit-null-checks");
                }
                if (this.m_PlatformProvider.enableStackTraces)
                {
                    arguments.Add("--enable-stacktrace");
                }
                if (this.m_PlatformProvider.enableArrayBoundsCheck)
                {
                    arguments.Add("--enable-array-bounds-check");
                }
                if (this.m_PlatformProvider.enableDivideByZeroCheck)
                {
                    arguments.Add("--enable-divide-by-zero-check");
                }
                if (this.m_PlatformProvider.developmentMode)
                {
                    arguments.Add("--development-mode");
                }
                if (PlayerSettings.GetApiCompatibilityLevel(BuildPipeline.GetBuildTargetGroup(this.m_PlatformProvider.target)) == ApiCompatibilityLevel.NET_4_6)
                {
                    arguments.Add("--dotnetprofile=\"net45\"");
                }
                Il2CppNativeCodeBuilder builder = this.m_PlatformProvider.CreateIl2CppNativeCodeBuilder();
                if (builder != null)
                {
                    Il2CppNativeCodeBuilderUtils.ClearAndPrepareCacheDirectory(builder);
                    arguments.AddRange(Il2CppNativeCodeBuilderUtils.AddBuilderArguments(builder, this.OutputFileRelativePath(), this.m_PlatformProvider.includePaths, this.m_DebugBuild));
                }
                arguments.Add($"--map-file-parser="{GetMapFileParserPath()}"");
                string environmentVariable = PlayerSettings.GetAdditionalIl2CppArgs();
                if (!string.IsNullOrEmpty(environmentVariable))
                {
                    arguments.Add(environmentVariable);
                }
                environmentVariable = Environment.GetEnvironmentVariable("IL2CPP_ADDITIONAL_ARGS");
                if (!string.IsNullOrEmpty(environmentVariable))
                {
                    arguments.Add(environmentVariable);
                }
                List<string> list2 = new List<string>(userAssemblies);
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = arg => "--assembly=\"" + Path.GetFullPath(arg) + "\"";
                }
                arguments.AddRange(Enumerable.Select<string, string>(list2, <>f__am$cache1));
                arguments.Add($"--generatedcppdir="{Path.GetFullPath(outputDirectory)}"");
                string info = "Converting managed assemblies to C++";
                if (builder != null)
                {
                    info = "Building native binary with IL2CPP...";
                }
                if (EditorUtility.DisplayCancelableProgressBar("Building Player", info, 0.3f))
                {
                    throw new OperationCanceledException();
                }
                Action<ProcessStartInfo> setupStartInfo = null;
                if (builder != null)
                {
                    setupStartInfo = new Action<ProcessStartInfo>(builder.SetupStartInfo);
                }
                this.RunIl2CppWithArguments(arguments, setupStartInfo, workingDirectory);
            }
        }

        private IEnumerable<string> FilterUserAssemblies(IEnumerable<string> assemblies, Predicate<string> isUsed, string managedDir)
        {
            <FilterUserAssemblies>c__AnonStorey0 storey = new <FilterUserAssemblies>c__AnonStorey0 {
                isUsed = isUsed,
                managedDir = managedDir
            };
            return Enumerable.Select<string, string>(Enumerable.Where<string>(assemblies, new Func<string, bool>(storey.<>m__0)), new Func<string, string>(storey.<>m__1));
        }

        public string GetCppOutputDirectoryInStagingArea() => 
            GetCppOutputPath(this.m_TempFolder);

        public static string GetCppOutputPath(string tempFolder) => 
            Path.Combine(tempFolder, "il2cppOutput");

        private string GetIl2CppExe() => 
            (this.m_PlatformProvider.il2CppFolder + "/build/il2cpp.exe");

        private static string GetMapFileParserPath() => 
            Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, (Application.platform != RuntimePlatform.WindowsEditor) ? "Tools/MapFileParser/MapFileParser" : @"Tools\MapFileParser\MapFileParser.exe"));

        private HashSet<string> GetUserAssemblies(string managedDir)
        {
            HashSet<string> set = new HashSet<string>();
            set.UnionWith(this.FilterUserAssemblies(this.m_RuntimeClassRegistry.GetUserAssemblies(), new Predicate<string>(this.m_RuntimeClassRegistry.IsDLLUsed), managedDir));
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = assembly => true;
            }
            set.UnionWith(this.FilterUserAssemblies(Directory.GetFiles(managedDir, "I18N*.dll", SearchOption.TopDirectoryOnly), <>f__am$cache0, managedDir));
            return set;
        }

        internal List<string> GetUserAssembliesToConvert(string managedDir)
        {
            HashSet<string> userAssemblies = this.GetUserAssemblies(managedDir);
            userAssemblies.Add(Directory.GetFiles(managedDir, "UnityEngine.dll", SearchOption.TopDirectoryOnly).Single<string>());
            userAssemblies.UnionWith(this.FilterUserAssemblies(Directory.GetFiles(managedDir, "*.dll", SearchOption.TopDirectoryOnly), new Predicate<string>(this.m_linkXmlReader.IsDLLUsed), managedDir));
            return userAssemblies.ToList<string>();
        }

        private string OutputFileRelativePath()
        {
            string path = Path.Combine(this.m_StagingAreaData, "Native");
            Directory.CreateDirectory(path);
            return Path.Combine(path, this.m_PlatformProvider.nativeLibraryFileName);
        }

        public void Run()
        {
            string cppOutputDirectoryInStagingArea = this.GetCppOutputDirectoryInStagingArea();
            string fullPath = Path.GetFullPath(Path.Combine(this.m_StagingAreaData, "Managed"));
            foreach (string str3 in Directory.GetFiles(fullPath))
            {
                FileInfo info = new FileInfo(str3) {
                    IsReadOnly = false
                };
            }
            AssemblyStripper.StripAssemblies(this.m_StagingAreaData, this.m_PlatformProvider, this.m_RuntimeClassRegistry);
            FileUtil.CreateOrCleanDirectory(cppOutputDirectoryInStagingArea);
            if (this.m_ModifyOutputBeforeCompile != null)
            {
                this.m_ModifyOutputBeforeCompile(cppOutputDirectoryInStagingArea);
            }
            this.ConvertPlayerDlltoCpp(this.GetUserAssembliesToConvert(fullPath), cppOutputDirectoryInStagingArea, fullPath);
            if ((this.m_PlatformProvider.CreateNativeCompiler() != null) && (this.m_PlatformProvider.CreateIl2CppNativeCodeBuilder() == null))
            {
                string outFile = this.OutputFileRelativePath();
                List<string> includePaths = new List<string>(this.m_PlatformProvider.includePaths) {
                    cppOutputDirectoryInStagingArea
                };
                this.m_PlatformProvider.CreateNativeCompiler().CompileDynamicLibrary(outFile, NativeCompiler.AllSourceFilesIn(cppOutputDirectoryInStagingArea), includePaths, this.m_PlatformProvider.libraryPaths, new string[0]);
            }
        }

        public void RunCompileAndLink()
        {
            Il2CppNativeCodeBuilder builder = this.m_PlatformProvider.CreateIl2CppNativeCodeBuilder();
            if (builder != null)
            {
                Il2CppNativeCodeBuilderUtils.ClearAndPrepareCacheDirectory(builder);
                List<string> arguments = Il2CppNativeCodeBuilderUtils.AddBuilderArguments(builder, this.OutputFileRelativePath(), this.m_PlatformProvider.includePaths, this.m_DebugBuild).ToList<string>();
                arguments.Add($"--map-file-parser="{GetMapFileParserPath()}"");
                arguments.Add($"--generatedcppdir="{Path.GetFullPath(this.GetCppOutputDirectoryInStagingArea())}"");
                Action<ProcessStartInfo> setupStartInfo = new Action<ProcessStartInfo>(builder.SetupStartInfo);
                string fullPath = Path.GetFullPath(Path.Combine(this.m_StagingAreaData, "Managed"));
                this.RunIl2CppWithArguments(arguments, setupStartInfo, fullPath);
            }
        }

        private void RunIl2CppWithArguments(List<string> arguments, Action<ProcessStartInfo> setupStartInfo, string workingDirectory)
        {
            string exe = this.GetIl2CppExe();
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = (current, arg) => current + arg + " ";
            }
            string args = Enumerable.Aggregate<string, string>(arguments, string.Empty, <>f__am$cache2);
            Console.WriteLine("Invoking il2cpp with arguments: " + args);
            Runner.RunManagedProgram(exe, args, workingDirectory, new Il2CppOutputParser(), setupStartInfo);
        }

        [CompilerGenerated]
        private sealed class <FilterUserAssemblies>c__AnonStorey0
        {
            internal Predicate<string> isUsed;
            internal string managedDir;

            internal bool <>m__0(string assembly) => 
                this.isUsed(assembly);

            internal string <>m__1(string usedAssembly) => 
                Path.Combine(this.managedDir, usedAssembly);
        }
    }
}

