namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEngine;

    internal class FastZip : IPostProcessorTask
    {
        public event ProgressHandler OnProgress;

        private void CreatePackagesWithFastzip(PostProcessorContext context)
        {
            string str5;
            string str6;
            string str7;
            FileInfo info;
            string workingdir = context.Get<string>("StagingArea");
            bool flag = context.Get<bool>("UseObb");
            AndroidLibraries libraries = context.Get<AndroidLibraries>("AndroidLibraries");
            AndroidXmlDocument document = context.Get<AndroidXmlDocument>("SettingsXml");
            string command = context.Get<string>("FastzipExe");
            string str3 = "Package.apk";
            string errorMsg = "Fastzip failed.";
            string str8 = " -j --seq -5";
            if (flag)
            {
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Using fastzip to build expansion package (OBB)");
                }
                str6 = ("main.obb" + str8 + " obbassets=assets") + " rawobb=assets";
                str5 = TasksCommon.Exec(command, str6, workingdir, errorMsg, 0);
                str7 = Path.Combine(workingdir, "main.obb");
                info = new FileInfo(str7);
                if (!File.Exists(str7) || (info.Length == 0L))
                {
                    Debug.LogError(str5);
                    CancelPostProcess.AbortBuildPointToConsole("OBB Builder Failed!", "Failed to build OBB package.");
                }
                if (info.Length >= 0x80000000L)
                {
                    Debug.LogError(str5);
                    CancelPostProcess.AbortBuildPointToConsole("OBB Builder Failed!", "OBB file too big for Android Market (max 2GB).");
                }
                document.PatchStringRes("bool", TasksCommon.GetMD5HashOfEOCD(str7), true.ToString());
                document.Save();
            }
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Using fastzip to build target package (APK)");
            }
            string str9 = " libs=lib";
            foreach (string str10 in libraries.GetLibraryDirectories())
            {
                str9 = str9 + $" {str10}=libs";
            }
            string str11 = " --apk";
            if (PlayerSettings.Android.keyaliasName.Length != 0)
            {
                string str12 = !Path.IsPathRooted(PlayerSettings.Android.keystoreName) ? Path.Combine(Directory.GetCurrentDirectory(), PlayerSettings.Android.keystoreName) : PlayerSettings.Android.keystoreName;
                str11 = $" -A --sign={str12},{PlayerSettings.Android.keyaliasPass},{PlayerSettings.Android.keyaliasName}";
            }
            string[] textArray1 = new string[] { str3, str8, str11, str9, " bin/classes.dex assets -0 -Z bin/resources.ap_ raw=assets" };
            str6 = string.Concat(textArray1);
            str5 = TasksCommon.Exec(command, str6, workingdir, errorMsg, 0);
            str7 = Path.Combine(workingdir, str3);
            info = new FileInfo(str7);
            if (!File.Exists(str7) || (info.Length == 0L))
            {
                Debug.LogError(str5);
                CancelPostProcess.AbortBuildPointToConsole("Fastzip Failed!", "Failed to build APK package.");
            }
        }

        public void Execute(PostProcessorContext context)
        {
            bool flag = context.Get<bool>("UseFastzip");
            bool flag2 = context.Get<bool>("ExportAndroidProject");
            if (flag && !flag2)
            {
                this.CreatePackagesWithFastzip(context);
            }
        }

        public string Name =>
            "fastzip";
    }
}

