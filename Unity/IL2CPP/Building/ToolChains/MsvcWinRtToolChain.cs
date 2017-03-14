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
    using Unity.IL2CPP.Common;

    public class MsvcWinRtToolChain : MsvcToolChain
    {
        private Unity.IL2CPP.Building.ToolChains.MsvcVersions.MsvcInstallation _msvcInstallation;
        [CompilerGenerated]
        private static Func<string, IEnumerable<string>> <>f__mg$cache0;

        public MsvcWinRtToolChain(Unity.IL2CPP.Common.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors) : base(architecture, buildConfiguration)
        {
            if (treatWarningsAsErrors)
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new Func<string, IEnumerable<string>>(MsvcToolChain.FlagsToMakeWarningsErrorsFor);
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

        public override void OnAfterLink(NPath outputFile, CppToolChainContext toolChainContext, bool forceRebuild, bool verbose)
        {
            string[] extensions = new string[] { ".exe" };
            if (outputFile.HasExtension(extensions))
            {
                WinRTManifest.Write(outputFile.Parent, outputFile.FileName, base.Architecture);
            }
        }

        [DebuggerHidden]
        public override IEnumerable<string> ToolChainDefines() => 
            new <ToolChainDefines>c__Iterator0 { 
                $this = this,
                $PC = -2
            };

        [DebuggerHidden]
        public override IEnumerable<NPath> ToolChainIncludePaths() => 
            new <ToolChainIncludePaths>c__Iterator3 { 
                $this = this,
                $PC = -2
            };

        public override IEnumerable<NPath> ToolChainLibraryPaths() => 
            this.MsvcInstallation.GetLibDirectories(base.Architecture, "store");

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
                    this._msvcInstallation = Unity.IL2CPP.Building.ToolChains.MsvcVersions.MsvcInstallation.GetLatestInstallationAtLeast(new Version(14, 0));
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
            internal MsvcWinRtToolChain $this;
            internal string <arg>__1;
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
                        this.$current = "/SUBSYSTEM:WINDOWS";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_018C;

                    case 3:
                        this.$current = "/NODEFAULTLIB:ole32.lib";
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_018C;

                    case 4:
                        this.$current = "/NODEFAULTLIB:kernel32.lib";
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_018C;

                    case 5:
                        if (this.$this.BuildConfiguration != BuildConfiguration.Debug)
                        {
                            goto Label_0183;
                        }
                        this.$current = "/NODEFAULTLIB:msvcrt.lib";
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_018C;

                    case 6:
                        goto Label_0183;

                    default:
                        goto Label_018A;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<arg>__1 = this.$locvar0.Current;
                        this.$current = this.<arg>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_018C;
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
                this.$current = "/APPCONTAINER";
                if (!this.$disposing)
                {
                    this.$PC = 2;
                }
                goto Label_018C;
            Label_0183:
                this.$PC = -1;
            Label_018A:
                return false;
            Label_018C:
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
                return new MsvcWinRtToolChain.<GetDefaultLinkerArgs>c__Iterator2 { 
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
            internal MsvcWinRtToolChain $this;
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
                        this.$current = "WINAPI_FAMILY=WINAPI_FAMILY_APP";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_0101;

                    case 3:
                        this.$PC = -1;
                        goto Label_00FF;

                    default:
                        goto Label_00FF;
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
                        goto Label_0101;
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
                this.$current = "__WRL_NO_DEFAULT_LIB__";
                if (!this.$disposing)
                {
                    this.$PC = 2;
                }
                goto Label_0101;
            Label_00FF:
                return false;
            Label_0101:
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
                return new MsvcWinRtToolChain.<ToolChainDefines>c__Iterator0 { $this = this.$this };
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
        private sealed class <ToolChainIncludePaths>c__Iterator3 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal IEnumerator<NPath> $locvar0;
            internal int $PC;
            internal MsvcWinRtToolChain $this;
            internal NPath <includePath>__1;
            internal NPath <unityIl2CppWinRT>__0;

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
                        this.$locvar0 = this.$this.<ToolChainIncludePaths>__BaseCallProxy3().GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    case 2:
                        goto Label_0109;

                    default:
                        goto Label_0110;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<includePath>__1 = this.$locvar0.Current;
                        this.$current = this.<includePath>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_0112;
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
                string[] append = new string[] { "Unity.IL2CPP.WinRT" };
                this.<unityIl2CppWinRT>__0 = CommonPaths.Il2CppRoot.Combine(append);
                if (this.<unityIl2CppWinRT>__0.DirectoryExists(""))
                {
                    this.$current = this.<unityIl2CppWinRT>__0;
                    if (!this.$disposing)
                    {
                        this.$PC = 2;
                    }
                    goto Label_0112;
                }
            Label_0109:
                this.$PC = -1;
            Label_0110:
                return false;
            Label_0112:
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
                return new MsvcWinRtToolChain.<ToolChainIncludePaths>c__Iterator3 { $this = this.$this };
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
        private sealed class <ToolChainStaticLibraries>c__Iterator1 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal int $PC;
            internal MsvcWinRtToolChain $this;
            internal string <staticLib>__1;

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
                        this.$current = "WindowsApp.lib";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_0101;

                    case 3:
                        this.$PC = -1;
                        goto Label_00FF;

                    default:
                        goto Label_00FF;
                }
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<staticLib>__1 = this.$locvar0.Current;
                        this.$current = this.<staticLib>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_0101;
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
                this.$current = "Shcore.lib";
                if (!this.$disposing)
                {
                    this.$PC = 2;
                }
                goto Label_0101;
            Label_00FF:
                return false;
            Label_0101:
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
                return new MsvcWinRtToolChain.<ToolChainStaticLibraries>c__Iterator1 { $this = this.$this };
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

