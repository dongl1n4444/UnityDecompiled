namespace Unity.IL2CPP.Building.ToolChains
{
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.ToolChains.Android;
    using Unity.IL2CPP.Common;
    using Unity.TinyProfiling;

    public class AndroidToolChain : CppToolChain
    {
        private AndroidNDKUtilities _androidNDK;
        [CompilerGenerated]
        private static Func<string, IEnumerable<string>> <>f__mg$cache0;
        private Func<string, IEnumerable<string>> AdditionalCompilerOptionsForSourceFile;

        public AndroidToolChain(Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors, NPath toolchainPath) : base(architecture, buildConfiguration)
        {
            this._androidNDK = new AndroidNDKUtilities(toolchainPath, architecture);
            if (treatWarningsAsErrors)
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new Func<string, IEnumerable<string>>(AndroidToolChain.FlagsToMakeWarningsErrorsFor);
                }
                this.AdditionalCompilerOptionsForSourceFile = <>f__mg$cache0;
            }
        }

        public override bool CanBuildInCurrentEnvironment() => 
            ((PlatformUtils.IsOSX() || PlatformUtils.IsWindows()) || PlatformUtils.IsLinux());

        public override NPath CompilerExecutableFor(NPath sourceFile)
        {
            string[] extensions = new string[] { ".c" };
            return (!sourceFile.HasExtension(extensions) ? this._androidNDK.CppCompilerPath : this._androidNDK.CCompilerPath);
        }

        [DebuggerHidden]
        public override IEnumerable<string> CompilerFlagsFor(CppCompilationInstruction cppCompilationInstruction) => 
            new <CompilerFlagsFor>c__Iterator2 { 
                cppCompilationInstruction = cppCompilationInstruction,
                $this = this,
                $PC = -2
            };

        [DebuggerHidden]
        private IEnumerable<string> DefaultCompilerFlags(CppCompilationInstruction cppCompilationInstruction) => 
            new <DefaultCompilerFlags>c__Iterator3 { 
                cppCompilationInstruction = cppCompilationInstruction,
                $this = this,
                $PC = -2
            };

        [DebuggerHidden]
        private IEnumerable<string> DefaultLinkerFlags(IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibaries, NPath outputFile) => 
            new <DefaultLinkerFlags>c__Iterator4 { 
                outputFile = outputFile,
                $this = this,
                $PC = -2
            };

        public override string ExecutableExtension() => 
            "";

        [DebuggerHidden]
        private static IEnumerable<string> FlagsToMakeWarningsErrorsFor(string sourceFile) => 
            new <FlagsToMakeWarningsErrorsFor>c__Iterator5 { 
                sourceFile = sourceFile,
                $PC = -2
            };

        protected override string GetInterestingOutputFromCompilationShellResult(Shell.ExecuteResult shellResult) => 
            shellResult.StdErr;

        protected override string GetInterestingOutputFromLinkerShellResult(Shell.ExecuteResult shellResult) => 
            shellResult.StdErr;

        public override CppProgramBuilder.LinkerInvocation MakeLinkerInvocation(IEnumerable<NPath> objectFiles, NPath outputFile, IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, IEnumerable<string> specifiedLinkerFlags, CppToolChainContext toolChainContext)
        {
            string tempFileName = Path.GetTempFileName();
            File.WriteAllText(tempFileName, objectFiles.InQuotes(SlashMode.Native).SeparateWithSpaces(), Encoding.ASCII);
            List<string> inputs = new List<string> {
                "@" + tempFileName.InQuotes(),
                "-o",
                outputFile.InQuotes()
            };
            inputs.AddRange(base.ChooseLinkerFlags(staticLibraries, dynamicLibraries, outputFile, specifiedLinkerFlags, new Func<IEnumerable<NPath>, IEnumerable<NPath>, NPath, IEnumerable<string>>(this.DefaultLinkerFlags)));
            inputs.AddRange(staticLibraries.InQuotes(SlashMode.Native));
            foreach (NPath path in dynamicLibraries)
            {
                inputs.Add("-l " + path.InQuotes());
            }
            List<NPath> list3 = new List<NPath>(objectFiles);
            list3.AddRange(staticLibraries);
            list3.AddRange(dynamicLibraries);
            CppProgramBuilder.LinkerInvocation invocation = new CppProgramBuilder.LinkerInvocation();
            Shell.ExecuteArgs args = new Shell.ExecuteArgs {
                Executable = this._androidNDK.LinkerPath.ToString(),
                Arguments = inputs.SeparateWithSpaces()
            };
            invocation.ExecuteArgs = args;
            invocation.ArgumentsInfluencingOutcome = inputs;
            invocation.FilesInfluencingOutcome = list3;
            return invocation;
        }

        public override string ObjectExtension() => 
            ".o";

        public override void OnAfterLink(NPath outputFile, CppToolChainContext toolChainContext, bool forceRebuild, bool verbose)
        {
            NPath path = new NPath(outputFile + ".debug");
            if (File.Exists(path.ToString()))
            {
                Console.WriteLine("Skipping Android library stripping");
            }
            else
            {
                File.Move(outputFile.ToString(), path.ToString());
                List<string> inputs = new List<string> {
                    path.InQuotes(),
                    outputFile.InQuotes(),
                    (base.BuildConfiguration != BuildConfiguration.Debug) ? "--strip-all" : "--strip-debug"
                };
                Shell.ExecuteArgs executeArgs = new Shell.ExecuteArgs {
                    Executable = this._androidNDK.ObjCopyPath.ToString(),
                    Arguments = inputs.SeparateWithSpaces()
                };
                using (TinyProfiler.Section("strip", ""))
                {
                    Shell.Execute(executeArgs, null);
                }
            }
        }

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

        public override string MapFileParserFormat =>
            "GCC";

        public override bool SupportsMapFileParser =>
            true;

        [CompilerGenerated]
        private sealed class <CompilerFlagsFor>c__Iterator2 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal IEnumerator<NPath> $locvar1;
            internal IEnumerator<NPath> $locvar2;
            internal IEnumerator<string> $locvar3;
            internal int $PC;
            internal AndroidToolChain $this;
            internal string <compilerFlag>__4;
            internal string <define>__1;
            internal NPath <includePath>__2;
            internal NPath <includePath>__3;
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

                    case 4:
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
                        goto Label_00E6;

                    case 3:
                        goto Label_0188;

                    case 4:
                        goto Label_023C;

                    case 5:
                        this.$PC = -1;
                        goto Label_02E1;

                    default:
                        goto Label_02E1;
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
                        goto Label_02E3;
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
            Label_00E6:
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
                        goto Label_02E3;
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
                this.$locvar2 = this.$this._androidNDK.GnuStlIncludePaths.GetEnumerator();
                num = 0xfffffffd;
            Label_0188:
                try
                {
                    while (this.$locvar2.MoveNext())
                    {
                        this.<includePath>__3 = this.$locvar2.Current;
                        this.$current = "-I" + this.<includePath>__3.InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        flag = true;
                        goto Label_02E3;
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
                this.$locvar3 = this.$this.ChooseCompilerFlags(this.cppCompilationInstruction, new Func<CppCompilationInstruction, IEnumerable<string>>(this.$this.DefaultCompilerFlags)).GetEnumerator();
                num = 0xfffffffd;
            Label_023C:
                try
                {
                    while (this.$locvar3.MoveNext())
                    {
                        this.<compilerFlag>__4 = this.$locvar3.Current;
                        this.$current = this.<compilerFlag>__4;
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        flag = true;
                        goto Label_02E3;
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
                this.$current = this.cppCompilationInstruction.SourceFile.InQuotes();
                if (!this.$disposing)
                {
                    this.$PC = 5;
                }
                goto Label_02E3;
            Label_02E1:
                return false;
            Label_02E3:
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
                return new AndroidToolChain.<CompilerFlagsFor>c__Iterator2 { 
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
        private sealed class <DefaultCompilerFlags>c__Iterator3 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal IEnumerator<string> $locvar1;
            internal int $PC;
            internal AndroidToolChain $this;
            internal string <flag>__1;
            internal string <flag>__2;
            internal CppCompilationInstruction cppCompilationInstruction;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 20:
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

                    case 0x15:
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
                        this.$current = "-c";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_04A7;

                    case 1:
                        this.$current = "-g";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_04A7;

                    case 2:
                        this.$current = "-DNDEBUG";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_04A7;

                    case 3:
                        this.$current = "-funwind-tables";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_04A7;

                    case 4:
                        this.$current = "-fno-limit-debug-info";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_04A7;

                    case 5:
                        this.$current = "-fPIC";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_04A7;

                    case 6:
                        this.$current = "-fdata-sections";
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_04A7;

                    case 7:
                        this.$current = "-ffunction-sections";
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_04A7;

                    case 8:
                        this.$current = "-Wa,--noexecstack";
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_04A7;

                    case 9:
                        this.$current = "-fno-rtti";
                        if (!this.$disposing)
                        {
                            this.$PC = 10;
                        }
                        goto Label_04A7;

                    case 10:
                    {
                        string[] extensions = new string[] { ".cpp" };
                        if (!this.cppCompilationInstruction.SourceFile.HasExtension(extensions))
                        {
                            break;
                        }
                        this.$current = "-std=c++11";
                        if (!this.$disposing)
                        {
                            this.$PC = 11;
                        }
                        goto Label_04A7;
                    }
                    case 11:
                        break;

                    case 12:
                        this.$current = "-fvisibility=hidden";
                        if (!this.$disposing)
                        {
                            this.$PC = 13;
                        }
                        goto Label_04A7;

                    case 13:
                        this.$current = "-fvisibility-inlines-hidden";
                        if (!this.$disposing)
                        {
                            this.$PC = 14;
                        }
                        goto Label_04A7;

                    case 14:
                        this.$current = "-fno-strict-overflow";
                        if (!this.$disposing)
                        {
                            this.$PC = 15;
                        }
                        goto Label_04A7;

                    case 15:
                        this.$current = (this.$this.BuildConfiguration != BuildConfiguration.Debug) ? "-Os" : "-O0";
                        if (!this.$disposing)
                        {
                            this.$PC = 0x10;
                        }
                        goto Label_04A7;

                    case 0x10:
                        this.$current = "--sysroot " + this.$this._androidNDK.SysRoot.InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 0x11;
                        }
                        goto Label_04A7;

                    case 0x11:
                        this.$current = "-gcc-toolchain " + this.$this._androidNDK.GccToolchain.InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 0x12;
                        }
                        goto Label_04A7;

                    case 0x12:
                        this.$current = "-target " + this.$this._androidNDK.Platform;
                        if (!this.$disposing)
                        {
                            this.$PC = 0x13;
                        }
                        goto Label_04A7;

                    case 0x13:
                        this.$locvar0 = this.$this._androidNDK.ArchitectureCompilerFlags.GetEnumerator();
                        num = 0xfffffffd;
                        goto Label_0371;

                    case 20:
                        goto Label_0371;

                    case 0x15:
                        goto Label_0427;

                    default:
                        goto Label_04A5;
                }
                this.$current = "-fno-strict-aliasing";
                if (!this.$disposing)
                {
                    this.$PC = 12;
                }
                goto Label_04A7;
            Label_0371:
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<flag>__1 = this.$locvar0.Current;
                        this.$current = this.<flag>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 20;
                        }
                        flag = true;
                        goto Label_04A7;
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
                if (this.$this.AdditionalCompilerOptionsForSourceFile == null)
                {
                    goto Label_049E;
                }
                this.$locvar1 = this.$this.AdditionalCompilerOptionsForSourceFile(this.cppCompilationInstruction.SourceFile.ToString()).GetEnumerator();
                num = 0xfffffffd;
            Label_0427:
                try
                {
                    while (this.$locvar1.MoveNext())
                    {
                        this.<flag>__2 = this.$locvar1.Current;
                        this.$current = this.<flag>__2;
                        if (!this.$disposing)
                        {
                            this.$PC = 0x15;
                        }
                        flag = true;
                        goto Label_04A7;
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
            Label_049E:
                this.$PC = -1;
            Label_04A5:
                return false;
            Label_04A7:
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
                return new AndroidToolChain.<DefaultCompilerFlags>c__Iterator3 { 
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
        private sealed class <DefaultLinkerFlags>c__Iterator4 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal int $PC;
            internal AndroidToolChain $this;
            internal string <flag>__1;
            internal NPath outputFile;

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
                        this.$current = "-Wl,-soname," + this.outputFile.FileName;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_02F7;

                    case 1:
                        this.$current = "-shared";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_02F7;

                    case 2:
                        this.$current = "-Wl,--no-undefined";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_02F7;

                    case 3:
                        this.$current = "-Wl,-z,noexecstack";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_02F7;

                    case 4:
                        this.$current = "-Wl,--gc-sections";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_02F7;

                    case 5:
                        this.$current = "-Wl,--build-id";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_02F7;

                    case 6:
                        this.$current = "--sysroot " + this.$this._androidNDK.SysRoot.InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_02F7;

                    case 7:
                        this.$current = "-gcc-toolchain " + this.$this._androidNDK.GccToolchain.InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_02F7;

                    case 8:
                        this.$current = "-target " + this.$this._androidNDK.Platform;
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_02F7;

                    case 9:
                        this.$current = "-L " + this.$this._androidNDK.GnuStlLibrary.InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 10;
                        }
                        goto Label_02F7;

                    case 10:
                        this.$current = "-lgnustl_static";
                        if (!this.$disposing)
                        {
                            this.$PC = 11;
                        }
                        goto Label_02F7;

                    case 11:
                        this.$current = "-Xlinker -Map=" + this.outputFile.ChangeExtension("map").InQuotes();
                        if (!this.$disposing)
                        {
                            this.$PC = 12;
                        }
                        goto Label_02F7;

                    case 12:
                        this.$locvar0 = this.$this._androidNDK.ArchitectureLinkerFlags.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 13:
                        break;

                    default:
                        goto Label_02F5;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<flag>__1 = this.$locvar0.Current;
                        this.$current = this.<flag>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 13;
                        }
                        flag = true;
                        goto Label_02F7;
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
                this.$PC = -1;
            Label_02F5:
                return false;
            Label_02F7:
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
                return new AndroidToolChain.<DefaultLinkerFlags>c__Iterator4 { 
                    $this = this.$this,
                    outputFile = this.outputFile
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
                return new AndroidToolChain.<FlagsToMakeWarningsErrorsFor>c__Iterator5 { sourceFile = this.sourceFile };
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
                return new AndroidToolChain.<ToolChainDefines>c__Iterator0();
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
                        this.$current = new NPath("");
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
            IEnumerator<NPath> IEnumerable<NPath>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AndroidToolChain.<ToolChainIncludePaths>c__Iterator1();
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

