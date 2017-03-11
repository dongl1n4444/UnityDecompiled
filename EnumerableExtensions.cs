using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

public static class EnumerableExtensions
{
    public static string AggregateWith(this IEnumerable<string> elements, string separator)
    {
        <AggregateWith>c__AnonStorey2 storey = new <AggregateWith>c__AnonStorey2 {
            separator = separator
        };
        if (elements.Any<string>())
        {
            return elements.Aggregate<string>(new Func<string, string, string>(storey.<>m__0));
        }
        return string.Empty;
    }

    public static string AggregateWithComma(this IEnumerable<string> elements) => 
        elements.AggregateWith(", ");

    public static string AggregateWithSpace(this IEnumerable<string> elements) => 
        elements.AggregateWith(" ");

    public static string AggregateWithUnderscore(this IEnumerable<string> elements) => 
        elements.AggregateWith("_");

    [DebuggerHidden]
    public static IEnumerable<T> Append<T>(this IEnumerable<T> inputs, T extra) => 
        new <Append>c__Iterator0<T> { 
            inputs = inputs,
            extra = extra,
            $PC = -2
        };

    [DebuggerHidden]
    public static IEnumerable<T> Prepend<T>(this IEnumerable<T> inputs, T extra) => 
        new <Prepend>c__Iterator1<T> { 
            extra = extra,
            inputs = inputs,
            $PC = -2
        };

    [CompilerGenerated]
    private sealed class <AggregateWith>c__AnonStorey2
    {
        internal string separator;

        internal string <>m__0(string buff, string s) => 
            (buff + this.separator + s);
    }

    [CompilerGenerated]
    private sealed class <Append>c__Iterator0<T> : IEnumerable, IEnumerable<T>, IEnumerator, IDisposable, IEnumerator<T>
    {
        internal T $current;
        internal bool $disposing;
        internal IEnumerator<T> $locvar0;
        internal int $PC;
        internal T <i>__1;
        internal T extra;
        internal IEnumerable<T> inputs;

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
                    this.$locvar0 = this.inputs.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                case 2:
                    this.$PC = -1;
                    goto Label_00D8;

                default:
                    goto Label_00D8;
            }
            try
            {
                while (this.$locvar0.MoveNext())
                {
                    this.<i>__1 = this.$locvar0.Current;
                    this.$current = this.<i>__1;
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    flag = true;
                    goto Label_00DA;
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
            this.$current = this.extra;
            if (!this.$disposing)
            {
                this.$PC = 2;
            }
            goto Label_00DA;
        Label_00D8:
            return false;
        Label_00DA:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new EnumerableExtensions.<Append>c__Iterator0<T> { 
                inputs = this.inputs,
                extra = this.extra
            };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator() => 
            this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();

        T IEnumerator<T>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }

    [CompilerGenerated]
    private sealed class <Prepend>c__Iterator1<T> : IEnumerable, IEnumerable<T>, IEnumerator, IDisposable, IEnumerator<T>
    {
        internal T $current;
        internal bool $disposing;
        internal IEnumerator<T> $locvar0;
        internal int $PC;
        internal T <i>__1;
        internal T extra;
        internal IEnumerable<T> inputs;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$disposing = true;
            this.$PC = -1;
            switch (num)
            {
                case 2:
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
                    this.$current = this.extra;
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    goto Label_00DA;

                case 1:
                    this.$locvar0 = this.inputs.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 2:
                    break;

                default:
                    goto Label_00D8;
            }
            try
            {
                while (this.$locvar0.MoveNext())
                {
                    this.<i>__1 = this.$locvar0.Current;
                    this.$current = this.<i>__1;
                    if (!this.$disposing)
                    {
                        this.$PC = 2;
                    }
                    flag = true;
                    goto Label_00DA;
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
        Label_00D8:
            return false;
        Label_00DA:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new EnumerableExtensions.<Prepend>c__Iterator1<T> { 
                extra = this.extra,
                inputs = this.inputs
            };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator() => 
            this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();

        T IEnumerator<T>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }
}

