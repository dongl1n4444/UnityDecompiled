namespace UnityEditor.iOS
{
    using System;

    public class XcodeScriptingController : IDisposable, IXcodeController
    {
        public void BuildProject(string projectPath)
        {
            XCodeUtils.BuildXCodeFromScript(projectPath);
        }

        public void CleanProject(string projectPath)
        {
            XCodeUtils.CleanXCodeFromScript(projectPath);
        }

        public void CloseAllOpenUnityProjects(string buildToolsDir, string projectPath)
        {
            XCodeUtils.CloseAllXCodeProjectsWithPath(projectPath);
        }

        public void Dispose()
        {
        }

        public string GetMobileDeviceList(string projectPath)
        {
            return "";
        }

        public bool InitializeXcodeApplication(string buildToolsDir)
        {
            return XCodeUtils.InitializeScriptingXcodeApplication();
        }

        public void Log(string msg, params object[] param)
        {
            Console.WriteLine(msg, param);
        }

        public void OpenProject(string projectPath)
        {
            XCodeUtils.OpenXCodeProjectWithPath(projectPath);
        }

        public void RunProject(string projectPath)
        {
            XCodeUtils.RunXCodeFromScript(projectPath);
        }

        public void SelectDeviceScheme(string projectPath)
        {
        }

        public void SelectSimulatorScheme(string version)
        {
        }
    }
}

