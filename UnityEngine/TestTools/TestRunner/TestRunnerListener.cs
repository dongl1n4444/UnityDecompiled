namespace UnityEngine.TestTools.TestRunner
{
    using NUnit.Framework.Interfaces;
    using System;

    internal interface TestRunnerListener
    {
        void RunFinished(ITestResult testResults);
        void RunStarted(ITest testsToRun);
        void TestFinished(ITestResult result);
        void TestStarted(ITest test);
    }
}

