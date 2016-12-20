namespace UnityEditor.PlaymodeTestsRunner.TestLauncher
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.PlaymodeTestsRunner;
    using UnityEngine.SceneManagement;

    internal class PlayerLauncher : RuntimeTestLauncher
    {
        private PlaymodeTestsControllerSettings m_Settings;

        public PlayerLauncher(PlaymodeTestsControllerSettings settings)
        {
            this.m_Settings = settings;
        }

        private void BuildAndRunPlayer(Scene scene)
        {
            string str = BuildPipeline.BuildPlayer(GetBuildOptions(scene));
            if (!string.IsNullOrEmpty(str))
            {
                Debug.LogError(str);
            }
        }

        private static BuildPlayerOptions GetBuildOptions(Scene scene)
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.scenes = new string[] { scene.path };
            options.options |= BuildOptions.AutoRunPlayer | BuildOptions.Development;
            options.target = EditorUserBuildSettings.activeBuildTarget;
            string str = Path.Combine(Path.Combine(FileUtil.GetUniqueTempPathInProject(), "TestRun"), "test." + PostprocessBuildPlayer.GetExtensionForBuildTarget(options.target, options.options));
            options.locationPathName = str;
            return options;
        }

        public Scene PrepareScene(string sceneName)
        {
            return base.CreateBootstrapScene(sceneName, delegate (PlaymodeTestsController runner) {
                PlaymodeTestsControllerExtensions.AddEventHandlerMonoBehaviour<ResultsRenderer>(runner);
                runner.settings = this.m_Settings;
            });
        }

        public override void Run()
        {
            using (new PlayerLauncherContextSettings())
            {
                string sceneName = base.CreateSceneName();
                Scene scene = this.PrepareScene(sceneName);
                this.BuildAndRunPlayer(scene);
                AssetDatabase.DeleteAsset(sceneName);
            }
        }
    }
}

