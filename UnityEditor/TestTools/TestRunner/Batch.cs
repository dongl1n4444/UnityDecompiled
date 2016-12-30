namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.TestTools.TestRunner;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal static class Batch
    {
        private static void InitiateRun(string testPlatform, string resultFilePath, string[] testFilters)
        {
            TestRunnerFilter filter = new TestRunnerFilter {
                names = testFilters
            };
            if (string.IsNullOrEmpty(testPlatform))
            {
                testPlatform = "editmode";
            }
            testPlatform = testPlatform.ToLower();
            if (testPlatform == "editmode")
            {
                RunEditModeTest(filter, resultFilePath);
            }
            else if (testPlatform == "playmode")
            {
                RunPlaymodeTests(filter, resultFilePath, null, null, false);
            }
            else
            {
                try
                {
                    BuildTarget targetPlatform = (BuildTarget) Enum.Parse(typeof(BuildTarget), testPlatform, true);
                    RunInPlayer(filter, resultFilePath, targetPlatform);
                }
                catch (ArgumentException)
                {
                    Debug.Log("Test platform not found (" + testPlatform + ")");
                    EditorApplication.Exit(4);
                }
            }
        }

        private static void RunEditModeTest(TestRunnerFilter filter, string resultFilePath)
        {
            EditModeLauncher launcher = new EditModeLauncher(filter);
            launcher.AddEventHandler<ResultSaverCallback>().resultFilePath = resultFilePath;
            launcher.AddEventHandler<BatchRunCallback>();
            launcher.Run();
        }

        private static void RunInPlayer(TestRunnerFilter filter, string resultFilePath, BuildTarget targetPlatform)
        {
            PlaymodeTestsControllerSettings settings = PlaymodeTestsControllerSettings.CreateRunnerSettings(filter);
            settings.resultFilePath = resultFilePath;
            settings.isBatchModeRun = true;
            new PlayerLauncher(settings, new BuildTarget?(targetPlatform)).Run();
        }

        private static void RunPlaymodeTests(string resultFilePath, string verboseLog, string[] testFilters, string[] testScenes)
        {
            TestRunnerFilter filter = new TestRunnerFilter {
                names = testFilters
            };
            RunPlaymodeTests(filter, resultFilePath, testScenes, null, false);
        }

        internal static void RunPlaymodeTests(string resultFilePath, string verboseLog, TestRunnerFilter filter, string[] testScenes, bool sceneBased)
        {
            RunPlaymodeTests(filter, resultFilePath, testScenes, new string[0], sceneBased);
        }

        private static void RunPlaymodeTests(TestRunnerFilter filter, string resultFilePath, string[] testScenes, string[] otherBuildScenes, bool sceneBased)
        {
            PlaymodeTestsControllerSettings settings = PlaymodeTestsControllerSettings.CreateRunnerSettings(filter);
            settings.resultFilePath = resultFilePath;
            settings.isBatchModeRun = true;
            new PlaymodeLauncher(settings).Run();
        }

        private static void RunTests(string testPlatform, string resultFilePath, string[] testFilters)
        {
            InitiateRun(testPlatform, resultFilePath, testFilters);
        }

        internal enum ReturnCodes
        {
            Failed = 2,
            Ok = 0,
            PlatformNotFoundReturnCode = 4,
            RunError = 3
        }
    }
}

