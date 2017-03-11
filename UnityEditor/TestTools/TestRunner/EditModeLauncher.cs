namespace UnityEditor.TestTools.TestRunner
{
    using NUnit.Framework.Interfaces;
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;
    using UnityEngine.TestTools;
    using UnityEngine.TestTools.NUnitExtensions;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal class EditModeLauncher : TestLauncherBase
    {
        private EditModeRunner m_EditModeRunner;
        private ITestFilter m_Filter;
        private UnityTestAssemblyRunner nUnitTestAssemblyRunner;

        public EditModeLauncher(TestRunnerFilter filter)
        {
            this.m_Filter = filter.BuildNUnitFilter();
            this.nUnitTestAssemblyRunner = new UnityTestAssemblyRunner(UnityTestAssemblyBuilder.GetNUnitTestBuilder(TestPlatform.EditMode));
            this.nUnitTestAssemblyRunner.Load(UnityTestAssemblyBuilder.GetUserAssemblies(true), UnityTestAssemblyBuilder.GetNUnitTestBuilderSettings(TestPlatform.EditMode));
            this.m_EditModeRunner = new EditModeRunner(this.nUnitTestAssemblyRunner);
        }

        public T AddEventHandler<T>() where T: ScriptableObject, TestRunnerListener => 
            this.m_EditModeRunner.AddEventHandler<T>();

        private bool OpenNewScene(out SceneSetup[] previousSceneSetup)
        {
            previousSceneSetup = null;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return false;
            }
            int sceneCount = SceneManager.sceneCount;
            bool flag3 = string.IsNullOrEmpty(SceneManager.GetSceneAt(0).path);
            if ((sceneCount == 1) && flag3)
            {
                EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                return true;
            }
            RemoveUntitledScenes();
            ReloadUnsavedDirtyScene();
            previousSceneSetup = EditorSceneManager.GetSceneManagerSetup();
            SceneManager.SetActiveScene(EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive));
            return true;
        }

        private static void ReloadUnsavedDirtyScene()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene sceneAt = SceneManager.GetSceneAt(i);
                bool flag = string.IsNullOrEmpty(sceneAt.path);
                bool isDirty = sceneAt.isDirty;
                if (flag && isDirty)
                {
                    EditorSceneManager.ReloadScene(sceneAt);
                }
            }
        }

        private static void RemoveUntitledScenes()
        {
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                Scene sceneAt = SceneManager.GetSceneAt(i);
                if (string.IsNullOrEmpty(sceneAt.path))
                {
                    EditorSceneManager.CloseScene(sceneAt, true);
                }
            }
        }

        public override void Run()
        {
            SceneSetup[] setupArray;
            base.ExecutePreBuildSetupMethods(this.nUnitTestAssemblyRunner.LoadedTest, this.m_Filter);
            int currentGroup = Undo.GetCurrentGroup();
            if (this.OpenNewScene(out setupArray))
            {
                EditModeRunnerCallback callback = this.AddEventHandler<EditModeRunnerCallback>();
                callback.previousSceneSetup = setupArray;
                callback.undoGroup = currentGroup;
                this.m_EditModeRunner.Run(this.m_Filter);
            }
        }
    }
}

