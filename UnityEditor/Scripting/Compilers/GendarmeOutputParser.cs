namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class GendarmeOutputParser : UnityScriptCompilerOutputParser
    {
        private static CompilerMessage CompilerErrorFor(GendarmeRuleData gendarmeRuleData)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(gendarmeRuleData.Problem);
            builder.AppendLine(gendarmeRuleData.Details);
            builder.AppendLine(string.IsNullOrEmpty(gendarmeRuleData.Location) ? $"{gendarmeRuleData.Source} at line : {gendarmeRuleData.Line}" : gendarmeRuleData.Location);
            string str = builder.ToString();
            return new CompilerMessage { 
                type = CompilerMessageType.Error,
                message = str,
                file = gendarmeRuleData.File,
                line = gendarmeRuleData.Line,
                column = 1
            };
        }

        private static string GetFileNameFrome(string currentLine)
        {
            int startIndex = currentLine.LastIndexOf("* Source:") + "* Source:".Length;
            int index = currentLine.IndexOf("(");
            if ((startIndex != -1) && (index != -1))
            {
                return currentLine.Substring(startIndex, index - startIndex).Trim();
            }
            return "";
        }

        private static GendarmeRuleData GetGendarmeRuleDataFor(IList<string> output, int index)
        {
            GendarmeRuleData data = new GendarmeRuleData();
            for (int i = index; i < output.Count; i++)
            {
                string currentLine = output[i];
                if (currentLine.StartsWith("Problem:"))
                {
                    data.Problem = currentLine.Substring(currentLine.LastIndexOf("Problem:", StringComparison.Ordinal) + "Problem:".Length);
                }
                else if (currentLine.StartsWith("* Details"))
                {
                    data.Details = currentLine;
                }
                else if (currentLine.StartsWith("* Source"))
                {
                    data.IsAssemblyError = false;
                    data.Source = currentLine;
                    data.Line = GetLineNumberFrom(currentLine);
                    data.File = GetFileNameFrome(currentLine);
                }
                else if (currentLine.StartsWith("* Severity"))
                {
                    data.Severity = currentLine;
                }
                else if (currentLine.StartsWith("* Location"))
                {
                    data.IsAssemblyError = true;
                    data.Location = currentLine;
                }
                else if (currentLine.StartsWith("* Target"))
                {
                    data.Target = currentLine;
                }
                else
                {
                    data.LastIndex = i;
                    return data;
                }
            }
            return data;
        }

        private static int GetLineNumberFrom(string currentLine)
        {
            int startIndex = currentLine.IndexOf("(") + 2;
            int index = currentLine.IndexOf(")");
            if ((startIndex != -1) && (index != -1))
            {
                return int.Parse(currentLine.Substring(startIndex, index - startIndex));
            }
            return 0;
        }

        public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, bool compilationHadFailure)
        {
            throw new ArgumentException("Gendarme Output Parser needs standard out");
        }

        [DebuggerHidden]
        public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, string[] standardOutput, bool compilationHadFailure) => 
            new <Parse>c__Iterator0 { 
                standardOutput = standardOutput,
                $PC = -2
            };

        [CompilerGenerated]
        private sealed class <Parse>c__Iterator0 : IEnumerable, IEnumerable<CompilerMessage>, IEnumerator, IDisposable, IEnumerator<CompilerMessage>
        {
            internal CompilerMessage $current;
            internal bool $disposing;
            internal int $PC;
            internal CompilerMessage <compilerErrorFor>__2;
            internal GendarmeRuleData <grd>__2;
            internal int <i>__1;
            internal string[] standardOutput;

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
                        this.<i>__1 = 0;
                        goto Label_00BA;

                    case 1:
                        this.<i>__1 = this.<grd>__2.LastIndex + 1;
                        break;

                    default:
                        goto Label_00D4;
                }
            Label_00AC:
                this.<i>__1++;
            Label_00BA:
                if (this.<i>__1 < this.standardOutput.Length)
                {
                    if (this.standardOutput[this.<i>__1].StartsWith("Problem:"))
                    {
                        this.<grd>__2 = GendarmeOutputParser.GetGendarmeRuleDataFor(this.standardOutput, this.<i>__1);
                        this.<compilerErrorFor>__2 = GendarmeOutputParser.CompilerErrorFor(this.<grd>__2);
                        this.$current = this.<compilerErrorFor>__2;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;
                    }
                    goto Label_00AC;
                }
                this.$PC = -1;
            Label_00D4:
                return false;
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
                return new GendarmeOutputParser.<Parse>c__Iterator0 { standardOutput = this.standardOutput };
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

