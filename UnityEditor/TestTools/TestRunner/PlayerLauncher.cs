namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.TestTools.TestRunner;

    internal class PlayerLauncher : RuntimeTestLauncherBase
    {
        private PlaymodeTestsControllerSettings m_Settings;
        private BuildTarget m_TargetPlatform;
        private string m_TempBuildLocation;

        public PlayerLauncher(PlaymodeTestsControllerSettings settings, BuildTarget? targetPlatform)
        {
            this.m_Settings = settings;
            this.m_TargetPlatform = !targetPlatform.HasValue ? EditorUserBuildSettings.activeBuildTarget : targetPlatform.Value;
        }

        private bool BuildAndRunPlayer(Scene scene, PlayerLauncherBuildOptions buildOptions)
        {
            Debug.Log("Building player with following options:\n" + buildOptions);
            if (buildOptions.BuildPlayerOptions.target == BuildTarget.WebGL)
            {
                Debug.LogError("Test runner is currently not supported on WebGL platform");
                return false;
            }
            string str = BuildPipeline.BuildPlayer(buildOptions.BuildPlayerOptions);
            if (!string.IsNullOrEmpty(str))
            {
                Debug.LogError(str);
            }
            return string.IsNullOrEmpty(str);
        }

        private PlayerLauncherBuildOptions GetBuildOptions(Scene scene)
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.scenes = new string[] { scene.path };
            options.options |= BuildOptions.AutoRunPlayer | BuildOptions.Development;
            options.target = this.m_TargetPlatform;
            if (PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.activeBuildTargetGroup) == ScriptingImplementation.IL2CPP)
            {
                options.options |= BuildOptions.Il2CPP;
            }
            BuildTargetGroup activeBuildTargetGroup = EditorUserBuildSettings.activeBuildTargetGroup;
            this.m_TempBuildLocation = Path.GetFullPath(FileUtil.GetUniqueTempPathInProject());
            string str = "PlayerWithTests." + PostprocessBuildPlayer.GetExtensionForBuildTarget(activeBuildTargetGroup, options.target, options.options);
            string str2 = Path.Combine(Path.Combine(this.m_TempBuildLocation, "PlayerWithTests"), str);
            options.locationPathName = str2;
            return new PlayerLauncherBuildOptions { 
                BuildPlayerOptions = options,
                PlayerDirectory = Path.Combine(this.m_TempBuildLocation, "PlayerWithTests")
            };
        }

        public Scene PrepareScene(string sceneName) => 
            base.CreateBootstrapScene(sceneName, delegate (PlaymodeTestsController runner) {
                runner.AddEventHandlerMonoBehaviour<PlayModeRunnerCallback>();
                runner.settings = this.m_Settings;
            });

        public override void Run()
        {
            using (PlayerLauncherContextSettings settings = new PlayerLauncherContextSettings())
            {
                string sceneName = base.CreateSceneName();
                Scene scene = this.PrepareScene(sceneName);
                PlayerLauncherBuildOptions buildOptions = this.GetBuildOptions(scene);
                base.LoadTestsAndExecutePreBuildSetupMethods(this.m_Settings.filter.BuildNUnitFilter());
                bool flag = this.BuildAndRunPlayer(scene, buildOptions);
                AssetDatabase.DeleteAsset(sceneName);
                if (flag)
                {
                    if (string.IsNullOrEmpty(this.m_Settings.resultFilePath))
                    {
                        this.m_Settings.resultFilePath = Path.Combine(buildOptions.PlayerDirectory, "TestResults.json");
                    }
                }
                else if (this.m_Settings.isBatchModeRun)
                {
                    settings.Dispose();
                    EditorApplication.Exit(3);
                }
            }
        }
    }
}

