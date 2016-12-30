namespace UnityEditor.TestTools.TestRunner
{
    using NUnit.Framework.Interfaces;
    using System;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.TestTools.TestRunner;

    internal class EditModeRunnerCallback : ScriptableObject, TestRunnerListener
    {
        private EditModeLauncherContextSettings m_Settings;
        public SceneSetup[] previousSceneSetup;
        public int undoGroup;

        private static void PerformUndo(int undoGroup)
        {
            EditorUtility.DisplayProgressBar("Undo", "Reverting changes to the scene", 0f);
            DateTime now = DateTime.Now;
            Undo.RevertAllDownToGroup(undoGroup);
            TimeSpan span = (TimeSpan) (DateTime.Now - now);
            if (span.TotalSeconds > 1.0)
            {
                TimeSpan span2 = (TimeSpan) (DateTime.Now - now);
                Debug.LogWarning("Undo after editor test run took " + span2.Seconds + " seconds.");
            }
            EditorUtility.ClearProgressBar();
        }

        public void RunFinished(ITestResult result)
        {
            if (this.previousSceneSetup != null)
            {
                EditorSceneManager.RestoreSceneManagerSetup(this.previousSceneSetup);
            }
            PerformUndo(this.undoGroup);
            EditorUtility.ClearProgressBar();
            this.m_Settings.Dispose();
        }

        public void RunStarted(ITest testsToRun)
        {
            this.m_Settings = new EditModeLauncherContextSettings();
        }

        public void TestFinished(ITestResult result)
        {
        }

        public void TestStarted(ITest test)
        {
            EditorUtility.DisplayProgressBar("Test Runner", "Running tests...", 1f);
        }
    }
}

