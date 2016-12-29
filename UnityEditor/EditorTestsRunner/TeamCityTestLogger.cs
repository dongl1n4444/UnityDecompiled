namespace UnityEditor.EditorTestsRunner
{
    using System;
    using UnityEditor.EditorTests;
    using UnityEngine;

    internal class TeamCityTestLogger : ITestRunnerCallback
    {
        private string suiteName;

        private static string Escape(string message) => 
            message?.Replace("|", "||").Replace("[", "|[").Replace("]", "|]").Replace("\r", "|r").Replace("\n", "|n").Replace("'", "|'");

        private static void LogFailedTest(ITestResult result)
        {
            string format = "{0}{1}";
            if (!string.IsNullOrEmpty(result.message) && !string.IsNullOrEmpty(result.stackTrace))
            {
                format = "{0}|n{1}";
            }
            string str2 = Escape(result.message);
            string str3 = string.Format(format, str2, Escape(result.stackTrace));
            Debug.Log($"##teamcity[testFailed name='{result.fullName}' message='{str2}' details='{str3}']");
        }

        public void RunFinished()
        {
            Debug.Log($"##teamcity[testSuiteFinished name='{this.suiteName}']");
        }

        public void RunFinishedException(Exception exception)
        {
        }

        public void RunStarted(string suiteName, int testCount)
        {
            this.suiteName = suiteName;
            Debug.Log($"##teamcity[testSuiteStarted name='{suiteName}']");
        }

        public void TestFinished(ITestResult testResult)
        {
            if (testResult.isIgnored)
            {
                Debug.Log($"##teamcity[testIgnored name='{testResult.fullName}' message='{Escape(testResult.message)}']");
            }
            else if (!testResult.isSuccess)
            {
                LogFailedTest(testResult);
            }
            Debug.Log($"##teamcity[testFinished name='{testResult.fullName}' duration='{Convert.ToInt32((double) (testResult.duration * 1000.0))}']");
        }

        public void TestStarted(string fullName)
        {
            Debug.Log($"##teamcity[testStarted name='{fullName}']");
        }
    }
}

