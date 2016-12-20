namespace UnityEditor.iOS
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditor;

    public class Resource
    {
        internal string name;
        internal string path;
        internal List<string> tags;
        internal List<Variant> variants;

        public Resource(string name)
        {
            this.tags = new List<string>();
            this.name = name;
        }

        public Resource(string name, string path)
        {
            this.tags = new List<string>();
            this.name = name;
            this.path = path;
        }

        public Resource AddOnDemandResourceTags(params string[] newTags)
        {
            foreach (string str in newTags)
            {
                if (!this.tags.Contains(str))
                {
                    this.tags.Add(str);
                }
            }
            return this;
        }

        public Resource BindVariant(string path, string name)
        {
            if (this.path != null)
            {
                throw new Exception("Variants should not be added to non-variant resource");
            }
            if (this.variants == null)
            {
                this.variants = new List<Variant>();
            }
            Variant item = new Variant {
                path = path,
                variantName = name
            };
            this.variants.Add(item);
            return this;
        }

        public Resource BindVariant(string path, iOSDeviceRequirement requirement)
        {
            if (this.path != null)
            {
                throw new Exception("Variants should not be added to non-variant resource");
            }
            if (this.variants == null)
            {
                this.variants = new List<Variant>();
            }
            Variant item = new Variant {
                path = path,
                requirement = requirement
            };
            this.variants.Add(item);
            return this;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Variant
        {
            public string path;
            public iOSDeviceRequirement requirement;
            public string variantName;
        }
    }
}

