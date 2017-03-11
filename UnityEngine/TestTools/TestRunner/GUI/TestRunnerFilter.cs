namespace UnityEngine.TestTools.TestRunner.GUI
{
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using NUnit.Framework.Internal.Filters;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    [Serializable]
    internal class TestRunnerFilter
    {
        [CompilerGenerated]
        private static Func<string, FullNameFilter> <>f__am$cache0;
        public static TestRunnerFilter empty = new TestRunnerFilter();
        public string[] names;

        public TestRunnerFilter()
        {
        }

        public TestRunnerFilter(string name) : this(textArray1)
        {
            string[] textArray1 = new string[] { name };
        }

        public TestRunnerFilter(string[] names)
        {
            this.names = names;
        }

        public ITestFilter BuildNUnitFilter()
        {
            TestFilter empty = TestFilter.Empty;
            if ((this.names == null) || (this.names.Length == 0))
            {
                return empty;
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = n => new FullNameFilter(n);
            }
            return new OrFilter(Enumerable.Select<string, FullNameFilter>(this.names, <>f__am$cache0).ToArray<FullNameFilter>());
        }

        public bool Matches(string name) => 
            this.NameMatches(name);

        public bool Matches(TestRunnerResult result) => 
            this.NameMatches(result.fullName);

        private bool NameMatches(string name)
        {
            if ((this.names == null) || (this.names.Length == 0))
            {
                return true;
            }
            foreach (string str in this.names)
            {
                if (name.Contains(str))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

