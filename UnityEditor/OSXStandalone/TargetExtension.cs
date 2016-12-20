namespace UnityEditor.OSXStandalone
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        public override IBuildPostprocessor CreateBuildPostprocessor()
        {
            return new OSXDesktopStandalonePostProcessor();
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
                return "OSXStandaloneEditorExtensions";
            }
        }

        public override string TargetName
        {
            get
            {
                return "OSXStandalone";
            }
        }
    }
}

