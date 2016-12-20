namespace UnityEditor.iOS
{
    using System;

    public interface IXcodeController
    {
        void BuildProject(string projectPath);
        void CleanProject(string projectPath);
        void CloseAllOpenUnityProjects(string buildToolsDir, string projectPath);
        string GetMobileDeviceList(string projectPath);
        bool InitializeXcodeApplication(string buildToolsDir);
        void Log(string msg, params object[] param);
        void OpenProject(string projectPath);
        void RunProject(string projectPath);
        void SelectDeviceScheme(string projectPath);
        void SelectSimulatorScheme(string version);
    }
}

