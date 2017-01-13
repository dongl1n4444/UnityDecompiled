namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.PlaymodeTestsRunner.TestLauncher;
    using UnityEditor.PlaymodeTestsRunner.TestRunnerCallbacks;
    using UnityEngine.PlaymodeTestsRunner;

    internal static class Batch
    {
        private static void InitiateRun(string testPlatform, string resultFilePath, string[] testFilters)
        {
            <InitiateRun>c__AnonStorey0 storey = new <InitiateRun>c__AnonStorey0 {
                resultFilePath = resultFilePath,
                filter = new TestRunnerFilter()
            };
            storey.filter.names = testFilters;
            if (testPlatform.ToLower() == "editmode")
            {
                RunEditModeTest(storey.filter, storey.resultFilePath, null);
            }
            else if (testPlatform.ToLower() == "playmode")
            {
                RunInEditor(storey.resultFilePath, storey.filter, null, null, false);
            }
            else
            {
                RunEditModeTest(storey.filter, storey.resultFilePath, new Action(storey.<>m__0));
            }
        }

        private static void RunEditModeTest(TestRunnerFilter filter, string resultFilePath, Action afterRun)
        {
            EditModeLauncher launcher = new EditModeLauncher(filter);
            ResultSaverCallback callback = launcher.AddEventHandler<ResultSaverCallback>();
            callback.resultDirectory = resultFilePath;
            callback.resultFileName = "TestResults-EditMode";
            if (afterRun == null)
            {
                launcher.AddEventHandler<BatchRunCallback>();
            }
            else
            {
                launcher.AddEventHandler<AfterRunFinishedCallback>().afterRun = afterRun;
            }
            launcher.Run();
        }

        private static void RunInEditor(string resultFilePath, TestRunnerFilter filter, string[] testScenes, string[] otherBuildScenes, bool sceneBased)
        {
            PlaymodeTestsControllerSettings settings = PlaymodeTestsControllerSettings.CreateRunnerSettings(filter);
            settings.resultFilePath = resultFilePath;
            settings.isBatchModeRun = true;
            new PlaymodeLauncher(settings).Run();
        }

        private static void RunPlaymodeTests(string resultFilePath, string verboseLog, string[] testFilters, string[] testScenes)
        {
            TestRunnerFilter filter = new TestRunnerFilter {
                names = testFilters
            };
            RunInEditor(resultFilePath, filter, testScenes, null, false);
        }

        internal static void RunPlaymodeTests(string resultFilePath, string verboseLog, TestRunnerFilter filter, string[] testScenes, bool sceneBased)
        {
            RunInEditor(resultFilePath, filter, testScenes, new string[0], sceneBased);
        }

        private static void RunTests(string testPlatform, string resultFilePath, string[] testFilters)
        {
            InitiateRun(testPlatform, resultFilePath, testFilters);
        }

        [CompilerGenerated]
        private sealed class <InitiateRun>c__AnonStorey0
        {
            internal TestRunnerFilter filter;
            internal string resultFilePath;

            internal void <>m__0()
            {
                Batch.RunInEditor(this.resultFilePath, this.filter, null, null, false);
            }
        }
    }
}

