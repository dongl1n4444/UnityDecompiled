namespace UnityEditor.Android
{
    using System;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Android.PostProcessor.Tasks;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class PostProcessAndroidPlayer
    {
        private static PostProcessorContext _context;

        private static int GetProjectType()
        {
            switch (EditorUserBuildSettings.androidBuildSystem)
            {
                case AndroidBuildSystem.Internal:
                    return (!_context.Get<bool>("ExportAndroidProject") ? 0 : 2);

                case AndroidBuildSystem.Gradle:
                    return 1;

                case AndroidBuildSystem.ADT:
                    return 2;

                case AndroidBuildSystem.VisualStudio:
                    return 3;
            }
            throw new UnityException($"Invalid Android build system {EditorUserBuildSettings.androidBuildSystem}. Update build settings.");
        }

        internal static void Launch(BuildTarget target, string installPath)
        {
            if (target != BuildTarget.Android)
            {
                CancelPostProcess.AbortBuild("Build failure", "Internal error: Target platform mismatch", null);
            }
            PostProcessRunner runner = new PostProcessRunner();
            _context.Set<string>("InstallPath", installPath);
            runner.AddNextTask(new PublishPackage());
            runner.RunAllTasks(_context);
        }

        internal static void PostProcess(BuildTarget target, string stagingAreaData, string stagingArea, string playerPackage, string installPath, string companyName, string productName, BuildOptions options, RuntimeClassRegistry usedClassRegistry)
        {
            if (target != BuildTarget.Android)
            {
                CancelPostProcess.AbortBuild("Build failure", "Internal error: Target platform mismatch", null);
            }
            _context.Set<string>("StagingAreaData", stagingAreaData);
            _context.Set<string>("StagingArea", stagingArea);
            _context.Set<RuntimeClassRegistry>("UsedClassRegistry", usedClassRegistry);
            bool flag = _context.Get<bool>("ExportAndroidProject");
            bool flag2 = _context.Get<int>("ProjectType") == 1;
            if (_context.Get<bool>("SourceBuild"))
            {
                string[] components = new string[] { playerPackage, "SourceBuild", productName };
                installPath = Paths.Combine(components);
            }
            _context.Set<string>("InstallPath", installPath);
            PostProcessRunner runner = new PostProcessRunner();
            runner.AddNextTask(new Initializer());
            runner.AddNextTask(new PrepareUnityResources());
            runner.AddNextTask(new SplitLargeFiles());
            runner.AddNextTask(new NonstreamingObbAssets());
            runner.AddNextTask(new PrepareUnityPackage());
            runner.AddNextTask(new PrepareUserResources());
            runner.AddNextTask(new PrepareAPKResources());
            runner.AddNextTask(new NativePlugins());
            if (!flag2)
            {
                runner.AddNextTask(new ProcessAAR());
            }
            runner.AddNextTask(new AddAndroidLibraries());
            runner.AddNextTask(new GenerateManifest());
            runner.AddNextTask(new BuildResources());
            if (!flag2 && !flag)
            {
                runner.AddNextTask(new CheckLibrariesConflict());
                runner.AddNextTask(new RunDex());
            }
            runner.AddNextTask(new RunIl2Cpp());
            runner.AddNextTask(new StreamingAssets());
            runner.AddNextTask(new FastZip());
            runner.AddNextTask(new AAPTPackage());
            if (flag)
            {
                runner.AddNextTask(new ExportProject());
            }
            else
            {
                if (flag2)
                {
                    runner.AddNextTask(new BuildGradleProject());
                }
                else
                {
                    runner.AddNextTask(new BuildAPK());
                }
                runner.AddNextTask(new MoveFinalPackage());
            }
            runner.RunAllTasks(_context);
        }

        internal static string PrepareForBuild(BuildOptions options, BuildTarget target)
        {
            if (target != BuildTarget.Android)
            {
                CancelPostProcess.AbortBuild("Build failure", "Internal error: Target platform mismatch", null);
            }
            if ((options & BuildOptions.BuildAdditionalStreamedScenes) == BuildOptions.CompressTextures)
            {
                _context = new PostProcessorContext();
                SetupContextForPreBuild(options, target);
                PostProcessRunner runner = new PostProcessRunner();
                runner.AddNextTask(new CheckPrerequisites());
                runner.AddNextTask(new CheckAndroidSdk());
                runner.AddNextTask(new CheckDevice());
                runner.RunAllTasks(_context);
            }
            return "";
        }

        private static void SetupContextForPreBuild(BuildOptions options, BuildTarget target)
        {
            _context.Set<BuildTarget>("BuildTarget", target);
            bool flag = (options & BuildOptions.Development) != BuildOptions.CompressTextures;
            _context.Set<bool>("DevelopmentPlayer", flag);
            bool flag2 = (options & BuildOptions.AcceptExternalModificationsToPlayer) != BuildOptions.CompressTextures;
            _context.Set<bool>("ExportAndroidProject", flag2);
            bool flag3 = (options & BuildOptions.AutoRunPlayer) != BuildOptions.CompressTextures;
            _context.Set<bool>("AutoRunPlayer", flag3);
            AndroidTargetDevice targetDevice = PlayerSettings.Android.targetDevice;
            _context.Set<AndroidTargetDevice>("TargetDevice", targetDevice);
            string playbackEngineDirectory = BuildPipeline.GetPlaybackEngineDirectory(target, options);
            _context.Set<string>("PlayerPackage", playbackEngineDirectory);
            ScriptingImplementation scriptingBackend = PlayerSettings.GetScriptingBackend(BuildPipeline.GetBuildTargetGroup(target));
            _context.Set<ScriptingImplementation>("ScriptingBackend", scriptingBackend);
            _context.Set<string>("AndroidPluginsPath", "Assets/Plugins/Android");
            _context.Set<int>("ProjectType", GetProjectType());
            _context.Set<int>("Minification", ((int) EditorUserBuildSettings.androidReleaseMinification) | (((int) EditorUserBuildSettings.androidDebugMinification) << 2));
            bool flag4 = (options & BuildOptions.InstallInBuildFolder) != BuildOptions.CompressTextures;
            _context.Set<bool>("SourceBuild", flag4);
            string str2 = !Unsupported.IsDeveloperBuild() ? (!flag ? "Release" : "Development") : EditorUserBuildSettings.androidBuildType.ToString();
            _context.Set<string>("Variation", str2);
        }
    }
}

