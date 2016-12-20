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
        private static Func<string, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache3;
        private readonly bool m_DevelopmentBuild;
        private readonly LinkXmlReader m_linkXmlReader = new LinkXmlReader();
        private readonly Action<string> m_ModifyOutputBeforeCompile;
        private readonly IIl2CppPlatformProvider m_PlatformProvider;
        private readonly RuntimeClassRegistry m_RuntimeClassRegistry;
        private readonly string m_StagingAreaData;
        private readonly string m_TempFolder;

        public IL2CPPBuilder(string tempFolder, string stagingAreaData, IIl2CppPlatformProvider platformProvider, Action<string> modifyOutputBeforeCompile, RuntimeClassRegistry runtimeClassRegistry, bool developmentBuild)
        {
            this.m_TempFolder = tempFolder;
            this.m_StagingAreaData = stagingAreaData;
            this.m_PlatformProvider = platformProvider;
            this.m_ModifyOutputBeforeCompile = modifyOutputBeforeCompile;
            this.m_RuntimeClassRegistry = runtimeClassRegistry;
            this.m_DevelopmentBuild = developmentBuild;
        }

        private void ConvertPlayerDlltoCpp(ICollection<string> userAssemblies, string outputDirectory, string workingDirectory)
        {
            if (userAssemblies.Count != 0)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<string, string>(null, (IntPtr) <ConvertPlayerDlltoCpp>m__1);
                }
                string[] strArray = Enumerable.ToArray<string>(Enumerable.Select<string, string>(Directory.GetFiles("Assets", "il2cpp_extra_types.txt", SearchOption.AllDirectories), <>f__am$cache1));
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
                if (this.m_PlatformProvider.loadSymbols)
                {
                    arguments.Add("--enable-symbol-loading");
                }
                if (this.m_PlatformProvider.developmentMode)
                {
                    arguments.Add("--development-mode");
                }
                Il2CppNativeCodeBuilder builder = this.m_PlatformProvider.CreateIl2CppNativeCodeBuilder();
                if (builder != null)
                {
                    Il2CppNativeCodeBuilderUtils.ClearAndPrepareCacheDirectory(builder);
                    arguments.AddRange(Il2CppNativeCodeBuilderUtils.AddBuilderArguments(builder, this.OutputFileRelativePath(), this.m_PlatformProvider.includePaths));
                }
                arguments.Add(string.Format("--map-file-parser=\"{0}\"", GetMapFileParserPath()));
                if (strArray.Length > 0)
                {
                    foreach (string str in strArray)
                    {
                        arguments.Add(string.Format("--extra-types.file=\"{0}\"", str));
                    }
                }
                string path = Path.Combine(this.m_PlatformProvider.il2CppFolder, "il2cpp_default_extra_types.txt");
                if (File.Exists(path))
                {
                    arguments.Add(string.Format("--extra-types.file=\"{0}\"", path));
                }
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
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = new Func<string, string>(null, (IntPtr) <ConvertPlayerDlltoCpp>m__2);
                }
                arguments.AddRange(Enumerable.Select<string, string>(list2, <>f__am$cache2));
                arguments.Add(string.Format("--generatedcppdir=\"{0}\"", Path.GetFullPath(outputDirectory)));
                if (EditorUtility.DisplayCancelableProgressBar("Building Player", "Converting managed assemblies to C++", 0.3f))
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
            return Enumerable.Select<string, string>(Enumerable.Where<string>(assemblies, new Func<string, bool>(storey, (IntPtr) this.<>m__0)), new Func<string, string>(storey, (IntPtr) this.<>m__1));
        }

        public string GetCppOutputDirectoryInStagingArea()
        {
            return GetCppOutputPath(this.m_TempFolder);
        }

        public static string GetCppOutputPath(string tempFolder)
        {
            return Path.Combine(tempFolder, "il2cppOutput");
        }

        private string GetIl2CppExe()
        {
            return (this.m_PlatformProvider.il2CppFolder + "/build/il2cpp.exe");
        }

        private static string GetMapFileParserPath()
        {
            return Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, (Application.platform != RuntimePlatform.WindowsEditor) ? "Tools/MapFileParser/MapFileParser" : @"Tools\MapFileParser\MapFileParser.exe"));
        }

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
            userAssemblies.Add(Enumerable.Single<string>(Directory.GetFiles(managedDir, "UnityEngine.dll", SearchOption.TopDirectoryOnly)));
            userAssemblies.UnionWith(this.FilterUserAssemblies(Directory.GetFiles(managedDir, "*.dll", SearchOption.TopDirectoryOnly), new Predicate<string>(this.m_linkXmlReader.IsDLLUsed), managedDir));
            return Enumerable.ToList<string>(userAssemblies);
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
            AssemblyStripper.StripAssemblies(this.m_StagingAreaData, this.m_PlatformProvider, this.m_RuntimeClassRegistry, this.m_DevelopmentBuild);
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
                List<string> arguments = Enumerable.ToList<string>(Il2CppNativeCodeBuilderUtils.AddBuilderArguments(builder, this.OutputFileRelativePath(), this.m_PlatformProvider.includePaths));
                arguments.Add(string.Format("--map-file-parser=\"{0}\"", GetMapFileParserPath()));
                arguments.Add(string.Format("--generatedcppdir=\"{0}\"", Path.GetFullPath(this.GetCppOutputDirectoryInStagingArea())));
                Action<ProcessStartInfo> setupStartInfo = new Action<ProcessStartInfo>(builder.SetupStartInfo);
                string fullPath = Path.GetFullPath(Path.Combine(this.m_StagingAreaData, "Managed"));
                this.RunIl2CppWithArguments(arguments, setupStartInfo, fullPath);
            }
        }

        private void RunIl2CppWithArguments(List<string> arguments, Action<ProcessStartInfo> setupStartInfo, string workingDirectory)
        {
            string exe = this.GetIl2CppExe();
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<string, string, string>(null, (IntPtr) <RunIl2CppWithArguments>m__3);
            }
            string args = Enumerable.Aggregate<string, string>(arguments, string.Empty, <>f__am$cache3);
            Console.WriteLine("Invoking il2cpp with arguments: " + args);
            Runner.RunManagedProgram(exe, args, workingDirectory, new Il2CppOutputParser(), setupStartInfo);
        }

        [CompilerGenerated]
        private sealed class <FilterUserAssemblies>c__AnonStorey0
        {
            internal Predicate<string> isUsed;
            internal string managedDir;

            internal bool <>m__0(string assembly)
            {
                return this.isUsed(assembly);
            }

            internal string <>m__1(string usedAssembly)
            {
                return Path.Combine(this.managedDir, usedAssembly);
            }
        }
    }
}

