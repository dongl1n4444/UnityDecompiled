namespace UnityEditor.SamsungTV
{
    using System;
    using UnityEditor.Modules;

    internal class SamsungTVBuildPostprocessor : DefaultBuildPostprocessor
    {
        public override void PostProcess(BuildPostProcessArgs args)
        {
            PostProcessSamsungTVPlayer.PostProcess(args.target, args.stagingAreaData, args.stagingArea, args.stagingAreaDataManaged, args.playerPackage, args.installPath, args.companyName, args.productName, args.options);
        }
    }
}

