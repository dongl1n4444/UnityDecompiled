namespace UnityEditor.Scripting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PragmaFixing30
    {
        private static bool CheckOrFixPragmas(string fileName, bool onlyCheck)
        {
            string str = File.ReadAllText(fileName);
            StringBuilder sb = new StringBuilder(str);
            LooseComments(sb);
            Match match = PragmaMatch(sb, "strict");
            if (!match.Success)
            {
                return false;
            }
            bool success = PragmaMatch(sb, "downcast").Success;
            bool hasImplicit = PragmaMatch(sb, "implicit").Success;
            if (success && hasImplicit)
            {
                return false;
            }
            if (!onlyCheck)
            {
                DoFixPragmasInFile(fileName, str, match.Index + match.Length, success, hasImplicit);
            }
            return true;
        }

        private static string[] CollectBadFiles()
        {
            List<string> list = new List<string>();
            foreach (string str in SearchRecursive(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "*.js"))
            {
                try
                {
                    if (FileNeedsPragmaFixing(str))
                    {
                        list.Add(str);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogError("Failed to fix pragmas in file '" + str + "'.\n" + exception.Message);
                }
            }
            return list.ToArray();
        }

        private static void DoFixPragmasInFile(string fileName, string oldText, int fixPos, bool hasDowncast, bool hasImplicit)
        {
            string str = string.Empty;
            string str2 = !HasWinLineEndings(oldText) ? "\n" : "\r\n";
            if (!hasImplicit)
            {
                str = str + str2 + "#pragma implicit";
            }
            if (!hasDowncast)
            {
                str = str + str2 + "#pragma downcast";
            }
            File.WriteAllText(fileName, oldText.Insert(fixPos, str));
        }

        private static bool FileNeedsPragmaFixing(string fileName)
        {
            return CheckOrFixPragmas(fileName, true);
        }

        public static void FixFiles(string[] filesToFix)
        {
            foreach (string str in filesToFix)
            {
                try
                {
                    FixPragmasInFile(str);
                }
                catch (Exception exception)
                {
                    Debug.LogError("Failed to fix pragmas in file '" + str + "'.\n" + exception.Message);
                }
            }
        }

        private static void FixJavaScriptPragmas()
        {
            string[] paths = CollectBadFiles();
            if (paths.Length != 0)
            {
                if (!InternalEditorUtility.inBatchMode)
                {
                    PragmaFixingWindow.ShowWindow(paths);
                }
                else
                {
                    FixFiles(paths);
                }
            }
        }

        private static void FixPragmasInFile(string fileName)
        {
            CheckOrFixPragmas(fileName, false);
        }

        private static bool HasWinLineEndings(string text)
        {
            return (text.IndexOf("\r\n") != -1);
        }

        private static void LooseComments(StringBuilder sb)
        {
            Regex regex = new Regex("//");
            IEnumerator enumerator = regex.Matches(sb.ToString()).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Match current = (Match) enumerator.Current;
                    int index = current.Index;
                    while (((index < sb.Length) && (sb[index] != '\n')) && (sb[index] != '\r'))
                    {
                        sb[index++] = ' ';
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        private static Match PragmaMatch(StringBuilder sb, string pragma)
        {
            return new Regex(@"#\s*pragma\s*" + pragma).Match(sb.ToString());
        }

        [DebuggerHidden]
        private static IEnumerable<string> SearchRecursive(string dir, string mask)
        {
            return new <SearchRecursive>c__Iterator0 { 
                dir = dir,
                mask = mask,
                $PC = -2
            };
        }

        [CompilerGenerated]
        private sealed class <SearchRecursive>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal string[] $locvar0;
            internal int $locvar1;
            internal IEnumerator<string> $locvar2;
            internal string[] $locvar3;
            internal int $locvar4;
            internal int $PC;
            internal string <d>__0;
            internal string <f>__1;
            internal string <f>__2;
            internal string dir;
            internal string mask;

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
                            if (this.$locvar2 != null)
                            {
                                this.$locvar2.Dispose();
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
                        this.$locvar0 = Directory.GetDirectories(this.dir);
                        this.$locvar1 = 0;
                        goto Label_00FB;

                    case 1:
                        break;

                    case 2:
                        goto Label_0165;

                    default:
                        goto Label_018D;
                }
            Label_0079:
                try
                {
                    while (this.$locvar2.MoveNext())
                    {
                        this.<f>__1 = this.$locvar2.Current;
                        this.$current = this.<f>__1;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        goto Label_018F;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar2 != null)
                    {
                        this.$locvar2.Dispose();
                    }
                }
                this.$locvar1++;
            Label_00FB:
                if (this.$locvar1 < this.$locvar0.Length)
                {
                    this.<d>__0 = this.$locvar0[this.$locvar1];
                    this.$locvar2 = PragmaFixing30.SearchRecursive(this.<d>__0, this.mask).GetEnumerator();
                    num = 0xfffffffd;
                    goto Label_0079;
                }
                this.$locvar3 = Directory.GetFiles(this.dir, this.mask);
                this.$locvar4 = 0;
                while (this.$locvar4 < this.$locvar3.Length)
                {
                    this.<f>__2 = this.$locvar3[this.$locvar4];
                    this.$current = this.<f>__2;
                    if (!this.$disposing)
                    {
                        this.$PC = 2;
                    }
                    goto Label_018F;
                Label_0165:
                    this.$locvar4++;
                }
                this.$PC = -1;
            Label_018D:
                return false;
            Label_018F:
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
                return new PragmaFixing30.<SearchRecursive>c__Iterator0 { 
                    dir = this.dir,
                    mask = this.mask
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

