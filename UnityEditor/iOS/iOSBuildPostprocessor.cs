namespace UnityEditor.iOS
{
    using System;
    using UnityEditor.Modules;

    internal class iOSBuildPostprocessor : DefaultBuildPostprocessor
    {
        private PostProcessorSettings settings;

        public iOSBuildPostprocessor(PostProcessorSettings settings)
        {
            this.settings = settings;
        }

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

