namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class PBXBuildFileData : PBXObjectData
    {
        [CompilerGenerated]
        private static Predicate<PBXElement> <>f__am$cache0;
        public List<string> assetTags;
        private static PropertyCommentChecker checkerData;
        public string compileFlags;
        public string fileRef;
        public bool weak;

        static PBXBuildFileData()
        {
            string[] props = new string[] { "fileRef/*" };
            checkerData = new PropertyCommentChecker(props);
        }

        public static PBXBuildFileData CreateFromFile(string fileRefGUID, bool weak, string compileFlags)
        {
            PBXBuildFileData data = new PBXBuildFileData {
                guid = PBXGUID.Generate()
            };
            data.SetPropertyString("isa", "PBXBuildFile");
            data.fileRef = fileRefGUID;
            data.compileFlags = compileFlags;
            data.weak = weak;
            data.assetTags = new List<string>();
            return data;
        }

        public override void UpdateProps()
        {
            base.SetPropertyString("fileRef", this.fileRef);
            PBXElementDict dict = null;
            if (base.m_Properties.Contains("settings"))
            {
                dict = base.m_Properties["settings"].AsDict();
            }
            if ((this.compileFlags != null) && (this.compileFlags != ""))
            {
                if (dict == null)
                {
                    dict = base.m_Properties.CreateDict("settings");
                }
                dict.SetString("COMPILER_FLAGS", this.compileFlags);
            }
            else if (dict != null)
            {
                dict.Remove("COMPILER_FLAGS");
            }
            if (this.weak)
            {
                if (dict == null)
                {
                    dict = base.m_Properties.CreateDict("settings");
                }
                PBXElementArray array = null;
                if (dict.Contains("ATTRIBUTES"))
                {
                    array = dict["ATTRIBUTES"].AsArray();
                }
                else
                {
                    array = dict.CreateArray("ATTRIBUTES");
                }
                bool flag = false;
                foreach (PBXElement element in array.values)
                {
                    if ((element is PBXElementString) && (element.AsString() == "Weak"))
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    array.AddString("Weak");
                }
            }
            else if ((dict != null) && dict.Contains("ATTRIBUTES"))
            {
                PBXElementArray array2 = dict["ATTRIBUTES"].AsArray();
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = el => (el is PBXElementString) && (el.AsString() == "Weak");
                }
                array2.values.RemoveAll(<>f__am$cache0);
                if (array2.values.Count == 0)
                {
                    dict.Remove("ATTRIBUTES");
                }
            }
            if (this.assetTags.Count > 0)
            {
                if (dict == null)
                {
                    dict = base.m_Properties.CreateDict("settings");
                }
                PBXElementArray array3 = dict.CreateArray("ASSET_TAGS");
                foreach (string str in this.assetTags)
                {
                    array3.AddString(str);
                }
            }
            else if (dict != null)
            {
                dict.Remove("ASSET_TAGS");
            }
            if ((dict != null) && (dict.values.Count == 0))
            {
                base.m_Properties.Remove("settings");
            }
        }

        public override void UpdateVars()
        {
            this.fileRef = base.GetPropertyString("fileRef");
            this.compileFlags = null;
            this.weak = false;
            this.assetTags = new List<string>();
            if (base.m_Properties.Contains("settings"))
            {
                PBXElementDict dict = base.m_Properties["settings"].AsDict();
                if (dict.Contains("COMPILER_FLAGS"))
                {
                    this.compileFlags = dict["COMPILER_FLAGS"].AsString();
                }
                if (dict.Contains("ATTRIBUTES"))
                {
                    PBXElementArray array = dict["ATTRIBUTES"].AsArray();
                    foreach (PBXElement element in array.values)
                    {
                        if ((element is PBXElementString) && (element.AsString() == "Weak"))
                        {
                            this.weak = true;
                        }
                    }
                }
                if (dict.Contains("ASSET_TAGS"))
                {
                    PBXElementArray array2 = dict["ASSET_TAGS"].AsArray();
                    foreach (PBXElement element2 in array2.values)
                    {
                        this.assetTags.Add(element2.AsString());
                    }
                }
            }
        }

        internal override PropertyCommentChecker checker
        {
            get
            {
                return checkerData;
            }
        }

        internal override bool shouldCompact
        {
            get
            {
                return true;
            }
        }
    }
}

