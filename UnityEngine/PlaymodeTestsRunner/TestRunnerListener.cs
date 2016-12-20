namespace UnityEngine.PlaymodeTestsRunner
{
    using System;
    using System.Collections.Generic;

    internal interface TestRunnerListener
    {
        void RunFinished(List<TestResult> testResults);
        void RunStarted(string platform, List<string> testsToRun);
        void TestFinished(TestResult test);
        void TestStarted(string testName);
    }
}

