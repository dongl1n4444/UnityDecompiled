namespace UnityEditor.Android.PostProcessor
{
    using System;

    internal interface IPostProcessorTask
    {
        event ProgressHandler OnProgress;

        void Execute(PostProcessorContext context);

        string Name { get; }
    }
}

