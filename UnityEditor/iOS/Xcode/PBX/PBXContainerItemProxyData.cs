namespace UnityEditor.iOS.Xcode.PBX
{
    using System;

    internal class PBXContainerItemProxyData : PBXObjectData
    {
        private static PropertyCommentChecker checkerData;

        static PBXContainerItemProxyData()
        {
            string[] props = new string[] { "containerPortal/*" };
            checkerData = new PropertyCommentChecker(props);
        }

        public static PBXContainerItemProxyData Create(string containerRef, string proxyType, string remoteGlobalGUID, string remoteInfo)
        {
            PBXContainerItemProxyData data = new PBXContainerItemProxyData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "PBXContainerItemProxy");
            data.SetPropertyString("containerPortal", containerRef);
            data.SetPropertyString("proxyType", proxyType);
            data.SetPropertyString("remoteGlobalIDString", remoteGlobalGUID);
            data.SetPropertyString("remoteInfo", remoteInfo);
            return data;
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

