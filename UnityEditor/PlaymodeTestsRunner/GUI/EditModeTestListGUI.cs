namespace UnityEditor.PlaymodeTestsRunner.GUI
{
    using System;
    using UnityEditor;
    using UnityEditor.PlaymodeTestsRunner.TestLauncher;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner;
    using UnityEngine.PlaymodeTestsRunner.TestListBuilder;

    [Serializable]
    internal class EditModeTestListGUI : TestListGUI
    {
        public override TestListElement GetTestList()
        {
            TestListUtil util = new TestListUtil(true);
            return util.GetEditmodeTests();
        }

        public override void RenderNoTestsInfo()
        {
            EditorGUILayout.HelpBox("No tests to show", MessageType.Info);
            if (GUILayout.Button("Create EditMode test", new GUILayoutOption[0]))
            {
                EditorApplication.ExecuteMenuItem("Assets/Create/Testing/EditMode Test C# Script (internal)");
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
            EditModeLauncher launcher = new EditModeLauncher(filter);
            launcher.AddEventHandler<WindowResultUpdater>();
            launcher.Run();
        }
    }
}

