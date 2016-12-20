namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class AssetDataSet : AssetCatalogItemWithVariants
    {
        internal AssetDataSet(string parentPath, string name, string authorId) : base(name, authorId)
        {
            base.m_Path = Path.Combine(parentPath, name + ".dataset");
        }

        public void AddVariant(DeviceRequirement requirement, string path, string typeIdentifier)
        {
            foreach (DataSetVariant variant in base.m_Variants)
            {
                if (((variant.id != null) && (typeIdentifier != null)) && (variant.id == typeIdentifier))
                {
                    throw new Exception("Two items within the same dataset must not have the same id");
                }
            }
            base.AddVariant(new DataSetVariant(requirement, path, typeIdentifier));
        }

        public override void Write(List<string> warnings)
        {
            Directory.CreateDirectory(base.m_Path);
            JsonDocument doc = new JsonDocument();
            JsonElementDict info = base.WriteInfoToJson(doc);
            base.WriteODRTagsToJson(info);
            JsonElementArray array = doc.root.CreateArray("data");
            foreach (DataSetVariant variant in base.m_Variants)
            {
                string fileName = Path.GetFileName(variant.path);
                if (!File.Exists(variant.path))
                {
                    if (warnings != null)
                    {
                        warnings.Add("File not found: " + variant.path);
                    }
                }
                else
                {
                    File.Copy(variant.path, Path.Combine(base.m_Path, fileName));
                }
                JsonElementDict item = array.AddDict();
                item.SetString("filename", fileName);
                base.WriteRequirementsToJson(item, variant.requirement);
                if (variant.id != null)
                {
                    item.SetString("universal-type-identifier", variant.id);
                }
            }
            doc.WriteToFile(Path.Combine(base.m_Path, "Contents.json"));
        }

        private class DataSetVariant : AssetCatalogItemWithVariants.VariantData
        {
            public string id;

            public DataSetVariant(DeviceRequirement requirement, string path, string id) : base(requirement, path)
            {
                this.id = id;
            }
        }
    }
}

