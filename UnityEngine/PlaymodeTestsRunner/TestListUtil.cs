namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine.PlaymodeTests;
    using UnityEngine.PlaymodeTestsRunner.TestListBuilder;

    internal class TestListUtil
    {
        [CompilerGenerated]
        private static Func<Assembly, IEnumerable<Type>> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodInfo, bool> <>f__am$cache1;
        private string[] DefaultUserAssemblies;
        private string[] DefaultUserEditorAssemblies;
        private bool isEditModeRunner;

        private TestListUtil() : this(false)
        {
        }

        public TestListUtil(bool isEditModeRunner)
        {
            this.DefaultUserAssemblies = new string[] { "Assembly-CSharp", "Assembly-UnityScript", "Assembly-CSharp-testable", "Assembly-UnityScript-testable", "Assembly-CSharp-firstpass", "Assembly-UnityScript-firstpass", "Assembly-CSharp-firstpass-testable", "Assembly-UnityScript-firstpass-testable" };
            this.DefaultUserEditorAssemblies = new string[] { "Assembly-CSharp-Editor", "Assembly-UnityScript-Editor", "Assembly-CSharp-Editor-testable", "Assembly-UnityScript-Editor-testable", "Assembly-CSharp-Editor-firstpass", "Assembly-UnityScript-Editor-firstpass", "Assembly-CSharp-Editor-firstpass-testable", "Assembly-UnityScript-Editor-firstpass-testable" };
            this.isEditModeRunner = isEditModeRunner;
        }

        public TestListElement GetEditmodeTests() => 
            this.GetEditmodeTests(null);

        public TestListElement GetEditmodeTests(TestRunnerFilter filter) => 
            this.GetTests(TestPlatform.EditMode);

        public TestListElement GetPlaymodeTests() => 
            this.GetPlaymodeTests(null);

        public TestListElement GetPlaymodeTests(TestRunnerFilter filter) => 
            this.GetTests(TestPlatform.PlayMode);

        private IEnumerable<MethodInfo> GetTestMethodList(TestPlatform testPlatform, Type type)
        {
            <GetTestMethodList>c__AnonStorey1 storey = new <GetTestMethodList>c__AnonStorey1 {
                testPlatform = testPlatform
            };
            return Enumerable.Where<MethodInfo>(type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance), new Func<MethodInfo, bool>(storey.<>m__0));
        }

        private TestListElement GetTests(TestPlatform testPlatform)
        {
            IEnumerable<Assembly> userAssemblies = this.GetUserAssemblies(testPlatform.IsFlagIncluded(TestPlatform.EditMode));
            IEnumerable<Type> typesFromAsseblies = this.GetTypesFromAsseblies(testPlatform, userAssemblies);
            return this.ParseTypesForTests(testPlatform, typesFromAsseblies);
        }

        internal virtual IEnumerable<Type> GetTypesFromAsseblies(TestPlatform editMode, IEnumerable<Assembly> assemblies)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = a => a.GetTypes();
            }
            return Enumerable.Where<Type>(Enumerable.SelectMany<Assembly, Type>(assemblies, <>f__am$cache0), new Func<Type, bool>(this.IsTypeATestForPlatform));
        }

        [DebuggerHidden]
        internal virtual IEnumerable<Assembly> GetUserAssemblies(bool includeEditorAssemblies) => 
            new <GetUserAssemblies>c__Iterator0 { 
                includeEditorAssemblies = includeEditorAssemblies,
                $this = this,
                $PC = -2
            };

        private bool IsTypeATestForPlatform(Type type)
        {
            if (type.GetCustomAttributes(typeof(TestAttribute), true).Any<object>())
            {
                return true;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = m => m.GetCustomAttributes(typeof(TestAttribute), true).Any<object>();
            }
            return Enumerable.Any<MethodInfo>(type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance), <>f__am$cache1);
        }

        private TestListElement ParseTypesForTests(TestPlatform testPlatform, IEnumerable<Type> types)
        {
            TestListGroup group = new TestListGroup("Root", "Root", "Root");
            TestsConstraint testsConstraint = testPlatform.GetTestsConstraint();
            foreach (Type type in types)
            {
                if (testsConstraint.IsClassTestSupported() && testsConstraint.IsClassATest(type))
                {
                    TestListItem child = new TestListItem(type.FullName, type.Name, type.FullName) {
                        name = "",
                        fullName = "",
                        testExecutor = new MonoBehaviourExecutor(type)
                    };
                    group.AddChildren(child);
                }
                if (testsConstraint.IsMethodTestSupported())
                {
                    IEnumerable<MethodInfo> testMethodList = this.GetTestMethodList(testPlatform, type);
                    TestListGroup group2 = new TestListGroup(type.FullName, type.Name, type.FullName);
                    foreach (MethodInfo info in testMethodList)
                    {
                        string id = type.FullName + "+" + info.Name;
                        TestListItem item2 = new TestListItem(id, info.Name, id);
                        if (this.isEditModeRunner)
                        {
                            item2.testExecutor = new EditModeExecutor(type, info);
                        }
                        else
                        {
                            item2.testExecutor = new MethodExecutor(type, info);
                        }
                        group2.AddChildren(item2);
                    }
                    if (testMethodList.Any<MethodInfo>())
                    {
                        group.AddChildren(group2);
                    }
                }
            }
            return group;
        }

        [CompilerGenerated]
        private sealed class <GetTestMethodList>c__AnonStorey1
        {
            internal TestPlatform testPlatform;

            internal bool <>m__0(MethodInfo m)
            {
                if (m.GetCustomAttributes(typeof(TestAttribute), true).Length == 0)
                {
                    return false;
                }
                TestAttribute attribute = (TestAttribute) m.GetCustomAttributes(typeof(TestAttribute), true)[0];
                if (!attribute.IncludeOnPlatform(this.testPlatform))
                {
                    return false;
                }
                if (!this.testPlatform.GetTestsConstraint().IsMethodATest(m))
                {
                    return false;
                }
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <GetUserAssemblies>c__Iterator0 : IEnumerable, IEnumerable<Assembly>, IEnumerator, IDisposable, IEnumerator<Assembly>
        {
            internal Assembly $current;
            internal bool $disposing;
            internal List<string>.Enumerator $locvar0;
            internal int $PC;
            internal TestListUtil $this;
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
                return new TestListUtil.<GetUserAssemblies>c__Iterator0 { 
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

