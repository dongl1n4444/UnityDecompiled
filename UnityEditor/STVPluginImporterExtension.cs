namespace UnityEditor
{
    using System;
    using UnityEditor.Modules;

    internal class STVPluginImporterExtension : DefaultPluginImporterExtension
    {
        public STVPluginImporterExtension() : base(GetProperties())
        {
        }

        public override bool CheckFileCollisions(string buildTargetName)
        {
            return false;
        }

        private static DefaultPluginImporterExtension.Property[] GetProperties()
        {
            return new DefaultPluginImporterExtension.Property[] { new DefaultPluginImporterExtension.Property("Model", STVPlugin.modelTag, STVPlugin.TVTargets.STANDARD_15, BuildPipeline.GetBuildTargetName(BuildTarget.SamsungTV)) };
        }
    }
}

