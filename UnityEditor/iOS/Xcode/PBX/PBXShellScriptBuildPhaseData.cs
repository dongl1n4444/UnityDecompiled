namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;

    internal class PBXShellScriptBuildPhaseData : FileGUIDListBase
    {
        public string name;
        public string shellPath;
        public string shellScript;

        public static PBXShellScriptBuildPhaseData Create(string name, string shellPath, string shellScript)
        {
            PBXShellScriptBuildPhaseData data = new PBXShellScriptBuildPhaseData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "PBXShellScriptBuildPhase");
            data.SetPropertyString("buildActionMask", "2147483647");
            data.files = new List<string>();
            data.SetPropertyString("runOnlyForDeploymentPostprocessing", "0");
            data.name = name;
            data.shellPath = shellPath;
            data.shellScript = shellScript;
            return data;
        }

        public override void UpdateProps()
        {
            base.UpdateProps();
            base.SetPropertyString("name", this.name);
            base.SetPropertyString("shellPath", this.shellPath);
            base.SetPropertyString("shellScript", this.shellScript);
        }

        public override void UpdateVars()
        {
            base.UpdateVars();
            this.name = base.GetPropertyString("name");
            this.shellPath = base.GetPropertyString("shellPath");
            this.shellScript = base.GetPropertyString("shellScript");
        }
    }
}

