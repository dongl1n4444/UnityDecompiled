namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;

    internal class AssetStoreSearchResults : AssetStoreResultBase<AssetStoreSearchResults>
    {
        internal List<Group> groups;

        public AssetStoreSearchResults(AssetStoreResultBase<AssetStoreSearchResults>.Callback c) : base(c)
        {
            this.groups = new List<Group>();
        }

        protected override void Parse(Dictionary<string, JSONValue> dict)
        {
            JSONValue value3 = dict["groups"];
            foreach (JSONValue value2 in value3.AsList(true))
            {
                Group group = Group.Create();
                this.ParseList(value2, ref group);
                this.groups.Add(group);
            }
            JSONValue value5 = dict["query"];
            JSONValue value4 = value5["offsets"];
            JSONValue value6 = dict["query"];
            List<JSONValue> list = value6["limits"].AsList(true);
            int num = 0;
            foreach (JSONValue value8 in value4.AsList(true))
            {
                Group group2 = this.groups[num];
                group2.offset = (int) value8.AsFloat(true);
                JSONValue value9 = list[num];
                group2.limit = (int) value9.AsFloat(true);
                this.groups[num] = group2;
                num++;
            }
        }

        private void ParseList(JSONValue matches, ref Group group)
        {
            List<AssetStoreAsset> assets = group.assets;
            if (matches.ContainsKey("error"))
            {
                base.error = matches["error"].AsString(true);
            }
            if (matches.ContainsKey("warnings"))
            {
                base.warnings = matches["warnings"].AsString(true);
            }
            if (matches.ContainsKey("name"))
            {
                group.name = matches["name"].AsString(true);
            }
            if (matches.ContainsKey("label"))
            {
                group.label = matches["label"].AsString(true);
            }
            if (group.label == null)
            {
            }
            group.label = group.name;
            if (matches.ContainsKey("total_found"))
            {
                JSONValue value6 = matches["total_found"];
                group.totalFound = (int) value6.AsFloat(true);
            }
            if (matches.ContainsKey("matches"))
            {
                JSONValue value8 = matches["matches"];
                foreach (JSONValue value7 in value8.AsList(true))
                {
                    AssetStoreAsset item = new AssetStoreAsset();
                    if ((value7.ContainsKey("id") && value7.ContainsKey("name")) && value7.ContainsKey("package_id"))
                    {
                        JSONValue value9 = value7["id"];
                        item.id = (int) value9.AsFloat();
                        item.name = value7["name"].AsString();
                        item.displayName = StripExtension(item.name);
                        JSONValue value11 = value7["package_id"];
                        item.packageID = (int) value11.AsFloat();
                        if (value7.ContainsKey("static_preview_url"))
                        {
                            item.staticPreviewURL = value7["static_preview_url"].AsString();
                        }
                        if (value7.ContainsKey("dynamic_preview_url"))
                        {
                            item.dynamicPreviewURL = value7["dynamic_preview_url"].AsString();
                        }
                        item.className = !value7.ContainsKey("class_name") ? "" : value7["class_name"].AsString();
                        if (value7.ContainsKey("price"))
                        {
                            item.price = value7["price"].AsString();
                        }
                        assets.Add(item);
                    }
                }
            }
        }

        private static string StripExtension(string path)
        {
            if (path == null)
            {
                return null;
            }
            int length = path.LastIndexOf(".");
            return ((length >= 0) ? path.Substring(0, length) : path);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Group
        {
            public List<AssetStoreAsset> assets;
            public int totalFound;
            public string label;
            public string name;
            public int offset;
            public int limit;
            public static AssetStoreSearchResults.Group Create()
            {
                return new AssetStoreSearchResults.Group { 
                    assets = new List<AssetStoreAsset>(),
                    label = "",
                    name = "",
                    offset = 0,
                    limit = -1
                };
            }
        }
    }
}

