namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;
    using UnityEditorInternal.VR;
    using UnityEngine;

    internal class BuildResources : IPostProcessorTask
    {
        private string _playerPackage;
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
            this._playerPackage = context.Get<string>("PlayerPackage");
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Updating resources with player settings");
            }
            this.ReplaceIconResources();
            this.ReplaceVRIconResources();
            bool flag = context.Get<bool>("ExportAndroidProject");
            bool flag2 = context.Get<int>("ProjectType") == 1;
            if (!flag && !flag2)
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
            string[] array = new string[] { "res.drawable-ldpi.app_icon.png", "res/drawable-ldpi/app_icon.png", "res.drawable-mdpi.app_icon.png", "res/drawable-mdpi/app_icon.png", "res.drawable-hdpi.app_icon.png", "res/drawable-hdpi/app_icon.png", "res.drawable-xhdpi.app_icon.png", "res/drawable-xhdpi/app_icon.png", "res.drawable-xxhdpi.app_icon.png", "res/drawable-xxhdpi/app_icon.png", "res.drawable-xxxhdpi.app_icon.png", "res/drawable-xxxhdpi/app_icon.png", "res.drawable-xhdpi.app_banner.png", "res/drawable-xhdpi/app_banner.png", "app_splash.png", "res/drawable/unity_static_splash.png" };
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

        private void ReplaceVRIconResource(string iconName)
        {
            string[] strArray = new string[] { "res.drawable-nodpi.", "res/drawable-nodpi/" };
            string[] vREnabledDevicesOnTargetGroup = VREditor.GetVREnabledDevicesOnTargetGroup(BuildTargetGroup.Android);
            if (PlayerSettings.virtualRealitySupported && (Array.IndexOf<string>(vREnabledDevicesOnTargetGroup, "daydream") >= 0))
            {
                if (!File.Exists(Path.Combine(this._stagingArea, strArray[0] + iconName)))
                {
                    string[] components = new string[] { this._playerPackage, Path.Combine("Apk/vr", iconName) };
                    string from = Paths.Combine(components);
                    string path = Path.Combine(this._stagingArea, Path.Combine(strArray[1], iconName));
                    string directoryName = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    FileUtil.CopyFileOrDirectory(from, path);
                }
                else
                {
                    for (int i = 0; i < strArray.Length; i += 2)
                    {
                        string str5 = Path.Combine(this._stagingArea, strArray[i] + iconName);
                        string str6 = Path.Combine(this._stagingArea, Path.Combine(strArray[i + 1], iconName));
                        if (File.Exists(str5))
                        {
                            FileUtil.DeleteFileOrDirectory(str6);
                            Directory.CreateDirectory(Path.GetDirectoryName(str6));
                            FileUtil.MoveFileOrDirectory(str5, str6);
                        }
                    }
                }
            }
        }

        private void ReplaceVRIconResources()
        {
            this.ReplaceVRIconResource("vr_icon_front.png");
            this.ReplaceVRIconResource("vr_icon_back.png");
        }

        public string Name =>
            "Compiling resources";
    }
}

