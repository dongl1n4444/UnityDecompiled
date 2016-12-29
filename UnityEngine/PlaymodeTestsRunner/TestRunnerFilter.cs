namespace UnityEngine.PlaymodeTestsRunner
{
    using System;

    [Serializable]
    internal class TestRunnerFilter
    {
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

        public bool Matches(string name) => 
            this.NameMatches(name);

        public bool Matches(TestResult result) => 
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

