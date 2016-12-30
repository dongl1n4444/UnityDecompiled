namespace Unity.IL2CPP.Statistics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity.TinyProfiling;

    public class ProfilerSnapshot
    {
        private readonly ReadOnlyCollection<TinyProfiler.ThreadContext> _profilerData;

        public ProfilerSnapshot(ReadOnlyCollection<TinyProfiler.ThreadContext> profilerData)
        {
            this._profilerData = profilerData;
        }

        public static ProfilerSnapshot Capture() => 
            new ProfilerSnapshot(TinyProfiler.CaptureSnapshot());

        [DebuggerHidden]
        public IEnumerable<TinyProfiler.TimedSection> GetSectionsByLabel(string label) => 
            new <GetSectionsByLabel>c__Iterator0 { 
                label = label,
                $this = this,
                $PC = -2
            };

        [CompilerGenerated]
        private sealed class <GetSectionsByLabel>c__Iterator0 : IEnumerable, IEnumerable<TinyProfiler.TimedSection>, IEnumerator, IDisposable, IEnumerator<TinyProfiler.TimedSection>
        {
            internal TinyProfiler.TimedSection $current;
            internal bool $disposing;
            internal IEnumerator<TinyProfiler.ThreadContext> $locvar0;
            internal List<TinyProfiler.TimedSection>.Enumerator $locvar1;
            internal int $PC;
            internal ProfilerSnapshot $this;
            internal TinyProfiler.ThreadContext <context>__1;
            internal TinyProfiler.TimedSection <section>__2;
            internal string label;

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
                            try
                            {
                            }
                            finally
                            {
                                this.$locvar1.Dispose();
                            }
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
                        this.$locvar0 = this.$this._profilerData.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0141;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_007B;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<context>__1 = this.$locvar0.Current;
                        this.$locvar1 = this.<context>__1.Sections.GetEnumerator();
                        num = 0xfffffffd;
                    Label_007B:
                        try
                        {
                            switch (num)
                            {
                                case 1:
                                    goto Label_00DD;
                            }
                            while (this.$locvar1.MoveNext())
                            {
                                this.<section>__2 = this.$locvar1.Current;
                                if (this.<section>__2.Label == this.label)
                                {
                                    this.$current = this.<section>__2;
                                    if (!this.$disposing)
                                    {
                                        this.$PC = 1;
                                    }
                                    flag = true;
                                    return true;
                                }
                            Label_00DD:;
                            }
                        }
                        finally
                        {
                            if (!flag)
                            {
                            }
                            this.$locvar1.Dispose();
                        }
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
            Label_0141:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<TinyProfiler.TimedSection> IEnumerable<TinyProfiler.TimedSection>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new ProfilerSnapshot.<GetSectionsByLabel>c__Iterator0 { 
                    $this = this.$this,
                    label = this.label
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Unity.TinyProfiling.TinyProfiler.TimedSection>.GetEnumerator();

            TinyProfiler.TimedSection IEnumerator<TinyProfiler.TimedSection>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

