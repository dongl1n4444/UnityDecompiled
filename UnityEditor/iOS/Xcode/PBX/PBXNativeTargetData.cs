namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;

    internal class PBXNativeTargetData : PBXObjectData
    {
        public string buildConfigList;
        private static PropertyCommentChecker checkerData;
        public GUIDList dependencies;
        public string name;
        public GUIDList phases;

        static PBXNativeTargetData()
        {
            string[] props = new string[] { "buildPhases/*", "buildRules/*", "dependencies/*", "productReference/*", "buildConfigurationList/*" };
            checkerData = new PropertyCommentChecker(props);
        }

        public static PBXNativeTargetData Create(string name, string productRef, string productType, string buildConfigList)
        {
            PBXNativeTargetData data = new PBXNativeTargetData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "PBXNativeTarget");
            data.buildConfigList = buildConfigList;
            data.phases = new GUIDList();
            data.SetPropertyList("buildRules", new List<string>());
            data.dependencies = new GUIDList();
            data.name = name;
            data.SetPropertyString("productName", name);
            data.SetPropertyString("productReference", productRef);
            data.SetPropertyString("productType", productType);
            return data;
        }

        public override void UpdateProps()
        {
            base.SetPropertyString("buildConfigurationList", this.buildConfigList);
            base.SetPropertyString("name", this.name);
            base.SetPropertyList("buildPhases", (List<string>) this.phases);
            base.SetPropertyList("dependencies", (List<string>) this.dependencies);
        }

        public override void UpdateVars()
        {
            this.buildConfigList = base.GetPropertyString("buildConfigurationList");
            this.name = base.GetPropertyString("name");
            this.phases = base.GetPropertyList("buildPhases");
            this.dependencies = base.GetPropertyList("dependencies");
        }

        internal override PropertyCommentChecker checker =>
            checkerData;
    }
}

