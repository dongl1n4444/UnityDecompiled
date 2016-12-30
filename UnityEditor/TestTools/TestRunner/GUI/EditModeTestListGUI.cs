namespace UnityEditor.TestTools.TestRunner.GUI
{
    using NUnit.Framework.Interfaces;
    using System;
    using UnityEditor;
    using UnityEditor.TestTools.TestRunner;
    using UnityEngine;
    using UnityEngine.TestTools;
    using UnityEngine.TestTools.TestRunner.GUI;
    using UnityEngine.TestTools.Utils;

    [Serializable]
    internal class EditModeTestListGUI : TestListGUI
    {
        public override ITest GetTestListNUnit()
        {
            TestListUtil util = new TestListUtil();
            return util.GetTestsWithNUnit(TestPlatform.EditMode);
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
            foreach (TestRunnerResult result in base.newResultList)
            {
                if (filter.Matches(result))
                {
                    result.resultType = TestRunnerResult.ResultType.NotRun;
                }
            }
            EditModeLauncher launcher = new EditModeLauncher(filter);
            launcher.AddEventHandler<WindowResultUpdater>();
            launcher.Run();
            GUIUtility.ExitGUI();
        }
    }
}

