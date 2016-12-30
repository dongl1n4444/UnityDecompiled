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

    internal class Msvc10Installation : MsvcInstallation
    {
        public Msvc10Installation(NPath visualStudioDir) : base(new Version(10, 0), visualStudioDir, false)
        {
            base.SDKDirectory = WindowsSDKs.GetWindows7SDKDirectory();
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
                architecture = architecture,
                $this = this,
                $PC = -2
            };

        public override IEnumerable<Type> SupportedArchitectures =>
            new Type[] { typeof(x86Architecture), typeof(x64Architecture) };

        [CompilerGenerated]
        private sealed class <GetIncludeDirectories>c__Iterator0 : IEnumerable, IEnumerable<NPath>, IEnumerator, IDisposable, IEnumerator<NPath>
        {
            internal NPath $current;
            internal bool $disposing;
            internal int $PC;
            internal Msvc10Installation $this;

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
                        goto Label_00A7;
                    }
                    case 1:
                    {
                        string[] textArray2 = new string[] { "Include" };
                        this.$current = this.$this.SDKDirectory.Combine(textArray2);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_00A7;
                    }
                    case 2:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_00A7:
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
                return new Msvc10Installation.<GetIncludeDirectories>c__Iterator0 { $this = this.$this };
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
            internal Msvc10Installation $this;
            internal Architecture architecture;

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
                        if (this.architecture is x86Architecture)
                        {
                            string[] append = new string[] { "VC", "lib" };
                            this.$current = this.$this.VisualStudioDirectory.Combine(append);
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                        }
                        else
                        {
                            if (!(this.architecture is x64Architecture))
                            {
                                throw new NotSupportedException("Unknown architecture: " + this.architecture);
                            }
                            string[] textArray3 = new string[] { "VC", "lib", "amd64" };
                            this.$current = this.$this.VisualStudioDirectory.Combine(textArray3);
                            if (!this.$disposing)
                            {
                                this.$PC = 3;
                            }
                        }
                        goto Label_017C;

                    case 1:
                    {
                        string[] textArray2 = new string[] { "Lib" };
                        this.$current = this.$this.SDKDirectory.Combine(textArray2);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_017C;
                    }
                    case 2:
                    case 4:
                        this.$PC = -1;
                        break;

                    case 3:
                    {
                        string[] textArray4 = new string[] { "Lib", "x64" };
                        this.$current = this.$this.SDKDirectory.Combine(textArray4);
                        if (!this.$disposing)
                        {
                            this.$PC = 4;
                        }
                        goto Label_017C;
                    }
                }
                return false;
            Label_017C:
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
                return new Msvc10Installation.<GetLibDirectories>c__Iterator1 { 
                    $this = this.$this,
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
    }
}

