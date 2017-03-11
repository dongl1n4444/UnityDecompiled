﻿namespace UnityEngine.TestTools.Utils
{
    using NUnit.Framework.Interfaces;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine.TestTools;
    using UnityEngine.TestTools.NUnitExtensions;

    internal class TestAssemblyProvider
    {
        private string[] DefaultUserAssemblies = new string[] { "Assembly-CSharp", "Assembly-UnityScript", "Assembly-CSharp-testable", "Assembly-UnityScript-testable", "Assembly-CSharp-firstpass", "Assembly-UnityScript-firstpass", "Assembly-CSharp-firstpass-testable", "Assembly-UnityScript-firstpass-testable" };
        private string[] DefaultUserEditorAssemblies = new string[] { "Assembly-CSharp-Editor", "Assembly-UnityScript-Editor", "Assembly-CSharp-Editor-testable", "Assembly-UnityScript-Editor-testable", "Assembly-CSharp-Editor-firstpass", "Assembly-UnityScript-Editor-firstpass", "Assembly-CSharp-Editor-firstpass-testable", "Assembly-UnityScript-Editor-firstpass-testable" };

        public ITest GetTestsWithNUnit(TestPlatform testPlatform)
        {
            IEnumerable<Assembly> userAssemblies = this.GetUserAssemblies(testPlatform.IsFlagIncluded(TestPlatform.EditMode));
            Dictionary<string, object> nUnitTestBuilderSettings = UnityTestAssemblyBuilder.GetNUnitTestBuilderSettings(testPlatform);
            return UnityTestAssemblyBuilder.GetNUnitTestBuilder(testPlatform).Build(userAssemblies.ToArray<Assembly>(), nUnitTestBuilderSettings);
        }

        [DebuggerHidden]
        internal virtual IEnumerable<Assembly> GetUserAssemblies(bool includeEditorAssemblies) => 
            new <GetUserAssemblies>c__Iterator0 { 
                includeEditorAssemblies = includeEditorAssemblies,
                $this = this,
                $PC = -2
            };

        [CompilerGenerated]
        private sealed class <GetUserAssemblies>c__Iterator0 : IEnumerable, IEnumerable<Assembly>, IEnumerator, IDisposable, IEnumerator<Assembly>
        {
            internal Assembly $current;
            internal bool $disposing;
            internal List<string>.Enumerator $locvar0;
            internal int $PC;
            internal TestAssemblyProvider $this;
            internal Assembly <a>__2;
            internal List<string> <assemblyList>__0;
            internal string <userAssembly>__1;
            internal bool includeEditorAssemblies;

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
                            this.$locvar0.Dispose();
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
                        this.<assemblyList>__0 = this.$this.DefaultUserAssemblies.ToList<string>();
                        if (this.includeEditorAssemblies)
                        {
                            this.<assemblyList>__0.AddRange(this.$this.DefaultUserEditorAssemblies);
                        }
                        this.$locvar0 = this.<assemblyList>__0.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0112;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_00DF;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<userAssembly>__1 = this.$locvar0.Current;
                        try
                        {
                            this.<a>__2 = Assembly.Load(this.<userAssembly>__1);
                        }
                        catch (FileNotFoundException)
                        {
                            continue;
                        }
                        if (this.<a>__2 != null)
                        {
                            this.$current = this.<a>__2;
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                            flag = true;
                            return true;
                        }
                    Label_00DF:;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.$locvar0.Dispose();
                }
                this.$PC = -1;
            Label_0112:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<Assembly> IEnumerable<Assembly>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new TestAssemblyProvider.<GetUserAssemblies>c__Iterator0 { 
                    $this = this.$this,
                    includeEditorAssemblies = this.includeEditorAssemblies
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Reflection.Assembly>.GetEnumerator();

            Assembly IEnumerator<Assembly>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

