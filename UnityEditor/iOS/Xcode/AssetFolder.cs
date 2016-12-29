namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class AssetFolder : AssetCatalogItem
    {
        private List<AssetCatalogItem> m_Items;
        private bool m_ProvidesNamespace;

        internal AssetFolder(string parentPath, string name, string authorId) : base(name, authorId)
        {
            this.m_Items = new List<AssetCatalogItem>();
            this.m_ProvidesNamespace = false;
            if (name != null)
            {
                base.m_Path = Path.Combine(parentPath, name);
            }
            else
            {
                base.m_Path = parentPath;
            }
        }

        public AssetCatalogItem GetChild(string name)
        {
            foreach (AssetCatalogItem item in this.m_Items)
            {
                if (item.name == name)
                {
                    return item;
                }
            }
            return null;
        }

        private T GetExistingItemWithType<T>(string name) where T: class
        {
            AssetCatalogItem child = this.GetChild(name);
            if (child != null)
            {
                if (!(child is T))
                {
                    throw new Exception("The given path is already occupied with an asset");
                }
                return (child as T);
            }
            return null;
        }

        public AssetBrandAssetGroup OpenBrandAssetGroup(string name)
        {
            AssetBrandAssetGroup existingItemWithType = this.GetExistingItemWithType<AssetBrandAssetGroup>(name);
            if (existingItemWithType != null)
            {
                return existingItemWithType;
            }
            AssetBrandAssetGroup item = new AssetBrandAssetGroup(base.m_Path, name, base.authorId);
            this.m_Items.Add(item);
            return item;
        }

        public AssetDataSet OpenDataSet(string name)
        {
            AssetDataSet existingItemWithType = this.GetExistingItemWithType<AssetDataSet>(name);
            if (existingItemWithType != null)
            {
                return existingItemWithType;
            }
            AssetDataSet item = new AssetDataSet(base.m_Path, name, base.authorId);
            this.m_Items.Add(item);
            return item;
        }

        public AssetFolder OpenFolder(string name)
        {
            AssetCatalogItem child = this.GetChild(name);
            if (child != null)
            {
                if (!(child is AssetFolder))
                {
                    throw new Exception("The given path is already occupied with an asset");
                }
                return (child as AssetFolder);
            }
            AssetFolder item = new AssetFolder(base.m_Path, name, base.authorId);
            this.m_Items.Add(item);
            return item;
        }

        public AssetImageSet OpenImageSet(string name)
        {
            AssetImageSet existingItemWithType = this.GetExistingItemWithType<AssetImageSet>(name);
            if (existingItemWithType != null)
            {
                return existingItemWithType;
            }
            AssetImageSet item = new AssetImageSet(base.m_Path, name, base.authorId);
            this.m_Items.Add(item);
            return item;
        }

        public AssetImageStack OpenImageStack(string name)
        {
            AssetImageStack existingItemWithType = this.GetExistingItemWithType<AssetImageStack>(name);
            if (existingItemWithType != null)
            {
                return existingItemWithType;
            }
            AssetImageStack item = new AssetImageStack(base.m_Path, name, base.authorId);
            this.m_Items.Add(item);
            return item;
        }

        public override void Write(List<string> warnings)
        {
            if (Directory.Exists(base.m_Path))
            {
                Directory.Delete(base.m_Path, true);
            }
            Directory.CreateDirectory(base.m_Path);
            this.WriteJson();
            foreach (AssetCatalogItem item in this.m_Items)
            {
                item.Write(warnings);
            }
        }

        private void WriteJson()
        {
            if (this.providesNamespace)
            {
                JsonDocument doc = new JsonDocument();
                base.WriteInfoToJson(doc);
                doc.root.CreateDict("properties").SetBoolean("provides-namespace", this.providesNamespace);
                doc.WriteToFile(Path.Combine(base.m_Path, "Contents.json"));
            }
        }

        public bool providesNamespace
        {
            get => 
                this.m_ProvidesNamespace;
            set
            {
                if ((this.m_Items.Count > 0) && (value != this.m_ProvidesNamespace))
                {
                    throw new Exception("Asset folder namespace providing status can't be changed after items have been added");
                }
                this.m_ProvidesNamespace = value;
            }
        }
    }
}

