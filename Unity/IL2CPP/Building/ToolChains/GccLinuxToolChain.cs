namespace Unity.IL2CPP.Building.ToolChains
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Common;

    public class GccLinuxToolChain : GccToolChain
    {
        public GccLinuxToolChain(Unity.IL2CPP.Common.Architecture architecture, BuildConfiguration buildConfiguration) : base(architecture, buildConfiguration)
        {
        }

        [DebuggerHidden]
        public override IEnumerable<string> ToolChainDefines() => 
            new <ToolChainDefines>c__Iterator0 { 
                $this = this,
                $PC = -2
            };

        [CompilerGenerated]
        private sealed class <ToolChainDefines>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal IEnumerator<string> $locvar0;
            internal int $PC;
            internal GccLinuxToolChain $this;
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
                        this.$PC = -1;
                        goto Label_00DC;

                    default:
                        goto Label_00DC;
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
                        goto Label_00DE;
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
                this.$current = "LINUX";
                if (!this.$disposing)
                {
                    this.$PC = 2;
                }
                goto Label_00DE;
            Label_00DC:
                return false;
            Label_00DE:
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
                return new GccLinuxToolChain.<ToolChainDefines>c__Iterator0 { $this = this.$this };
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

