namespace UnityEditor.Modules
{
    using System;
    using UnityEditor.BuildReporting;

    internal interface IBuildAnalyzer
    {
        void OnAddedExecutable(BuildReport report, int fileIndex);
    }
}

