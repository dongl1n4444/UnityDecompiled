namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;

    internal class AddAndroidLibraries : IPostProcessorTask
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Adding Android libraries");
            }
            string str = context.Get<string>("TargetLibrariesFolder");
            AndroidLibraries libraries = new AndroidLibraries();
            libraries.FindAndAddLibraryProjects(Path.Combine(str, "*"));
            context.Set<AndroidLibraries>("AndroidLibraries", libraries);
        }

        public string Name =>
            "Processing Android libraries";
    }
}

