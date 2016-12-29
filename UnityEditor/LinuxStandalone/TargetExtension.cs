namespace UnityEditor.LinuxStandalone
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        public override IBuildPostprocessor CreateBuildPostprocessor() => 
            new LinuxDesktopStandalonePostProcessor();

        public override IBuildWindowExtension CreateBuildWindowExtension() => 
            new DesktopStandaloneBuildWindowExtension();

        public override IPluginImporterExtension CreatePluginImporterExtension() => 
            new DesktopPluginImporterExtension();

        public override IScriptingImplementations CreateScriptingImplementations() => 
            new DesktopStandalonePostProcessor.ScriptingImplementations();

        public override string JamTarget =>
            "LinuxStandaloneEditorExtensions";

        public override string TargetName =>
            "LinuxStandalone";
    }
}

