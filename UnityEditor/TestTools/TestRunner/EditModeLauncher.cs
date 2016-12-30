namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal class EditModeLauncher : TestLauncherBase
    {
        private EditModeRunner m_Runner;

        public EditModeLauncher(TestRunnerFilter filter)
        {
            this.m_Runner = new EditModeRunner(filter);
        }

        public T AddEventHandler<T>() where T: ScriptableObject, TestRunnerListener => 
            this.m_Runner.AddEventHandler<T>();

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
            int currentGroup = Undo.GetCurrentGroup();
            if (this.OpenNewScene(out setupArray))
            {
                EditModeRunnerCallback callback = this.AddEventHandler<EditModeRunnerCallback>();
                callback.previousSceneSetup = setupArray;
                callback.undoGroup = currentGroup;
                this.m_Runner.Run();
            }
        }
    }
}

