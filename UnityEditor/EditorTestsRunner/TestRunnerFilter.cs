namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Core;
    using System;

    internal class TestRunnerFilter
    {
        public string[] categories;
        public static TestRunnerFilter empty = new TestRunnerFilter();
        public string[] names;
        public TestName[] testnames;
    }
}

