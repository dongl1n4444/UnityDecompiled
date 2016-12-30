namespace UnityEditor.TestTools.TestRunner
{
    using NUnit.Framework.Interfaces;
    using System;
    using System.IO;
    using System.Xml;
    using UnityEditor.Utils;
    using UnityEngine;
    using UnityEngine.TestTools.TestRunner;

    internal class ResultSaverCallback : ScriptableObject, TestRunnerListener
    {
        private DateTime m_StartTime;
        public string resultFilePath;

        private void EnsureDirectoryExists(string resultFilePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(resultFilePath));
        }

        private string GetDefaultResultFilePath()
        {
            string str = "TestResults-" + DateTime.Now.Ticks + ".xml";
            string currentDirectory = Directory.GetCurrentDirectory();
            string[] components = new string[] { currentDirectory, str };
            return Paths.Combine(components);
        }

        public virtual void RunFinished(ITestResult testResults)
        {
            if (string.IsNullOrEmpty(this.resultFilePath))
            {
                this.resultFilePath = this.GetDefaultResultFilePath();
            }
            this.EnsureDirectoryExists(this.resultFilePath);
            XmlWriterSettings settings = new XmlWriterSettings {
                Indent = true,
                NewLineOnAttributes = false
            };
            using (StreamWriter writer = File.CreateText(this.resultFilePath))
            {
                using (XmlWriter writer2 = XmlWriter.Create(writer, settings))
                {
                    testResults.ToXml(true).WriteTo(writer2);
                }
            }
            Debug.Log("Saving results to: " + this.resultFilePath);
        }

        public void RunStarted(ITest testsToRun)
        {
        }

        public void TestFinished(ITestResult test)
        {
        }

        public void TestStarted(ITest fullName)
        {
        }
    }
}

