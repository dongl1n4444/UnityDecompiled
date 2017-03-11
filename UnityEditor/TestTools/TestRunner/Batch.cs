namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;
    using UnityEngine.TestTools.TestRunner;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal static class Batch
    {
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache0;
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache1;
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache2;
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache3;
        private static string s_DeferedBatchRunPath = "Temp/DeferedBatchRun";

        private static void CheckForCompilationErrors()
        {
            if (EditorUtility.scriptCompilationFailed)
            {
                Debug.LogError("Scripts had compilation errors");
                EditorApplication.Exit(3);
            }
        }

        private static void CompilationWatch()
        {
            if (!EditorApplication.isCompiling)
            {
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new EditorApplication.CallbackFunction(Batch.CompilationWatch);
                }
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, <>f__mg$cache1);
                CheckForCompilationErrors();
            }
        }

        private static void DeferBatchRun(string testPlatform, string resultFilePath, string[] testFilters)
        {
            using (StreamWriter writer = new StreamWriter(s_DeferedBatchRunPath))
            {
                writer.WriteLine(testPlatform);
                writer.WriteLine(resultFilePath);
                writer.WriteLine(string.Join(";", testFilters));
            }
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new EditorApplication.CallbackFunction(Batch.CompilationWatch);
            }
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, <>f__mg$cache0);
        }

        private static void DeferredRunInit()
        {
            string testPlatform = null;
            string resultFilePath = null;
            string[] testFilters = null;
            if (<>f__mg$cache3 == null)
            {
                <>f__mg$cache3 = new EditorApplication.CallbackFunction(Batch.DeferredRunInit);
            }
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, <>f__mg$cache3);
            using (StreamReader reader = new StreamReader(s_DeferedBatchRunPath))
            {
                string str3 = reader.ReadLine();
                if (!string.IsNullOrEmpty(str3))
                {
                    testPlatform = str3;
                }
                str3 = reader.ReadLine();
                if (!string.IsNullOrEmpty(str3))
                {
                    resultFilePath = str3;
                }
                str3 = reader.ReadLine();
                if (!string.IsNullOrEmpty(str3))
                {
                    char[] separator = new char[] { ';' };
                    testFilters = str3.Split(separator);
                }
            }
            File.Delete(s_DeferedBatchRunPath);
            InitiateRun(testPlatform, resultFilePath, testFilters);
        }

        private static void InitiateRun(string testPlatform, string resultFilePath, string[] testFilters)
        {
            CheckForCompilationErrors();
            Debug.Log("Running tests for " + testPlatform);
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

        [DidReloadScripts]
        private static void OnDidReloadScripts()
        {
            if (File.Exists(s_DeferedBatchRunPath))
            {
                if (<>f__mg$cache2 == null)
                {
                    <>f__mg$cache2 = new EditorApplication.CallbackFunction(Batch.DeferredRunInit);
                }
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, <>f__mg$cache2);
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
            if (EditorApplication.isCompiling)
            {
                DeferBatchRun(testPlatform, resultFilePath, testFilters);
            }
            else
            {
                InitiateRun(testPlatform, resultFilePath, testFilters);
            }
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

