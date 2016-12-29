namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;

    internal abstract class AssetCatalogItem
    {
        public readonly string authorId;
        protected string m_Path;
        protected Dictionary<string, string> m_Properties = new Dictionary<string, string>();
        public readonly string name;

        public AssetCatalogItem(string name, string authorId)
        {
            if ((name != null) && name.Contains("/"))
            {
                throw new Exception("Asset catalog item must not have slashes in name");
            }
            this.name = name;
            this.authorId = authorId;
        }

        public abstract void Write(List<string> warnings);
        protected JsonElementDict WriteInfoToJson(JsonDocument doc)
        {
            JsonElementDict dict = doc.root.CreateDict("info");
            dict.SetInteger("version", 1);
            dict.SetString("author", this.authorId);
            return dict;
        }

        public string path =>
            this.m_Path;
    }
}

