namespace Unity.IL2CPP.Building.ToolChains
{
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Common;

    public class TizenToolChain : GccLinuxToolChain
    {
        private TizenSDKUtilities _tizenSDK;
        private Func<string, IEnumerable<string>> AdditionalCompilerOptionsForSourceFile;

        public TizenToolChain(Unity.IL2CPP.Common.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors, NPath toolchainPath) : base(architecture, buildConfiguration)
        {
            this._tizenSDK = new TizenSDKUtilities(toolchainPath, architecture);
        }

        public override bool CanBuildInCurrentEnvironment() => 
            ((PlatformUtils.IsOSX() || PlatformUtils.IsWindows()) || PlatformUtils.IsLinux());

        public override NPath CompilerExecutableFor(NPath sourceFile)
        {
            string[] extensions = new string[] { ".c" };
            if (sourceFile.HasExtension(extensions))
            {
                return new NPath(this._tizenSDK.GetCompilerPrefix());
            }
            return new NPath(this._tizenSDK.GetCompilerPrefix() + "++");
        }

        [DebuggerHidden]
        public override IEnumerable<string> CompilerFlagsFor(CppCompilationInstruction cppSourceCompileInstruction) => 
            new <CompilerFlagsFor>c__Iterator4 { 
                cppSourceCompileInstruction = cppSourceCompileInstruction,
                $this = this,
                $PC = -2
            };

        [DebuggerHidden]
        private IEnumerable<string> DefaultCompilerFlagsFor(CppCompilationInstruction cppCompilationInstruction) => 
            new <DefaultCompilerFlagsFor>c__Iterator3 { 
                cppCompilationInstruction = cppCompilationInstruction,
                $this = this,
                $PC = -2
            };

        [DebuggerHidden]
        private IEnumerable<string> DefaultLinkerFlags(IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibaries, NPath outputFile) => 
            new <DefaultLinkerFlags>c__Iterator2 { 
                $this = this,
                $PC = -2
            };

        public override string ExecutableExtension() => 
            "";

        protected override string GetInterestingOutputFromCompilationShellResult(Shell.ExecuteResult shellResult) => 
            shellResult.StdErr;

        protected override string GetInterestingOutputFromLinkerShellResult(Shell.ExecuteResult shellResult) => 
            shellResult.StdErr;

        public override NPath LinkerExecutableFor() => 
            this._tizenSDK.GetLinkerPath();

        public override CppProgramBuilder.LinkerInvocation MakeLinkerInvocation(IEnumerable<NPath> objectFiles, NPath outputFile, IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, IEnumerable<string> specifiedLinkerFlags, CppToolChainContext toolChainContext)
        {
            List<NPath> list = new List<NPath>(objectFiles);
            List<string> inputs = new List<string> {
                "-o",
                outputFile.InQuotes()
            };
            inputs.AddRange(base.ChooseLinkerFlags(staticLibraries, dynamicLibraries, outputFile, specifiedLinkerFlags, new Func<IEnumerable<NPath>, IEnumerable<NPath>, NPath, IEnumerable<string>>(this.DefaultLinkerFlags)));
            list.AddRange(staticLibraries);
            list.AddRange(dynamicLibraries);
            inputs.AddRange(staticLibraries.InQuotes(SlashMode.Native));
            inputs.AddRange(this.ToolChainStaticLibraries().InQuotes());
            inputs.AddRange(dynamicLibraries.InQuotes(SlashMode.Native));
            inputs.AddRange(objectFiles.InQuotes(SlashMode.Native));
            CppProgramBuilder.LinkerInvocation invocation = new CppProgramBuilder.LinkerInvocation();
            Shell.ExecuteArgs args = new Shell.ExecuteArgs {
                Executable = this.LinkerExecutableFor().ToString(),
                Arguments = inputs.SeparateWithSpaces()
            };
            invocation.ExecuteArgs = args;
            invocation.ArgumentsInfluencingOutcome = inputs;
            invocation.FilesInfluencingOutcome = objectFiles.Concat<NPath>(staticLibraries);
            return invocation;
        }

        public override string ObjectExtension() => 
            ".o";

        public override IEnumerable<string> OutputArgumentFor(NPath objectFile) => 
            new string[] { "-o", objectFile.InQuotes() };

        [DebuggerHidden]
        public override IEnumerable<string> ToolChainDefines() => 
            new <ToolChainDefines>c__Iterator0 { 
                $this = this,
                $PC = -2
            };

        [DebuggerHidden]
        public override IEnumerable<NPath> ToolChainIncludePaths() => 
            new <ToolChainIncludePaths>c__Iterator1 { $PC = -2 };

        public override string DynamicLibraryExtension =>
            ".so";

        [CompilerGenerated]
        private sealed class <CompilerFlagsFor>c__Iterator4 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal IEnumerator<NPath> $locvar1;
            internal IEnumerator<string> $locvar2;
            internal int $PC;
            internal TizenToolChain $this;
            internal string <compilerFlag>__3;
            internal string <define>__1;
            internal NPath <includePath>__2;
            internal CppCompilationInstruction cppSourceCompileInstruction;

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
                        this.$locvar0 = this.cppSourceCompileInstruction.Defines.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    case 2:
                        goto Label_00E2;

                    case 3:
                        goto Label_0196;

                    case 4:
                        this.$PC = -1;
                        goto Label_023B;

                    default:
                        goto Label_023B;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<define>__1 = this.$locvar0.Current;
                        this.$current = "-D" + this.<define>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_023D;
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
                this.$locvar1 = this.cppSourceCompileInstruction.IncludePaths.GetEnumerator();
                num = 0xfffffffd;
            Label_00E2:
                try
                {
                    while (this.$locvar1.MoveNext())
                    {
                        this.<includePath>__2 = this.$locvar1.Current;
                        this.$current = "-I" + this.<includePath>__2.InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        flag = true;
                        goto Label_023D;
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
                this.$locvar2 = this.$this.ChooseCompilerFlags(this.cppSourceCompileInstruction, new Func<CppCompilationInstruction, IEnumerable<string>>(this.$this.DefaultCompilerFlagsFor)).GetEnumerator();
                num = 0xfffffffd;
            Label_0196:
                try
                {
                    while (this.$locvar2.MoveNext())
                    {
                        this.<compilerFlag>__3 = this.$locvar2.Current;
                        this.$current = this.<compilerFlag>__3;
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        flag = true;
                        goto Label_023D;
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
                this.$current = this.cppSourceCompileInstruction.SourceFile.InQuotes();
                if (!this.$disposing)
                {
                    this.$PC = 4;
                }
                goto Label_023D;
            Label_023B:
                return false;
            Label_023D:
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
                return new TizenToolChain.<CompilerFlagsFor>c__Iterator4 { 
                    $this = this.$this,
                    cppSourceCompileInstruction = this.cppSourceCompileInstruction
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
        private sealed class <DefaultCompilerFlagsFor>c__Iterator3 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal int $PC;
            internal TizenToolChain $this;
            internal string <flag>__1;
            internal CppCompilationInstruction cppCompilationInstruction;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 15:
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
                        this.$current = this.$this._tizenSDK.GetToolchainFlags();
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_0327;

                    case 1:
                        this.$current = "-c";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0327;

                    case 2:
                        this.$current = "-g";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_0327;

                    case 3:
                        this.$current = "-O3";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_0327;

                    case 4:
                        this.$current = "-DNDEBUG";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_0327;

                    case 5:
                        this.$current = "-fexceptions";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_0327;

                    case 6:
                        this.$current = "-DTIZEN";
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_0327;

                    case 7:
                        this.$current = "--sysroot=" + this.$this._tizenSDK.GetSysroot().InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_0327;

                    case 8:
                        this.$current = "-fPIC";
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_0327;

                    case 9:
                        this.$current = "-fdata-sections";
                        if (!this.$disposing)
                        {
                            this.$PC = 10;
                        }
                        goto Label_0327;

                    case 10:
                        this.$current = "-ffunction-sections";
                        if (!this.$disposing)
                        {
                            this.$PC = 11;
                        }
                        goto Label_0327;

                    case 11:
                        this.$current = "-fmessage-length=0";
                        if (!this.$disposing)
                        {
                            this.$PC = 12;
                        }
                        goto Label_0327;

                    case 12:
                        this.$current = this.$this._tizenSDK.GetTargetFlags();
                        if (!this.$disposing)
                        {
                            this.$PC = 13;
                        }
                        goto Label_0327;

                    case 13:
                    {
                        string[] extensions = new string[] { ".cpp" };
                        if (!this.cppCompilationInstruction.SourceFile.HasExtension(extensions))
                        {
                            break;
                        }
                        this.$current = "-std=c++11";
                        if (!this.$disposing)
                        {
                            this.$PC = 14;
                        }
                        goto Label_0327;
                    }
                    case 14:
                        break;

                    case 15:
                        goto Label_02A7;

                    default:
                        goto Label_0325;
                }
                if (this.$this.AdditionalCompilerOptionsForSourceFile == null)
                {
                    goto Label_031E;
                }
                this.$locvar0 = this.$this.AdditionalCompilerOptionsForSourceFile(this.cppCompilationInstruction.SourceFile.ToString()).GetEnumerator();
                num = 0xfffffffd;
            Label_02A7:
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<flag>__1 = this.$locvar0.Current;
                        this.$current = this.<flag>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 15;
                        }
                        flag = true;
                        goto Label_0327;
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
            Label_031E:
                this.$PC = -1;
            Label_0325:
                return false;
            Label_0327:
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
                return new TizenToolChain.<DefaultCompilerFlagsFor>c__Iterator3 { 
                    $this = this.$this,
                    cppCompilationInstruction = this.cppCompilationInstruction
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
        private sealed class <DefaultLinkerFlags>c__Iterator2 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal TizenToolChain $this;

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
                        this.$current = "-lpthread";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_01D0;

                    case 1:
                        this.$current = "-ldl";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_01D0;

                    case 2:
                        this.$current = "-ldlog";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_01D0;

                    case 3:
                        this.$current = "-shared";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_01D0;

                    case 4:
                        this.$current = this.$this._tizenSDK.GetToolchainFlags();
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_01D0;

                    case 5:
                        this.$current = "-Xlinker --as-needed";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_01D0;

                    case 6:
                        this.$current = "-Wl,--no-undefined";
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_01D0;

                    case 7:
                        this.$current = "-Wl,-Bsymbolic";
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_01D0;

                    case 8:
                        this.$current = "--sysroot=" + this.$this._tizenSDK.GetSysroot().InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_01D0;

                    case 9:
                        this.$current = "-L" + (this.$this._tizenSDK.GetSysroot() + "/usr/lib").InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 10;
                        }
                        goto Label_01D0;

                    case 10:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_01D0:
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
                return new TizenToolChain.<DefaultLinkerFlags>c__Iterator2 { $this = this.$this };
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
        private sealed class <ToolChainDefines>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal int $PC;
            internal TizenToolChain $this;
            internal string <f>__1;

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
                        this.$locvar0 = this.$this.<ToolChainDefines>__BaseCallProxy0().GetEnumerator();
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
                        this.<f>__1 = this.$locvar0.Current;
                        this.$current = this.<f>__1;
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
                this.$current = "TIZEN";
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
                return new TizenToolChain.<ToolChainDefines>c__Iterator0 { $this = this.$this };
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
        private sealed class <ToolChainIncludePaths>c__Iterator1 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;

            [DebuggerHidden]
            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                this.$PC = -1;
                if (this.$PC == 0)
                {
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
                return new TizenToolChain.<ToolChainIncludePaths>c__Iterator1();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<NiceIO.NPath>.GetEnumerator();

            NPath IEnumerator<NPath>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

