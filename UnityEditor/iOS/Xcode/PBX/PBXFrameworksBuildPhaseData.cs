namespace UnityEditor.iOS.Xcode.PBX
{
    using System.Collections.Generic;

    internal class PBXFrameworksBuildPhaseData : FileGUIDListBase
    {
        public static PBXFrameworksBuildPhaseData Create()
        {
            PBXFrameworksBuildPhaseData data = new PBXFrameworksBuildPhaseData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "PBXFrameworksBuildPhase");
            data.SetPropertyString("buildActionMask", "2147483647");
            data.files = new List<string>();
            data.SetPropertyString("runOnlyForDeploymentPostprocessing", "0");
            return data;
        }
    }
}

