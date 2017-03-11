namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;

    internal class StreamingAssets : IPostProcessorTask
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
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
            string[] source = new string[] { ".ress", ".resource", ".unity3d", ".obb" };
            string[] fileSystemEntries = Directory.GetFileSystemEntries(path);
            foreach (string str6 in fileSystemEntries)
            {
                string str7 = Path.GetExtension(str6).ToLower();
                if (source.Contains<string>(str7))
                {
                    string str8 = str4;
                    string fileName = Path.GetFileName(str6);
                    if (flag)
                    {
                        if (str7 == ".obb")
                        {
                            str8 = str5;
                            fileName = Path.GetFileNameWithoutExtension(fileName);
                        }
                        else if (((string.Compare(fileName, "level0.resS", true) != 0) && (string.Compare(fileName, "sharedassets0.assets.resS", true) != 0)) && ((string.Compare(fileName, "sharedassets0.resource", true) != 0) && (string.Compare(fileName, "data.unity3d", true) != 0)))
                        {
                            str8 = str5;
                        }
                    }
                    if (!Directory.Exists(str8))
                    {
                        Directory.CreateDirectory(str8);
                    }
                    string destFileName = Path.Combine(str8, fileName);
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

