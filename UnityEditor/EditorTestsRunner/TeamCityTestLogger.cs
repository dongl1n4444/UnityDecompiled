namespace UnityEditor.EditorTestsRunner
{
    using System;
    using UnityEditor.EditorTests;
    using UnityEngine;

    internal class TeamCityTestLogger : ITestRunnerCallback
    {
        private string suiteName;

        private static string Escape(string message)
        {
            if (message == null)
            {
                return "";
            }
            return message.Replace("|", "||").Replace("[", "|[").Replace("]", "|]").Replace("\r", "|r").Replace("\n", "|n").Replace("'", "|'");
        }

        private static void LogFailedTest(ITestResult result)
        {
            string format = "{0}{1}";
            if (!string.IsNullOrEmpty(result.message) && !string.IsNullOrEmpty(result.stackTrace))
            {
                format = "{0}|n{1}";
            }
            string str2 = Escape(result.message);
            string str3 = string.Format(format, str2, Escape(result.stackTrace));
            Debug.Log(string.Format("##teamcity[testFailed name='{0}' message='{1}' details='{2}']", result.fullName, str2, str3));
        }

        public void RunFinished()
        {
            Debug.Log(string.Format("##teamcity[testSuiteFinished name='{0}']", this.suiteName));
        }

        public void RunFinishedException(Exception exception)
        {
        }

        public void RunStarted(string suiteName, int testCount)
        {
            this.suiteName = suiteName;
            Debug.Log(string.Format("##teamcity[testSuiteStarted name='{0}']", suiteName));
        }

        public void TestFinished(ITestResult testResult)
        {
            if (testResult.isIgnored)
            {
                Debug.Log(string.Format("##teamcity[testIgnored name='{0}' message='{1}']", testResult.fullName, Escape(testResult.message)));
            }
            else if (!testResult.isSuccess)
            {
                LogFailedTest(testResult);
            }
            Debug.Log(string.Format("##teamcity[testFinished name='{0}' duration='{1}']", testResult.fullName, Convert.ToInt32((double) (testResult.duration * 1000.0))));
        }

        public void TestStarted(string fullName)
        {
            Debug.Log(string.Format("##teamcity[testStarted name='{0}']", fullName));
        }
    }
}

