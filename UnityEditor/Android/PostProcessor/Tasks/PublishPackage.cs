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
        private bool _developmentPlayer;
        private AndroidDevice _device;
        private string _installPath;
        private string _packageName;

        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            this._packageName = context.Get<string>("PackageName");
            this._installPath = context.Get<string>("InstallPath");
            this._device = context.Get<AndroidDevice>("AndroidDevice");
            this._developmentPlayer = context.Get<bool>("DevelopmentPlayer");
            this.UploadAPK(false);
            this.UploadOBB(context);
            this.StartApplication(context);
        }

        private void StartApplication(PostProcessorContext context)
        {
            string path = context.Get<string>("ManifestName");
            if (this._developmentPlayer)
            {
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Setting up profiler tunnel");
                }
                this._device.Forward($"tcp:{ProfilerDriver.directConnectionPort}", "localabstract:Unity-" + this._packageName, null);
            }
            string activityWithLaunchIntent = new AndroidManifest(path).GetActivityWithLaunchIntent();
            if (activityWithLaunchIntent.Length == 0)
            {
                CancelPostProcess.AbortBuild("Unable to start activity!", "No activity in the manifest with action MAIN and category LAUNCHER. Try launching the application manually on the device.");
            }
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Attempting to start Unity Player on device " + this._device.Describe());
            }
            this._device.Launch(this._packageName, activityWithLaunchIntent, null);
        }

        private void UploadAPK(bool retryUpload)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Copying APK package to device " + this._device.Describe());
            }
            string str = this._device.Install(this._installPath, null);
            if (!retryUpload && (str.Contains("[INSTALL_FAILED_UPDATE_INCOMPATIBLE]") || str.Contains("[INSTALL_PARSE_FAILED_INCONSISTENT_CERTIFICATES]")))
            {
                Debug.LogWarning("Application update incompatible (signed with different keys?); removing previous installation (PlayerPrefs will be lost)...\n");
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Removing " + this._packageName + " from device " + this._device.Describe());
                }
                this._device.Uninstall(this._packageName, null);
                this.UploadAPK(true);
            }
            else
            {
                if (((str.Contains("protocol failure") || str.Contains("No space left on device")) || (str.Contains("[INSTALL_FAILED_INSUFFICIENT_STORAGE]") || str.Contains("[INSTALL_FAILED_UPDATE_INCOMPATIBLE]"))) || ((str.Contains("[INSTALL_FAILED_MEDIA_UNAVAILABLE]") || str.Contains("[INSTALL_PARSE_FAILED_INCONSISTENT_CERTIFICATES]")) || str.Contains("Failure [")))
                {
                    Debug.LogError("Installation failed with the following output:\n" + str);
                    CancelPostProcess.AbortBuildPointToConsole("Unable to install APK!", "Installation failed.");
                }
                bool flag = this._developmentPlayer || Unsupported.IsDeveloperBuild();
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Setting device property CheckJNI to " + flag);
                }
                this._device.SetProperty("debug.checkjni", !flag ? "0" : "1", null);
            }
        }

        private void UploadOBB(PostProcessorContext context)
        {
            string str = $"{Path.GetFileNameWithoutExtension(this._installPath)}.main.obb";
            string path = Path.Combine(Path.GetDirectoryName(this._installPath), str);
            if (context.Get<bool>("UseObb") && File.Exists(path))
            {
                int bundleVersionCode = PlayerSettings.Android.bundleVersionCode;
                string[] strArray = new string[] { "/mnt/shell/emulated", "/mnt/shell/emulated/0/Android", this._device.ExternalStorageRoot + "/Android" };
                bool flag2 = false;
                Exception exception = null;
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Copying APK Expansion file to device " + this._device.Describe());
                }
                foreach (string str3 in strArray)
                {
                    string str4 = "obb";
                    string str5 = $"main.{bundleVersionCode.ToString()}.{this._packageName}.obb";
                    string dst = $"{str3}/{str4}/{this._packageName}/{str5}";
                    try
                    {
                        this._device.Push(path, dst, null);
                        flag2 = true;
                        break;
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                    }
                }
                if (!flag2)
                {
                    Debug.LogException(exception);
                    CancelPostProcess.AbortBuildPointToConsole("Unable to deploy OBB to device", "Failed pushing OBB file to the device.");
                }
            }
        }

        public string Name =>
            "Publishing output package";
    }
}

