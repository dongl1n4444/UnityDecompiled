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
            TestAssemblyProvider provider = new TestAssemblyProvider();
            return provider.GetTestsWithNUnit(TestPlatform.EditMode);
        }

        public override void RenderNoTestsInfo()
        {
            EditorGUILayout.HelpBox("No tests to show", MessageType.Info);
            if (GUILayout.Button("Create EditMode test", new GUILayoutOption[0]))
            {
                EditorApplication.ExecuteMenuItem("Assets/Create/Testing/EditMode Test C# Script");
            }
        }

        protected override void RunTests(TestRunnerFilter filter)
        {
            if (!EditModeRunner.RunningTests)
            {
                foreach (TestRunnerResult result in base.newResultList)
                {
                    if (filter.Matches(result))
                    {
                        result.resultStatus = TestRunnerResult.ResultStatus.NotRun;
                    }
                }
                EditModeLauncher launcher = new EditModeLauncher(filter);
                launcher.AddEventHandler<WindowResultUpdater>();
                launcher.Run();
            }
        }
    }
}

