namespace Unity.IL2CPP.Building
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Building.BuildDescriptions;
    using Unity.IL2CPP.Building.Hashing;
    using Unity.IL2CPP.Building.Platforms;
    using Unity.IL2CPP.Building.Statistics;
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;
    using Unity.TinyProfiling;

    public sealed class CppProgramBuilder
    {
        private readonly CppToolChain _cppToolChain;
        private readonly bool _forceRebuild;
        private readonly NPath _globalObjectCacheDirectory;
        private readonly FileHashProvider _headerHashProvider;
        private readonly ProgramBuildDescription _programBuildDescription;
        private readonly bool _verbose;
        private readonly NPath _workingDirectory;
        [CompilerGenerated]
        private static Func<NPath, NPath> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<NPath, IEnumerable<NPath>> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<CppCompilationInstruction, long> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<IntermediateObjectFileCompilationData, ProvideObjectResult> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<CompilationResult, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<CompilationResult, TimeSpan> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<ProvideObjectResult, NPath> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<NPath, bool> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<NPath, string> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache9;

        public CppProgramBuilder(CppToolChain cppToolChain, ProgramBuildDescription programBuildDescription, bool verbose, bool forceRebuild)
        {
            this._verbose = verbose;
            this._forceRebuild = forceRebuild;
            this._programBuildDescription = programBuildDescription;
            this._cppToolChain = cppToolChain;
            if (programBuildDescription.GlobalCacheDirectory == null)
            {
            }
            this._workingDirectory = TempDir.Empty("workingdir_" + programBuildDescription.GetType().Name);
            this._globalObjectCacheDirectory = this._workingDirectory.EnsureDirectoryExists("globalcache");
            string[] fileExtensions = new string[] { ".h", ".inc" };
            this._headerHashProvider = new FileHashProvider(fileExtensions);
        }

        public NPath Build()
        {
            IBuildStatistics statistics;
            return this.Build(out statistics);
        }

        public NPath Build(out IBuildStatistics statistics)
        {
            CppProgramBuildStatistics statisticsCollector = new CppProgramBuildStatistics();
            object[] arg = new object[] { this._programBuildDescription.OutputFile.FileName, this._cppToolChain.GetType().Name, Environment.NewLine, this._programBuildDescription.OutputFile.Parent, this._workingDirectory };
            Console.WriteLine("Building {0} with {1}.{2}\tOutput directory: {3}{2}\tCache directory: {4}", arg);
            if (!this.CanBuildInCurrentEnvironment())
            {
                throw new InvalidOperationException($"Builder is unable to build using selected toolchain ({this._cppToolChain.GetType().Name}) or architecture ({this._cppToolChain.Architecture})!");
            }
            using (TinyProfiler.Section("BuildBinary", ""))
            {
                CppCompilationInstruction[] instructionArray;
                IEnumerable<NPath> enumerable;
                CppToolChainContext toolChainContext = this._cppToolChain.CreateToolChainContext();
                using (TinyProfiler.Section("ToolChain OnBeforeCompile Build", ""))
                {
                    this._cppToolChain.OnBeforeCompile(this._programBuildDescription, toolChainContext, this._workingDirectory, this._forceRebuild, this._verbose);
                }
                using (TinyProfiler.Section("FindFilesToCompile", ""))
                {
                    instructionArray = this._programBuildDescription.CppCompileInstructions.Concat<CppCompilationInstruction>(toolChainContext.ExtraCompileInstructions).ToArray<CppCompilationInstruction>();
                    foreach (CppCompilationInstruction instruction in instructionArray)
                    {
                        instruction.Defines = instruction.Defines.Concat<string>(this._cppToolChain.ToolChainDefines());
                        instruction.IncludePaths = instruction.IncludePaths.Concat<NPath>(this._cppToolChain.ToolChainIncludePaths()).Concat<NPath>(toolChainContext.ExtraIncludeDirectories);
                    }
                }
                using (TinyProfiler.Section("Calculate header hashes", ""))
                {
                    this._headerHashProvider.Initialize(instructionArray);
                }
                using (TinyProfiler.Section("BuildAllCppFiles", ""))
                {
                    enumerable = this.BuildAllCppFiles(instructionArray, statisticsCollector);
                }
                using (TinyProfiler.Section("OnBeforeLink Build", ""))
                {
                    this.OnBeforeLink(enumerable, toolChainContext);
                }
                using (TinyProfiler.Section("Postprocess Object Files", ""))
                {
                    this.PostprocessObjectFiles(enumerable, toolChainContext);
                }
                using (TinyProfiler.Section("ProgramDescription Finalize Build", ""))
                {
                    this._programBuildDescription.FinalizeBuild(this._cppToolChain);
                }
                using (TinyProfiler.Section("ToolChain OnAfterLink Build", ""))
                {
                    this._cppToolChain.OnAfterLink(this._programBuildDescription.OutputFile, toolChainContext, this._forceRebuild, this._verbose);
                }
                using (TinyProfiler.Section("Clean IL2CPP Cache", ""))
                {
                    this.CleanWorkingDirectory(enumerable);
                }
            }
            statistics = statisticsCollector;
            return this._programBuildDescription.OutputFile;
        }

        private IEnumerable<NPath> BuildAllCppFiles(IEnumerable<CppCompilationInstruction> sourceFilesToCompile, IBuildStatisticsCollector statisticsCollector)
        {
            using (TinyProfiler.Section("Compile", ""))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = f => new FileInfo(f.SourceFile.ToString()).Length;
                }
                CppCompilationInstruction[] instructionArray = sourceFilesToCompile.OrderByDescending<CppCompilationInstruction, long>(<>f__am$cache2).ToArray<CppCompilationInstruction>();
                statisticsCollector.IncrementTotalFileCountBy(instructionArray.Length);
                List<IntermediateObjectFileCompilationData> list = ParallelFor.RunWithResult<CppCompilationInstruction, IntermediateObjectFileCompilationData>(instructionArray, new Func<CppCompilationInstruction, IntermediateObjectFileCompilationData>(this.BuildIntermediateObjectFileData)).ToList<IntermediateObjectFileCompilationData>();
                List<IntermediateObjectFileCompilationData> source = new List<IntermediateObjectFileCompilationData>();
                List<IntermediateObjectFileCompilationData> list3 = new List<IntermediateObjectFileCompilationData>();
                foreach (IntermediateObjectFileCompilationData data in list)
                {
                    if (this.IsCached(data))
                    {
                        source.Add(data);
                    }
                    else
                    {
                        list3.Add(data);
                    }
                }
                statisticsCollector.IncrementCacheHitCountBy(source.Count);
                List<ProvideObjectResult> list4 = new List<ProvideObjectResult>();
                List<CompilationResult> list5 = new List<CompilationResult>();
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = d => new ProvideObjectResult { ObjectFile = d.ObjectFile };
                }
                list4.AddRange(source.Select<IntermediateObjectFileCompilationData, ProvideObjectResult>(<>f__am$cache3));
                if (list3.Count > 0)
                {
                    List<ProvideObjectResult> collection = ParallelFor.RunWithResult<IntermediateObjectFileCompilationData, ProvideObjectResult>(list3.ToArray(), new Func<IntermediateObjectFileCompilationData, ProvideObjectResult>(this.ProvideObjectFile)).ToList<ProvideObjectResult>();
                    list4.AddRange(collection);
                    list5.AddRange(collection.OfType<CompilationResult>());
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = cr => !cr.Success;
                    }
                    CompilationResult result = list5.FirstOrDefault<CompilationResult>(<>f__am$cache4);
                    if (result != null)
                    {
                        throw new BuilderFailedException(result.InterestingOutput + Environment.NewLine + "Invocation was: " + result.Invocation.Summary());
                    }
                }
                Console.WriteLine(string.Concat(new object[] { "ObjectFiles: ", list4.Count<ProvideObjectResult>(), " of which compiled: ", list5.Count }));
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = cr => cr.Duration;
                }
                foreach (CompilationResult result2 in list5.OrderByDescending<CompilationResult, TimeSpan>(<>f__am$cache5).Take<CompilationResult>(10))
                {
                    Console.WriteLine("\tTime Compile: {0} milliseconds {1}", result2.Duration.TotalMilliseconds, result2.Invocation.SourceFile.FileName);
                }
                Console.WriteLine("Total compilation time: {0} milliseconds.", stopwatch.ElapsedMilliseconds);
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = c => c.ObjectFile;
                }
                return list4.Select<ProvideObjectResult, NPath>(<>f__am$cache6).ToArray<NPath>();
            }
        }

        private BuilderFailedException BuilderFailedExceptionForFailedLinkerExecution(LinkerResult result, Shell.ExecuteArgs executableInvocation) => 
            new BuilderFailedException(string.Format("{0} {1}{2}{2}{3}", new object[] { executableInvocation.Executable, executableInvocation.Arguments, Environment.NewLine, result.InterestingOutput }));

        private IntermediateObjectFileCompilationData BuildIntermediateObjectFileData(CppCompilationInstruction cppCompilationInstruction)
        {
            string str;
            NPath path;
            CompilationInvocation invocation = new CompilationInvocation {
                CompilerExecutable = this._cppToolChain.CompilerExecutableFor(cppCompilationInstruction.SourceFile),
                SourceFile = cppCompilationInstruction.SourceFile,
                Arguments = this._cppToolChain.CompilerFlagsFor(cppCompilationInstruction),
                EnvVars = this._cppToolChain.EnvVars()
            };
            using (TinyProfiler.Section("HashCompilerInvocation", cppCompilationInstruction.SourceFile.FileName))
            {
                str = invocation.Hash(this._headerHashProvider.HashForAllHeaderFilesReachableByFilesIn(cppCompilationInstruction));
            }
            NPath cacheDirectory = cppCompilationInstruction.CacheDirectory;
            if (cacheDirectory != null)
            {
                path = cacheDirectory;
            }
            else
            {
                path = this._globalObjectCacheDirectory;
            }
            string[] append = new string[] { str };
            NPath path2 = path.Combine(append).ChangeExtension(this._cppToolChain.ObjectExtension());
            return new IntermediateObjectFileCompilationData { 
                CppCompilationInstruction = cppCompilationInstruction,
                CompilationInvocation = invocation,
                ObjectFile = path2
            };
        }

        public bool CanBuildInCurrentEnvironment() => 
            this._cppToolChain.CanBuildInCurrentEnvironment();

        private void CleanWorkingDirectory(IEnumerable<NPath> compiledObjectFiles)
        {
            <CleanWorkingDirectory>c__AnonStorey0 storey = new <CleanWorkingDirectory>c__AnonStorey0 {
                compiledObjectFiles = compiledObjectFiles
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = file => file.Parent;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = d => d.Files(false);
            }
            IEnumerable<NPath> source = storey.compiledObjectFiles.Select<NPath, NPath>(<>f__am$cache0).Distinct<NPath>().SelectMany<NPath, NPath>(<>f__am$cache1);
            NPath[] pathArray = source.Where<NPath>(new Func<NPath, bool>(storey.<>m__0)).ToArray<NPath>();
            foreach (NPath path in pathArray)
            {
                File.SetLastAccessTimeUtc(path.ToString(), DateTime.UtcNow);
            }
            if (!BuildingTestRunnerHelper.SkipCleaningCacheAfterCppBuild)
            {
                NPath[] pathArray3 = source.Where<NPath>(new Func<NPath, bool>(storey.<>m__1)).ToArray<NPath>();
                foreach (NPath path2 in pathArray3)
                {
                    try
                    {
                        path2.Delete(DeleteMode.Normal);
                    }
                    catch (IOException)
                    {
                    }
                }
                if (this._programBuildDescription.GlobalCacheDirectory == null)
                {
                    this._workingDirectory.Delete(DeleteMode.Soft);
                }
                Console.WriteLine("Cleaned up {0} object files.", pathArray3.Length);
            }
        }

        public static CppToolChain CppToolChainFor(RuntimePlatform platform, Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors) => 
            PlatformSupport.For(platform).MakeCppToolChain(architecture, buildConfiguration, treatWarningsAsErrors);

        public static CppProgramBuilder Create(RuntimePlatform platform, ProgramBuildDescription programBuildDescription, bool verbose, Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration, bool forceRebuild, bool treatWarningsAsErrors)
        {
            PlatformSupport support = PlatformSupport.For(platform);
            return new CppProgramBuilder(CppToolChainFor(platform, architecture, buildConfiguration, treatWarningsAsErrors), support.PostProcessProgramBuildDescription(programBuildDescription), verbose, forceRebuild);
        }

        private NPath FindStaticLibrary(NPath staticLib)
        {
            NPath path;
            <FindStaticLibrary>c__AnonStorey5 storey = new <FindStaticLibrary>c__AnonStorey5 {
                staticLib = staticLib
            };
            try
            {
                if (<>f__am$cache7 == null)
                {
                    <>f__am$cache7 = p => p.FileExists("");
                }
                path = this._cppToolChain.ToolChainLibraryPaths().Select<NPath, NPath>(new Func<NPath, NPath>(storey.<>m__0)).Single<NPath>(<>f__am$cache7);
            }
            catch
            {
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = p => p.ToString();
                }
                if (<>f__am$cache9 == null)
                {
                    <>f__am$cache9 = (x, y) => $"{x}{Environment.NewLine}	{y}";
                }
                throw new Exception($"Could not locate the exact path of {storey.staticLib} inside these directories:{Environment.NewLine}	{this._cppToolChain.ToolChainLibraryPaths().Select<NPath, string>(<>f__am$cache8).Aggregate<string>(<>f__am$cache9)}");
            }
            return path;
        }

        private string HashLinkerInvocation(LinkerInvocation linkerInvocation, IEnumerable<NPath> objectFiles)
        {
            <HashLinkerInvocation>c__AnonStorey3 storey = new <HashLinkerInvocation>c__AnonStorey3 {
                objectFiles = objectFiles
            };
            using (TinyProfiler.Section("hash linker invocation", ""))
            {
                StringBuilder builder = new StringBuilder();
                foreach (string str in linkerInvocation.ArgumentsInfluencingOutcome)
                {
                    builder.Append(str);
                }
                builder.Append(linkerInvocation.ExecuteArgs.Executable);
                foreach (NPath path in storey.objectFiles)
                {
                    builder.Append(path.FileName);
                }
                foreach (NPath path2 in linkerInvocation.FilesInfluencingOutcome.Where<NPath>(new Func<NPath, bool>(storey.<>m__0)))
                {
                    builder.Append(HashTools.HashOfFile(!path2.IsRelative ? path2 : this.FindStaticLibrary(path2)));
                }
                return HashTools.HashOf(builder.ToString());
            }
        }

        private bool IsCached(IntermediateObjectFileCompilationData data) => 
            (((data.CppCompilationInstruction.CacheDirectory != null) && !this._forceRebuild) && data.ObjectFile.FileExists(""));

        private void OnBeforeLink(IEnumerable<NPath> objectFiles, CppToolChainContext toolChainContext)
        {
            using (TinyProfiler.Section("ToolChain OnBeforeLink Build", ""))
            {
                this._cppToolChain.OnBeforeLink(this._programBuildDescription, this._workingDirectory, objectFiles, toolChainContext, this._forceRebuild, this._verbose);
            }
            using (TinyProfiler.Section("ProgramBuildDescription OnBeforeLink Build", ""))
            {
                this._programBuildDescription.OnBeforeLink(this._workingDirectory, objectFiles, toolChainContext, this._forceRebuild, this._verbose);
            }
        }

        private void PostprocessObjectFiles(IEnumerable<NPath> objectFiles, CppToolChainContext toolChainContext)
        {
            using (TinyProfiler.Section("link", ""))
            {
                this._programBuildDescription.OutputFile.EnsureParentDirectoryExists();
                LinkerInvocation linkerInvocation = this._cppToolChain.MakeLinkerInvocation(objectFiles, this._programBuildDescription.OutputFile, this._programBuildDescription.StaticLibraries, this._programBuildDescription.DynamicLibraries, this._programBuildDescription.AdditionalLinkerFlags, toolChainContext);
                string str = this.HashLinkerInvocation(linkerInvocation, objectFiles);
                string[] append = new string[] { "linkresult_" + str };
                NPath path = this._workingDirectory.Combine(append);
                if (!this._forceRebuild && path.DirectoryExists(""))
                {
                    path.Files(false).Copy(this._programBuildDescription.OutputFile.Parent);
                }
                else
                {
                    Shell.ExecuteResult result;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    foreach (NPath path2 in this._workingDirectory.Directories("linkresult_*", false))
                    {
                        path2.Delete(DeleteMode.Normal);
                    }
                    path.EnsureDirectoryExists("");
                    string[] textArray2 = new string[] { this._programBuildDescription.OutputFile.FileName };
                    NPath outputFile = path.Combine(textArray2);
                    LinkerInvocation invocation2 = this._cppToolChain.MakeLinkerInvocation(objectFiles, outputFile, this._programBuildDescription.StaticLibraries, this._programBuildDescription.DynamicLibraries, this._programBuildDescription.AdditionalLinkerFlags, toolChainContext);
                    using (TinyProfiler.Section("link", ""))
                    {
                        result = Shell.Execute(invocation2.ExecuteArgs, null);
                    }
                    LinkerResult result2 = this._cppToolChain.ShellResultToLinkerResult(result);
                    if (!result2.Success)
                    {
                        throw this.BuilderFailedExceptionForFailedLinkerExecution(result2, invocation2.ExecuteArgs);
                    }
                    path.Files(false).Copy(this._programBuildDescription.OutputFile.Parent);
                    if (this._verbose)
                    {
                        Console.WriteLine(result.StdOut.Trim());
                    }
                    Console.WriteLine("Total link time: {0} milliseconds.", stopwatch.ElapsedMilliseconds);
                }
            }
        }

        private ProvideObjectResult ProvideObjectFile(IntermediateObjectFileCompilationData data)
        {
            Shell.ExecuteResult result;
            data.CompilationInvocation.Arguments = data.CompilationInvocation.Arguments.Concat<string>(this._cppToolChain.OutputArgumentFor(data.ObjectFile));
            using (TinyProfiler.Section("Compile", data.CppCompilationInstruction.SourceFile.FileName))
            {
                result = data.CompilationInvocation.Execute();
            }
            CompilationResult result2 = this._cppToolChain.ShellResultToCompilationResult(result);
            result2.Invocation = data.CompilationInvocation;
            result2.ObjectFile = data.ObjectFile;
            if ((result2.Success && this._verbose) && !string.IsNullOrWhiteSpace(result.StdOut))
            {
                Console.WriteLine(result.StdOut.Trim());
            }
            return result2;
        }

        [CompilerGenerated]
        private sealed class <CleanWorkingDirectory>c__AnonStorey0
        {
            internal IEnumerable<NPath> compiledObjectFiles;

            internal bool <>m__0(NPath objectFile)
            {
                <CleanWorkingDirectory>c__AnonStorey1 storey = new <CleanWorkingDirectory>c__AnonStorey1 {
                    <>f__ref$0 = this,
                    objectFile = objectFile
                };
                return this.compiledObjectFiles.Any<NPath>(new Func<NPath, bool>(storey.<>m__0));
            }

            internal bool <>m__1(NPath objectFile)
            {
                <CleanWorkingDirectory>c__AnonStorey2 storey = new <CleanWorkingDirectory>c__AnonStorey2 {
                    <>f__ref$0 = this,
                    objectFile = objectFile
                };
                return !this.compiledObjectFiles.Any<NPath>(new Func<NPath, bool>(storey.<>m__0));
            }

            private sealed class <CleanWorkingDirectory>c__AnonStorey1
            {
                internal CppProgramBuilder.<CleanWorkingDirectory>c__AnonStorey0 <>f__ref$0;
                internal NPath objectFile;

                internal bool <>m__0(NPath compiledObjectFile) => 
                    string.Equals(this.objectFile.FileNameWithoutExtension, compiledObjectFile.FileNameWithoutExtension, StringComparison.OrdinalIgnoreCase);
            }

            private sealed class <CleanWorkingDirectory>c__AnonStorey2
            {
                internal CppProgramBuilder.<CleanWorkingDirectory>c__AnonStorey0 <>f__ref$0;
                internal NPath objectFile;

                internal bool <>m__0(NPath compiledObjectFile) => 
                    string.Equals(this.objectFile.FileNameWithoutExtension, compiledObjectFile.FileNameWithoutExtension, StringComparison.OrdinalIgnoreCase);
            }
        }

        [CompilerGenerated]
        private sealed class <FindStaticLibrary>c__AnonStorey5
        {
            internal NPath staticLib;

            internal NPath <>m__0(NPath p)
            {
                NPath[] append = new NPath[] { this.staticLib };
                return p.Combine(append);
            }
        }

        [CompilerGenerated]
        private sealed class <HashLinkerInvocation>c__AnonStorey3
        {
            internal IEnumerable<NPath> objectFiles;

            internal bool <>m__0(NPath file)
            {
                <HashLinkerInvocation>c__AnonStorey4 storey = new <HashLinkerInvocation>c__AnonStorey4 {
                    <>f__ref$3 = this,
                    file = file
                };
                return !this.objectFiles.Any<NPath>(new Func<NPath, bool>(storey.<>m__0));
            }

            private sealed class <HashLinkerInvocation>c__AnonStorey4
            {
                internal CppProgramBuilder.<HashLinkerInvocation>c__AnonStorey3 <>f__ref$3;
                internal NPath file;

                internal bool <>m__0(NPath o) => 
                    (this.file == o);
            }
        }

        public class LinkerInvocation
        {
            public List<string> ArgumentsInfluencingOutcome;
            public Unity.IL2CPP.Shell.ExecuteArgs ExecuteArgs;
            public IEnumerable<NPath> FilesInfluencingOutcome;
        }
    }
}

