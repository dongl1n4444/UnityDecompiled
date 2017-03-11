namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class PrepareUserResources : IPostProcessorTask
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Processing user-provided Android resources");
            }
            string str = context.Get<string>("AndroidPluginsPath");
            string str2 = context.Get<string>("TargetLibrariesFolder");
            string path = Path.Combine(str, "res");
            if (Directory.Exists(path))
            {
                UnityEngine.Debug.LogWarning("OBSOLETE - Providing Android resources in Assets/Plugins/Android/res is deprecated, please move your resources to an Android Library. See \"Building Plugins for Android\" section of the Manual.");
                if (this.HasDaydreamVRIconResources(path))
                {
                    UnityEngine.Debug.LogWarning("Daydream VR Icon resources have been specified in the Assets/Plugins/Android/res folder. These icons will not be merged into the final APK. Please use the Daydream VR Device settings to set custom Daydream VR Icons.");
                }
                this.GenerateAndroidLibraryWithResources(context, path, Path.Combine(str2, "unity-android-resources"));
            }
        }

        private void GenerateAndroidLibraryWithResources(PostProcessorContext context, string sourceDir, string targetDir)
        {
            int num = context.Get<int>("TargetSDKVersion");
            Directory.CreateDirectory(targetDir);
            string path = Path.Combine(targetDir, "res");
            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(targetDir, AndroidLibraries.ProjectPropertiesFileName), $"android.library=true

target=android-{num}");
            File.WriteAllText(Path.Combine(targetDir, "AndroidManifest.xml"), $"<?xml version="1.0" encoding="utf-8"?><manifest xmlns:android="http://schemas.android.com/apk/res/android" package="{PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android)}"
android:versionCode="1" android:versionName="1.0"></manifest>");
            FileUtil.CopyDirectoryRecursiveForPostprocess(sourceDir, path, true);
        }

        private bool HasDaydreamVRIconResources(string userResourcesPath)
        {
            string[] directories = Directory.GetDirectories(userResourcesPath, "drawable*");
            string[] strArray2 = new string[] { "vr_icon_front.png", "vr_icon_back.png" };
            foreach (string str in directories)
            {
                foreach (string str2 in strArray2)
                {
                    string[] components = new string[] { str, str2 };
                    if (File.Exists(Paths.Combine(components)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string Name =>
            "Processing resources";
    }
}

