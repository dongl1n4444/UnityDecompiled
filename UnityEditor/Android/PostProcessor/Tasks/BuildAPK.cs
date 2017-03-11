namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEngine;

    internal class BuildAPK : IPostProcessorTask
    {
        private bool _sign_using_keystore;
        private string _stagingArea;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
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
                UnityEngine.Debug.LogError(message);
                CancelPostProcess.AbortBuildPointToConsole("APK Aligning Failed!", errorMsg);
            }
        }

        private void BuildApk(PostProcessorContext context)
        {
            bool flag = context.Get<bool>("DevelopmentPlayer");
            AndroidLibraries libraries = context.Get<AndroidLibraries>("AndroidLibraries");
            string str = Path.Combine(Environment.CurrentDirectory, this._stagingArea);
            string[] first = new string[] { "apk", $"{str}/Package_unaligned.apk", "-z", $"{str}/assets.ap_", "-z", $"{str}/bin/resources.ap_", "-nf", $"{str}/libs", "-f", $"{str}/bin/classes.dex", "-v" };
            if (this._sign_using_keystore)
            {
                string[] second = new string[] { "-u" };
                first = first.Concat<string>(second).ToArray<string>();
            }
            foreach (string str2 in libraries.GetLibraryDirectories())
            {
                string[] textArray3 = new string[] { "-nf", str2 };
                first = first.Concat<string>(textArray3).ToArray<string>();
            }
            foreach (string str3 in libraries.GetAssetsDirectories())
            {
                string[] textArray4 = new string[] { "-A", str3 };
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
                UnityEngine.Debug.LogError(message);
                CancelPostProcess.AbortBuildPointToConsole("APK Builder Failed!", "Failed to build APK package.");
            }
        }

        public void Execute(PostProcessorContext context)
        {
            bool flag = context.Get<bool>("UseFastzip");
            this._sign_using_keystore = PlayerSettings.Android.keyaliasName.Length != 0;
            if (!flag)
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
                if (this._sign_using_keystore)
                {
                    this.SignPackage(context);
                }
            }
        }

        private string Quote(string input) => 
            $""{input}"";

        private void SignPackage(PostProcessorContext context)
        {
            AndroidSDKTools tools = context.Get<AndroidSDKTools>("SDKTools");
            string errorMsg = "Failed to sign APK package.";
            string str2 = Path.Combine(Environment.CurrentDirectory, this._stagingArea);
            string input = !Path.IsPathRooted(PlayerSettings.Android.keystoreName) ? Path.Combine(Directory.GetCurrentDirectory(), PlayerSettings.Android.keystoreName) : PlayerSettings.Android.keystoreName;
            string[] textArray1 = new string[] { "sign --ks ", this.Quote(input), " --ks-pass pass:", this.Quote(PlayerSettings.Android.keystorePass), " --ks-key-alias ", this.Quote(PlayerSettings.Android.keyaliasName), " --key-pass pass:", this.Quote(PlayerSettings.Android.keyaliasPass), " ", this.Quote($"{str2}/Package.apk") };
            string args = string.Concat(textArray1);
            string message = TasksCommon.Exec(tools.APKSIGNER, args, this._stagingArea, errorMsg, 0);
            if (message.Contains("sign") || message.Contains("Warning"))
            {
                UnityEngine.Debug.LogError(message);
                CancelPostProcess.AbortBuildPointToConsole("APK Signing Failed!", errorMsg);
            }
        }

        public string Name =>
            "Creating APK package";
    }
}

