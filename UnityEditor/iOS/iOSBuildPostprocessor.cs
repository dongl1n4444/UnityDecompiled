namespace UnityEditor.iOS
{
    using System;
    using UnityEditor;
    using UnityEditor.iOS.Il2Cpp;
    using UnityEditor.Modules;
    using UnityEditorInternal;

    internal class iOSBuildPostprocessor : DefaultBuildPostprocessor
    {
        private PostProcessorSettings settings;

        public iOSBuildPostprocessor(PostProcessorSettings settings)
        {
            this.settings = settings;
        }

        public IIl2CppPlatformProvider GetPlatformProvider(BuildTarget target, bool isDevelopmentBuild, string dataDirectory) => 
            new iOSIl2CppPlatformProvider(target, isDevelopmentBuild, dataDirectory);

        public override void LaunchPlayer(BuildLaunchPlayerArgs args)
        {
            PostProcessiPhonePlayer.Launch(args);
        }

        public override void PostProcess(BuildPostProcessArgs args)
        {
            PostProcessiPhonePlayer.PostProcess(this.settings, args);
        }

        public override bool SupportsInstallInBuildFolder() => 
            true;
    }
}

