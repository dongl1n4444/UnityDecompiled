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

    public class ClangToolChain : CppToolChain
    {
        [CompilerGenerated]
        private static Func<NPath, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, IEnumerable<string>> <>f__mg$cache0;
        private Func<string, IEnumerable<string>> AdditionalCompilerOptionsForSourceFile;

        public ClangToolChain(Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors) : base(architecture, buildConfiguration)
        {
            if (treatWarningsAsErrors)
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new Func<string, IEnumerable<string>>(null, (IntPtr) FlagsToMakeWarningsErrorsFor);
                }
                this.AdditionalCompilerOptionsForSourceFile = <>f__mg$cache0;
            }
        }

        public override bool CanBuildInCurrentEnvironment()
        {
            return PlatformUtils.IsOSX();
        }

        public override NPath CompilerExecutableFor(NPath sourceFile)
        {
            string[] extensions = new string[] { ".c" };
            return MacBuildToolPath(!sourceFile.HasExtension(extensions) ? "usr/bin/clang++" : "usr/bin/clang");
        }

        [DebuggerHidden]
        public override IEnumerable<string> CompilerFlagsFor(CppCompilationInstruction cppCompilationInstruction)
        {
            return new <CompilerFlagsFor>c__Iterator2 { 
                cppCompilationInstruction = cppCompilationInstruction,
                $this = this,
                $PC = -2
            };
        }

        [DebuggerHidden]
        private IEnumerable<string> DefaultCompilerFlags(CppCompilationInstruction cppCompilationInstruction)
        {
            return new <DefaultCompilerFlags>c__Iterator3 { 
                cppCompilationInstruction = cppCompilationInstruction,
                $this = this,
                $PC = -2
            };
        }

        [DebuggerHidden]
        private IEnumerable<string> DefaultLinkerFlags(IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibaries, NPath outputFile)
        {
            return new <DefaultLinkerFlags>c__Iterator4 { 
                outputFile = outputFile,
                $this = this,
                $PC = -2
            };
        }

        public override string ExecutableExtension()
        {
            return "";
        }

        [DebuggerHidden]
        private static IEnumerable<string> FlagsToMakeWarningsErrorsFor(string sourceFile)
        {
            return new <FlagsToMakeWarningsErrorsFor>c__Iterator5 { 
                sourceFile = sourceFile,
                $PC = -2
            };
        }

        protected override string GetInterestingOutputFromCompilationShellResult(Shell.ExecuteResult shellResult)
        {
            return shellResult.StdErr;
        }

        protected override string GetInterestingOutputFromLinkerShellResult(Shell.ExecuteResult shellResult)
        {
            return shellResult.StdErr;
        }

        private static NPath MacBuildToolPath(string path)
        {
            return new NPath("/" + path);
        }

        private static NPath MacDevSDKPath()
        {
            <MacDevSDKPath>c__AnonStorey6 storey = new <MacDevSDKPath>c__AnonStorey6 {
                sdksParentFolder = new NPath("/Applications/Xcode.app/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs")
            };
            string[] source = new string[] { "MacOSx10.11.sdk", "MacOSx10.10.sdk", "MacOSx10.9.sdk" };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<NPath, bool>(null, (IntPtr) <MacDevSDKPath>m__0);
            }
            return Enumerable.First<NPath>(Enumerable.Select<string, NPath>(source, new Func<string, NPath>(storey, (IntPtr) this.<>m__0)), <>f__am$cache0);
        }

        public override CppProgramBuilder.LinkerInvocation MakeLinkerInvocation(IEnumerable<NPath> objectFiles, NPath outputFile, IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, IEnumerable<string> specifiedLinkerFlags, CppToolChainContext toolChainContext)
        {
            List<NPath> list = new List<NPath>(objectFiles);
            List<string> inputs = new List<string> {
                "-o",
                outputFile.InQuotes()
            };
            string[] extensions = new string[] { this.DynamicLibraryExtension };
            if (outputFile.HasExtension(extensions))
            {
                inputs.Add("-dylib");
            }
            inputs.AddRange(base.ChooseLinkerFlags(staticLibraries, dynamicLibraries, outputFile, specifiedLinkerFlags, new Func<IEnumerable<NPath>, IEnumerable<NPath>, NPath, IEnumerable<string>>(this, (IntPtr) this.DefaultLinkerFlags)));
            list.AddRange(staticLibraries);
            list.AddRange(dynamicLibraries);
            inputs.AddRange(Extensions.InQuotes(staticLibraries, SlashMode.Native));
            inputs.AddRange(ExtensionMethods.InQuotes(this.ToolChainStaticLibraries()));
            inputs.AddRange(Extensions.InQuotes(dynamicLibraries, SlashMode.Native));
            inputs.AddRange(Extensions.InQuotes(objectFiles, SlashMode.Native));
            CppProgramBuilder.LinkerInvocation invocation = new CppProgramBuilder.LinkerInvocation();
            Shell.ExecuteArgs args = new Shell.ExecuteArgs {
                Executable = MacBuildToolPath("usr/bin/ld").ToString(),
                Arguments = ExtensionMethods.SeparateWithSpaces(inputs)
            };
            invocation.ExecuteArgs = args;
            invocation.ArgumentsInfluencingOutcome = inputs;
            invocation.FilesInfluencingOutcome = Enumerable.Concat<NPath>(objectFiles, staticLibraries);
            return invocation;
        }

        public override string ObjectExtension()
        {
            return ".o";
        }

        public override IEnumerable<string> OutputArgumentFor(NPath objectFile)
        {
            return new string[] { "-o", objectFile.InQuotes() };
        }

        [DebuggerHidden]
        public override IEnumerable<string> ToolChainDefines()
        {
            return new <ToolChainDefines>c__Iterator0 { $PC = -2 };
        }

        [DebuggerHidden]
        public override IEnumerable<NPath> ToolChainIncludePaths()
        {
            return new <ToolChainIncludePaths>c__Iterator1 { $PC = -2 };
        }

        public override string DynamicLibraryExtension
        {
            get
            {
                return ".dylib";
            }
        }

        public override string MapFileParserFormat
        {
            get
            {
                return "Clang";
            }
        }

        public override bool SupportsMapFileParser
        {
            get
            {
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <CompilerFlagsFor>c__Iterator2 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal IEnumerator<NPath> $locvar1;
            internal IEnumerator<string> $locvar2;
            internal int $PC;
            internal ClangToolChain $this;
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
                        this.$current = "-I\"" + this.<includePath>__1 + "\"";
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
                this.$locvar2 = this.$this.ChooseCompilerFlags(this.cppCompilationInstruction, new Func<CppCompilationInstruction, IEnumerable<string>>(this.$this, (IntPtr) this.DefaultCompilerFlags)).GetEnumerator();
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
                return new ClangToolChain.<CompilerFlagsFor>c__Iterator2 { 
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
        private sealed class <DefaultCompilerFlags>c__Iterator3 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal int $PC;
            internal ClangToolChain $this;
            internal string <p>__0;
            internal CppCompilationInstruction cppCompilationInstruction;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 13:
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
                        this.$current = "-g";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_02EA;

                    case 1:
                        this.$current = "-c";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_02EA;

                    case 2:
                        this.$current = "-fvisibility=hidden";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_02EA;

                    case 3:
                        this.$current = "-fno-strict-overflow";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_02EA;

                    case 4:
                        if (this.$this.BuildConfiguration != BuildConfiguration.Debug)
                        {
                            this.$current = "-O3";
                            if (!this.$disposing)
                            {
                                this.$PC = 6;
                            }
                        }
                        else
                        {
                            this.$current = "-O0";
                            if (!this.$disposing)
                            {
                                this.$PC = 5;
                            }
                        }
                        goto Label_02EA;

                    case 5:
                    case 6:
                        this.$current = "-mmacosx-version-min=10.8";
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_02EA;

                    case 7:
                        this.$current = "-arch";
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_02EA;

                    case 8:
                        this.$current = (this.$this.Architecture.Bits != 0x20) ? "x86_64" : "i386";
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_02EA;

                    case 9:
                        this.$current = "-isysroot";
                        if (!this.$disposing)
                        {
                            this.$PC = 10;
                        }
                        goto Label_02EA;

                    case 10:
                        this.$current = ClangToolChain.MacDevSDKPath().ToString();
                        if (!this.$disposing)
                        {
                            this.$PC = 11;
                        }
                        goto Label_02EA;

                    case 11:
                    {
                        string[] extensions = new string[] { ".cpp" };
                        if (this.cppCompilationInstruction.SourceFile.HasExtension(extensions))
                        {
                            this.$current = "-stdlib=libc++";
                            if (!this.$disposing)
                            {
                                this.$PC = 12;
                            }
                            goto Label_02EA;
                        }
                        break;
                    }
                    case 12:
                        break;

                    case 13:
                        goto Label_026A;

                    default:
                        goto Label_02E8;
                }
                if (this.$this.AdditionalCompilerOptionsForSourceFile == null)
                {
                    goto Label_02E1;
                }
                this.$locvar0 = this.$this.AdditionalCompilerOptionsForSourceFile.Invoke(this.cppCompilationInstruction.SourceFile.ToString()).GetEnumerator();
                num = 0xfffffffd;
            Label_026A:
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<p>__0 = this.$locvar0.Current;
                        this.$current = this.<p>__0;
                        if (!this.$disposing)
                        {
                            this.$PC = 13;
                        }
                        flag = true;
                        goto Label_02EA;
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
            Label_02E1:
                this.$PC = -1;
            Label_02E8:
                return false;
            Label_02EA:
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
                return new ClangToolChain.<DefaultCompilerFlags>c__Iterator3 { 
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
        private sealed class <DefaultLinkerFlags>c__Iterator4 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal ClangToolChain $this;
            internal NPath outputFile;

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
                        this.$current = "-arch";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_0194;

                    case 1:
                        this.$current = (this.$this.Architecture.Bits != 0x20) ? "x86_64" : "i386";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0194;

                    case 2:
                        this.$current = "-macosx_version_min";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_0194;

                    case 3:
                        this.$current = "10.8";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_0194;

                    case 4:
                        this.$current = "-lSystem";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_0194;

                    case 5:
                        this.$current = "-lc++";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_0194;

                    case 6:
                        this.$current = "-lpthread";
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_0194;

                    case 7:
                        this.$current = "-map";
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_0194;

                    case 8:
                        this.$current = this.outputFile.ChangeExtension("map").InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_0194;

                    case 9:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0194:
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
                return new ClangToolChain.<DefaultLinkerFlags>c__Iterator4 { 
                    $this = this.$this,
                    outputFile = this.outputFile
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
        private sealed class <FlagsToMakeWarningsErrorsFor>c__Iterator5 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal string sourceFile;

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
                        if (!this.sourceFile.Contains("generatedcpp") || this.sourceFile.Contains("pinvoke-targets.cpp"))
                        {
                            break;
                        }
                        this.$current = "-Werror";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_00DF;

                    case 1:
                        this.$current = "-Wno-extern-initializer";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_00DF;

                    case 2:
                        this.$current = "-Wno-trigraphs";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_00DF;

                    case 3:
                        this.$current = "-Wno-tautological-compare";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_00DF;

                    case 4:
                        break;

                    default:
                        goto Label_00DD;
                }
                this.$PC = -1;
            Label_00DD:
                return false;
            Label_00DF:
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
                return new ClangToolChain.<FlagsToMakeWarningsErrorsFor>c__Iterator5 { sourceFile = this.sourceFile };
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
        private sealed class <MacDevSDKPath>c__AnonStorey6
        {
            internal NPath sdksParentFolder;

            internal NPath <>m__0(string sdk)
            {
                string[] append = new string[] { sdk };
                return this.sdksParentFolder.Combine(append);
            }
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
                return new ClangToolChain.<ToolChainDefines>c__Iterator0();
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
        private sealed class <ToolChainIncludePaths>c__Iterator1 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
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
                        string[] append = new string[] { "usr/include" };
                        this.$current = ClangToolChain.MacDevSDKPath().Combine(append);
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
                return new ClangToolChain.<ToolChainIncludePaths>c__Iterator1();
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
    }
}

