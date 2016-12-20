namespace UnityEditor.PlaymodeTestsRunner.TestRunnerCallbacks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor.PlaymodeTestsRunner;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner;

    internal class ResultSaverCallback : ScriptableObject, TestRunnerListener
    {
        private string k_DefaultResultFileName = "TestsResults.xml";
        private readonly List<TestResult> m_Results = new List<TestResult>();
        public string resultDirectory;
        public string resultFileName;

        public virtual void RunFinished(List<TestResult> testResults)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            if (!string.IsNullOrEmpty(this.resultDirectory))
            {
                currentDirectory = this.resultDirectory;
            }
            string fileName = Path.GetFileName(currentDirectory);
            if (!string.IsNullOrEmpty(fileName) && !Directory.Exists(currentDirectory))
            {
                currentDirectory = currentDirectory.Substring(0, currentDirectory.Length - fileName.Length);
            }
            else if (this.resultFileName != null)
            {
                fileName = this.resultFileName;
            }
            else
            {
                fileName = this.k_DefaultResultFileName;
            }
            if (!fileName.EndsWith(".xml"))
            {
                fileName = fileName + ".xml";
            }
            char[] separator = new char[] { '/' };
            string[] strArray = Application.dataPath.Split(separator);
            string suiteName = strArray[strArray.Length - 2];
            new XmlResultWriter(suiteName, Application.platform.ToString(), this.m_Results.ToArray()).WriteToFile(currentDirectory, fileName);
        }

        public void RunStarted(string platform, List<string> testsToRun)
        {
        }

        public void TestFinished(TestResult test)
        {
            this.m_Results.Add(test);
        }

        public void TestStarted(string fullName)
        {
        }
    }
}

