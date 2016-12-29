namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEngine;

    internal class BuildAPK : IPostProcessorTask
    {
        private string _stagingArea;

        public event ProgressHandler OnProgress;

        private void AlignPackage(PostProcessorContext context)
        {
            AndroidSDKTools tools = context.Get<AndroidSDKTools>("SDKTools");
            string errorMsg = "Failed to align APK package.";
            string str2 = Path.Combine(Environment.CurrentDirectory, this._stagingArea);
            string args = string.Format("4 \"{0}/Package_unaligned.apk\" \"{0}/Package.apk\"", str2);
            string message = TasksCommon.Exec(tools.ZIPALIGN, args, this._stagingArea, errorMsg, 0);
            if ((message.Contains("zipalign") || message.Contains("Warning")) || !File.Exists(Path.Combine(str2, "Package.apk")))
            {
                Debug.LogError(message);
                CancelPostProcess.AbortBuildPointToConsole("APK Aligning Failed!", errorMsg);
            }
        }

        private void BuildApk(PostProcessorContext context)
        {
            bool flag = context.Get<bool>("DevelopmentPlayer");
            AndroidLibraries libraries = context.Get<AndroidLibraries>("AndroidLibraries");
            bool flag2 = PlayerSettings.Android.keyaliasName.Length != 0;
            string str = Path.Combine(Environment.CurrentDirectory, this._stagingArea);
            string[] first = new string[] { "apk", $"{str}/Package_unaligned.apk", "-z", $"{str}/assets.ap_", "-z", $"{str}/bin/resources.ap_", "-nf", $"{str}/libs", "-f", $"{str}/bin/classes.dex", "-v" };
            foreach (string str2 in libraries.GetLibraryDirectories())
            {
                string[] second = new string[] { "-nf", str2 };
                first = first.Concat<string>(second).ToArray<string>();
            }
            foreach (string str3 in libraries.GetAssetsDirectories())
            {
                string[] textArray3 = new string[] { "-A", str3 };
                first = first.Concat<string>(textArray3).ToArray<string>();
            }
            if (flag2)
            {
                string str4 = !Path.IsPathRooted(PlayerSettings.Android.keystoreName) ? Path.Combine(Directory.GetCurrentDirectory(), PlayerSettings.Android.keystoreName) : PlayerSettings.Android.keystoreName;
                string[] textArray4 = new string[] { "-k", str4, "-kp", PlayerSettings.Android.keystorePass, "-kk", PlayerSettings.Android.keyaliasName, "-kkp", PlayerSettings.Android.keyaliasPass };
                first = first.Concat<string>(textArray4).ToArray<string>();
            }
            if (flag || Unsupported.IsDeveloperBuild())
            {
                string[] textArray5 = new string[] { "-d" };
                first = first.Concat<string>(textArray5).ToArray<string>();
            }
            if (File.Exists(Path.Combine(this._stagingArea, "raw.ap_")))
            {
                string[] textArray6 = new string[] { "-z", $"{str}/raw.ap_" };
                first = first.Concat<string>(textArray6).ToArray<string>();
            }
            string message = TasksCommon.SDKTool(context, first, this._stagingArea, "Failed to build apk.");
            string fileName = Path.Combine(this._stagingArea, "Package_unaligned.apk");
            FileInfo info = new FileInfo(fileName);
            if (!File.Exists(fileName) || (info.Length == 0L))
            {
                Debug.LogError(message);
                CancelPostProcess.AbortBuildPointToConsole("APK Builder Failed!", "Failed to build APK package.");
            }
        }

        public void Execute(PostProcessorContext context)
        {
            if (!context.Get<bool>("UseFastzip"))
            {
                this._stagingArea = context.Get<string>("StagingArea");
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Building target package from assets archive and pre-built binaries");
                }
                this.BuildApk(context);
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Optimizing target package alignment");
                }
                this.AlignPackage(context);
            }
        }

        public string Name =>
            "Creating APK package";
    }
}

