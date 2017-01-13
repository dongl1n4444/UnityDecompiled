namespace UnityEditor.Android
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class AndroidBuildPostprocessor : DefaultBuildPostprocessor
    {
        public override string GetExtension(BuildTarget target, BuildOptions options) => 
            "apk";

        public override void LaunchPlayer(BuildLaunchPlayerArgs args)
        {
            PostProcessAndroidPlayer.Launch(args.target, args.installPath);
        }

        public override void PostProcess(BuildPostProcessArgs args)
        {
            PostProcessAndroidPlayer.PostProcess(args.target, args.stagingAreaData, args.stagingArea, args.playerPackage, args.installPath, args.companyName, args.productName, args.options, args.usedClassRegistry);
        }

        public override string PrepareForBuild(BuildOptions options, BuildTarget target) => 
            PostProcessAndroidPlayer.PrepareForBuild(options, target);

        public override bool SupportsInstallInBuildFolder() => 
            true;
    }
}

