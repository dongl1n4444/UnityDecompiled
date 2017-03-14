namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using UnityEditor;

    internal class XboxOnePlatformSetup : IPlatformSetup
    {
        private ScriptingImplementation oldScriptingImplementation;
        private string oldXboxOneAdditionalDebugPorts;
        private XboxOneDeployMethod oldXboxOneDeployMethod;

        public void CleanUp()
        {
            EditorUserBuildSettings.xboxOneDeployMethod = this.oldXboxOneDeployMethod;
            EditorUserBuildSettings.xboxOneAdditionalDebugPorts = this.oldXboxOneAdditionalDebugPorts;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.XboxOne, this.oldScriptingImplementation);
        }

        public void Setup()
        {
            this.oldXboxOneDeployMethod = EditorUserBuildSettings.xboxOneDeployMethod;
            this.oldXboxOneAdditionalDebugPorts = EditorUserBuildSettings.xboxOneAdditionalDebugPorts;
            this.oldScriptingImplementation = PlayerSettings.GetScriptingBackend(BuildTargetGroup.XboxOne);
            EditorUserBuildSettings.xboxOneDeployMethod = XboxOneDeployMethod.Package;
            EditorUserBuildSettings.xboxOneAdditionalDebugPorts = "34999";
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.XboxOne, ScriptingImplementation.IL2CPP);
        }
    }
}

