namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class AssetImageStackLayer : AssetCatalogItem
    {
        private AssetImageSet m_Imageset;
        private string m_ReferencedName;

        internal AssetImageStackLayer(string assetCatalogPath, string name, string authorId) : base(name, authorId)
        {
            this.m_Imageset = null;
            this.m_ReferencedName = null;
            base.m_Path = Path.Combine(assetCatalogPath, name + ".imagestacklayer");
            this.m_Imageset = new AssetImageSet(base.m_Path, "Content", authorId);
        }

        public AssetImageSet GetImageSet()
        {
            return this.m_Imageset;
        }

        public string ReferencedName()
        {
            return this.m_ReferencedName;
        }

        public void SetReference(string name)
        {
            this.m_Imageset = null;
            this.m_ReferencedName = name;
        }

        public override void Write(List<string> warnings)
        {
            Directory.CreateDirectory(base.m_Path);
            JsonDocument doc = new JsonDocument();
            base.WriteInfoToJson(doc);
            if (this.m_ReferencedName != null)
            {
                JsonElementDict dict2 = doc.root.CreateDict("properties").CreateDict("content-reference");
                dict2.SetString("type", "image-set");
                dict2.SetString("name", this.m_ReferencedName);
                dict2.SetString("matching-style", "fully-qualified-name");
            }
            if (this.m_Imageset != null)
            {
                this.m_Imageset.Write(warnings);
            }
            doc.WriteToFile(Path.Combine(base.m_Path, "Contents.json"));
        }
    }
}

