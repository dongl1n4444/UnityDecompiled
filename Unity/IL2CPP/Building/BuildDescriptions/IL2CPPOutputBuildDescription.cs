namespace Unity.IL2CPP.Building.BuildDescriptions
{
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.Statistics;
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;
    using Unity.TinyProfiling;

    public class IL2CPPOutputBuildDescription : ProgramBuildDescription
    {
        private readonly IEnumerable<string> _additionalDefines;
        private readonly IEnumerable<NPath> _additionalIncludeDirectories;
        private readonly Unity.IL2CPP.Common.Architecture _architecture;
        private readonly NPath _cacheDirectory;
        private readonly CppToolChain _cppToolChain;
        private NPath _dataFolder;
        private NPath _libil2cppCacheDirectory;
        private NPath _mapFileParser;
        private readonly RuntimeBuildType _runtimeLibrary;
        private readonly NPath _sourceDirectory;
        private readonly IEnumerable<string> _specifiedCompilerFlags;
        private readonly IEnumerable<string> _specifiedLinkerFlags;
        private readonly IEnumerable<NPath> _staticLibraries;
        [CompilerGenerated]
        private static Func<NPath, IEnumerable<NPath>> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<NPath, bool> <>f__am$cache1;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <ForceRebuildMapFileParser>k__BackingField;

        public IL2CPPOutputBuildDescription(IL2CPPOutputBuildDescription other)
        {
            this._sourceDirectory = other._sourceDirectory;
            this._cacheDirectory = other._cacheDirectory;
            this.ForceRebuildMapFileParser = other.ForceRebuildMapFileParser;
            this._staticLibraries = other._staticLibraries;
            this._additionalDefines = other._additionalDefines;
            this._additionalIncludeDirectories = other._additionalIncludeDirectories;
            base._outputFile = other._outputFile;
            this._specifiedCompilerFlags = other._specifiedCompilerFlags;
            this._specifiedLinkerFlags = other._specifiedLinkerFlags;
            this._cppToolChain = other._cppToolChain;
            this._dataFolder = other._dataFolder;
            this._libil2cppCacheDirectory = other._libil2cppCacheDirectory;
            this._mapFileParser = other._mapFileParser;
            this._runtimeLibrary = other._runtimeLibrary;
        }

        public IL2CPPOutputBuildDescription(NPath sourceDirectory, NPath cacheDirectory, NPath outputFile, DotNetProfile dotnetProfile, CppToolChain cppToolChain, NPath dataFolder, bool forceRebuildMapFileParser, RuntimeBuildType runtimeLibrary, Unity.IL2CPP.Common.Architecture architecture, IEnumerable<string> additionalDefines = null, IEnumerable<NPath> additionalIncludeDirectories = null, IEnumerable<NPath> staticLibraries = null, IEnumerable<string> specifiedCompilerFlags = null, IEnumerable<string> specifiedLinkerFlags = null, NPath libil2cppCacheDirectory = null, NPath mapFileParser = null)
        {
            this._sourceDirectory = sourceDirectory;
            this._cacheDirectory = cacheDirectory?.EnsureDirectoryExists("");
            this._cppToolChain = cppToolChain;
            this.ForceRebuildMapFileParser = forceRebuildMapFileParser;
            if (staticLibraries == null)
            {
            }
            this._staticLibraries = new NPath[0];
            if (additionalDefines == null)
            {
            }
            this._additionalDefines = new string[0];
            if (additionalIncludeDirectories == null)
            {
            }
            this._additionalIncludeDirectories = new NPath[0];
            base._outputFile = outputFile;
            this._specifiedCompilerFlags = specifiedCompilerFlags;
            this._specifiedLinkerFlags = specifiedLinkerFlags;
            this._runtimeLibrary = runtimeLibrary;
            this._architecture = architecture;
            this._dataFolder = dataFolder;
            this._mapFileParser = mapFileParser;
            if ((libil2cppCacheDirectory == null) || (libil2cppCacheDirectory == cacheDirectory))
            {
                this._libil2cppCacheDirectory = (cacheDirectory == null) ? TempDir.Empty("libil2cpp") : cacheDirectory.Combine(new string[] { "libil2cpp" }).EnsureDirectoryExists("");
            }
            else
            {
                this._libil2cppCacheDirectory = libil2cppCacheDirectory;
            }
            if (dotnetProfile == DotNetProfile.Net45)
            {
                string[] second = new string[] { "NET_4_0" };
                this._additionalDefines = this._additionalDefines.Concat<string>(second);
            }
        }

        [DebuggerHidden]
        public override IEnumerable<string> AdditionalDefinesFor(NPath sourceFile) => 
            new <AdditionalDefinesFor>c__Iterator3 { 
                sourceFile = sourceFile,
                $this = this,
                $PC = -2
            };

        public override IEnumerable<NPath> AdditionalIncludePathsFor(NPath sourceFile)
        {
            if (this.IsBoehmFile(sourceFile))
            {
                return this._additionalIncludeDirectories.Concat<NPath>(this.BoehmIncludeDirs);
            }
            if (this.IsLibIL2CPPFile(sourceFile))
            {
                if (this._runtimeLibrary == RuntimeBuildType.LibMono)
                {
                    string[] append = new string[] { "mono" };
                    return this._additionalIncludeDirectories.Concat<NPath>(this.LibIL2CPPIncludeDirs).Concat<NPath>(LibMonoIncludeDirs).Append<NPath>(MonoInstall.MonoBleedingEdgeIncludeDirectory.Combine(append));
                }
                return this._additionalIncludeDirectories.Concat<NPath>(this.LibIL2CPPIncludeDirs);
            }
            if (this._runtimeLibrary == RuntimeBuildType.LibMono)
            {
                string[] textArray2 = new string[] { "mono" };
                return this._additionalIncludeDirectories.Concat<NPath>(LibMonoIncludeDirs).Concat<NPath>(this.LibIL2CPPIncludeDirs.Append<NPath>(this._sourceDirectory)).Append<NPath>(MonoInstall.MonoBleedingEdgeIncludeDirectory.Combine(textArray2));
            }
            return this._additionalIncludeDirectories.Concat<NPath>(this.LibIL2CPPIncludeDirs.Append<NPath>(this._sourceDirectory));
        }

        [DebuggerHidden]
        protected virtual IEnumerable<string> BoehmDefines() => 
            new <BoehmDefines>c__Iterator7 { $PC = -2 };

        private NPath BuildMapFileParser()
        {
            using (TinyProfiler.Section("BuildMapFileParser", ""))
            {
                NPath cacheDirectory = this._cacheDirectory?.Combine(new string[] { "MapFileParserCache" }).EnsureDirectoryExists("");
                return CppProgramBuilder.Create(RuntimePlatform.Current, new MapFileParserBuildDescription(cacheDirectory), false, Unity.IL2CPP.Common.Architecture.BestThisMachineCanRun, BuildConfiguration.Release, this.ForceRebuildMapFileParser, false).BuildAndLogStatsForTestRunner();
            }
        }

        private CppCompilationInstruction CppCompilationInstructionFor(NPath sourceFile, NPath cacheDirectory) => 
            new CppCompilationInstruction { 
                SourceFile = sourceFile,
                Defines = this.AdditionalDefinesFor(sourceFile),
                IncludePaths = this.AdditionalIncludePathsFor(sourceFile),
                CompilerFlags = this.AdditionalCompilerFlags,
                CacheDirectory = cacheDirectory
            };

        public override void FinalizeBuild(CppToolChain toolChain)
        {
            if (toolChain.SupportsMapFileParser)
            {
                if ((this._mapFileParser == null) || !this._mapFileParser.Exists(""))
                {
                    this._mapFileParser = this.BuildMapFileParser();
                }
                this.RunMapFileParser(toolChain, this.OutputFile, this._mapFileParser);
            }
            if ((this._sourceDirectory != base._outputFile.Parent) && this._sourceDirectory.DirectoryExists("Data"))
            {
                string[] append = new string[] { "Data" };
                this._sourceDirectory.Combine(append).Copy(base._outputFile.Parent.MakeAbsolute());
            }
        }

        [DebuggerHidden]
        private IEnumerable<NPath> GCSourceFiles() => 
            new <GCSourceFiles>c__Iterator0 { $PC = -2 };

        protected virtual bool IsBoehmFile(NPath sourceFile) => 
            sourceFile.IsChildOf(BoehmDir);

        protected virtual bool IsLibIL2CPPFile(NPath sourceFile) => 
            sourceFile.IsChildOf(LibIL2CPPDir);

        protected virtual IEnumerable<CppCompilationInstruction> LibIL2CPPCompileInstructions()
        {
            NPath[] foldersToGlob = new NPath[] { LibIL2CPPDir };
            return (from sourceFile in SourceFilesIn(foldersToGlob).Concat<NPath>(this.GCSourceFiles()) select this.CppCompilationInstructionFor(sourceFile, this._libil2cppCacheDirectory));
        }

        private IEnumerable<CppCompilationInstruction> LibMonoCompileInstructions()
        {
            NPath[] foldersToGlob = new NPath[4];
            foldersToGlob[0] = LibMonoDir;
            string[] append = new string[] { "mono-runtime" };
            foldersToGlob[1] = LibIL2CPPDir.Combine(append);
            string[] textArray2 = new string[] { "os" };
            foldersToGlob[2] = LibIL2CPPDir.Combine(textArray2);
            string[] textArray3 = new string[] { "utils" };
            foldersToGlob[3] = LibIL2CPPDir.Combine(textArray3);
            return (from sourceFile in SourceFilesIn(foldersToGlob).Concat<NPath>(this.GCSourceFiles()).Concat<NPath>(this.SpecificLibIL2CPPFiles()) select this.CppCompilationInstructionFor(sourceFile, this._libil2cppCacheDirectory));
        }

        public override void OnBeforeLink(NPath workingDirectory, IEnumerable<NPath> objectFiles, CppToolChainContext toolChainContext, bool forceRebuild, bool verbose)
        {
            if (this._runtimeLibrary == RuntimeBuildType.LibIL2CPPDynamic)
            {
                LibIL2CPPDynamicLibProgramDescription programBuildDescription = new LibIL2CPPDynamicLibProgramDescription(this.LibIL2CPPCompileInstructions(), this.LibIL2CppDynamicLibraryLocation, this._libil2cppCacheDirectory);
                new CppProgramBuilder(this._cppToolChain, programBuildDescription, verbose, forceRebuild).BuildAndLogStatsForTestRunner();
            }
        }

        private void RunMapFileParser(CppToolChain toolChain, NPath outputFile, NPath mapFileParser)
        {
            NPath path = outputFile.ChangeExtension("map");
            string[] append = new string[] { "SymbolMap" };
            NPath path2 = this._dataFolder.Combine(append);
            string str = $"-format={toolChain.MapFileParserFormat} "{path}" "{path2}"";
            Console.WriteLine("Encoding map file using command: {0} {1}", mapFileParser, str);
            using (TinyProfiler.Section("Running MapFileParser", ""))
            {
                Shell.ExecuteAndCaptureOutput(mapFileParser, str, null);
            }
        }

        private static IEnumerable<NPath> SourceFilesIn(params NPath[] foldersToGlob)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = d => d.Files("*.c*", true);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = f => f.HasExtension(new string[] { "c", "cpp" });
            }
            return foldersToGlob.SelectMany<NPath, NPath>(<>f__am$cache0).Where<NPath>(<>f__am$cache1);
        }

        [DebuggerHidden]
        private IEnumerable<NPath> SpecificLibIL2CPPFiles() => 
            new <SpecificLibIL2CPPFiles>c__Iterator1 { $PC = -2 };

        public override IEnumerable<string> AdditionalCompilerFlags
        {
            get
            {
                IEnumerable<string> enumerable;
                if (this._specifiedCompilerFlags != null)
                {
                    enumerable = this._specifiedCompilerFlags;
                }
                else
                {
                    enumerable = Enumerable.Empty<string>();
                }
                return enumerable;
            }
        }

        public override IEnumerable<string> AdditionalLinkerFlags =>
            new <>c__Iterator6 { 
                $this=this,
                $PC=-2
            };

        protected static NPath BoehmDir =>
            CommonPaths.Il2CppRoot?.Combine(new string[] { "external/boehmgc" });

        protected virtual IEnumerable<NPath> BoehmIncludeDirs
        {
            get
            {
                NPath[] pathArray1 = new NPath[2];
                string[] append = new string[] { "include" };
                pathArray1[0] = BoehmDir.Combine(append);
                string[] textArray2 = new string[] { "libatomic_ops/src" };
                pathArray1[1] = BoehmDir.Combine(textArray2);
                return pathArray1;
            }
        }

        public override IEnumerable<CppCompilationInstruction> CppCompileInstructions =>
            new <>c__Iterator2 { 
                $this=this,
                $PC=-2
            };

        public override IEnumerable<NPath> DynamicLibraries =>
            new <>c__Iterator5 { 
                $this=this,
                $PC=-2
            };

        public bool ForceRebuildMapFileParser { get; private set; }

        public override NPath GlobalCacheDirectory =>
            this._cacheDirectory;

        protected static NPath LibIL2CPPDir =>
            CommonPaths.Il2CppRoot?.Combine(new string[] { "libil2cpp" });

        public NPath LibIL2CppDynamicLibraryLocation
        {
            get
            {
                NPath path = !this._cppToolChain.DynamicLibrariesHaveToSitNextToExecutable() ? this._dataFolder : this.OutputFile.Parent;
                string[] append = new string[] { "libil2cpp" };
                return path.Combine(append).ChangeExtension(this._cppToolChain.DynamicLibraryExtension);
            }
        }

        protected virtual IEnumerable<NPath> LibIL2CPPIncludeDirs
        {
            get
            {
                NPath[] pathArray1 = new NPath[2];
                pathArray1[0] = LibIL2CPPDir;
                string[] append = new string[] { "include" };
                pathArray1[1] = BoehmDir.Combine(append);
                return pathArray1;
            }
        }

        private static NPath LibMonoDir =>
            CommonPaths.Il2CppRoot?.Combine(new string[] { "libmono" });

        private static IEnumerable<NPath> LibMonoIncludeDirs
        {
            get
            {
                NPath[] pathArray1 = new NPath[2];
                pathArray1[0] = LibMonoDir;
                string[] append = new string[] { "mono-runtime" };
                pathArray1[1] = LibIL2CPPDir.Combine(append);
                return pathArray1;
            }
        }

        private IEnumerable<NPath> LibMonoStaticLibraries =>
            new <>c__Iterator4 { 
                $this=this,
                $PC=-2
            };

        public override IEnumerable<NPath> StaticLibraries
        {
            get
            {
                if (this._runtimeLibrary == RuntimeBuildType.LibMono)
                {
                    return this._staticLibraries.Concat<NPath>(this.LibMonoStaticLibraries);
                }
                return this._staticLibraries;
            }
        }

        [CompilerGenerated]
        private sealed class <>c__Iterator2 : IEnumerable, IEnumerable<CppCompilationInstruction>, IEnumerator, IDisposable, IEnumerator<CppCompilationInstruction>
        {
            internal CppCompilationInstruction $current;
            internal bool $disposing;
            internal IEnumerator<CppCompilationInstruction> $locvar0;
            internal IEnumerator<CppCompilationInstruction> $locvar1;
            internal IEnumerator<CppCompilationInstruction> $locvar2;
            internal int $PC;
            internal IL2CPPOutputBuildDescription $this;
            internal CppCompilationInstruction <i>__1;
            internal CppCompilationInstruction <i>__2;
            internal CppCompilationInstruction <i>__3;

            internal CppCompilationInstruction <>m__0(NPath sourceFile) => 
                this.$this.CppCompilationInstructionFor(sourceFile, this.$this._cacheDirectory);

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
                    {
                        NPath[] foldersToGlob = new NPath[] { this.$this._sourceDirectory };
                        this.$locvar0 = IL2CPPOutputBuildDescription.SourceFilesIn(foldersToGlob).Select<NPath, CppCompilationInstruction>(new Func<NPath, CppCompilationInstruction>(this.<>m__0)).GetEnumerator();
                        num = 0xfffffffd;
                        break;
                    }
                    case 1:
                        break;

                    case 2:
                        goto Label_0104;

                    case 3:
                        goto Label_01AA;

                    default:
                        goto Label_0226;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<i>__1 = this.$locvar0.Current;
                        this.$current = this.<i>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_0228;
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
                if (this.$this._runtimeLibrary != RuntimeBuildType.LibIL2CPPStatic)
                {
                    if (this.$this._runtimeLibrary != RuntimeBuildType.LibMono)
                    {
                        goto Label_021F;
                    }
                    this.$locvar2 = this.$this.LibMonoCompileInstructions().GetEnumerator();
                    num = 0xfffffffd;
                    goto Label_01AA;
                }
                this.$locvar1 = this.$this.LibIL2CPPCompileInstructions().GetEnumerator();
                num = 0xfffffffd;
            Label_0104:
                try
                {
                    while (this.$locvar1.MoveNext())
                    {
                        this.<i>__2 = this.$locvar1.Current;
                        this.$current = this.<i>__2;
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        flag = true;
                        goto Label_0228;
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
                goto Label_021F;
            Label_01AA:
                try
                {
                    while (this.$locvar2.MoveNext())
                    {
                        this.<i>__3 = this.$locvar2.Current;
                        this.$current = this.<i>__3;
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        flag = true;
                        goto Label_0228;
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
            Label_021F:
                this.$PC = -1;
            Label_0226:
                return false;
            Label_0228:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<CppCompilationInstruction> IEnumerable<CppCompilationInstruction>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new IL2CPPOutputBuildDescription.<>c__Iterator2 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Unity.IL2CPP.Building.CppCompilationInstruction>.GetEnumerator();

            CppCompilationInstruction IEnumerator<CppCompilationInstruction>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <>c__Iterator4 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;
            internal IL2CPPOutputBuildDescription $this;
            internal NPath <libMonoDirectory>__1;
            internal NPath <libMonoDirectory>__2;

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
                    {
                        if (!PlatformUtils.IsWindows())
                        {
                            if (PlatformUtils.IsOSX())
                            {
                                this.<libMonoDirectory>__2 = MonoInstall.MonoBleedingEdgeEmbedRuntimesDirectoryFor(PlatformUtils.Architecture.x64);
                                string[] textArray4 = new string[] { "libmonoruntime-il2cpp.a" };
                                this.$current = this.<libMonoDirectory>__2.Combine(textArray4);
                                if (!this.$disposing)
                                {
                                    this.$PC = 7;
                                }
                                goto Label_027C;
                            }
                            break;
                        }
                        this.<libMonoDirectory>__1 = MonoInstall.MonoBleedingEdgeEmbedRuntimesDirectoryFor((this.$this._architecture.Name != "x86") ? PlatformUtils.Architecture.x64 : PlatformUtils.Architecture.x86);
                        string[] append = new string[] { "libmonoruntime-boehm-il2cpp.lib" };
                        this.$current = this.<libMonoDirectory>__1.Combine(append);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_027C;
                    }
                    case 1:
                    {
                        string[] textArray2 = new string[] { "libmonoutils-il2cpp.lib" };
                        this.$current = this.<libMonoDirectory>__1.Combine(textArray2);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_027C;
                    }
                    case 2:
                    {
                        string[] textArray3 = new string[] { "eglib.lib" };
                        this.$current = this.<libMonoDirectory>__1.Combine(textArray3);
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_027C;
                    }
                    case 3:
                        this.$current = new NPath("Mswsock.lib");
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_027C;

                    case 4:
                        this.$current = new NPath("Mincore.lib");
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_027C;

                    case 5:
                        this.$current = new NPath("Psapi.lib");
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_027C;

                    case 6:
                    case 10:
                        break;

                    case 7:
                    {
                        string[] textArray5 = new string[] { "libwapi.a" };
                        this.$current = this.<libMonoDirectory>__2.Combine(textArray5);
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_027C;
                    }
                    case 8:
                    {
                        string[] textArray6 = new string[] { "libmonoutils-il2cpp.a" };
                        this.$current = this.<libMonoDirectory>__2.Combine(textArray6);
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_027C;
                    }
                    case 9:
                    {
                        string[] textArray7 = new string[] { "libeglib.a" };
                        this.$current = this.<libMonoDirectory>__2.Combine(textArray7);
                        if (!this.$disposing)
                        {
                            this.$PC = 10;
                        }
                        goto Label_027C;
                    }
                    default:
                        goto Label_027A;
                }
                this.$PC = -1;
            Label_027A:
                return false;
            Label_027C:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<NPath> IEnumerable<NPath>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new IL2CPPOutputBuildDescription.<>c__Iterator4 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();

            NPath IEnumerator<NPath>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <>c__Iterator5 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;
            internal IL2CPPOutputBuildDescription $this;

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
                        if (this.$this._runtimeLibrary != RuntimeBuildType.LibIL2CPPDynamic)
                        {
                            break;
                        }
                        this.$current = this.$this.LibIL2CppDynamicLibraryLocation;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
                        break;

                    default:
                        goto Label_005F;
                }
                this.$PC = -1;
            Label_005F:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<NPath> IEnumerable<NPath>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new IL2CPPOutputBuildDescription.<>c__Iterator5 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();

            NPath IEnumerator<NPath>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <>c__Iterator6 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal int $PC;
            internal IL2CPPOutputBuildDescription $this;
            internal string <flag>__1;

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
                        if (this.$this._specifiedLinkerFlags == null)
                        {
                            goto Label_00CC;
                        }
                        this.$locvar0 = this.$this._specifiedLinkerFlags.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    case 2:
                        this.$current = "-framework Foundation";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_0130;

                    case 3:
                        goto Label_0127;

                    default:
                        goto Label_012E;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<flag>__1 = this.$locvar0.Current;
                        this.$current = this.<flag>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_0130;
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
            Label_00CC:
                if ((this.$this._runtimeLibrary == RuntimeBuildType.LibMono) && PlatformUtils.IsOSX())
                {
                    this.$current = "-liconv";
                    if (!this.$disposing)
                    {
                        this.$PC = 2;
                    }
                    goto Label_0130;
                }
            Label_0127:
                this.$PC = -1;
            Label_012E:
                return false;
            Label_0130:
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
                return new IL2CPPOutputBuildDescription.<>c__Iterator6 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

            string IEnumerator<string>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <AdditionalDefinesFor>c__Iterator3 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal IEnumerator<string> $locvar1;
            internal int $PC;
            internal IL2CPPOutputBuildDescription $this;
            internal string <define>__1;
            internal string <define>__2;
            internal NPath sourceFile;

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
                        this.$locvar0 = this.$this._additionalDefines.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    case 2:
                        goto Label_00F2;

                    case 3:
                        goto Label_01B6;

                    case 4:
                    case 5:
                        goto Label_022B;

                    default:
                        goto Label_0232;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<define>__1 = this.$locvar0.Current;
                        this.$current = this.<define>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_0234;
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
                if (!this.$this.IsBoehmFile(this.sourceFile))
                {
                    goto Label_0166;
                }
                this.$locvar1 = this.$this.BoehmDefines().GetEnumerator();
                num = 0xfffffffd;
            Label_00F2:
                try
                {
                    while (this.$locvar1.MoveNext())
                    {
                        this.<define>__2 = this.$locvar1.Current;
                        this.$current = this.<define>__2;
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        flag = true;
                        goto Label_0234;
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
            Label_0166:
                if (this.$this._runtimeLibrary == RuntimeBuildType.LibIL2CPPDynamic)
                {
                    this.$current = !this.$this.IsLibIL2CPPFile(this.sourceFile) ? "LIBIL2CPP_IMPORT_CODEGEN_API" : "LIBIL2CPP_EXPORT_CODEGEN_API";
                    if (!this.$disposing)
                    {
                        this.$PC = 3;
                    }
                    goto Label_0234;
                }
            Label_01B6:
                if ((this.$this._runtimeLibrary == RuntimeBuildType.LibIL2CPPStatic) || (this.$this._runtimeLibrary == RuntimeBuildType.LibIL2CPPDynamic))
                {
                    this.$current = "RUNTIME_IL2CPP";
                    if (!this.$disposing)
                    {
                        this.$PC = 4;
                    }
                    goto Label_0234;
                }
                if (this.$this._runtimeLibrary == RuntimeBuildType.LibMono)
                {
                    this.$current = "RUNTIME_MONO";
                    if (!this.$disposing)
                    {
                        this.$PC = 5;
                    }
                    goto Label_0234;
                }
            Label_022B:
                this.$PC = -1;
            Label_0232:
                return false;
            Label_0234:
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
                return new IL2CPPOutputBuildDescription.<AdditionalDefinesFor>c__Iterator3 { 
                    $this = this.$this,
                    sourceFile = this.sourceFile
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

            string IEnumerator<string>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <BoehmDefines>c__Iterator7 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
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
                        this.$current = "ALL_INTERIOR_POINTERS=1";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_01F3;

                    case 1:
                        this.$current = "GC_GCJ_SUPPORT=1";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_01F3;

                    case 2:
                        this.$current = "JAVA_FINALIZATION=1";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_01F3;

                    case 3:
                        this.$current = "NO_EXECUTE_PERMISSION=1";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_01F3;

                    case 4:
                        this.$current = "GC_NO_THREADS_DISCOVERY=1";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_01F3;

                    case 5:
                        this.$current = "IGNORE_DYNAMIC_LOADING=1";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_01F3;

                    case 6:
                        this.$current = "GC_DONT_REGISTER_MAIN_STATIC_DATA=1";
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_01F3;

                    case 7:
                        this.$current = "GC_VERSION_MAJOR=7";
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_01F3;

                    case 8:
                        this.$current = "GC_VERSION_MINOR=4";
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_01F3;

                    case 9:
                        this.$current = "GC_VERSION_MICRO=0";
                        if (!this.$disposing)
                        {
                            this.$PC = 10;
                        }
                        goto Label_01F3;

                    case 10:
                        this.$current = "GC_THREADS=1";
                        if (!this.$disposing)
                        {
                            this.$PC = 11;
                        }
                        goto Label_01F3;

                    case 11:
                        this.$current = "USE_MMAP=1";
                        if (!this.$disposing)
                        {
                            this.$PC = 12;
                        }
                        goto Label_01F3;

                    case 12:
                        this.$current = "USE_MUNMAP=1";
                        if (!this.$disposing)
                        {
                            this.$PC = 13;
                        }
                        goto Label_01F3;

                    case 13:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_01F3:
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
                return new IL2CPPOutputBuildDescription.<BoehmDefines>c__Iterator7();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

            string IEnumerator<string>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <GCSourceFiles>c__Iterator0 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
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
                    {
                        string[] append = new string[] { "extra/gc.c" };
                        this.$current = IL2CPPOutputBuildDescription.BoehmDir.Combine(append);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_0093;
                    }
                    case 1:
                    {
                        string[] textArray2 = new string[] { "extra/krait_signal_handler.c" };
                        this.$current = IL2CPPOutputBuildDescription.BoehmDir.Combine(textArray2);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0093;
                    }
                    case 2:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0093:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<NPath> IEnumerable<NPath>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new IL2CPPOutputBuildDescription.<GCSourceFiles>c__Iterator0();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();

            NPath IEnumerator<NPath>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <SpecificLibIL2CPPFiles>c__Iterator1 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
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
                    {
                        string[] append = new string[] { "char-conversions.cpp" };
                        this.$current = IL2CPPOutputBuildDescription.LibIL2CPPDir.Combine(append);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;
                    }
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
            IEnumerator<NPath> IEnumerable<NPath>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new IL2CPPOutputBuildDescription.<SpecificLibIL2CPPFiles>c__Iterator1();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();

            NPath IEnumerator<NPath>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        public class LibIL2CPPDynamicLibProgramDescription : ProgramBuildDescription
        {
            private readonly NPath _cacheDirectory;
            private readonly IEnumerable<CppCompilationInstruction> _cppCompileInstructions;

            public LibIL2CPPDynamicLibProgramDescription(IEnumerable<CppCompilationInstruction> cppCompileInstructions, NPath outputFile, NPath cacheDirectory)
            {
                this._cppCompileInstructions = cppCompileInstructions;
                base._outputFile = outputFile;
                this._cacheDirectory = cacheDirectory;
            }

            public override IEnumerable<CppCompilationInstruction> CppCompileInstructions =>
                this._cppCompileInstructions;

            public override NPath GlobalCacheDirectory =>
                this._cacheDirectory;
        }

        private class MapFileParserBuildDescription : ProgramBuildDescription
        {
            private readonly NPath _cacheDirectory;
            [CompilerGenerated]
            private static Func<NPath, bool> <>f__am$cache0;

            public MapFileParserBuildDescription(NPath cacheDirectory)
            {
                if (cacheDirectory == null)
                {
                }
                this._cacheDirectory = TempDir.Empty("mapfileparser");
            }

            public override IEnumerable<CppCompilationInstruction> CppCompileInstructions
            {
                get
                {
                    string[] append = new string[] { "MapFileParser" };
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = c => !c.Elements.Contains<string>("Tests");
                    }
                    return (from f in CommonPaths.Il2CppRoot.Combine(append).Files("*.cpp", true).Where<NPath>(<>f__am$cache0) select new CppCompilationInstruction { 
                        SourceFile = f,
                        CacheDirectory = this._cacheDirectory
                    });
                }
            }

            public override NPath GlobalCacheDirectory =>
                this._cacheDirectory;

            public override NPath OutputFile
            {
                get
                {
                    string[] append = new string[] { "build" };
                    string[] textArray2 = new string[] { "MapFileParser.exe" };
                    return this.GlobalCacheDirectory.Combine(append).Combine(textArray2);
                }
            }
        }
    }
}

