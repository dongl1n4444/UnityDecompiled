namespace Unity.IL2CPP.Building.ToolChains
{
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.BuildDescriptions;
    using Unity.IL2CPP.Building.Hashing;
    using Unity.IL2CPP.Building.ToolChains.MsvcVersions;
    using Unity.IL2CPP.Common;
    using Unity.TinyProfiling;

    public abstract class MsvcToolChain : CppToolChain
    {
        private FileHashProvider _idlHashProvider;
        [CompilerGenerated]
        private static Func<NPath, IEnumerable<NPath>> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<NPath, IEnumerable<NPath>> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<NPath, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache3;
        protected Func<string, IEnumerable<string>> AdditionalCompilerOptionsForSourceFile;

        protected MsvcToolChain(Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration) : base(architecture, buildConfiguration)
        {
            string[] fileExtensions = new string[] { ".idl" };
            this._idlHashProvider = new FileHashProvider(fileExtensions);
        }

        public override bool CanBuildInCurrentEnvironment()
        {
            try
            {
                return (this.MsvcInstallation != null);
            }
            catch
            {
                return false;
            }
        }

        private void CompileCOMManifest(ProgramBuildDescription programBuildDescription, CppToolChainContext toolChainContext, NPath workingDirectory, bool forceRebuild, bool verbose)
        {
            string[] extensions = new string[] { ".dll" };
            if (programBuildDescription.OutputFile.HasExtension(extensions))
            {
                NPath path = programBuildDescription.OutputFile.ChangeExtension(".tlb");
                if (path.Exists(""))
                {
                    string hashForAllHeaderFilesPossiblyInfluencingCompilation = HashTools.HashOfFile(path);
                    CompilationInvocation invocation = new CompilationInvocation {
                        CompilerExecutable = this.MsvcInstallation.GetSDKToolPath("mt.exe"),
                        SourceFile = path,
                        Arguments = this.ManifestCompilerArgumentsFor(path, programBuildDescription.OutputFile)
                    };
                    string[] append = new string[] { invocation.Hash(hashForAllHeaderFilesPossiblyInfluencingCompilation) + ".manifest" };
                    NPath path2 = workingDirectory.Combine(append);
                    MsvcToolChainContext context = (MsvcToolChainContext) toolChainContext;
                    context.ManifestPath = path2;
                    if (!path2.Exists("") || forceRebuild)
                    {
                        Shell.ExecuteResult result;
                        invocation.Arguments = EnumerableExtensions.Append<string>(invocation.Arguments, this.ManifestOutputArgumentFor(path2));
                        using (TinyProfiler.Section("Compile manifest", path2.ToString()))
                        {
                            result = invocation.Execute();
                        }
                        if (result.ExitCode != 0)
                        {
                            throw new BuilderFailedException(string.Format(result.StdOut + "{0}{0}Invocation was: " + invocation.Summary(), Environment.NewLine));
                        }
                        if (verbose)
                        {
                            Console.WriteLine(result.StdOut.Trim());
                        }
                    }
                }
            }
        }

        private void CompileIDL(ProgramBuildDescription programBuildDescription, CppToolChainContext toolChainContext, NPath workingDirectory, bool forceRebuild, bool verbose)
        {
            <CompileIDL>c__AnonStoreyB yb = new <CompileIDL>c__AnonStoreyB {
                forceRebuild = forceRebuild,
                programBuildDescription = programBuildDescription,
                verbose = verbose,
                $this = this
            };
            string[] extensions = new string[] { ".dll" };
            if (yb.programBuildDescription.OutputFile.HasExtension(extensions))
            {
                IHaveSourceDirectories directories = yb.programBuildDescription as IHaveSourceDirectories;
                if (directories != null)
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = new Func<NPath, IEnumerable<NPath>>(null, (IntPtr) <CompileIDL>m__0);
                    }
                    NPath[] data = Enumerable.ToArray<NPath>(Enumerable.SelectMany<NPath, NPath>(directories.SourceDirectories, <>f__am$cache0));
                    if (data.Length != 0)
                    {
                        yb.idlResultDirectory = workingDirectory.EnsureDirectoryExists("idlGenerated");
                        yb.includePaths = this.ToolChainIncludePaths();
                        IEnumerable<IdlCompilationResult> enumerable = ParallelFor.RunWithResult<NPath, IdlCompilationResult>(data, new Func<NPath, IdlCompilationResult>(yb, (IntPtr) this.<>m__0));
                        MsvcToolChainContext context = (MsvcToolChainContext) toolChainContext;
                        foreach (IdlCompilationResult result in enumerable)
                        {
                            if (!result.Success)
                            {
                                throw new BuilderFailedException(string.Format(result.StdOut + "{0}{0}Invocation was: " + result.Invocation.Summary(), Environment.NewLine));
                            }
                            context.AddCompileInstructions(Enumerable.Select<NPath, CppCompilationInstruction>(result.OutputDirectory.Files("*_i.c", false), new Func<NPath, CppCompilationInstruction>(yb, (IntPtr) this.<>m__1)));
                            context.AddIncludeDirectory(result.OutputDirectory);
                            foreach (NPath path in result.OutputDirectory.Files("*.tlb", false))
                            {
                                string[] append = new string[] { path.FileName };
                                path.Copy(yb.programBuildDescription.OutputFile.Parent.Combine(append));
                            }
                            foreach (NPath path2 in result.OutputDirectory.Files("*.winmd", false))
                            {
                                string[] textArray3 = new string[] { path2.FileName };
                                path2.Copy(yb.programBuildDescription.OutputFile.Parent.Combine(textArray3));
                            }
                        }
                    }
                }
            }
        }

        public override NPath CompilerExecutableFor(NPath sourceFile)
        {
            return this.MsvcInstallation.GetVSToolPath(base.Architecture, "cl.exe");
        }

        [DebuggerHidden]
        public override IEnumerable<string> CompilerFlagsFor(CppCompilationInstruction cppCompilationInstruction)
        {
            return new <CompilerFlagsFor>c__Iterator4 { 
                cppCompilationInstruction = cppCompilationInstruction,
                $this = this,
                $PC = -2
            };
        }

        public override CppToolChainContext CreateToolChainContext()
        {
            return new MsvcToolChainContext();
        }

        [DebuggerHidden]
        protected virtual IEnumerable<string> DefaultCompilerFlags(CppCompilationInstruction cppCompilationInstruction)
        {
            return new <DefaultCompilerFlags>c__Iterator5 { 
                cppCompilationInstruction = cppCompilationInstruction,
                $this = this,
                $PC = -2
            };
        }

        public override bool DynamicLibrariesHaveToSitNextToExecutable()
        {
            return true;
        }

        public override Dictionary<string, string> EnvVars()
        {
            return new Dictionary<string, string> { { 
                "PATH",
                this.MsvcInstallation.GetPathEnvVariable(base.Architecture)
            } };
        }

        public override string ExecutableExtension()
        {
            return ".exe";
        }

        private void FindModuleDefinitionFiles(ProgramBuildDescription programBuildDescription, CppToolChainContext toolChainContext)
        {
            string[] extensions = new string[] { ".dll" };
            if (programBuildDescription.OutputFile.HasExtension(extensions))
            {
                IHaveSourceDirectories directories = programBuildDescription as IHaveSourceDirectories;
                if (directories != null)
                {
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = new Func<NPath, IEnumerable<NPath>>(null, (IntPtr) <FindModuleDefinitionFiles>m__1);
                    }
                    NPath[] source = Enumerable.ToArray<NPath>(Enumerable.SelectMany<NPath, NPath>(directories.SourceDirectories, <>f__am$cache1));
                    if (source.Length != 0)
                    {
                        if (source.Length > 1)
                        {
                            if (<>f__am$cache2 == null)
                            {
                                <>f__am$cache2 = new Func<NPath, string>(null, (IntPtr) <FindModuleDefinitionFiles>m__2);
                            }
                            if (<>f__am$cache3 == null)
                            {
                                <>f__am$cache3 = new Func<string, string, string>(null, (IntPtr) <FindModuleDefinitionFiles>m__3);
                            }
                            throw new BuilderFailedException(string.Format("Found more than one module definition file in source directories:{0}\t{1}", Environment.NewLine, Enumerable.Aggregate<string>(Enumerable.Select<NPath, string>(source, <>f__am$cache2), <>f__am$cache3)));
                        }
                        MsvcToolChainContext context = (MsvcToolChainContext) toolChainContext;
                        context.ModuleDefinitionPath = Enumerable.Single<NPath>(source);
                    }
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerable<string> FlagsToMakeWarningsErrorsFor(string sourceFile)
        {
            return new <FlagsToMakeWarningsErrorsFor>c__Iterator9 { $PC = -2 };
        }

        [DebuggerHidden]
        protected virtual IEnumerable<string> GetDefaultLinkerArgs(IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, NPath outputFile)
        {
            return new <GetDefaultLinkerArgs>c__Iterator3 { 
                outputFile = outputFile,
                staticLibraries = staticLibraries,
                dynamicLibraries = dynamicLibraries,
                $this = this,
                $PC = -2
            };
        }

        [DebuggerHidden]
        private IEnumerable<string> IdlCompilerArgumentsFor(NPath idl)
        {
            return new <IdlCompilerArgumentsFor>c__Iterator6 { 
                idl = idl,
                $this = this,
                $PC = -2
            };
        }

        [DebuggerHidden]
        private IEnumerable<string> IdlOutputArgumentsFor(NPath outputDir, NPath outputTlbPath, NPath outputWinmdPath)
        {
            return new <IdlOutputArgumentsFor>c__Iterator7 { 
                outputDir = outputDir,
                outputTlbPath = outputTlbPath,
                outputWinmdPath = outputWinmdPath,
                $PC = -2
            };
        }

        public override CppProgramBuilder.LinkerInvocation MakeLinkerInvocation(IEnumerable<NPath> objectFiles, NPath outputFile, IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, IEnumerable<string> specifiedLinkerFlags, CppToolChainContext toolChainContext)
        {
            <MakeLinkerInvocation>c__AnonStoreyA ya = new <MakeLinkerInvocation>c__AnonStoreyA();
            List<NPath> list = new List<NPath>(objectFiles);
            list.AddRange(staticLibraries);
            MsvcToolChainContext context = (MsvcToolChainContext) toolChainContext;
            string str = outputFile.ExtensionWithDot.ToLower();
            if ((str != ".dll") && (str != ".exe"))
            {
                string[] append = new string[] { outputFile.FileName + ".exe" };
                outputFile = outputFile.Parent.Combine(append);
            }
            List<string> inputs = new List<string> {
                "/out:" + outputFile.InQuotes()
            };
            if (context.ManifestPath != null)
            {
                inputs.Add("/MANIFESTUAC:NO");
                inputs.Add("/MANIFEST:EMBED");
                inputs.Add(string.Format("/MANIFESTINPUT:{0}", context.ManifestPath.InQuotes()));
                list.Add(context.ManifestPath);
            }
            if (context.ModuleDefinitionPath != null)
            {
                inputs.Add(string.Format("/DEF:{0}", context.ModuleDefinitionPath.InQuotes()));
                list.Add(context.ModuleDefinitionPath);
            }
            ya.bestWorkingDirectory = PickBestDirectoryToUseAsWorkingDirectoryFromObjectFilePaths(objectFiles);
            IEnumerable<string> enumerable = Enumerable.Select<NPath, string>(objectFiles, new Func<NPath, string>(ya, (IntPtr) this.<>m__0));
            inputs.AddRange(base.ChooseLinkerFlags(staticLibraries, dynamicLibraries, outputFile, specifiedLinkerFlags, new Func<IEnumerable<NPath>, IEnumerable<NPath>, NPath, IEnumerable<string>>(this, (IntPtr) this.GetDefaultLinkerArgs)));
            string tempFileName = Path.GetTempFileName();
            File.WriteAllText(tempFileName, ExtensionMethods.SeparateWithSpaces(ExtensionMethods.InQuotes(enumerable)), Encoding.UTF8);
            CppProgramBuilder.LinkerInvocation invocation = new CppProgramBuilder.LinkerInvocation();
            Shell.ExecuteArgs args = new Shell.ExecuteArgs {
                Arguments = ExtensionMethods.SeparateWithSpaces(EnumerableExtensions.Append<string>(inputs, "@" + ExtensionMethods.InQuotes(tempFileName))),
                Executable = this.MsvcInstallation.GetVSToolPath(base.Architecture, "link.exe").ToString(),
                EnvVars = this.EnvVars(),
                WorkingDirectory = ya.bestWorkingDirectory.ToString()
            };
            invocation.ExecuteArgs = args;
            invocation.ArgumentsInfluencingOutcome = inputs;
            invocation.FilesInfluencingOutcome = list;
            return invocation;
        }

        [DebuggerHidden]
        private IEnumerable<string> ManifestCompilerArgumentsFor(NPath tlbFile, NPath outputFile)
        {
            return new <ManifestCompilerArgumentsFor>c__Iterator8 { 
                outputFile = outputFile,
                tlbFile = tlbFile,
                $PC = -2
            };
        }

        private string ManifestOutputArgumentFor(NPath path)
        {
            return string.Format("/out:{0}", path.InQuotes());
        }

        public override string ObjectExtension()
        {
            return ".obj";
        }

        public override void OnBeforeCompile(ProgramBuildDescription programBuildDescription, CppToolChainContext toolChainContext, NPath workingDirectory, bool forceRebuild, bool verbose)
        {
            base.OnBeforeCompile(programBuildDescription, toolChainContext, workingDirectory, forceRebuild, verbose);
            this.CompileIDL(programBuildDescription, toolChainContext, workingDirectory, forceRebuild, verbose);
            this.CompileCOMManifest(programBuildDescription, toolChainContext, workingDirectory, forceRebuild, verbose);
            this.FindModuleDefinitionFiles(programBuildDescription, toolChainContext);
        }

        [DebuggerHidden]
        public override IEnumerable<string> OutputArgumentFor(NPath objectFile)
        {
            return new <OutputArgumentFor>c__Iterator2 { 
                objectFile = objectFile,
                $PC = -2
            };
        }

        private static NPath PickBestDirectoryToUseAsWorkingDirectoryFromObjectFilePaths(IEnumerable<NPath> objectFiles)
        {
            Dictionary<NPath, int> dictionary = new Dictionary<NPath, int>();
            foreach (NPath path in objectFiles)
            {
                if (!path.IsRelative)
                {
                    int num;
                    if (!dictionary.TryGetValue(path.Parent, out num))
                    {
                        dictionary.Add(path.Parent, 1);
                    }
                    else
                    {
                        Dictionary<NPath, int> dictionary2;
                        NPath path2;
                        (dictionary2 = dictionary)[path2 = path.Parent] = dictionary2[path2] + 1;
                    }
                }
            }
            int num2 = -2147483648;
            NPath key = null;
            foreach (KeyValuePair<NPath, int> pair in dictionary)
            {
                int num3 = pair.Value & pair.Key.ToString().Length;
                if (num3 > num2)
                {
                    num2 = num3;
                    key = pair.Key;
                }
            }
            return key;
        }

        public override IEnumerable<Type> SupportedArchitectures()
        {
            return this.MsvcInstallation.SupportedArchitectures;
        }

        [DebuggerHidden]
        public override IEnumerable<string> ToolChainDefines()
        {
            return new <ToolChainDefines>c__Iterator0 { 
                $this = this,
                $PC = -2
            };
        }

        public override IEnumerable<NPath> ToolChainIncludePaths()
        {
            return this.MsvcInstallation.GetIncludeDirectories();
        }

        public override IEnumerable<NPath> ToolChainLibraryPaths()
        {
            return this.MsvcInstallation.GetLibDirectories(base.Architecture, null);
        }

        [DebuggerHidden]
        public override IEnumerable<string> ToolChainStaticLibraries()
        {
            return new <ToolChainStaticLibraries>c__Iterator1 { 
                $this = this,
                $PC = -2
            };
        }

        public override string DynamicLibraryExtension
        {
            get
            {
                return ".dll";
            }
        }

        public override string MapFileParserFormat
        {
            get
            {
                return "MSVC";
            }
        }

        public abstract Unity.IL2CPP.Building.ToolChains.MsvcVersions.MsvcInstallation MsvcInstallation { get; }

        public override bool SupportsMapFileParser
        {
            get
            {
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <CompileIDL>c__AnonStoreyB
        {
            internal MsvcToolChain $this;
            internal bool forceRebuild;
            internal NPath idlResultDirectory;
            internal IEnumerable<NPath> includePaths;
            internal ProgramBuildDescription programBuildDescription;
            internal bool verbose;

            internal MsvcToolChain.IdlCompilationResult <>m__0(NPath idl)
            {
                Shell.ExecuteResult result2;
                string hashForAllHeaderFilesPossiblyInfluencingCompilation = this.$this._idlHashProvider.HashForAllIncludableFilesInDirectories(EnumerableExtensions.Append<NPath>(this.includePaths, idl.Parent));
                CompilationInvocation invocation = new CompilationInvocation {
                    CompilerExecutable = this.$this.MsvcInstallation.GetSDKToolPath("midl.exe"),
                    SourceFile = idl,
                    EnvVars = this.$this.EnvVars(),
                    Arguments = this.$this.IdlCompilerArgumentsFor(idl)
                };
                string[] append = new string[] { invocation.Hash(hashForAllHeaderFilesPossiblyInfluencingCompilation) };
                NPath outputDirectory = this.idlResultDirectory.Combine(append);
                if ((outputDirectory.DirectoryExists("") && (Enumerable.Count<NPath>(outputDirectory.Files(false)) > 0)) && !this.forceRebuild)
                {
                    return MsvcToolChain.IdlCompilationResult.SuccessfulResult(outputDirectory);
                }
                outputDirectory.EnsureDirectoryExists("");
                string[] textArray2 = new string[] { this.programBuildDescription.OutputFile.ChangeExtension(".tlb").FileName };
                NPath outputTlbPath = outputDirectory.Combine(textArray2);
                string[] textArray3 = new string[] { this.programBuildDescription.OutputFile.ChangeExtension(".winmd").FileName };
                NPath outputWinmdPath = outputDirectory.Combine(textArray3);
                invocation.Arguments = Enumerable.Concat<string>(invocation.Arguments, this.$this.IdlOutputArgumentsFor(outputDirectory, outputTlbPath, outputWinmdPath));
                using (TinyProfiler.Section("Compile IDL", idl.ToString()))
                {
                    result2 = invocation.Execute();
                }
                MsvcToolChain.IdlCompilationResult result3 = MsvcToolChain.IdlCompilationResult.FromShellExecuteResult(result2, invocation, outputDirectory);
                if (result3.Success && this.verbose)
                {
                    Console.WriteLine(result3.StdOut.Trim());
                }
                return result3;
            }

            internal CppCompilationInstruction <>m__1(NPath sourceFile)
            {
                return new CppCompilationInstruction { 
                    SourceFile = sourceFile,
                    CompilerFlags = this.programBuildDescription.AdditionalCompilerFlags,
                    IncludePaths = this.programBuildDescription.AdditionalIncludePathsFor(sourceFile),
                    Defines = this.programBuildDescription.AdditionalDefinesFor(sourceFile)
                };
            }
        }

        [CompilerGenerated]
        private sealed class <CompilerFlagsFor>c__Iterator4 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal IEnumerator<NPath> $locvar1;
            internal IEnumerator<string> $locvar2;
            internal int $PC;
            internal MsvcToolChain $this;
            internal string <compilerFlag>__2;
            internal string <define>__0;
            internal NPath <includePath>__1;
            internal CppCompilationInstruction cppCompilationInstruction;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar0 != null)
                            {
                                this.$locvar0.Dispose();
                            }
                        }
                        break;

                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar1 != null)
                            {
                                this.$locvar1.Dispose();
                            }
                        }
                        break;

                    case 3:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar2 != null)
                            {
                                this.$locvar2.Dispose();
                            }
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.$locvar0 = this.cppCompilationInstruction.Defines.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    case 2:
                        goto Label_00E2;

                    case 3:
                        goto Label_0197;

                    case 4:
                        this.$PC = -1;
                        goto Label_023C;

                    default:
                        goto Label_023C;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<define>__0 = this.$locvar0.Current;
                        this.$current = "/D" + this.<define>__0;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_023E;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar0 != null)
                    {
                        this.$locvar0.Dispose();
                    }
                }
                this.$locvar1 = this.cppCompilationInstruction.IncludePaths.GetEnumerator();
                num = 0xfffffffd;
            Label_00E2:
                try
                {
                    while (this.$locvar1.MoveNext())
                    {
                        this.<includePath>__1 = this.$locvar1.Current;
                        this.$current = "/I\"" + this.<includePath>__1 + "\"";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        flag = true;
                        goto Label_023E;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar1 != null)
                    {
                        this.$locvar1.Dispose();
                    }
                }
                this.$locvar2 = this.$this.ChooseCompilerFlags(this.cppCompilationInstruction, new Func<CppCompilationInstruction, IEnumerable<string>>(this.$this, (IntPtr) this.$this.DefaultCompilerFlags)).GetEnumerator();
                num = 0xfffffffd;
            Label_0197:
                try
                {
                    while (this.$locvar2.MoveNext())
                    {
                        this.<compilerFlag>__2 = this.$locvar2.Current;
                        this.$current = this.<compilerFlag>__2;
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        flag = true;
                        goto Label_023E;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar2 != null)
                    {
                        this.$locvar2.Dispose();
                    }
                }
                this.$current = this.cppCompilationInstruction.SourceFile.InQuotes();
                if (!this.$disposing)
                {
                    this.$PC = 4;
                }
                goto Label_023E;
            Label_023C:
                return false;
            Label_023E:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MsvcToolChain.<CompilerFlagsFor>c__Iterator4 { 
                    $this = this.$this,
                    cppCompilationInstruction = this.cppCompilationInstruction
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DefaultCompilerFlags>c__Iterator5 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal int $PC;
            internal MsvcToolChain $this;
            private static Func<string, bool> <>f__am$cache0;
            internal bool <hasClrFlag>__0;
            internal string <p>__1;
            internal CppCompilationInstruction cppCompilationInstruction;

            private static bool <>m__0(string flag)
            {
                return flag.ToLower().StartsWith("/clr");
            }

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 0x10:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar0 != null)
                            {
                                this.$locvar0.Dispose();
                            }
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        if (<>f__am$cache0 == null)
                        {
                            <>f__am$cache0 = new Func<string, bool>(null, (IntPtr) <>m__0);
                        }
                        this.<hasClrFlag>__0 = Enumerable.Any<string>(this.cppCompilationInstruction.CompilerFlags, <>f__am$cache0);
                        this.$current = (this.$this.BuildConfiguration != BuildConfiguration.Debug) ? "/MD" : "/MDd";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_04DE;

                    case 1:
                        this.$current = "/c";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_04DE;

                    case 2:
                        this.$current = "/bigobj";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_04DE;

                    case 3:
                        this.$current = "/W3";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_04DE;

                    case 4:
                        this.$current = "/Zi";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_04DE;

                    case 5:
                        if (this.<hasClrFlag>__0)
                        {
                            break;
                        }
                        this.$current = "/EHs";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_04DE;

                    case 6:
                        this.$current = "/GR-";
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_04DE;

                    case 7:
                        break;

                    case 8:
                        this.$current = "/wd4102";
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_04DE;

                    case 9:
                        this.$current = "/wd4800";
                        if (!this.$disposing)
                        {
                            this.$PC = 10;
                        }
                        goto Label_04DE;

                    case 10:
                        this.$current = "/wd4307";
                        if (!this.$disposing)
                        {
                            this.$PC = 11;
                        }
                        goto Label_04DE;

                    case 11:
                        this.$current = "/wd4056";
                        if (!this.$disposing)
                        {
                            this.$PC = 12;
                        }
                        goto Label_04DE;

                    case 12:
                        this.$current = "/wd4190";
                        if (!this.$disposing)
                        {
                            this.$PC = 13;
                        }
                        goto Label_04DE;

                    case 13:
                        this.$current = "/wd4723";
                        if (!this.$disposing)
                        {
                            this.$PC = 14;
                        }
                        goto Label_04DE;

                    case 14:
                        this.$current = "/wd4467";
                        if (!this.$disposing)
                        {
                            this.$PC = 15;
                        }
                        goto Label_04DE;

                    case 15:
                        if (this.$this.AdditionalCompilerOptionsForSourceFile == null)
                        {
                            goto Label_0371;
                        }
                        this.$locvar0 = this.$this.AdditionalCompilerOptionsForSourceFile.Invoke(this.cppCompilationInstruction.SourceFile.ToString()).GetEnumerator();
                        num = 0xfffffffd;
                        goto Label_02FA;

                    case 0x10:
                        goto Label_02FA;

                    case 0x11:
                        this.$current = "/Zc:inline";
                        if (!this.$disposing)
                        {
                            this.$PC = 0x12;
                        }
                        goto Label_04DE;

                    case 0x12:
                        if (this.<hasClrFlag>__0)
                        {
                            goto Label_04D5;
                        }
                        this.$current = "/RTC1";
                        if (!this.$disposing)
                        {
                            this.$PC = 0x13;
                        }
                        goto Label_04DE;

                    case 0x13:
                    case 0x1a:
                        goto Label_04D5;

                    case 20:
                        this.$current = "/Oi";
                        if (!this.$disposing)
                        {
                            this.$PC = 0x15;
                        }
                        goto Label_04DE;

                    case 0x15:
                        this.$current = "/Oy-";
                        if (!this.$disposing)
                        {
                            this.$PC = 0x16;
                        }
                        goto Label_04DE;

                    case 0x16:
                        this.$current = "/GS-";
                        if (!this.$disposing)
                        {
                            this.$PC = 0x17;
                        }
                        goto Label_04DE;

                    case 0x17:
                        this.$current = "/Gw";
                        if (!this.$disposing)
                        {
                            this.$PC = 0x18;
                        }
                        goto Label_04DE;

                    case 0x18:
                        this.$current = "/GF";
                        if (!this.$disposing)
                        {
                            this.$PC = 0x19;
                        }
                        goto Label_04DE;

                    case 0x19:
                        this.$current = "/Zo";
                        if (!this.$disposing)
                        {
                            this.$PC = 0x1a;
                        }
                        goto Label_04DE;

                    default:
                        goto Label_04DC;
                }
                this.$current = "/Gy";
                if (!this.$disposing)
                {
                    this.$PC = 8;
                }
                goto Label_04DE;
            Label_02FA:
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<p>__1 = this.$locvar0.Current;
                        this.$current = this.<p>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 0x10;
                        }
                        flag = true;
                        goto Label_04DE;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar0 != null)
                    {
                        this.$locvar0.Dispose();
                    }
                }
            Label_0371:
                if (this.$this.BuildConfiguration == BuildConfiguration.Debug)
                {
                    this.$current = "/Od";
                    if (!this.$disposing)
                    {
                        this.$PC = 0x11;
                    }
                }
                else
                {
                    this.$current = "/Ox";
                    if (!this.$disposing)
                    {
                        this.$PC = 20;
                    }
                }
                goto Label_04DE;
            Label_04D5:
                this.$PC = -1;
            Label_04DC:
                return false;
            Label_04DE:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MsvcToolChain.<DefaultCompilerFlags>c__Iterator5 { 
                    $this = this.$this,
                    cppCompilationInstruction = this.cppCompilationInstruction
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FlagsToMakeWarningsErrorsFor>c__Iterator9 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = "/WX";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MsvcToolChain.<FlagsToMakeWarningsErrorsFor>c__Iterator9();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetDefaultLinkerArgs>c__Iterator3 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal IEnumerator<NPath> $locvar1;
            internal IEnumerator<NPath> $locvar2;
            internal IEnumerator<string> $locvar3;
            internal int $PC;
            internal MsvcToolChain $this;
            internal string <libpath>__3;
            internal string <library>__0;
            internal NPath <library>__1;
            internal NPath <library>__2;
            internal IEnumerable<NPath> dynamicLibraries;
            internal NPath outputFile;
            internal IEnumerable<NPath> staticLibraries;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 0x10:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar0 != null)
                            {
                                this.$locvar0.Dispose();
                            }
                        }
                        break;

                    case 0x11:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar1 != null)
                            {
                                this.$locvar1.Dispose();
                            }
                        }
                        break;

                    case 0x12:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar2 != null)
                            {
                                this.$locvar2.Dispose();
                            }
                        }
                        break;

                    case 0x13:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar3 != null)
                            {
                                this.$locvar3.Dispose();
                            }
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                string[] textArray1;
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.$current = "/DEBUG";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_050C;

                    case 1:
                        this.$current = "/INCREMENTAL:NO";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_050C;

                    case 2:
                        this.$current = "/MAP";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_050C;

                    case 3:
                        this.$current = "/LARGEADDRESSAWARE";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_050C;

                    case 4:
                        this.$current = "/NXCOMPAT";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_050C;

                    case 5:
                        this.$current = "/DYNAMICBASE";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_050C;

                    case 6:
                        this.$current = "/NOLOGO";
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_050C;

                    case 7:
                        this.$current = "/TLBID:1";
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_050C;

                    case 8:
                        if (this.$this.BuildConfiguration == BuildConfiguration.Debug)
                        {
                            break;
                        }
                        this.$current = "/LTCG";
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_050C;

                    case 9:
                        this.$current = "/OPT:REF";
                        if (!this.$disposing)
                        {
                            this.$PC = 10;
                        }
                        goto Label_050C;

                    case 10:
                        this.$current = "/OPT:ICF";
                        if (!this.$disposing)
                        {
                            this.$PC = 11;
                        }
                        goto Label_050C;

                    case 11:
                        if (!(this.$this.Architecture is ARMv7Architecture))
                        {
                            break;
                        }
                        this.$current = "/OPT:LBR";
                        if (!this.$disposing)
                        {
                            this.$PC = 12;
                        }
                        goto Label_050C;

                    case 12:
                        break;

                    case 13:
                        goto Label_0240;

                    case 14:
                        goto Label_0284;

                    case 15:
                        this.$locvar0 = this.$this.ToolChainStaticLibraries().GetEnumerator();
                        num = 0xfffffffd;
                        goto Label_02BE;

                    case 0x10:
                        goto Label_02BE;

                    case 0x11:
                        goto Label_034E;

                    case 0x12:
                        goto Label_03DE;

                    case 0x13:
                        goto Label_048D;

                    default:
                        goto Label_050A;
                }
                if (this.$this.Architecture is x64Architecture)
                {
                    this.$current = "/HIGHENTROPYVA";
                    if (!this.$disposing)
                    {
                        this.$PC = 13;
                    }
                    goto Label_050C;
                }
            Label_0240:
                textArray1 = new string[] { this.$this.DynamicLibraryExtension };
                if (this.outputFile.HasExtension(textArray1))
                {
                    this.$current = "/DLL";
                    if (!this.$disposing)
                    {
                        this.$PC = 14;
                    }
                    goto Label_050C;
                }
            Label_0284:
                this.$current = "/NODEFAULTLIB:uuid.lib";
                if (!this.$disposing)
                {
                    this.$PC = 15;
                }
                goto Label_050C;
            Label_02BE:
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<library>__0 = this.$locvar0.Current;
                        this.$current = ExtensionMethods.InQuotes(this.<library>__0);
                        if (!this.$disposing)
                        {
                            this.$PC = 0x10;
                        }
                        flag = true;
                        goto Label_050C;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar0 != null)
                    {
                        this.$locvar0.Dispose();
                    }
                }
                this.$locvar1 = this.staticLibraries.GetEnumerator();
                num = 0xfffffffd;
            Label_034E:
                try
                {
                    while (this.$locvar1.MoveNext())
                    {
                        this.<library>__1 = this.$locvar1.Current;
                        this.$current = this.<library>__1.InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 0x11;
                        }
                        flag = true;
                        goto Label_050C;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar1 != null)
                    {
                        this.$locvar1.Dispose();
                    }
                }
                this.$locvar2 = this.dynamicLibraries.GetEnumerator();
                num = 0xfffffffd;
            Label_03DE:
                try
                {
                    while (this.$locvar2.MoveNext())
                    {
                        this.<library>__2 = this.$locvar2.Current;
                        this.$current = this.<library>__2.ChangeExtension(".lib").InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 0x12;
                        }
                        flag = true;
                        goto Label_050C;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar2 != null)
                    {
                        this.$locvar2.Dispose();
                    }
                }
                this.$locvar3 = ExtensionMethods.PrefixedWith(Extensions.InQuotes(this.$this.ToolChainLibraryPaths(), SlashMode.Native), "/LIBPATH:").GetEnumerator();
                num = 0xfffffffd;
            Label_048D:
                try
                {
                    while (this.$locvar3.MoveNext())
                    {
                        this.<libpath>__3 = this.$locvar3.Current;
                        this.$current = this.<libpath>__3;
                        if (!this.$disposing)
                        {
                            this.$PC = 0x13;
                        }
                        flag = true;
                        goto Label_050C;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar3 != null)
                    {
                        this.$locvar3.Dispose();
                    }
                }
                this.$PC = -1;
            Label_050A:
                return false;
            Label_050C:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MsvcToolChain.<GetDefaultLinkerArgs>c__Iterator3 { 
                    $this = this.$this,
                    outputFile = this.outputFile,
                    staticLibraries = this.staticLibraries,
                    dynamicLibraries = this.dynamicLibraries
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <IdlCompilerArgumentsFor>c__Iterator6 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal MsvcToolChain $this;
            private static Func<NPath, string> <>f__am$cache0;
            private static Func<string, string, string> <>f__am$cache1;
            internal string <aggregatedIncludes>__1;
            internal NPath[] <toolChainIncludes>__0;
            internal NPath idl;

            private static string <>m__0(NPath x)
            {
                return x.ToString();
            }

            private static string <>m__1(string x, string y)
            {
                return (x + ";" + y);
            }

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = "/W3";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_0345;

                    case 1:
                        this.$current = "/nologo";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0345;

                    case 2:
                        this.$current = "/ns_prefix";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_0345;

                    case 3:
                        this.$current = "/char unsigned";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_0345;

                    case 4:
                        if (!(this.$this.Architecture is x86Architecture))
                        {
                            if (this.$this.Architecture is x64Architecture)
                            {
                                this.$current = "/env x64";
                                if (!this.$disposing)
                                {
                                    this.$PC = 6;
                                }
                                goto Label_0345;
                            }
                            if (this.$this.Architecture is ARMv7Architecture)
                            {
                                this.$current = "/env arm32";
                                if (!this.$disposing)
                                {
                                    this.$PC = 7;
                                }
                                goto Label_0345;
                            }
                            if (this.$this.Architecture is ARM64Architecture)
                            {
                                this.$current = "/env arm64";
                                if (!this.$disposing)
                                {
                                    this.$PC = 8;
                                }
                                goto Label_0345;
                            }
                            break;
                        }
                        this.$current = "/env win32";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_0345;

                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        break;

                    case 9:
                        goto Label_025A;

                    case 10:
                        this.$current = string.Format("/metadata_dir \"{0}\"", this.$this.MsvcInstallation.GetUnionMetadataDirectory());
                        if (!this.$disposing)
                        {
                            this.$PC = 11;
                        }
                        goto Label_0345;

                    case 11:
                        goto Label_02C6;

                    case 12:
                        this.$current = string.Format("/h \"{0}_h.h\"", this.idl.FileNameWithoutExtension);
                        if (!this.$disposing)
                        {
                            this.$PC = 13;
                        }
                        goto Label_0345;

                    case 13:
                        this.$current = this.idl.InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 14;
                        }
                        goto Label_0345;

                    case 14:
                        this.$PC = -1;
                        goto Label_0343;

                    default:
                        goto Label_0343;
                }
                this.<toolChainIncludes>__0 = Enumerable.ToArray<NPath>(this.$this.ToolChainIncludePaths());
                if (this.<toolChainIncludes>__0.Length > 0)
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = new Func<NPath, string>(null, (IntPtr) <>m__0);
                    }
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = new Func<string, string, string>(null, (IntPtr) <>m__1);
                    }
                    this.<aggregatedIncludes>__1 = Enumerable.Aggregate<string>(Enumerable.Select<NPath, string>(this.<toolChainIncludes>__0, <>f__am$cache0), <>f__am$cache1);
                    this.$current = string.Format("/I \"{0}\"", this.<aggregatedIncludes>__1);
                    if (!this.$disposing)
                    {
                        this.$PC = 9;
                    }
                    goto Label_0345;
                }
            Label_025A:
                if (this.$this.MsvcInstallation.HasMetadataDirectories())
                {
                    this.$current = "/winrt";
                    if (!this.$disposing)
                    {
                        this.$PC = 10;
                    }
                    goto Label_0345;
                }
            Label_02C6:
                this.$current = "/Oicf";
                if (!this.$disposing)
                {
                    this.$PC = 12;
                }
                goto Label_0345;
            Label_0343:
                return false;
            Label_0345:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MsvcToolChain.<IdlCompilerArgumentsFor>c__Iterator6 { 
                    $this = this.$this,
                    idl = this.idl
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <IdlOutputArgumentsFor>c__Iterator7 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal NPath outputDir;
            internal NPath outputTlbPath;
            internal NPath outputWinmdPath;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = string.Format("/out{0}", this.outputDir.InQuotes());
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_00C0;

                    case 1:
                        this.$current = string.Format("/tlb {0}", this.outputTlbPath.InQuotes());
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_00C0;

                    case 2:
                        this.$current = string.Format("/winmd {0}", this.outputWinmdPath.InQuotes());
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_00C0;

                    case 3:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_00C0:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MsvcToolChain.<IdlOutputArgumentsFor>c__Iterator7 { 
                    outputDir = this.outputDir,
                    outputTlbPath = this.outputTlbPath,
                    outputWinmdPath = this.outputWinmdPath
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <MakeLinkerInvocation>c__AnonStoreyA
        {
            internal NPath bestWorkingDirectory;

            internal string <>m__0(NPath p)
            {
                if (p.IsChildOf(this.bestWorkingDirectory))
                {
                    return p.RelativeTo(this.bestWorkingDirectory).ToString();
                }
                return p.ToString();
            }
        }

        [CompilerGenerated]
        private sealed class <ManifestCompilerArgumentsFor>c__Iterator8 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal NPath outputFile;
            internal NPath tlbFile;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = "/nologo";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_010B;

                    case 1:
                        this.$current = "/verbose";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_010B;

                    case 2:
                        this.$current = string.Format("/identity:\"{0},type=win32,version=1.0.0.0\"", this.outputFile.FileNameWithoutExtension);
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_010B;

                    case 3:
                        this.$current = string.Format("/tlb:{0}", this.tlbFile.InQuotes());
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_010B;

                    case 4:
                        this.$current = string.Format("/dll:{0}", ExtensionMethods.InQuotes(this.outputFile.FileName));
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_010B;

                    case 5:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_010B:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MsvcToolChain.<ManifestCompilerArgumentsFor>c__Iterator8 { 
                    outputFile = this.outputFile,
                    tlbFile = this.tlbFile
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <OutputArgumentFor>c__Iterator2 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal NPath objectFile;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = "/Fd" + this.objectFile.ChangeExtension(".pdb").InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_0097;

                    case 1:
                        this.$current = "/Fo" + this.objectFile.InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0097;

                    case 2:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0097:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MsvcToolChain.<OutputArgumentFor>c__Iterator2 { objectFile = this.objectFile };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ToolChainDefines>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal MsvcToolChain $this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = "_WIN32";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_0269;

                    case 1:
                        this.$current = "WIN32";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0269;

                    case 2:
                        this.$current = "_WINDOWS";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_0269;

                    case 3:
                        this.$current = "WINDOWS";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_0269;

                    case 4:
                        this.$current = "_UNICODE";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_0269;

                    case 5:
                        this.$current = "UNICODE";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_0269;

                    case 6:
                        this.$current = "_CRT_SECURE_NO_WARNINGS";
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_0269;

                    case 7:
                        this.$current = "_SCL_SECURE_NO_WARNINGS";
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_0269;

                    case 8:
                        this.$current = "_WINSOCK_DEPRECATED_NO_WARNINGS";
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_0269;

                    case 9:
                        if (this.$this.BuildConfiguration != BuildConfiguration.Debug)
                        {
                            this.$current = "_NDEBUG";
                            if (!this.$disposing)
                            {
                                this.$PC = 13;
                            }
                        }
                        else
                        {
                            this.$current = "_DEBUG";
                            if (!this.$disposing)
                            {
                                this.$PC = 10;
                            }
                        }
                        goto Label_0269;

                    case 10:
                        this.$current = "DEBUG";
                        if (!this.$disposing)
                        {
                            this.$PC = 11;
                        }
                        goto Label_0269;

                    case 11:
                        this.$current = "IL2CPP_DEBUG";
                        if (!this.$disposing)
                        {
                            this.$PC = 12;
                        }
                        goto Label_0269;

                    case 12:
                    case 14:
                        if (this.$this.Architecture is ARMv7Architecture)
                        {
                            this.$current = "__arm__";
                            if (!this.$disposing)
                            {
                                this.$PC = 15;
                            }
                            goto Label_0269;
                        }
                        break;

                    case 13:
                        this.$current = "NDEBUG";
                        if (!this.$disposing)
                        {
                            this.$PC = 14;
                        }
                        goto Label_0269;

                    case 15:
                        break;

                    default:
                        goto Label_0267;
                }
                this.$PC = -1;
            Label_0267:
                return false;
            Label_0269:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MsvcToolChain.<ToolChainDefines>c__Iterator0 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ToolChainStaticLibraries>c__Iterator1 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal int $PC;
            internal MsvcToolChain $this;
            internal string <lib>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            if (this.$locvar0 != null)
                            {
                                this.$locvar0.Dispose();
                            }
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.$locvar0 = this.$this.<ToolChainStaticLibraries>__BaseCallProxy0().GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    case 2:
                        this.$PC = -1;
                        goto Label_00DC;

                    default:
                        goto Label_00DC;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<lib>__0 = this.$locvar0.Current;
                        this.$current = this.<lib>__0;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_00DE;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar0 != null)
                    {
                        this.$locvar0.Dispose();
                    }
                }
                this.$current = "ws2_32.lib";
                if (!this.$disposing)
                {
                    this.$PC = 2;
                }
                goto Label_00DE;
            Label_00DC:
                return false;
            Label_00DE:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MsvcToolChain.<ToolChainStaticLibraries>c__Iterator1 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        private class IdlCompilationResult
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private CompilationInvocation <Invocation>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private NPath <OutputDirectory>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string <StdOut>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool <Success>k__BackingField;

            public static MsvcToolChain.IdlCompilationResult FromShellExecuteResult(Shell.ExecuteResult shellExecuteResult, CompilationInvocation invocation, NPath outputDirectory)
            {
                return new MsvcToolChain.IdlCompilationResult { 
                    Success = shellExecuteResult.ExitCode == 0,
                    StdOut = (shellExecuteResult.StdOut + Environment.NewLine + shellExecuteResult.StdErr).Trim(),
                    OutputDirectory = outputDirectory,
                    Invocation = invocation
                };
            }

            public static MsvcToolChain.IdlCompilationResult SuccessfulResult(NPath outputDirectory)
            {
                return new MsvcToolChain.IdlCompilationResult { 
                    Success = true,
                    OutputDirectory = outputDirectory
                };
            }

            public CompilationInvocation Invocation { get; private set; }

            public NPath OutputDirectory { get; private set; }

            public string StdOut { get; private set; }

            public bool Success { get; private set; }
        }
    }
}

