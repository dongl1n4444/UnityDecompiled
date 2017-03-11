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
    using UnityEditor.Utils;

    internal class PrepareUnityResources : IPostProcessorTask
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event ProgressHandler OnProgress;

        private void CleanupMdbFiles(string path)
        {
            string[] strArray = AndroidFileLocator.Find(Path.Combine(path, "*.mdb"));
            foreach (string str in strArray)
            {
                FileUtil.DeleteFileOrDirectory(str);
            }
        }

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Copying unity resources");
            }
            string str = context.Get<string>("PlayerPackage");
            string str2 = context.Get<string>("StagingArea");
            string from = context.Get<string>("StagingAreaData");
            string[] components = new string[] { str, "Data", "unity default resources" };
            string[] textArray2 = new string[] { from, "unity default resources" };
            FileUtil.CopyFileOrDirectory(Paths.Combine(components), Paths.Combine(textArray2));
            string[] textArray3 = new string[] { str2, "assets", "bin" };
            string path = Paths.Combine(textArray3);
            Directory.CreateDirectory(path);
            FileUtil.MoveFileOrDirectory(from, Path.Combine(path, "Data"));
            if (!context.Get<bool>("DevelopmentPlayer"))
            {
                string[] textArray4 = new string[] { path, "Data", "Managed" };
                this.CleanupMdbFiles(Paths.Combine(textArray4));
            }
        }

        public string Name =>
            "Preparing Unity resources";
    }
}

