namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEngine;

    internal class BuildResources : IPostProcessorTask
    {
        private string _stagingArea;

        public event ProgressHandler OnProgress;

        private void CompileResources(PostProcessorContext context)
        {
            AndroidLibraries libraries = context.Get<AndroidLibraries>("AndroidLibraries");
            string str = context.Get<string>("AndroidJarPath");
            AndroidSDKTools tools = context.Get<AndroidSDKTools>("SDKTools");
            string str2 = "gen";
            string fullName = Directory.CreateDirectory(Path.Combine(this._stagingArea, str2)).FullName;
            string args = $"package --auto-add-overlay -v -f -m -J "{str2}" -M "{"AndroidManifest.xml"}" -S "{"res"}" -I "{str}" -F {"bin/resources.ap_"}";
            if (libraries.Count > 0)
            {
                args = args + $" --extra-packages {string.Join(":", libraries.GetPackageNames())}";
                foreach (string str5 in libraries.GetResourceDirectories())
                {
                    args = args + $" -S "{str5}"";
                }
            }
            string errorMsg = "Failed to re-package resources.";
            string str7 = TasksCommon.Exec(tools.AAPT, args, this._stagingArea, errorMsg, 3);
            if (!str7.Contains("Done!") || !File.Exists(Path.Combine(this._stagingArea, "bin/resources.ap_")))
            {
                Debug.LogError("Failed to re-package resources with the following parameters:\n" + args + "\n" + str7);
                CancelPostProcess.AbortBuildPointToConsole("Resource re-package failed!", errorMsg);
            }
            if (libraries.Count > 0)
            {
                List<string> list = new List<string>();
                foreach (string str8 in Directory.GetFiles(fullName, "*.java", SearchOption.AllDirectories))
                {
                    list.Add(str8.Substring(fullName.Length + 1));
                }
                string str9 = Directory.CreateDirectory(Path.Combine(this._stagingArea, "bin/classes")).FullName;
                string str10 = $"-bootclasspath "{str}" -d "{str9}" -source 1.6 -target 1.6 -encoding UTF-8 "{string.Join("\" \"", list.ToArray())}"";
                string str11 = "Failed to recompile android resource files.";
                string str12 = TasksCommon.Exec(AndroidJavaTools.javacPath, str10, fullName, str11, 0);
                if (str12.Trim().Length > 0)
                {
                    Debug.LogError("Failed to compile resources with the following parameters:\n" + str10 + "\n" + str12);
                    CancelPostProcess.AbortBuildPointToConsole("Resource compilation failed!", str11);
                }
            }
        }

        public void Execute(PostProcessorContext context)
        {
            this._stagingArea = context.Get<string>("StagingArea");
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Updating resources with player settings");
            }
            this.ReplaceIconResources();
            if (!context.Get<bool>("ExportAndroidProject"))
            {
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Repackaging and recompiling resources");
                }
                this.CompileResources(context);
            }
        }

        private void ReplaceIconResources()
        {
            string[] array = new string[] { "res.drawable-ldpi.app_icon.png", "res/drawable-ldpi/app_icon.png", "res.drawable-mdpi.app_icon.png", "res/drawable-mdpi/app_icon.png", "res.drawable-hdpi.app_icon.png", "res/drawable-hdpi/app_icon.png", "res.drawable-xhdpi.app_icon.png", "res/drawable-xhdpi/app_icon.png", "res.drawable-xxhdpi.app_icon.png", "res/drawable-xxhdpi/app_icon.png", "res.drawable-xxxhdpi.app_icon.png", "res/drawable-xxxhdpi/app_icon.png", "res.drawable-xhdpi.app_banner.png", "res/drawable-xhdpi/app_banner.png", "app_splash.png", "assets/bin/Data/splash.png" };
            if (!PlayerSettings.advancedLicense)
            {
                Array.Resize<string>(ref array, array.Length - 2);
            }
            if (!PlayerSettings.Android.androidBannerEnabled)
            {
                FileUtil.DeleteFileOrDirectory(Path.Combine(this._stagingArea, "res/drawable-xhdpi/app_banner.png"));
            }
            for (int i = 0; i < array.Length; i += 2)
            {
                string path = Path.Combine(this._stagingArea, array[i]);
                string str3 = Path.Combine(this._stagingArea, array[i + 1]);
                if (File.Exists(path))
                {
                    FileUtil.DeleteFileOrDirectory(str3);
                    Directory.CreateDirectory(Path.GetDirectoryName(str3));
                    FileUtil.MoveFileOrDirectory(path, str3);
                }
            }
        }

        public string Name =>
            "Compiling resources";
    }
}

