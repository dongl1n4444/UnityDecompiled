namespace UnityEditor.Android
{
    using System;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Android.PostProcessor.Tasks;

    internal class PostProcessAndroidPlayer
    {
        private static PostProcessorContext _context;

        internal static void PostProcess(BuildTarget target, string stagingAreaData, string stagingArea, string playerPackage, string installPath, string companyName, string productName, BuildOptions options, RuntimeClassRegistry usedClassRegistry)
        {
            if (target != BuildTarget.Android)
            {
                CancelPostProcess.AbortBuild("Build failure", "Internal error: Target platform mismatch");
            }
            _context.Set<BuildTarget>("BuildTarget", target);
            _context.Set<string>("StagingAreaData", stagingAreaData);
            _context.Set<string>("StagingArea", stagingArea);
            _context.Set<string>("InstallPath", installPath);
            _context.Set<RuntimeClassRegistry>("UsedClassRegistry", usedClassRegistry);
            bool flag = (options & BuildOptions.AutoRunPlayer) != BuildOptions.CompressTextures;
            _context.Set<bool>("AutoRunPlayer", flag);
            bool flag2 = (options & BuildOptions.AcceptExternalModificationsToPlayer) != BuildOptions.CompressTextures;
            _context.Set<bool>("ExportAndroidProject", flag2);
            _context.Set<string>("AndroidPluginsPath", "Assets/Plugins/Android");
            switch (EditorUserBuildSettings.androidBuildSystem)
            {
                case AndroidBuildSystem.Internal:
                    _context.Set<int>("ProjectType", 0);
                    break;

                case AndroidBuildSystem.Gradle:
                    _context.Set<int>("ProjectType", 1);
                    break;

                case AndroidBuildSystem.ADT:
                    _context.Set<int>("ProjectType", 2);
                    break;

                default:
                    _context.Set<int>("ProjectType", !flag2 ? 0 : 2);
                    break;
            }
            bool flag3 = _context.Get<int>("ProjectType") == 1;
            PostProcessRunner runner = new PostProcessRunner();
            runner.AddNextTask(new Initializer());
            runner.AddNextTask(new PrepareUnityResources());
            runner.AddNextTask(new SplitLargeFiles());
            runner.AddNextTask(new NonstreamingObbAssets());
            runner.AddNextTask(new PrepareUnityPackage());
            runner.AddNextTask(new PrepareUserResources());
            runner.AddNextTask(new PrepareAPKResources());
            runner.AddNextTask(new NativePlugins());
            if (!flag3)
            {
                runner.AddNextTask(new ProcessAAR());
            }
            runner.AddNextTask(new AddAndroidLibraries());
            runner.AddNextTask(new GenerateManifest());
            if (!flag3)
            {
                runner.AddNextTask(new BuildResources());
                if (!flag2)
                {
                    runner.AddNextTask(new CheckLibrariesConflict());
                    runner.AddNextTask(new RunDex());
                }
            }
            runner.AddNextTask(new RunIl2Cpp());
            runner.AddNextTask(new StreamingAssets());
            runner.AddNextTask(new FastZip());
            runner.AddNextTask(new AAPTPackage());
            if (flag2)
            {
                runner.AddNextTask(new ExportProject());
            }
            else
            {
                if (flag3)
                {
                    runner.AddNextTask(new BuildGradleProject());
                }
                else
                {
                    runner.AddNextTask(new BuildAPK());
                }
                runner.AddNextTask(new PublishPackage());
            }
            runner.RunAllTasks(_context);
        }

        internal static string PrepareForBuild(BuildOptions options, BuildTarget target)
        {
            if (target != BuildTarget.Android)
            {
                CancelPostProcess.AbortBuild("Build failure", "Internal error: Target platform mismatch");
            }
            if ((options & BuildOptions.BuildAdditionalStreamedScenes) == BuildOptions.CompressTextures)
            {
                _context = new PostProcessorContext();
                SetupContextForPreBuild(_context, options, target);
                PostProcessRunner runner = new PostProcessRunner();
                runner.AddNextTask(new CheckPrerequisites());
                runner.AddNextTask(new CheckAndroidSdk());
                runner.AddNextTask(new CheckDevice());
                runner.RunAllTasks(_context);
            }
            return "";
        }

        private static void SetupContextForPreBuild(PostProcessorContext context, BuildOptions options, BuildTarget target)
        {
            context.Set<BuildTarget>("BuildTarget", target);
            bool flag = (options & BuildOptions.Development) != BuildOptions.CompressTextures;
            context.Set<bool>("DevelopmentPlayer", flag);
            bool flag2 = (options & BuildOptions.AcceptExternalModificationsToPlayer) != BuildOptions.CompressTextures;
            context.Set<bool>("ExportAndroidProject", flag2);
            bool flag3 = (options & BuildOptions.AutoRunPlayer) != BuildOptions.CompressTextures;
            context.Set<bool>("AutoRunPlayer", flag3);
            AndroidTargetDevice targetDevice = PlayerSettings.Android.targetDevice;
            context.Set<AndroidTargetDevice>("TargetDevice", targetDevice);
            string playbackEngineDirectory = BuildPipeline.GetPlaybackEngineDirectory(target, options);
            context.Set<string>("PlayerPackage", playbackEngineDirectory);
            ScriptingImplementation scriptingBackend = PlayerSettings.GetScriptingBackend(BuildPipeline.GetBuildTargetGroup(target));
            context.Set<ScriptingImplementation>("ScriptingBackend", scriptingBackend);
        }
    }
}

