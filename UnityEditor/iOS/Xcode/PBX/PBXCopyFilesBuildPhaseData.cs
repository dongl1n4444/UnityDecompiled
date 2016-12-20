namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;

    internal class PBXCopyFilesBuildPhaseData : FileGUIDListBase
    {
        private static PropertyCommentChecker checkerData;
        public string name;

        static PBXCopyFilesBuildPhaseData()
        {
            string[] props = new string[] { "files/*" };
            checkerData = new PropertyCommentChecker(props);
        }

        public static PBXCopyFilesBuildPhaseData Create(string name, string subfolderSpec)
        {
            PBXCopyFilesBuildPhaseData data = new PBXCopyFilesBuildPhaseData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "PBXCopyFilesBuildPhase");
            data.SetPropertyString("buildActionMask", "2147483647");
            data.SetPropertyString("dstPath", "");
            data.SetPropertyString("dstSubfolderSpec", subfolderSpec);
            data.files = new List<string>();
            data.SetPropertyString("runOnlyForDeploymentPostprocessing", "0");
            data.name = name;
            return data;
        }

        public override void UpdateProps()
        {
            base.SetPropertyList("files", (List<string>) base.files);
            base.SetPropertyString("name", this.name);
        }

        public override void UpdateVars()
        {
            base.files = base.GetPropertyList("files");
            this.name = base.GetPropertyString("name");
        }

        internal override PropertyCommentChecker checker
        {
            get
            {
                return checkerData;
            }
        }
    }
}

