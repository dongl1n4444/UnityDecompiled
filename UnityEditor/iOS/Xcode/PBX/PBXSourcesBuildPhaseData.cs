namespace UnityEditor.iOS.Xcode.PBX
{
    using System.Collections.Generic;

    internal class PBXSourcesBuildPhaseData : FileGUIDListBase
    {
        public static PBXSourcesBuildPhaseData Create()
        {
            PBXSourcesBuildPhaseData data = new PBXSourcesBuildPhaseData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "PBXSourcesBuildPhase");
            data.SetPropertyString("buildActionMask", "2147483647");
            data.files = new List<string>();
            data.SetPropertyString("runOnlyForDeploymentPostprocessing", "0");
            return data;
        }
    }
}

