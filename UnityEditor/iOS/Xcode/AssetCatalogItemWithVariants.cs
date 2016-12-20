namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal abstract class AssetCatalogItemWithVariants : AssetCatalogItem
    {
        protected List<string> m_ODRTags;
        protected List<VariantData> m_Variants;

        protected AssetCatalogItemWithVariants(string name, string authorId) : base(name, authorId)
        {
            this.m_Variants = new List<VariantData>();
            this.m_ODRTags = new List<string>();
        }

        public void AddOnDemandResourceTag(string tag)
        {
            if (!this.m_ODRTags.Contains(tag))
            {
                this.m_ODRTags.Add(tag);
            }
        }

        protected void AddVariant(VariantData newItem)
        {
            foreach (VariantData data in this.m_Variants)
            {
                if (data.requirement.values == newItem.requirement.values)
                {
                    throw new Exception("The given requirement has been already added");
                }
                if (Path.GetFileName(data.path) == Path.GetFileName(base.path))
                {
                    throw new Exception("Two items within the same set must not have the same file name");
                }
            }
            if (Path.GetFileName(newItem.path) == "Contents.json")
            {
                throw new Exception("The file name must not be equal to Contents.json");
            }
            this.m_Variants.Add(newItem);
        }

        public bool HasVariant(DeviceRequirement requirement)
        {
            foreach (VariantData data in this.m_Variants)
            {
                if (data.requirement.values == requirement.values)
                {
                    return true;
                }
            }
            return false;
        }

        protected void WriteODRTagsToJson(JsonElementDict info)
        {
            if (this.m_ODRTags.Count > 0)
            {
                JsonElementArray array = info.CreateArray("on-demand-resource-tags");
                foreach (string str in this.m_ODRTags)
                {
                    array.AddString(str);
                }
            }
        }

        protected void WriteRequirementsToJson(JsonElementDict item, DeviceRequirement req)
        {
            foreach (KeyValuePair<string, string> pair in req.values)
            {
                if ((pair.Value != null) && (pair.Value != ""))
                {
                    item.SetString(pair.Key, pair.Value);
                }
            }
        }

        protected class VariantData
        {
            public string path;
            public DeviceRequirement requirement;

            public VariantData(DeviceRequirement requirement, string path)
            {
                this.requirement = requirement;
                this.path = path;
            }
        }
    }
}

