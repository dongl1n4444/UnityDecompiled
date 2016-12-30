namespace UnityEditor.TestTools.TestRunner
{
    using NUnit.Framework.Interfaces;
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.TestTools.TestRunner;

    internal class TestRunnerCallback : ScriptableObject, TestRunnerListener
    {
        public void RunFinished(ITestResult testResults)
        {
            EditorApplication.isPlaying = false;
            AssetDatabase.DeleteAsset(PlaymodeTestsController.GetController().settings.bootstrapScene);
        }

        public void RunStarted(ITest testsToRun)
        {
        }

        public void TestFinished(ITestResult test)
        {
        }

        public void TestStarted(ITest testName)
        {
        }
    }
}

