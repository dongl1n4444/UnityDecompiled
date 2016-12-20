namespace UnityEditor.iOS.Xcode.PBX
{
    using System;

    internal class PBXReferenceProxyData : PBXObjectData
    {
        private static PropertyCommentChecker checkerData;

        static PBXReferenceProxyData()
        {
            string[] props = new string[] { "remoteRef/*" };
            checkerData = new PropertyCommentChecker(props);
        }

        public static PBXReferenceProxyData Create(string path, string fileType, string remoteRef, string sourceTree)
        {
            PBXReferenceProxyData data = new PBXReferenceProxyData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "PBXReferenceProxy");
            data.SetPropertyString("path", path);
            data.SetPropertyString("fileType", fileType);
            data.SetPropertyString("remoteRef", remoteRef);
            data.SetPropertyString("sourceTree", sourceTree);
            return data;
        }

        internal override PropertyCommentChecker checker
        {
            get
            {
                return checkerData;
            }
        }

        public string path
        {
            get
            {
                return base.GetPropertyString("path");
            }
        }
    }
}

