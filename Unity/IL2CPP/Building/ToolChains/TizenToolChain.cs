namespace Unity.IL2CPP.Building.ToolChains
{
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Common;

    public class TizenToolChain : GccToolChain
    {
        public TizenToolChain(Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration) : base(architecture, buildConfiguration)
        {
        }

        public override bool CanBuildInCurrentEnvironment() => 
            (PlatformUtils.IsLinux() || PlatformUtils.IsOSX());

        public override NPath CompilerExecutableFor(NPath sourceFile)
        {
            string[] extensions = new string[] { ".c" };
            if (sourceFile.HasExtension(extensions))
            {
                return new NPath(TizenSDKUtilities.GetCompilerPrefix() + "gcc");
            }
            return new NPath(TizenSDKUtilities.GetCompilerPrefix() + "g++");
        }

        [DebuggerHidden]
        public override IEnumerable<string> CompilerFlagsFor(CppCompilationInstruction cppCompilationInstruction) => 
            new <CompilerFlagsFor>c__Iterator0 { 
                cppCompilationInstruction = cppCompilationInstruction,
                $this = this,
                $PC = -2
            };

        protected override string GetInterestingOutputFromCompilationShellResult(Shell.ExecuteResult shellResult) => 
            shellResult.StdErr;

        protected override string GetInterestingOutputFromLinkerShellResult(Shell.ExecuteResult shellResult) => 
            shellResult.StdErr;

        protected override List<string> GetLinkerArgs(IEnumerable<NPath> objectFiles, NPath outputFile, IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, IEnumerable<string> specifiedLinkerFlags)
        {
            List<string> list = base.GetLinkerArgs(objectFiles, outputFile, staticLibraries, dynamicLibraries, specifiedLinkerFlags);
            list.Add("-L" + TizenSDKUtilities.GetSysroot() + "/usr/lib");
            list.Add("--sysroot=" + TizenSDKUtilities.GetSysroot());
            return list;
        }

        public override NPath LinkerExecutableFor() => 
            TizenSDKUtilities.GetLinkerPath();

        [CompilerGenerated]
        private sealed class <CompilerFlagsFor>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
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
                    case 4:
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
                        this.$current = "-DTIZEN";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_0160;

                    case 1:
                        this.$current = "-fno-strict-overflow";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0160;

                    case 2:
                        this.$current = "--sysroot=" + TizenSDKUtilities.GetSysroot();
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_0160;

                    case 3:
                        this.$locvar0 = this.$this.<CompilerFlagsFor>__BaseCallProxy0(this.cppCompilationInstruction).GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 4:
                        break;

                    default:
                        goto Label_015E;
                }
                try
                {
                    switch (num)
                    {
                        case 4:
                            goto Label_0126;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<flag>__1 = this.$locvar0.Current;
                        if (!this.<flag>__1.Equals("-m32") && !this.<flag>__1.Equals("-m64"))
                        {
                            this.$current = this.<flag>__1;
                            if (!this.$disposing)
                            {
                                this.$PC = 4;
                            }
                            flag = true;
                            goto Label_0160;
                        }
                    Label_0126:;
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
            Label_015E:
                return false;
            Label_0160:
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
                return new TizenToolChain.<CompilerFlagsFor>c__Iterator0 { 
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
    }
}

