namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class PluginsHelper
    {
        public static bool CheckFileCollisions(BuildTarget buildTarget)
        {
            IPluginImporterExtension pluginImporterExtension = null;
            if (ModuleManager.IsPlatformSupported(buildTarget))
            {
                pluginImporterExtension = ModuleManager.GetPluginImporterExtension(buildTarget);
            }
            return pluginImporterExtension?.CheckFileCollisions(BuildPipeline.GetBuildTargetName(buildTarget));
        }
    }
}

