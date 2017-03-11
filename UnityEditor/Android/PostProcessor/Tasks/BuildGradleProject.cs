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
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Building");
            }
            string str = context.Get<string>("StagingArea");
            int system = context.Get<int>("ProjectType");
            if (system != 1)
            {
                throw new UnityException("Illegal project type: " + system);
            }
            AndroidProjectExport.Create(system).Export(context, null);
            bool flag = PlayerSettings.Android.keyaliasName.Length != 0;
            bool flag2 = context.Get<bool>("DevelopmentPlayer");
            string sourceFileName = "";
            string task = "";
            string[] components = new string[] { "Temp", "gradleOut" };
            string workingdir = Paths.Combine(components);
            if (flag2 || Unsupported.IsDeveloperBuild())
            {
                task = "assembleDebug";
                string[] textArray2 = new string[] { workingdir, "build", "outputs", "apk", "gradleOut-debug.apk" };
                sourceFileName = Paths.Combine(textArray2);
            }
            else
            {
                if (!flag)
                {
                    CancelPostProcess.AbortBuild("Build Failure", "Release builds have to be signed when using Gradle", null);
                    return;
                }
                task = "assembleRelease";
                string[] textArray3 = new string[] { workingdir, "build", "outputs", "apk", "gradleOut-release.apk" };
                sourceFileName = Paths.Combine(textArray3);
            }
            GradleWrapper.Run(workingdir, task, delegate (string task) {
                if (((this.OnProgress != null) && (task != "")) && (task[0] == ':'))
                {
                    this.OnProgress(this, "Task " + task.Substring(1));
                }
            });
            string[] textArray4 = new string[] { str, "Package.apk" };
            File.Move(sourceFileName, Paths.Combine(textArray4));
        }

        public string Name =>
            "Building Gradle project";
    }
}

