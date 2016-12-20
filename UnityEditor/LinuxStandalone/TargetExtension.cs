namespace UnityEditor.LinuxStandalone
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        public override IBuildPostprocessor CreateBuildPostprocessor()
        {
            return new LinuxDesktopStandalonePostProcessor();
        }

        public override IBuildWindowExtension CreateBuildWindowExtension()
        {
            return new DesktopStandaloneBuildWindowExtension();
        }

        public override IPluginImporterExtension CreatePluginImporterExtension()
        {
            return new DesktopPluginImporterExtension();
        }

        public override IScriptingImplementations CreateScriptingImplementations()
        {
            return new DesktopStandalonePostProcessor.ScriptingImplementations();
        }

        public override string JamTarget
        {
            get
            {
                return "LinuxStandaloneEditorExtensions";
            }
        }

        public override string TargetName
        {
            get
            {
                return "LinuxStandalone";
            }
        }
    }
}

