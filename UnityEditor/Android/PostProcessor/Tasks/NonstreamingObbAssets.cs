namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;

    internal class NonstreamingObbAssets : IPostProcessorTask
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (context.Get<bool>("UseObb"))
            {
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Preparing expansion assets");
                }
                this.PrepareNonStreamingAssetsForObb(context);
            }
        }

        private void PrepareNonStreamingAssetsForObb(PostProcessorContext context)
        {
            string str = context.Get<string>("StagingArea");
            string[] components = new string[] { str, "assets", "bin" };
            string path = Paths.Combine(components);
            FileUtil.CopyDirectoryRecursive(Path.Combine(str, "assets"), Path.Combine(str, "obbassets"));
            string[] textArray2 = new string[] { str, "obbassets", "bin" };
            string str3 = Paths.Combine(textArray2);
            foreach (string str4 in Directory.GetFiles(str3, "*", SearchOption.AllDirectories))
            {
                if (Regex.IsMatch(str4.Substring(str3.Length + 1).Replace(@"\", "/"), @"^(?:Data/mainData|Data/sharedassets0\.assets(?:\.res[GS]?)?(?:\.split\w+)?|Data/sharedassets0\.resource(?:\.res[GS]?)?(?:\.split\w+)?|Data/level0(?:\.res[GS]?)?(?:\.split\w+)?|Data/globalgamemanagers(?:\.assets)?(?:\.split\w+)?|Data/unity default resources|Data/Resources/unity_builtin_extra|Data/PlayerConnectionConfigFile|Data/Managed/.+\.dll(?:\.mdb)?(?:\.pdb)?|Data/.+\.resS|Data/data\.unity3d(?:\.obb)?)$"))
                {
                    FileUtil.DeleteFileOrDirectory(str4);
                }
            }
            foreach (string str5 in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                if (!Regex.IsMatch(str5.Substring(path.Length + 1).Replace(@"\", "/"), @"^(?:Data/mainData|Data/sharedassets0\.assets(?:\.res[GS]?)?(?:\.split\w+)?|Data/sharedassets0\.resource(?:\.res[GS]?)?(?:\.split\w+)?|Data/level0(?:\.res[GS]?)?(?:\.split\w+)?|Data/globalgamemanagers(?:\.assets)?(?:\.split\w+)?|Data/unity default resources|Data/Resources/unity_builtin_extra|Data/PlayerConnectionConfigFile|Data/Managed/.+\.dll(?:\.mdb)?(?:\.pdb)?|Data/.+\.resS|Data/data\.unity3d(?:\.obb)?)$"))
                {
                    FileUtil.DeleteFileOrDirectory(str5);
                }
            }
        }

        public string Name =>
            "Processing OBB assets";
    }
}

