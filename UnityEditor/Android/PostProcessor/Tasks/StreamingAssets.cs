namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;

    internal class StreamingAssets : IPostProcessorTask
    {
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Copying streaming assets");
            }
            bool flag = context.Get<bool>("UseObb");
            string str = context.Get<string>("StagingArea");
            string[] components = new string[] { str, "assets", "bin" };
            string str2 = Paths.Combine(components);
            string[] textArray2 = new string[] { str2, "Data" };
            string path = Paths.Combine(textArray2);
            string[] textArray3 = new string[] { str, "raw", "bin", "Data" };
            string str4 = Paths.Combine(textArray3);
            string[] textArray4 = new string[] { str, "rawobb", "bin", "Data" };
            string str5 = Paths.Combine(textArray4);
            string strB = ".resS";
            string[] fileSystemEntries = Directory.GetFileSystemEntries(path);
            foreach (string str7 in fileSystemEntries)
            {
                if (string.Compare(Path.GetExtension(str7), strB, true) == 0)
                {
                    string str8 = str4;
                    string fileName = Path.GetFileName(str7);
                    if ((flag && (string.Compare(fileName, "level0.resS", true) != 0)) && (string.Compare(fileName, "sharedassets0.assets.resS", true) != 0))
                    {
                        str8 = str5;
                    }
                    if (!Directory.Exists(str8))
                    {
                        Directory.CreateDirectory(str8);
                    }
                    string destFileName = Path.Combine(str8, fileName);
                    File.Move(str7, destFileName);
                }
            }
            if (Directory.Exists("Assets/StreamingAssets"))
            {
                FileUtil.CopyDirectoryRecursiveForPostprocess("Assets/StreamingAssets", Path.Combine(str, !flag ? "raw" : "rawobb"), true);
            }
        }

        public string Name =>
            "Preparing streaming assets";
    }
}

