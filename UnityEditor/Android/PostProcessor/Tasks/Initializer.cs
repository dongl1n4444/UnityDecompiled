namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor;
    using UnityEngine;

    internal class Initializer : IPostProcessorTask
    {
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Setting up target contents directory");
            }
            string str = context.Get<string>("StagingArea");
            BuildTarget target = context.Get<BuildTarget>("BuildTarget");
            string dir = Path.Combine(str, "android-libraries");
            FileUtil.CreateOrCleanDirectory(dir);
            context.Set<string>("TargetLibrariesFolder", dir);
            Directory.CreateDirectory(Path.Combine(str, "bin"));
            bool useAPKExpansionFiles = PlayerSettings.Android.useAPKExpansionFiles;
            context.Set<bool>("UseObb", useAPKExpansionFiles);
            context.Set<int>("GearVRMinSdkVersion", 0x13);
            string path = Path.Combine(BuildPipeline.GetBuildToolsDirectory(target), "fastzip");
            if (!File.Exists(path))
            {
                path = Path.Combine(BuildPipeline.GetBuildToolsDirectory(target), "fastzip.exe");
            }
            context.Set<string>("FastzipExe", path);
            bool flag2 = File.Exists(path);
            context.Set<bool>("UseFastzip", flag2);
            if (PlayerSettings.Android.androidTVCompatibility && !this.IsOrientationAndroidTVCompatible())
            {
                Debug.LogWarning("The orientation specified is not compatible with Android TV.\nPlease select Landscape or AutoRotation with Landscape enabled orientation, or disable Android TV compatibility in Player Settings.");
            }
        }

        private bool IsOrientationAndroidTVCompatible()
        {
            UIOrientation defaultInterfaceOrientation = PlayerSettings.defaultInterfaceOrientation;
            switch (defaultInterfaceOrientation)
            {
                case UIOrientation.LandscapeLeft:
                case UIOrientation.LandscapeRight:
                    return true;
            }
            bool flag2 = PlayerSettings.allowedAutorotateToLandscapeLeft || PlayerSettings.allowedAutorotateToLandscapeRight;
            return ((defaultInterfaceOrientation == UIOrientation.AutoRotation) && flag2);
        }

        public string Name =>
            "Creating staging area";
    }
}

