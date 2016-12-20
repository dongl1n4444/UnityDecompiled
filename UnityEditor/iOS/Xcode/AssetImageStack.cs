namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class AssetImageStack : AssetCatalogItem
    {
        private List<AssetImageStackLayer> m_Layers;

        internal AssetImageStack(string assetCatalogPath, string name, string authorId) : base(name, authorId)
        {
            this.m_Layers = new List<AssetImageStackLayer>();
            base.m_Path = Path.Combine(assetCatalogPath, name + ".imagestack");
        }

        public AssetImageStackLayer AddLayer(string name)
        {
            foreach (AssetImageStackLayer layer in this.m_Layers)
            {
                if (layer.name == name)
                {
                    throw new Exception("A layer with given name already exists");
                }
            }
            AssetImageStackLayer item = new AssetImageStackLayer(base.m_Path, name, base.authorId);
            this.m_Layers.Add(item);
            return item;
        }

        public override void Write(List<string> warnings)
        {
            Directory.CreateDirectory(base.m_Path);
            JsonDocument doc = new JsonDocument();
            base.WriteInfoToJson(doc);
            JsonElementArray array = doc.root.CreateArray("layers");
            foreach (AssetImageStackLayer layer in this.m_Layers)
            {
                layer.Write(warnings);
                array.AddDict().SetString("filename", Path.GetFileName(layer.path));
            }
            doc.WriteToFile(Path.Combine(base.m_Path, "Contents.json"));
        }
    }
}

