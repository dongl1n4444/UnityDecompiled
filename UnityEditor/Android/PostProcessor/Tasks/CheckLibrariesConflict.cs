namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEngine;

    internal class CheckLibrariesConflict : IPostProcessorTask
    {
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Checking Android libraries for conflicts");
            }
            string str = context.Get<string>("StagingArea");
            AndroidLibraries libraries = context.Get<AndroidLibraries>("AndroidLibraries");
            foreach (string str2 in libraries.GetPackageNames())
            {
                string path = Path.Combine(str, "gen");
                char[] separator = new char[] { '.' };
                string[] strArray2 = str2.Split(separator);
                for (int i = 0; i < strArray2.Length; i++)
                {
                    <Execute>c__AnonStorey0 storey = new <Execute>c__AnonStorey0 {
                        segment = strArray2[i]
                    };
                    string[] strArray3 = Enumerable.Where<string>(Directory.GetDirectories(path), new Func<string, bool>(storey, (IntPtr) this.<>m__0)).Cast<string>().ToArray<string>();
                    if (strArray3.Length < 1)
                    {
                        Debug.LogError($"Something went wrong when parsing the generated resources - couldn't find a directory matching {Path.Combine(path, storey.segment)}");
                        CancelPostProcess.AbortBuildPointToConsole("Error parsing resource!", "Package " + str2 + " has a resource error.");
                    }
                    DirectoryInfo info = new DirectoryInfo(strArray3[0]);
                    if (string.Compare(info.Name, storey.segment) != 0)
                    {
                        Debug.LogError($"Plugin Bundle ID conflict detected: package {str2} has conflicts with other plugins ({info.Name} vs {storey.segment}). Make sure you use the same case for your package names.");
                        CancelPostProcess.AbortBuildPointToConsole("Plugin Bundle ID conflict detected!", "Package " + str2 + " has conflicts with other plugins.");
                    }
                    path = Path.Combine(path, storey.segment);
                }
            }
        }

        public string Name =>
            "Verifying Android libraries";

        [CompilerGenerated]
        private sealed class <Execute>c__AnonStorey0
        {
            internal string segment;

            internal bool <>m__0(string s) => 
                this.segment.Equals(new DirectoryInfo(s).Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

