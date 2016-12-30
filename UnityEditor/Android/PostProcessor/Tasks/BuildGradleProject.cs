namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class BuildGradleProject : IPostProcessorTask
    {
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            string str;
            string str2;
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Building");
            }
            int system = context.Get<int>("ProjectType");
            if (system != 1)
            {
                throw new UnityException("Illegal project type: " + system);
            }
            AndroidProjectExport.Create(system).Export(context, null);
            bool flag = PlayerSettings.Android.keyaliasName.Length != 0;
            bool flag2 = context.Get<bool>("DevelopmentPlayer");
            string[] components = new string[] { "Temp", "gradleOut" };
            string workingdir = Paths.Combine(components);
            if (flag2 || Unsupported.IsDeveloperBuild())
            {
                str2 = "assembleDebug";
                string[] textArray2 = new string[] { workingdir, "build", "outputs", "apk", "gradleOut-debug.apk" };
                str = Paths.Combine(textArray2);
            }
            else
            {
                if (!flag)
                {
                    CancelPostProcess.AbortBuild("Build Failure", "Release builds have to be signed when using Gradle", null);
                    return;
                }
                str2 = "assembleRelease";
                string[] textArray3 = new string[] { workingdir, "build", "outputs", "apk", "gradleOut-release.apk" };
                str = Paths.Combine(textArray3);
            }
            GradleWrapper.Run(workingdir, str2, delegate (string task) {
                if (((this.OnProgress != null) && (task != "")) && (task[0] == ':'))
                {
                    this.OnProgress(this, "Task " + task.Substring(1));
                }
            });
            string str4 = context.Get<string>("StagingArea");
            string[] textArray4 = new string[] { str4, "Package.apk" };
            File.Move(str, Paths.Combine(textArray4));
        }

        public string Name =>
            "Building Gradle project";
    }
}

