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

    internal class ProcessAAR : IPostProcessorTask
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            string str = context.Get<string>("StagingArea");
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Preparing and unpacking AAR plugins");
            }
            string str2 = Path.Combine(str, "aar");
            string path = Path.Combine(str, "android-libraries");
            Directory.CreateDirectory(path);
            string[] strArray = AndroidFileLocator.Find(Path.Combine(str2, "*.aar"));
            foreach (string str4 in strArray)
            {
                string fileName = Path.GetFileName(str4);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(str4);
                string str7 = Path.Combine(path, fileNameWithoutExtension);
                if (Directory.Exists(str7))
                {
                    CancelPostProcess.AbortBuild("Build failure", "Plugin conflict detected for file " + fileName, null);
                }
                Directory.CreateDirectory(str7);
                TasksCommon.Exec(AndroidJavaTools.jarPath, "xf \"" + str4 + "\"", str7, "Error unpacking file " + fileName, 0);
                string str8 = Path.Combine(str7, "libs");
                Directory.CreateDirectory(str8);
                FileUtil.MoveFileOrDirectory(Path.Combine(str7, "classes.jar"), Path.Combine(str8, "classes.jar"));
                string str9 = Path.Combine(str7, "jni");
                if (Directory.Exists(str9))
                {
                    string str10 = Path.Combine(str7, "libs");
                    foreach (string str11 in Directory.GetDirectories(str9, "*"))
                    {
                        string to = Path.Combine(str10, FileUtil.RemovePathPrefix(str11, str9));
                        FileUtil.MoveFileOrDirectory(str11, to);
                    }
                }
                string str13 = Path.Combine(str7, "src");
                if (!Directory.Exists(str13))
                {
                    Directory.CreateDirectory(str13);
                }
                if (!File.Exists(Path.Combine(str7, AndroidLibraries.ProjectPropertiesFileName)))
                {
                    int num3 = context.Get<int>("TargetSDKVersion");
                    File.WriteAllText(Path.Combine(str7, AndroidLibraries.ProjectPropertiesFileName), $"android.library=true

target=android-{num3}");
                }
            }
        }

        public string Name =>
            "Processing AAR plugins";
    }
}

