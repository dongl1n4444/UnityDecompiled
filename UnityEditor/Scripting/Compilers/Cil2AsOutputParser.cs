namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class Cil2AsOutputParser : UnityScriptCompilerOutputParser
    {
        private static CompilerMessage CompilerErrorFor(StringBuilder currentErrorBuffer) => 
            new CompilerMessage { 
                type = CompilerMessageType.Error,
                message = currentErrorBuffer.ToString()
            };

        [DebuggerHidden]
        public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, string[] standardOutput, bool compilationHadFailure) => 
            new <Parse>c__Iterator0 { 
                errorOutput = errorOutput,
                $PC = -2
            };

        [CompilerGenerated]
        private sealed class <Parse>c__Iterator0 : IEnumerable, IEnumerable<CompilerMessage>, IEnumerator, IDisposable, IEnumerator<CompilerMessage>
        {
            internal CompilerMessage $current;
            internal bool $disposing;
            internal string[] $locvar0;
            internal int $locvar1;
            internal int $PC;
            internal StringBuilder <currentErrorBuffer>__0;
            internal bool <parsingError>__0;
            internal string <str>__1;
            internal string[] errorOutput;

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
                        this.<parsingError>__0 = false;
                        this.<currentErrorBuffer>__0 = new StringBuilder();
                        this.$locvar0 = this.errorOutput;
                        this.$locvar1 = 0;
                        goto Label_0115;

                    case 1:
                        this.<currentErrorBuffer>__0.Length = 0;
                        break;

                    case 2:
                        goto Label_0158;

                    default:
                        goto Label_015F;
                }
            Label_00B9:
                this.<currentErrorBuffer>__0.AppendLine(this.<str>__1.Substring("ERROR: ".Length));
                this.<parsingError>__0 = true;
            Label_0106:
                this.$locvar1++;
            Label_0115:
                if (this.$locvar1 < this.$locvar0.Length)
                {
                    this.<str>__1 = this.$locvar0[this.$locvar1];
                    if (!this.<str>__1.StartsWith("ERROR: "))
                    {
                        if (this.<parsingError>__0)
                        {
                            this.<currentErrorBuffer>__0.AppendLine(this.<str>__1);
                        }
                        goto Label_0106;
                    }
                    if (!this.<parsingError>__0)
                    {
                        goto Label_00B9;
                    }
                    this.$current = Cil2AsOutputParser.CompilerErrorFor(this.<currentErrorBuffer>__0);
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    goto Label_0161;
                }
                if (this.<parsingError>__0)
                {
                    this.$current = Cil2AsOutputParser.CompilerErrorFor(this.<currentErrorBuffer>__0);
                    if (!this.$disposing)
                    {
                        this.$PC = 2;
                    }
                    goto Label_0161;
                }
            Label_0158:
                this.$PC = -1;
            Label_015F:
                return false;
            Label_0161:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<CompilerMessage> IEnumerable<CompilerMessage>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new Cil2AsOutputParser.<Parse>c__Iterator0 { errorOutput = this.errorOutput };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<UnityEditor.Scripting.Compilers.CompilerMessage>.GetEnumerator();

            CompilerMessage IEnumerator<CompilerMessage>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

