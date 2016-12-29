namespace Unity.IL2CPP.Building.ToolChains
{
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.ToolChains.MsvcVersions;

    public class MsvcDesktopToolChain : MsvcToolChain
    {
        private Unity.IL2CPP.Building.ToolChains.MsvcVersions.MsvcInstallation _msvcInstallation;
        [CompilerGenerated]
        private static Func<string, IEnumerable<string>> <>f__mg$cache0;

        public MsvcDesktopToolChain(Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors) : base(architecture, buildConfiguration)
        {
            if (treatWarningsAsErrors)
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new Func<string, IEnumerable<string>>(null, (IntPtr) MsvcToolChain.FlagsToMakeWarningsErrorsFor);
                }
                base.AdditionalCompilerOptionsForSourceFile = <>f__mg$cache0;
            }
        }

        [DebuggerHidden]
        protected override IEnumerable<string> GetDefaultLinkerArgs(IEnumerable<NPath> staticLibraries, IEnumerable<NPath> dynamicLibraries, NPath outputFile) => 
            new <GetDefaultLinkerArgs>c__Iterator2 { 
                staticLibraries = staticLibraries,
                dynamicLibraries = dynamicLibraries,
                outputFile = outputFile,
                $this = this,
                $PC = -2
            };

        [DebuggerHidden]
        public override IEnumerable<string> ToolChainDefines() => 
            new <ToolChainDefines>c__Iterator0 { 
                $this = this,
                $PC = -2
            };

        [DebuggerHidden]
        public override IEnumerable<string> ToolChainStaticLibraries() => 
            new <ToolChainStaticLibraries>c__Iterator1 { 
                $this = this,
                $PC = -2
            };

        public override Unity.IL2CPP.Building.ToolChains.MsvcVersions.MsvcInstallation MsvcInstallation
        {
            get
            {
                if (this._msvcInstallation == null)
                {
                    this._msvcInstallation = Unity.IL2CPP.Building.ToolChains.MsvcVersions.MsvcInstallation.GetLatestInstalled();
                }
                return this._msvcInstallation;
            }
        }

        [CompilerGenerated]
        private sealed class <GetDefaultLinkerArgs>c__Iterator2 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal int $PC;
            internal MsvcDesktopToolChain $this;
            internal string <arg>__0;
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
                        this.$locvar0 = this.$this.<GetDefaultLinkerArgs>__BaseCallProxy2(this.staticLibraries, this.dynamicLibraries, this.outputFile).GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    case 2:
                        this.$current = "/ENTRY:wWinMainCRTStartup";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_015D;

                    case 3:
                    case 4:
                        this.$PC = -1;
                        goto Label_015B;

                    default:
                        goto Label_015B;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<arg>__0 = this.$locvar0.Current;
                        this.$current = this.<arg>__0;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_015D;
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
                string[] extensions = new string[] { ".exe" };
                if (this.outputFile.HasExtension(extensions))
                {
                    this.$current = "/SUBSYSTEM:CONSOLE";
                    if (!this.$disposing)
                    {
                        this.$PC = 2;
                    }
                }
                else
                {
                    this.$current = "/SUBSYSTEM:WINDOWS";
                    if (!this.$disposing)
                    {
                        this.$PC = 4;
                    }
                }
                goto Label_015D;
            Label_015B:
                return false;
            Label_015D:
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
                return new MsvcDesktopToolChain.<GetDefaultLinkerArgs>c__Iterator2 { 
                    $this = this.$this,
                    staticLibraries = this.staticLibraries,
                    dynamicLibraries = this.dynamicLibraries,
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
        private sealed class <ToolChainDefines>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal int $PC;
            internal MsvcDesktopToolChain $this;
            internal string <f>__0;

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
                        goto Label_00EA;

                    default:
                        goto Label_00F1;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<f>__0 = this.$locvar0.Current;
                        this.$current = this.<f>__0;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_00F3;
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
                if (!(this.$this._msvcInstallation is Msvc10Installation))
                {
                    this.$current = "WINAPI_FAMILY=WINAPI_FAMILY_DESKTOP_APP";
                    if (!this.$disposing)
                    {
                        this.$PC = 2;
                    }
                    goto Label_00F3;
                }
            Label_00EA:
                this.$PC = -1;
            Label_00F1:
                return false;
            Label_00F3:
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
                return new MsvcDesktopToolChain.<ToolChainDefines>c__Iterator0 { $this = this.$this };
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
        private sealed class <ToolChainStaticLibraries>c__Iterator1 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal int $PC;
            internal MsvcDesktopToolChain $this;
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
                        this.$locvar0 = this.$this.<ToolChainStaticLibraries>__BaseCallProxy1().GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    case 2:
                        this.$current = "advapi32.lib";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_016A;

                    case 3:
                        this.$current = "ole32.lib";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_016A;

                    case 4:
                        this.$current = "oleaut32.lib";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_016A;

                    case 5:
                        this.$current = "Shell32.lib";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_016A;

                    case 6:
                        this.$PC = -1;
                        goto Label_0168;

                    default:
                        goto Label_0168;
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
                        goto Label_016A;
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
                this.$current = "user32.lib";
                if (!this.$disposing)
                {
                    this.$PC = 2;
                }
                goto Label_016A;
            Label_0168:
                return false;
            Label_016A:
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
                return new MsvcDesktopToolChain.<ToolChainStaticLibraries>c__Iterator1 { $this = this.$this };
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

