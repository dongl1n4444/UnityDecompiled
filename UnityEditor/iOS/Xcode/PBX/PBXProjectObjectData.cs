namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;

    internal class PBXProjectObjectData : PBXObjectData
    {
        public string buildConfigList;
        private static PropertyCommentChecker checkerData;
        public List<string> knownAssetTags = new List<string>();
        public List<ProjectReference> projectReferences = new List<ProjectReference>();
        public List<string> targets = new List<string>();

        static PBXProjectObjectData()
        {
            string[] props = new string[] { "buildConfigurationList/*", "mainGroup/*", "projectReferences/*/ProductGroup/*", "projectReferences/*/ProjectRef/*", "targets/*" };
            checkerData = new PropertyCommentChecker(props);
        }

        public void AddReference(string productGroup, string projectRef)
        {
            this.projectReferences.Add(ProjectReference.Create(productGroup, projectRef));
        }

        public override void UpdateProps()
        {
            base.m_Properties.values.Remove("projectReferences");
            if (this.projectReferences.Count > 0)
            {
                PBXElementArray array = base.m_Properties.CreateArray("projectReferences");
                foreach (ProjectReference reference in this.projectReferences)
                {
                    PBXElementDict dict = array.AddDict();
                    dict.SetString("ProductGroup", reference.group);
                    dict.SetString("ProjectRef", reference.projectRef);
                }
            }
            base.SetPropertyList("targets", this.targets);
            base.SetPropertyString("buildConfigurationList", this.buildConfigList);
            if (this.knownAssetTags.Count > 0)
            {
                PBXElementDict dict2;
                if (base.m_Properties.Contains("attributes"))
                {
                    dict2 = base.m_Properties["attributes"].AsDict();
                }
                else
                {
                    dict2 = base.m_Properties.CreateDict("attributes");
                }
                PBXElementArray array2 = dict2.CreateArray("knownAssetTags");
                foreach (string str in this.knownAssetTags)
                {
                    array2.AddString(str);
                }
            }
        }

        public override void UpdateVars()
        {
            this.projectReferences = new List<ProjectReference>();
            if (base.m_Properties.Contains("projectReferences"))
            {
                PBXElementArray array = base.m_Properties["projectReferences"].AsArray();
                foreach (PBXElement element in array.values)
                {
                    PBXElementDict dict = element.AsDict();
                    if (dict.Contains("ProductGroup") && dict.Contains("ProjectRef"))
                    {
                        string group = dict["ProductGroup"].AsString();
                        string projectRef = dict["ProjectRef"].AsString();
                        this.projectReferences.Add(ProjectReference.Create(group, projectRef));
                    }
                }
            }
            this.targets = base.GetPropertyList("targets");
            this.buildConfigList = base.GetPropertyString("buildConfigurationList");
            this.knownAssetTags = new List<string>();
            if (base.m_Properties.Contains("attributes"))
            {
                PBXElementDict dict2 = base.m_Properties["attributes"].AsDict();
                if (dict2.Contains("knownAssetTags"))
                {
                    PBXElementArray array2 = dict2["knownAssetTags"].AsArray();
                    foreach (PBXElement element2 in array2.values)
                    {
                        this.knownAssetTags.Add(element2.AsString());
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

        public string mainGroup
        {
            get
            {
                return base.GetPropertyString("mainGroup");
            }
        }
    }
}

