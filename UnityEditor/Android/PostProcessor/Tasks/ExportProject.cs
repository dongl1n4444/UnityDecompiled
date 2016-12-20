namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class ExportProject : IPostProcessorTask
    {
        private string _playerPackage;

        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (context.Get<bool>("ExportAndroidProject"))
            {
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Exporting Google Android project");
                }
                this._playerPackage = context.Get<string>("PlayerPackage");
                string stagingArea = context.Get<string>("StagingArea");
                AndroidLibraries androidLibraries = context.Get<AndroidLibraries>("AndroidLibraries");
                string targetPath = context.Get<string>("InstallPath");
                string packageName = context.Get<string>("PackageName");
                bool useObb = context.Get<bool>("UseObb");
                int platformApiLevel = context.Get<int>("PlatformApiLevel");
                string googleBuildTools = context.Get<AndroidSDKTools>("SDKTools").BuildToolsVersion(null);
                int num2 = context.Get<int>("ProjectType");
                switch (num2)
                {
                    case 2:
                    {
                        string[] components = new string[] { this._playerPackage, "Source" };
                        string[] textArray2 = new string[] { TasksCommon.GetClassDirectory(context), "classes.jar" };
                        AndroidProjectExport.ExportADT(Paths.Combine(components), Paths.Combine(textArray2), BuildPipeline.GetBuildToolsDirectory(BuildTarget.Android), stagingArea, androidLibraries, targetPath, packageName, PlayerSettings.productName, platformApiLevel, useObb);
                        return;
                    }
                    case 1:
                    {
                        string[] textArray3 = new string[] { this._playerPackage, "Source" };
                        string[] textArray4 = new string[] { TasksCommon.GetClassDirectory(context), "classes.jar" };
                        AndroidProjectExport.ExportGradle(Paths.Combine(textArray3), Paths.Combine(textArray4), BuildPipeline.GetBuildToolsDirectory(BuildTarget.Android), stagingArea, androidLibraries, targetPath, packageName, PlayerSettings.productName, platformApiLevel, googleBuildTools, useObb);
                        return;
                    }
                }
                throw new UnityException("Unknown project type: " + num2);
            }
        }

        public string Name
        {
            get
            {
                return "Exporting project";
            }
        }
    }
}

