namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;

    internal class XCConfigurationListData : PBXObjectData
    {
        public GUIDList buildConfigs;
        private static PropertyCommentChecker checkerData;

        static XCConfigurationListData()
        {
            string[] props = new string[] { "buildConfigurations/*" };
            checkerData = new PropertyCommentChecker(props);
        }

        public static XCConfigurationListData Create()
        {
            XCConfigurationListData data = new XCConfigurationListData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "XCConfigurationList");
            data.buildConfigs = new GUIDList();
            data.SetPropertyString("defaultConfigurationIsVisible", "0");
            return data;
        }

        public override void UpdateProps()
        {
            base.SetPropertyList("buildConfigurations", (List<string>) this.buildConfigs);
        }

        public override void UpdateVars()
        {
            this.buildConfigs = base.GetPropertyList("buildConfigurations");
        }

        internal override PropertyCommentChecker checker =>
            checkerData;
    }
}

