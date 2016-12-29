namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.iOS.Xcode;

    internal class PBXGroupData : PBXObjectData
    {
        private static PropertyCommentChecker checkerData;
        public GUIDList children;
        public string name;
        public string path;
        public PBXSourceTree tree;

        static PBXGroupData()
        {
            string[] props = new string[] { "children/*" };
            checkerData = new PropertyCommentChecker(props);
        }

        public static PBXGroupData Create(string name, string path, PBXSourceTree tree)
        {
            if (name.Contains("/"))
            {
                throw new Exception("Group name must not contain '/'");
            }
            PBXGroupData data = new PBXGroupData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "PBXGroup");
            data.name = name;
            data.path = path;
            data.tree = PBXSourceTree.Group;
            data.children = new GUIDList();
            return data;
        }

        public static PBXGroupData CreateRelative(string name) => 
            Create(name, name, PBXSourceTree.Group);

        public override void UpdateProps()
        {
            base.SetPropertyList("children", (List<string>) this.children);
            if (this.name == this.path)
            {
                base.SetPropertyString("name", null);
            }
            else
            {
                base.SetPropertyString("name", this.name);
            }
            if (this.path == "")
            {
                base.SetPropertyString("path", null);
            }
            else
            {
                base.SetPropertyString("path", this.path);
            }
            base.SetPropertyString("sourceTree", FileTypeUtils.SourceTreeDesc(this.tree));
        }

        public override void UpdateVars()
        {
            this.children = base.GetPropertyList("children");
            this.path = base.GetPropertyString("path");
            this.name = base.GetPropertyString("name");
            if (this.name == null)
            {
                this.name = this.path;
            }
            if (this.path == null)
            {
                this.path = "";
            }
            this.tree = FileTypeUtils.ParseSourceTree(base.GetPropertyString("sourceTree"));
        }

        internal override PropertyCommentChecker checker =>
            checkerData;
    }
}

