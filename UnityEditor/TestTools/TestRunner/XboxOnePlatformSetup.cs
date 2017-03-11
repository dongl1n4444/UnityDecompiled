namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using UnityEditor;

    internal class XboxOnePlatformSetup : IPlatformSetup
    {
        public void CleanUp()
        {
        }

        public void Setup()
        {
            EditorUserBuildSettings.xboxOneDeployMethod = XboxOneDeployMethod.Package;
            EditorUserBuildSettings.xboxOneAdditionalDebugPorts = "34999";
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.XboxOne, ScriptingImplementation.IL2CPP);
        }
    }
}

