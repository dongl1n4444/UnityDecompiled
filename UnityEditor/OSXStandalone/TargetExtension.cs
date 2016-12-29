namespace UnityEditor.OSXStandalone
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        public override IBuildPostprocessor CreateBuildPostprocessor() => 
            new OSXDesktopStandalonePostProcessor();

        public override IBuildWindowExtension CreateBuildWindowExtension() => 
            new DesktopStandaloneBuildWindowExtension();

        public override IPluginImporterExtension CreatePluginImporterExtension() => 
            new DesktopPluginImporterExtension();

        public override IScriptingImplementations CreateScriptingImplementations() => 
            new DesktopStandalonePostProcessor.ScriptingImplementations();

        public override string JamTarget =>
            "OSXStandaloneEditorExtensions";

        public override string TargetName =>
            "OSXStandalone";
    }
}

