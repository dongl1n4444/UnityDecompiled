namespace UnityEditor.WindowsStandalone
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        public override IBuildPostprocessor CreateBuildPostprocessor()
        {
            return new WindowsDesktopStandalonePostProcessor();
        }

        public override IBuildWindowExtension CreateBuildWindowExtension()
        {
            return new WindowsStandaloneBuildWindowExtension();
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
                return "WindowsStandaloneEditorExtensions";
            }
        }

        public override string TargetName
        {
            get
            {
                return "WindowsStandalone";
            }
        }
    }
}

