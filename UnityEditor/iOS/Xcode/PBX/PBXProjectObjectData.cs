namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.iOS.Xcode;

    internal class PBXProjectObjectData : PBXObjectData
    {
        public string buildConfigList;
        public List<PBXCapabilityType.TargetCapabilityPair> capabilities = new List<PBXCapabilityType.TargetCapabilityPair>();
        private static PropertyCommentChecker checkerData;
        public string entitlementsFile;
        public List<string> knownAssetTags = new List<string>();
        public List<ProjectReference> projectReferences = new List<ProjectReference>();
        public List<string> targets = new List<string>();
        public Dictionary<string, string> teamIDs = new Dictionary<string, string>();

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
            foreach (PBXCapabilityType.TargetCapabilityPair pair in this.capabilities)
            {
                PBXElementDict dict3 = !base.m_Properties.Contains("attributes") ? base.m_Properties.CreateDict("attributes") : base.m_Properties["attributes"].AsDict();
                PBXElementDict dict4 = !dict3.Contains("TargetAttributes") ? dict3.CreateDict("TargetAttributes") : dict3["TargetAttributes"].AsDict();
                PBXElementDict dict5 = !dict4.Contains(pair.targetGuid) ? dict4.CreateDict(pair.targetGuid) : dict4[pair.targetGuid].AsDict();
                PBXElementDict dict6 = !dict5.Contains("SystemCapabilities") ? dict5.CreateDict("SystemCapabilities") : dict5["SystemCapabilities"].AsDict();
                string id = pair.capability.id;
                (!dict6.Contains(id) ? dict6.CreateDict(id) : dict6[id].AsDict()).SetString("enabled", "1");
            }
            foreach (KeyValuePair<string, string> pair2 in this.teamIDs)
            {
                PBXElementDict dict8 = !base.m_Properties.Contains("attributes") ? base.m_Properties.CreateDict("attributes") : base.m_Properties["attributes"].AsDict();
                PBXElementDict dict9 = !dict8.Contains("TargetAttributes") ? dict8.CreateDict("TargetAttributes") : dict8["TargetAttributes"].AsDict();
                (!dict9.Contains(pair2.Key) ? dict9.CreateDict(pair2.Key) : dict9[pair2.Key].AsDict()).SetString("DevelopmentTeam", pair2.Value);
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
                this.capabilities = new List<PBXCapabilityType.TargetCapabilityPair>();
                this.teamIDs = new Dictionary<string, string>();
                if (dict2.Contains("TargetAttributes"))
                {
                    PBXElementDict dict3 = dict2["TargetAttributes"].AsDict();
                    foreach (KeyValuePair<string, PBXElement> pair in dict3.values)
                    {
                        if (pair.Key == "DevelopmentTeam")
                        {
                            this.teamIDs.Add(pair.Key, pair.Value.AsString());
                        }
                        if (pair.Key == "SystemCapabilities")
                        {
                            PBXElementDict dict4 = dict2["SystemCapabilities"].AsDict();
                            foreach (KeyValuePair<string, PBXElement> pair2 in dict4.values)
                            {
                                this.capabilities.Add(new PBXCapabilityType.TargetCapabilityPair(pair.Key, PBXCapabilityType.StringToPBXCapabilityType(pair2.Value.AsString())));
                            }
                        }
                    }
                }
            }
        }

        internal override PropertyCommentChecker checker =>
            checkerData;

        public string mainGroup =>
            base.GetPropertyString("mainGroup");
    }
}

