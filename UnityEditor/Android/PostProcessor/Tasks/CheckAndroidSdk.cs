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
    using UnityEditorInternal;
    using UnityEngine;

    internal class CheckAndroidSdk : IPostProcessorTask
    {
        private AndroidSDKTools _sdkTools;
        public const int kMinAndroidSDKBuildToolsVersion = 0x19;
        public const int kMinAndroidSDKPlatformToolsVersion = 0x17;
        public const int kMinAndroidSDKPlatformVersion = 0x17;
        public const int kMinAndroidSDKToolsVersion = 0x18;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event ProgressHandler OnProgress;

        private static int AskUpdateSdk(string title, string message) => 
            EditorUtility.DisplayDialogComplex(title, message, "Update Android SDK", "Cancel", "Use Highest Installed");

        private int EnsureSDKComponentVersion(int minVersion, SDKComponentDetector detector) => 
            this.EnsureSDKComponentVersion(new Version(minVersion, 0, 0), detector).Major;

        private Version EnsureSDKComponentVersion(Version minVersion, SDKComponentDetector detector)
        {
            while (!detector.Detect(this._sdkTools, minVersion, this.OnProgress))
            {
                if (!InternalEditorUtility.inBatchMode)
                {
                    string updateMessage = detector.GetUpdateMessage(this._sdkTools, minVersion);
                    switch (AskUpdateSdk(detector.UpdateTitle, updateMessage))
                    {
                        case 1:
                            throw new UnityException(updateMessage);

                        case 2:
                            return detector.Version;
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    if (detector.Update(this._sdkTools, minVersion, this.OnProgress))
                    {
                        return detector.Version;
                    }
                }
                if (InternalEditorUtility.inBatchMode)
                {
                    throw new UnityException("Failed to update Android SDK.");
                }
            }
            return detector.Version;
        }

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Checking Android SDK and components");
            }
            bool flag = context.Get<int>("ProjectType") == 3;
            this._sdkTools = !flag ? AndroidSDKTools.GetInstance() : VisualStudioAndroidSDKTools.GetInstance();
            if (this._sdkTools == null)
            {
                string message = "Unable to locate Android SDK.";
                if (flag)
                {
                    message = message + "\n- Make sure Visual Studio 2015 Update 3 with Android support for C++ is installed." + "\n- Set Android SDK path in Visual Studio (Tools -> Options -> Cross Platform).";
                }
                CancelPostProcess.AbortBuild("Build failure!", message, null);
            }
            context.Set<AndroidSDKTools>("SDKTools", this._sdkTools);
            this.EnsureSDKComponentVersion(0x18, new SDKToolsDetector(this));
            this.EnsureSDKComponentVersion(0x17, new SDKPlatformToolsDetector(this));
            int minSdkVersion = (int) PlayerSettings.Android.minSdkVersion;
            int targetSdkVersion = (int) PlayerSettings.Android.targetSdkVersion;
            int minVersion = Math.Max(0x17, Math.Max(minSdkVersion, targetSdkVersion));
            int num4 = this.EnsureSDKComponentVersion(minVersion, new SDKPlatformDetector(this));
            if (minSdkVersion > num4)
            {
                object[] objArray1 = new object[] { "Build set to use Minimum SDK of ", PlayerSettings.Android.minSdkVersion, " but the latest installed SDK on the system is ", num4, ".\nPlease use the Android SDK installation tool to install the minimum required SDK version.\n" };
                string str2 = string.Concat(objArray1);
                CancelPostProcess.AbortBuild("Requested minimum Android SDK not installed", str2, null);
            }
            if (targetSdkVersion == 0)
            {
                targetSdkVersion = num4;
            }
            else if (targetSdkVersion < minSdkVersion)
            {
                UnityEngine.Debug.LogWarning(string.Concat(new object[] { "Selected target SDK version (", targetSdkVersion, ") is lower than the selected minimum SDK version (", minSdkVersion, ").\n Setting target SDK version to ", minSdkVersion, ".\n" }));
                targetSdkVersion = minSdkVersion;
                PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions) minSdkVersion;
            }
            else if (targetSdkVersion > num4)
            {
                UnityEngine.Debug.LogWarning(string.Concat(new object[] { "Selected target SDK version (", targetSdkVersion, ") is higher than the latest installed SDK version (", num4, ").\n Setting target SDK version to ", num4, ".\n" }));
                targetSdkVersion = num4;
                PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            }
            context.Set<int>("MinSDKVersion", minSdkVersion);
            context.Set<int>("TargetSDKVersion", targetSdkVersion);
            if (PlayerSettings.virtualRealitySupported)
            {
                VrSupportChecker.CheckVrSdkVersions(context);
            }
            if (this._sdkTools.IsVisualStudio)
            {
                ((VisualStudioAndroidSDKTools) this._sdkTools).SetBuildToolsVersion(targetSdkVersion);
                this.EnsureSDKComponentVersion(targetSdkVersion, new VisualStudioSDKBuildToolsDetector(this));
            }
            else
            {
                this.EnsureSDKComponentVersion(0x19, new SDKBuildToolsDetector(this));
            }
            this._sdkTools.UpdateToolsDirectories();
            this._sdkTools.DumpDiagnostics();
            AndroidJavaTools.DumpDiagnostics();
            string str3 = "android.jar";
            string androidPlatformPath = this._sdkTools.GetAndroidPlatformPath(targetSdkVersion);
            if (androidPlatformPath == null)
            {
                string str5 = "Android SDK does not include your Target SDK of " + targetSdkVersion + ".\nPlease use the Android SDK installation tool to install your target SDK version.\n";
                CancelPostProcess.AbortBuild("Target Android SDK not installed", str5, null);
            }
            str3 = Path.Combine(androidPlatformPath, str3);
            context.Set<string>("AndroidJarPath", str3);
        }

        public string Name =>
            "Android SDK Detection";

        private class SDKBuildToolsDetector : CheckAndroidSdk.SDKComponentDetector
        {
            public SDKBuildToolsDetector(IPostProcessorTask task) : base(task)
            {
            }

            protected override Version GetVersion(AndroidSDKTools sdkTools) => 
                sdkTools.BuildToolsVersion(null);

            protected override string Name =>
                "SDK Build Tools";
        }

        private abstract class SDKComponentDetector
        {
            private const string kUpdateFailedMessage = "\nMake sure Android SDK path is writable by the Editor.";
            private const string kVisualStudioUpdateFailedMessage = "\nPath can be changed in Visual Studio (Tools -> Options -> Cross Platform -> C++ -> Android -> Android SDK). Restart the build for changes to take effect.";
            protected int m_FailCount;
            protected readonly IPostProcessorTask m_Task;
            protected System.Version m_Version;

            public SDKComponentDetector(IPostProcessorTask task)
            {
                this.m_Task = task;
                this.m_Version = Utils.DefaultVersion;
            }

            public bool Detect(AndroidSDKTools sdkTools, System.Version minVersion, ProgressHandler onProgress)
            {
                if (onProgress != null)
                {
                    onProgress(this.m_Task, $"Detecting current {this.Name} version...");
                }
                this.m_Version = this.GetVersion(sdkTools);
                bool flag = !this.UpdateNeeded(minVersion);
                this.m_FailCount = !flag ? (this.m_FailCount + 1) : 0;
                return flag;
            }

            protected virtual string GetUpdateMessage(System.Version minVersion) => 
                $"{this.Name} version {this.m_Version} < {minVersion}.";

            public string GetUpdateMessage(AndroidSDKTools sdkTools, System.Version minVersion)
            {
                string updateMessage = this.GetUpdateMessage(minVersion);
                if (this.m_FailCount > 1)
                {
                    updateMessage = updateMessage + "\nMake sure Android SDK path is writable by the Editor.";
                    if (sdkTools.IsVisualStudio)
                    {
                        updateMessage = updateMessage + "\nPath can be changed in Visual Studio (Tools -> Options -> Cross Platform -> C++ -> Android -> Android SDK). Restart the build for changes to take effect.";
                    }
                }
                return updateMessage;
            }

            protected abstract System.Version GetVersion(AndroidSDKTools sdkTools);
            public virtual bool Update(AndroidSDKTools sdkTools, System.Version minVersion, ProgressHandler onProgress)
            {
                if (onProgress != null)
                {
                    onProgress(this.m_Task, "Updating Android SDK - Tools, Platform Tools and Build Tools...");
                }
                sdkTools.UpdateSDK(null);
                return this.Detect(sdkTools, minVersion, onProgress);
            }

            protected virtual bool UpdateNeeded(System.Version minVersion) => 
                (this.m_Version < minVersion);

            protected abstract string Name { get; }

            public virtual string UpdateTitle =>
                "Android SDK is outdated";

            public System.Version Version =>
                this.m_Version;
        }

        private class SDKPlatformDetector : CheckAndroidSdk.SDKComponentDetector
        {
            private readonly string m_MinPlatformName;

            public SDKPlatformDetector(IPostProcessorTask task) : base(task)
            {
            }

            protected override string GetUpdateMessage(Version minVersion) => 
                $"Required API level {minVersion.Major}.";

            protected override Version GetVersion(AndroidSDKTools sdkTools)
            {
                int topAndroidPlatformAvailable = sdkTools.GetTopAndroidPlatformAvailable(null);
                return ((topAndroidPlatformAvailable <= 0) ? Utils.DefaultVersion : new Version(topAndroidPlatformAvailable, 0, 0));
            }

            public override bool Update(AndroidSDKTools sdkTools, Version minVersion, ProgressHandler onProgress)
            {
                if (onProgress != null)
                {
                    onProgress(base.m_Task, $"Updating Android SDK to API level {minVersion.Major} ...");
                }
                sdkTools.InstallPlatform(minVersion.Major, null);
                return base.Detect(sdkTools, minVersion, onProgress);
            }

            protected override string Name =>
                "SDK Platform";

            public override string UpdateTitle =>
                "Android SDK is missing required platform API";
        }

        private class SDKPlatformToolsDetector : CheckAndroidSdk.SDKComponentDetector
        {
            public SDKPlatformToolsDetector(IPostProcessorTask task) : base(task)
            {
            }

            protected override Version GetVersion(AndroidSDKTools sdkTools) => 
                sdkTools.PlatformToolsVersion(null);

            protected override string Name =>
                "SDK Platform Tools";
        }

        private class SDKToolsDetector : CheckAndroidSdk.SDKComponentDetector
        {
            public SDKToolsDetector(IPostProcessorTask task) : base(task)
            {
            }

            protected override Version GetVersion(AndroidSDKTools sdkTools) => 
                sdkTools.ToolsVersion(null);

            protected override string Name =>
                "SDK Tools";
        }

        private class VisualStudioSDKBuildToolsDetector : CheckAndroidSdk.SDKBuildToolsDetector
        {
            public VisualStudioSDKBuildToolsDetector(IPostProcessorTask task) : base(task)
            {
            }

            protected override string GetUpdateMessage(Version minVersion) => 
                $"{this.Name} version {minVersion} not found.";

            protected override bool UpdateNeeded(Version minVersion) => 
                (base.m_Version != minVersion);
        }
    }
}

