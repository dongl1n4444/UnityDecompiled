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
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.Portability;
    using Unity.Options;

    public class EmscriptenToolChain : CppToolChain
    {
        private readonly bool _setEnvironmentVariables;
        [CompilerGenerated]
        private static Func<string, NPath> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, NPath> <>f__am$cache1;
        public static bool DeepDebugging = false;

        public EmscriptenToolChain(Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration, [Optional, DefaultParameterValue(false)] bool setEnvironmentVariables) : base(architecture, buildConfiguration)
        {
            this._setEnvironmentVariables = setEnvironmentVariables;
        }

        public override bool CanBuildInCurrentEnvironment()
        {
            return true;
        }

        public override NPath CompilerExecutableFor(NPath sourceFile)
        {
            return EmscriptenPaths.Python;
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
            return new <DefaultCompilerFlags>c__Iterator4 { 
                cppCompilationInstruction = cppCompilationInstruction,
                $this = this,
                $PC = -2
            };
        }

        [DebuggerHidden]
        private IEnumerable<string> DefaultLinkerFlags(IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, NPath outputFile)
        {
            return new <DefaultLinkerFlags>c__Iterator3 { 
                $this = this,
                $PC = -2
            };
        }

        public override Dictionary<string, string> EnvVars()
        {
            if (this._setEnvironmentVariables)
            {
                return EmscriptenPaths.EmscriptenEnvironmentVariables;
            }
            return null;
        }

        public override string ExecutableExtension()
        {
            return ".html";
        }

        protected override string GetInterestingOutputFromCompilationShellResult(Shell.ExecuteResult shellResult)
        {
            return shellResult.StdErr;
        }

        protected override string GetInterestingOutputFromLinkerShellResult(Shell.ExecuteResult shellResult)
        {
            return shellResult.StdErr;
        }

        private static string GetShortPathName(string path)
        {
            if (!PlatformUtils.IsWindows() || (Encoding.UTF8.GetByteCount(path) == path.Length))
            {
                return path;
            }
            int capacity = WindowsGetShortPathName(path, null, 0);
            if (capacity == 0)
            {
                return path;
            }
            StringBuilder lpszShortPath = new StringBuilder(capacity);
            capacity = WindowsGetShortPathName(path, lpszShortPath, lpszShortPath.Capacity);
            if (capacity == 0)
            {
                return path;
            }
            return lpszShortPath.ToString(0, capacity);
        }

        public override CppProgramBuilder.LinkerInvocation MakeLinkerInvocation(IEnumerable<NPath> objectFiles, NPath outputFile, IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, IEnumerable<string> specifiedLinkerFlags, CppToolChainContext toolChainContext)
        {
            NPath path2;
            List<NPath> list = new List<NPath>(objectFiles);
            string shortPathName = GetShortPathName(Path.GetFullPath(NPath.CreateTempDirectory("").CreateFile("response.rsp").ToString()));
            if (outputFile != null)
            {
                path2 = outputFile;
            }
            else
            {
                string[] append = new string[] { "test.html" };
                path2 = NPath.CreateTempDirectory("em_out").Combine(append);
            }
            List<string> inputs = new List<string> {
                EmscriptenPaths.Emcc.InQuotes(),
                "-o",
                path2.InQuotes()
            };
            inputs.AddRange(base.ChooseLinkerFlags(staticLibraries, dynamicLibraries, outputFile, specifiedLinkerFlags, new Func<IEnumerable<NPath>, IEnumerable<NPath>, NPath, IEnumerable<string>>(this, (IntPtr) this.DefaultLinkerFlags)));
            string path = !Enumerable.Any<NPath>(objectFiles) ? null : Enumerable.First<NPath>(objectFiles).Parent.ToString();
            using (TextWriter writer = new Unity.IL2CPP.Portability.StreamWriter(shortPathName))
            {
                foreach (NPath path3 in objectFiles)
                {
                    if (path != null)
                    {
                        writer.Write("\"{0}\"\n", path3.RelativeTo(new NPath(path)));
                    }
                    else
                    {
                        writer.Write("\"{0}\"\n", path3.FileName);
                    }
                }
            }
            Console.WriteLine("Response file: {0}", shortPathName);
            list.AddRange(staticLibraries);
            if (EmscriptenBuildingOptions.JsPre != null)
            {
                foreach (string str3 in EmscriptenBuildingOptions.JsPre)
                {
                    inputs.Add("--pre-js \"" + str3 + "\"");
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<string, NPath>(null, (IntPtr) <MakeLinkerInvocation>m__0);
                }
                list.AddRange(Enumerable.Select<string, NPath>(EmscriptenBuildingOptions.JsPre, <>f__am$cache0));
            }
            if (EmscriptenBuildingOptions.JsLibraries != null)
            {
                foreach (string str4 in EmscriptenBuildingOptions.JsLibraries)
                {
                    inputs.Add("--js-library \"" + str4 + "\"");
                }
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<string, NPath>(null, (IntPtr) <MakeLinkerInvocation>m__1);
                }
                list.AddRange(Enumerable.Select<string, NPath>(EmscriptenBuildingOptions.JsLibraries, <>f__am$cache1));
            }
            inputs.AddRange(Extensions.InQuotes(staticLibraries, SlashMode.Native));
            inputs.AddRange(ExtensionMethods.InQuotes(this.ToolChainStaticLibraries()));
            string str5 = EmscriptenPaths.Python.ToString();
            if (PlatformUtils.IsWindows())
            {
                str5 = EmscriptenPaths.Python.InQuotes();
            }
            CppProgramBuilder.LinkerInvocation invocation = new CppProgramBuilder.LinkerInvocation();
            Shell.ExecuteArgs args = new Shell.ExecuteArgs {
                Executable = str5,
                Arguments = ExtensionMethods.SeparateWithSpaces(EnumerableExtensions.Append<string>(inputs, "@" + ExtensionMethods.InQuotes(shortPathName))),
                EnvVars = this.EnvVars(),
                WorkingDirectory = path
            };
            invocation.ExecuteArgs = args;
            invocation.ArgumentsInfluencingOutcome = inputs;
            invocation.FilesInfluencingOutcome = list;
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

        public override IEnumerable<Type> SupportedArchitectures()
        {
            return new Type[] { typeof(EmscriptenJavaScriptArchitecture) };
        }

        [DebuggerHidden]
        public override IEnumerable<string> ToolChainDefines()
        {
            return new <ToolChainDefines>c__Iterator1 { $PC = -2 };
        }

        [DebuggerHidden]
        public override IEnumerable<NPath> ToolChainIncludePaths()
        {
            return new <ToolChainIncludePaths>c__Iterator0 { $PC = -2 };
        }

        [DllImport("kernel32.dll", EntryPoint="GetShortPathName", CharSet=CharSet.Unicode, SetLastError=true)]
        private static extern int WindowsGetShortPathName([MarshalAs(UnmanagedType.LPWStr)] string lpszLongPath, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszShortPath, int cchBuffer);

        public override string DynamicLibraryExtension
        {
            get
            {
                throw new InvalidOperationException("Emscripten does not support dynamic libraries");
            }
        }

        [CompilerGenerated]
        private sealed class <CompilerFlagsFor>c__Iterator2 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal IEnumerator<string> $locvar1;
            internal IEnumerator<NPath> $locvar2;
            internal int $PC;
            internal EmscriptenToolChain $this;
            internal string <compilerFlag>__0;
            internal string <define>__1;
            internal NPath <includePath>__2;
            internal CppCompilationInstruction cppCompilationInstruction;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 3:
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

                    case 4:
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

                    case 5:
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
                        string[] extensions = new string[] { ".c" };
                        if (!this.cppCompilationInstruction.SourceFile.HasExtension(extensions))
                        {
                            this.$current = EmscriptenPaths.Emcpp.InQuotes();
                            if (!this.$disposing)
                            {
                                this.$PC = 2;
                            }
                        }
                        else
                        {
                            this.$current = EmscriptenPaths.Emcc.InQuotes();
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                        }
                        goto Label_02D7;
                    }
                    case 1:
                    case 2:
                        this.$locvar0 = this.$this.ChooseCompilerFlags(this.cppCompilationInstruction, new Func<CppCompilationInstruction, IEnumerable<string>>(this.$this, (IntPtr) this.DefaultCompilerFlags)).GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 3:
                        break;

                    case 4:
                        goto Label_016B;

                    case 5:
                        goto Label_0203;

                    case 6:
                        this.$PC = -1;
                        goto Label_02D5;

                    default:
                        goto Label_02D5;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<compilerFlag>__0 = this.$locvar0.Current;
                        this.$current = this.<compilerFlag>__0;
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        flag = true;
                        goto Label_02D7;
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
                this.$locvar1 = this.cppCompilationInstruction.Defines.GetEnumerator();
                num = 0xfffffffd;
            Label_016B:
                try
                {
                    while (this.$locvar1.MoveNext())
                    {
                        this.<define>__1 = this.$locvar1.Current;
                        this.$current = "-D" + this.<define>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        flag = true;
                        goto Label_02D7;
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
                this.$locvar2 = this.cppCompilationInstruction.IncludePaths.GetEnumerator();
                num = 0xfffffffd;
            Label_0203:
                try
                {
                    while (this.$locvar2.MoveNext())
                    {
                        this.<includePath>__2 = this.$locvar2.Current;
                        this.$current = "-I\"" + EmscriptenToolChain.GetShortPathName(Path.GetFullPath(this.<includePath>__2.ToString())) + "\"";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        flag = true;
                        goto Label_02D7;
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
                this.$current = ExtensionMethods.InQuotes(EmscriptenToolChain.GetShortPathName(Path.GetFullPath(this.cppCompilationInstruction.SourceFile.ToString())));
                if (!this.$disposing)
                {
                    this.$PC = 6;
                }
                goto Label_02D7;
            Label_02D5:
                return false;
            Label_02D7:
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
                return new EmscriptenToolChain.<CompilerFlagsFor>c__Iterator2 { 
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
        private sealed class <DefaultCompilerFlags>c__Iterator4 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal EmscriptenToolChain $this;
            internal CppCompilationInstruction cppCompilationInstruction;

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
                        this.$current = "-Wno-unused-value";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_01F3;

                    case 1:
                        this.$current = "-Wno-invalid-offsetof";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_01F3;

                    case 2:
                        this.$current = "-nostdinc";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_01F3;

                    case 3:
                        this.$current = "-fno-strict-overflow";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_01F3;

                    case 4:
                    {
                        string[] extensions = new string[] { ".cpp" };
                        if (!this.cppCompilationInstruction.SourceFile.HasExtension(extensions))
                        {
                            break;
                        }
                        this.$current = "-std=c++11";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_01F3;
                    }
                    case 5:
                        break;

                    case 6:
                        this.$current = "-g";
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_01F3;

                    case 7:
                        this.$current = "-s ALIASING_FUNCTION_POINTERS=0";
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_01F3;

                    case 8:
                        this.$current = "-s SAFE_HEAP=3";
                        if (!this.$disposing)
                        {
                            this.$PC = 9;
                        }
                        goto Label_01F3;

                    case 9:
                    case 11:
                        goto Label_01EA;

                    case 10:
                        this.$current = "-g0";
                        if (!this.$disposing)
                        {
                            this.$PC = 11;
                        }
                        goto Label_01F3;

                    default:
                        goto Label_01F1;
                }
                if (this.$this.BuildConfiguration == BuildConfiguration.Debug)
                {
                    if (!EmscriptenToolChain.DeepDebugging)
                    {
                        this.$current = "-O1";
                        if (!this.$disposing)
                        {
                            this.$PC = 10;
                        }
                    }
                    else
                    {
                        this.$current = "-O0";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                    }
                    goto Label_01F3;
                }
            Label_01EA:
                this.$PC = -1;
            Label_01F1:
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
                return new EmscriptenToolChain.<DefaultCompilerFlags>c__Iterator4 { 
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
        private sealed class <DefaultLinkerFlags>c__Iterator3 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal EmscriptenToolChain $this;

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
                        if (this.$this.BuildConfiguration != BuildConfiguration.Debug)
                        {
                            break;
                        }
                        if (!EmscriptenToolChain.DeepDebugging)
                        {
                            this.$current = "-O1";
                            if (!this.$disposing)
                            {
                                this.$PC = 5;
                            }
                        }
                        else
                        {
                            this.$current = "-O0";
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                        }
                        goto Label_011E;

                    case 1:
                        this.$current = "-g";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_011E;

                    case 2:
                        this.$current = "-s ALIASING_FUNCTION_POINTERS=0";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_011E;

                    case 3:
                        this.$current = "-s SAFE_HEAP=3";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_011E;

                    case 4:
                    case 6:
                        break;

                    case 5:
                        this.$current = "-g0";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_011E;

                    default:
                        goto Label_011C;
                }
                this.$PC = -1;
            Label_011C:
                return false;
            Label_011E:
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
                return new EmscriptenToolChain.<DefaultLinkerFlags>c__Iterator3 { $this = this.$this };
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
        private sealed class <ToolChainDefines>c__Iterator1 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
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
                return new EmscriptenToolChain.<ToolChainDefines>c__Iterator1();
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
        private sealed class <ToolChainIncludePaths>c__Iterator0 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
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
                return new EmscriptenToolChain.<ToolChainIncludePaths>c__Iterator0();
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

        [ProgramOptions]
        public class EmscriptenBuildingOptions
        {
            [HideFromHelp]
            public static string[] JsLibraries;
            [HideFromHelp]
            public static string[] JsPre;

            public static void SetToDefaults()
            {
                JsPre = null;
                JsLibraries = null;
            }
        }
    }
}

