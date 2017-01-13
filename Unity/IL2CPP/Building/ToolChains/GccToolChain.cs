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

    public class GccToolChain : CppToolChain
    {
        public GccToolChain(Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration) : base(architecture, buildConfiguration)
        {
        }

        public override bool CanBuildInCurrentEnvironment() => 
            PlatformUtils.IsLinux();

        public override NPath CompilerExecutableFor(NPath sourceFile)
        {
            string[] append = new string[1];
            string[] extensions = new string[] { ".c" };
            append[0] = !sourceFile.HasExtension(extensions) ? "g++" : "gcc";
            return new NPath("/usr/bin").Combine(append);
        }

        [DebuggerHidden]
        public override IEnumerable<string> CompilerFlagsFor(CppCompilationInstruction cppCompilationInstruction) => 
            new <CompilerFlagsFor>c__Iterator2 { 
                cppCompilationInstruction = cppCompilationInstruction,
                $this = this,
                $PC = -2
            };

        [DebuggerHidden]
        private IEnumerable<string> DefaultCompilerFlagsFor(CppCompilationInstruction sourceFile) => 
            new <DefaultCompilerFlagsFor>c__Iterator3 { 
                $this = this,
                $PC = -2
            };

        public override bool DynamicLibrariesHaveToSitNextToExecutable() => 
            true;

        public override string ExecutableExtension() => 
            "";

        protected override string GetInterestingOutputFromCompilationShellResult(Shell.ExecuteResult shellResult) => 
            shellResult.StdErr;

        protected override string GetInterestingOutputFromLinkerShellResult(Shell.ExecuteResult shellResult) => 
            shellResult.StdErr;

        protected virtual List<string> GetLinkerArgs(IEnumerable<NPath> objectFiles, NPath outputFile, IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, IEnumerable<string> specifiedLinkerFlags)
        {
            List<string> list = new List<string>();
            if ((base.Architecture is x64Architecture) || (base.Architecture is x86Architecture))
            {
                list.Add("-m" + base.Architecture.Bits);
            }
            string[] extensions = new string[] { this.DynamicLibraryExtension };
            if (outputFile.HasExtension(extensions))
            {
                list.Add("-shared");
            }
            list.AddRange(objectFiles.InQuotes(SlashMode.Native));
            list.AddRange(staticLibraries.InQuotes(SlashMode.Native));
            list.AddRange(this.ToolChainStaticLibraries().InQuotes());
            if (dynamicLibraries.Count<NPath>() > 1)
            {
                throw new ArgumentException("We've never tried to link to more than one shared library on linux. there be rpath dragons");
            }
            foreach (NPath path in dynamicLibraries)
            {
                list.Add("-L" + path.Parent.InQuotes());
                if (!path.FileName.StartsWith("lib"))
                {
                    throw new ArgumentException("linux only seems to support dynamic libraries whose name start with 'lib'.");
                }
                list.Add("-l" + path.FileNameWithoutExtension.Substring(3));
                list.Add("-Wl,-rpath,'$ORIGIN'");
            }
            string[] collection = new string[] { "-lpthread", "-ldl", "-o", outputFile.InQuotes() };
            list.AddRange(collection);
            return list;
        }

        public virtual NPath LinkerExecutableFor() => 
            new NPath("/usr/bin/g++");

        public override CppProgramBuilder.LinkerInvocation MakeLinkerInvocation(IEnumerable<NPath> objectFiles, NPath outputFile, IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, IEnumerable<string> specifiedLinkerFlags, CppToolChainContext toolChainContext)
        {
            List<string> inputs = this.GetLinkerArgs(objectFiles, outputFile, staticLibraries, dynamicLibraries, specifiedLinkerFlags);
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
            new <ToolChainDefines>c__Iterator0 { $PC = -2 };

        [DebuggerHidden]
        public override IEnumerable<NPath> ToolChainIncludePaths() => 
            new <ToolChainIncludePaths>c__Iterator1 { $PC = -2 };

        public override string DynamicLibraryExtension =>
            ".so";

        [CompilerGenerated]
        private sealed class <CompilerFlagsFor>c__Iterator2 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal IEnumerator<NPath> $locvar1;
            internal IEnumerator<string> $locvar2;
            internal int $PC;
            internal GccToolChain $this;
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
                        this.<define>__0 = this.$locvar0.Current;
                        this.$current = "-D" + this.<define>__0;
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
                this.$locvar1 = this.cppCompilationInstruction.IncludePaths.GetEnumerator();
                num = 0xfffffffd;
            Label_00E2:
                try
                {
                    while (this.$locvar1.MoveNext())
                    {
                        this.<includePath>__1 = this.$locvar1.Current;
                        this.$current = "-I" + this.<includePath>__1.InQuotes();
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
                this.$locvar2 = this.$this.ChooseCompilerFlags(this.cppCompilationInstruction, new Func<CppCompilationInstruction, IEnumerable<string>>(this.$this.DefaultCompilerFlagsFor)).GetEnumerator();
                num = 0xfffffffd;
            Label_0196:
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
                this.$current = this.cppCompilationInstruction.SourceFile.InQuotes();
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
                return new GccToolChain.<CompilerFlagsFor>c__Iterator2 { 
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
        private sealed class <DefaultCompilerFlagsFor>c__Iterator3 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal GccToolChain $this;

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
                        this.$current = "-c";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_01EF;

                    case 1:
                        this.$current = "-g";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_01EF;

                    case 2:
                        this.$current = "-O0";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_01EF;

                    case 3:
                        this.$current = "-Wno-unused-value";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_01EF;

                    case 4:
                        this.$current = "-Wno-invalid-offsetof";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_01EF;

                    case 5:
                        this.$current = "-fvisibility=hidden";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_01EF;

                    case 6:
                        this.$current = "-fno-rtti";
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_01EF;

                    case 7:
                        this.$current = "-pthread";
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_01EF;

                    case 8:
                        this.$current = "-fPIC";
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_01EF;

                    case 9:
                        this.$current = "-fno-strict-overflow";
                        if (!this.$disposing)
                        {
                            this.$PC = 10;
                        }
                        goto Label_01EF;

                    case 10:
                        if (!(this.$this.Architecture is x64Architecture) && !(this.$this.Architecture is x86Architecture))
                        {
                            break;
                        }
                        this.$current = "-m" + this.$this.Architecture.Bits;
                        if (!this.$disposing)
                        {
                            this.$PC = 11;
                        }
                        goto Label_01EF;

                    case 11:
                        break;

                    default:
                        goto Label_01ED;
                }
                this.$PC = -1;
            Label_01ED:
                return false;
            Label_01EF:
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
                return new GccToolChain.<DefaultCompilerFlagsFor>c__Iterator3 { $this = this.$this };
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
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new GccToolChain.<ToolChainDefines>c__Iterator0();
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
                return new GccToolChain.<ToolChainIncludePaths>c__Iterator1();
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

