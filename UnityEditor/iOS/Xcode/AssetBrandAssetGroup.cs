namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class AssetBrandAssetGroup : AssetCatalogItem
    {
        private List<AssetBrandAssetItem> m_Items;

        internal AssetBrandAssetGroup(string assetCatalogPath, string name, string authorId) : base(name, authorId)
        {
            this.m_Items = new List<AssetBrandAssetItem>();
            base.m_Path = Path.Combine(assetCatalogPath, name + ".brandassets");
        }

        private void AddItem(AssetCatalogItem item, string idiom, string role, int width, int height)
        {
            foreach (AssetBrandAssetItem item2 in this.m_Items)
            {
                if (item2.item.name == item.name)
                {
                    throw new Exception("An item with given name already exists");
                }
            }
            AssetBrandAssetItem item3 = new AssetBrandAssetItem {
                item = item,
                idiom = idiom,
                role = role,
                width = width,
                height = height
            };
            this.m_Items.Add(item3);
        }

        public AssetImageSet OpenImageSet(string name, string idiom, string role, int width, int height)
        {
            AssetImageSet item = new AssetImageSet(base.m_Path, name, base.authorId);
            this.AddItem(item, idiom, role, width, height);
            return item;
        }

        public AssetImageStack OpenImageStack(string name, string idiom, string role, int width, int height)
        {
            AssetImageStack item = new AssetImageStack(base.m_Path, name, base.authorId);
            this.AddItem(item, idiom, role, width, height);
            return item;
        }

        public override void Write(List<string> warnings)
        {
            Directory.CreateDirectory(base.m_Path);
            JsonDocument doc = new JsonDocument();
            base.WriteInfoToJson(doc);
            JsonElementArray array = doc.root.CreateArray("assets");
            foreach (AssetBrandAssetItem item in this.m_Items)
            {
                JsonElementDict dict = array.AddDict();
                dict.SetString("size", $"{item.width}x{item.height}");
                dict.SetString("idiom", item.idiom);
                dict.SetString("role", item.role);
                dict.SetString("filename", Path.GetFileName(item.item.path));
                item.item.Write(warnings);
            }
            doc.WriteToFile(Path.Combine(base.m_Path, "Contents.json"));
        }

        private class AssetBrandAssetItem
        {
            internal int height;
            internal string idiom = null;
            internal AssetCatalogItem item = null;
            internal string role = null;
            internal int width;
        }
    }
}

