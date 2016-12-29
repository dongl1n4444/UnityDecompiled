namespace UnityEditor.PlaymodeTestsRunner.TestLauncher
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner;
    using UnityEngine.SceneManagement;

    internal class PlaymodeLauncher : RuntimeTestLauncher
    {
        private int m_InitPlaying;
        private Scene m_Scene;
        private PlaymodeTestsControllerSettings m_Settings;

        public PlaymodeLauncher(PlaymodeTestsControllerSettings settings)
        {
            this.m_Settings = settings;
        }

        public override void Run()
        {
            ConsoleWindow.SetConsoleErrorPause(false);
            Application.runInBackground = true;
            string sceneName = base.CreateSceneName();
            this.m_Scene = base.CreateBootstrapScene(sceneName, delegate (PlaymodeTestsController runner) {
                runner.AddEventHandlerMonoBehaviour<ResultsRenderer>();
                runner.AddEventHandlerScriptableObject<WindowResultUpdater>();
                runner.AddEventHandlerScriptableObject<TestRunnerCallback>();
                if (this.m_Settings.isBatchModeRun)
                {
                    runner.AddEventHandlerScriptableObject<PlaymodeResultSaverCallback>();
                    runner.AddEventHandlerScriptableObject<BatchRunCallback>();
                }
                runner.settings = this.m_Settings;
            });
            if (this.m_Settings.sceneBased)
            {
                EditorBuildSettings.scenes = new List<EditorBuildSettingsScene> { new EditorBuildSettingsScene(sceneName, true) }.ToArray();
            }
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateCallback));
            this.m_InitPlaying = 3;
        }

        public void UpdateCallback()
        {
            if (this.m_InitPlaying-- > 0)
            {
                if (this.m_InitPlaying == 1)
                {
                    base.ExecuteSceneSetupMethods(this.m_Settings.filter);
                }
                if (this.m_InitPlaying == 0)
                {
                    if (this.m_Scene.IsValid())
                    {
                        SceneManager.SetActiveScene(this.m_Scene);
                    }
                    EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateCallback));
                    EditorApplication.isPlaying = true;
                }
            }
        }

        [InitializeOnLoad]
        public class BackgroundWatcher
        {
            [CompilerGenerated]
            private static EditorApplication.CallbackFunction <>f__mg$cache0;

            static BackgroundWatcher()
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new EditorApplication.CallbackFunction(PlaymodeLauncher.BackgroundWatcher.PlaymodeStateChanged);
                }
                EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.playmodeStateChanged, <>f__mg$cache0);
            }

            private static void PlaymodeStateChanged()
            {
                if (PlaymodeTestsController.IsControllerOnScene())
                {
                    PlaymodeTestsController controller = PlaymodeTestsController.GetController();
                    if ((controller != null) && (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode))
                    {
                        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
                        if (!string.IsNullOrEmpty(controller.settings.originalScene))
                        {
                            EditorSceneManager.OpenScene(controller.settings.originalScene);
                            AssetDatabase.DeleteAsset(controller.settings.bootstrapScene);
                        }
                    }
                }
            }
        }
    }
}

