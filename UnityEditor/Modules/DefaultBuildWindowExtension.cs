namespace UnityEditor.Modules
{
    using System;

    internal class DefaultBuildWindowExtension : IBuildWindowExtension
    {
        public virtual bool EnabledBuildAndRunButton() => 
            true;

        public virtual bool EnabledBuildButton() => 
            true;

        public virtual bool ShouldDrawDevelopmentPlayerCheckbox() => 
            true;

        public virtual bool ShouldDrawExplicitDivideByZeroCheckbox() => 
            false;

        public virtual bool ShouldDrawExplicitNullCheckbox() => 
            false;

        public virtual bool ShouldDrawForceOptimizeScriptsCheckbox() => 
            false;

        public virtual bool ShouldDrawProfilerCheckbox() => 
            true;

        public virtual bool ShouldDrawScriptDebuggingCheckbox() => 
            true;

        public virtual void ShowInternalPlatformBuildOptions()
        {
        }

        public virtual void ShowPlatformBuildOptions()
        {
        }
    }
}

