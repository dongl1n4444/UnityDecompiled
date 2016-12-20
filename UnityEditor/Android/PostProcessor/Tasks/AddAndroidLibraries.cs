namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;

    internal class AddAndroidLibraries : IPostProcessorTask
    {
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

        public string Name
        {
            get
            {
                return "Processing Android libraries";
            }
        }
    }
}

