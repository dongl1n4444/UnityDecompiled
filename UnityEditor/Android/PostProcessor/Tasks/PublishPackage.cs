namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PublishPackage : IPostProcessorTask
    {
        private string _packageName;
        private string _stagingArea;
        private bool _useObb;

        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            this._stagingArea = context.Get<string>("StagingArea");
            this._packageName = context.Get<string>("PackageName");
            this._useObb = context.Get<bool>("UseObb");
            if (context.Get<bool>("AutoRunPlayer"))
            {
                this.UploadAndStartPlayer(context, false);
            }
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Moving final Android package");
            }
            this.MoveFinalAndroidPackage(context);
        }

        private void MoveFinalAndroidPackage(PostProcessorContext context)
        {
            string path = context.Get<string>("InstallPath");
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path);
                }
                catch (IOException)
                {
                    CancelPostProcess.AbortBuild("Unable to create new apk!", $"Unable to write target apk because {path} is a non-empty directory");
                }
            }
            else
            {
                File.Delete(path);
                if (File.Exists(path))
                {
                    CancelPostProcess.AbortBuild("Unable to delete old apk!", $"Target apk could not be overwritten: {path}");
                }
            }
            FileUtil.MoveFileOrDirectory(Path.Combine(this._stagingArea, "Package.apk"), path);
            if (!File.Exists(path))
            {
                CancelPostProcess.AbortBuild("Unable to create new apk!", $"Unable to move file '{Path.Combine(this._stagingArea, "Package.apk")}' -> '{path}");
            }
            if (this._useObb && File.Exists(Path.Combine(this._stagingArea, "main.obb")))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                string str3 = Path.Combine(Path.GetDirectoryName(path), $"{fileNameWithoutExtension}.main.obb");
                FileUtil.DeleteFileOrDirectory(str3);
                FileUtil.MoveFileOrDirectory(Path.Combine(this._stagingArea, "main.obb"), str3);
            }
        }

        private void UploadAndStartPlayer(PostProcessorContext context, bool retryUpload)
        {
            bool flag = context.Get<bool>("DevelopmentPlayer");
            AndroidDevice device = context.Get<AndroidDevice>("AndroidDevice");
            string path = context.Get<string>("ManifestName");
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Copying APK package to device " + device.Describe());
            }
            string str2 = device.Install(Path.Combine(this._stagingArea, "Package.apk"), null);
            if (!retryUpload && (str2.Contains("[INSTALL_FAILED_UPDATE_INCOMPATIBLE]") || str2.Contains("[INSTALL_PARSE_FAILED_INCONSISTENT_CERTIFICATES]")))
            {
                Debug.LogWarning("Application update incompatible (signed with different keys?); removing previous installation (PlayerPrefs will be lost)...\n");
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Removing " + this._packageName + " from device " + device.Describe());
                }
                device.Uninstall(this._packageName, null);
                this.UploadAndStartPlayer(context, true);
            }
            else
            {
                if (((str2.Contains("protocol failure") || str2.Contains("No space left on device")) || (str2.Contains("[INSTALL_FAILED_INSUFFICIENT_STORAGE]") || str2.Contains("[INSTALL_FAILED_UPDATE_INCOMPATIBLE]"))) || ((str2.Contains("[INSTALL_FAILED_MEDIA_UNAVAILABLE]") || str2.Contains("[INSTALL_PARSE_FAILED_INCONSISTENT_CERTIFICATES]")) || str2.Contains("Failure [")))
                {
                    Debug.LogError("Installation failed with the following output:\n" + str2);
                    CancelPostProcess.AbortBuildPointToConsole("Unable to install APK!", "Installation failed.");
                }
                bool flag2 = flag || Unsupported.IsDeveloperBuild();
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Setting device property CheckJNI to " + flag2);
                }
                device.SetProperty("debug.checkjni", !flag2 ? "0" : "1", null);
                if (this._useObb && File.Exists(Path.Combine(this._stagingArea, "main.obb")))
                {
                    int bundleVersionCode = PlayerSettings.Android.bundleVersionCode;
                    string[] strArray = new string[] { "/mnt/shell/emulated", "/mnt/shell/emulated/0/Android", device.ExternalStorageRoot + "/Android" };
                    bool flag3 = false;
                    Exception exception = null;
                    if (this.OnProgress != null)
                    {
                        this.OnProgress(this, "Copying APK Expansion file to device " + device.Describe());
                    }
                    foreach (string str3 in strArray)
                    {
                        string str4 = "obb";
                        string str5 = $"main.{bundleVersionCode.ToString()}.{this._packageName}.obb";
                        string dst = $"{str3}/{str4}/{this._packageName}/{str5}";
                        try
                        {
                            device.Push(Path.Combine(this._stagingArea, "main.obb"), dst, null);
                            flag3 = true;
                            break;
                        }
                        catch (Exception exception2)
                        {
                            exception = exception2;
                        }
                    }
                    if (!flag3)
                    {
                        Debug.LogException(exception);
                        CancelPostProcess.AbortBuildPointToConsole("Unable to deploy OBB to device", "Failed pushing OBB file to the device.");
                    }
                }
                if (flag)
                {
                    if (this.OnProgress != null)
                    {
                        this.OnProgress(this, "Setting up profiler tunnel");
                    }
                    device.Forward($"tcp:{ProfilerDriver.directConnectionPort}", "localabstract:Unity-" + this._packageName, null);
                }
                string activityWithLaunchIntent = new AndroidManifest(path).GetActivityWithLaunchIntent();
                if (activityWithLaunchIntent.Length == 0)
                {
                    CancelPostProcess.AbortBuild("Unable to start activity!", "No activity in the manifest with action MAIN and category LAUNCHER. Try launching the application manually on the device.");
                }
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Attempting to start Unity Player on device " + device.Describe());
                }
                device.Launch(this._packageName, activityWithLaunchIntent, null);
            }
        }

        public string Name =>
            "Publishing output package";
    }
}

