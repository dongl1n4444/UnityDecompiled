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

    internal class AAPTPackage : IPostProcessorTask
    {
        private string _stagingArea;
        public const int kNumberOfAAPTRetries = 3;

        public event ProgressHandler OnProgress;

        private void AAPTPack(PostProcessorContext context, string apkName, string directory, bool compress)
        {
            AndroidSDKTools tools = context.Get<AndroidSDKTools>("SDKTools");
            string str = "";
            if (!compress)
            {
                str = " -0 \"\"";
            }
            string str2 = "!.svn:!.git:!.ds_store:!*.scc:.*:!CVS:!thumbs.db:!picasa.ini:!*~";
            string args = string.Format("package -v -f -F {0} -A {1}{2} --ignore-assets \"{3}\"", new object[] { apkName, directory, str, str2 });
            string str4 = TasksCommon.Exec(tools.AAPT, args, this._stagingArea, "Android Asset Packaging Tool failed.", 3);
            if (!str4.Contains("Found 0 custom asset files") && (!str4.Contains("Done!") || !File.Exists(Path.Combine(this._stagingArea, apkName))))
            {
                Debug.LogError(string.Format("Android Asset Packaging Tool failed: {0} {1} \n {2}", tools.AAPT, args, str4));
                CancelPostProcess.AbortBuildPointToConsole("AAPT Failed!", "Android Asset Packaging Tool failed.");
            }
        }

        private void BuildObb(PostProcessorContext context)
        {
            bool flag = File.Exists(Path.Combine(this._stagingArea, "obb.ap_"));
            bool flag2 = File.Exists(Path.Combine(this._stagingArea, "rawobb.ap_"));
            if (flag || flag2)
            {
                string str = Path.Combine(Environment.CurrentDirectory, this._stagingArea);
                string[] first = new string[] { "apk", string.Format("{0}/main.obb", str), "-u" };
                if (flag)
                {
                    string[] second = new string[] { "-z", string.Format("{0}/obb.ap_", str) };
                    first = Enumerable.ToArray<string>(Enumerable.Concat<string>(first, second));
                }
                if (flag2)
                {
                    string[] textArray3 = new string[] { "-z", string.Format("{0}/rawobb.ap_", str) };
                    first = Enumerable.ToArray<string>(Enumerable.Concat<string>(first, textArray3));
                }
                string message = TasksCommon.SDKTool(context, first, this._stagingArea, "Failed to build OBB.");
                string fileName = Path.Combine(this._stagingArea, "main.obb");
                FileInfo info = new FileInfo(fileName);
                if (!File.Exists(fileName) || (info.Length == 0L))
                {
                    Debug.LogError(message);
                    CancelPostProcess.AbortBuildPointToConsole("OBB Builder Failed!", "Failed to build OBB package.");
                }
                if (info.Length >= 0x80000000L)
                {
                    Debug.LogError(message);
                    CancelPostProcess.AbortBuildPointToConsole("OBB Builder Failed!", "OBB file too big for Android Market (max 2GB).");
                }
                AndroidXmlDocument document = context.Get<AndroidXmlDocument>("SettingsXml");
                document.PatchStringRes("bool", TasksCommon.GetMD5HashOfEOCD(fileName), true.ToString());
                document.Save();
            }
        }

        public void Execute(PostProcessorContext context)
        {
            if (!context.Get<bool>("UseFastzip"))
            {
                bool flag2 = context.Get<bool>("ExportAndroidProject");
                this._stagingArea = context.Get<string>("StagingArea");
                bool flag3 = context.Get<bool>("UseObb");
                if (!flag2)
                {
                    if (this.OnProgress != null)
                    {
                        this.OnProgress(this, "Building streaming package");
                    }
                    if (Directory.Exists(Path.Combine(this._stagingArea, "raw")))
                    {
                        this.AAPTPack(context, "raw.ap_", "raw", false);
                    }
                }
                if (flag3)
                {
                    if (this.OnProgress != null)
                    {
                        this.OnProgress(this, "Building expansion asset package (OBB)");
                    }
                    this.AAPTPack(context, "obb.ap_", "obbassets", true);
                    if (this.OnProgress != null)
                    {
                        this.OnProgress(this, "Building expansion streaming package (OBB)");
                    }
                    if (Directory.Exists(Path.Combine(this._stagingArea, "rawobb")))
                    {
                        this.AAPTPack(context, "rawobb.ap_", "rawobb", false);
                    }
                    if (this.OnProgress != null)
                    {
                        this.OnProgress(this, "Creating APK expansion package (OBB)");
                    }
                    this.BuildObb(context);
                }
                if (!flag2)
                {
                    if (this.OnProgress != null)
                    {
                        this.OnProgress(this, "Building asset package");
                    }
                    this.AAPTPack(context, "assets.ap_", "assets", true);
                }
            }
        }

        public string Name
        {
            get
            {
                return "AAPT: Compiling all assets into one archive";
            }
        }
    }
}

