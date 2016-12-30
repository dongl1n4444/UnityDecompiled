namespace UnityScript.MonoDevelop.ProjectModel
{
    using MonoDevelop.Core;
    using MonoDevelop.Projects;
    using MonoDevelop.Projects.Formats.MSBuild;
    using System.Reflection;

    public class UnityScriptProjectServiceExtension : ProjectServiceExtension
    {
        protected override BuildResult Build(IProgressMonitor monitor, IBuildTarget item, ConfigurationSelector configuration)
        {
            DotNetAssemblyProject project = item as DotNetAssemblyProject;
            if ((project != null) && (project.get_LanguageBinding() is UnityScriptLanguageBinding))
            {
                MSBuildProjectHandler msBuildProjectHandler = project.get_ResourceHandler() as MSBuildProjectHandler;
                if (msBuildProjectHandler != null)
                {
                    PropertyInfo useMSBuildEngineByDefault = msBuildProjectHandler.GetType().GetProperty("UseMSBuildEngineByDefault", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (useMSBuildEngineByDefault == null)
                    {
                        BuildResult result = new BuildResult();
                        result.AddError("Could not find UseMSBuildEngineByDefault property in MSBuildProjectHandler. Building of UnityScript projects is not supported.");
                        return result;
                    }
                    useMSBuildEngineByDefault.SetValue(msBuildProjectHandler, false);
                }
            }
            return base.Build(monitor, item, configuration);
        }
    }
}

