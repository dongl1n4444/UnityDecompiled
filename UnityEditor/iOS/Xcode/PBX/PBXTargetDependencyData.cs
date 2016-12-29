namespace UnityEditor.iOS.Xcode.PBX
{
    using System;

    internal class PBXTargetDependencyData : PBXObjectData
    {
        private static PropertyCommentChecker checkerData;

        static PBXTargetDependencyData()
        {
            string[] props = new string[] { "target/*", "targetProxy/*" };
            checkerData = new PropertyCommentChecker(props);
        }

        public static PBXTargetDependencyData Create(string target, string targetProxy)
        {
            PBXTargetDependencyData data = new PBXTargetDependencyData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "PBXTargetDependency");
            data.SetPropertyString("target", target);
            data.SetPropertyString("targetProxy", targetProxy);
            return data;
        }

        internal override PropertyCommentChecker checker =>
            checkerData;
    }
}

