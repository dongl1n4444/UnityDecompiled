namespace UnityEditor.Tizen
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class TizenBuildPostprocessor : DefaultBuildPostprocessor
    {
        public override string GetExtension(BuildTarget target, BuildOptions options) => 
            "tpk";

        public override void PostProcess(BuildPostProcessArgs args)
        {
            PostProcessTizenPlayer.PostProcess(args.target, args.stagingAreaData, args.stagingArea, args.stagingAreaDataManaged, args.playerPackage, args.installPath, args.companyName, args.productName, args.options);
        }
    }
}

