namespace UnityEditor.WebGL
{
    using System;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        private static WebGlBuildWindowExtension s_BuildWindow;

        public override IBuildPostprocessor CreateBuildPostprocessor()
        {
            return new WebGlBuildPostprocessor();
        }

        public override IBuildWindowExtension CreateBuildWindowExtension()
        {
            IBuildWindowExtension extension;
            if (s_BuildWindow != null)
            {
                extension = s_BuildWindow;
            }
            else
            {
                extension = s_BuildWindow = new WebGlBuildWindowExtension();
            }
            return extension;
        }

        public override ISettingEditorExtension CreateSettingsEditorExtension()
        {
            return new WebGlSettingsExtension();
        }

        public override IUserAssembliesValidator CreateUserAssembliesValidatorExtension()
        {
            return null;
        }

        public override string JamTarget
        {
            get
            {
                return "WebGLExtensions";
            }
        }

        public override string TargetName
        {
            get
            {
                return "WebGL";
            }
        }
    }
}

