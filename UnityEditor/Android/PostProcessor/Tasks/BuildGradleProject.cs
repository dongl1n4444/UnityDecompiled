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
        private string _playerPackage;

        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Building");
            }
            this._playerPackage = context.Get<string>("PlayerPackage");
            string stagingArea = context.Get<string>("StagingArea");
            AndroidLibraries androidLibraries = context.Get<AndroidLibraries>("AndroidLibraries");
            string packageName = context.Get<string>("PackageName");
            bool useObb = context.Get<bool>("UseObb");
            int platformApiLevel = context.Get<int>("PlatformApiLevel");
            int num2 = context.Get<int>("ProjectType");
            string googleBuildTools = context.Get<AndroidSDKTools>("SDKTools").BuildToolsVersion(null);
            if (num2 != 1)
            {
                throw new UnityException("Illegal project type: " + num2);
            }
            string[] components = new string[] { this._playerPackage, "Source" };
            string[] textArray2 = new string[] { TasksCommon.GetClassDirectory(context), "classes.jar" };
            AndroidProjectExport.ExportGradle(Paths.Combine(components), Paths.Combine(textArray2), BuildPipeline.GetBuildToolsDirectory(BuildTarget.Android), stagingArea, androidLibraries, null, packageName, PlayerSettings.productName, platformApiLevel, googleBuildTools, useObb);
            bool flag2 = PlayerSettings.Android.keyaliasName.Length != 0;
            bool flag3 = context.Get<bool>("DevelopmentPlayer");
            string sourceFileName = "";
            string task = "";
            string[] textArray3 = new string[] { stagingArea, "gradleOut" };
            string workingdir = Paths.Combine(textArray3);
            if (flag3)
            {
                task = "assembleDebug";
                string[] textArray4 = new string[] { workingdir, "build", "outputs", "apk", "gradleOut-debug.apk" };
                sourceFileName = Paths.Combine(textArray4);
            }
            else
            {
                if (!flag2)
                {
                    CancelPostProcess.AbortBuild("Build Failure", "Release builds have to be signed when using Gradle");
                    return;
                }
                task = "assembleRelease";
                string[] textArray5 = new string[] { workingdir, "build", "outputs", "apk", "gradleOut-release.apk" };
                sourceFileName = Paths.Combine(textArray5);
            }
            GradleWrapper.Run(workingdir, task, delegate (string task) {
                if (((this.OnProgress != null) && (task != "")) && (task[0] == ':'))
                {
                    this.OnProgress(this, "Task " + task.Substring(1));
                }
            });
            string[] textArray6 = new string[] { stagingArea, "Package.apk" };
            File.Move(sourceFileName, Paths.Combine(textArray6));
        }

        public string Name
        {
            get
            {
                return "Building Gradle project";
            }
        }
    }
}

