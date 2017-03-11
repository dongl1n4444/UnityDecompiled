namespace UnityEditor.TestTools.TestRunner.GUI
{
    using NUnit.Framework.Interfaces;
    using System;
    using UnityEditor;
    using UnityEditor.TestTools.TestRunner;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.TestTools;
    using UnityEngine.TestTools.TestRunner;
    using UnityEngine.TestTools.TestRunner.GUI;
    using UnityEngine.TestTools.Utils;

    [Serializable]
    internal class PlayModeTestListGUI : TestListGUI
    {
        public override ITest GetTestListNUnit()
        {
            TestAssemblyProvider provider = new TestAssemblyProvider();
            return provider.GetTestsWithNUnit(TestPlatform.PlayMode);
        }

        public override void PrintHeadPanel()
        {
            if (TestRunnerWindow.IsPlaymodeTestRunnerEnabled())
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(false) };
                EditorGUILayout.BeginHorizontal(options);
                base.PrintHeadPanel();
                if (GUILayout.Button("Run all in player (" + EditorUserBuildSettings.activeBuildTarget + ")", EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    this.RunTestsInPlayer(null);
                }
                EditorGUILayout.EndHorizontal();
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandHeight(false) };
                EditorGUILayout.BeginHorizontal(optionArray2);
                if (base.m_Window.m_IsSceneBased)
                {
                    SceneAsset asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
                    GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(100f) };
                    EditorGUILayout.LabelField("Showing tests for:", optionArray3);
                    EditorGUILayout.ObjectField(asset, typeof(SceneAsset), false, new GUILayoutOption[0]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        public override void RenderNoTestsInfo()
        {
            EditorGUILayout.HelpBox("No tests to show", MessageType.Info);
            if (GUILayout.Button("Create Playmode test with methods", new GUILayoutOption[0]))
            {
                EditorApplication.ExecuteMenuItem("Assets/Create/Testing/PlayMode Test With Methods C# Script (internal)");
            }
        }

        public override void RenderTestList()
        {
            if (TestRunnerWindow.IsPlaymodeTestRunnerEnabled())
            {
                base.RenderTestList();
            }
            else
            {
                string message = "Enabling PlayMode will include additional assemblies in the build that may increase the size of the build and compilation time.\n\nYou can disable PlayMode tests in the context menu of this window.\n\nThis action requires restarting the editor.";
                EditorGUILayout.HelpBox(message, MessageType.Warning);
                if (GUILayout.Button("Enable playmode tests", new GUILayoutOption[0]) && EditorUtility.DisplayDialog("Enable PlayMode Tests", message, "Enable", "Cancel"))
                {
                    TestRunnerWindow.EnablePlaymodeTestRunnerEnabled(true);
                    EditorUtility.DisplayDialog("Enable PlayMode Tests", "You need to restart the editor now", "Ok");
                }
            }
        }

        protected override void RunTests(TestRunnerFilter filter)
        {
            foreach (TestRunnerResult result in base.newResultList)
            {
                if (filter.Matches(result))
                {
                    result.resultStatus = TestRunnerResult.ResultStatus.NotRun;
                }
            }
            PlaymodeTestsControllerSettings settings = PlaymodeTestsControllerSettings.CreateRunnerSettings(filter);
            settings.bootstrapScene = SceneManager.GetActiveScene().path;
            settings.originalScene = SceneManager.GetActiveScene().path;
            new PlaymodeLauncher(settings).Run();
        }

        protected void RunTestsInPlayer(TestRunnerFilter filter)
        {
            new PlayerLauncher(PlaymodeTestsControllerSettings.CreateRunnerSettings(filter), null).Run();
            GUIUtility.ExitGUI();
        }
    }
}

