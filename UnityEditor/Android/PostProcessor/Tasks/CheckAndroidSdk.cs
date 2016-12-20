namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class CheckAndroidSdk : IPostProcessorTask
    {
        private AndroidSDKTools _sdkTools;

        public event ProgressHandler OnProgress;

        private static int AskUpdateSdk(string title, string message)
        {
            return EditorUtility.DisplayDialogComplex(title, message, "Update Android SDK", "Cancel", "Continue");
        }

        private int EnsureSDKComponentVersion(int minVersion, SDKComponentDetector detector)
        {
            int componentVersion = detector.GetComponentVersion(this._sdkTools, this.OnProgress);
            while (componentVersion < minVersion)
            {
                string title = "Android SDK is outdated";
                string upgradeMsg = detector.GetUpgradeMsg(componentVersion, minVersion);
                if (!InternalEditorUtility.inBatchMode)
                {
                    switch (AskUpdateSdk(title, upgradeMsg))
                    {
                        case 1:
                            throw new UnityException(upgradeMsg);

                        case 2:
                            return componentVersion;
                    }
                }
                int num4 = 0x10;
                while ((componentVersion < minVersion) && (0 < num4--))
                {
                    if (this.OnProgress != null)
                    {
                        this.OnProgress(this, "Updating Android SDK - Tools and Platform Tools");
                    }
                    this._sdkTools.UpdateSDK(null);
                    componentVersion = detector.GetComponentVersion(this._sdkTools, this.OnProgress);
                }
            }
            return componentVersion;
        }

        private int EnsureSDKPlatformAPI(string minPlatformName, int minPlatformApiLevel)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Detecting installed platforms");
            }
            int topAndroidPlatformAvailable = this._sdkTools.GetTopAndroidPlatformAvailable(null);
            while (topAndroidPlatformAvailable < minPlatformApiLevel)
            {
                string title = "Android SDK is missing required platform api";
                string message = string.Format("Minimum platform required is {0} (API level {1})", minPlatformName, minPlatformApiLevel);
                if (!InternalEditorUtility.inBatchMode)
                {
                    switch (AskUpdateSdk(title, message))
                    {
                        case 1:
                            throw new UnityException(message);

                        case 2:
                            return topAndroidPlatformAvailable;
                    }
                }
                int num4 = 0x10;
                while ((topAndroidPlatformAvailable < minPlatformApiLevel) && (0 < num4--))
                {
                    if (this.OnProgress != null)
                    {
                        this.OnProgress(this, string.Format("Updating Android SDK - {0} (API level {1})...", minPlatformName, minPlatformApiLevel));
                    }
                    this._sdkTools.InstallPlatform(minPlatformApiLevel, null);
                    topAndroidPlatformAvailable = this._sdkTools.GetTopAndroidPlatformAvailable(null);
                }
            }
            return topAndroidPlatformAvailable;
        }

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Checking Android SDK and components");
            }
            this._sdkTools = AndroidSDKTools.GetInstance();
            if (this._sdkTools == null)
            {
                CancelPostProcess.AbortBuild("Build failure!", "Unable to locate Android SDK");
            }
            context.Set<AndroidSDKTools>("SDKTools", this._sdkTools);
            this.EnsureSDKComponentVersion(0x18, new SDKToolsDetector(this));
            this.EnsureSDKComponentVersion(0x17, new SDKBuildToolsDetector(this));
            this.EnsureSDKComponentVersion(0x17, new SDKPlatformToolsDetector(this));
            int num = this.EnsureSDKPlatformAPI("Android 6.0", 0x17);
            context.Set<int>("PlatformApiLevel", num);
            int num2 = Math.Max((int) PlayerSettings.Android.minSdkVersion, num);
            context.Set<int>("TargetSDKVersion", num2);
            this._sdkTools.UpdateToolsDirectories();
            this._sdkTools.DumpDiagnostics();
            AndroidJavaTools.DumpDiagnostics();
            string str = "android.jar";
            string androidPlatformPath = this._sdkTools.GetAndroidPlatformPath(num);
            if (androidPlatformPath.Length == 0)
            {
                EditorPrefs.SetString("AndroidSdkRoot", "");
                string message = "Android SDK does not include any platforms! Did you run Android SDK setup to install the platform(s)?\nMinimum platform required for build is Android 5.0 (API level 21)\n";
                CancelPostProcess.AbortBuild("No platforms found", message);
            }
            str = Path.Combine(androidPlatformPath, str);
            context.Set<string>("AndroidJarPath", str);
        }

        public string Name
        {
            get
            {
                return "Android SDK Detection";
            }
        }

        private class SDKBuildToolsDetector : CheckAndroidSdk.SDKComponentDetector
        {
            private IPostProcessorTask _task;

            public SDKBuildToolsDetector(IPostProcessorTask task)
            {
                this._task = task;
            }

            public override int GetComponentVersion(AndroidSDKTools sdkTools, ProgressHandler onProgress)
            {
                if (onProgress != null)
                {
                    onProgress(this._task, "Detecting current build tools version");
                }
                return int.Parse(Regex.Match(sdkTools.BuildToolsVersion(null), @"\d+").Value);
            }

            public override string GetUpgradeMsg(int currentVerion, int minVersion)
            {
                return string.Format("SDK Build Tools version {0} < {1}", currentVerion, minVersion);
            }
        }

        private abstract class SDKComponentDetector
        {
            protected SDKComponentDetector()
            {
            }

            public abstract int GetComponentVersion(AndroidSDKTools sdkTools, ProgressHandler onProgress);
            public abstract string GetUpgradeMsg(int currentVerion, int minVersion);
        }

        private class SDKPlatformToolsDetector : CheckAndroidSdk.SDKComponentDetector
        {
            private IPostProcessorTask _task;

            public SDKPlatformToolsDetector(IPostProcessorTask task)
            {
                this._task = task;
            }

            public override int GetComponentVersion(AndroidSDKTools sdkTools, ProgressHandler onProgress)
            {
                if (onProgress != null)
                {
                    onProgress(this._task, "Detecting current platform tools version");
                }
                return int.Parse(Regex.Match(sdkTools.PlatformToolsVersion(null), @"\d+").Value);
            }

            public override string GetUpgradeMsg(int currentVerion, int minVersion)
            {
                return string.Format("SDK Platform Tools version {0} < {1}", currentVerion, minVersion);
            }
        }

        private class SDKToolsDetector : CheckAndroidSdk.SDKComponentDetector
        {
            private IPostProcessorTask _task;

            public SDKToolsDetector(IPostProcessorTask task)
            {
                this._task = task;
            }

            public override int GetComponentVersion(AndroidSDKTools sdkTools, ProgressHandler onProgress)
            {
                if (onProgress != null)
                {
                    onProgress(this._task, "Detecting current tools version");
                }
                return int.Parse(Regex.Match(sdkTools.ToolsVersion(null), @"\d+").Value);
            }

            public override string GetUpgradeMsg(int currentVerion, int minVersion)
            {
                return string.Format("SDK Tools version {0} < {1}", currentVerion, minVersion);
            }
        }
    }
}

