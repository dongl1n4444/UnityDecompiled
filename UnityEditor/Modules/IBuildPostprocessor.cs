namespace UnityEditor.Modules
{
    using System;
    using UnityEditor;

    internal interface IBuildPostprocessor
    {
        string GetExtension(BuildTarget target, BuildOptions options);
        void LaunchPlayer(BuildLaunchPlayerArgs args);
        void PostProcess(BuildPostProcessArgs args);
        void PostProcessScriptsOnly(BuildPostProcessArgs args);
        string PrepareForBuild(BuildOptions options, BuildTarget target);
        bool SupportsInstallInBuildFolder();
        bool SupportsScriptsOnlyBuild();
    }
}

