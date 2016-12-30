namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Linq;
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
            string[] source = new string[] { ".ress", ".resource" };
            string[] fileSystemEntries = Directory.GetFileSystemEntries(path);
            foreach (string str6 in fileSystemEntries)
            {
                if (source.Contains<string>(Path.GetExtension(str6).ToLower()))
                {
                    string str7 = str4;
                    string fileName = Path.GetFileName(str6);
                    if ((flag && (string.Compare(fileName, "level0.resS", true) != 0)) && ((string.Compare(fileName, "sharedassets0.assets.resS", true) != 0) && (string.Compare(fileName, "sharedassets0.resource", true) != 0)))
                    {
                        str7 = str5;
                    }
                    if (!Directory.Exists(str7))
                    {
                        Directory.CreateDirectory(str7);
                    }
                    string destFileName = Path.Combine(str7, fileName);
                    File.Move(str6, destFileName);
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

