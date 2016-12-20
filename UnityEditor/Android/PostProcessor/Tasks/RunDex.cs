namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;

    internal class RunDex : IPostProcessorTask
    {
        private AndroidLibraries _androidLibraries;
        private string _stagingArea;

        public event ProgressHandler OnProgress;

        private void BuildDex(PostProcessorContext context)
        {
            string[] components = new string[] { TasksCommon.GetClassDirectory(context), "classes.jar" };
            string[] textArray2 = new string[] { this._stagingArea, "bin", "classes.jar" };
            FileUtil.CopyFileOrDirectory(Paths.Combine(components), Paths.Combine(textArray2));
            string[] array = new string[] { "dx", "--dex", "--verbose", "--output=bin/classes.dex", "bin/classes.jar" };
            if (Directory.Exists(Path.Combine(this._stagingArea, "bin/classes")))
            {
                ArrayUtility.Add<string>(ref array, "bin/classes");
            }
            if (Directory.Exists(Path.Combine(this._stagingArea, "plugins")))
            {
                ArrayUtility.Add<string>(ref array, "plugins");
            }
            foreach (string str in this._androidLibraries.GetLibraryDirectories())
            {
                ArrayUtility.Add<string>(ref array, str);
            }
            foreach (string str2 in this._androidLibraries.GetCompiledJarFiles())
            {
                ArrayUtility.Add<string>(ref array, str2);
            }
            TasksCommon.SDKTool(context, array, this._stagingArea, "Unable to convert classes into dex format.");
        }

        public void Execute(PostProcessorContext context)
        {
            this._stagingArea = context.Get<string>("StagingArea");
            this._androidLibraries = context.Get<AndroidLibraries>("AndroidLibraries");
            bool flag = context.Get<bool>("HasJarPlugins");
            bool flag2 = context.Get<bool>("ExportAndroidProject");
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Converting Java classes to dex-format");
            }
            if (!flag2 && (flag || (this._androidLibraries.Count > 0)))
            {
                this.BuildDex(context);
            }
            else
            {
                string[] components = new string[] { TasksCommon.GetClassDirectory(context), "classes.dex" };
                string[] textArray2 = new string[] { this._stagingArea, "bin", "classes.dex" };
                FileUtil.CopyFileOrDirectory(Paths.Combine(components), Paths.Combine(textArray2));
            }
        }

        public string Name
        {
            get
            {
                return "Building DEX";
            }
        }
    }
}

