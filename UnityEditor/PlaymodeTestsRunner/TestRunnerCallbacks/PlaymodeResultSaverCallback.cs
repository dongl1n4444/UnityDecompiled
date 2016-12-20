namespace UnityEditor.PlaymodeTestsRunner.TestRunnerCallbacks
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.PlaymodeTestsRunner;

    internal class PlaymodeResultSaverCallback : ResultSaverCallback
    {
        public override void RunFinished(List<TestResult> testResults)
        {
            if (string.IsNullOrEmpty(base.resultDirectory))
            {
                base.resultDirectory = PlaymodeTestsController.GetController().settings.resultFilePath;
            }
            if (string.IsNullOrEmpty(base.resultFileName))
            {
                base.resultFileName = "TestResults-PlayMode";
            }
            base.RunFinished(testResults);
        }
    }
}

