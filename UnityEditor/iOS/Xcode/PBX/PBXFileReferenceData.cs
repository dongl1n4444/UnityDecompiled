namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.IO;
    using UnityEditor.iOS.Xcode;

    internal class PBXFileReferenceData : PBXObjectData
    {
        private string m_ExplicitFileType = null;
        private string m_LastKnownFileType = null;
        private string m_Path = null;
        public string name;
        public PBXSourceTree tree;

        public static PBXFileReferenceData CreateFromFile(string path, string projectFileName, PBXSourceTree tree)
        {
            string str = PBXGUID.Generate();
            PBXFileReferenceData data = new PBXFileReferenceData();
            data.SetPropertyString("isa", "PBXFileReference");
            data.guid = str;
            data.path = path;
            data.name = projectFileName;
            data.tree = tree;
            return data;
        }

        public static PBXFileReferenceData CreateFromFolderReference(string path, string projectFileName, PBXSourceTree tree)
        {
            PBXFileReferenceData data = CreateFromFile(path, projectFileName, tree);
            data.m_LastKnownFileType = "folder";
            return data;
        }

        public override void UpdateProps()
        {
            string ext = null;
            if (this.m_ExplicitFileType != null)
            {
                base.SetPropertyString("explicitFileType", this.m_ExplicitFileType);
            }
            else if (this.m_LastKnownFileType != null)
            {
                base.SetPropertyString("lastKnownFileType", this.m_LastKnownFileType);
            }
            else
            {
                if (this.name != null)
                {
                    ext = Path.GetExtension(this.name);
                }
                else if (this.m_Path != null)
                {
                    ext = Path.GetExtension(this.m_Path);
                }
                if (ext != null)
                {
                    if (FileTypeUtils.IsFileTypeExplicit(ext))
                    {
                        base.SetPropertyString("explicitFileType", FileTypeUtils.GetTypeName(ext));
                    }
                    else
                    {
                        base.SetPropertyString("lastKnownFileType", FileTypeUtils.GetTypeName(ext));
                    }
                }
            }
            if (this.m_Path == this.name)
            {
                base.SetPropertyString("name", null);
            }
            else
            {
                base.SetPropertyString("name", this.name);
            }
            if (this.m_Path == null)
            {
                base.SetPropertyString("path", "");
            }
            else
            {
                base.SetPropertyString("path", this.m_Path);
            }
            base.SetPropertyString("sourceTree", FileTypeUtils.SourceTreeDesc(this.tree));
        }

        public override void UpdateVars()
        {
            this.name = base.GetPropertyString("name");
            this.m_Path = base.GetPropertyString("path");
            if (this.name == null)
            {
                this.name = this.m_Path;
            }
            if (this.m_Path == null)
            {
                this.m_Path = "";
            }
            this.tree = FileTypeUtils.ParseSourceTree(base.GetPropertyString("sourceTree"));
            this.m_ExplicitFileType = base.GetPropertyString("explicitFileType");
            this.m_LastKnownFileType = base.GetPropertyString("lastKnownFileType");
        }

        public bool isFolderReference =>
            ((this.m_LastKnownFileType != null) && (this.m_LastKnownFileType == "folder"));

        public string path
        {
            get => 
                this.m_Path;
            set
            {
                this.m_ExplicitFileType = null;
                this.m_LastKnownFileType = null;
                this.m_Path = value;
            }
        }

        internal override bool shouldCompact =>
            true;
    }
}

