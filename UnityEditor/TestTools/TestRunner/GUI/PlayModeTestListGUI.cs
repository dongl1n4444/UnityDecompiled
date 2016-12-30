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
            TestListUtil util = new TestListUtil();
            return util.GetTestsWithNUnit(TestPlatform.PlayMode);
        }

        public override void PrintHeadPanel()
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

        public override void RenderNoTestsInfo()
        {
            EditorGUILayout.HelpBox("No tests to show", MessageType.Info);
            if (GUILayout.Button("Create Playmode test with methods", new GUILayoutOption[0]))
            {
                EditorApplication.ExecuteMenuItem("Assets/Create/Testing/PlayMode Test With Methods C# Script (internal)");
            }
        }

        protected override void RunTests(TestRunnerFilter filter)
        {
            foreach (TestRunnerResult result in base.newResultList)
            {
                if (filter.Matches(result))
                {
                    result.resultType = TestRunnerResult.ResultType.NotRun;
                }
            }
            PlaymodeTestsControllerSettings settings = PlaymodeTestsControllerSettings.CreateRunnerSettings(filter);
            settings.bootstrapScene = SceneManager.GetActiveScene().path;
            settings.originalScene = SceneManager.GetActiveScene().path;
            new PlaymodeLauncher(settings).Run();
            GUIUtility.ExitGUI();
        }

        protected void RunTestsInPlayer(TestRunnerFilter filter)
        {
            new PlayerLauncher(PlaymodeTestsControllerSettings.CreateRunnerSettings(filter), null).Run();
            GUIUtility.ExitGUI();
        }
    }
}

