namespace UnityEditor.iOS.Xcode.PBX
{
    using System.Collections.Generic;

    internal class PBXResourcesBuildPhaseData : FileGUIDListBase
    {
        public static PBXResourcesBuildPhaseData Create()
        {
            PBXResourcesBuildPhaseData data = new PBXResourcesBuildPhaseData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "PBXResourcesBuildPhase");
            data.SetPropertyString("buildActionMask", "2147483647");
            data.files = new List<string>();
            data.SetPropertyString("runOnlyForDeploymentPostprocessing", "0");
            return data;
        }
    }
}

