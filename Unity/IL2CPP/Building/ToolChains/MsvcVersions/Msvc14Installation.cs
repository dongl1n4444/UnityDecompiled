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

    internal class Msvc14Installation : MsvcInstallation
    {
        private readonly NPath _netfxsdkDir;
        private readonly List<NPath> _sdkBinDirectories;
        private readonly NPath _sdkUnionMetadataDirectory;
        private readonly string _sdkVersion;

        public Msvc14Installation(NPath visualStudioDir) : base(new Version(14, 0), visualStudioDir)
        {
            this._sdkBinDirectories = new List<NPath>();
            base.SDKDirectory = WindowsSDKs.GetWindows10SDKDirectory(out this._sdkVersion);
            this._netfxsdkDir = WindowsSDKs.GetDotNetFrameworkSDKDirectory();
            if (base.SDKDirectory != null)
            {
                string[] append = new string[] { "bin" };
                NPath item = base.SDKDirectory.Combine(append);
                string[] textArray2 = new string[] { this._sdkVersion };
                NPath path2 = item.Combine(textArray2);
                if (path2.DirectoryExists(""))
                {
                    this._sdkBinDirectories.Add(path2);
                }
                this._sdkBinDirectories.Add(item);
                string[] textArray3 = new string[] { "UnionMetadata" };
                NPath path3 = base.SDKDirectory.Combine(textArray3);
                string[] textArray4 = new string[] { this._sdkVersion };
                NPath path4 = path3.Combine(textArray4);
                this._sdkUnionMetadataDirectory = !path4.DirectoryExists("") ? path3 : path4;
            }
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

        protected override IEnumerable<NPath> GetSDKBinDirectories() => 
            this._sdkBinDirectories;

        public override NPath GetUnionMetadataDirectory() => 
            this._sdkUnionMetadataDirectory;

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
            internal Msvc14Installation $this;
            internal NPath <includeDirectory>__0;

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
                        goto Label_0184;
                    }
                    case 1:
                    {
                        string[] textArray2 = new string[] { "Include" };
                        string[] textArray3 = new string[] { this.$this._sdkVersion };
                        this.<includeDirectory>__0 = this.$this.SDKDirectory.Combine(textArray2).Combine(textArray3);
                        string[] textArray4 = new string[] { "shared" };
                        this.$current = this.<includeDirectory>__0.Combine(textArray4);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0184;
                    }
                    case 2:
                    {
                        string[] textArray5 = new string[] { "um" };
                        this.$current = this.<includeDirectory>__0.Combine(textArray5);
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_0184;
                    }
                    case 3:
                    {
                        string[] textArray6 = new string[] { "winrt" };
                        this.$current = this.<includeDirectory>__0.Combine(textArray6);
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_0184;
                    }
                    case 4:
                    {
                        string[] textArray7 = new string[] { "ucrt" };
                        this.$current = this.<includeDirectory>__0.Combine(textArray7);
                        if (!this.$disposing)
                        {
                            this.$PC = 5;
                        }
                        goto Label_0184;
                    }
                    case 5:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0184:
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
                return new Msvc14Installation.<GetIncludeDirectories>c__Iterator0 { $this = this.$this };
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
            internal Msvc14Installation $this;
            internal NPath <libDirectory>__0;
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
                        string[] append = new string[] { "Lib" };
                        string[] textArray2 = new string[] { this.$this._sdkVersion };
                        this.<libDirectory>__0 = this.$this.SDKDirectory.Combine(append).Combine(textArray2);
                        string[] textArray3 = new string[] { "VC", "lib" };
                        this.<vcLibPath>__1 = this.$this.VisualStudioDirectory.Combine(textArray3);
                        if (this.sdkSubset != null)
                        {
                            string[] textArray4 = new string[] { this.sdkSubset };
                            this.<vcLibPath>__1 = this.<vcLibPath>__1.Combine(textArray4);
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
                            string[] textArray8 = new string[] { "amd64" };
                            this.$current = this.<vcLibPath>__1.Combine(textArray8);
                            if (!this.$disposing)
                            {
                                this.$PC = 5;
                            }
                        }
                        else
                        {
                            if (!(this.architecture is ARMv7Architecture))
                            {
                                throw new NotSupportedException($"Architecture {this.architecture} is not supported by MsvcToolChain!");
                            }
                            string[] textArray12 = new string[] { "arm" };
                            this.$current = this.<vcLibPath>__1.Combine(textArray12);
                            if (!this.$disposing)
                            {
                                this.$PC = 9;
                            }
                        }
                        goto Label_044D;
                    }
                    case 1:
                    {
                        string[] textArray5 = new string[] { "um", "x86" };
                        this.$current = this.<libDirectory>__0.Combine(textArray5);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_044D;
                    }
                    case 2:
                    {
                        string[] textArray6 = new string[] { "ucrt", "x86" };
                        this.$current = this.<libDirectory>__0.Combine(textArray6);
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_044D;
                    }
                    case 3:
                    {
                        if (this.$this._netfxsdkDir == null)
                        {
                            break;
                        }
                        string[] textArray7 = new string[] { "lib", "um", "x86" };
                        this.$current = this.$this._netfxsdkDir.Combine(textArray7);
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_044D;
                    }
                    case 4:
                    case 8:
                    case 12:
                        break;

                    case 5:
                    {
                        string[] textArray9 = new string[] { "um", "x64" };
                        this.$current = this.<libDirectory>__0.Combine(textArray9);
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        goto Label_044D;
                    }
                    case 6:
                    {
                        string[] textArray10 = new string[] { "ucrt", "x64" };
                        this.$current = this.<libDirectory>__0.Combine(textArray10);
                        if (!this.$disposing)
                        {
                            this.$PC = 7;
                        }
                        goto Label_044D;
                    }
                    case 7:
                    {
                        if (this.$this._netfxsdkDir == null)
                        {
                            break;
                        }
                        string[] textArray11 = new string[] { "lib", "um", "x64" };
                        this.$current = this.$this._netfxsdkDir.Combine(textArray11);
                        if (!this.$disposing)
                        {
                            this.$PC = 8;
                        }
                        goto Label_044D;
                    }
                    case 9:
                    {
                        string[] textArray13 = new string[] { "um", "arm" };
                        this.$current = this.<libDirectory>__0.Combine(textArray13);
                        if (!this.$disposing)
                        {
                            this.$PC = 10;
                        }
                        goto Label_044D;
                    }
                    case 10:
                    {
                        string[] textArray14 = new string[] { "ucrt", "arm" };
                        this.$current = this.<libDirectory>__0.Combine(textArray14);
                        if (!this.$disposing)
                        {
                            this.$PC = 11;
                        }
                        goto Label_044D;
                    }
                    case 11:
                    {
                        if (this.$this._netfxsdkDir == null)
                        {
                            break;
                        }
                        string[] textArray15 = new string[] { "lib", "um", "arm" };
                        this.$current = this.$this._netfxsdkDir.Combine(textArray15);
                        if (!this.$disposing)
                        {
                            this.$PC = 12;
                        }
                        goto Label_044D;
                    }
                    default:
                        goto Label_044B;
                }
                this.$PC = -1;
            Label_044B:
                return false;
            Label_044D:
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
                return new Msvc14Installation.<GetLibDirectories>c__Iterator1 { 
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
            internal Msvc14Installation $this;

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
                        string[] append = new string[] { "VC", "vcpackages", "platform.winmd" };
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
                return new Msvc14Installation.<GetPlatformMetadataReferences>c__Iterator2 { $this = this.$this };
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
            internal Msvc14Installation $this;

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
                        string[] append = new string[] { "windows.winmd" };
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
                return new Msvc14Installation.<GetWindowsMetadataReferences>c__Iterator3 { $this = this.$this };
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

