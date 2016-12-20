namespace UnityEditor.iOS
{
    using System;
    using System.Runtime.InteropServices;

    internal class XCodeUtils
    {
        private const string nativeDll = "__Internal";

        [DllImport("__Internal")]
        internal static extern void BuildXCodeFromScript(string xcodeProjectPath);
        [DllImport("__Internal")]
        internal static extern bool CheckXCodeCompatibleWithPlugin(string pluginPath);
        [DllImport("__Internal")]
        internal static extern bool CheckXCodeInstalled();
        [DllImport("__Internal")]
        internal static extern void CleanXCodeFromScript(string xcodeProjectPath);
        [DllImport("__Internal")]
        internal static extern void CloseAllXCodeProjectsWithPath(string xcodeProjectPath);
        [DllImport("__Internal")]
        internal static extern int GetXcodeMajorVersionNumber();
        [DllImport("__Internal")]
        internal static extern bool InitializeScriptingXcodeApplication();
        [DllImport("__Internal")]
        internal static extern void LaunchXCode();
        [DllImport("__Internal")]
        internal static extern void OpenXCodeProjectWithPath(string xcodeProjectPath);
        [DllImport("__Internal")]
        internal static extern void RunXCodeFromScript(string xcodeProjectPath);
        [DllImport("__Internal")]
        internal static extern void TerminateXCode();
        [DllImport("__Internal")]
        internal static extern void TerminateXCodeFromScript(string xcodeProjectPath);
    }
}

