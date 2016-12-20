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
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;
    using Unity.TinyProfiling;

    public class IL2CPPOutputBuildDescription : ProgramBuildDescription
    {
        private readonly IEnumerable<string> _additionalDefines;
        private readonly IEnumerable<NPath> _additionalIncludeDirectories;
        private readonly NPath _cacheDirectory;
        private readonly CppToolChain _cppToolChain;
        private NPath _dataFolder;
        private bool _libIL2CPPAsDynamicLibrary;
        private NPath _libil2cppCacheDirectory;
        private NPath _mapFileParser;
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
        }

        public IL2CPPOutputBuildDescription(NPath sourceDirectory, NPath cacheDirectory, NPath outputFile, DotNetProfile dotnetProfile, CppToolChain cppToolChain, NPath dataFolder, bool forceRebuildMapFileParser, bool libil2cppAsDynamicLibrary, [Optional, DefaultParameterValue(null)] IEnumerable<string> additionalDefines, [Optional, DefaultParameterValue(null)] IEnumerable<NPath> additionalIncludeDirectories, [Optional, DefaultParameterValue(null)] IEnumerable<NPath> staticLibraries, [Optional, DefaultParameterValue(null)] IEnumerable<string> specifiedCompilerFlags, [Optional, DefaultParameterValue(null)] IEnumerable<string> specifiedLinkerFlags, [Optional, DefaultParameterValue(null)] NPath libil2cppCacheDirectory, [Optional, DefaultParameterValue(null)] NPath mapFileParser)
        {
            this._sourceDirectory = sourceDirectory;
            this._cacheDirectory = (cacheDirectory == null) ? null : cacheDirectory.EnsureDirectoryExists("");
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
            this._libIL2CPPAsDynamicLibrary = libil2cppAsDynamicLibrary;
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
                this._additionalDefines = Enumerable.Concat<string>(this._additionalDefines, second);
            }
        }

        [DebuggerHidden]
        public override IEnumerable<string> AdditionalDefinesFor(NPath sourceFile)
        {
            return new <AdditionalDefinesFor>c__Iterator1 { 
                sourceFile = sourceFile,
                $this = this,
                $PC = -2
            };
        }

        public override IEnumerable<NPath> AdditionalIncludePathsFor(NPath sourceFile)
        {
            if (this.IsBoehmFile(sourceFile))
            {
                return Enumerable.Concat<NPath>(this._additionalIncludeDirectories, this.BoehmIncludeDirs);
            }
            if (this.IsLibIL2CPPFile(sourceFile))
            {
                return Enumerable.Concat<NPath>(this._additionalIncludeDirectories, this.LibIL2CPPIncludeDirs);
            }
            return Enumerable.Concat<NPath>(this._additionalIncludeDirectories, EnumerableExtensions.Append<NPath>(this.LibIL2CPPIncludeDirs, this._sourceDirectory));
        }

        [DebuggerHidden]
        protected virtual IEnumerable<string> BoehmDefines()
        {
            return new <BoehmDefines>c__Iterator3 { $PC = -2 };
        }

        private NPath BuildMapFileParser()
        {
            using (TinyProfiler.Section("BuildMapFileParser", ""))
            {
                NPath cacheDirectory = (this._cacheDirectory == null) ? null : this._cacheDirectory.Combine(new string[] { "MapFileParserCache" }).EnsureDirectoryExists("");
                return CppProgramBuilder.Create(RuntimePlatform.Current, new MapFileParserBuildDescription(cacheDirectory), false, Unity.IL2CPP.Building.Architecture.BestThisMachineCanRun, BuildConfiguration.Release, this.ForceRebuildMapFileParser, false).Build();
            }
        }

        private CppCompilationInstruction CppCompilationInstructionFor(NPath sourceFile, NPath cacheDirectory)
        {
            return new CppCompilationInstruction { 
                SourceFile = sourceFile,
                Defines = this.AdditionalDefinesFor(sourceFile),
                IncludePaths = this.AdditionalIncludePathsFor(sourceFile),
                CompilerFlags = this.AdditionalCompilerFlags,
                CacheDirectory = cacheDirectory
            };
        }

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

        protected virtual bool IsBoehmFile(NPath sourceFile)
        {
            return sourceFile.IsChildOf(BoehmDir);
        }

        protected virtual bool IsLibIL2CPPFile(NPath sourceFile)
        {
            return sourceFile.IsChildOf(LibIL2CPPDir);
        }

        protected virtual IEnumerable<CppCompilationInstruction> LibIL2CPPCompileInstructions()
        {
            NPath[] foldersToGlob = new NPath[] { LibIL2CPPDir };
            string[] append = new string[] { "extra/gc.c" };
            return Enumerable.Select<NPath, CppCompilationInstruction>(EnumerableExtensions.Append<NPath>(SourceFilesIn(foldersToGlob), BoehmDir.Combine(append)), new Func<NPath, CppCompilationInstruction>(this, (IntPtr) this.<LibIL2CPPCompileInstructions>m__0));
        }

        public override void OnBeforeLink(NPath workingDirectory, IEnumerable<NPath> objectFiles, CppToolChainContext toolChainContext, bool forceRebuild, bool verbose)
        {
            if (this._libIL2CPPAsDynamicLibrary)
            {
                LibIL2CPPDynamicLibProgramDescription programBuildDescription = new LibIL2CPPDynamicLibProgramDescription(this.LibIL2CPPCompileInstructions(), this.LibIL2CppDynamicLibraryLocation, this._libil2cppCacheDirectory);
                new CppProgramBuilder(this._cppToolChain, programBuildDescription, verbose, forceRebuild).Build();
            }
        }

        private void RunMapFileParser(CppToolChain toolChain, NPath outputFile, NPath mapFileParser)
        {
            NPath path = outputFile.ChangeExtension("map");
            string[] append = new string[] { "SymbolMap" };
            NPath path2 = this._dataFolder.Combine(append);
            string str = string.Format("-format={0} \"{1}\" \"{2}\"", toolChain.MapFileParserFormat, path, path2);
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
                <>f__am$cache0 = new Func<NPath, IEnumerable<NPath>>(null, (IntPtr) <SourceFilesIn>m__1);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<NPath, bool>(null, (IntPtr) <SourceFilesIn>m__2);
            }
            return Enumerable.Where<NPath>(Enumerable.SelectMany<NPath, NPath>(foldersToGlob, <>f__am$cache0), <>f__am$cache1);
        }

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

        public override IEnumerable<string> AdditionalLinkerFlags
        {
            get
            {
                IEnumerable<string> enumerable;
                if (this._specifiedLinkerFlags != null)
                {
                    enumerable = this._specifiedLinkerFlags;
                }
                else
                {
                    enumerable = Enumerable.Empty<string>();
                }
                return enumerable;
            }
        }

        protected static NPath BoehmDir
        {
            get
            {
                return ((CommonPaths.Il2CppRoot == null) ? null : CommonPaths.Il2CppRoot.Combine(new string[] { "external/boehmgc" }));
            }
        }

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

        public override IEnumerable<CppCompilationInstruction> CppCompileInstructions
        {
            get
            {
                return new <>c__Iterator0 { 
                    $this = this,
                    $PC = -2
                };
            }
        }

        public override IEnumerable<NPath> DynamicLibraries
        {
            get
            {
                return new <>c__Iterator2 { 
                    $this = this,
                    $PC = -2
                };
            }
        }

        public bool ForceRebuildMapFileParser { get; private set; }

        public override NPath GlobalCacheDirectory
        {
            get
            {
                return this._cacheDirectory;
            }
        }

        protected static NPath LibIL2CPPDir
        {
            get
            {
                return ((CommonPaths.Il2CppRoot == null) ? null : CommonPaths.Il2CppRoot.Combine(new string[] { "libil2cpp" }));
            }
        }

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

        public override IEnumerable<NPath> StaticLibraries
        {
            get
            {
                return this._staticLibraries;
            }
        }

        [CompilerGenerated]
        private sealed class <>c__Iterator0 : IEnumerable, IEnumerable<CppCompilationInstruction>, IEnumerator, IDisposable, IEnumerator<CppCompilationInstruction>
        {
            internal CppCompilationInstruction $current;
            internal bool $disposing;
            internal IEnumerator<CppCompilationInstruction> $locvar0;
            internal IEnumerator<CppCompilationInstruction> $locvar1;
            internal int $PC;
            internal IL2CPPOutputBuildDescription $this;
            internal CppCompilationInstruction <i>__0;
            internal CppCompilationInstruction <i>__1;

            internal CppCompilationInstruction <>m__0(NPath sourceFile)
            {
                return this.$this.CppCompilationInstructionFor(sourceFile, this.$this._cacheDirectory);
            }

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
                    {
                        NPath[] foldersToGlob = new NPath[] { this.$this._sourceDirectory };
                        this.$locvar0 = Enumerable.Select<NPath, CppCompilationInstruction>(IL2CPPOutputBuildDescription.SourceFilesIn(foldersToGlob), new Func<NPath, CppCompilationInstruction>(this, (IntPtr) this.<>m__0)).GetEnumerator();
                        num = 0xfffffffd;
                        break;
                    }
                    case 1:
                        break;

                    case 2:
                        goto Label_0104;

                    default:
                        goto Label_017F;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<i>__0 = this.$locvar0.Current;
                        this.$current = this.<i>__0;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_0181;
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
                if (this.$this._libIL2CPPAsDynamicLibrary)
                {
                    goto Label_017F;
                }
                this.$locvar1 = this.$this.LibIL2CPPCompileInstructions().GetEnumerator();
                num = 0xfffffffd;
            Label_0104:
                try
                {
                    while (this.$locvar1.MoveNext())
                    {
                        this.<i>__1 = this.$locvar1.Current;
                        this.$current = this.<i>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        flag = true;
                        goto Label_0181;
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
                this.$PC = -1;
            Label_017F:
                return false;
            Label_0181:
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
                return new IL2CPPOutputBuildDescription.<>c__Iterator0 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<Unity.IL2CPP.Building.CppCompilationInstruction>.GetEnumerator();
            }

            CppCompilationInstruction IEnumerator<CppCompilationInstruction>.Current
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
        private sealed class <>c__Iterator2 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
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
                        if (!this.$this._libIL2CPPAsDynamicLibrary)
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
                        goto Label_005E;
                }
                this.$PC = -1;
            Label_005E:
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
                return new IL2CPPOutputBuildDescription.<>c__Iterator2 { $this = this.$this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();
            }

            NPath IEnumerator<NPath>.Current
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
        private sealed class <AdditionalDefinesFor>c__Iterator1 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal IEnumerator<string> $locvar1;
            internal int $PC;
            internal IL2CPPOutputBuildDescription $this;
            internal string <define>__0;
            internal string <define>__1;
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
                        goto Label_00EA;

                    case 3:
                        goto Label_01AD;

                    default:
                        goto Label_01B4;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<define>__0 = this.$locvar0.Current;
                        this.$current = this.<define>__0;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_01B6;
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
                    goto Label_015E;
                }
                this.$locvar1 = this.$this.BoehmDefines().GetEnumerator();
                num = 0xfffffffd;
            Label_00EA:
                try
                {
                    while (this.$locvar1.MoveNext())
                    {
                        this.<define>__1 = this.$locvar1.Current;
                        this.$current = this.<define>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        flag = true;
                        goto Label_01B6;
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
            Label_015E:
                if (this.$this._libIL2CPPAsDynamicLibrary)
                {
                    this.$current = !this.$this.IsLibIL2CPPFile(this.sourceFile) ? "LIBIL2CPP_IMPORT_CODEGEN_API" : "LIBIL2CPP_EXPORT_CODEGEN_API";
                    if (!this.$disposing)
                    {
                        this.$PC = 3;
                    }
                    goto Label_01B6;
                }
            Label_01AD:
                this.$PC = -1;
            Label_01B4:
                return false;
            Label_01B6:
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
                return new IL2CPPOutputBuildDescription.<AdditionalDefinesFor>c__Iterator1 { 
                    $this = this.$this,
                    sourceFile = this.sourceFile
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
        private sealed class <BoehmDefines>c__Iterator3 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
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
                return new IL2CPPOutputBuildDescription.<BoehmDefines>c__Iterator3();
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

            public override IEnumerable<CppCompilationInstruction> CppCompileInstructions
            {
                get
                {
                    return this._cppCompileInstructions;
                }
            }

            public override NPath GlobalCacheDirectory
            {
                get
                {
                    return this._cacheDirectory;
                }
            }
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
                        <>f__am$cache0 = new Func<NPath, bool>(null, (IntPtr) <get_CppCompileInstructions>m__0);
                    }
                    return Enumerable.Select<NPath, CppCompilationInstruction>(Enumerable.Where<NPath>(CommonPaths.Il2CppRoot.Combine(append).Files("*.cpp", true), <>f__am$cache0), new Func<NPath, CppCompilationInstruction>(this, (IntPtr) this.<get_CppCompileInstructions>m__1));
                }
            }

            public override NPath GlobalCacheDirectory
            {
                get
                {
                    return this._cacheDirectory;
                }
            }

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

