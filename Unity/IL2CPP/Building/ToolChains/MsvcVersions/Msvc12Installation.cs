namespace Unity.IL2CPP.Building.ToolChains.MsvcVersions
{
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Unity.IL2CPP.Building;

    internal class Msvc12Installation : MsvcInstallation
    {
        public Msvc12Installation(NPath visualStudioDir) : base(new Version(12, 0), visualStudioDir, true)
        {
            base.SDKDirectory = WindowsSDKs.GetWindows81SDKDirectory();
        }

        [DebuggerHidden]
        public override IEnumerable<NPath> GetIncludeDirectories(Architecture architecture) => 
            new <GetIncludeDirectories>c__Iterator0 { 
                $this = this,
                $PC = -2
            };

        [DebuggerHidden]
        public override IEnumerable<NPath> GetLibDirectories(Architecture architecture, string sdkSubset = null) => 
            new <GetLibDirectories>c__Iterator1 { 
                sdkSubset = sdkSubset,
                architecture = architecture,
                $this = this,
                $PC = -2
            };

        [DebuggerHidden]
        public override IEnumerable<NPath> GetPlatformMetadataReferences() => 
            new <GetPlatformMetadataReferences>c__Iterator2 { 
                $this = this,
                $PC = -2
            };

        public override NPath GetUnionMetadataDirectory()
        {
            string[] append = new string[] { "References", "CommonConfiguration", "Neutral" };
            return base.SDKDirectory.Combine(append);
        }

        [DebuggerHidden]
        public override IEnumerable<NPath> GetWindowsMetadataReferences() => 
            new <GetWindowsMetadataReferences>c__Iterator3 { 
                $this = this,
                $PC = -2
            };

        public override bool HasMetadataDirectories() => 
            true;

        public override IEnumerable<Type> SupportedArchitectures =>
            new Type[] { typeof(x86Architecture), typeof(ARMv7Architecture), typeof(x64Architecture) };

        [CompilerGenerated]
        private sealed class <GetIncludeDirectories>c__Iterator0 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;
            internal Msvc12Installation $this;
            internal NPath <includeDirectory>__1;

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
                        string[] append = new string[] { "VC", "include" };
                        this.$current = this.$this.VisualStudioDirectory.Combine(append);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_0134;
                    }
                    case 1:
                    {
                        string[] textArray2 = new string[] { "Include" };
                        this.<includeDirectory>__1 = this.$this.SDKDirectory.Combine(textArray2);
                        string[] textArray3 = new string[] { "shared" };
                        this.$current = this.<includeDirectory>__1.Combine(textArray3);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0134;
                    }
                    case 2:
                    {
                        string[] textArray4 = new string[] { "um" };
                        this.$current = this.<includeDirectory>__1.Combine(textArray4);
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_0134;
                    }
                    case 3:
                    {
                        string[] textArray5 = new string[] { "winrt" };
                        this.$current = this.<includeDirectory>__1.Combine(textArray5);
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_0134;
                    }
                    case 4:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0134:
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
                return new Msvc12Installation.<GetIncludeDirectories>c__Iterator0 { $this = this.$this };
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
        private sealed class <GetLibDirectories>c__Iterator1 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;
            internal Msvc12Installation $this;
            internal NPath <sdkLibDirectory>__1;
            internal NPath <vcLibPath>__1;
            internal Architecture architecture;
            internal string sdkSubset;

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
                        string[] append = new string[] { "VC", "lib" };
                        this.<vcLibPath>__1 = this.$this.VisualStudioDirectory.Combine(append);
                        string[] textArray2 = new string[] { "lib", "winv6.3", "um" };
                        this.<sdkLibDirectory>__1 = this.$this.SDKDirectory.Combine(textArray2);
                        if (this.sdkSubset != null)
                        {
                            string[] textArray3 = new string[] { this.sdkSubset };
                            this.<vcLibPath>__1 = this.<vcLibPath>__1.Combine(textArray3);
                        }
                        if (this.architecture is x86Architecture)
                        {
                            this.$current = this.<vcLibPath>__1;
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                        }
                        else if (this.architecture is x64Architecture)
                        {
                            string[] textArray5 = new string[] { "amd64" };
                            this.$current = this.<vcLibPath>__1.Combine(textArray5);
                            if (!this.$disposing)
                            {
                                this.$PC = 3;
                            }
                        }
                        else
                        {
                            if (!(this.architecture is ARMv7Architecture))
                            {
                                throw new NotSupportedException($"Architecture {this.architecture} is not supported by MSVC 12 compiler!");
                            }
                            string[] textArray7 = new string[] { "arm" };
                            this.$current = this.<vcLibPath>__1.Combine(textArray7);
                            if (!this.$disposing)
                            {
                                this.$PC = 5;
                            }
                        }
                        goto Label_0245;
                    }
                    case 1:
                    {
                        string[] textArray4 = new string[] { "x86" };
                        this.$current = this.<sdkLibDirectory>__1.Combine(textArray4);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0245;
                    }
                    case 2:
                    case 4:
                    case 6:
                        this.$PC = -1;
                        break;

                    case 3:
                    {
                        string[] textArray6 = new string[] { "x64" };
                        this.$current = this.<sdkLibDirectory>__1.Combine(textArray6);
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_0245;
                    }
                    case 5:
                    {
                        string[] textArray8 = new string[] { "arm" };
                        this.$current = this.<sdkLibDirectory>__1.Combine(textArray8);
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_0245;
                    }
                }
                return false;
            Label_0245:
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
                return new Msvc12Installation.<GetLibDirectories>c__Iterator1 { 
                    $this = this.$this,
                    sdkSubset = this.sdkSubset,
                    architecture = this.architecture
                };
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
        private sealed class <GetPlatformMetadataReferences>c__Iterator2 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;
            internal Msvc12Installation $this;

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
                        string[] append = new string[] { "..", "Microsoft SDKs", "Windows", "v8.1", "ExtensionSDKs", "Microsoft.VCLibs", "12.0", "References", "CommonConfiguration", "neutral", "platform.winmd" };
                        this.$current = this.$this.VisualStudioDirectory.Combine(append);
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
                return new Msvc12Installation.<GetPlatformMetadataReferences>c__Iterator2 { $this = this.$this };
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
        private sealed class <GetWindowsMetadataReferences>c__Iterator3 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;
            internal Msvc12Installation $this;

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
                        string[] append = new string[] { "Windows.winmd" };
                        this.$current = this.$this.GetUnionMetadataDirectory().Combine(append);
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
                return new Msvc12Installation.<GetWindowsMetadataReferences>c__Iterator3 { $this = this.$this };
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

