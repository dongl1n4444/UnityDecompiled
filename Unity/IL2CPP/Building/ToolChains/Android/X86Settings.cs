namespace Unity.IL2CPP.Building.ToolChains.Android
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class X86Settings : TargetArchitectureSettings
    {
        public override string ABI =>
            "x86";

        public override string Arch =>
            "x86";

        public override string BinPrefix =>
            "i686-linux-android";

        public override IEnumerable<string> CxxFlags =>
            new <>c__Iterator0 { $PC=-2 };

        public override string Platform =>
            "i686-none-linux-android";

        public override string TCPrefix =>
            "x86";

        [CompilerGenerated]
        private sealed class <>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
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
                        this.$current = "-mtune=atom";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_0090;

                    case 1:
                        this.$current = "-mssse3";
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        goto Label_0090;

                    case 2:
                        this.$current = "-mfpmath=sse";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_0090;

                    case 3:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0090:
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
                return new X86Settings.<>c__Iterator0();
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

