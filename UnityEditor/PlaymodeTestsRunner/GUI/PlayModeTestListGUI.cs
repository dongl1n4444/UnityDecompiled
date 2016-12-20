namespace UnityEditor.PlaymodeTestsRunner.GUI
{
    using System;
    using UnityEditor;
    using UnityEditor.PlaymodeTestsRunner.TestLauncher;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner;
    using UnityEngine.PlaymodeTestsRunner.TestListBuilder;
    using UnityEngine.SceneManagement;

    [Serializable]
    internal class PlayModeTestListGUI : TestListGUI
    {
        public override TestListElement GetTestList()
        {
            TestListUtil util = new TestListUtil(false);
            return util.GetPlaymodeTests();
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
            else
            {
                EditorGUILayout.LabelField("Code-based tests", new GUILayoutOption[0]);
            }
            EditorGUILayout.EndHorizontal();
        }

        public override void RenderNoTestsInfo()
        {
            EditorGUILayout.HelpBox("No tests to show", MessageType.Info);
            if (GUILayout.Button("Create Playmode test with methods", new GUILayoutOption[0]))
            {
                EditorApplication.ExecuteMenuItem("Assets/Create/Playmode Test With Methods C# Script");
            }
            if (GUILayout.Button("Create Playmode test as MonoBehaviour", new GUILayoutOption[0]))
            {
                EditorApplication.ExecuteMenuItem("Assets/Create/Playmode Test C# Script");
            }
        }

        protected override void RunTests(TestRunnerFilter filter)
        {
            foreach (TestResult result in base.newResultList)
            {
                if (filter.Matches(result))
                {
                    result.resultType = TestResult.ResultType.NotRun;
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
            new PlayerLauncher(PlaymodeTestsControllerSettings.CreateRunnerSettings(filter)).Run();
            GUIUtility.ExitGUI();
        }
    }
}

